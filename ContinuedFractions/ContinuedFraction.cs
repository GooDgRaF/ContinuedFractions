using System.Collections;
using System.Diagnostics;
using System.Text;


namespace ContinuedFractions;

/// <summary>
/// [not?] Lazy continued fractions of the maximal BigInteger.MaxValue length [ :( ]
/// </summary>
public partial class CFraction : IEnumerable<BigInteger> {

  private const int numberOfCoeffs = 40; // 40 хватает для точности double

#region Data and Constructors
  private          IEnumerator<BigInteger>? _cfEnumerator; // "Правило" для получения следующего коэффициента бесконечной дроби
  private readonly List<BigInteger>         _cfCashed;     // Конечные непрерывные дроби лежат тут целиком

  private CFraction(List<BigInteger> cf) {
    _cfCashed     = cf;
    _cfEnumerator = null;
  }

  private CFraction(IEnumerable<BigInteger> cf) {
    _cfEnumerator = cf.GetEnumerator();
    _cfCashed     = new List<BigInteger>();

    if (_cfEnumerator.MoveNext()) {
      _cfCashed.Add(_cfEnumerator.Current);
      if (!_cfEnumerator.MoveNext()) { // обработка случая [a0]
        _cfEnumerator.Dispose();
        _cfEnumerator = null;
      }
      else { // если был ещё элемент, то добавим его [a0;a1] так как итератор сдвинули успешно
        _cfCashed.Add(_cfEnumerator.Current);
      }
    }
    else { // имеем дело с [], то есть с бесконечностью
      _cfEnumerator.Dispose();
      _cfEnumerator = null;
    }
  }
#endregion

#region Indexer
  public BigInteger? this[int i] {
    get
      {
        Debug.Assert(i >= 0, $"Index should be non negative. Found: i = {i}");
        if (!CacheUpToIndex(i)) {
          return null;
        }

        return _cfCashed[i];
      }
  }

  private bool CacheUpToIndex(BigInteger i) {
    while (_cfCashed.Count <= i + 2 && _cfEnumerator is not null) {
      if (!_cfEnumerator.MoveNext()) {
        _cfEnumerator.Dispose();
        _cfEnumerator = null;

        if (_cfCashed[^1] == 1) { // приводим число к каноническому виду, то есть, когда последний коэффициент отличен от единицы
          _cfCashed.RemoveAt(_cfCashed.Count - 1);
          _cfCashed[^1]++;
        }
      }
      else {
        _cfCashed.Add(_cfEnumerator.Current); // Добавляем следующий элемент в кэш
      }
    }


    return _cfCashed.Count > i;
  }
#endregion

#region Factories
  public static CFraction FromRational(BigInteger numerator, BigInteger denominator) {
    if (numerator == 0 && denominator == 0) {
      throw new ArgumentException("Can't handle the 0/0 fraction!");
    }

    return new CFraction(RationalGenerator(numerator, denominator));
  }

  public static CFraction FromCoeffs(IList<int> cf) => FromCoeffs(cf.Select(v => (BigInteger)v).ToArray());

  public static CFraction FromCoeffs(IList<BigInteger> cf) {
    List<BigInteger> r = cf.ToList();
    if (r.Skip(1).Any(c => c < 1)) {
      throw new ArgumentException
        ("There should be all coefficients of the continued function (after the first one) greater than zero!");
    }

    if (r.Count > 1) {
      if (r[^1] == 1) {
        r.RemoveAt(r.Count - 1);
        r[^1]++;
      }
    }

    return new CFraction(r);
  }

  public static CFraction FromGenerator(IEnumerable<BigInteger> cf) => new CFraction(cf);
  public static CFraction FromGenerator(IEnumerable<int>        cf) => new CFraction(cf.Select(v => (BigInteger)v));
#endregion

#region Overrides
  public override string ToString() {
    StringBuilder res = new StringBuilder("[");

    var elementsToString = this.Take(numberOfCoeffs + 1).ToList();

    if (elementsToString.Count == 0) {
      return res.Append(']').ToString();
    }

    res.Append(elementsToString[0]);

    if (elementsToString.Count == 1) {
      res.Append(']');

      return res.ToString();
    }

    res.Append(';'); // Добавляем точку с запятой после первого члена

    var remainingElements = elementsToString.Skip(1).Take(numberOfCoeffs);

    bool isFirst = true;
    foreach (var coefficient in remainingElements) {
      if (isFirst) {
        isFirst = false;
      }
      else {
        res.Append(',');
      }
      res.Append(coefficient);
    }

    // Если у нас 41-ый элемент существует, значит последовательность может быть длиннее
    if (elementsToString.Count > numberOfCoeffs) {
      res.Append(",...");
    }

    res.Append(']');

    return res.ToString();
  }
#endregion

#region Static
  private static IEnumerable<BigInteger> RationalGenerator(BigInteger numerator, BigInteger denominator) {
    while (denominator != 0) {
      BigInteger quotient  = GosperMatrix.FloorDiv(numerator, denominator);
      BigInteger remainder = numerator - quotient * denominator;

      yield return quotient;

      numerator   = denominator;
      denominator = remainder;
    }
  }
#endregion

#region IEnumerable<BigInteger> implementation
  private class CFEnumerator : IEnumerator<BigInteger> {

    private readonly CFraction _cf;

    private int _index = -1; // Перед первым элементом

    public CFEnumerator(CFraction cf) => _cf = cf;

    public bool MoveNext() {
      _index++;

      return _cf.CacheUpToIndex(_index);
    }

    public BigInteger Current => _cf._cfCashed[_index];

    object IEnumerator.Current => Current;

    public void Reset() { _index = -1; }

    public void Dispose() { }

  }

  public IEnumerator<BigInteger> GetEnumerator() { return new CFEnumerator(this); }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
#endregion

}
