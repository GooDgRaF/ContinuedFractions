using System.Diagnostics;

namespace ContinuedFractions;

/// <summary>
/// Represents a continued fraction using <see cref="BigInteger"/> coefficients.
/// This implementation features lazy evaluation and caching of computed coefficients.
/// Core arithmetic operations aim for exact results using BigInteger. However, comparisons and representations
/// are effectively limited by a fixed number of coefficients (defaulting to a quantity sufficient for double precision).
/// </summary>
public partial class CFraction : IUnaryNegationOperators<CFraction, CFraction>
                               , IUnaryPlusOperators<CFraction, CFraction>
                               , IAdditionOperators<CFraction, CFraction, CFraction>
                               , ISubtractionOperators<CFraction, CFraction, CFraction>
                               , IMultiplyOperators<CFraction, CFraction, CFraction>
                               , IDivisionOperators<CFraction, CFraction, CFraction> {

  /// <summary>
  /// A pre-allocated array containing only zero, used for constructing the reciprocal [0; a0, a1, ...] from [a0; a1, ...].
  /// </summary>
  private static readonly BigInteger[] zeroForRcp = new BigInteger[] { 0 };

  /// <summary>
  /// Checks if this continued fraction represents Infinity.
  /// Private helper method.
  /// </summary>
  /// <returns><c>true</c> if this instance is Infinity; otherwise, <c>false</c>.</returns>
  private bool IsInfinity() => IsInfinity(this);

  /// <summary>
  /// Checks if this continued fraction represents Infinity.
  /// Private helper method.
  /// </summary>
  /// <returns><c>true</c> if this instance is Infinity; otherwise, <c>false</c>.</returns>
  private static bool IsInfinity(CFraction cf) => cf.Equals(Infinity);

#region Unary operators
  /// <summary>
  /// Calculates the reciprocal (1/x) of the continued fraction.
  /// </summary>
  /// <returns>A new <see cref="CFraction"/> representing the reciprocal.</returns>
  /// <remarks>
  /// Handles special cases:
  /// - 1/Infinity ([]) returns 0 ([0]).
  /// - 1/[0; a1, a2, ...] returns [a1; a2, ...].
  /// - 1/[a0; a1, ...] (a0 > 0) returns [0; a0, a1, ...].
  /// - For negative numbers (a0 less 0), it uses the identity 1/(-x) = -(1/x).
  /// </remarks>
  public CFraction Rcp() {
    BigInteger? a0 = this[0]; // Access the first coefficient

    if (a0 is null)
      return FromRational(0, 1);

    if (a0 == 0)
      return FromGenerator(this.Skip(1));

    if (a0 > 0)
      return FromGenerator(zeroForRcp.Concat(this));

    if (a0 < 0)
      // TODO: Can this be done more efficiently without creating two intermediate CFractions?
      return -(-this).Rcp();

    throw new ArgumentException("It can not be true! Unexpected value for the first coefficient.");
  }

  /// <summary>
  /// Unary plus operator (returns the value operand).
  /// </summary>
  /// <param name="cf">The continued fraction operand.</param>
  /// <returns>The operand itself.</returns>
  public static CFraction operator +(CFraction cf) => cf;

  /// <summary>
  /// Unary negation operator (returns the negative of the operand).
  /// </summary>
  /// <param name="cf">The continued fraction operand.</param>
  /// <returns>A new <see cref="CFraction"/> representing the negative value.</returns>
  /// <remarks>Uses a linear fractional transformation: -x = (-1*x + 0) / (0*x + 1).</remarks>
  public static CFraction operator -(CFraction cf) => cf.Transform(-1, 0, 0, 1);
#endregion

#region CFrac (op) Frac
  /// <summary>Adds a rational number (fraction) to a continued fraction.</summary>
  /// <param name="cf">The continued fraction operand.</param>
  /// <param name="frac">The rational number operand (p/q).</param>
  /// <returns>The result of the addition as a <see cref="CFraction"/>.</returns>
  /// <remarks>Uses a linear fractional transformation: x + p/q = (q*x + p) / (0*x + q).</remarks>
  public static CFraction operator +(CFraction cf, Frac frac) => cf.Transform(frac.q, frac.p, 0, frac.q);

  /// <summary>Adds a continued fraction to a rational number (fraction).</summary>
  /// <param name="frac">The rational number operand (p/q).</param>
  /// <param name="cf">The continued fraction operand.</param>
  /// <returns>The result of the addition as a <see cref="CFraction"/>.</returns>
  public static CFraction operator +(Frac frac, CFraction cf) => cf + frac;

  /// <summary>Subtracts a rational number (fraction) from a continued fraction.</summary>
  /// <param name="cf">The continued fraction operand (minuend).</param>
  /// <param name="frac">The rational number operand (subtrahend, p/q).</param>
  /// <returns>The result of the subtraction as a <see cref="CFraction"/>.</returns>
  /// <remarks>Uses a linear fractional transformation: x - p/q = (q*x - p) / (0*x + q).</remarks>
  public static CFraction operator -(CFraction cf, Frac frac) => cf.Transform(frac.q, -frac.p, 0, frac.q);

  /// <summary>Subtracts a continued fraction from a rational number (fraction).</summary>
  /// <param name="frac">The rational number operand (minuend, p/q).</param>
  /// <param name="cf">The continued fraction operand (subtrahend).</param>
  /// <returns>The result of the subtraction as a <see cref="CFraction"/>.</returns>
  /// <remarks>Uses a linear fractional transformation: p/q - x = (-q*x + p) / (0*x + q).</remarks>
  public static CFraction operator -(Frac frac, CFraction cf) => cf.Transform(-frac.q, frac.p, 0, frac.q);


  /// <summary>Multiplies a continued fraction by a rational number (fraction).</summary>
  /// <param name="cf">The continued fraction operand.</param>
  /// <param name="frac">The rational number operand (p/q).</param>
  /// <returns>The result of the multiplication as a <see cref="CFraction"/>.</returns>
  /// <remarks>Uses a linear fractional transformation: x * (p/q) = (p*x + 0) / (0*x + q).</remarks>
  /// <exception cref="ArgumentException">Thrown if attempting to multiply Infinity by zero (Inf * 0).</exception>
  public static CFraction operator *(CFraction cf, Frac frac) {
    if (IsInfinity(cf) && frac.p == 0) {
      throw new ArgumentException("Indeterminate form: Cannot multiply Infinity by Zero (CFraction * Frac(0,q)).");
    }
    // cf * 0 = 0
    if (frac.p == 0) {
      return Zero;
    }
    // Inf * (p/q) = Inf
    if (IsInfinity(cf)) {
      return Infinity;
    }
    if (cf.Equals(Zero)) {
      return Zero;
    }

    return cf.Transform(frac.p, 0, 0, frac.q);
  }

  /// <summary>Multiplies a rational number (fraction) by a continued fraction.</summary>
  /// <param name="frac">The rational number operand (p/q).</param>
  /// <param name="cf">The continued fraction operand.</param>
  /// <returns>The result of the multiplication as a <see cref="CFraction"/>.</returns>
  public static CFraction operator *(Frac frac, CFraction cf) => cf * frac;

  /// <summary>Divides a continued fraction by a rational number (fraction).</summary>
  /// <param name="cf">The continued fraction operand (dividend).</param>
  /// <param name="frac">The rational number operand (divisor, p/q).</param>
  /// <returns>The result of the division as a <see cref="CFraction"/>.</returns>
  /// <remarks>Uses a linear fractional transformation: x / (p/q) = (q*x + 0) / (0*x + p).</remarks>
  /// <exception cref="DivideByZeroException">Thrown if the rational number divisor <paramref name="frac"/> is zero (p=0).</exception>
  public static CFraction operator /(CFraction cf, Frac frac) {
    if (frac.p == 0) {
      throw new DivideByZeroException("Division by zero: Cannot divide CFraction by zero Frac.");
    }
    //Inf / (p/q) = Inf
    if (IsInfinity(cf)) {
      return Infinity; // Sign would depend on sign(p/q) if we had -Inf
    }
    // 0 / (p/q) = 0
    if (cf.Equals(Zero)) {
      return Zero;
    }

    return cf.Transform(frac.q, 0, 0, frac.p);
  }

  /// <summary>Divides a rational number (fraction) by a continued fraction.</summary>
  /// <param name="frac">The rational number operand (dividend, p/q).</param>
  /// <param name="cf">The continued fraction operand (divisor).</param>
  /// <returns>The result of the division as a <see cref="CFraction"/>.</returns>
  /// <remarks>Calculated as <paramref name="frac"/> * <paramref name="cf"/>.Rcp().</remarks>
  /// <exception cref="DivideByZeroException">Thrown if the continued fraction divisor <paramref name="cf"/> is zero.</exception>
  public static CFraction operator /(Frac frac, CFraction cf) {
    if (cf.Equals(Zero)) {
      throw new DivideByZeroException("Division by zero: Cannot divide Frac by zero CFraction.");
    }
    // (p/q) / Inf = 0
    if (IsInfinity(cf)) {
      return Zero;
    }

    return cf.Transform(0, frac.p, frac.q, 0);
  }
#endregion

#region CFrac (op) BigInteger
  /// <summary>Adds a <see cref="BigInteger"/> to a continued fraction.</summary>
  public static CFraction operator +(CFraction cf, BigInteger a) => cf + (a, 1);

  /// <summary>Adds a continued fraction to a <see cref="BigInteger"/>.</summary>
  public static CFraction operator +(BigInteger a, CFraction cf) => cf + a;

  /// <summary>Subtracts a <see cref="BigInteger"/> from a continued fraction.</summary>
  public static CFraction operator -(CFraction cf, BigInteger a) => cf - (a, 1);

  /// <summary>Subtracts a continued fraction from a <see cref="BigInteger"/>.</summary>
  public static CFraction operator -(BigInteger a, CFraction cf) => (a, 1) - cf;

  /// <summary>Multiplies a continued fraction by a <see cref="BigInteger"/>.</summary>
  public static CFraction operator *(CFraction cf, BigInteger a) => cf * (a, 1);

  /// <summary>Multiplies a <see cref="BigInteger"/> by a continued fraction.</summary>
  public static CFraction operator *(BigInteger a, CFraction cf) => cf * a;

  /// <summary>Divides a continued fraction by a <see cref="BigInteger"/>.</summary>
  public static CFraction operator /(CFraction cf, BigInteger a) => cf / (a, 1);

  /// <summary>Divides a <see cref="BigInteger"/> by a continued fraction.</summary>
  public static CFraction operator /(BigInteger a, CFraction cf) => (a, 1) / cf;
#endregion

#region CFrac (op) CFrac
  /// <summary>
  /// Adds two continued fractions using Gosper's algorithm.
  /// </summary>
  /// <param name="cf1">The first continued fraction operand.</param>
  /// <param name="cf2">The second continued fraction operand.</param>
  /// <returns>The sum as a <see cref="CFraction"/>.</returns>
  /// <remarks>
  /// Handles infinities: Inf + X = Inf, X + Inf = Inf, Inf + Inf = Inf.
  /// Uses <see cref="Transform2_main"/> with the addition matrix from <see cref="GosperMatrix"/>.
  /// </remarks>
  public static CFraction operator +(CFraction cf1, CFraction cf2) {
    bool cf1IsInf = cf1.IsInfinity();
    bool cf2IsInf = cf2.IsInfinity();

    if (cf1IsInf || cf2IsInf) {
      return Infinity;
    }

    return cf1.Transform2(cf2, GosperMatrix.Addition());
  }

  /// <summary>
  /// Subtracts one continued fraction from another using Gosper's algorithm.
  /// </summary>
  /// <param name="cf1">The continued fraction minuend.</param>
  /// <param name="cf2">The continued fraction subtrahend.</param>
  /// <returns>The difference as a <see cref="CFraction"/>.</returns>
  /// <remarks>
  /// Handles infinities:
  /// - Inf - X = Inf
  /// - X - Inf is conceptually -Inf. Since -Inf is not represented, this currently returns <see cref="Infinity"/>.
  /// - Inf - Inf is indeterminate and throws an <see cref="ArgumentException"/>.
  /// Uses <see cref="Transform2_main"/> with the subtraction matrix from <see cref="GosperMatrix"/>.
  /// </remarks>
  /// <exception cref="ArgumentException">Thrown for the indeterminate form Inf - Inf.</exception>
  public static CFraction operator -(CFraction cf1, CFraction cf2) {
    bool cf1IsInf = cf1.IsInfinity();
    bool cf2IsInf = cf2.IsInfinity();

    if (cf1IsInf && cf2IsInf) {
      throw new ArgumentException("Indeterminate form: Cannot subtract Infinity from Infinity.");
    }
    if (cf1IsInf) { // Inf - X = Inf
      return Infinity;
    }
    if (cf2IsInf) { // X - Inf = Inf
      return Infinity;
    }

    return cf1.Transform2(cf2, GosperMatrix.Subtraction());
  }

  /// <summary>
  /// Multiplies two continued fractions using Gosper's algorithm.
  /// </summary>
  /// <param name="cf1">The first continued fraction operand.</param>
  /// <param name="cf2">The second continued fraction operand.</param>
  /// <returns>The product as a <see cref="CFraction"/>.</returns>
  /// <remarks>
  /// Handles infinities and zeros:
  /// - Inf * 0 or 0 * Inf is indeterminate and throws an <see cref="ArgumentException"/>.
  /// - Inf * X (X != 0) = Inf.
  /// - X * Inf (X != 0) = Inf.
  /// - 0 * X = 0.
  /// - X * 0 = 0.
  /// Uses <see cref="Transform2_main"/> with the multiplication matrix from <see cref="GosperMatrix"/>.
  /// </remarks>
  /// <exception cref="ArgumentException">Thrown for the indeterminate form Inf * 0.</exception>
  public static CFraction operator *(CFraction cf1, CFraction cf2) {
    bool cf1IsInf  = cf1.IsInfinity();
    bool cf2IsInf  = cf2.IsInfinity();
    bool cf1IsZero = cf1.Equals(Zero);
    bool cf2IsZero = cf2.Equals(Zero);

    if ((cf1IsInf && cf2IsZero) || (cf1IsZero && cf2IsInf)) {
      //Inf * 0 or 0 * Inf
      throw new ArgumentException("Indeterminate form: Cannot multiply Infinity by Zero.");
    }
    if (cf1IsInf || cf2IsInf) { // Inf * X (X!=0) = Inf, X * Inf (X!=0) = Inf
      return Infinity;
    }
    if (cf1IsZero || cf2IsZero) { // 0 * X = 0, X * 0 = 0
      return Zero;
    }

    return cf1.Transform2(cf2, GosperMatrix.Multiplication());
  }

  /// <summary>
  /// Divides one continued fraction by another using Gosper's algorithm.
  /// </summary>
  /// <param name="cf1">The continued fraction dividend.</param>
  /// <param name="cf2">The continued fraction divisor.</param>
  /// <returns>The quotient as a <see cref="CFraction"/>.</returns>
  /// <remarks>
  /// Handles infinities and zeros:
  /// - X / 0 throws <see cref="DivideByZeroException"/>.
  /// - Inf / Inf is indeterminate and throws <see cref="ArgumentException"/>.
  /// - 0 / X (X != 0) = 0.
  /// - X / Inf (X != 0, X != Inf) = 0.
  /// - Inf / X (X != 0, X != Inf) = Inf.
  /// Uses <see cref="Transform2_main"/> with the division matrix from <see cref="GosperMatrix"/>.
  /// </remarks>
  /// <exception cref="DivideByZeroException">Thrown if the divisor <paramref name="cf2"/> is zero.</exception>
  /// <exception cref="ArgumentException">Thrown for the indeterminate form Inf / Inf.</exception>
  public static CFraction operator /(CFraction cf1, CFraction cf2) {
    bool cf1IsInf  = cf1.IsInfinity();
    bool cf2IsInf  = cf2.IsInfinity();
    bool cf1IsZero = cf1.Equals(Zero);
    bool cf2IsZero = cf2.Equals(Zero);

    if (cf2IsZero) {
      throw new DivideByZeroException("Division by zero: Cannot divide CFraction by zero CFraction.");
    }
    if (cf1IsInf && cf2IsInf) { // Inf / Inf
      throw new ArgumentException("Indeterminate form: Cannot divide Infinity by Infinity.");
    }
    if (cf1IsZero) { // 0 / X (X!=0) = 0
      return Zero;
    }
    if (cf2IsInf) { // X / Inf (X finite, X!=0) = 0
      return Zero;
    }
    if (cf1IsInf) { // Inf / X (X finite, X!=0) = Inf
      return Infinity;
    }

    return cf1.Transform2(cf2, GosperMatrix.Division());
  }
#endregion

#region Custom CFrac operations
  /// <summary>
  /// Applies a linear fractional transformation (LFT) to the continued fraction.
  /// Calculates f(x) = (a*x + b) / (c*x + d), where x is the current continued fraction.
  /// </summary>
  /// <param name="a">Coefficient 'a' of the LFT.</param>
  /// <param name="b">Coefficient 'b' of the LFT.</param>
  /// <param name="c">Coefficient 'c' of the LFT.</param>
  /// <param name="d">Coefficient 'd' of the LFT.</param>
  /// <returns>A new <see cref="CFraction"/> representing the result of the transformation.</returns>
  /// <remarks>Uses the <see cref="Transform_main"/> helper method with a <see cref="Matrix22"/> representation.</remarks>
  public CFraction Transform(BigInteger a, BigInteger b, BigInteger c, BigInteger d)
    => new CFraction(Transform_main(this, new Matrix22(a, b, c, d)));

  /// <summary>
  /// Applies a binary operation represented by an initial Gosper matrix to this continued fraction and another one.
  /// This is a more direct way to call the underlying Gosper algorithm implementation.
  /// </summary>
  /// <param name="cf2">The second continued fraction operand.</param>
  /// <param name="initialMatrix">The <see cref="GosperMatrix"/> defining the binary operation (e.g., from <see cref="GosperMatrix.Addition()"/>).</param>
  /// <returns>A new <see cref="CFraction"/> representing the result of the operation.</returns>
  /// <remarks>Uses the <see cref="Transform2_main"/> helper method.</remarks>
  public CFraction Transform2(CFraction cf2, GosperMatrix initialMatrix)
    => new CFraction(Transform2_main(this, cf2, initialMatrix));
#endregion

  /// <summary>
  /// Core implementation of Gosper's algorithm for binary operations on two continued fractions (cf1, cf2).
  /// Generates the coefficients of the resulting continued fraction.
  /// </summary>
  /// <param name="cf1">The first continued fraction operand.</param>
  /// <param name="cf2">The second continued fraction operand.</param>
  /// <param name="initialMatrix">The <see cref="GosperMatrix"/> defining the operation Z(x,y).</param>
  /// <returns>An <see cref="IEnumerable{T}"/> yielding the <see cref="BigInteger"/> coefficients of the resulting continued fraction.</returns>
  private static IEnumerable<BigInteger> Transform2_main(CFraction cf1, CFraction cf2, GosperMatrix initialMatrix) {
    GosperMatrix matrix = initialMatrix;

    // --- Initialization Step ---
    // Ingest the integer parts (a0, b0) if they exist.
    BigInteger? initialTerm1 = cf1[0];
    if (initialTerm1.HasValue)
      matrix = matrix.IngestX(initialTerm1.Value);
    BigInteger? initialTerm2 = cf2[0];
    if (initialTerm2.HasValue)
      matrix = matrix.IngestY(initialTerm2.Value);

    // Indices to track the next coefficient to consume from each fraction (start from a1, b1)
    int index1 = 1;
    int index2 = 1;

    // --- Safety fuse against potential hangs (heuristic) ---
    int       consecutiveConsumeWithoutProduce = 0;
    const int MAX_CONSUME_WITHOUT_PRODUCE      = 50;

    // --- Main Loop ---
    while (true) {
      bool producedTerm = false;
      while (matrix.TryGetNextTerm(out BigInteger? q)) {
        yield return q.Value;

        matrix                           = matrix.Produce(q.Value);
        producedTerm                     = true;
        consecutiveConsumeWithoutProduce = 0;
      }

      // --- Safety Fuse Check ---
      if (!producedTerm) {
        consecutiveConsumeWithoutProduce++;
        if (consecutiveConsumeWithoutProduce > MAX_CONSUME_WITHOUT_PRODUCE) {
          // Output the rational approximation of the current matrix state (A/E) as the final part.
          Debug.WriteLine
            (
             $"Warning: CFraction binary operation terminated by safety fuse after {MAX_CONSUME_WITHOUT_PRODUCE} consecutive consumes without producing output. Matrix: {matrix}"
            );

          // As x,y -> inf, Z -> A/E. Generate the CF for A/E.
          BigInteger finalNum = matrix.A;
          BigInteger finalDen = matrix.E;
          if (finalDen != 0) {
            foreach (BigInteger finalTerm in RationalGenerator(finalNum, finalDen)) {
              yield return finalTerm;
            }
          }

          break;
        }
      }

      // 2. Consume Phase: Request next term(s) from input fractions if needed.
      BigInteger? term1 = cf1[index1];
      BigInteger? term2 = cf2[index2];

      bool consumedThisTurn = false;
      if (term1.HasValue) {
        matrix = matrix.IngestX(term1.Value);
        index1++;
        consumedThisTurn = true;
      }
      if (term2.HasValue) {
        matrix = matrix.IngestY(term2.Value);
        index2++;
        consumedThisTurn = true;
      }

      // 3. Termination Check (Normal):
      if (!producedTerm && !consumedThisTurn) {
        BigInteger finalNum = matrix.A;
        BigInteger finalDen = matrix.E;
        if (finalDen != 0) {
          foreach (BigInteger finalTerm in RationalGenerator(finalNum, finalDen)) {
            yield return finalTerm;
          }
        }

        break;
      }
    }
  }

  /// <summary>
  /// Core implementation for applying a Linear Fractional Transformation (LFT) represented by a matrix
  /// to a single continued fraction. Generates the coefficients of the resulting continued fraction.
  /// </summary>
  /// <param name="cf">The input continued fraction.</param>
  /// <param name="init">The initial <see cref="Matrix22"/> representing the LFT f(x) = (ax+b)/(cx+d).</param>
  /// <returns>An <see cref="IEnumerable{T}"/> yielding the <see cref="BigInteger"/> coefficients of the transformed continued fraction.</returns>
  private static IEnumerable<BigInteger> Transform_main(CFraction cf, Matrix22 init) {
    Matrix22 m = init;
    int      i = 0;

    while (true) {
      BigInteger? val = cf[i];
      if (val is null) {
        break;
      }

      m *= Matrix22.Homographic(val.Value);

      // Try to produce integer terms from the updated matrix 'm'.
      foreach ((BigInteger q, Matrix22 r) step in Matrix22.GenerateLFTSteps(m)) {
        m = step.r;

        yield return step.q;
      }

      i++;
    }

    // --- Final Step for Finite Input ---
    // If the input 'cf' was finite and non-empty (i != 0), the final matrix 'm'
    // represents a rational number m[0]/m[2].
    if (i != 0 && m[2] != 0) {
      {
        foreach (BigInteger finalTerm in RationalGenerator(m[0], m[2])) {
          yield return finalTerm;
        }
      }
    }
  }

}
