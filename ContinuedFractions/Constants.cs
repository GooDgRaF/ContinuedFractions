namespace ContinuedFractions;

public partial struct ContinuedFraction {

  public static ContinuedFraction E() => new ContinuedFraction(EGenerator());
  public static ContinuedFraction Sqrt2() => new ContinuedFraction(Sqrt2Generator());


  private static IEnumerable<int> EGenerator() {
    yield return 2;

    int k = 0;
    while (true) {
      yield return 1;
      yield return 2 * k + 2;
      yield return 1;

      k++;
    }
  }


  private static IEnumerable<int> Sqrt2Generator() {
    yield return 1;
    while (true) {
      yield return 2;
    }
  }

}
