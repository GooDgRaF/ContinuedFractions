namespace ContinuedFractions;

/// <summary>
/// Represents a continued fraction using <see cref="BigInteger"/> coefficients.
/// This implementation features lazy evaluation and caching of computed coefficients.
/// Core arithmetic operations aim for exact results using BigInteger. However, comparisons and representations
/// are effectively limited by a fixed number of coefficients (defaulting to a quantity sufficient for double precision).
/// </summary>
public partial class CFraction {

  /// <summary>
  /// Converts the continued fraction to its exact rational representation (p/q).
  /// </summary>
  /// <returns>A <see cref="Frac"/> tuple representing the rational number (numerator p, denominator q).</returns>
  /// <remarks>
  /// If the continued fraction is infinite, this method doesn't terminate. Use GenerateConvergents().ElementAt(i).
  /// </remarks>
  public Frac ToFrac() => GenerateConvergents().Last();

  /// <summary>
  /// Generates the sequence of convergents (rational approximations) for the continued fraction.
  /// </summary>
  /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Frac"/> tuples, each representing a convergent (p_n, q_n).</returns>
  /// <remarks>
  /// This method generates convergents based on the *currently cached* coefficients.
  /// It does not automatically force evaluation of the entire continued fraction if it's lazily generated.
  /// To get convergents further down an infinite sequence, ensure more coefficients are cached first (e.g., by accessing a higher index).
  /// </remarks>
  public IEnumerable<(BigInteger numerator, BigInteger denominator)> GenerateConvergents() => ToRational(_cfCashed);

  /// <summary>
  /// Explicitly converts the continued fraction to a <see cref="double"/>.
  /// </summary>
  /// <param name="cf">The continued fraction to convert.</param>
  /// <returns>
  /// A <see cref="double"/> approximation of the continued fraction's value.
  /// Returns <see cref="double.PositiveInfinity"/> if the fraction is <see cref="Infinity"/> (empty coefficient list).
  /// </returns>
  /// <remarks>
  /// The conversion uses up to the first 40 coefficients (<c>numberOfCoeffs</c>) to calculate the rational approximation
  /// and then performs floating-point division. The precision is limited by both the number of coefficients used
  /// and the inherent limitations of <see cref="double"/> representation.
  /// </remarks>
  public static explicit operator double(CFraction cf) {
    var d = cf.Take(40).ToList();
    if (d.Count == 0) {
      return double.PositiveInfinity;
    }

    (var a, var b) = ToRational(d).Last();

    return (double)a / (double)b;
  }

  /// <summary>
  /// Generates the sequence of convergents (p_n/q_n) for a given sequence of continued fraction coefficients.
  /// </summary>
  /// <param name="cfCoeffs">The sequence of coefficients [a0; a1, a2, ...].</param>
  /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Frac"/> tuples representing the convergents (p_n, q_n).</returns>
  /// <remarks>
  /// Implements the standard recurrence relations:
  /// p_n = a_n * p_{n-1} + p_{n-2}
  /// q_n = a_n * q_{n-1} + q_{n-2}
  /// with initial values p_{-1}=1, p_{-2}=0, q_{-1}=0, q_{-2}=1.
  /// </remarks>
  private static IEnumerable<(BigInteger numerator, BigInteger denominator)> ToRational(IEnumerable<BigInteger> cfCoeffs) {
    if (!cfCoeffs.Any()) {
      yield return (1, 0);
    }

    BigInteger p_prev        = 1;
    BigInteger p_before_prev = 0;
    BigInteger q_prev        = 0;
    BigInteger q_before_prev = 1;

    foreach (BigInteger coeff_n in cfCoeffs) {
      BigInteger p_n = coeff_n * p_prev + p_before_prev;
      BigInteger q_n = coeff_n * q_prev + q_before_prev;

      yield return (p_n, q_n);

      p_before_prev = p_prev;
      p_prev        = p_n;
      q_before_prev = q_prev;
      q_prev        = q_n;
    }
  }

}
