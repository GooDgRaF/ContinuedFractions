namespace ContinuedFractions;

public partial class CFraction : IAdditiveIdentity<CFraction, CFraction>, IMultiplicativeIdentity<CFraction, CFraction> {

  public static CFraction AdditiveIdentity       => Zero;
  public static CFraction MultiplicativeIdentity => One;

  public static readonly CFraction E        = new CFraction(EGenerator());
  public static readonly CFraction Sqrt2    = new CFraction(Sqrt2Generator());
  public static readonly CFraction Infinity = new CFraction(Array.Empty<BigInteger>());
  public static readonly CFraction Zero     = new CFraction(new BigInteger[] { 0 });
  public static readonly CFraction One      = new CFraction(new BigInteger[] { 1 });

  public static IEnumerable<BigInteger> EGenerator() {
    yield return 2;

    BigInteger k = 0;
    while (true) {
      yield return 1;
      yield return 2 * k + 2;
      yield return 1;

      k++;
    }
  }


  public static IEnumerable<BigInteger> Sqrt2Generator() {
    yield return 1;

    while (true) {
      yield return 2;
    }
  }

}
