namespace Tests;

[TestFixture]
public class ComparationTests {

  private static readonly CFraction r001 = CFraction.FromRational(1, 1); // 1   = [1]
  private static readonly CFraction r010 = CFraction.FromRational(1, 2); // 1/2 = [0;2]
  private static readonly CFraction r011 = CFraction.FromRational(2, 1); // 2   = [2]
  private static readonly CFraction r100 = CFraction.FromRational(1, 3); // 1/3 = [0;3]
  private static readonly CFraction r110 = CFraction.FromRational(2, 3); // 2/3 = [0;1,2]
  private static readonly CFraction r101 = CFraction.FromRational(3, 2); // 3/2 = [1;2]
  private static readonly CFraction r111 = CFraction.FromRational(3, 1); // 3   = [3]

  private static readonly CFraction rn010 = CFraction.FromRational(-1, 2); // -1/2 = [-1;2]
  private static readonly CFraction rn100 = CFraction.FromRational(-1, 3); // -1/3 = [-1; 1, 2]
  private static readonly CFraction rn011 = CFraction.FromRational(-2, 3); // -2/3 = [-1;3]
  private static readonly CFraction rn101 = CFraction.FromRational(-3, 2); // -3/2 = [-2;2]
  private static readonly CFraction rn111 = CFraction.FromRational(-3, 1); // -3   = [-3]
  private static readonly CFraction rn001 = CFraction.FromRational(-1, 1); // -1   = [-1]
  private static readonly CFraction rn002 = CFraction.FromRational(-2, 1); // -2   = [-2]

  [Test]
  public void Equals() {
    Assert.That(r001.Equals(r001));           // 1 == 1
    Assert.That(r001.Equals(r010) is false);  // 1 != 1/2
    Assert.That(rn010.Equals(rn010));         // -1/2 == -1/2
    Assert.That(rn010.Equals(r010) is false); // -1/2 != 1/2


    var r1 = CFraction.FromRational(5, 7); // 5/7 = [0;1,2,2]
    var r2 = CFraction.FromRational(5, 7);
    Assert.That(r1.Equals(r2)); // 5/7 == 5/7

    Assert.That(CFraction.E.Equals(CFraction.E));              // e == e
    Assert.That(CFraction.E.Equals(CFraction.Sqrt2) is false); // e != sqrt(2)
    Assert.That(CFraction.Zero.Equals(CFraction.Zero));        // 0 == 0
  }

  [Test]
  public void CompareToTest() {
    Assert.That(r011.CompareTo(r101) > 0); // 2 > 3/2
    Assert.That(r100.CompareTo(r110) < 0); // 1/3 < 2/3
    Assert.That(r111.CompareTo(r011) > 0); // 3 > 2
    Assert.That(r011.CompareTo(r111) < 0); // 2 < 3

    Assert.That(rn010.CompareTo(rn100) < 0); // -1/2 < -1/3  (|-1/2| > |-1/3|)
    Assert.That(rn100.CompareTo(rn010) > 0); // -1/3 > -1/2
    Assert.That(rn111.CompareTo(rn002) < 0); // -3 < -2
    Assert.That(rn002.CompareTo(rn111) > 0); // -2 > -3
  }

  [Test]
  public void Compare_ThisShorter_EvenIndexEnd() {
    // 1/2 = [0; 2], 3/7 = [0; 2, 3]
    var r1 = r010;
    var r2 = CFraction.FromRational(3, 7);
    Assert.That(r1.CompareTo(r2) > 0); // 1/2 > 3/7
  }

  [Test]
  public void Compare_ThisShorter_OddIndexEnd() {
    // 2/3 = [0; 1, 2], 5/7 = [0; 1, 2, 2]
    var r1 = r110;
    var r2 = CFraction.FromRational(5, 7);
    Assert.That(r1.CompareTo(r2) < 0); // 2/3 < 5/7
  }

  [Test]
  public void Compare_OtherShorter_EvenIndexEnd() {
    // 5/7 = [0; 1, 2, 2], 2/3 = [0; 1, 2]
    var r1 = CFraction.FromRational(5, 7);
    var r2 = r110;
    Assert.That(r1.CompareTo(r2) > 0); // 5/7 > 2/3
  }

  [Test]
  public void Compare_OtherShorter_OddIndexEnd() {
    // 3/7 = [0; 2, 3], 1/2 = [0; 2]
    var r1 = CFraction.FromRational(3, 7);
    var r2 = r010;
    Assert.That(r1.CompareTo(r2) < 0); // 3/7 < 1/2
  }

  [Test]
  public void Compare_DiffAtEvenIndex() {
    // 2/3 = [0; 1, 2], 3/4 = [0; 1, 3]
    var r1 = r110;
    var r2 = CFraction.FromRational(3, 4);
    Assert.That(r1.CompareTo(r2) < 0); // 2/3 < 3/4
  }

  [Test]
  public void Compare_DiffAtOddIndex() {
    // 1/2 = [0; 2], 1/3 = [0; 3]  <- was [0;2] vs [0;3], now [-1;2] vs [-1; 1, 2]
    var r1 = r010;                     // 1/2 = [0; 2]
    var r2 = r100;                     // 1/3 = [0; 3]
    Assert.That(r1.CompareTo(r2) > 0); // 1/2 > 1/3
  }

  [Test]
  public void Compare_Negative_r1() {
    var r1 = -CFraction.FromRational(1, 2); // -1/2 = [-1; 2]
    var r2 = CFraction.FromRational(1, 2);  // 1/2  = [0; 2]
    Assert.That(r1.CompareTo(r2) < 0);      // -1/2 < 1/2
  }

  [Test]
  public void Compare_Negative_r2() {
    var r1 = CFraction.FromRational(1, 2);  // 1/2  = [0; 2]
    var r2 = -CFraction.FromRational(1, 2); // -1/2 = [-1; 2]
    Assert.That(r1.CompareTo(r2) > 0);      // 1/2 > -1/2
  }

  [Test]
  public void Compare_Negative_Both() {
    var r1 = -CFraction.FromRational(1, 2); // -1/2 = [-1; 2]
    var r2 = -CFraction.FromRational(1, 3); // -1/3 = [-1; 1, 2]
    Assert.That(r1.CompareTo(r2) < 0);      // -1/2 < -1/3 (|-1/2| > |-1/3|)
  }

  [Test]
  public void Compare_Zero_vs_Positive() {
    var r1 = CFraction.Zero;               // 0    = [0]
    var r2 = CFraction.FromRational(1, 2); // 1/2  = [0; 2]
    Assert.That(r1.CompareTo(r2) < 0);     // 0 < 1/2
    Assert.That(r2.CompareTo(r1) > 0);     // 1/2 > 0
  }

  [Test]
  public void Compare_Zero_vs_Negative() {
    var r1 = CFraction.Zero;                // 0     = [0]
    var r2 = -CFraction.FromRational(1, 2); // -1/2  = [-1; 2]
    Assert.That(r1.CompareTo(r2) > 0);      // 0 > -1/2
    Assert.That(r2.CompareTo(r1) < 0);      // -1/2 < 0
  }

  [Test]
  public void Compare_Zero_Equals_Zero() {
    Assert.That(CFraction.Zero.CompareTo(CFraction.Zero) == 0); // 0 == 0
  }

  [Test]
  public void Compare_NegativeInteger_vs_NegativeFraction() {
    var r1 = rn001;                    // -1   = [-1]
    var r2 = rn101;                    // -3/2 = [-2; 2]
    Assert.That(r1.CompareTo(r2) > 0); // -1 > -3/2  (-1 > -1.5)
    Assert.That(r2.CompareTo(r1) < 0); // -3/2 < -1
  }

  [Test]
  public void Compare_NegativeFraction_vs_NegativeInteger() {
    var r1 = rn101;                    // -3/2 = [-2; 2]
    var r2 = rn001;                    // -1   = [-1]
    Assert.That(r1.CompareTo(r2) < 0); // -3/2 < -1
    Assert.That(r2.CompareTo(r1) > 0); // -1 > -3/2
  }

  [Test]
  public void Compare_NegativeInteger_vs_SmallerNegativeInteger() {
    var r1 = rn001;                    // -1   = [-1]
    var r2 = rn002;                    // -2   = [-2]
    Assert.That(r1.CompareTo(r2) > 0); // -1 > -2
    Assert.That(r2.CompareTo(r1) < 0); // -2 < -1
  }

  [Test]
  public void Compare_PositiveFraction_vs_NegativeFraction() {
    var r1 = r010;                     // 1/2  = [0; 2]
    var r2 = rn010;                    // -1/2 = [-1; 2]
    Assert.That(r1.CompareTo(r2) > 0); // 1/2 > -1/2
    Assert.That(r2.CompareTo(r1) < 0); // -1/2 < 1/2
  }

}
