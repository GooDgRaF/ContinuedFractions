using ContinuedFractions;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class ComparationTests {


  [Test]
  public void EQTest() {
    var r001 = ContinuedFraction.FromRational(1,1);
    var r010 = ContinuedFraction.FromRational(1,2);
    var r011 = ContinuedFraction.FromRational(2,1);
    var r100 = ContinuedFraction.FromRational(1,3);
    var r110 = ContinuedFraction.FromRational(2,3);
    var r101 = ContinuedFraction.FromRational(3,2);
    var r111 = ContinuedFraction.FromRational(3,1);



    Assert.That(r001.Equals(r001));
    Assert.That(r001.Equals(r010) is false);

    Assert.That(ContinuedFraction.E().Equals(ContinuedFraction.E()));
    Assert.That(ContinuedFraction.E().Equals(ContinuedFraction.Sqrt2()) is false);

    Assert.That(r011.CompareTo(r101) > 0);
    Assert.That(r100.CompareTo(r110) < 0);
    Assert.That(r111.CompareTo(r011) > 0);
    Assert.That(r011.CompareTo(r111) < 0);

    // todo: Больше сравнений!
  }

}
