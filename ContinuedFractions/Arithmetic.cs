using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ContinuedFractions;

public partial class CFraction : IUnaryNegationOperators<CFraction, CFraction>
                               , IUnaryPlusOperators<CFraction, CFraction>
                               , IAdditionOperators<CFraction, CFraction, CFraction>
                               , ISubtractionOperators<CFraction, CFraction, CFraction>
                               , IMultiplyOperators<CFraction, CFraction, CFraction>
                               , IDivisionOperators<CFraction, CFraction, CFraction> {

  private static readonly BigInteger[] zeroForRcp = new BigInteger[] { 0 };

#region Unary operators
  public CFraction Rcp() { //reciprocal
    if (this[0] is null)
      return FromRational(0, 1); // 1/inf --> 0

    if (this[0] == 0)
      return FromGenerator(this.Skip(1)); // 1/[0;a0,a1,..] --> [a0;a1,...]

    if (this[0] > 0)
      return FromGenerator(zeroForRcp.Concat(this)); // 1/[a0;a1,...] --> [0;a0,a1,..]

    if (this[0] < 0)
      return -(-this).Rcp(); // todo: можно ли лучше, чем в две промежуточные непрерывные дроби?

    throw new ArgumentException("It can not be true!");
  }

  public static CFraction operator +(CFraction cf) => cf;

  public static CFraction operator -(CFraction cf) => cf.Transform(-1, 0, 0, 1);
#endregion

#region CFrac (op) Frac
  public static CFraction operator +(CFraction cf,   Frac      frac) => cf.Transform(frac.q, frac.p, 0, frac.q);
  public static CFraction operator +(Frac      frac, CFraction cf)   => cf + frac;

  public static CFraction operator -(CFraction cf,   Frac      frac) => cf.Transform(frac.q, -frac.p, 0, frac.q);
  public static CFraction operator -(Frac      frac, CFraction cf)   => cf.Transform(-frac.q, frac.p, 0, frac.q);

  public static CFraction operator *(CFraction cf, Frac frac) {
    if (cf.Equals(Infinity) && frac.p == 0) {
      throw new ArgumentException("inf * 0!");
    }

    return cf.Transform(frac.p, 0, 0, frac.q);
  }

  public static CFraction operator *(Frac frac, CFraction cf) => cf * frac;

  public static CFraction operator /(CFraction cf, Frac frac) {
    if (frac.p == 0) {
      throw new DivideByZeroException("Division by zero in: CFraction / Frac.");
    }

    return cf.Transform(frac.q, 0, 0, frac.p);
  }

  public static CFraction operator /(Frac frac, CFraction cf) {
    if (cf.Equals(Zero)) {
      throw new DivideByZeroException("Division by zero in: Frac / CFraction.");
    }

    return frac * cf.Rcp();
  }
#endregion

#region CFrac (op) BigInteger
  public static CFraction operator +(CFraction  cf, BigInteger a)  => cf + (a, 1);
  public static CFraction operator +(BigInteger a,  CFraction  cf) => cf + a;

  public static CFraction operator -(CFraction  cf, BigInteger a)  => cf - (a, 1);
  public static CFraction operator -(BigInteger a,  CFraction  cf) => (a, 1) - cf;

  public static CFraction operator *(CFraction  cf, BigInteger a)  => cf * (a, 1);
  public static CFraction operator *(BigInteger a,  CFraction  cf) => cf * a;

  public static CFraction operator /(CFraction  cf, BigInteger a)  => cf / (a, 1);
  public static CFraction operator /(BigInteger a,  CFraction  cf) => (a, 1) / cf;
#endregion

#region CFrac (op) CFrac
  private bool IsInfinity() => Equals(Infinity);

  public static CFraction operator +(CFraction cf1, CFraction cf2) {
    // Явные проверки для сложения
    bool cf1IsInf = cf1.IsInfinity();
    bool cf2IsInf = cf2.IsInfinity();

    if (cf1IsInf || cf2IsInf) {
      // Inf + X = Inf, X + Inf = Inf, Inf + Inf = Inf
      // (Случай -Inf не рассматриваем, т.к. у нас его нет)
      return Infinity;
    }

    // Если ни один не бесконечность, выполняем алгоритм
    return FromGenerator(Transform2_main(cf1, cf2, GosperMatrix.Addition()));
  }

  public static CFraction operator -(CFraction cf1, CFraction cf2) {
    bool cf1IsInf = cf1.IsInfinity();
    bool cf2IsInf = cf2.IsInfinity();

    if (cf1IsInf && cf2IsInf) {
      // Inf - Inf не определено
      throw new ArgumentException("Cannot subtract Infinity from Infinity.");
    }
    if (cf1IsInf) { // Inf - X = Inf
      return Infinity;
    }
    if (cf2IsInf) {    // X - Inf = -Inf (у нас нет -Inf, вернем Inf?)
      return Infinity; // Или throw new NotSupportedException("Result would be negative infinity.");
    }

    return FromGenerator(Transform2_main(cf1, cf2, GosperMatrix.Subtraction()));
  }

  public static CFraction operator *(CFraction cf1, CFraction cf2) {
    bool cf1IsInf  = cf1.IsInfinity();
    bool cf2IsInf  = cf2.IsInfinity();
    bool cf1IsZero = cf1.Equals(Zero);
    bool cf2IsZero = cf2.Equals(Zero);

    if ((cf1IsInf && cf2IsZero) || (cf1IsZero && cf2IsInf)) {
      // Inf * 0 или 0 * Inf не определено
      throw new ArgumentException("Cannot multiply Infinity by Zero.");
    }
    if (cf1IsInf || cf2IsInf) { // Inf * X (X!=0) = Inf, X * Inf (X!=0) = Inf
      return Infinity;
    }
    if (cf1IsZero || cf2IsZero) { // 0 * X = 0, X * 0 = 0 (случай Inf*0 отсечен выше)
      return Zero;
    }

    return FromGenerator(Transform2_main(cf1, cf2, GosperMatrix.Multiplication()));
  }

  public static CFraction operator /(CFraction cf1, CFraction cf2) {
    bool cf1IsInf  = cf1.IsInfinity();
    bool cf2IsInf  = cf2.IsInfinity();
    bool cf1IsZero = cf1.Equals(Zero);
    bool cf2IsZero = cf2.Equals(Zero);

    if (cf2IsZero) { // Деление на ноль
      throw new DivideByZeroException("Division by zero CFraction.");
    }
    if (cf1IsInf && cf2IsInf) { // Inf / Inf не определено
      throw new ArgumentException("Cannot divide Infinity by Infinity.");
    }
    if (cf1IsZero) { // 0 / X (X!=0) = 0
      return Zero;
    }
    if (cf2IsInf) { // X / Inf (X!=0) = 0
      return Zero;
    }
    if (cf1IsInf) { // Inf / X (X!=Inf, X!=0) = Inf
      return Infinity;
    }

    // Обычный случай
    return FromGenerator(Transform2_main(cf1, cf2, GosperMatrix.Division()));
  }
#endregion

#region Custom CFrac operations
  public CFraction Transform(BigInteger a, BigInteger b, BigInteger c, BigInteger d)
    => new CFraction(Transform_main(this, new Matrix22(a, b, c, d)));

  public CFraction Transform2(CFraction cf2, GosperMatrix initialMatrix)
    => new CFraction(Transform2_main(this, cf2, initialMatrix));
#endregion

  private static IEnumerable<BigInteger> Transform2_main(CFraction cf1, CFraction cf2, GosperMatrix initialMatrix) {
    GosperMatrix matrix = initialMatrix;

    // --- ШАГ ИНИЦИАЛИЗАЦИИ ---
    BigInteger? initialTerm1 = cf1[0];
    if (initialTerm1.HasValue)
      matrix = matrix.IngestX(initialTerm1.Value);
    BigInteger? initialTerm2 = cf2[0];
    if (initialTerm2.HasValue)
      matrix = matrix.IngestY(initialTerm2.Value);

    int index1 = 1;
    int index2 = 1;

    // --- Предохранитель от зависания (эвристика) ---
    int       consecutiveConsumeWithoutProduce = 0;
    const int MAX_CONSUME_WITHOUT_PRODUCE      = 50;

    // --- ОСНОВНОЙ ЦИКЛ ---
    while (true) {
      bool producedTerm = false;
      // 1. Фаза Produce
      while (matrix.TryGetNextTerm(out var q)) {
        yield return (BigInteger)q;

        matrix                           = matrix.Produce((BigInteger)q);
        producedTerm                     = true;
        consecutiveConsumeWithoutProduce = 0; // Сброс счетчика при успехе
      }

      // --- Проверка предохранителя ---
      if (!producedTerm) // Увеличиваем счетчик, только если не произвели член
      {
        consecutiveConsumeWithoutProduce++;
        if (consecutiveConsumeWithoutProduce > MAX_CONSUME_WITHOUT_PRODUCE) {
          Debug.WriteLine
            (
             $"Warning: CFraction binary operation terminated after {MAX_CONSUME_WITHOUT_PRODUCE} consecutive consumes without producing output. Matrix: {matrix}"
            );

          BigInteger ap = matrix.A;
          BigInteger ep = matrix.E;
          if (ep != 0) {
            foreach (BigInteger finalTerm in RationalGenerator(ap, ep)) {
              yield return finalTerm;
            }
          }

          break; // Выход из while(true)
        }
      }

      // 2. Фаза Consume
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

      // 3. Нормальная проверка завершения:
      // Если ничего не произвели на ЭТОЙ итерации (producedTerm=false)
      // И ничего не смогли потребить (consumedThisTurn=false)
      if (!producedTerm && !consumedThisTurn) {
        // Нормальное завершение, пытаемся сгенерировать остаток ap/ep
        BigInteger ap = matrix.A;
        BigInteger ep = matrix.E;
        if (ep != 0) {
          foreach (BigInteger finalTerm in RationalGenerator(ap, ep)) {
            yield return finalTerm;
          }
        }

        break; // Выход из while(true)
      }
    } // конец while(true)
  }

  private static IEnumerable<BigInteger> Transform_main(CFraction cf, Matrix22 init) {
    Matrix22 m = init;
    int      i = 0;

    while (true) {
      BigInteger? val = cf[i];
      if (val is null) {
        break;
      }
      m *= Matrix22.Homographic((BigInteger)val);
      foreach ((BigInteger q, Matrix22 r) step in Matrix22.GenerateLFTSteps(m)) {
        m = step.r;

        yield return step.q;
      }

      i++;
    }

    // Если исходная непрерывная дробь была конечной, то надо выполнить ещё раз алгоритм Евклида.
    if (i != 0) {
      foreach (BigInteger finalTerm in RationalGenerator(m[0], m[2])) {
        yield return finalTerm;
      }
    }
  }

}
