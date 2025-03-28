using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Diagnostics.CodeAnalysis;


namespace ContinuedFractions;

public partial class CFraction {

  public IEnumerable<(BigInteger numerator, BigInteger denominator)> ToRational() => ToRational(_cfCashed);

  private static IEnumerable<(BigInteger numerator, BigInteger denominator)> ToRational(IEnumerable<int> cf) {
    decimal p1 = 1;
    decimal p0 = 0;
    decimal q1 = 0;
    decimal q0 = 1;

    foreach (int coeff in cf) {
      decimal p = coeff * p1 + p0;
      decimal q = coeff * q1 + q0;

      yield return ((BigInteger)p, (BigInteger)q);

      p0 = p1;
      p1 = p;
      q0 = q1;
      q1 = q;
    }
  }

  public static explicit operator double(CFraction cf) {
    var d = cf.Take(40).ToList();
    if (d.Count == 0) {
      return double.PositiveInfinity;
    }

    (var a, var b) = ToRational(d).Last();

    return (double)((decimal)a / (decimal)b);
  }

}
