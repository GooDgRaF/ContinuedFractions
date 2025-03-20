namespace Tests;

[TestFixture]
public class ArithmeticTests {

  public static CFraction cfFin1 = CFraction.FromRational(10, 7); // [1; 2, 3] = 10/7

  [Test]
  public void AddFrac_Infinity() {
    var r = CFraction.Infinity + (22,7);

    Assert.That(CFraction.FromCoeffs(Array.Empty<int>()).Equals(r), "Adding m/n to the infinity should be equals to infinity.");
  }

  [Test]
  public void AddFrac_Zero() {
    var r = cfFin1 + (0, 1);
    Assert.That(cfFin1.Equals(r), "Adding 0/1 should return the original CF");
  }

  [Test]
  public void AddFrac_1_1() {
    var r          = cfFin1 + (1, 1); // 10/7 + 1/1 = 17/7
    var expectedCf = CFraction.FromRational(17, 7);
    Assert.That
      (
       expectedCf.Equals(r)
     , $"Adding 1/1 should return 17 / 7. Found: {expectedCf.FromCF().Last().numerator} / {expectedCf.FromCF().Last().denominator}"
      );
  }

  [Test]
  public void AddFrac_1_2() {
    var r          = cfFin1 + (1, 2); // 10/7 + 1/2 = 27/14
    var expectedCf = CFraction.FromRational(27, 14);
    Assert.That(expectedCf.Equals(r), "Adding 1/2");
  }

  [Test]
  public void AddFrac_Sqrt2_Plus_1() {
    var r          = CFraction.Sqrt2 + (1, 1); // Sqrt(2) + 1 = [2; 2, 2, 2, ...]
    var expectedCf = CFraction.FromGenerator(new int[] { 2 }.Concat(Enumerable.Repeat(2, 1000)));
    Assert.That(expectedCf.Equals(r), "Adding 1 to Sqrt(2)");
  }

}
