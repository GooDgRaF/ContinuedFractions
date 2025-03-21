using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace ContinuedFractions;

public partial struct CFraction :
  // IUnaryNegationOperators<CFraction, CFraction> ,
  IUnaryPlusOperators<CFraction, CFraction> {

  private static readonly int[] zeroForRcp = new int[] { 0 };

  public CFraction Rcp() {
    return this[0] switch
             {
               null => FromRational(0, 1)                     // 1/inf --> 0
             , 0    => FromGenerator(this.Skip(1))            // 1/[0;a0,a1,..] --> [a0;a1,...]
             , > 0  => FromGenerator(zeroForRcp.Concat(this)) // 1/[a0;a1,...] --> [0;a0,a1,..]
             , _    => throw new NotImplementedException("я хз что делать с отрицательными числами пока что")
             };
  }

  public static CFraction operator +(CFraction value) => value;

  // public static CFraction operator -(CFraction value) { // хз что это такое пока что
  //   throw new NotImplementedException();
  // }

  public static CFraction operator +(CFraction cf,   Frac      frac) => cf.CF_transform(new Matrix22(frac.q, frac.p, 0, frac.q));
  public static CFraction operator +(Frac      frac, CFraction cf)   => cf + frac;


  public static CFraction operator *(CFraction cf,   Frac      frac) => cf.CF_transform(new Matrix22(frac.p, 0, 0, frac.q));
  public static CFraction operator *(Frac      frac, CFraction cf)   => cf * frac;


  public static CFraction operator /(CFraction cf,   Frac      frac) => cf.CF_transform(new Matrix22(frac.q, 0, 0, frac.p));
  public static CFraction operator /(Frac      frac, CFraction cf)   => cf / frac;


  public CFraction CF_transform(Matrix22 init) => new CFraction(CF_transform_main(init));

  public IEnumerable<int> CF_transform_main(Matrix22 init) {
    Matrix22 m = init;
    int      i = 0;

    while (true) {
      int? val = this[i];
      if (val == null) {
        break;
      }
      m = LFT_step(m, (int)val);
      foreach ((int q, Matrix22 r) gcd in GCD(m)) {
        m = gcd.r;

        yield return gcd.q;
      }

      i++;
    }

    // Если исходная непрерывная дробь была конечной, то надо выполнить ещё раз алгоритм Евклида.
    if (i != 0 && m[2] != 0) {
      foreach (int k in GCD(m[0], m[2]))
        yield return k;
    }
  }

  private static Matrix22 LFT_step(Matrix22 m, int a) => m * Matrix22.Homographic(a);

  private static IEnumerable<int> GCD(int a, int b) {
    while (b != 0) {
      int q = Math.DivRem(a, b, out int r);

      yield return q;

      a = b;
      b = r;
    }
  }

  private static bool GCD_step(Matrix22 m, [NotNullWhen(true)] out int? q, [NotNullWhen(true)] out Matrix22? r) {
    if (m[2] == 0 && m[3] == 0) {
      throw new ArgumentException($"GCD_step: That series has already ended. Found: the second row of matrix is zero.");
    }

    q = null;
    r = null;

    if (m[2] == 0 || m[3] == 0) { // Функция неограниченна, недостаточно информации для определения частного
      return false;
    }

    if (m[2] != 0 && m[3] < 0) { // Есть ноль в области определения
      return false;
    }

    // todo: сделать целочисленные вычисления в дробях явно

    // Функция ограничена
    double n0 = (double)m[0] / m[2];
    double n1 = (double)m[1] / m[3];

    double v0 = Math.Min(n0, n1);
    double v1 = Math.Max(n0, n1);

    int d0 = (int)Math.Floor(v0);
    int d1 = (int)Math.Floor(v1);

    // если d1 == d0, то частное определяется однозначно,
    // если d1 отличается больше чем на 1 от d0, то ничего определённого сказать нельзя
    // если d1 == d0 + 1 и d1 == v1, то эта граница достигается только либо в 0, либо в oo; поэтому ответ d0.
    if (d1 == d0 || (d1 == d0 + 1 && Math.Abs(d1 - v1) < 1e-14)) {
      q = d0;
      r = new Matrix22(m[2], m[3], m[0] - d0 * m[2], m[1] - d0 * m[3]);

      return true;
    }

    return false;
  }

  public static IEnumerable<(int q, Matrix22 r)> GCD(Matrix22 m) {
    while (true) {
      if (m[0] == 0 && m[1] == 0) { // числитель равен нулю
        break;
      }

      if (!GCD_step(m, out int? q, out Matrix22? r)) {
        break;
      }

      yield return ((int)q, (Matrix22)r);

      m = (Matrix22)r;
    }
  }

}
