using System.Numerics;
using NUnit.Framework.Legacy; // For SequenceEqual

namespace Tests
{
    [TestFixture]
    public class CFractionFactoryTests
    {
        [Test]
        public void FromRational_PositiveFraction_CorrectCoeffs()
        {
            BigInteger numerator = 22;
            BigInteger denominator = 7;
            List<int> expectedCoeffs = new List<int> { 3, 7 };

            CFraction fraction = CFraction.FromRational(numerator, denominator);
            List<int> actualCoeffs = fraction.Take(expectedCoeffs.Count);

            CollectionAssert.AreEqual(expectedCoeffs, actualCoeffs, "Coefficients for 22/7 should be [3, 7]");
        }

        [Test]
        public void FromRational_ZeroNumerator_CorrectCoeffs()
        {
            BigInteger numerator = 0;
            BigInteger denominator = 7;
            List<int> expectedCoeffs = new List<int> { 0 };

            CFraction fraction = CFraction.FromRational(numerator, denominator);
            List<int> actualCoeffs = fraction.Take(expectedCoeffs.Count);

            CollectionAssert.AreEqual(expectedCoeffs, actualCoeffs, "Coefficients for 0/7 should be [0]");
        }

        [Test]
        public void FromRational_Overflow_ThrowsException()
        {
            BigInteger numerator = BigInteger.Pow(int.MaxValue + 1L, 2);
            BigInteger denominator = 1;

            Assert.Throws<OverflowException>(() => CFraction.FromRational(numerator, denominator).Take(1),
                "Expected OverflowException if coefficient is out of int.MaxValue range");
        }

        [Test]
        public void FromCoeffs_ValidCoeffs_CorrectCFraction()
        {
            List<int> inputCoeffs = new List<int> { 1, 2, 3, 4 };
            List<int> expectedCoeffs = inputCoeffs;

            CFraction fraction = CFraction.FromCoeffs(inputCoeffs);
            List<int> actualCoeffs = fraction.Take(expectedCoeffs.Count);

            CollectionAssert.AreEqual(expectedCoeffs, actualCoeffs, "For valid coefficients, Take() should return the same");
        }

        [Test]
        public void FromCoeffs_ZeroInCoeffsAfterFirst_ThrowsException()
        {
            List<int> invalidCoeffs = new List<int> { 1, 2, 0, 4 };

            Assert.Throws<ArgumentException>(() => CFraction.FromCoeffs(invalidCoeffs),
                "Expected ArgumentException for zero coefficient after the first one");
        }

        [Test]
        public void FromCoeffs_ZeroFirstCoeff_ValidCFraction()
        {
            List<int> validCoeffsWithZeroFirst = new List<int> { 0, 2, 3, 4 };
            List<int> expectedCoeffs = validCoeffsWithZeroFirst;

            CFraction fraction = CFraction.FromCoeffs(validCoeffsWithZeroFirst);
            List<int> actualCoeffs = fraction.Take(expectedCoeffs.Count);

            CollectionAssert.AreEqual(expectedCoeffs, actualCoeffs, "Zero as the first coefficient should be valid");
        }

        [Test]
        public void FromCoeffs_EmptyList_ValidCFraction()
        {
            List<int> emptyCoeffs = new List<int>();
            List<int> expectedCoeffs = emptyCoeffs;

            CFraction fraction = CFraction.FromCoeffs(emptyCoeffs);
            List<int> actualCoeffs = fraction.Take(expectedCoeffs.Count);

            CollectionAssert.AreEqual(expectedCoeffs, actualCoeffs, "Empty coefficients list should be valid (infinity cf)");
        }

        [Test]
        public void FromGenerator_List_CorrectCFraction()
        {
            List<int> coeffs = new List<int> { 1, 2, 3 };
            IEnumerable<int> generator = coeffs;
            List<int> expectedCoeffs = coeffs;

            CFraction fraction = CFraction.FromGenerator(generator);
            List<int> actualCoeffs = fraction.Take(expectedCoeffs.Count);

            CollectionAssert.AreEqual(expectedCoeffs, actualCoeffs, "FromGenerator should work with List<int>");
        }

        [Test]
        public void FromGenerator_Array_CorrectCFraction()
        {
            int[] coeffs = new int[] { 1, 2, 3 };
            IEnumerable<int> generator = coeffs;
            List<int> expectedCoeffs = coeffs.ToList();

            CFraction fraction = CFraction.FromGenerator(generator);
            List<int> actualCoeffs = fraction.Take(expectedCoeffs.Count);

            CollectionAssert.AreEqual(expectedCoeffs, actualCoeffs, "FromGenerator should work with int[]");
        }

        [Test]
        public void FromGenerator_IEnumerable_CorrectCFraction()
        {
            IEnumerable<int> generator = GetTestGenerator();
            List<int> expectedCoeffs = new List<int> { 1, 2, 3 };

            CFraction fraction = CFraction.FromGenerator(generator);
            List<int> actualCoeffs = fraction.Take(expectedCoeffs.Count);

            CollectionAssert.AreEqual(expectedCoeffs, actualCoeffs, "FromGenerator should work with IEnumerable<int> generator");
        }

        private static IEnumerable<int> GetTestGenerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        // [Test]
        // public void FromRational_NegativeFraction_CorrectCoeffs()
        // {
        //     BigInteger numerator = -22;
        //     BigInteger denominator = 7;
        //     List<int> expectedCoeffs = new List<int> { -4, 1, 6 };
        //
        //     CFraction fraction = CFraction.FromRational(numerator, denominator);
        //     List<int> actualCoeffs = fraction.Take(expectedCoeffs.Count);
        //
        //     CollectionAssert.AreEqual(expectedCoeffs, actualCoeffs, "Coefficients for -22/7 should be [-4, 1, 6]");
        // }
    }
}