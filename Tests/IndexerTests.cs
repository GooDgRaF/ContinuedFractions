namespace Tests;

[TestFixture]
public class IndexerTests {

  [Test]
  public void Indexer_OneElements() {
    List<int> coeffs   = new List<int> { 1 };
    CFraction fraction = CFraction.FromGenerator(coeffs);

    int? actualValue = fraction[0];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo(1), "Indexer should return the first coefficient for index 0");
  }

  [Test]
  public void Indexer_TwoElementsOnes() {
    List<int> coeffs   = new List<int> { 1, 1 };
    CFraction fraction = CFraction.FromGenerator(coeffs);

    int? actualValue = fraction[0];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo(2), "Indexer should return the digit 2");
  }

  [Test]
  public void Indexer_TwoElements() {
    List<int> coeffs   = new List<int> { 1, 2 };
    CFraction fraction = CFraction.FromGenerator(coeffs);

    int? actualValue = fraction[0];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo(1), "Indexer should return the first coefficient for index 0");
  }

  [Test]
  public void Indexer_ValidIndexWithinCache() {
    List<int> coeffs   = new List<int> { 1, 2, 3, 4, 5 };
    CFraction fraction = CFraction.FromGenerator(coeffs);
    fraction.Take(3);

    int? actualValue = fraction[1];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo(2), "Indexer should return cached value for index within cache");
  }


  [Test]
  public void Indexer_ValidIndexOutsideCache() {
    List<int> coeffs   = new List<int> { 1, 2, 3, 4, 5 };
    CFraction fraction = CFraction.FromGenerator(coeffs);
    fraction.Take(2);

    int? actualValue = fraction[3];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo(4), "Indexer should cache and return value for index outside cache");
    Assert.That(fraction.Take(4).Count, Is.EqualTo(4), "Indexer should extend cache to requested index");
  }

  [Test]
  public void Indexer_IndexAtEnd_ReturnsNull() {
    List<int> coeffs   = new List<int> { 1, 2, 3 };
    CFraction fraction = CFraction.FromGenerator(coeffs);

    int? actualValue = fraction[3]; // Index at the end

    Assert.That(actualValue, Is.Null, "Indexer should return null for index at the end of CFraction");
  }

  [Test]
  public void Indexer_EmptyCFraction_ReturnsNullForAnyIndex() {
    CFraction fraction = CFraction.Infinity;

    int? actualValueForZeroIndex  = fraction[0];
    int? actualValueForLargeIndex = fraction[10];

    Assert.That(actualValueForZeroIndex, Is.Null, "Indexer should return null for index 0 for empty CFraction");
    Assert.That(actualValueForLargeIndex, Is.Null, "Indexer should return null for large index for empty CFraction");
  }

}
