using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace ContinuedFractions;

public partial class CFraction : IUnaryNegationOperators<CFraction, CFraction>, IUnaryPlusOperators<CFraction, CFraction> {

  private static readonly int[] zeroForRcp = new int[] { 0 };

  public CFraction Rcp() { //reciprocal
    return this[0] switch
             {
               null => FromRational(0, 1)                     // 1/inf --> 0
             , 0    => FromGenerator(this.Skip(1))            // 1/[0;a0,a1,..] --> [a0;a1,...]
             , > 0  => FromGenerator(zeroForRcp.Concat(this)) // 1/[a0;a1,...] --> [0;a0,a1,..]
             , < 0  => -(-this).Rcp()                         // todo: можно ли лучше, чем в две промежуточные непрерывные дроби?
             };
  }

#region Unary operators
  public static CFraction operator +(CFraction cf) => cf;

  public static CFraction operator -(CFraction cf) => cf.Transform(-1, 0, 0, 1);

  // public static CFraction operator -(CFraction cf) {
  //   if (cf[0] is null) { // -inf = inf
  //     return Infinity;
  //   }
  //   int val0 = (int)cf[0]!;
  //   if (cf._cfEnumerator is null && cf._cfCashed.Count == 1) {
  //     return cf[0] == 0 ? Zero : new CFraction(new int[] { -val0 }); // -0 = 0, -a = a
  //   }
  //
  //   int val1 = (int)cf[1]!;
  //
  //   return FromGenerator(new int[] { -val0 - 1, 1, val1 - 1 }.Concat(cf.Skip(2)));
  // } // todo: подумать, лучше как сейчас через постоянное умножение, или вот так, но тогда надо решить проблему:
  // CFraction cf = CFraction.FromRational(10,7);
  // Console.WriteLine($"{cf}");                   [1;2,3]
  // Console.WriteLine($"{-cf}");                  [-2;1,1,3]
  // Console.WriteLine($"{-(-cf)}");               [1;1,0,1,3]
  // Console.WriteLine($"({-(-(-cf))})");          [-2;1,0,0,1,3]
#endregion

#region Frac
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

#region Int
  public static CFraction operator +(CFraction cf, int       a)  => cf + (a, 1);
  public static CFraction operator +(int       a,  CFraction cf) => cf + a;

  public static CFraction operator -(CFraction cf, int       a)  => cf - (a, 1);
  public static CFraction operator -(int       a,  CFraction cf) => (a, 1) - cf;

  public static CFraction operator *(CFraction cf, int       a)  => cf * (a, 1);
  public static CFraction operator *(int       a,  CFraction cf) => cf * a;

  public static CFraction operator /(CFraction cf, int       a)  => cf / (a, 1);
  public static CFraction operator /(int       a,  CFraction cf) => (a, 1) / cf;
#endregion

#region Binary Arithmetic Operators
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
    return FromGenerator(ApplyBinaryOperation(cf1, cf2, GosperMatrix.Addition()));
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
    if (cf2IsInf) { // X - Inf = -Inf (у нас нет -Inf, вернем Inf?)
      return Infinity; // Или throw new NotSupportedException("Result would be negative infinity.");
    }

    return FromGenerator(ApplyBinaryOperation(cf1, cf2, GosperMatrix.Subtraction()));
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

    return FromGenerator(ApplyBinaryOperation(cf1, cf2, GosperMatrix.Multiplication()));
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
    return FromGenerator(ApplyBinaryOperation(cf1, cf2, GosperMatrix.Division()));
  }
#endregion

  /// <summary>
  /// Perform a transformation by: (a*x + b) / (c*x + d)  where x is this continued function.
  /// </summary>
  /// <param name="a"></param>
  /// <param name="b"></param>
  /// <param name="c"></param>
  /// <param name="d"></param>
  /// <returns></returns>
  public CFraction Transform(int a, int b, int c, int d) => new CFraction(CF_transform_main(new Matrix22(a, b, c, d)));

  private CFraction IdTransform() => Transform(1, 0, 0, 1);

  private IEnumerable<int> CF_transform_main(Matrix22 init) {
    Matrix22 m = init;
    int      i = 0;

    while (true) {
      int? val = this[i];
      if (val == null) {
        break;
      }
      m = LFT_step(m, (int)val);
      foreach ((int q, Matrix22 r) gcd in GCD(m)) {
        m = gcd.r;

        yield return gcd.q;
      }

      i++;
    }

    // Если исходная непрерывная дробь была конечной, то надо выполнить ещё раз алгоритм Евклида.
    if (i != 0) {
      foreach (int finalTerm in RationalGenerator(m[0], m[2])) {
        yield return finalTerm;
      }
    }
  }

  private static Matrix22 LFT_step(Matrix22 m, int a) => m * Matrix22.Homographic(a);

  private static IEnumerable<int> GCD(int a, int b) {
    while (b != 0) {
      int q = Math.DivRem(a, b, out int r);

      yield return q;

      a = b;
      b = r;
    }
  }

  private static bool GCD_step(Matrix22 m, [NotNullWhen(true)] out int? q, [NotNullWhen(true)] out Matrix22? r) {
    if (m[2] == 0 && m[3] == 0) {
      throw new ArgumentException($"GCD_step: That series has already ended. Found: the second row of matrix is zero.");
    }

    q = null;
    r = null;

    if (m[2] == 0 || m[3] == 0) { // Функция неограниченна, недостаточно информации для определения частного
      return false;
    }

    if (m[2] != 0 && m[3] < 0) { // Есть ноль в области определения
      return false;
    }

    // todo: сделать целочисленные вычисления в дробях явно

    // Функция ограничена
    double n0 = (double)m[0] / m[2];
    double n1 = (double)m[1] / m[3];

    double v0 = Math.Min(n0, n1);
    double v1 = Math.Max(n0, n1);

    int d0 = (int)Math.Floor(v0);
    int d1 = (int)Math.Floor(v1);

    // если d1 == d0, то частное определяется однозначно,
    // если d1 отличается больше чем на 1 от d0, то ничего определённого сказать нельзя
    // если d1 == d0 + 1 и d1 == v1, то эта граница достигается только либо в 0, либо в oo; поэтому ответ d0.
    if (d1 == d0 || (d1 == d0 + 1 && Math.Abs(d1 - v1) < 1e-14)) {
      q = d0;
      r = new Matrix22(m[2], m[3], m[0] - d0 * m[2], m[1] - d0 * m[3]);

      return true;
    }

    return false;
  }

  private static IEnumerable<(int q, Matrix22 r)> GCD(Matrix22 m) {
    while (true) {
      if (m[0] == 0 && m[1] == 0) { // числитель равен нулю
        break;
      }

      if (!GCD_step(m, out int? q, out Matrix22? r)) {
        break;
      }

      yield return ((int)q, (Matrix22)r);

      m = (Matrix22)r;
    }
  }

  public static IEnumerable<int> ApplyBinaryOperation(CFraction cf1, CFraction cf2, GosperMatrix initialMatrix) {
    GosperMatrix matrix = initialMatrix;

    // --- ШАГ ИНИЦИАЛИЗАЦИИ ---
    int? initialTerm1 = cf1[0];
    if (initialTerm1.HasValue)
      matrix = matrix.IngestX(initialTerm1.Value);
    int? initialTerm2 = cf2[0];
    if (initialTerm2.HasValue)
      matrix = matrix.IngestY(initialTerm2.Value);

    int index1 = 1;
    int index2 = 1;

    // --- Предохранитель от зависания ---
    int consecutiveConsumeWithoutProduce = 0;
    // Порог можно настроить. Слишком маленький - обрежет медленно сходящиеся,
    // слишком большой - будет долго "висеть" перед остановкой.
    // Значение 50-100 кажется разумным для начала.
    const int MAX_CONSUME_WITHOUT_PRODUCE = 50;

    // --- ОСНОВНОЙ ЦИКЛ ---
    while (true) {
      bool producedTerm = false;
      // 1. Фаза Produce
      while (matrix.TryGetNextTerm(out var q)) {
        if (q < int.MinValue || q > int.MaxValue)
          throw new OverflowException($"Generated CF term '{q}' exceeds int range.");

        yield return (int)q;

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
            ); // Опционально
          // Принудительный выход с попыткой генерации остатка ap/ep
          BigInteger ap = matrix.A;
          BigInteger ep = matrix.E;
          if (ep != 0) {
            foreach (int finalTerm in RationalGenerator(ap, ep)) {
              yield return finalTerm;
            }
          }

          break; // Выход из while(true)
        }
      }

      // 2. Фаза Consume
      int? term1 = cf1[index1];
      int? term2 = cf2[index2];

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
          foreach (int finalTerm in RationalGenerator(ap, ep)) {
            yield return finalTerm;
          }
        }

        break; // Выход из while(true)
      }
    } // конец while(true)
  }

}
