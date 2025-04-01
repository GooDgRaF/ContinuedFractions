namespace Tests;

[TestFixture]
public class EnumerableTests {

  [Test]
  public void Enumerable_FiniteCFraction_CorrectCoefficients() {
    CFraction fraction       = CFraction.FromRational(10, 7); // [1; 2, 3]
    List<int> expectedCoeffs = new List<int> { 1, 2, 3 };
    List<int> actualCoeffs   = new List<int>();

    foreach (int coeff in fraction) {
      actualCoeffs.Add(coeff);
    }

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "For finite CFraction, should iterate through all coefficients");
  }

  [Test]
  public void Enumerable_EmptyCFraction_NoCoefficients() {
    CFraction fraction     = CFraction.FromCoeffs(new List<int>()); // Empty CFraction
    List<int> actualCoeffs = new List<int>();

    foreach (int coeff in fraction) {
      actualCoeffs.Add(coeff);
    }

    Assert.That(actualCoeffs, Is.Empty, "For empty CFraction, should not iterate through any coefficients");
  }

  [Test]
  public void Enumerable_ResetEnumerator_IterateFromBeginning() {
    CFraction fraction = CFraction.FromRational(10, 7); // [1; 2, 3]
    List<int> expectedCoeffs =
      new List<int>
        {
          1
        , 2
        , 3
        , 1
        , 2
        , 3
        };
    List<int> actualCoeffs = new List<int>();

    using var enumerator = fraction.GetEnumerator();

    while (enumerator.MoveNext()) {
      actualCoeffs.Add(enumerator.Current);
    }

    enumerator.Reset();

    while (enumerator.MoveNext()) {
      actualCoeffs.Add(enumerator.Current);
    }

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Reset should allow to iterate from beginning again");
  }

  [Test]
  public void Enumerable_ManualMoveNext_CurrentValuesCorrect() {
    CFraction fraction       = CFraction.FromRational(10, 7); // [1; 2, 3]
    List<int> expectedCoeffs = new List<int> { 1, 2, 3 };
    List<int> actualCoeffs   = new List<int>();
    var       enumerator     = fraction.GetEnumerator();

    Assert.That(enumerator.MoveNext(), Is.True, "MoveNext should return true for the first coefficient");
    Assert.That(enumerator.Current, Is.EqualTo(1), "Current should be the first coefficient");
    actualCoeffs.Add(enumerator.Current);

    Assert.That(enumerator.MoveNext(), Is.True, "MoveNext should return true for the second coefficient");
    Assert.That(enumerator.Current, Is.EqualTo(2), "Current should be the second coefficient");
    actualCoeffs.Add(enumerator.Current);

    Assert.That(enumerator.MoveNext(), Is.True, "MoveNext should return true for the third coefficient");
    Assert.That(enumerator.Current, Is.EqualTo(3), "Current should be the third coefficient");
    actualCoeffs.Add(enumerator.Current);

    Assert.That(enumerator.MoveNext(), Is.False, "MoveNext should return false after the last coefficient");

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients collected with manual MoveNext should be correct");
  }

#region Cache and Enumerable Interaction Tests
  [Test]
  public void Enumerable_PartialCache_FullIteration() {
    CFraction fraction      = CFraction.E;
    var       _             = fraction[1];
    List<int> actualCoeffs  = new List<int>();
    int       expectedCount = 10;

    foreach (int coeff in fraction.Take(expectedCount)) {
      actualCoeffs.Add(coeff);
    }

    List<int> expectedCoeffs = CFraction.EGenerator().Take(expectedCount).ToList();
    Assert.That
      (actualCoeffs, Is.EqualTo(expectedCoeffs), "Full iteration after partial cache should return correct coefficients");
  }

  [Test]
  public void Enumerable_CacheExpansionDuringIteration() {
    CFraction fraction       = CFraction.FromRational(142, 43); // [3; 3, 3, 4]
    List<int> actualCoeffs   = new List<int>();
    List<int> expectedCoeffs = new List<int> { 3, 3, 3, 4 };

    foreach (int coeff in fraction) {
      actualCoeffs.Add(coeff);
    }

    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Iteration should expand cache and return all coefficients");
    Assert.That(fraction[3], Is.EqualTo(4), "Cache should be fully populated after iteration");
  }

  [Test]
  public void Enumerable_IterationAfterIndexerAccess() {
    CFraction fraction = CFraction.FromRational(355, 113); // [3; 7, 16]
    _ = fraction[2];
    _ = fraction[0];
    _ = fraction[1];

    List<int> actualCoeffs = new List<int>();
    foreach (int coeff in fraction) {
      actualCoeffs.Add(coeff);
    }
    List<int> expectedCoeffs = new List<int> { 3, 7, 16 };
    Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Iteration after indexer access should return correct coefficients");
  }

  [Test]
  public void Enumerable_PartialIteration_WithCaching() {
    CFraction fraction = CFraction.E;
    _ = fraction.Take(5);

    List<int> nextCoeffs         = fraction.Skip(5).Take(5).ToList();
    List<int> expectedNextCoeffs = CFraction.EGenerator().Take(10).Skip(5).ToList();
    Assert.That
      (
       nextCoeffs
     , Is.EqualTo(expectedNextCoeffs)
     , "Subsequent iteration should return correct coefficients after initial caching"
      );
  }
#endregion

}
