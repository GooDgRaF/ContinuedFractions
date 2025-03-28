using System.Numerics;

namespace Tests;

[TestFixture]
public class FactoryTests {

#region FromRational
  [Test]
  public void FromRational_One() {
    BigInteger numerator      = 1;
    BigInteger denominator    = 1;
    List<int>  expectedCoeffs = new List<int> { 1 };

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for 1/1 should be [1]");
  }

  [Test]
  public void FromRational_ZeroNumerator() {
    BigInteger numerator      = 0;
    BigInteger denominator    = 7;
    List<int>  expectedCoeffs = new List<int> { 0 };

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for 0/7 should be [0]");
  }

  [Test]
  public void FromRational_PositiveFraction() {
    BigInteger numerator      = 22;
    BigInteger denominator    = 7;
    List<int>  expectedCoeffs = new List<int> { 3, 7 };

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for 22/7 should be [3, 7]");
  }

  [Test]
  public void FromRational_NegativeFraction_CorrectCoeffs() {
    BigInteger numerator      = -22;
    BigInteger denominator    = 7;
    List<int>  expectedCoeffs = new List<int> { -4, 1, 6 };

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for -22/7 should be [-4, 1, 6]");
  }

  [Test]
  public void FromRational_NegativeDenominator_CorrectCoeffs() {
    BigInteger numerator      = 22;
    BigInteger denominator    = -7;
    List<int>  expectedCoeffs = new List<int> { -4, 1, 6 };

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for 22/-7 should be [-4, 1, 6]");
  }

  [Test]
  public void FromRational_BothNegative_CorrectCoeffs() {
    BigInteger numerator      = -22;
    BigInteger denominator    = -7;
    List<int>  expectedCoeffs = new List<int> { 3, 7 };

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for -22/-7 should be [3, 7]");
  }

  [Test]
  public void FromRational_FractionLessThanOne_CorrectCoeffs() {
    BigInteger numerator      = 7;
    BigInteger denominator    = 22;
    List<int>  expectedCoeffs = new List<int> { 0, 3, 7 };

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for 7/22 should be [0, 3, 7]");
  }

  [Test]
  public void FromRational_ReducibleFraction_CorrectCoeffs() {
    BigInteger numerator      = 44;
    BigInteger denominator    = 14;
    List<int>  expectedCoeffs = new List<int> { 3, 7 };

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients for 44/14 should be [3, 7]");
  }

  [Test]
  public void FromRational_Infinity() {
    Assert.That(CFraction.Infinity, Is.EqualTo(CFraction.FromRational(1, 0)), "The fraction 1/0 should be equal to infinity.");
  }

  [Test]
  public void FromRational_ZeroByZero_ThrowsArgumentException() {
    BigInteger numerator   = 0;
    BigInteger denominator = 0;

    Assert.Throws<ArgumentException>
      (() => CFraction.FromRational(numerator, denominator).Take(1), "Expected ArgumentException for 0/0 fraction");
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
#endregion

#region FromCoeffs
  [Test]
  public void FromCoeffs_ValidCoeffs() {
    List<int> inputCoeffs    = new List<int> { 1, 2, 3, 4 };
    List<int> expectedCoeffs = inputCoeffs;

    CFraction cf           = CFraction.FromCoeffs(inputCoeffs);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

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

    CFraction cf           = CFraction.FromCoeffs(validCoeffsWithZeroFirst);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Zero as the first coefficient should be valid");
  }

  [Test]
  public void FromCoeffs_EmptyList() {
    List<int> emptyCoeffs    = new List<int>();
    List<int> expectedCoeffs = emptyCoeffs;

    CFraction cf           = CFraction.FromCoeffs(emptyCoeffs);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Empty coefficients list should be valid (infinity cf)");
    Assert.That(cf, Is.EqualTo(CFraction.Infinity), "Empty coefficients list should be valid (infinity cf)");
  }

  [Test]
  public void FromCoeffs_SingleCoeffList() {
    List<int> singleCoeffList = new List<int> { 5 };
    List<int> expectedCoeffs  = singleCoeffList;

    CFraction cf           = CFraction.FromCoeffs(singleCoeffList);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Single coefficient list should be valid");
  }

  [Test]
  public void FromCoeffs_ListEndsWithOne() {
    List<int> inputCoeffs    = new List<int> { 5, 1 };
    List<int> expectedCoeffs = new List<int> { 6 };

    CFraction cf           = CFraction.FromCoeffs(inputCoeffs);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "List ending with 1 should be canonicalized");
  }

  [Test]
  public void FromCoeffs_NegativeFirstCoeff() {
    List<int> inputCoeffs    = new List<int> { -1, 2, 3 };
    List<int> expectedCoeffs = inputCoeffs;

    CFraction cf           = CFraction.FromCoeffs(inputCoeffs);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Negative first coefficient should be valid");
  }
#endregion

#region FromGenerator
  [Test]
  public void FromGenerator_List() {
    List<int>        coeffs         = new List<int> { 1, 2, 3 };
    IEnumerable<int> generator      = coeffs;
    List<int>        expectedCoeffs = coeffs;

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "FromGenerator should work with List<int>");
  }

  [Test]
  public void FromGenerator_DifferentRepresentations() {
    List<int>        coeffs         = new List<int> { 1, 2, 3, 1 };
    IEnumerable<int> generator      = coeffs;
    List<int>        expectedCoeffs = new List<int> { 1, 2, 4 };

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "The [1;2,3,1] should be the same as [1;2,4]");
  }

  [Test]
  public void FromGenerator_Array_CorrectCFraction() {
    int[]            coeffs         = new int[] { 1, 2, 3 };
    IEnumerable<int> generator      = coeffs;
    List<int>        expectedCoeffs = coeffs.ToList();

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "FromGenerator should work with int[]");
  }

  [Test]
  public void FromGenerator_IEnumerable() {
    IEnumerable<int> generator      = GetTestGenerator();
    List<int>        expectedCoeffs = new List<int> { 1, 2, 3 };

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "FromGenerator should work with IEnumerable<int> generator");
  }

  [Test]
  public void FromGenerator_EmptyGenerator() {
    IEnumerable<int> generator      = Enumerable.Empty<int>();
    List<int>        expectedCoeffs = new List<int>();

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "FromGenerator should work with empty generator");
  }
#endregion


  private static IEnumerable<int> GetTestGenerator() {
    yield return 1;
    yield return 2;
    yield return 3;
  }

}
