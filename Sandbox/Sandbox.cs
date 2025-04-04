using ContinuedFractions;

namespace Sandbox;

public class Sandbox {

  public static string CFPrint(CFraction cf) => $"{cf}\t\t== {(double)cf}";

  public static void Main(string[] args) {
    Console.WriteLine($"{CFPrint((CFraction.E + 1 )/ (CFraction.E - 1))}");
  }

}
