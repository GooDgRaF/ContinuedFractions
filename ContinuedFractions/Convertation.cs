using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Diagnostics.CodeAnalysis;


namespace ContinuedFractions;

public partial struct CFraction
{
  public IEnumerable<(BigInteger numerator, BigInteger denominator)> ToRational() => ToRational(_cfCashed);

      private static IEnumerable<(BigInteger numerator, BigInteger denominator)> ToRational(IEnumerable<int> cf) {
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
}