using ContinuedFractions;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class OperatorTests {

  public static ContinuedFraction cfFin1 = new ContinuedFraction(new int[] { 1, 2, 3 }); // 10 / 7

  [Test]
  public void AddFracTest() {
    var r01 = cfFin1 + new Frac(0, 1);
  }

}
