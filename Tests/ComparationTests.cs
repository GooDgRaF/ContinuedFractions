using ContinuedFractions;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class ComparationTests {

  private static readonly ContinuedFraction r001 = ContinuedFraction.FromRational(1, 1); // 1   = [1]
  private static readonly ContinuedFraction r010 = ContinuedFraction.FromRational(1, 2); // 1/2 = [0;2]
  private static readonly ContinuedFraction r011 = ContinuedFraction.FromRational(2, 1); // 2   = [2]
  private static readonly ContinuedFraction r100 = ContinuedFraction.FromRational(1, 3); // 1/3 = [0;3]
  private static readonly ContinuedFraction r110 = ContinuedFraction.FromRational(2, 3); // 2/3 = [0;1,2]
  private static readonly ContinuedFraction r101 = ContinuedFraction.FromRational(3, 2); // 3/2 = [1;2]
  private static readonly ContinuedFraction r111 = ContinuedFraction.FromRational(3, 1); // 3   = [3]

  [Test]
  public void Equals() {
    Assert.That(r001.Equals(r001));          // 1 == 1
    Assert.That(r001.Equals(r010) is false); // 1 != 1/2

    var r1 = ContinuedFraction.FromRational(5, 7); // 5/7 = [0;1,2,2]
    var r2 = ContinuedFraction.FromRational(5, 7);
    Assert.That(r1.Equals(r2)); // 5/7 == 5/7

    Assert.That(ContinuedFraction.E().Equals(ContinuedFraction.E()));              // e == e
    Assert.That(ContinuedFraction.E().Equals(ContinuedFraction.Sqrt2()) is false); // e != sqrt(2)
  }

  [Test]
  public void CompareToTest() {
    Assert.That(r011.CompareTo(r101) > 0); // 2 > 3/2
    Assert.That(r100.CompareTo(r110) < 0); // 1/3 < 2/3
    Assert.That(r111.CompareTo(r011) > 0); // 3 > 2
    Assert.That(r011.CompareTo(r111) < 0); // 2 < 3
  }

  [Test]
  public void Compare_ThisShorter_EvenIndexEnd() {
    // 1/2 = [0; 2], 3/7 = [0; 2, 3]
    var r1 = r010;
    var r2 = ContinuedFraction.FromRational(3, 7);
    Assert.That(r1.CompareTo(r2) > 0); // 1/2 > 3/7
  }

  [Test]
  public void Compare_ThisShorter_OddIndexEnd() {
    // 2/3 = [0; 1, 2], 5/7 = [0; 1, 2, 2]
    var r1 = r110;
    var r2 = ContinuedFraction.FromRational(5, 7);
    Assert.That(r1.CompareTo(r2) < 0); // 2/3 < 5/7
  }

  [Test]
  public void Compare_OtherShorter_EvenIndexEnd() {
    // 5/7 = [0; 1, 2, 2], 2/3 = [0; 1, 2]
    var r1 = ContinuedFraction.FromRational(5, 7);
    var r2 = r110;
    Assert.That(r1.CompareTo(r2) > 0); // 5/7 > 2/3
  }

  [Test]
  public void Compare_OtherShorter_OddIndexEnd() {
    // 3/7 = [0; 2, 3], 1/2 = [0; 2]
    var r1 = ContinuedFraction.FromRational(3, 7);
    var r2 = r010;
    Assert.That(r1.CompareTo(r2) < 0); // 3/7 < 1/2
  }

  [Test]
  public void Compare_DiffAtEvenIndex() {
    // 2/3 = [0; 1, 2], 3/4 = [0; 1, 3]
    var r1 = r110;
    var r2 = ContinuedFraction.FromRational(3, 4);
    Assert.That(r1.CompareTo(r2) < 0); // 2/3 < 3/4
  }

  [Test]
  public void Compare_DiffAtOddIndex() {
    // 1/2 = [0; 2], 1/3 = [0; 3]
    var r1 = r010;
    var r2 = r100;
    Assert.That(r1.CompareTo(r2) > 0); // 1/2 > 1/3
  }

  // [Test]
  // public void Compare_Negative_r1() {
  //   var r1 = -ContinuedFraction.FromRational(1, 2);
  //   var r2 = ContinuedFraction.FromRational(1, 2);
  //   Assert.That(r1.CompareTo(r2) < 0);
  // }
  //
  // [Test]
  // public void Compare_Negative_r2() {
  //   var r1 = ContinuedFraction.FromRational(1, 2);
  //   var r2 = -ContinuedFraction.FromRational(1, 2);
  //   Assert.That(r1.CompareTo(r2) > 0);
  // }
  //
  // [Test]
  // public void Compare_Negative_Both() {
  //   var r1 = -ContinuedFraction.FromRational(1, 2);
  //   var r2 = -ContinuedFraction.FromRational(1, 3);
  //   Assert.That(r1.CompareTo(r2) < 0); // -1/2 < -1/3
  // }
  //
  // [Test]
  // public void Compare_Zero() {
  //   var r1 = ContinuedFraction.Zero;
  //   var r2 = ContinuedFraction.FromRational(1, 2);
  //   Assert.That(r1.CompareTo(r2) < 0);
  //   Assert.That(r2.CompareTo(r1) > 0);
  //   Assert.That(r1.CompareTo(ContinuedFraction.Zero) == 0);
  // }

}
