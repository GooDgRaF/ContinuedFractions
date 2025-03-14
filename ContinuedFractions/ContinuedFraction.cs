using System.Collections;
using System.Numerics;
using System.Text;

namespace ContinuedFractions;

/// <summary>
/// Lazy continued fractions of arbitrary lenght
/// </summary>
public readonly partial struct ContinuedFraction {

  private readonly IEnumerable<int> _kConvergent;

  private ContinuedFraction(IEnumerable<int> cf) { _kConvergent = cf; }

  public IEnumerable<(BigInteger numerator, BigInteger denominator)> FromCF() => FromCF(_kConvergent);

  public IEnumerable<Matrix22> kConvergentMatrix() {
    Matrix22 res = Matrix22.Id();

    foreach (int kC in _kConvergent) {
      res *= Matrix22.Homographic(kC);
      yield return res;
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
    var elementsToString = _kConvergent.Take(41).ToList();

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
  private static IEnumerable<(BigInteger numerator, BigInteger denominator)> FromCF(IEnumerable<int> kConvergent) {
    BigInteger p1 = 1;
    BigInteger p0 = 0;
    BigInteger q1 = 0;
    BigInteger q0 = 1;

    foreach (int coeff in kConvergent) {
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
#endregion

}
