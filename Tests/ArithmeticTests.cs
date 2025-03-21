﻿namespace Tests;

[TestFixture]
public class ArithmeticTests {

  public static CFraction cfFin1 = CFraction.FromRational(10, 7); // [1; 2, 3] = 10/7

 // #region Reciprocal Tests (Rcp)
 //
 //  [Test]
 //  public void Rcp_Infinity_ReturnsZero() {
 //    var cf = CFraction.Infinity.Rcp();
 //    var expectedCf = CFraction.FromRational(0, 1);
 //
 //    Assert.That(expectedCf.Equals(cf), "Reciprocal of Infinity should be Zero.");
 //  }
 //
 //  [Test]
 //  public void Rcp_Zero_ReturnsInfinity() {
 //    var zeroCf = CFraction.FromRational(0, 1);
 //    var cf = zeroCf.Rcp(); //
 //
 //    Assert.That(CFraction.Infinity.Equals(cf), "Reciprocal of Zero should be Infinity.");
 //  }
 //
 //
 //  [Test]
 //  public void Rcp_Fraction_LessThanOne() {
 //    var fracLessThanOne = CFraction.FromRational(1, 2); // [0; 2]
 //    var cf             = fracLessThanOne.Rcp();
 //    var expectedCf    = CFraction.FromRational(2, 1);    // [2]
 //
 //    Assert.That(expectedCf.Equals(cf), "Reciprocal of [0; 2] (1/2) should be [2] (2/1).");
 //  }
 //
 //  [Test]
 //  public void Rcp_Integer() {
 //    var integerCf   = CFraction.FromRational(2, 1);    // [2]
 //    var cf         = integerCf.Rcp();    // Теперь вызываем как метод экземпляра на integerCf
 //    var expectedCf  = CFraction.FromRational(1, 2);    // [0; 2]
 //
 //    Assert.That(expectedCf.Equals(cf), "Reciprocal of [2] (2/1) should be [0; 2] (1/2).");
 //  }
 //
 //  [Test]
 //  public void Rcp_Fraction_GreaterThanOne() {
 //    var fracGreaterThanOne = CFraction.FromRational(7, 2); // [3; 2]
 //    var cf                  = fracGreaterThanOne.Rcp(); // Теперь вызываем как метод экземпляра на fracGreaterThanOne
 //    var expectedCf         = CFraction.FromRational(2, 7);    // [0; 3, 2]
 //
 //    Assert.That(expectedCf.Equals(cf), "Reciprocal of [3; 2] (7/2) should be [0; 3, 2] (2/7).");
 //  }
 //  #endregion

#region Addition with Rational Tests
  [Test]
  public void AddFrac_Infinity() {
    var cf = CFraction.Infinity + (22, 7);

    Assert.That(CFraction.FromCoeffs(Array.Empty<int>()).Equals(cf), "Adding m/n to the infinity should be equals to infinity.");
  }

  [Test]
  public void AddFrac_Zero() {
    var cf = cfFin1 + (0, 1);
    Assert.That(cfFin1.Equals(cf), "Adding 0/1 should return the original CF");
  }

  [Test]
  public void AddFrac_1_1() {
    var cf          = cfFin1 + (1, 1); // 10/7 + 1/1 = 17/7
    var expectedCf = CFraction.FromRational(17, 7);
    Assert.That
      (
       expectedCf.Equals(cf)
     , $"Adding 1/1 should return 17 / 7. Found: {cf.ToRational().Last().numerator} / {cf.ToRational().Last().denominator}"
      );
  }

  [Test]
  public void AddFrac_1_2() {
    var cf          = cfFin1 + (1, 2); // 10/7 + 1/2 = 27/14
    var expectedCf = CFraction.FromRational(27, 14);
    Assert.That(expectedCf.Equals(cf), "Adding 1/2");
  }

  [Test]
  public void AddFrac_100_70() {
    var cf          = cfFin1 + (100, 70); // 10/7 + 100/70 = 200/70 = 20/7 =
    var expectedCf = CFraction.FromRational(20, 7);
    Assert.That
      (
       expectedCf.Equals(cf)
     , $"Adding 100/70 should return 20 / 7. Found: {cf.ToRational().Last().numerator} / {cf.ToRational().Last().denominator}"
      );
  }

  [Test]
  public void AddFrac_NonSimple() {
    var cf         = CFraction.FromRational(100, 32) + (75, 32); // 100/32 + 75/32 = 175/32
    var expectedCf = CFraction.FromRational(175, 32);
    Assert.That
      (
       expectedCf.Equals(cf)
     , $"100/32 + 75/32 = 175/32. Found: {cf.ToRational().Last().numerator} / {cf.ToRational().Last().denominator}"
      );
  }

  [Test]
  public void AddFrac_100times() {
    var cf = CFraction.FromRational(0, 100);
    for (int i = 0; i < 100; i++) {
      cf += (1,100);
    }
    var expectedCf = CFraction.FromRational(1, 1);
    Assert.That
      (
       expectedCf.Equals(cf)
     , $"0.01*100 = 1. Found: {cf.ToRational().Last().numerator} / {cf.ToRational().Last().denominator}"
      );
  }

  [Test]
  public void AddFrac_Sequential() {
    var cf = CFraction.E + (1, 2);
    _ = cf.Take(10);
    var cf1 = cf + (2, 3);
    _ = cf1.Take(20);

    var expectedCf = CFraction.E + (7,6);
    Assert.That
      (
       expectedCf.Equals(cf1)
     , $"e + 1/2 + 2/3 = e + 7/6. Found: {cf.ToRational().Last().numerator} / {cf.ToRational().Last().denominator}"
      );
  }

  [Test]
  public void AddFrac_Sqrt2_Plus_1() {
    var cf          = CFraction.Sqrt2 + (1, 1); // Sqrt(2) + 1 = [2; 2, 2, 2, ...]
    var expectedCf = CFraction.FromGenerator(new int[] { 2 }.Concat(Enumerable.Repeat(2, 1000)));
    Assert.That(expectedCf.Equals(cf), "Adding 1 to Sqrt(2)");
  }
#endregion

#region Multiplication by Rational Tests
  [Test]
  public void MultiplyFrac_Infinity() {
    var cf = CFraction.Infinity * (22, 7);
    Assert.That(CFraction.Infinity.Equals(cf), "Multiplying infinity by m/n should be infinity.");
  }

  [Test]
  public void MultiplyFrac_Zero() {
    var cf          = cfFin1 * (0, 1);
    var expectedCf = CFraction.FromRational(0, 1);
    Assert.That(expectedCf.Equals(cf), "Multiplying by 0/1 should return zero.");
  }

  [Test]
  public void MultiplyFrac_One() {
    var cf = cfFin1 * (1, 1);
    Assert.That(cfFin1.Equals(cf), "Multiplying by 1/1 should return the original CF.");
  }

  [Test]
  public void MultiplyFrac_Positive_2_1() {
    var cf          = cfFin1 * (2, 1); // 10/7 * 2/1 = 20/7 = [2; 1, 1, 6]
    var expectedCf = CFraction.FromRational(20, 7);
    Assert.That(expectedCf.Equals(cf), "Multiplying by 2/1");
  }

  [Test]
  public void MultiplyFrac_Positive_1_2() {
    var cf          = cfFin1 * (1, 2); // 10/7 * 1/2 = 10/14 = 5/7
    var expectedCf = CFraction.FromRational(5, 7);
    Assert.That(expectedCf.Equals(cf), "Multiplying by 1/2");
  }

  [Test]
  public void MultiplyFrac_Positive_3_2() {
    var cf          = cfFin1 * (3, 2); // 10/7 * 3/2 = 30/14 = 15/7 = [2; 7]
    var expectedCf = CFraction.FromRational(15, 7);
    Assert.That(expectedCf.Equals(cf), "Multiplying by 3/2 (positive fraction > 1)");
  }

  [Test]
  public void MultiplyFrac_Sqrt2_By_2() {
    var cf = CFraction.Sqrt2 * (2, 1); // Sqrt(2) * 2 = 2*Sqrt(2) = [2; 1, 4, 1, 4, 1, 4, ...]
    // Используем dedicated generator для 2*Sqrt(2) в тестах
    var expectedCf = CFraction.FromGenerator(TwoSqrt2TestGenerator());

    // Теперь используем Equals напрямую, полагаясь на comparationCut
    Assert.That(cf.Equals(expectedCf), "Multiplying Sqrt(2) by 2");
  }

#endregion



#region Generators for Tests

  // 2*Sqrt(2)
  private static IEnumerable<int> TwoSqrt2TestGenerator() {
    yield return 2;

    while (true) {
      yield return 1;
      yield return 4;
    }
  }

#endregion

}
