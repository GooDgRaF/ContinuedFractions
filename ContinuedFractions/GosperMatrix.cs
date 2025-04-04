using System.Diagnostics.CodeAnalysis;

namespace ContinuedFractions;

/// <summary>
/// Represents the state of Gosper's algorithm for binary operations on continued fractions.
/// The matrix holds coefficients for a transformation Z(x,y) = (Axy + Bx + Cy + D) / (Exy + Fx + Gy + H),
/// where x and y are the remaining parts of the input continued fractions.
/// </summary>
public class GosperMatrix {

  /// <summary> Coefficient A of the numerator (Axy). </summary>
  internal readonly BigInteger A;

  /// <summary> Coefficient B of the numerator (Bx). </summary>
  internal readonly BigInteger B;

  /// <summary> Coefficient C of the numerator (Cy). </summary>
  internal readonly BigInteger C;

  /// <summary> Coefficient D of the numerator (D). </summary>
  internal readonly BigInteger D;

  /// <summary> Coefficient E of the denominator (Exy). </summary>
  internal readonly BigInteger E;

  /// <summary> Coefficient F of the denominator (Fx). </summary>
  internal readonly BigInteger F;

  /// <summary> Coefficient G of the denominator (Gy). </summary>
  internal readonly BigInteger G;

  /// <summary> Coefficient H of the denominator (H). </summary>
  internal readonly BigInteger H;

  /// <summary>
  /// Initializes a new instance of the <see cref="GosperMatrix"/> class with the specified coefficients.
  /// </summary>
  private GosperMatrix(
      BigInteger a
    , BigInteger b
    , BigInteger c
    , BigInteger d
    , BigInteger e
    , BigInteger f
    , BigInteger g
    , BigInteger h
    ) {
    A = a;
    B = b;
    C = c;
    D = d;
    E = e;
    F = f;
    G = g;
    H = h;
  }

  /// <summary>
  /// Updates the matrix by consuming the next term from the first input continued fraction (x).
  /// Replaces x with (term + 1/x').
  /// </summary>
  /// <param name="term">The next coefficient from the first continued fraction.</param>
  /// <returns>A new <see cref="GosperMatrix"/> representing the updated state.</returns>
  public GosperMatrix IngestX(BigInteger term) {
    BigInteger nextA = A * term + C;
    BigInteger nextB = B * term + D;
    BigInteger nextC = A;
    BigInteger nextD = B;
    BigInteger nextE = E * term + G;
    BigInteger nextF = F * term + H;
    BigInteger nextG = E;
    BigInteger nextH = F;

    return new GosperMatrix
      (
       nextA
     , nextB
     , nextC
     , nextD
     , nextE
     , nextF
     , nextG
     , nextH
      );
  }

  /// <summary>
  /// Updates the matrix by consuming the next term from the second input continued fraction (y).
  /// Replaces y with (term + 1/y').
  /// </summary>
  /// <param name="term">The next coefficient from the second continued fraction.</param>
  /// <returns>A new <see cref="GosperMatrix"/> representing the updated state.</returns>
  public GosperMatrix IngestY(BigInteger term) {
    BigInteger nextA = A * term + B;
    BigInteger nextB = A;
    BigInteger nextC = C * term + D;
    BigInteger nextD = C;
    BigInteger nextE = E * term + F;
    BigInteger nextF = E;
    BigInteger nextG = G * term + H;
    BigInteger nextH = G;

    return new GosperMatrix
      (
       nextA
     , nextB
     , nextC
     , nextD
     , nextE
     , nextF
     , nextG
     , nextH
      );
  }

  /// <summary>
  /// Updates the matrix after producing an output term 'q'.
  /// Replaces Z(x,y) with 1 / (Z(x,y) - q).
  /// </summary>
  /// <param name="q">The integer term produced.</param>
  /// <returns>A new <see cref="GosperMatrix"/> representing the updated state.</returns>
  public GosperMatrix Produce(BigInteger q) {
    // New Numerator = Old Denominator
    BigInteger nextA = E;
    BigInteger nextB = F;
    BigInteger nextC = G;
    BigInteger nextD = H;
    // New Denominator = Old Numerator - q * Old Denominator
    BigInteger nextE = A - q * E;
    BigInteger nextF = B - q * F;
    BigInteger nextG = C - q * G;
    BigInteger nextH = D - q * H;

    return new GosperMatrix
      (
       nextA
     , nextB
     , nextC
     , nextD
     , nextE
     , nextF
     , nextG
     , nextH
      );
  }


  /// <summary>
  /// Attempts to determine the next integer term 'q' of the resulting continued fraction.
  /// It analyzes the limiting values of the transformation Z(x,y) = Num/Den as x and y approach infinity and 1.
  /// If the floor of these limiting values is consistent, 'q' is determined.
  /// </summary>
  /// <param name="q">When this method returns <c>true</c>, contains the extracted integer term; otherwise, <see langword="null"/>.</param>
  /// <returns><c>true</c> if the next integer term 'q' could be uniquely determined; otherwise, <c>false</c>.</returns>
  /// <remarks>
  /// The analysis involves calculating the function's value or limit at the corners of the domain (x,y) in {[1, inf], [1, inf]}.
  /// Z(inf, inf) ~ A/E
  /// Z(inf, 1)   ~ (A+B)/(E+F)
  /// Z(1,   inf) ~ (A+C)/(E+G)
  /// Z(1,   1)   ~ (A+B+C+D)/(E+F+G+H)
  /// If the floor of all defined limits is the same, that floor is the next term 'q'.
  /// Potential division by zero and sign changes in the denominator prevent determining 'q'.
  /// </remarks>
  public bool TryGetNextTerm([NotNullWhen(true)] out BigInteger? q) {
    q = null;

    // Calculate coefficients for Z(x+1, y+1) where x, y >= 0
    // This shifts the analysis to the domain [0, inf] x [0, inf]
    BigInteger ap = A;
    BigInteger bp = A + B;
    BigInteger cp = A + C;
    BigInteger dp = A + B + C + D;

    BigInteger ep = E;
    BigInteger fp = E + F;
    BigInteger gp = E + G;
    BigInteger hp = E + F + G + H;

    if (ep == 0 && fp == 0 && gp == 0 && hp == 0) {
      return false;
    }

    // checking signs of coefficients gives a necessary (not sufficient) condition for sign stability.
    var denSigns = new List<int>();
    if (ep != 0)
      denSigns.Add(ep.Sign);
    if (fp != 0)
      denSigns.Add(fp.Sign);
    if (gp != 0)
      denSigns.Add(gp.Sign);
    if (hp != 0)
      denSigns.Add(hp.Sign);

    bool hasPositive = denSigns.Exists(s => s > 0);
    bool hasNegative = denSigns.Exists(s => s < 0);

    if (hasPositive && hasNegative) {
      // Mixed signs suggest potential denominator zero/sign change. Cannot safely determine q.
      return false;
    }

    BigInteger? minFloor = null;
    BigInteger? maxFloor = null;

    // Calculate floor for the four corner cases (limits/values)
    UpdateMinMaxFloor(ap, ep); // Z(inf, inf) ~ a'/e' (Limit as x,y -> inf)
    UpdateMinMaxFloor(bp, fp); // Z(inf, 1)   ~ b'/f' (Limit as x -> inf, y = 0)
    UpdateMinMaxFloor(cp, gp); // Z(1,   inf) ~ c'/g' (Limit as x = 0, y -> inf)
    UpdateMinMaxFloor(dp, hp); // Z(1,   1)   = d'/h' (Value at x = 0, y = 0)

    // Helper to compute FloorDiv and update min/max floor values.
    void UpdateMinMaxFloor(BigInteger num, BigInteger den) {
      if (den == 0)
        return;

      int targetSign = hasPositive ? 1 : (hasNegative ? -1 : 0);
      if (targetSign != 0 && den.Sign != 0 && den.Sign != targetSign) {
        return;
      }

      BigInteger currentFloor = FloorDiv(num, den);

      if (minFloor == null) {
        minFloor = currentFloor;
        maxFloor = currentFloor;
      }
      else {
        if (currentFloor < minFloor)
          minFloor = currentFloor;
        if (currentFloor > maxFloor)
          maxFloor = currentFloor;
      }
    }

    if (minFloor == null) {
      // Could not compute any finite limit floor (e.g., all denominators were zero).
      return false;
    }

    if (minFloor == maxFloor) {
      // All computed floors are the same. This is our term q.
      q = minFloor;

      return true;
    }

    // Floors differ. Not enough information to produce a term.
    return false;
  }

  /// <summary>
  /// Computes the floor of the division of two <see cref="BigInteger"/> numbers (n / d).
  /// Handles negative numbers correctly, according to the definition of floor (rounding towards negative infinity).
  /// </summary>
  /// <param name="n">The numerator.</param>
  /// <param name="d">The denominator.</param>
  /// <returns>The largest integer less than or equal to n / d.</returns>
  /// <exception cref="DivideByZeroException">Thrown if the denominator <paramref name="d"/> is zero.</exception>
  public static BigInteger FloorDiv(BigInteger n, BigInteger d) {
    if (d == 0)
      throw new DivideByZeroException("Attempted FloorDiv by zero.");

    BigInteger q = BigInteger.DivRem(n, d, out BigInteger r);

    // If n and d have different signs AND there is a remainder,
    // the division rounded towards zero (q) is higher than the floor. Subtract 1.
    bool differentSigns = n.Sign * d.Sign == -1;
    if (differentSigns && r != 0) {
      return q - 1;
    }

    return q;
  }

  // --- Static factory methods for initial matrices ---

  /// <summary>
  /// Gets the initial <see cref="GosperMatrix"/> for addition (x + y).
  /// Z(x,y) = (0xy + 1x + 1y + 0) / (0xy + 0x + 0y + 1) = x + y.
  /// </summary>
  /// <returns>The matrix for addition.</returns>
  public static GosperMatrix Addition()
    => new GosperMatrix
      (
       0
     , 1
     , 1
     , 0
     , 0
     , 0
     , 0
     , 1
      );

  /// <summary>
  /// Gets the initial <see cref="GosperMatrix"/> for subtraction (x - y).
  /// Z(x,y) = (0xy + 1x - 1y + 0) / (0xy + 0x + 0y + 1) = x - y.
  /// </summary>
  /// <returns>The matrix for subtraction.</returns>
  public static GosperMatrix Subtraction()
    => new GosperMatrix
      (
       0
     , 1
     , -1
     , 0
     , 0
     , 0
     , 0
     , 1
      );

  /// <summary>
  /// Gets the initial <see cref="GosperMatrix"/> for multiplication (x * y).
  /// Z(x,y) = (1xy + 0x + 0y + 0) / (0xy + 0x + 0y + 1) = xy.
  /// </summary>
  /// <returns>The matrix for multiplication.</returns>
  public static GosperMatrix Multiplication()
    => new GosperMatrix
      (
       1
     , 0
     , 0
     , 0
     , 0
     , 0
     , 0
     , 1
      );

  /// <summary>
  /// Gets the initial <see cref="GosperMatrix"/> for division (x / y).
  /// Z(x,y) = (0xy + 1x + 0y + 0) / (0xy + 0x + 1y + 0) = x / y.
  /// </summary>
  /// <returns>The matrix for division.</returns>
  public static GosperMatrix Division()
    => new GosperMatrix
      (
       0
     , 1
     , 0
     , 0
     , 0
     , 0
     , 1
     , 0
      );

  /// <summary>
  /// Returns a string representation of the transformation Z(x,y).
  /// </summary>
  /// <returns>A string in the format "(Axy+Bx+Cy+D)/(Exy+Fx+Gy+H)".</returns>
  public override string ToString() { return $"({A}xy+{B}x+{C}y+{D})/({E}xy+{F}x+{G}y+{H})"; }

}
