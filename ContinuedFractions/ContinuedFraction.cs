using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Diagnostics.CodeAnalysis;


namespace ContinuedFractions;

/// <summary>
/// [not?] Lazy continued fractions of the maximal int.MaxValue length [ :( ]
/// </summary>
public partial class CFraction : IEnumerable<int> {

#region Data and Constructors
  private          IEnumerator<int>? _cfEnumerator; // "Правило" для получения следующего коэффициента бесконечной дроби
  private readonly List<int>         _cfCashed;     // Конечные непрерывные дроби лежат тут целиком

  private CFraction(List<int> cf) {
    _cfCashed     = cf;
    _cfEnumerator = null;
  }

  private CFraction(IEnumerable<int> cf) {
    _cfEnumerator = cf.GetEnumerator();
    _cfCashed     = new List<int>();

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
  public int? this[int i] {
    get
      {
        Debug.Assert(i >= 0, $"Index should be non negative. Found: i = {i}");
        if (!CacheUpToIndex(i)) {
          return null;
        }

        return _cfCashed[i];
      }
  }

  private bool CacheUpToIndex(int i) {
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

    int sign = numerator.Sign * denominator.Sign;

    numerator   = BigInteger.Abs(numerator);
    denominator = BigInteger.Abs(denominator);

    return sign switch
             {
               < 0 => new CFraction(RationalGenerator(-numerator, denominator)).IdTransform()
             , _   => new CFraction(RationalGenerator(numerator, denominator))
             };
  }

  public static CFraction FromCoeffs(IList<int> cf) {
    List<int> r = cf.ToList();
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

  public static CFraction FromGenerator(IEnumerable<int> cf) => new CFraction(cf);
#endregion

#region Overrides
  public override string ToString() {
    StringBuilder res = new StringBuilder("[");

    // Берем только до 40 элементов для отображения: чтобы по абсолютной точности соответствовать типу double
    var elementsToString = this.Take(41).ToList();

    if (elementsToString.Count == 0) {
      return res.Append(']').ToString();
    }

    res.Append(elementsToString[0]);

    if (elementsToString.Count == 1) {
      res.Append(']');

      return res.ToString();
    }

    res.Append(';'); // Добавляем точку с запятой после первого члена

    var remainingElements = elementsToString.Skip(1).Take(39);

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
    if (elementsToString.Count > 40) {
      res.Append(",...");
    }

    res.Append(']');

    return res.ToString();
  }
#endregion

#region Static
  private static IEnumerable<int> RationalGenerator(BigInteger numerator, BigInteger denominator) {
    while (denominator != 0) {
      BigInteger quotient = BigInteger.DivRem(numerator, denominator, out BigInteger remainder);
      if (quotient < int.MinValue || quotient > int.MaxValue) {
        throw new OverflowException("Coefficient is out of range for int.");
      }

      yield return (int)quotient;

      numerator   = denominator;
      denominator = remainder;
    }
  }
#endregion

#region IEnumerable<int> implementation
  private class CFEnumerator : IEnumerator<int> {

    private readonly CFraction _cf;

    private int _index = -1; // Перед первым элементом

    public CFEnumerator(CFraction cf) => _cf = cf;

    public bool MoveNext() {
      _index++;

      return _cf.CacheUpToIndex(_index);
    }

    public int Current => _cf._cfCashed[_index];

    object IEnumerator.Current => Current;

    public void Reset() { _index = -1; }

    public void Dispose() { }

  }

  public IEnumerator<int> GetEnumerator() { return new CFEnumerator(this); }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
#endregion

}
