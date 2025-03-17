namespace ContinuedFractions;


public readonly partial struct ContinuedFraction{

  public static ContinuedFraction operator +(ContinuedFraction cf, Frac frac)
    => cf.CF_transform(new Matrix22(frac.q, frac.p, 0, frac.q));

}
