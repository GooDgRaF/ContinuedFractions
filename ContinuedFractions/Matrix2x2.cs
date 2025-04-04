using System.Diagnostics.CodeAnalysis;

namespace ContinuedFractions;

internal readonly struct Matrix22 {

  private readonly BigInteger[] _m;

  public BigInteger this[int i] => _m[i];

  public Matrix22(BigInteger a11, BigInteger a12, BigInteger a21, BigInteger a22) { _m = new[] { a11, a12, a21, a22 }; }

  public static Matrix22 operator *(Matrix22 left, Matrix22 right) {
    return new Matrix22
      (
       left._m[0] * right._m[0] + left._m[1] * right._m[2] // a11*b11 + a12*b21
     , left._m[0] * right._m[1] + left._m[1] * right._m[3] // a11*b12 + a12*b22
     , left._m[2] * right._m[0] + left._m[3] * right._m[2] // a21*b11 + a22*b21
     , left._m[2] * right._m[1] + left._m[3] * right._m[3] // a21*b12 + a22*b22
      );
  }

  public override string ToString() { return $"{_m[0]} {_m[1]}\n{_m[2]} {_m[3]}"; }

  private static bool TryProduceTerm(Matrix22 m, [NotNullWhen(true)] out BigInteger? q, [NotNullWhen(true)] out Matrix22? r) {
    q = null;
    r = null;
    BigInteger a = m[0], b = m[1], c = m[2], d = m[3];

    // Проверка знаменателей
    if (c == 0 && d == 0) {
      // Числитель/(0x+0) - неопределенность или константа/0
      // Это должно быть обработано до вызова TryProduceTerm или здесь как ошибка
      throw new ArgumentException("TryProduceTerm: Second row of the matrix is zero.");
    }

    // Если один из пределов бесконечен (c=0 или d=0), мы не можем определить конечное q
    if (c == 0 || d == 0) {
      return false;
    }

    // Знаменатель пересекает ноль в области x > 0.
    if (c.Sign * d.Sign == -1) {
      return false;
    }

    BigInteger floor_ac = GosperMatrix.FloorDiv(a, c);
    BigInteger floor_bd = GosperMatrix.FloorDiv(b, d);

    if (floor_ac == floor_bd) {
      q = floor_ac;
      BigInteger r10 = a - q.Value * c; // Используем q.Value, т.к. он точно не null здесь
      BigInteger r11 = b - q.Value * d;
      r = new Matrix22(c, d, r10, r11);

      return true;
    }

    bool ac_is_integer = a % c == 0;
    bool bd_is_integer = b % d == 0;

    if (floor_ac == floor_bd + 1 && ac_is_integer) // a/c = floor_ac, floor(b/d)=floor_ac-1
    {
      q = floor_bd;
      BigInteger r10 = a - q.Value * c;
      BigInteger r11 = b - q.Value * d;
      r = new Matrix22(c, d, r10, r11);

      return true;
    }

    if (floor_bd == floor_ac + 1 && bd_is_integer) // b/d = floor_bd, floor(a/c)=floor_bd-1
    {
      q = floor_ac;
      BigInteger r10 = a - q.Value * c;
      BigInteger r11 = b - q.Value * d;
      r = new Matrix22(c, d, r10, r11);

      return true;
    }

    // Если ни одно из условий не выполнилось, q определить нельзя
    return false;
  }

  internal static Matrix22 Homographic(BigInteger a11) => new Matrix22(a11, 1, 1, 0);

  internal static IEnumerable<(BigInteger q, Matrix22 r)> GenerateLFTSteps(Matrix22 m) {
    while (true) {
      if (m[0] == 0 && m[1] == 0) { // числитель равен нулю
        break;
      }

      if (!TryProduceTerm(m, out BigInteger? q, out Matrix22? r)) {
        break;
      }

      yield return ((BigInteger)q, (Matrix22)r);

      m = (Matrix22)r;
    }
  }

}
