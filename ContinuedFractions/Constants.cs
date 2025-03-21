namespace ContinuedFractions;

public partial struct CFraction {

  public static CFraction E        = new CFraction(EGenerator());
  public static CFraction Sqrt2    = new CFraction(Sqrt2Generator());
  public static CFraction Infinity = new CFraction(new int[] { });
  public static CFraction Zero     = new CFraction(new int[] { 0 });


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
