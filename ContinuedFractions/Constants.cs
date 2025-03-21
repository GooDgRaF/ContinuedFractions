namespace ContinuedFractions;

public partial struct CFraction {

  public static readonly CFraction E     = new CFraction(EGenerator());
  public static readonly CFraction Sqrt2 = new CFraction(Sqrt2Generator());
  public static readonly CFraction Infinity = new CFraction(new List<int>());


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
