namespace ContinuedFractions;

public partial class CFraction : IComparable<CFraction>, IEquatable<CFraction>, IComparisonOperators<CFraction, CFraction, bool> {

  public int CompareTo(CFraction? other) {
    if (other is null) {
      return 1;
    }

    // A < B <==> isLexmin([a0;-a1,a2,-a3,...,a2k,-a(2k+1),...], [b0;-b1,b2,-b3,...,b2k,-b(2k+1),...])
    int i = 0;
    while (i <= numberOfCoeffs) {
      BigInteger? val1 = this[i];
      BigInteger? val2 = other[i];

      if (val1 == null && val2 == null) {
        return 0;
      }

      if (val2 == null) { // cf_other закончилась раньше
        return (i % 2 == 0) ? -1 : 1;
      }

      if (val1 == null) { // cf_this закончилась раньше
        return (i % 2 == 0) ? 1 : -1;
      }


      if (i % 2 != 0) { // для нечетных индексов меняем знак
        val1 = -val1;
        val2 = -val2;
      }

      if (val1 < val2) {
        return -1;
      }
      if (val1 > val2) {
        return 1;
      }

      i++;
    }

    return 0;
  }

  public bool Equals(CFraction? other) => this.CompareTo(other) == 0;

  public static bool operator ==(CFraction? left, CFraction? right) {
    if (left is null && right is null) {
      return true;
    }

    if (left is null || right is null) {
      return false;
    }

    return left.Equals(right);
  }

  public static bool operator !=(CFraction? left, CFraction? right) { return !(left == right); }

  public override bool Equals(object? obj) { return obj is CFraction other && Equals(other); }

  public override int GetHashCode() {
    if (this == Infinity) {
      return int.MaxValue;
    }
    HashCode hash = new HashCode();
    foreach (int term in this.Take(numberOfCoeffs)) {
      hash.Add(term);
    }

    return hash.ToHashCode();
  }

  public static bool operator >(CFraction  left, CFraction right) => left.CompareTo(right) > 0;
  public static bool operator >=(CFraction left, CFraction right) => left.CompareTo(right) >= 0;
  public static bool operator <(CFraction  left, CFraction right) => left.CompareTo(right) < 0;
  public static bool operator <=(CFraction left, CFraction right) => left.CompareTo(right) <= 0;

}
