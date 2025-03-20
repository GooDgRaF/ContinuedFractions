namespace Tests;

[TestFixture]
public class CFractionIndexerTests {

  [Test]
  public void Indexer_ValidIndexWithinCache_ReturnsCachedNullableValue() {
    List<int> coeffs   = new List<int> { 1, 2, 3, 4, 5 };
    CFraction fraction = CFraction.FromCoeffs(coeffs);
    fraction.Take(3);

    int? actualValue = fraction[1];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo(2), "Indexer should return cached value for index within cache");
  }

  [Test]
  public void Indexer_ValidIndexOutsideCache_CachesAndReturnsNullableValue() {
    List<int> coeffs   = new List<int> { 1, 2, 3, 4, 5 };
    CFraction fraction = CFraction.FromCoeffs(coeffs);
    fraction.Take(2);

    int? actualValue = fraction[3];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo(4), "Indexer should cache and return value for index outside cache");
    Assert.That(fraction.Take(4).Count, Is.EqualTo(4), "Indexer should extend cache to requested index");
  }

  [Test]
  public void Indexer_IndexZero_ReturnsFirstNullableCoefficient() {
    List<int> coeffs   = new List<int> { 10, 20, 30 };
    CFraction fraction = CFraction.FromCoeffs(coeffs);

    int? actualValue = fraction[0];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo(10), "Indexer should return the first coefficient for index 0");
  }

  [Test]
  public void Indexer_LargeValidIndex_ReturnsCorrectNullableValue() {
    List<int> coeffs   = Enumerable.Range(1, 100).ToList();
    CFraction fraction = CFraction.FromCoeffs(coeffs);

    int? actualValue = fraction[99];

    Assert.That(actualValue.HasValue, Is.True, "Indexer should return a nullable value with a value");
    Assert.That(actualValue.Value, Is.EqualTo(100), "Indexer should handle large valid indices");
  }

  [Test]
  public void Indexer_IndexAtEnd_ReturnsNull() {
    List<int> coeffs   = new List<int> { 1, 2, 3 };
    CFraction fraction = CFraction.FromCoeffs(coeffs);

    int? actualValue = fraction[3]; // Index at the end

    Assert.That(actualValue, Is.Null, "Indexer should return null for index at the end of CFraction");
  }

  [Test]
  public void Indexer_IndexOutOfRange_ThrowsIndexOutOfRangeException() {
    List<int> coeffs   = new List<int> { 1, 2, 3 };
    CFraction fraction = CFraction.FromCoeffs(coeffs);

    Assert.Throws<IndexOutOfRangeException>
      (
       ()
         => {
         var temp = fraction[4];
       }
     , // Index out of range
       "Indexer should throw IndexOutOfRangeException for index out of range"
      );
  }

  [Test]
  public void Indexer_IndexOutOfRangeAfterPartialCache_ThrowsIndexOutOfRangeException() {
    List<int> coeffs   = new List<int> { 1, 2, 3 };
    CFraction fraction = CFraction.FromCoeffs(coeffs);
    fraction.Take(2);

    Assert.Throws<IndexOutOfRangeException>
      (
       ()
         => {
         var temp = fraction[4];
       }
     , // Index out of range
       "Indexer should throw IndexOutOfRangeException for index out of range even after partial cache"
      );
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
