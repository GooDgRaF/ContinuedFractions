using System.Numerics;

namespace ContinuedFractions;


public readonly partial struct ContinuedFraction :
  // IUnaryNegationOperators<ContinuedFraction, ContinuedFraction> ,
 IUnaryPlusOperators<ContinuedFraction,ContinuedFraction>
{

  public static ContinuedFraction operator +(ContinuedFraction value) => value;

  // public static ContinuedFraction operator -(ContinuedFraction value) { // хз что это такое пока что
  //   throw new NotImplementedException();
  // }

  public static ContinuedFraction operator +(ContinuedFraction cf, Frac frac)
    => cf.CF_transform(new Matrix22(frac.q, frac.p, 0, frac.q));

  public static ContinuedFraction operator +(Frac frac, ContinuedFraction cf) => cf + frac;

}
