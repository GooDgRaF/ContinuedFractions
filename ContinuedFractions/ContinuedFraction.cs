using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Diagnostics.CodeAnalysis;


namespace ContinuedFractions;

/// <summary>
/// Lazy continued fractions of the maximal int.MaxValue length [ :( ]
/// </summary>
public readonly partial struct CFraction {

#region Data and Constructors
  private readonly IEnumerable<int> _cfEnumerable; // "Генератор" правил
  private readonly IEnumerator<int> _cfEnumerator; // "Правило" для получения следующего коэффициента бесконечной дроби
  private readonly List<int>        _cfCashed;     // Конечные непрерывные дроби лежат тут целиком


  private bool CacheUpToIndex(int i) {
    while (_cfCashed.Count <= i) {
      if (!_cfEnumerator.MoveNext()) {
        return false;
      }
      _cfCashed.Add(_cfEnumerator.Current); // Добавляем следующий элемент в кэш
    }

    return true;
  }

  private CFraction(List<int> cf) {
    _cfCashed     = cf;
    _cfEnumerable = new List<int>();
    _cfEnumerator = _cfEnumerable.GetEnumerator();
  }

  private CFraction(IEnumerable<int> cf) {
    _cfEnumerable = cf;
    _cfEnumerator = cf.GetEnumerator();
    _cfCashed     = new List<int>();
  }
#endregion

#region Properties
  // public bool IsFinite { get; private set; }; // у readonly структур не может быть set-полей.
#endregion

#region TakeRegion
  public int? this[int i] {
    get
      {
        Debug.Assert(i >= 0, $"Index should be non negative. Found: i = {i}");

        if (_cfCashed.Count > i) {
          return _cfCashed[i];
        }

        if (CacheUpToIndex(i)) {
          return _cfCashed[i];
        }
        if (_cfCashed.Count == i || _cfCashed.Count == 0) {
          return null;
        }

        throw new IndexOutOfRangeException
          ($"The given index i = {i} is out of boundaries of the cf. It's length is {_cfCashed.Count}");
      }
  }

  public List<int> Take(int n) {
    if (_cfCashed.Count >= n) {
      return _cfCashed[..n];
    }

    return CacheUpToIndex(n - 1) ? _cfCashed[..n] : _cfCashed[.._cfCashed.Count];
  }
#endregion

#region Factories
  public static CFraction FromRational(BigInteger numerator, BigInteger denominator)
    => new(RationalGenerator(numerator, denominator));

  public static CFraction FromCoeffs(IList<int> cf) {
    List<int> r = cf.ToList();
    if (r.Skip(1).Any(c => c == 0)) {
      throw new ArgumentException("There should be no zeros among coefficients (after the first one) of the continued function!");
    }

    return new CFraction(cf);
  }

  public static CFraction FromGenerator(IEnumerable<int> cf) => new CFraction(cf);
#endregion

#region Overrides
  public override string ToString() {
    StringBuilder res = new StringBuilder("[");

    // Берем только до 40 элементов для отображения: чтобы по абсолютной точности соответствовать типу double
    var elementsToString = Take(41);

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

}
