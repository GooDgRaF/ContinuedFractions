namespace Tests;

[TestFixture]
public class FactoryTests {

  // --- Вспомогательный метод для форматирования сообщений об ошибках ---
  private string FormatFailMessage(
      IEnumerable<BigInteger> expectedCoeffs
    , IEnumerable<BigInteger> actualCoeffs
    , string                  description
    ) {
    string expectedStr = FormatCoefficients(expectedCoeffs);
    string actualStr   = FormatCoefficients(actualCoeffs);

    return $"{description} failed.\nExpected Coefficients: {expectedStr}\nActual Coefficients:   {actualStr}";
  }

  private string FormatCoefficients(IEnumerable<BigInteger> coeffs) {
    return !coeffs.Any() ? "[] (Infinity)" : $"[{string.Join(", ", coeffs)}]";
  }

  // --- Вспомогательный метод для сообщений об ошибках при сравнении дробей ---
  private string FormatFailMessage(CFraction expected, CFraction actual, string description) {
    // Используем ToString() дробей для сообщения
    string expectedStr = expected.ToString();
    string actualStr   = actual.ToString();

    return $"{description} failed.\nExpected Fraction: {expectedStr}\nActual Fraction:   {actualStr}";
  }

#region FromRational
  [Test]
  public void FromRational_One() {
    BigInteger       numerator      = 1;
    BigInteger       denominator    = 1;
    List<BigInteger> expectedCoeffs = new List<BigInteger> { 1 };
    string           description    = "CFraction.FromRational(1, 1)";

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromRational_ZeroNumerator() {
    BigInteger       numerator      = 0;
    BigInteger       denominator    = 7;
    List<BigInteger> expectedCoeffs = new List<BigInteger> { 0 };
    string           description    = "CFraction.FromRational(0, 7)";

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromRational_PositiveFraction() {
    BigInteger       numerator      = 22;
    BigInteger       denominator    = 7;
    List<BigInteger> expectedCoeffs = new List<BigInteger> { 3, 7 };
    string           description    = "CFraction.FromRational(22, 7)";

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromRational_NegativeFraction_CorrectCoeffs() {
    BigInteger       numerator      = -22;
    BigInteger       denominator    = 7;
    List<BigInteger> expectedCoeffs = new List<BigInteger> { -4, 1, 6 };
    string           description    = "CFraction.FromRational(-22, 7)";

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromRational_NegativeDenominator_CorrectCoeffs() {
    BigInteger       numerator      = 22;
    BigInteger       denominator    = -7;
    List<BigInteger> expectedCoeffs = new List<BigInteger> { -4, 1, 6 };
    string           description    = "CFraction.FromRational(22, -7)";

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromRational_BothNegative_CorrectCoeffs() {
    BigInteger       numerator      = -22;
    BigInteger       denominator    = -7;
    List<BigInteger> expectedCoeffs = new List<BigInteger> { 3, 7 };
    string           description    = "CFraction.FromRational(-22, -7)";

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromRational_FractionLessThanOne_CorrectCoeffs() {
    BigInteger       numerator      = 7;
    BigInteger       denominator    = 22;
    List<BigInteger> expectedCoeffs = new List<BigInteger> { 0, 3, 7 };
    string           description    = "CFraction.FromRational(7, 22)";

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromRational_ReducibleFraction_CorrectCoeffs() {
    BigInteger       numerator      = 44;
    BigInteger       denominator    = 14;
    List<BigInteger> expectedCoeffs = new List<BigInteger> { 3, 7 }; // Should simplify to 22/7
    string           description    = "CFraction.FromRational(44, 14)";

    CFraction cf           = CFraction.FromRational(numerator, denominator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromRational_Infinity() {
    BigInteger numerator   = 1;
    BigInteger denominator = 0;
    CFraction  expected    = CFraction.Infinity;
    string     description = "CFraction.FromRational(1, 0)";

    CFraction actual = CFraction.FromRational(numerator, denominator);

    Assert.That(actual, Is.EqualTo(expected), FormatFailMessage(expected, actual, description));
  }

  [Test]
  public void FromRational_ZeroByZero_ThrowsArgumentException() {
    BigInteger numerator   = 0;
    BigInteger denominator = 0;
    string     description = "CFraction.FromRational(0, 0)";

    // Using Take() might hide the original exception source, better to test creation
    Assert.Throws<ArgumentException>
      (() => CFraction.FromRational(numerator, denominator), $"{description} should throw ArgumentException.");
  }
#endregion

#region FromCoeffs
  [Test]
  public void FromCoeffs_ValidCoeffs() {
    List<BigInteger> inputCoeffs    = new List<BigInteger> { 1, 2, 3, 4 };
    List<BigInteger> expectedCoeffs = inputCoeffs;
    string           description    = $"CFraction.FromCoeffs({FormatCoefficients(inputCoeffs)})";

    CFraction cf           = CFraction.FromCoeffs(inputCoeffs);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromCoeffs_ZeroInCoeffsAfterFirst_ThrowsException() {
    List<BigInteger> invalidCoeffs = new List<BigInteger> { 1, 2, 0, 4 };
    string           description   = $"CFraction.FromCoeffs({FormatCoefficients(invalidCoeffs)})";

    Assert.Throws<ArgumentException>
      (
       () => CFraction.FromCoeffs(invalidCoeffs)
     , $"{description} should throw ArgumentException for zero coefficient after the first."
      );
  }

  [Test]
  public void FromCoeffs_ZeroFirstCoeff() {
    List<BigInteger> validCoeffsWithZeroFirst = new List<BigInteger> { 0, 2, 3, 4 };
    List<BigInteger> expectedCoeffs           = validCoeffsWithZeroFirst;
    string           description              = $"CFraction.FromCoeffs({FormatCoefficients(validCoeffsWithZeroFirst)})";

    CFraction cf           = CFraction.FromCoeffs(validCoeffsWithZeroFirst);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromCoeffs_EmptyList_RepresentsInfinity() {
    List<BigInteger> emptyCoeffs    = new List<BigInteger>();
    List<BigInteger> expectedCoeffs = emptyCoeffs;
    CFraction        expectedCf     = CFraction.Infinity;
    string           description    = $"CFraction.FromCoeffs({FormatCoefficients(emptyCoeffs)})";

    CFraction cf           = CFraction.FromCoeffs(emptyCoeffs);
    var       actualCoeffs = cf.Take(1); // Take at least one to check emptiness

    Assert.That
      (
       actualCoeffs
     , Is.EqualTo(expectedCoeffs)
     , FormatFailMessage(expectedCoeffs, actualCoeffs, $"{description} coefficients check")
      );
    Assert.That(cf, Is.EqualTo(expectedCf), FormatFailMessage(expectedCf, cf, $"{description} fraction check"));
  }

  [Test]
  public void FromCoeffs_SingleCoeffList() {
    List<BigInteger> singleCoeffList = new List<BigInteger> { 5 };
    List<BigInteger> expectedCoeffs  = singleCoeffList;
    string           description     = $"CFraction.FromCoeffs({FormatCoefficients(singleCoeffList)})";

    CFraction cf           = CFraction.FromCoeffs(singleCoeffList);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromCoeffs_ListEndsWithOne_IsCanonicalized() {
    List<BigInteger> inputCoeffs    = new List<BigInteger> { 5, 1 };
    List<BigInteger> expectedCoeffs = new List<BigInteger> { 6 };
    string           description    = $"CFraction.FromCoeffs({FormatCoefficients(inputCoeffs)}) canonicalization";

    CFraction cf = CFraction.FromCoeffs(inputCoeffs);
    // Take potentially more coeffs to ensure it doesn't just stop early
    var actualCoeffs = cf.Take(expectedCoeffs.Count + 1);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromCoeffs_ListEndsWithOne_SingleElementCase_IsCanonicalized() {
    List<BigInteger> inputCoeffs = new List<BigInteger> { 1 }; // Represents integer 1
    List<BigInteger> expectedCoeffs = new List<BigInteger> { 1 }; // Should remain [1], not canonicalized to [0] or similar
    string           description = $"CFraction.FromCoeffs({FormatCoefficients(inputCoeffs)}) canonicalization (single element)";

    CFraction cf           = CFraction.FromCoeffs(inputCoeffs);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count + 1); // Take more

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromCoeffs_NegativeFirstCoeff() {
    List<BigInteger> inputCoeffs    = new List<BigInteger> { -1, 2, 3 };
    List<BigInteger> expectedCoeffs = inputCoeffs;
    string           description    = $"CFraction.FromCoeffs({FormatCoefficients(inputCoeffs)})";

    CFraction cf           = CFraction.FromCoeffs(inputCoeffs);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromCoeffs_NegativeCoeffAfterFirst_ThrowsException() {
    List<BigInteger> invalidCoeffs = new List<BigInteger> { 1, -2, 3 };
    string           description   = $"CFraction.FromCoeffs({FormatCoefficients(invalidCoeffs)})";

    // Assuming negative coefficients after the first are invalid according to standard form
    Assert.Throws<ArgumentException>
      (
       () => CFraction.FromCoeffs(invalidCoeffs)
     , $"{description} should throw ArgumentException for negative coefficient after the first."
      );
  }
#endregion

#region FromGenerator
  [Test]
  public void FromGenerator_FiniteList() {
    List<BigInteger>        coeffs         = new List<BigInteger> { 1, 2, 3 };
    IEnumerable<BigInteger> generator      = coeffs.ToList(); // Use ToList to ensure it's a copy if needed
    List<BigInteger>        expectedCoeffs = coeffs;
    string                  description    = "CFraction.FromGenerator(List<BigInteger>)";

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count + 1); // Take more to check finiteness

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromGenerator_CanonicalizationCheck() {
    List<BigInteger>        coeffs         = new List<BigInteger> { 1, 2, 3, 1 };
    IEnumerable<BigInteger> generator      = coeffs.ToList();
    List<BigInteger>        expectedCoeffs = new List<BigInteger> { 1, 2, 4 }; // Canonical form
    string                  description    = "CFraction.FromGenerator requiring canonicalization";

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count + 1); // Take more

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromGenerator_Array() {
    BigInteger[]            coeffs         = new BigInteger[] { 1, 2, 3 };
    IEnumerable<BigInteger> generator      = coeffs;
    List<BigInteger>        expectedCoeffs = coeffs.ToList();
    string                  description    = "CFraction.FromGenerator(int[])";

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count + 1); // Take more

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromGenerator_InfiniteGenerator() {
    // Example: Generator for Sqrt(2) = [1; (2)]
    IEnumerable<BigInteger> generator      = GetSqrt2Generator();
    List<BigInteger>        expectedCoeffs = new List<BigInteger> { 1, 2, 2, 2, 2 }; // First 5 coeffs
    string                  description    = "CFraction.FromGenerator(Infinite Sqrt2 Generator)";

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(expectedCoeffs.Count);

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), FormatFailMessage(expectedCoeffs, actualCoeffs, description));
  }

  [Test]
  public void FromGenerator_EmptyGenerator_RepresentsInfinity() {
    IEnumerable<BigInteger> generator      = Enumerable.Empty<BigInteger>();
    List<BigInteger>        expectedCoeffs = new List<BigInteger>();
    CFraction               expectedCf     = CFraction.Infinity;
    string                  description    = "CFraction.FromGenerator(Empty Generator)";

    CFraction cf           = CFraction.FromGenerator(generator);
    var       actualCoeffs = cf.Take(1); // Take at least one

    Assert.That
      (
       actualCoeffs
     , Is.EqualTo(expectedCoeffs)
     , FormatFailMessage(expectedCoeffs, actualCoeffs, $"{description} coefficients check")
      );
    Assert.That(cf, Is.EqualTo(expectedCf), FormatFailMessage(expectedCf, cf, $"{description} fraction check"));
  }
#endregion

  // --- Helper Generators ---
  private static IEnumerable<BigInteger> GetSqrt2Generator() {
    yield return 1;

    while (true) {
      yield return 2;
    }
  }

}
