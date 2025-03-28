using System.Threading.Channels;
using ContinuedFractions;

namespace Sandbox;

public class Sandbox {

  public static string CFPrint(CFraction cf) => $"{cf}\t\t== {(double)cf}";

  public static void Main(string[] args) {
    CFraction cf = CFraction.FromGenerator(new int[] { -1, 1, 1 }.Concat(Enumerable.Repeat(2, 100)));
    Console.WriteLine($"{(double)cf}");
    Console.WriteLine($"{-double.Sqrt(2)+1}");
  }

}
