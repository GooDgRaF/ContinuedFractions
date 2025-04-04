namespace ContinuedFractions;

/// <summary>
/// Represents a continued fraction using <see cref="BigInteger"/> coefficients.
/// This implementation features lazy evaluation and caching of computed coefficients.
/// Core arithmetic operations aim for exact results using BigInteger. However, comparisons and representations
/// are effectively limited by a fixed number of coefficients (defaulting to a quantity sufficient for double precision).
/// </summary>
public partial class CFraction : IComparable<CFraction>, IEquatable<CFraction>, IComparisonOperators<CFraction, CFraction, bool> {

  /// <summary>
  /// Compares this continued fraction to another.
  /// </summary>
  /// <param name="other">The continued fraction to compare to.</param>
  /// <returns>
  /// A negative value if this instance is less than <paramref name="other"/>.
  /// Zero if this instance is equal to <paramref name="other"/>.
  /// A positive value if this instance is greater than <paramref name="other"/>.
  /// Returns 1 if <paramref name="other"/> is null.
  /// </returns>
  /// <remarks>
  /// The comparison uses a modified lexicographical comparison of coefficients.
  /// For coefficients at odd indices (a1, a3, ...), their signs are flipped
  /// before comparison (A less B iff isLexmin([a0;-a1,a2,-a3,...], [b0;-b1,b2,-b3,...])).
  /// The comparison is limited by <see cref="numberOfCoeffs"/>.
  /// </remarks>
  public int CompareTo(CFraction? other) {
    if (other is null) {
      return 1; // Consistent with standard IComparable behavior
    }

    if (this[0] is null && other[0] is not null) {
      return 1;
    }
    if (this[0] is not null && other[0] is null) {
      return -1;
    }

    int i = 0;
    while (i <= numberOfCoeffs) {
      BigInteger? val1 = this[i];
      BigInteger? val2 = other[i];

      if (val1 is null && val2 is null) {
        return 0;
      }

      if (val2 == null) {
        return (i % 2 == 0) ? -1 : 1;
      }

      if (val1 == null) {
        return (i % 2 == 0) ? 1 : -1;
      }


      if (i % 2 != 0) {
        val1 = -val1;
        val2 = -val2;
      }

      if (val1 < val2) {
        return -1;
      }
      if (val1 > val2) {
        return 1;
      }

      i++;
    }

    return 0;
  }

  /// <summary>
  /// Indicates whether the current object is equal to another object of the same type.
  /// </summary>
  /// <param name="other">An object to compare with this object.</param>
  /// <returns><c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c> .</returns>
  /// <remarks>Equality is determined by the <see cref="CompareTo"/> method returning 0.</remarks>
  public bool Equals(CFraction? other) => this.CompareTo(other) == 0;

  /// <summary>
  /// Determines whether two specified <see cref="CFraction"/> objects have the same value.
  /// </summary>
  /// <param name="left">The first continued fraction to compare, or null.</param>
  /// <param name="right">The second continued fraction to compare, or null.</param>
  /// <returns><c>true</c> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <c>false</c> .</returns>
  public static bool operator ==(CFraction? left, CFraction? right) {
    if (ReferenceEquals(left, right)) {
      return true;
    }
    if (left is null || right is null) {
      return false;
    }

    return left.Equals(right);
  }

  /// <summary>
  /// Determines whether two specified <see cref="CFraction"/> objects have different values.
  /// </summary>
  /// <param name="left">The first continued fraction to compare, or null.</param>
  /// <param name="right">The second continued fraction to compare, or null.</param>
  /// <returns><c>true</c> if the value of <paramref name="left"/> is different from the value of <paramref name="right"/>; otherwise, <c>false</c> .</returns>
  public static bool operator !=(CFraction? left, CFraction? right) => !(left == right);

  /// <summary>
  /// Determines whether the specified object is equal to the current object.
  /// </summary>
  /// <param name="obj">The object to compare with the current object.</param>
  /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c> .</returns>
  public override bool Equals(object? obj) => obj is CFraction other && Equals(other);

  /// <summary>
  /// Serves as the default hash function.
  /// </summary>
  /// <returns>A hash code for the current object.</returns>
  /// <remarks>
  /// The hash code is calculated based on the first <see cref="numberOfCoeffs"/> coefficients.
  /// Infinity has a distinct hash code.
  /// </remarks>
  public override int GetHashCode() {
    if (this == Infinity) {
      return 2147483647;
    }
    HashCode hash = new HashCode();
    foreach (BigInteger term in this.Take(numberOfCoeffs)) {
      hash.Add(term);
    }

    return hash.ToHashCode();
  }


  /// <summary>
  /// Determines whether one specified <see cref="CFraction"/> is greater than another specified <see cref="CFraction"/>.
  /// </summary>
  /// <param name="left">The first continued fraction to compare.</param>
  /// <param name="right">The second continued fraction to compare.</param>
  /// <returns><c>true</c> if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, <c>false</c> .</returns>
  public static bool operator >(CFraction left, CFraction right) => left.CompareTo(right) > 0;

  /// <summary>
  /// Determines whether one specified <see cref="CFraction"/> is greater than or equal to another specified <see cref="CFraction"/>.
  /// </summary>
  /// <param name="left">The first continued fraction to compare.</param>
  /// <param name="right">The second continued fraction to compare.</param>
  /// <returns><c>true</c> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, <c>false</c> .</returns>
  public static bool operator >=(CFraction left, CFraction right) => left.CompareTo(right) >= 0;

  /// <summary>
  /// Determines whether one specified <see cref="CFraction"/> is less than another specified <see cref="CFraction"/>.
  /// </summary>
  /// <param name="left">The first continued fraction to compare.</param>
  /// <param name="right">The second continued fraction to compare.</param>
  /// <returns><c>true</c> if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, <c>false</c> .</returns>
  public static bool operator <(CFraction left, CFraction right) => left.CompareTo(right) < 0;

  /// <summary>
  /// Determines whether one specified <see cref="CFraction"/> is less than or equal to another specified <see cref="CFraction"/>.
  /// </summary>
  /// <param name="left">The first continued fraction to compare.</param>
  /// <param name="right">The second continued fraction to compare.</param>
  /// <returns><c>true</c> if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, <c>false</c> .</returns>
  public static bool operator <=(CFraction left, CFraction right) => left.CompareTo(right) <= 0;

}
