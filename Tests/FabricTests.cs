using System.Numerics;

namespace Tests;

[TestFixture]
public class FactoryTests {

  [Test]
  public void FromRational_One() {
    BigInteger numerator      = 1;
    BigInteger denominator    = 1;
    List<int>  expectedCoeffs = new List<int> { 1 };

    CFraction fraction     = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for 1/1 should be [1]");
  }

  [Test]
  public void FromRational_ZeroNumerator() {
    BigInteger numerator      = 0;
    BigInteger denominator    = 7;
    List<int>  expectedCoeffs = new List<int> { 0 };

    CFraction fraction     = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for 0/7 should be [0]");
  }

  [Test]
  public void FromRational_PositiveFraction() {
    BigInteger numerator      = 22;
    BigInteger denominator    = 7;
    List<int>  expectedCoeffs = new List<int> { 3, 7 };

    CFraction fraction     = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for 22/7 should be [3, 7]");
  }

  [Test]
  public void FromRational_Overflow_ThrowsException() {
    BigInteger numerator   = BigInteger.Pow(int.MaxValue + 1L, 2);
    BigInteger denominator = 1;

    Assert.Throws<OverflowException>
      (
       () => CFraction.FromRational(numerator, denominator).Take(1)
     , "Expected OverflowException if coefficient is out of int.MaxValue range"
      );
  }

  [Test]
  public void FromCoeffs_ValidCoeffs() {
    List<int> inputCoeffs    = new List<int> { 1, 2, 3, 4 };
    List<int> expectedCoeffs = inputCoeffs;

    CFraction fraction     = CFraction.FromCoeffs(inputCoeffs);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "For valid coefficients, Take() should return the same");
  }

  [Test]
  public void FromCoeffs_ZeroInCoeffsAfterFirst_ThrowsException() {
    List<int> invalidCoeffs = new List<int> { 1, 2, 0, 4 };

    Assert.Throws<ArgumentException>
      (() => CFraction.FromCoeffs(invalidCoeffs), "Expected ArgumentException for zero coefficient after the first one");
  }

  [Test]
  public void FromCoeffs_ZeroFirstCoeff() {
    List<int> validCoeffsWithZeroFirst = new List<int> { 0, 2, 3, 4 };
    List<int> expectedCoeffs           = validCoeffsWithZeroFirst;

    CFraction fraction     = CFraction.FromCoeffs(validCoeffsWithZeroFirst);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Zero as the first coefficient should be valid");
  }

  [Test]
  public void FromCoeffs_EmptyList() {
    List<int> emptyCoeffs    = new List<int>();
    List<int> expectedCoeffs = emptyCoeffs;

    CFraction fraction     = CFraction.FromCoeffs(emptyCoeffs);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Empty coefficients list should be valid (infinity cf)");
  }

  [Test]
  public void FromGenerator_List() {
    List<int>        coeffs         = new List<int> { 1, 2, 3 };
    IEnumerable<int> generator      = coeffs;
    List<int>        expectedCoeffs = coeffs;

    CFraction fraction     = CFraction.FromGenerator(generator);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "FromGenerator should work with List<int>");
  }

  [Test]
  public void FromGenerator_DifferentRepresentations() {
    List<int>        coeffs         = new List<int> { 1, 2, 3, 1 };
    IEnumerable<int> generator      = coeffs;
    List<int>        expectedCoeffs = new List<int> { 1, 2, 4 };

    CFraction fraction     = CFraction.FromGenerator(generator);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "The [1;2,3,1] should be the same as [1;2,4]");
  }

  [Test]
  public void FromGenerator_Array_CorrectCFraction() {
    int[]            coeffs         = new int[] { 1, 2, 3 };
    IEnumerable<int> generator      = coeffs;
    List<int>        expectedCoeffs = coeffs.ToList();

    CFraction fraction     = CFraction.FromGenerator(generator);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "FromGenerator should work with int[]");
  }

  [Test]
  public void FromGenerator_IEnumerable() {
    IEnumerable<int> generator      = GetTestGenerator();
    List<int>        expectedCoeffs = new List<int> { 1, 2, 3 };

    CFraction fraction     = CFraction.FromGenerator(generator);
    var       actualCoeffs = fraction.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "FromGenerator should work with IEnumerable<int> generator");
  }

  private static IEnumerable<int> GetTestGenerator() {
    yield return 1;
    yield return 2;
    yield return 3;
  }

  // [Test]
  // public void FromRational_NegativeFraction_CorrectCoeffs()
  // {
  //     BigInteger numerator = -22;
  //     BigInteger denominator = 7;
  //     List<int> expectedCoeffs = new List<int> { -4, 1, 6 };
  //
  //     CFraction fraction = CFraction.FromRational(numerator, denominator);
  //     var actualCoeffs = fraction.Take(expectedCoeffs.Count);
  //
  //     Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for -22/7 should be [-4, 1, 6]");
  // }

}
