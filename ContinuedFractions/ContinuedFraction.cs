using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Diagnostics.CodeAnalysis;


namespace ContinuedFractions;

/// <summary>
/// Lazy continued fractions of the maximal int.MaxValue length [ :( ]
/// </summary>
public readonly partial struct ContinuedFraction {

  private readonly IEnumerator<int> _cfEnumerator;

  private readonly List<int> _cfCashed = new List<int>();

  private bool CacheUpToIndex(int i) {
    while (_cfCashed.Count <= i) {
      if (!_cfEnumerator.MoveNext()) {
        return false;
      }
      _cfCashed.Add(_cfEnumerator.Current); // Добавляем следующий элемент в кэш
    }

    return true;
  }

  public int this[int i] { // -1 означает, что непрерывная дробь закончилась
    get
      {
        Debug.Assert(i >= 0, $"Index should be non negative. Found: i = {i}");

        if (_cfCashed.Count > i) {
          return _cfCashed[i];
        }

        return CacheUpToIndex(i) ? _cfCashed[i] : -1;
      }
  }

  public List<int> Take(int n) {
    if (_cfCashed.Count > n) {
      return _cfCashed[..n];
    }

    return CacheUpToIndex(n - 1) ? _cfCashed[..n] : _cfCashed[.._cfCashed.Count];
  }


  public ContinuedFraction(IEnumerable<int> cf) {
    _cfEnumerator = cf.GetEnumerator();
    // _cfCashed.Capacity = 128;
  }

  public IEnumerable<(BigInteger numerator, BigInteger denominator)> FromCF() => FromCF(_cfCashed);

  private static Matrix22 LFT_step(Matrix22 m, int a) => m * Matrix22.Homographic(a);

  public ContinuedFraction CF_transform(Matrix22 init) => new ContinuedFraction(CF_transform_main(init));

  public IEnumerable<int> CF_transform_main(Matrix22 init) {
    Matrix22 m = init;
    int      i = 0;

    while (true) {
      int val = this[i];
      if (val == -1) {
        break;
      }
      m = LFT_step(m, val);
      foreach ((int q, Matrix22 r) gcd in GCD(m)) {
        m = gcd.r;

        yield return gcd.q;
      }

      i++;
    }

    // Если исходная непрерывная дробь была конечной, то надо выполнить ещё раз алгоритм Евклида.
    if (m[2] != 0) {
      foreach (int k in GCD(m[0], m[2]))
        yield return k;
    }
  }

#region Factories
  public static ContinuedFraction FromRational(BigInteger numerator, BigInteger denominator)
    => new(RationalGenerator(numerator, denominator));
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
  private static IEnumerable<(BigInteger numerator, BigInteger denominator)> FromCF(IEnumerable<int> cf) {
    BigInteger p1 = 1;
    BigInteger p0 = 0;
    BigInteger q1 = 0;
    BigInteger q0 = 1;

    foreach (int coeff in cf) {
      BigInteger p = coeff * p1 + p0;
      BigInteger q = coeff * q1 + q0;

      yield return (p, q);

      p0 = p1;
      p1 = p;
      q0 = q1;
      q1 = q;
    }
  }

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

  private static bool GCD_step(Matrix22 m, [NotNullWhen(true)] out int? q, [NotNullWhen(true)] out Matrix22? r) {
    if (m[2] == 0 && m[3] == 0) {
      throw new ArgumentException($"GCD_step: That series has already ended. Found: the second row of matrix is zero.");
    }

    q = null;
    r = null;

    if (m[2] == 0 || m[3] == 0) { // Функция неограниченна, недостаточно информации для определения частного
      return false;
    }

    if (m[2] != 0 && m[3] < 0) { // Есть ноль в области определения
      return false;
    }

    // Функция ограничена
    double n0 = (double)m[0] / m[2];
    double n1 = (double)m[1] / m[3];

    double v0 = Math.Min(n0, n1);
    double v1 = Math.Max(n0, n1);

    int d0 = (int)Math.Floor(v0);
    int d1 = (int)Math.Floor(v1);

    // если d1 == d0, то частное определяется однозначно,
    // если d1 отличается больше чем на 1 от d0, то ничего определённого сказать нельзя
    // если d1 == d0 + 1 и d1 == v1, то эта граница достигается только либо в 0, либо в oo; поэтому ответ d0.
    if (d1 == d0 || (d1 == d0 + 1 && Math.Abs(d1 - v1) < 1e-14)) {
      q = d0;
      r = new Matrix22(m[2], m[3], m[0] - d0 * m[2], m[1] - d0 * m[3]);

      return true;
    }

    return false;
  }

  private static IEnumerable<int> GCD(int a, int b) {
    while (b != 0) {
      int q = Math.DivRem(a, b, out int r);

      yield return q;

      a = b;
      b = r;
    }
  }

  public static IEnumerable<(int q, Matrix22 r)> GCD(Matrix22 m) {
    while (true) {
      if (m[0] == 0 && m[1] == 0) { // числитель равен нулю
        break;
      }

      if (!GCD_step(m, out int? q, out Matrix22? r)) {
        break;
      }

      yield return ((int)q, (Matrix22)r);

      m = (Matrix22)r;
    }
  }
#endregion

}
