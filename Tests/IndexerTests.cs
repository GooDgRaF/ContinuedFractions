namespace Tests;

[TestFixture]
public class IndexerTests {

  [Test]
  public void Indexer_OneElements() {
    List<BigInteger> coeffs   = new List<BigInteger> { 1 };
    CFraction        fraction = CFraction.FromGenerator(coeffs);

    BigInteger? actualValue = fraction[0];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo((BigInteger)1), "Indexer should return the first coefficient for index 0");
  }

  [Test]
  public void Indexer_TwoElementsOnes() {
    List<BigInteger> coeffs   = new List<BigInteger> { 1, 1 };
    CFraction        fraction = CFraction.FromGenerator(coeffs);

    BigInteger? actualValue = fraction[0];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo((BigInteger)2), "Indexer should return the digit 2");
  }

  [Test]
  public void Indexer_TwoElements() {
    List<BigInteger> coeffs   = new List<BigInteger> { 1, 2 };
    CFraction        fraction = CFraction.FromGenerator(coeffs);

    BigInteger? actualValue = fraction[0];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo((BigInteger)1), "Indexer should return the first coefficient for index 0");
  }

  [Test]
  public void Indexer_ValidIndexWithinCache() {
    List<BigInteger> coeffs   = new List<BigInteger> { 1, 2, 3, 4, 5 };
    CFraction        fraction = CFraction.FromGenerator(coeffs);
    fraction.Take(3);

    BigInteger? actualValue = fraction[1];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo((BigInteger)2), "Indexer should return cached value for index within cache");
  }


  [Test]
  public void Indexer_ValidIndexOutsideCache() {
    List<BigInteger> coeffs   = new List<BigInteger> { 1, 2, 3, 4, 5 };
    CFraction        fraction = CFraction.FromGenerator(coeffs);
    _ = fraction.Take(2);

    BigInteger? actualValue = fraction[3];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo((BigInteger)4), "Indexer should cache and return value for index outside cache");
    Assert.That(fraction.Take(4).Count, Is.EqualTo(4), "Indexer should extend cache to requested index");
  }

  [Test]
  public void Indexer_IndexAtEnd_ReturnsNull() {
    List<BigInteger> coeffs   = new List<BigInteger> { 1, 2, 3 };
    CFraction        fraction = CFraction.FromGenerator(coeffs);

    BigInteger? actualValue = fraction[3]; // Index at the end

    Assert.That(actualValue, Is.Null, "Indexer should return null for index at the end of CFraction");
  }

  [Test]
  public void Indexer_EmptyCFraction_ReturnsNullForAnyIndex() {
    CFraction fraction = CFraction.Infinity;

    BigInteger? actualValueForZeroIndex  = fraction[0];
    BigInteger? actualValueForLargeIndex = fraction[10];

    Assert.That(actualValueForZeroIndex, Is.Null, "Indexer should return null for index 0 for empty CFraction");
    Assert.That(actualValueForLargeIndex, Is.Null, "Indexer should return null for large index for empty CFraction");
  }

}
