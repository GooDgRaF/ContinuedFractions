namespace ContinuedFractions;

public partial struct CFraction : IComparable<CFraction>, IEquatable<CFraction> {

  private const int comparationCut = 41;


  public int CompareTo(CFraction other) { // A < B <==> lexmin([a0;-a1,a2,-a3,...,a2k,-a(2k+1),...], [b0;-b1,b2,-b3,...,b2k,-b(2k+1),...])
    int i = 0;
    while (i < comparationCut) {
      int? val1 = this[i];
      int? val2 = other[i];

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

  public bool Equals(CFraction other) => this.CompareTo(other) == 0;

  public static bool operator ==(CFraction left, CFraction right) { return left.Equals(right); }

  public static bool operator !=(CFraction left, CFraction right) { return !(left == right); }

  public override bool Equals(object? obj) { return obj is CFraction other && Equals(other); }

  public override int GetHashCode() { // можно договориться, что кэш берём с первых 40 элементов
    throw new NotImplementedException();
  }

}
