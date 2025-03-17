using ContinuedFractions;

namespace Tests;

public class Sandbox {

  public static void Main() {
    // var x = ContinuedFraction.E();
    var x = new ContinuedFraction(new List<int>(){2,1,1,5,1});
    var y = x.CF_transform(new Matrix22(2,0,0,1));
    Console.WriteLine($"{y}");
  }

}
