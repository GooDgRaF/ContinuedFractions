using ContinuedFractions;
namespace Sandbox;


public class Sandbox {

  public static void Main(string[] args) {

    ContinuedFraction x = ContinuedFraction.E().CF_transform(new Matrix22(2,0,0,1));
    Console.WriteLine($"{x}");
  }


}
