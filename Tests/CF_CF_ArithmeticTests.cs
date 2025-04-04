namespace Tests;

[TestFixture]
public class CF_CF_ArithmeticTests {

  // --- Базовые дроби (как и раньше) ---
  public static readonly CFraction cfPosFin   = CFraction.FromRational(10, 7);  // [1; 2, 3]
  public static readonly CFraction cfNegFin   = CFraction.FromRational(-10, 7); // [-2; 1, 1, 3]
  public static readonly CFraction cfPosFin2  = CFraction.FromRational(2, 3);   // [0; 1, 2]
  public static readonly CFraction cfNegFin2  = CFraction.FromRational(-2, 3);  // [-1; 1, 2]
  public static readonly CFraction cfZero     = CFraction.Zero;
  public static readonly CFraction cfOne      = CFraction.One;
  public static readonly CFraction cfMinusOne = -CFraction.One;
  public static readonly CFraction cfTwo      = CFraction.FromCoeffs(new BigInteger[] { 2 });
  public static readonly CFraction cfInfinity = CFraction.Infinity; // Предполагаем, что это []
  public static readonly CFraction cfE        = CFraction.E;
  public static readonly CFraction cfSqrt2    = CFraction.Sqrt2;

  // --- Вспомогательный метод для сообщения ---
  private string FormatFailMessage(CFraction expected, CFraction actual, string operationDescription) {
    string expectedStr = expected.ToString(); // Ваш ToString по умолчанию
    string actualStr   = actual.ToString();

    return $"{operationDescription} failed.\nExpected: {expectedStr}\nActual:   {actualStr}";
  }


#region (+) Addition
  // --- Тесты с нулем ---
  [Test]
  public void Add_Zero_Zero() {
    var actual = cfZero + cfZero;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 + 0"));
  }

  [Test]
  public void Add_Zero_PosFin() {
    var actual = cfZero + cfPosFin;
    Assert.That(actual, Is.EqualTo(cfPosFin), FormatFailMessage(cfPosFin, actual, "0 + [1; 2, 3]"));
  }

  [Test]
  public void Add_PosFin_Zero() {
    var actual = cfPosFin + cfZero;
    Assert.That(actual, Is.EqualTo(cfPosFin), FormatFailMessage(cfPosFin, actual, "[1; 2, 3] + 0"));
  }

  [Test]
  public void Add_Zero_NegFin() {
    var actual = cfZero + cfNegFin;
    Assert.That(actual, Is.EqualTo(cfNegFin), FormatFailMessage(cfNegFin, actual, "0 + [-2; 1, 1, 3]"));
  }

  [Test]
  public void Add_NegFin_Zero() {
    var actual = cfNegFin + cfZero;
    Assert.That(actual, Is.EqualTo(cfNegFin), FormatFailMessage(cfNegFin, actual, "[-2; 1, 1, 3] + 0"));
  }

  [Test]
  public void Add_Zero_E() {
    var actual = cfZero + cfE;
    Assert.That(actual, Is.EqualTo(cfE), FormatFailMessage(cfE, actual, "0 + E"));
  }

  [Test]
  public void Add_E_Zero() {
    var actual = cfE + cfZero;
    Assert.That(actual, Is.EqualTo(cfE), FormatFailMessage(cfE, actual, "E + 0"));
  }


  // --- Тесты с бесконечностью ---
  // Проверяем, что Infinity представляется как [] и Equals работает ожидаемо
  [Test]
  public void Add_Infinity_Infinity() {
    var actual = cfInfinity + cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf + Inf"));
  }

  [Test]
  public void Add_Infinity_PosFin() {
    var actual = cfInfinity + cfPosFin;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf + pf"));
  }

  [Test]
  public void Add_PosFin_Infinity() {
    var actual = cfPosFin + cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "pf + Inf"));
  }

  [Test]
  public void Add_Infinity_NegFin() {
    var actual = cfInfinity + cfNegFin;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf + nf"));
  }

  [Test]
  public void Add_NegFin_Infinity() {
    var actual = cfNegFin + cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "nf + Inf"));
  }

  [Test]
  public void Add_Infinity_Zero() {
    var actual = cfInfinity + cfZero;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf + 0"));
  }

  [Test]
  public void Add_Zero_Infinity() {
    var actual = cfZero + cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "0 + Inf"));
  }

  [Test]
  public void Add_Infinity_E() {
    var actual = cfInfinity + cfE;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf + E"));
  }

  [Test]
  public void Add_E_Infinity() {
    var actual = cfE + cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "E + Inf"));
  }


  // --- Конечная + Конечная ---
  [Test]
  public void Add_PosFin_PosFin() {
    var expected = CFraction.FromRational(20, 7); // [2; 1, 6]
    var actual   = cfPosFin + cfPosFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] + [1;2,3]"));
  }

  [Test]
  public void Add_PosFin_PosFin2() {
    var expected = CFraction.FromRational(44, 21); // [2; 10, 2]
    var actual   = cfPosFin + cfPosFin2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] + [0;1,2]"));
  }

  [Test]
  public void Add_PosFin_NegFin() {
    var expected = cfZero;
    var actual   = cfPosFin + cfNegFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] + [-2;1,1,3]"));
  }

  [Test]
  public void Add_PosFin_NegFin2() {
    var expected = CFraction.FromRational(16, 21); // [0; 1, 3, 5]
    var actual   = cfPosFin + cfNegFin2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] + [-1;1,2]"));
  }

  [Test]
  public void Add_NegFin_NegFin() {
    var expected = CFraction.FromRational(-20, 7); // [-3; 7]
    var actual   = cfNegFin + cfNegFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "nf + nf"));
  }


  // --- Конечная + Бесконечная ---
  [Test]
  public void Add_One_Sqrt2() {
    var expected = CFraction.FromGenerator(Gen_2_222()); // [2; (2)]
    var actual   = cfOne + cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "1 + Sqrt2"));
  }

  private static IEnumerable<BigInteger> Gen_2_222() {
    yield return 2;

    while (true)
      yield return 2;
  }

  [Test]
  public void Add_PosFin_E() {
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             4, 6, 1, 4, 4, 443, 1, 1, 1, 1, 2, 1, 4, 1, 1, 3, 1, 1, 1, 1
           , 2, 1, 2, 4, 18, 1, 3, 1, 15, 1, 22, 12, 3, 1, 1, 4, 97, 1, 2, 1
           , 1, 3, 1
           }
        );
    var actual = cfPosFin + cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "[1;2,3] + E"));
  }

  [Test]
  public void Add_E_PosFin() { // Проверка коммутативности
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             4, 6, 1, 4, 4, 443, 1, 1, 1, 1, 2, 1, 4, 1, 1, 3, 1, 1, 1, 1
           , 2, 1, 2, 4, 18, 1, 3, 1, 15, 1, 22, 12, 3, 1, 1, 4, 97, 1, 2, 1
           , 1, 3, 1
           }
        );
    var actual = cfE + cfPosFin;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E + [1;2,3]"));
  }


  // --- Бесконечная + Бесконечная ---
  [Test]
  public void Add_Sqrt2_Sqrt2() {
    var expected = CFraction.FromGenerator(Gen_2_1414()); // [2; (1, 4)]
    var actual   = cfSqrt2 + cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "Sqrt2 + Sqrt2"));
  }

  private static IEnumerable<BigInteger> Gen_2_1414() {
    yield return 2;

    while (true) {
      yield return 1;
      yield return 4;
    }
  }

  [Test]
  public void Add_E_E() {
    // 2e = [5; 2, 3, 1, 2, 1, 1, 8, ...]
    // Генерируем ожидаемое значение с достаточной глубиной
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             5, 2, 3, 2, 3, 1, 2, 1, 3, 4, 3, 1, 4, 1, 3, 6, 3, 1, 6, 1
           , 3, 8, 3, 1, 8, 1, 3, 10, 3, 1, 10, 1, 3, 12, 3, 1, 12, 1, 3, 14
           , 3, 1, 14
           }
        );

    var actual = cfE + cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E + E"));
  }
#endregion

#region (-) Subtraction
  // --- Тесты с нулем ---
  [Test]
  public void Subtract_Zero_Zero() {
    var actual = cfZero - cfZero;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 - 0"));
  }

  [Test]
  public void Subtract_Zero_PosFin() {
    // 0 - 10/7 = -10/7
    var expected = cfNegFin; // [-2; 1, 1, 3]
    var actual   = cfZero - cfPosFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "0 - [1; 2, 3]"));
  }

  [Test]
  public void Subtract_PosFin_Zero() {
    var actual = cfPosFin - cfZero;
    Assert.That(actual, Is.EqualTo(cfPosFin), FormatFailMessage(cfPosFin, actual, "[1; 2, 3] - 0"));
  }

  [Test]
  public void Subtract_Zero_NegFin() {
    // 0 - (-10/7) = 10/7
    var expected = cfPosFin; // [1; 2, 3]
    var actual   = cfZero - cfNegFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "0 - [-2; 1, 1, 3]"));
  }

  [Test]
  public void Subtract_NegFin_Zero() {
    var actual = cfNegFin - cfZero;
    Assert.That(actual, Is.EqualTo(cfNegFin), FormatFailMessage(cfNegFin, actual, "[-2; 1, 1, 3] - 0"));
  }

  [Test]
  public void Subtract_Zero_E() {
    var expectedApprox = -CFraction.E;
    var actual         = cfZero - cfE;
    // Сравнение по конечному числу членов (как делает ваш Equals)
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "0 - E"));
  }

  [Test]
  public void Subtract_E_Zero() {
    var actual = cfE - cfZero;
    Assert.That(actual, Is.EqualTo(cfE), FormatFailMessage(cfE, actual, "E - 0"));
  }


  // --- Тесты с бесконечностью ---
  [Test]
  public void Subtract_Infinity_Infinity_Throws() {
    // Inf - Inf не определено
    Assert.Throws<ArgumentException>
      (
       ()
         => {
         var _ = cfInfinity - cfInfinity;
       }
     , "Inf - Inf should throw ArgumentException"
      );
  }

  [Test]
  public void Subtract_Infinity_PosFin() {
    // Inf - X = Inf
    var actual = cfInfinity - cfPosFin;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf - pf"));
  }

  [Test]
  public void Subtract_PosFin_Infinity() {
    // X - Inf = -Inf (но возвращаем Inf по вашей логике)
    var actual = cfPosFin - cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "pf - Inf"));
  }

  [Test]
  public void Subtract_Infinity_NegFin() {
    // Inf - X = Inf
    var actual = cfInfinity - cfNegFin;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf - nf"));
  }

  [Test]
  public void Subtract_NegFin_Infinity() {
    // X - Inf = -Inf (но возвращаем Inf по вашей логике)
    var actual = cfNegFin - cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "nf - Inf"));
  }

  [Test]
  public void Subtract_Infinity_Zero() {
    // Inf - 0 = Inf
    var actual = cfInfinity - cfZero;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf - 0"));
  }

  [Test]
  public void Subtract_Zero_Infinity() {
    // 0 - Inf = -Inf (но возвращаем Inf по вашей логике)
    var actual = cfZero - cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "0 - Inf"));
  }

  [Test]
  public void Subtract_Infinity_E() {
    // Inf - E = Inf
    var actual = cfInfinity - cfE;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf - E"));
  }

  [Test]
  public void Subtract_E_Infinity() {
    // E - Inf = -Inf (но возвращаем Inf по вашей логике)
    var actual = cfE - cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "E - Inf"));
  }


  // --- Конечная - Конечная ---
  [Test]
  public void Subtract_PosFin_PosFin() {
    // 10/7 - 10/7 = 0
    var expected = cfZero;
    var actual   = cfPosFin - cfPosFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] - [1;2,3]"));
  }

  [Test]
  public void Subtract_PosFin_PosFin2() {
    // 10/7 - 2/3 = (30 - 14) / 21 = 16/21
    var expected = CFraction.FromRational(16, 21); // [0; 1, 3, 5]
    var actual   = cfPosFin - cfPosFin2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] - [0;1,2]"));
  }

  [Test]
  public void Subtract_PosFin_NegFin() {
    // 10/7 - (-10/7) = 20/7
    var expected = CFraction.FromRational(20, 7); // [2; 1, 6]
    var actual   = cfPosFin - cfNegFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] - [-2;1,1,3]"));
  }

  [Test]
  public void Subtract_NegFin_PosFin() {
    // -10/7 - 10/7 = -20/7
    var expected = CFraction.FromRational(-20, 7); // [-3; 7]
    var actual   = cfNegFin - cfPosFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[-2;1,1,3] - [1;2,3]"));
  }

  [Test]
  public void Subtract_NegFin_NegFin() {
    // -10/7 - (-10/7) = 0
    var expected = cfZero;
    var actual   = cfNegFin - cfNegFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "nf - nf"));
  }


  // --- Конечная - Бесконечная ---
  // Генераторы для периодических дробей
  private static IEnumerable<BigInteger> Gen_m1_222() {
    yield return -1;
    yield return 1;
    yield return 1;

    while (true)
      yield return 2;
  }

  private static IEnumerable<BigInteger> Gen_0_222() {
    yield return 0;

    while (true)
      yield return 2;
  }

  [Test]
  public void Subtract_One_Sqrt2() {
    // 1 - Sqrt2 = 1 - [1; (2)] ~= -0.414... = [-1; (2)]
    var expected = CFraction.FromGenerator(Gen_m1_222());
    var actual   = cfOne - cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "1 - Sqrt2"));
  }

  [Test]
  public void Subtract_Sqrt2_One() {
    // Sqrt2 - 1 = [1; (2)] - 1 = [0; (2)]
    var expected = CFraction.FromGenerator(Gen_0_222());
    var actual   = cfSqrt2 - cfOne;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "Sqrt2 - 1"));
  }

  [Test]
  public void Subtract_PosFin_E() {
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             -2, 1, 2, 2, 4, 1, 2, 9, 8, 1, 19, 1, 1, 14, 1, 1, 12, 1, 1, 11
           , 1, 1, 1, 18, 2, 1, 2, 2, 4, 22, 1, 3, 1, 8, 1, 1, 9, 2, 1, 1
           , 3
           }
        );
    var actual = cfPosFin - cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "[1;2,3] - E"));
  }

  [Test]
  public void Subtract_E_PosFin() {
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             1, 3, 2, 4, 1, 2, 9, 8, 1, 19, 1, 1, 14, 1, 1, 12, 1, 1, 11, 1
           , 1, 1, 18, 2, 1, 2, 2, 4, 22, 1, 3, 1, 8, 1, 1, 9, 2, 1, 1, 3
           , 8
           }
        );
    var actual = cfE - cfPosFin;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E - [1;2,3]"));
  }


  // --- Бесконечная - Бесконечная ---
  [Test]
  public void Subtract_Sqrt2_Sqrt2() {
    // Sqrt2 - Sqrt2 = 0
    var expected = cfZero;
    var actual   = cfSqrt2 - cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "Sqrt2 - Sqrt2"));
  }

  [Test]
  public void Subtract_E_E() {
    // E - E = 0
    var expected = cfZero;
    var actual   = cfE - cfE;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "E - E"));
  }

  [Test]
  public void Subtract_E_Sqrt2() {
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             1, 3, 3, 2, 6, 3, 17, 1, 1, 3, 3, 1, 8, 2, 20, 2, 6, 1, 1, 2
           , 1, 38, 8, 17, 1, 1, 1, 2, 1, 2, 3, 1, 1, 1, 3, 17, 65, 1, 2, 2
           , 3, 3, 1, 21, 1, 3
           }
        );
    var actual = cfE - cfSqrt2;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E - Sqrt2"));
  }

  [Test]
  public void Subtract_Sqrt2_E() {
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             -2, 1, 2, 3, 2, 6, 3, 17, 1, 1, 3, 3, 1, 8, 2, 20, 2, 6, 1, 1
           , 2, 1, 38, 8, 17, 1, 1, 1, 2, 1, 2, 3, 1, 1, 1, 3, 17, 65, 1, 2
           , 2, 3, 3, 1, 21, 1
           }
        );
    var actual = cfSqrt2 - cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "Sqrt2 - E"));
  }
#endregion

#region (*) Multiplication
  // --- Тесты с нулем ---
  [Test]
  public void Multiply_Zero_Zero() {
    var actual = cfZero * cfZero;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 * 0"));
  }

  [Test]
  public void Multiply_Zero_PosFin() {
    var actual = cfZero * cfPosFin;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 * [1; 2, 3]"));
  }

  [Test]
  public void Multiply_PosFin_Zero() {
    var actual = cfPosFin * cfZero;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "[1; 2, 3] * 0"));
  }

  [Test]
  public void Multiply_Zero_NegFin() {
    var actual = cfZero * cfNegFin;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 * [-2; 1, 1, 3]"));
  }

  [Test]
  public void Multiply_NegFin_Zero() {
    var actual = cfNegFin * cfZero;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "[-2; 1, 1, 3] * 0"));
  }

  [Test]
  public void Multiply_Zero_E() {
    var actual = cfZero * cfE;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 * E"));
  }

  [Test]
  public void Multiply_E_Zero() {
    var actual = cfE * cfZero;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "E * 0"));
  }

  // --- Тесты с бесконечностью ---
  [Test]
  public void Multiply_Infinity_Zero_Throws() {
    // Inf * 0 не определено
    Assert.Throws<ArgumentException>
      (
       ()
         => {
         var _ = cfInfinity * cfZero;
       }
     , "Inf * 0 should throw ArgumentException"
      );
  }

  [Test]
  public void Multiply_Zero_Infinity_Throws() {
    // 0 * Inf не определено
    Assert.Throws<ArgumentException>
      (
       ()
         => {
         var _ = cfZero * cfInfinity;
       }
     , "0 * Inf should throw ArgumentException"
      );
  }

  [Test]
  public void Multiply_Infinity_Infinity() {
    // Inf * Inf = Inf
    var actual = cfInfinity * cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf * Inf"));
  }

  [Test]
  public void Multiply_Infinity_PosFin() {
    // Inf * X (X!=0) = Inf
    var actual = cfInfinity * cfPosFin;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf * pf"));
  }

  [Test]
  public void Multiply_PosFin_Infinity() {
    // X * Inf (X!=0) = Inf
    var actual = cfPosFin * cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "pf * Inf"));
  }

  [Test]
  public void Multiply_Infinity_NegFin() {
    // Inf * X (X!=0) = Inf (знак не учитываем для Infinity)
    var actual = cfInfinity * cfNegFin;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf * nf"));
  }

  [Test]
  public void Multiply_NegFin_Infinity() {
    // X * Inf (X!=0) = Inf
    var actual = cfNegFin * cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "nf * Inf"));
  }

  [Test]
  public void Multiply_Infinity_E() {
    // Inf * E = Inf
    var actual = cfInfinity * cfE;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf * E"));
  }

  [Test]
  public void Multiply_E_Infinity() {
    // E * Inf = Inf
    var actual = cfE * cfInfinity;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "E * Inf"));
  }


  // --- Конечная * Конечная ---
  [Test]
  public void Multiply_PosFin_PosFin() {
    // (10/7) * (10/7) = 100/49
    var expected = CFraction.FromRational(100, 49); // [2; 24, 2]
    var actual   = cfPosFin * cfPosFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] * [1;2,3]"));
  }

  [Test]
  public void Multiply_PosFin_PosFin2() {
    // (10/7) * (2/3) = 20/21
    var expected = CFraction.FromRational(20, 21); // [0; 1, 20]
    var actual   = cfPosFin * cfPosFin2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] * [0;1,2]"));
  }

  [Test]
  public void Multiply_PosFin_NegFin() {
    // (10/7) * (-10/7) = -100/49
    var expected = CFraction.FromRational(-100, 49); // [-3; 1, 23, 2]
    var actual   = cfPosFin * cfNegFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] * [-2;1,1,3]"));
  }

  [Test]
  public void Multiply_NegFin_PosFin() {             // Commutativity
    var expected = CFraction.FromRational(-100, 49); // [-3; 1, 23, 2]
    var actual   = cfNegFin * cfPosFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[-2;1,1,3] * [1;2,3]"));
  }

  [Test]
  public void Multiply_NegFin_NegFin() {
    // (-10/7) * (-10/7) = 100/49
    var expected = CFraction.FromRational(100, 49); // [2; 24, 2]
    var actual   = cfNegFin * cfNegFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "nf * nf"));
  }


  // --- Конечная * Бесконечная ---
  [Test]
  public void Multiply_One_Sqrt2() {
    // 1 * Sqrt2 = Sqrt2
    var expected = cfSqrt2;
    var actual   = cfOne * cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "1 * Sqrt2"));
  }

  [Test]
  public void Multiply_Two_Sqrt2() {
    // 2 * Sqrt2 ~= 2.828... = [2; (1, 4)]
    var expected = CFraction.FromGenerator(Gen_2_1414());
    var actual   = cfTwo * cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "2 * Sqrt2"));
  }

  [Test]
  public void Multiply_PosFin_E() {
    // (10/7) * e ~= 1.42857 * 2.71828 = 3.8832... = [3; 1, 7, 1, 1, 1, 1, ...]
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             3, 1, 7, 1, 1, 3, 3, 2, 30, 2, 10, 1, 1, 1, 1, 1, 5, 51, 4, 1
           , 2, 14, 2, 1, 3, 1, 1, 1, 2, 4, 2, 4, 12, 4, 6, 34, 1, 9, 1, 2
           , 1, 14
           }
        );
    var actual = cfPosFin * cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "[1;2,3] * E"));
  }

  [Test]
  public void Multiply_E_PosFin() { // Commutativity
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             3, 1, 7, 1, 1, 3, 3, 2, 30, 2, 10, 1, 1, 1, 1, 1, 5, 51, 4, 1
           , 2, 14, 2, 1, 3, 1, 1, 1, 2, 4, 2, 4, 12, 4, 6, 34, 1, 9, 1, 2
           , 1, 14
           }
        );
    var actual = cfE * cfPosFin;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E * [1;2,3]"));
  }

  [Test]
  public void Multiply_NegFin_E() {
    // (-10/7) * e ~= -3.8832... = [-4; 8, 1, 1, ...]
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             -4, 8, 1, 1, 3, 3, 2, 30, 2, 10, 1, 1, 1, 1, 1, 5, 51, 4, 1, 2
           , 14, 2, 1, 3, 1, 1, 1, 2, 4, 2, 4, 12, 4, 6, 34, 1, 9, 1, 2, 1
           , 14, 1, 2, 2, 4, 11
           }
        );
    var actual = cfNegFin * cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "[-2;1,1,3] * E"));
  }


  // --- Бесконечная * Бесконечная ---
  [Test]
  public void Multiply_Sqrt2_Sqrt2() { // BIG TROUBLE!
    // Sqrt2 * Sqrt2 = 2
    var expected = cfTwo; // [2]
    var actual   = cfSqrt2 * cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "Sqrt2 * Sqrt2"));
  }

  [Test]
  public void Multiply_Sqrt2_Sqrt2_Approx() {
    // Sqrt2 * Sqrt2 = 2
    var expected = 2;
    var actual   = cfSqrt2 * cfSqrt2;
    Assert.That((double)actual, Is.EqualTo(expected), "cfSqrt2 * cfSqrt2 ~= 2");
  }

  [Test]
  public void Multiply_E_E() {
    // e * e = e^2 ~= 7.389... = [7; 2, 1, 1, 3, 18, 5, ...]
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             7, 2, 1, 1, 3, 18, 5, 1, 1, 6, 30, 8, 1, 1, 9, 42, 11, 1, 1, 12
           , 54, 14, 1, 1, 15, 66, 17, 1, 1, 18, 78, 20, 1, 1, 21, 90, 23, 1, 1, 24
           , 102
           }
        );
    var actual = cfE * cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E * E"));
  }

  [Test]
  public void Multiply_E_Sqrt2() {
    // e * Sqrt2 ~= 2.71828 * 1.41421 = 3.844... = [3; 1, 5, 1, 1, 3, ...]
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             3, 1, 5, 2, 2, 1, 1, 1, 1, 1, 1, 13, 1, 1, 1, 94, 1, 9, 1, 1
           , 1, 2, 2, 3, 1, 4, 1, 7, 31, 1, 3, 1, 4, 1, 83, 15, 1, 2, 1, 3
           , 3, 1, 4, 2, 104
           }
        );
    var actual = cfE * cfSqrt2;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E * Sqrt2"));
  }

  [Test]
  public void Multiply_Sqrt2_E() { // Commutativity
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             3, 1, 5, 2, 2, 1, 1, 1, 1, 1, 1, 13, 1, 1, 1, 94, 1, 9, 1, 1
           , 1, 2, 2, 3, 1, 4, 1, 7, 31, 1, 3, 1, 4, 1, 83, 15, 1, 2, 1, 3
           , 3, 1, 4, 2, 104
           }
        );
    var actual = cfSqrt2 * cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "Sqrt2 * E"));
  }
#endregion

#region (/) Division
  // --- Тесты с нулем ---
  [Test]
  public void Divide_Zero_PosFin() {
    // 0 / X (X!=0) = 0
    var actual = cfZero / cfPosFin;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 / [1; 2, 3]"));
  }

  [Test]
  public void Divide_Zero_NegFin() {
    // 0 / X (X!=0) = 0
    var actual = cfZero / cfNegFin;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 / [-2; 1, 1, 3]"));
  }

  [Test]
  public void Divide_Zero_E() {
    // 0 / E = 0
    var actual = cfZero / cfE;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 / E"));
  }

  [Test]
  public void Divide_PosFin_Zero_Throws() {
    // X / 0
    Assert.Throws<DivideByZeroException>
      (
       ()
         => {
         var _ = cfPosFin / cfZero;
       }
     , "pf / 0 should throw DivideByZeroException"
      );
  }

  [Test]
  public void Divide_NegFin_Zero_Throws() {
    // X / 0
    Assert.Throws<DivideByZeroException>
      (
       ()
         => {
         var _ = cfNegFin / cfZero;
       }
     , "nf / 0 should throw DivideByZeroException"
      );
  }

  [Test]
  public void Divide_E_Zero_Throws() {
    // E / 0
    Assert.Throws<DivideByZeroException>
      (
       ()
         => {
         var _ = cfE / cfZero;
       }
     , "E / 0 should throw DivideByZeroException"
      );
  }

  [Test]
  public void Divide_Zero_Zero_Throws() {
    // 0 / 0
    Assert.Throws<DivideByZeroException>
      (
       ()
         => {
         var _ = cfZero / cfZero;
       }
     , "0 / 0 should throw DivideByZeroException"
      );
  }

  // --- Тесты с бесконечностью ---
  [Test]
  public void Divide_Infinity_Infinity_Throws() {
    // Inf / Inf не определено
    Assert.Throws<ArgumentException>
      (
       ()
         => {
         var _ = cfInfinity / cfInfinity;
       }
     , "Inf / Inf should throw ArgumentException"
      );
  }

  [Test]
  public void Divide_Infinity_PosFin() {
    // Inf / X (X!=0, X!=Inf) = Inf
    var actual = cfInfinity / cfPosFin;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf / pf"));
  }

  [Test]
  public void Divide_PosFin_Infinity() {
    // X / Inf (X!=0) = 0
    var actual = cfPosFin / cfInfinity;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "pf / Inf"));
  }

  [Test]
  public void Divide_Infinity_NegFin() {
    // Inf / X (X!=0, X!=Inf) = Inf
    var actual = cfInfinity / cfNegFin;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf / nf"));
  }

  [Test]
  public void Divide_NegFin_Infinity() {
    // X / Inf (X!=0) = 0
    var actual = cfNegFin / cfInfinity;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "nf / Inf"));
  }

  [Test]
  public void Divide_Infinity_Zero_Throws() {
    // Inf / 0
    Assert.Throws<DivideByZeroException>
      (
       ()
         => {
         var _ = cfInfinity / cfZero;
       }
     , "Inf / 0 should throw DivideByZeroException"
      );
  }

  [Test]
  public void Divide_Zero_Infinity() {
    // 0 / Inf = 0
    var actual = cfZero / cfInfinity;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "0 / Inf"));
  }

  [Test]
  public void Divide_Infinity_E() {
    // Inf / E = Inf
    var actual = cfInfinity / cfE;
    Assert.That(actual, Is.EqualTo(cfInfinity), FormatFailMessage(cfInfinity, actual, "Inf / E"));
  }

  [Test]
  public void Divide_E_Infinity() {
    // E / Inf = 0
    var actual = cfE / cfInfinity;
    Assert.That(actual, Is.EqualTo(cfZero), FormatFailMessage(cfZero, actual, "E / Inf"));
  }


  // --- Конечная / Конечная ---
  [Test]
  public void Divide_PosFin_PosFin() {
    // (10/7) / (10/7) = 1
    var expected = cfOne; // [1]
    var actual   = cfPosFin / cfPosFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] / [1;2,3]"));
  }

  [Test]
  public void Divide_PosFin_PosFin2() {
    // (10/7) / (2/3) = (10/7) * (3/2) = 30/14 = 15/7
    var expected = CFraction.FromRational(15, 7); // [2; 7]
    var actual   = cfPosFin / cfPosFin2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] / [0;1,2]"));
  }

  [Test]
  public void Divide_PosFin2_PosFin() {
    // (2/3) / (10/7) = (2/3) * (7/10) = 14/30 = 7/15
    var expected = CFraction.FromRational(7, 15); // [0; 2, 7]
    var actual   = cfPosFin2 / cfPosFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[0;1,2] / [1;2,3]"));
  }

  [Test]
  public void Divide_PosFin_NegFin() {
    // (10/7) / (-10/7) = -1
    var expected = cfMinusOne; // [-1]
    var actual   = cfPosFin / cfNegFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[1;2,3] / [-2;1,1,3]"));
  }

  [Test]
  public void Divide_NegFin_PosFin() {
    // (-10/7) / (10/7) = -1
    var expected = cfMinusOne; // [-1]
    var actual   = cfNegFin / cfPosFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "[-2;1,1,3] / [1;2,3]"));
  }

  [Test]
  public void Divide_NegFin_NegFin() {
    // (-10/7) / (-10/7) = 1
    var expected = cfOne; // [1]
    var actual   = cfNegFin / cfNegFin;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "nf / nf"));
  }


  // --- Конечная / Бесконечная ---
  [Test]
  public void Divide_One_Sqrt2() {
    // 1 / Sqrt2 = Sqrt2 / 2 ~= 0.707... = [0; 1, (2)]
    var expected = cfSqrt2.Rcp();
    var actual   = cfOne / cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "1 / Sqrt2"));
  }

  [Test]
  public void Divide_Two_Sqrt2() {
    // 2 / Sqrt2 = Sqrt2 = [1; (2)]
    var expected = cfSqrt2;
    var actual   = cfTwo / cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "2 / Sqrt2"));
  }

  [Test]
  public void Divide_PosFin_E() {
    // (10/7) / e ~= 1.42857 / 2.71828 = 0.5255...
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             0, 1, 1, 9, 3, 2, 9, 2, 5, 1, 6, 2, 5, 5, 5, 2, 1, 1, 5, 24
           , 1, 12, 2, 1, 3, 1, 3, 1, 1, 15, 1, 1, 3, 1, 7, 1, 3, 1, 1, 1
           , 3, 1, 8, 34, 1, 17
           }
        );
    var actual = cfPosFin / cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "[1;2,3] / E"));
  }

  [Test]
  public void Divide_NegFin_E() {
    // (-10/7) / e ~= -0.5255... = [-1; 1, 10, 1, 1, ...]
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             -1, 2, 9, 3, 2, 9, 2, 5, 1, 6, 2, 5, 5, 5, 2, 1, 1, 5, 24, 1
           , 12, 2, 1, 3, 1, 3, 1, 1, 15, 1, 1, 3, 1, 7, 1, 3, 1, 1, 1, 3
           , 1, 8, 34, 1, 17, 23
           }
        );
    var actual = cfNegFin / cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "[-2;1,1,3] / E"));
  }

  // --- Бесконечная / Конечная ---
  [Test]
  public void Divide_Sqrt2_One() {
    // Sqrt2 / 1 = Sqrt2
    var expected = cfSqrt2;
    var actual   = cfSqrt2 / cfOne;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "Sqrt2 / 1"));
  }

  [Test]
  public void Divide_Sqrt2_Two() {
    // Sqrt2 / 2 = [0; 1, (2)]
    var expected = cfSqrt2.Rcp();
    var actual   = cfSqrt2 / cfTwo;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "Sqrt2 / 2"));
  }

  [Test]
  public void Divide_E_PosFin() {
    // e / (10/7) = (7 * e) / 10 ~= 1.90279... = [1; 1, 9, 1, 1, ...]
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             1, 1, 9, 3, 2, 9, 2, 5, 1, 6, 2, 5, 5, 5, 2, 1, 1, 5, 24, 1
           , 12, 2, 1, 3, 1, 3, 1, 1, 15, 1, 1, 3, 1, 7, 1, 3, 1, 1, 1, 3
           , 1, 8, 34, 1, 17, 23
           }
        );
    var actual = cfE / cfPosFin;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E / [1;2,3]"));
  }

  [Test]
  public void Divide_E_NegFin() {
    // e / (-10/7) = -(7 * e) / 10 ~= -1.90279... = [-2; 10, 1, 1, ...]
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             -2, 10, 3, 2, 9, 2, 5, 1, 6, 2, 5, 5, 5, 2, 1, 1, 5, 24, 1, 12
           , 2, 1, 3, 1, 3, 1, 1, 15, 1, 1, 3, 1, 7, 1, 3, 1, 1, 1, 3, 1
           , 8, 34, 1, 17, 23, 18
           }
        );
    var actual = cfE / cfNegFin;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E / [-2;1,1,3]"));
  }


  // --- Бесконечная / Бесконечная ---
  [Test]
  public void Divide_Sqrt2_Sqrt2() {
    // Sqrt2 / Sqrt2 = 1
    var expected = cfOne;
    var actual   = cfSqrt2 / cfSqrt2;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "Sqrt2 / Sqrt2"));
  }

  [Test]
  public void Divide_E_E() {
    // E / E = 1
    var expected = cfOne;
    var actual   = cfE / cfE;
    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, "E / E"));
  }

  [Test]
  public void Divide_E_Sqrt2() {
    // e / Sqrt2 ~= 2.71828 / 1.41421 = 1.9221...
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             1, 1, 11, 1, 5, 4, 3, 6, 1, 4, 1, 46, 1, 20, 3, 1, 4, 1, 1, 10
           , 1, 3, 63, 1, 1, 2, 2, 2, 41, 1, 1, 7, 2, 1, 2, 1, 1, 1, 1, 2
           , 2, 4, 52, 6, 3, 5
           }
        );
    var actual = cfE / cfSqrt2;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "E / Sqrt2"));
  }

  [Test]
  public void Divide_Sqrt2_E() {
    // Sqrt2 / e ~= 1.41421 / 2.71828 = 0.5202...
    var expectedApprox =
      CFraction.FromCoeffs
        (
         new BigInteger[]
           {
             0, 1, 1, 11, 1, 5, 4, 3, 6, 1, 4, 1, 46, 1, 20, 3, 1, 4, 1, 1
           , 10, 1, 3, 63, 1, 1, 2, 2, 2, 41, 1, 1, 7, 2, 1, 2, 1, 1, 1, 1
           , 2, 2, 4, 52, 6, 3
           }
        );
    var actual = cfSqrt2 / cfE;
    Assert.That(actual, Is.EqualTo(expectedApprox), FormatFailMessage(expectedApprox, actual, "Sqrt2 / E"));
  }
#endregion

}
