namespace ContinuedFractions;

/// <summary>
/// Represents a continued fraction using <see cref="BigInteger"/> coefficients.
/// This implementation features lazy evaluation and caching of computed coefficients.
/// Core arithmetic operations aim for exact results using BigInteger. However, comparisons and representations
/// are effectively limited by a fixed number of coefficients (defaulting to a quantity sufficient for double precision).
/// </summary>
public partial class CFraction : IAdditiveIdentity<CFraction, CFraction>, IMultiplicativeIdentity<CFraction, CFraction> {

  /// <summary>
  /// Gets the additive identity (zero) for continued fractions.
  /// </summary>
  public static CFraction AdditiveIdentity => Zero;

  /// <summary>
  /// Gets the multiplicative identity (one) for continued fractions.
  /// </summary>
  public static CFraction MultiplicativeIdentity => One;

  /// <summary>
  /// Represents the mathematical constant e ([2; 1, 2, 1, 1, 4, 1, 1, 6, ...]) as a continued fraction.
  /// </summary>
  public static readonly CFraction E = new CFraction(EGenerator());

  /// <summary>
  /// Represents the square root of 2 ([1; 2, 2, 2, ...]) as a continued fraction.
  /// </summary>
  public static readonly CFraction Sqrt2 = new CFraction(Sqrt2Generator());

  /// <summary>
  /// Represents positive infinity ([]) as a continued fraction.
  /// </summary>
  public static readonly CFraction Infinity = new CFraction(Array.Empty<BigInteger>());

  /// <summary>
  /// Represents zero ([0]) as a continued fraction.
  /// </summary>
  public static readonly CFraction Zero = new CFraction(new BigInteger[] { 0 });

  /// <summary>
  /// Represents one ([1]) as a continued fraction.
  /// </summary>
  public static readonly CFraction One = new CFraction(new BigInteger[] { 1 });

  /// <summary>
  /// Generates the coefficients for the continued fraction representation of e.
  /// </summary>
  /// <returns>An infinite sequence of coefficients for e: 2, 1, 2, 1, 1, 4, 1, 1, 6, ...</returns>
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


  /// <summary>
  /// Generates the coefficients for the continued fraction representation of the square root of 2.
  /// </summary>
  /// <returns>An infinite sequence of coefficients for sqrt(2): 1, 2, 2, 2, ...</returns>
  public static IEnumerable<BigInteger> Sqrt2Generator() {
    yield return 1;

    while (true) {
      yield return 2;
    }
  }

}