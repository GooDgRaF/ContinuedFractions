using System.Numerics;

namespace ContinuedFractions;

public readonly struct Matrix22 : IAdditionOperators<Matrix22, Matrix22, Matrix22>
                                , IMultiplyOperators<Matrix22, Matrix22, Matrix22> {

  private readonly int[] _m;

  public Matrix22(int a11, int a12, int a21, int a22) { _m = new[] { a11, a12, a21, a22 }; }

  public static Matrix22 Homographic(int a11) => new Matrix22(a11, 1, 1, 0);

  public static Matrix22 operator +(Matrix22 left, Matrix22 right) {
    return new Matrix22(left._m[0] + right._m[0], left._m[1] + right._m[1], left._m[2] + right._m[2], left._m[3] + right._m[3]);
  }

  public static Matrix22 operator *(Matrix22 left, Matrix22 right) {
    // Матричное умножение для 2x2:
    // [a11 a12] * [b11 b12] = [a11*b11+a12*b21 a11*b12+a12*b22]
    // [a21 a22]   [b21 b22]   [a21*b11+a22*b21 a21*b12+a22*b22]

    return new Matrix22
      (
       left._m[0] * right._m[0] + left._m[1] * right._m[2] // a11*b11 + a12*b21
     , left._m[0] * right._m[1] + left._m[1] * right._m[3] // a11*b12 + a12*b22
     , left._m[2] * right._m[0] + left._m[3] * right._m[2] // a21*b11 + a22*b21
     , left._m[2] * right._m[1] + left._m[3] * right._m[3] // a21*b12 + a22*b22
      );
  }

  public static Matrix22 Id() => new Matrix22(1, 0, 0, 1);

  public override string ToString() { return $"{_m[0]} {_m[1]}\n{_m[2]} {_m[3]}"; }
}
