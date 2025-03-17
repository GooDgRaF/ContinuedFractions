using ContinuedFractions;

namespace Tests;

public class Sandbox {

  public static void Main() {
    // Console.WriteLine($"{ContinuedFraction.FromRational(66,13)}");
    var x = ContinuedFraction.E();
    // var x = new ContinuedFraction(new List<int>(){2,1,1,5,1});
    var y = x.CF_transform(new Matrix22(2,0,0,1));
    foreach (int i in y.Take(10)) {
      Console.Write($"{i} ");
    }
  }

}
