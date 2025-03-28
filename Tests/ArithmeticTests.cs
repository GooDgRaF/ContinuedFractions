namespace Tests;

[TestFixture]
public class ArithmeticTests {

  // Базовые дроби для тестов
  public static CFraction cfPosFin      = CFraction.FromRational(10, 7);  // [1; 2, 3]
  public static CFraction cfNegFin      = CFraction.FromRational(-10, 7); // [-2; 1, 1, 3]
  public static CFraction cfPosFin_7_2  = CFraction.FromRational(7, 2);   // [3; 2]
  public static CFraction cfPosFin_2_7  = CFraction.FromRational(2, 7);   // [0; 3, 2]
  public static CFraction cfPosInt_3    = CFraction.FromRational(3, 1);   // [3]
  public static CFraction cfNegFin_m7_2 = CFraction.FromRational(-7, 2);  // [-4; 2]
  public static CFraction cfNegFin_m2_7 = CFraction.FromRational(-2, 7);  // [-1; 1, 2, 2]
  public static CFraction cfNegInt_m3   = CFraction.FromRational(-3, 1);  // [-3]
  public static CFraction cfSqrt2       = CFraction.Sqrt2;                // [1; 2, 2, ...]
  public static CFraction cfE           = CFraction.E;                    // [2; 1, 2, 1, 1, 4, ...]

  public static Frac fracPos    = (3, 2);  // 3/2
  public static Frac fracNeg    = (-3, 2); // -3/2
  public static Frac fracPosBig = (22, 7); // Пример ~Пи
  public static Frac fracNegBig = (-22, 7);
  public static Frac fracZero   = (0, 1);
  public static Frac fracOne    = (1, 1);
  public static Frac fracNegOne = (-1, 1);
  public static Frac frac_10_7  = (10, 7);
  public static Frac frac_m10_7 = (-10, 7);
  public static Frac frac_1_7   = (1, 7);
  public static Frac frac_m1_7  = (-1, 7);


#region Reciprocal Tests (Rcp)
  // --- Edge Cases ---
  [Test]
  public void Rcp_Infinity() {
    var rcp        = CFraction.Infinity.Rcp();
    var expectedCf = CFraction.Zero;
    Assert.That(expectedCf.Equals(rcp), $"rcp = {rcp}\nexpected = {expectedCf}");
  }

  [Test]
  public void Rcp_Zero() {
    var rcp        = CFraction.Zero.Rcp();
    var expectedCf = CFraction.Infinity;
    Assert.That(expectedCf.Equals(rcp), $"rcp = {rcp}\nexpected = {expectedCf}");
  }

  // --- Positive Cases ---
  [Test]
  public void Rcp_Positive_GreaterThanOne() {
    var rcp        = cfPosFin_7_2.Rcp(); // Rcp(7/2) = 2/7
    var expectedCf = CFraction.FromRational(2, 7);
    Assert.That(expectedCf.Equals(rcp), $"rcp = {rcp}\nexpected = {expectedCf}");
  }

  [Test]
  public void Rcp_Positive_LessThanOne() {
    var rcp        = cfPosFin_2_7.Rcp(); // Rcp(2/7) = 7/2
    var expectedCf = CFraction.FromRational(7, 2);
    Assert.That(expectedCf.Equals(rcp), $"rcp = {rcp}\nexpected = {expectedCf}");
  }

  [Test]
  public void Rcp_Positive_Integer() {
    var rcp        = cfPosInt_3.Rcp(); // Rcp(3/1) = 1/3
    var expectedCf = CFraction.FromRational(1, 3);
    Assert.That(expectedCf.Equals(rcp), $"rcp = {rcp}\nexpected = {expectedCf}");
  }

  [Test]
  public void Rcp_Positive_Irrational_Sqrt2() {
    var rcp        = cfSqrt2.Rcp(); // Rcp([1; 2, 2...]) = [0; 1, 2, 2...]
    var expectedCf = CFraction.FromGenerator(new int[] { 0 }.Concat(cfSqrt2));
    Assert.That(expectedCf.Equals(rcp), $"rcp = {rcp}\nexpected = {expectedCf}");
  }


  // --- Negative Cases ---
  [Test]
  public void Rcp_Negative_AbsGreaterThanOne() {
    var rcp        = cfNegFin_m7_2.Rcp(); // Rcp(-7/2) = -2/7
    var expectedCf = CFraction.FromRational(-2, 7);
    Assert.That(expectedCf.Equals(rcp), $"rcp = {rcp}\nexpected = {expectedCf}");
  }

  [Test]
  public void Rcp_Negative_AbsLessThanOne() {
    var rcp        = cfNegFin_m2_7.Rcp(); // Rcp(-2/7) = -7/2
    var expectedCf = CFraction.FromRational(-7, 2);
    Assert.That(expectedCf.Equals(rcp), $"rcp = {rcp}\nexpected = {expectedCf}");
  }

  [Test]
  public void Rcp_NegativeInteger() {
    var rcp        = cfNegInt_m3.Rcp(); // Rcp(-3/1) = -1/3
    var expectedCf = CFraction.FromRational(-1, 3);
    Assert.That(expectedCf.Equals(rcp), $"rcp = {rcp}\nexpected = {expectedCf}");
  }
#endregion

#region Addition Tests (+)
  // --- Edge Cases ---
  [Test]
  public void Add_CFPos_FracZero() {
    var cf = cfPosFin + fracZero; // 10/7 + 0 = 10/7
    Assert.That(cfPosFin.Equals(cf), $"cf = {cf}\nexpected = {cfPosFin}");
  }

  [Test]
  public void Add_CFZero_FracPos() {
    var cf         = CFraction.Zero + fracPos; // 0 + 3/2 = 3/2
    var expectedCf = CFraction.FromRational(fracPos.p, fracPos.q);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_CFZero_FracZero() {
    var cf = CFraction.Zero + fracZero; // 0 + 0 = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Add_CFInfinity_FracAny() {
    var cf1 = CFraction.Infinity + fracPos;
    var cf2 = CFraction.Infinity + fracNeg;
    var cf3 = CFraction.Infinity + fracZero;
    Assert.That(CFraction.Infinity.Equals(cf1), $"cf1 = {cf1}\nexpected = {CFraction.Infinity}");
    Assert.That(CFraction.Infinity.Equals(cf2), $"cf2 = {cf2}\nexpected = {CFraction.Infinity}");
    Assert.That(CFraction.Infinity.Equals(cf3), $"cf3 = {cf3}\nexpected = {CFraction.Infinity}");
  }

  // --- Positive Cases ---
  [Test]
  public void Add_CFPos_FracPos() {
    var cf         = cfPosFin + fracPos; // 10/7 + 3/2 = 41/14
    var expectedCf = CFraction.FromRational(41, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_FracPos_CFPos() // Проверка коммутативности
  {
    var cf         = fracPos + cfPosFin; // 3/2 + 10/7 = 41/14
    var expectedCf = CFraction.FromRational(41, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_Sqrt2_Plus_1() {
    var cf         = cfSqrt2 + fracOne; // Sqrt(2) + 1 = [2; 2, 2, ...]
    var expectedCf = CFraction.FromGenerator(new int[] { 2 }.Concat(Enumerable.Repeat(2, 100)));
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  // --- Negative Cases ---
  [Test]
  public void Add_CFPos_FracNeg_ResultPos() {
    var cf         = cfPosFin + frac_m1_7; // 10/7 + (-1/7) = 9/7
    var expectedCf = CFraction.FromRational(9, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_CFPos_FracNeg_ResultNeg() {
    var cf         = cfPosFin + fracNeg; // 10/7 + (-3/2) = -1/14
    var expectedCf = CFraction.FromRational(-1, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_CFPos_FracNeg_ResultZero() {
    var cf         = cfPosFin + frac_m10_7; // 10/7 + (-10/7) = 0
    var expectedCf = CFraction.Zero;
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_CFNeg_FracPos_ResultPos() {
    var cf         = cfNegFin + fracPosBig; // -10/7 + 22/7 = 12/7
    var expectedCf = CFraction.FromRational(12, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_CFNeg_FracPos_ResultNeg() {
    var cf         = cfNegFin + frac_1_7; // -10/7 + 1/7 = -9/7
    var expectedCf = CFraction.FromRational(-9, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_CFNeg_FracPos_ResultZero() {
    var cf         = cfNegFin + frac_10_7; // -10/7 + 10/7 = 0
    var expectedCf = CFraction.Zero;
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_CFNeg_FracNeg() {
    var cf         = cfNegFin + fracNeg; // -10/7 + (-3/2) = -41/14
    var expectedCf = CFraction.FromRational(-41, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_CFZero_FracNeg() {
    var cf         = CFraction.Zero + fracNeg; // 0 + (-3/2) = -3/2
    var expectedCf = CFraction.FromRational(fracNeg.p, fracNeg.q);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_FracNeg_CFZero() // Commutativity check
  {
    var cf         = fracNeg + CFraction.Zero; // (-3/2) + 0 = -3/2
    var expectedCf = CFraction.FromRational(fracNeg.p, fracNeg.q);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Add_NegSqrt2_Plus_1() {
    var cf = -cfSqrt2 + fracOne; // -Sqrt(2) + 1
    var expectedCf = CFraction.FromGenerator(new int[] { -1, 1, 1 }.Concat(Enumerable.Repeat(2, 100)));
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }
#endregion

#region Subtraction Tests (-)
  // --- Edge Cases ---
  [Test]
  public void Sub_CFPos_FracZero() {
    var cf = cfPosFin - fracZero; // 10/7 - 0 = 10/7
    Assert.That(cfPosFin.Equals(cf), $"cf = {cf}\nexpected = {cfPosFin}");
  }

  [Test]
  public void Sub_CFZero_FracPos() {
    var cf         = CFraction.Zero - fracPos; // 0 - 3/2 = -3/2
    var expectedCf = CFraction.FromRational(-fracPos.p, fracPos.q);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracPos_CFZero() {
    var cf         = fracPos - CFraction.Zero; // 3/2 - 0 = 3/2
    var expectedCf = CFraction.FromRational(fracPos.p, fracPos.q);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_CFZero_FracZero() {
    var cf = CFraction.Zero - fracZero; // 0 - 0 = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Sub_CFInfinity_FracAny() {
    var cf1 = CFraction.Infinity - fracPos;
    var cf2 = CFraction.Infinity - fracNeg;
    Assert.That(CFraction.Infinity.Equals(cf1), $"cf1 = {cf1}\nexpected = {CFraction.Infinity}");
    Assert.That(CFraction.Infinity.Equals(cf2), $"cf2 = {cf2}\nexpected = {CFraction.Infinity}");
  }

  [Test]
  public void Sub_FracAny_CFInfinity() {
    var cf1 = fracPos - CFraction.Infinity; // 3/2 - Inf = -Inf? (Impl may give Inf)
    var cf2 = fracNeg - CFraction.Infinity; // -3/2 - Inf = -Inf? (Impl may give Inf)
    // Assuming Transform gives Infinity for finite - Infinity
    Assert.That(CFraction.Infinity.Equals(cf1), $"cf1 = {cf1}\nexpected = {CFraction.Infinity}");
    Assert.That(CFraction.Infinity.Equals(cf2), $"cf2 = {cf2}\nexpected = {CFraction.Infinity}");
  }

  [Test]
  public void Sub_CFZero_FracNeg() {
    var cf         = CFraction.Zero - fracNeg; // 0 - (-3/2) = 3/2
    var expectedCf = CFraction.FromRational(-fracNeg.p, fracNeg.q);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracNeg_CFZero() {
    var cf         = fracNeg - CFraction.Zero; // -3/2 - 0 = -3/2
    var expectedCf = CFraction.FromRational(fracNeg.p, fracNeg.q);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }


  // --- Positive Cases ---
  [Test]
  public void Sub_CFPos_FracPos_ResultPos() {
    var cf         = cfPosFin - fracOne; // 10/7 - 1 = 3/7
    var expectedCf = CFraction.FromRational(3, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_CFPos_FracPos_ResultNeg() {
    var cf         = cfPosFin - fracPos; // 10/7 - 3/2 = -1/14
    var expectedCf = CFraction.FromRational(-1, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_CFPos_FracPos_ResultZero() {
    var cf         = cfPosFin - frac_10_7; // 10/7 - 10/7 = 0
    var expectedCf = CFraction.Zero;
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracPos_CFPos_ResultPos() {
    var cf         = fracPosBig - cfPosFin; // 22/7 - 10/7 = 12/7
    var expectedCf = CFraction.FromRational(12, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracPos_CFPos_ResultNeg() {
    var cf         = fracOne - cfPosFin; // 1 - 10/7 = -3/7
    var expectedCf = CFraction.FromRational(-3, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracPos_CFPos_ResultZero() {
    var cf         = frac_10_7 - cfPosFin; // 10/7 - 10/7 = 0
    var expectedCf = CFraction.Zero;
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  // --- Negative Cases ---
  [Test]
  public void Sub_CFPos_FracNeg() // Equivalent to Add_CFPos_FracPos
  {
    var cf         = cfPosFin - fracNeg; // 10/7 - (-3/2) = 41/14
    var expectedCf = CFraction.FromRational(41, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_CFNeg_FracPos() // Always negative
  {
    var cf         = cfNegFin - fracPos; // -10/7 - 3/2 = -41/14
    var expectedCf = CFraction.FromRational(-41, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracPos_CFNeg() // Equivalent to Add_FracPos_CFPos
  {
    var cf         = fracPos - cfNegFin; // 3/2 - (-10/7) = 41/14
    var expectedCf = CFraction.FromRational(41, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }


  [Test]
  public void Sub_CFNeg_FracNeg_ResultPos() {
    var cf         = cfNegFin - fracNegBig; // -10/7 - (-22/7) = 12/7
    var expectedCf = CFraction.FromRational(12, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_CFNeg_FracNeg_ResultNeg() {
    var cf         = cfNegFin - frac_m1_7; // -10/7 - (-1/7) = -9/7
    var expectedCf = CFraction.FromRational(-9, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_CFNeg_FracNeg_ResultZero() {
    var cf         = cfNegFin - frac_m10_7; // -10/7 - (-10/7) = 0
    var expectedCf = CFraction.Zero;
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracNeg_CFPos() // Always negative
  {
    var cf         = fracNeg - cfPosFin; // -3/2 - 10/7 = -41/14
    var expectedCf = CFraction.FromRational(-41, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracNeg_CFNeg_ResultPos() {
    var cf         = frac_m1_7 - cfNegFin; // -1/7 - (-10/7) = 9/7
    var expectedCf = CFraction.FromRational(9, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracNeg_CFNeg_ResultNeg() {
    var cf         = fracNeg - cfNegFin; // -3/2 - (-10/7) = -1/14
    var expectedCf = CFraction.FromRational(-1, 14);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Sub_FracNeg_CFNeg_ResultZero() {
    var cf         = frac_m10_7 - cfNegFin; // -10/7 - (-10/7) = 0
    var expectedCf = CFraction.Zero;
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }
#endregion

#region Multiplication Tests (*)
  // --- Edge Cases ---
  [Test]
  public void Mul_CFPos_FracZero() {
    var cf = cfPosFin * fracZero; // 10/7 * 0 = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Mul_CFZero_FracPos() {
    var cf = CFraction.Zero * fracPos; // 0 * 3/2 = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Mul_CFZero_FracZero() {
    var cf = CFraction.Zero * fracZero; // 0 * 0 = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Mul_CFInfinity_FracPos() {
    var cf = CFraction.Infinity * fracPos; // Inf * 3/2 = Inf
    Assert.That(CFraction.Infinity.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Infinity}");
  }

  [Test]
  public void Mul_CFInfinity_FracNeg() {
    var cf = CFraction.Infinity * fracNeg; // Inf * -3/2 = Inf
    Assert.That(CFraction.Infinity.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Infinity}");
  }

  [Test]
  public void Mul_CFInfinity_FracZero_Throws() {
    Assert.Throws<ArgumentException>
      (
       ()
         => {
         var cf = CFraction.Infinity * fracZero;
       }
       // Message remains descriptive for exceptions
     , "Mul: Infinity * Zero Frac should throw ArgumentException."
      );
  }

  // --- Positive Cases ---
  [Test]
  public void Mul_CFPos_FracPos() {
    var cf         = cfPosFin * fracPos; // (10/7) * (3/2) = 15/7
    var expectedCf = CFraction.FromRational(15, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Mul_FracPos_CFPos() // Commutativity
  {
    var cf         = fracPos * cfPosFin; // (3/2) * (10/7) = 15/7
    var expectedCf = CFraction.FromRational(15, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Mul_CFPos_FracOne() {
    var cf = cfPosFin * fracOne; // 10/7 * 1 = 10/7
    Assert.That(cfPosFin.Equals(cf), $"cf = {cf}\nexpected = {cfPosFin}");
  }

  [Test]
  public void Mul_Sqrt2_By_2() {
    var cf         = cfSqrt2 * (2, 1); // Sqrt(2) * 2
    var expectedCf = CFraction.FromGenerator(TwoSqrt2TestGenerator());
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  // --- Negative Cases ---
  [Test]
  public void Mul_CFPos_FracNeg() {
    var cf         = cfPosFin * fracNeg; // (10/7) * (-3/2) = -15/7
    var expectedCf = CFraction.FromRational(-15, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Mul_CFNeg_FracPos() {
    var cf         = cfNegFin * fracPos; // (-10/7) * (3/2) = -15/7
    var expectedCf = CFraction.FromRational(-15, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Mul_CFNeg_FracNeg() {
    var cf         = cfNegFin * fracNeg; // (-10/7) * (-3/2) = 15/7
    var expectedCf = CFraction.FromRational(15, 7);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Mul_CFZero_FracNeg() {
    var cf = CFraction.Zero * fracNeg; // 0 * (-3/2) = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Mul_CFNeg_FracZero() {
    var cf = cfNegFin * fracZero; // (-10/7) * 0 = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Mul_CFPos_FracNegOne() {
    var cf         = cfPosFin * fracNegOne; // 10/7 * (-1) = -10/7
    var expectedCf = cfNegFin;              // Should be exactly the negative counterpart
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Mul_CFNeg_FracNegOne() {
    var cf         = cfNegFin * fracNegOne; // -10/7 * (-1) = 10/7
    var expectedCf = cfPosFin;              // Should be exactly the positive counterpart
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }
#endregion

#region Division Tests (/)
  // --- Edge Cases ---
  [Test]
  public void Div_CFPos_FracZero_Throws() {
    Assert.Throws<DivideByZeroException>
      (
       ()
         => {
         var cf = cfPosFin / fracZero;
       }
       // Message remains descriptive for exceptions
     , "Div: CF / Zero Frac should throw"
      );
  }

  [Test]
  public void Div_CFZero_FracPos() {
    var cf = CFraction.Zero / fracPos; // 0 / (3/2) = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Div_CFZero_FracZero_Throws() // 0 / 0
  {
    Assert.Throws<DivideByZeroException>
      (
       ()
         => {
         var cf = CFraction.Zero / fracZero;
       }
       // Message remains descriptive for exceptions
     , "Div: Zero CF / Zero Frac should throw"
      );
  }

  [Test]
  public void Div_FracPos_CFZero_Throws() {
    Assert.Throws<DivideByZeroException>
      (
       ()
         => {
         var cf = fracPos / CFraction.Zero;
       }
       // Message remains descriptive for exceptions
     , "Div: Positive Frac / Zero CF should throw DivideByZeroException."
      );
  }

  [Test]
  public void Div_FracZero_CFPos() // 0 / (10/7) = 0
  {
    var cf = fracZero / cfPosFin; // 0 * Rcp(10/7) = 0 * 7/10 = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Div_CFInfinity_FracPos() {
    var cf = CFraction.Infinity / fracPos; // Inf / (3/2) = Inf * 2/3 = Inf
    Assert.That(CFraction.Infinity.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Infinity}");
  }

  [Test]
  public void Div_CFInfinity_FracNeg() {
    var cf = CFraction.Infinity / fracNeg; // Inf / (-3/2) = Inf * (-2/3) = Inf
    Assert.That(CFraction.Infinity.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Infinity}");
  }

  [Test]
  public void Div_CFInfinity_FracZero_Throws() // Inf / 0
  {
    Assert.Throws<DivideByZeroException>
      (
       ()
         => {
         var cf = CFraction.Infinity / fracZero;
       }
       // Message remains descriptive for exceptions
     , "Div: Infinity CF / Zero Frac should throw"
      );
  }

  // --- Positive Cases ---
  [Test]
  public void Div_CFPos_FracPos() {
    var cf         = cfPosFin / fracPos; // (10/7) / (3/2) = 20/21
    var expectedCf = CFraction.FromRational(20, 21);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Div_FracPos_CFPos() {
    var cf         = fracPos / cfPosFin; // (3/2) / (10/7) = 21/20
    var expectedCf = CFraction.FromRational(21, 20);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Div_CFPos_FracOne() {
    var cf = cfPosFin / fracOne; // 10/7 / 1 = 10/7
    Assert.That(cfPosFin.Equals(cf), $"cf = {cf}\nexpected = {cfPosFin}");
  }

  [Test]
  public void Div_FracOne_CFPos() // 1 / CF = Rcp(CF)
  {
    var cf         = fracOne / cfPosFin; // 1 / (10/7) = 7/10
    var expectedCf = cfPosFin.Rcp();
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  // --- Negative Cases ---
  [Test]
  public void Div_CFPos_FracNeg() {
    var cf         = cfPosFin / fracNeg; // (10/7) / (-3/2) = -20/21
    var expectedCf = CFraction.FromRational(-20, 21);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Div_CFNeg_FracPos() {
    var cf         = cfNegFin / fracPos; // (-10/7) / (3/2) = -20/21
    var expectedCf = CFraction.FromRational(-20, 21);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Div_CFNeg_FracNeg() {
    var cf         = cfNegFin / fracNeg; // (-10/7) / (-3/2) = 20/21
    var expectedCf = CFraction.FromRational(20, 21);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Div_FracPos_CFNeg() {
    var cf         = fracPos / cfNegFin; // (3/2) / (-10/7) = -21/20
    var expectedCf = CFraction.FromRational(-21, 20);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Div_FracNeg_CFPos() {
    var cf         = fracNeg / cfPosFin; // (-3/2) / (10/7) = -21/20
    var expectedCf = CFraction.FromRational(-21, 20);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Div_FracNeg_CFNeg() {
    var cf         = fracNeg / cfNegFin; // (-3/2) / (-10/7) = 21/20
    var expectedCf = CFraction.FromRational(21, 20);
    Assert.That(expectedCf.Equals(cf), $"cf = {cf}\nexpected = {expectedCf}");
  }

  [Test]
  public void Div_CFZero_FracNeg() {
    var cf = CFraction.Zero / fracNeg; // 0 / (-3/2) = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }

  [Test]
  public void Div_FracZero_CFNeg() // 0 / (-10/7) = 0
  {
    var cf = fracZero / cfNegFin; // 0 * Rcp(-10/7) = 0 * (-7/10) = 0
    Assert.That(CFraction.Zero.Equals(cf), $"cf = {cf}\nexpected = {CFraction.Zero}");
  }
#endregion

#region Generators for Tests (Helper)
  // 2*Sqrt(2) = [2; 1, 4, 1, 4, ...]
  private static IEnumerable<int> TwoSqrt2TestGenerator() {
    yield return 2;

    while (true) {
      yield return 1;
      yield return 4;
    }
  }
#endregion

}
