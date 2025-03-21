using ContinuedFractions;
namespace Sandbox;


public class Sandbox {

  public static void Main(string[] args) {
    Console.WriteLine($"{(7*CFraction.FromRational(4,7) - 4 + 10) / 3}");
  }


}
