using System.Numerics;
using System.Threading.Channels;
using ContinuedFractions;

namespace Sandbox;

public class Sandbox {

  public static string CFPrint(CFraction cf) => $"{cf}\t\t== {(double)cf}";

  public static void Main(string[] args) {
    Console.WriteLine($"{CFPrint((CFraction.E + CFraction.Sqrt2) / 2)}");
  }

}
