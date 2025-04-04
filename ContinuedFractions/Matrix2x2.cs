using System.Diagnostics.CodeAnalysis;

namespace ContinuedFractions;

/// <summary>
/// Represents a 2x2 matrix with <see cref="BigInteger"/> elements.
/// Used for representing linear fractional transformations (LFTs) of the form (ax+b)/(cx+d).
/// </summary>
internal readonly struct Matrix22 {

  /// <summary>
  /// The elements of the matrix stored in a flat array: [a11, a12, a21, a22].
  /// </summary>
  private readonly BigInteger[] _m;

  /// <summary>
  /// Gets the element of the matrix at the specified flat index.
  /// </summary>
  /// <param name="i">The flat index (0=a11, 1=a12, 2=a21, 3=a22).</param>
  /// <returns>The <see cref="BigInteger"/> element at the specified index.</returns>
  public BigInteger this[int i] => _m[i];

  /// <summary>
  /// Initializes a new instance of the <see cref="Matrix22"/> struct with the specified elements.
  /// Represents the transformation f(x) = (a11*x + a12) / (a21*x + a22).
  /// </summary>
  /// <param name="a11">Element at row 1, column 1.</param>
  /// <param name="a12">Element at row 1, column 2.</param>
  /// <param name="a21">Element at row 2, column 1.</param>
  /// <param name="a22">Element at row 2, column 2.</param>
  public Matrix22(BigInteger a11, BigInteger a12, BigInteger a21, BigInteger a22) { _m = new[] { a11, a12, a21, a22 }; }

  /// <summary>
  /// Multiplies two 2x2 matrices, representing the composition of their corresponding LFTs.
  /// If M represents f(x) and N represents g(x), then M*N represents f(g(x)).
  /// </summary>
  /// <param name="left">The left matrix operand.</param>
  /// <param name="right">The right matrix operand.</param>
  /// <returns>The resulting <see cref="Matrix22"/> from the multiplication.</returns>
  public static Matrix22 operator *(Matrix22 left, Matrix22 right) {
    return new Matrix22
      (
       left._m[0] * right._m[0] + left._m[1] * right._m[2], // a11*b11 + a12*b21
       left._m[0] * right._m[1] + left._m[1] * right._m[3], // a11*b12 + a12*b22
       left._m[2] * right._m[0] + left._m[3] * right._m[2], // a21*b11 + a22*b21
       left._m[2] * right._m[1] + left._m[3] * right._m[3]  // a21*b12 + a22*b22
      );
  }

  /// <summary>
  /// Returns a string representation of the matrix.
  /// </summary>
  /// <returns>A string in the format "a11 a12\na21 a22".</returns>
  public override string ToString() { return $"{_m[0]} {_m[1]}\n{_m[2]} {_m[3]}"; }

  /// <summary>
  /// Tries to extract the next integer term 'q' from the linear fractional transformation represented by the matrix 'm'.
  /// This is possible if the floor of the LFT is constant for all x >= 1.
  /// It checks if floor( (a*x+b)/(c*x+d) ) is constant by comparing floor(a/c) and floor(b/d).
  /// </summary>
  /// <param name="m">The matrix representing the LFT f(x) = (ax+b)/(cx+d).</param>
  /// <param name="q">When this method returns <c>true</c>, contains the extracted integer term; otherwise, <see langword="null"/>.</param>
  /// <param name="r">When this method returns <c>true</c>, contains the matrix representing the remaining transformation g(x) = 1 / (f(x) - q); otherwise, <see langword="null"/>.</param>
  /// <returns><c>true</c> if the next integer term 'q' could be determined; otherwise, <c>false</c>.</returns>
  /// <exception cref="ArgumentException">Thrown if the second row of the matrix (c, d) is zero.</exception>
  private static bool TryProduceTerm(Matrix22 m, [NotNullWhen(true)] out BigInteger? q, [NotNullWhen(true)] out Matrix22? r) {
    q = null;
    r = null;
    BigInteger a = m[0], b = m[1], c = m[2], d = m[3];

    // Check denominator validity
    if (c == 0 && d == 0) {
      throw new ArgumentException("TryProduceTerm: Second row of the matrix is zero.");
    }

    // If c or d is zero, the LFT approaches a constant or infinity at one end.
    if (c == 0 || d == 0) {
      return false;
    }

    // If the denominator (c*x + d) changes sign for x > 0, the LFT is not monotonic
    if (c.Sign * d.Sign == -1) {
      return false;
    }

    // Calculate the floor of the limits as x->infinity (a/c) and x->0 (b/d)
    BigInteger floor_ac = GosperMatrix.FloorDiv(a, c);
    BigInteger floor_bd = GosperMatrix.FloorDiv(b, d);

    if (floor_ac == floor_bd) {
      // If the floors of the limits are equal, this is our integer term q.
      q = floor_ac;
      BigInteger r10 = c;
      BigInteger r11 = d;
      BigInteger r21 = a - q.Value * c;
      BigInteger r22 = b - q.Value * d;
      r = new Matrix22(r10, r11, r21, r22);

      return true;
    }

    bool ac_is_integer = a % c == 0;
    bool bd_is_integer = b % d == 0;

    // Case 1: floor(a/c) == floor(b/d) + 1 AND a/c is an exact integer.
    // Means limit at infinity is integer floor_ac, limit at 0 has floor floor_ac - 1.
    if (floor_ac == floor_bd + 1 && ac_is_integer)
    {
      q = floor_bd;
      BigInteger r10 = c;
      BigInteger r11 = d;
      BigInteger r21 = a - q.Value * c;
      BigInteger r22 = b - q.Value * d;
      r = new Matrix22(r10, r11, r21, r22);
      return true;
    }

    // Case 2: floor(b/d) == floor(a/c) + 1 AND b/d is an exact integer.
    // Means limit at 0 is integer floor_bd, limit at infinity has floor floor_bd - 1.
    if (floor_bd == floor_ac + 1 && bd_is_integer)
    {
      q = floor_ac;
      BigInteger r10 = c;
      BigInteger r11 = d;
      BigInteger r21 = a - q.Value * c;
      BigInteger r22 = b - q.Value * d;
      r = new Matrix22(r10, r11, r21, r22);
      return true;
    }

    // If none of the conditions are met, we cannot determine a single integer term q.
    return false;
  }

  /// <summary>
  /// Creates a matrix representing the homographic function f(x) = a11 + 1/x.
  /// This corresponds to the matrix [a11, 1; 1, 0].
  /// It's used to incorporate the next term 'a11' of a continued fraction into the transformation.
  /// </summary>
  /// <param name="a11">The integer term to incorporate.</param>
  /// <returns>The <see cref="Matrix22"/> [a11, 1; 1, 0].</returns>
  internal static Matrix22 Homographic(BigInteger a11) => new Matrix22(a11, 1, 1, 0);

  /// <summary>
  /// Generates a sequence of integer terms (q) and remainder matrices (r)
  /// by repeatedly applying <see cref="TryProduceTerm"/> to the initial matrix 'm'.
  /// </summary>
  /// <param name="m">The initial matrix representing the LFT.</param>
  /// <returns>An <see cref="IEnumerable{T}"/> of tuples, where each tuple contains an integer term 'q' and the corresponding remainder matrix 'r'.</returns>
  internal static IEnumerable<(BigInteger q, Matrix22 r)> GenerateLFTSteps(Matrix22 m) {
    while (true) {
      if (m[0] == 0 && m[1] == 0) {
        break;
      }

      if (!TryProduceTerm(m, out BigInteger? q, out Matrix22? r)) {
        break;
      }

      yield return (q.Value, r.Value);

      m = r.Value;
    }
  }

}