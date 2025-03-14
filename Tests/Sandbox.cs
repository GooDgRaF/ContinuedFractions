using ContinuedFractions;

namespace Tests;

public class Sandbox {

  public static void gcd(int a, int b, int c, int d) {
    // a*x + b = q * (c*x + d) + r

    while (c != 0) {
      int q  = a / c;
      int r1 = a - q * c;
      int r2 = b - q * d;

      Console.WriteLine($"{a}*x + {b}");

      a = c;
      b = d;
      c = r1;
      d = r2;
    }

    Console.WriteLine($"{a}*x + {b}");
  }

  public static void Main() { gcd(6, 8, 4, 6); }

}
