namespace ContinuedFractions;


public readonly partial struct ContinuedFraction : IComparable<ContinuedFraction>, IEquatable<ContinuedFraction> {

  private const int compCut = 100;


  public int CompareTo(ContinuedFraction other) {
    using var cf_this = _cf.GetEnumerator();
    using var cf_other = other._cf.GetEnumerator();

    int index = 0;
    while (index < compCut) {
      bool next1 = cf_this.MoveNext();
      bool next2 = cf_other.MoveNext();

      if (!next1 && !next2) {
        return 0;
      }

      if (!next1) { // смотри дерево Штерна-Броко
        return cf_other.Current;
      }

      if (!next2) {
        return -cf_this.Current;
      }

      int val1 = cf_this.Current;
      int val2 = cf_other.Current;

      if (index % 2 != 0) { // для нечетных индексов меняем знак
        val1 = -val1;
        val2 = -val2;
      }

      if (val1 < val2) {
        return -1;
      }
      if (val1 > val2) {
        return 1;
      }

      index++;
    }

    return 0;
  }

  public bool Equals(ContinuedFraction other) => this.CompareTo(other) == 0;

  public override bool Equals(object? obj) {
    return obj is ContinuedFraction other && Equals(other);
  }

  public override int GetHashCode() { // можно договориться, что кэш берём с первых 40 элементов
    throw new NotImplementedException();
  }

}
