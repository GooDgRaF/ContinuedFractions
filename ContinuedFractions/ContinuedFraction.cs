using System.Collections;
using System.Diagnostics;
using System.Text;

namespace ContinuedFractions;

/// <summary>
/// Represents a continued fraction using <see cref="BigInteger"/> coefficients.
/// This implementation features lazy evaluation and caching of computed coefficients.
/// Core arithmetic operations aim for exact results using BigInteger. However, comparisons and representations
/// are effectively limited by a fixed number of coefficients (defaulting to a quantity sufficient for double precision).
/// </summary>
public partial class CFraction : IEnumerable<BigInteger> {

  /// <summary>
  /// Default number of coefficients used for comparisons, hashing, and string representation.
  /// Chosen to provide sufficient precision comparable to a standard <see cref="double"/>.
  /// </summary>
  /// <remarks>
  /// Increasing this value improves precision for comparisons and representations but may impact performance.
  /// Arithmetic operations are not limited by this constant and work with available coefficients.
  /// </remarks>
  private const int numberOfCoeffs = 40; // Approx. double precision

#region Data and Constructors
  /// <summary>
  /// The enumerator responsible for generating subsequent coefficients if the fraction is infinite or not fully evaluated yet.
  /// It becomes <see langword="null"/> once the generator is exhausted or if the fraction was created from a finite list.
  /// </summary>
  private IEnumerator<BigInteger>? _cfEnumerator;

  /// <summary>
  /// A list caching the coefficients that have already been computed or were provided initially.
  /// For finite fractions, this list holds all coefficients.
  /// For potentially infinite fractions, it grows as coefficients are requested via the indexer or enumeration.
  /// </summary>
  private readonly List<BigInteger> _cfCashed;

  /// <summary>
  /// Private constructor used internally to create a <see cref="CFraction"/> from a fully known list of coefficients (finite fraction).
  /// The generator enumerator is explicitly set to <see langword="null"/>.
  /// </summary>
  /// <param name="cf">The list of coefficients representing the finite continued fraction.</param>
  private CFraction(List<BigInteger> cf) {
    _cfCashed = cf;

    // Apply canonicalization: [..., a_{n-1}, 1] -> [..., a_{n-1} + 1]
    if (_cfCashed.Count > 1 && _cfCashed[^1] == 1) {
      _cfCashed.RemoveAt(_cfCashed.Count - 1);
      _cfCashed[^1]++;
    }
    _cfEnumerator = null;
  }

  /// <summary>
  /// Private constructor used internally to create a <see cref="CFraction"/> from an enumerable sequence (potentially infinite generator).
  /// Initializes the cache with the first few coefficients.
  /// </summary>
  /// <param name="cf">The enumerable sequence generating the coefficients.</param>
  private CFraction(IEnumerable<BigInteger> cf) {
    _cfEnumerator = cf.GetEnumerator();
    _cfCashed     = new List<BigInteger>();

    // Try to cache the first coefficient (a0)
    if (_cfEnumerator.MoveNext()) {
      _cfCashed.Add(_cfEnumerator.Current);

      // Try to cache the second coefficient (a1) to handle finite [a0]
      if (!_cfEnumerator.MoveNext()) {
        _cfEnumerator.Dispose();
        _cfEnumerator = null;
      }
      else {
        // Sequence has at least [a0, a1].
        _cfCashed.Add(_cfEnumerator.Current);
      }
    }
    else {
      // Sequence was empty [], representing Infinity.
      _cfEnumerator.Dispose();
      _cfEnumerator = null;
    }
    // Note: Canonicalization ([..., a_n-1, 1] -> [..., a_n-1 + 1]) is handled later in CacheUpToIndex when the end is reached.
  }
#endregion

#region Indexer
  /// <summary>
  /// Gets the coefficient at the specified index <paramref name="i"/> (a_i).
  /// If the coefficient is not already cached, it attempts to generate and cache coefficients up to that index.
  /// </summary>
  /// <param name="i">The zero-based index of the coefficient (a0, a1, a2, ...).</param>
  /// <returns>The <see cref="BigInteger"/> coefficient at index <paramref name="i"/>, or <see langword="null"/> if the fraction has fewer than <paramref name="i"/>+1 coefficients.</returns>
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

  /// <summary>
  /// Ensures that coefficients up to and including the specified index <paramref name="targetIndex"/> are cached.
  /// If the fraction is generated lazily, this method advances the underlying enumerator.
  /// Handles generator exhaustion and applies canonicalization (merging a trailing 1).
  /// </summary>
  /// <param name="targetIndex">The minimum index required in the cache.</param>
  /// <returns><c>true</c> if the cache contains at least <paramref name="targetIndex"/>+1 elements after execution; <c>false</c> otherwise (meaning the fraction terminated earlier).</returns>
  private bool CacheUpToIndex(int targetIndex) {
    while (_cfCashed.Count <= targetIndex + 2 && _cfEnumerator is not null) {
      if (!_cfEnumerator.MoveNext()) {
        _cfEnumerator.Dispose();
        _cfEnumerator = null;

        if (_cfCashed.Count > 1 && _cfCashed[^1] == 1) { // Canonisation
          _cfCashed.RemoveAt(_cfCashed.Count - 1);
          _cfCashed[^1]++;
        }

        break;
      }
      _cfCashed.Add(_cfEnumerator.Current);
    }

    return _cfCashed.Count > targetIndex;
  }
#endregion

#region Factories
  /// <summary>
  /// Creates a <see cref="CFraction"/> from a rational number (numerator / denominator).
  /// </summary>
  /// <param name="numerator">The numerator of the fraction.</param>
  /// <param name="denominator">The denominator of the fraction.</param>
  /// <returns>A <see cref="CFraction"/> representing the rational number.</returns>
  /// <exception cref="ArgumentException">Thrown if both numerator and denominator are zero (0/0).</exception>
  /// <exception cref="DivideByZeroException">Implicitly handled by RationalGenerator if denominator is initially zero (results in Infinity or specific CF).</exception>
  public static CFraction FromRational(BigInteger numerator, BigInteger denominator) {
    if (numerator == 0 && denominator == 0) {
      throw new ArgumentException("Can't handle the 0/0 fraction!", nameof(numerator));
    }

    return new CFraction(RationalGenerator(numerator, denominator));
  }

  /// <summary>
  /// Creates a finite <see cref="CFraction"/> from a list of integer coefficients.
  /// </summary>
  /// <param name="cf">The list of coefficients [a0, a1, a2, ...].</param>
  /// <returns>A <see cref="CFraction"/> representing the finite continued fraction.</returns>
  /// <exception cref="ArgumentException">Thrown if any coefficient after the first (a1, a2, ...) is less than 1.</exception>
  public static CFraction FromCoeffs(IList<int> cf) => FromCoeffs(cf.Select(v => (BigInteger)v).ToArray());

  /// <summary>
  /// Creates a finite <see cref="CFraction"/> from a list of <see cref="BigInteger"/> coefficients.
  /// Ensures the resulting fraction is in canonical form (does not end with 1, unless it's just [1]).
  /// </summary>
  /// <param name="cf">The list of coefficients [a0, a1, a2, ...].</param>
  /// <returns>A <see cref="CFraction"/> representing the finite continued fraction.</returns>
  /// <exception cref="ArgumentException">Thrown if any coefficient after the first (a1, a2, ...) is less than 1.</exception>
  public static CFraction FromCoeffs(IList<BigInteger> cf) {
    List<BigInteger> r = cf.ToList();
    if (r.Skip(1).Any(c => c < 1)) {
      throw new ArgumentException
        ("There should be all coefficients of the continued function (after the first one) greater than zero!", nameof(cf));
    }

    return new CFraction(r);
  }

  /// <summary>
  /// Creates a potentially infinite <see cref="CFraction"/> from a generator sequence of <see cref="BigInteger"/> coefficients.
  /// Coefficients are evaluated lazily as needed.
  /// </summary>
  /// <param name="cf">The enumerable sequence generating the coefficients.</param>
  /// <returns>A new <see cref="CFraction"/> instance.</returns>
  /// <exception cref="ArgumentException">Can be thrown later during evaluation if the generator yields invalid coefficients (e.g., non-positive after the first).</exception>
  public static CFraction FromGenerator(IEnumerable<BigInteger> cf) => new CFraction(cf);

  /// <summary>
  /// Creates a potentially infinite <see cref="CFraction"/> from a generator sequence of integer coefficients.
  /// Coefficients are evaluated lazily as needed.
  /// </summary>
  /// <param name="cf">The enumerable sequence generating the integer coefficients.</param>
  /// <returns>A new <see cref="CFraction"/> instance.</returns>
  public static CFraction FromGenerator(IEnumerable<int> cf) => new CFraction(cf.Select(v => (BigInteger)v));
#endregion

#region Overrides
  /// <summary>
  /// Returns a string representation of the continued fraction.
  /// Format: "[a0; a1, a2, ..., an]" for finite fractions.
  /// Format: "[a0; a1, a2, ..., ak, ...]" for infinite or partially evaluated fractions, showing up to <see cref="numberOfCoeffs"/> coefficients after a0.
  /// Format: "[]" for Infinity.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() {
    StringBuilder res = new StringBuilder("[");

    CacheUpToIndex(numberOfCoeffs);
    var elementsToDisplay = this.Take(numberOfCoeffs + 1).ToList();

    if (elementsToDisplay.Count == 0) {
      return res.Append(']').ToString();
    }

    res.Append(elementsToDisplay[0]);

    if (elementsToDisplay.Count == 1) {
      res.Append(']');

      return res.ToString();
    }

    // Add the separator for subsequent coefficients
    res.Append(';');

    bool isFirst = true;
    foreach (var coefficient in elementsToDisplay.Skip(1)) {
      if (isFirst) {
        isFirst = false;
      }
      else {
        res.Append(',');
      }
      res.Append(coefficient);
    }

    if (elementsToDisplay.Count > numberOfCoeffs) {
      res.Append(",...");
    }

    res.Append(']');

    return res.ToString();
  }
#endregion

#region Static Helpers
  /// <summary>
  /// Generates the continued fraction coefficients for a rational number (numerator / denominator)
  /// using the Euclidean algorithm.
  /// </summary>
  /// <param name="numerator">Initial numerator.</param>
  /// <param name="denominator">Initial denominator.</param>
  /// <returns>An <see cref="IEnumerable{T}"/> yielding the <see cref="BigInteger"/> coefficients.</returns>
  /// <remarks>Handles cases where the denominator is zero (yields no coefficients -> Infinity) or negative.</remarks>
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
  /// <summary>
  /// Private enumerator class to iterate over the coefficients of the <see cref="CFraction"/>.
  /// Uses the cache and potentially the underlying generator.
  /// </summary>
  private class CFEnumerator : IEnumerator<BigInteger> {

    /// <summary> The continued fraction being enumerated. </summary>
    private readonly CFraction _cf;

    /// <summary> The current index of the enumerator (-1 means before the first element). </summary>
    private int _index = -1;

    /// <summary> Initializes a new instance of the <see cref="CFEnumerator"/> class. </summary>
    /// <param name="cf">The continued fraction to enumerate.</param>
    public CFEnumerator(CFraction cf) => _cf = cf;

    /// <summary> Advances the enumerator to the next coefficient. </summary>
    /// <returns><c>true</c> if the enumerator was successfully advanced to the next element;
    /// <c>false</c> if the enumerator has passed the end of the collection.</returns>
    public bool MoveNext() {
      _index++;

      return _cf.CacheUpToIndex(_index);
    }

    /// <summary> Gets the coefficient at the current position of the enumerator. </summary>
    /// <exception cref="IndexOutOfRangeException">Thrown implicitly by List if index is invalid (original behavior).</exception>
    public BigInteger Current => _cf._cfCashed[_index];

    /// <summary> Gets the coefficient at the current position of the enumerator (explicit interface implementation). </summary>
    object IEnumerator.Current => Current;

    /// <summary> Sets the enumerator to its initial position, which is before the first element in the collection. </summary>
    public void Reset() { _index = -1; }

    /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
    public void Dispose() { }

  }

  /// <summary>
  /// Returns an enumerator that iterates through the coefficients of the continued fraction.
  /// </summary>
  /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the coefficients.</returns>
  public IEnumerator<BigInteger> GetEnumerator() { return new CFEnumerator(this); }

  /// <summary>
  /// Returns an enumerator that iterates through the coefficients of the continued fraction (explicit interface implementation).
  /// </summary>
  /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the coefficients.</returns>
  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
#endregion

}
