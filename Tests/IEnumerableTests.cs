using NUnit.Framework;
using ContinuedFractions;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class EnumerableTests
    {
        [Test]
        public void Enumerable_FiniteCFraction_CorrectCoefficients()
        {
            CFraction fraction = CFraction.FromRational(10, 7); // [1; 2, 3]
            List<int> expectedCoeffs = new List<int> { 1, 2, 3 };
            List<int> actualCoeffs = new List<int>();

            foreach (int coeff in fraction)
            {
                actualCoeffs.Add(coeff);
            }

            Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "For finite CFraction, should iterate through all coefficients");
        }

        [Test]
        public void Enumerable_EmptyCFraction_NoCoefficients()
        {
            CFraction fraction = CFraction.FromCoeffs(new List<int>()); // Empty CFraction
            List<int> actualCoeffs = new List<int>();

            foreach (int coeff in fraction)
            {
                actualCoeffs.Add(coeff);
            }

            Assert.That(actualCoeffs, Is.Empty, "For empty CFraction, should not iterate through any coefficients"); // Is.Empty для проверки пустой коллекции
        }

        [Test]
        public void Enumerable_ResetEnumerator_IterateFromBeginning()
        {
            CFraction fraction = CFraction.FromRational(10, 7); // [1; 2, 3]
            List<int> expectedCoeffs = new List<int> { 1, 2, 3, 1, 2, 3 };
            List<int> actualCoeffs = new List<int>();

            using var enumerator = fraction.GetEnumerator();

            while (enumerator.MoveNext())
            {
                actualCoeffs.Add(enumerator.Current);
            }

            enumerator.Reset();

            while (enumerator.MoveNext())
            {
                actualCoeffs.Add(enumerator.Current);
            }

            Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Reset should allow to iterate from beginning again");
        }

        [Test]
        public void Enumerable_ManualMoveNext_CurrentValuesCorrect()
        {
            CFraction fraction = CFraction.FromRational(10, 7); // [1; 2, 3]
            List<int> expectedCoeffs = new List<int> { 1, 2, 3 };
            List<int> actualCoeffs = new List<int>();
            var enumerator = fraction.GetEnumerator();

            Assert.That(enumerator.MoveNext(), Is.True, "MoveNext should return true for the first coefficient");
            Assert.That(enumerator.Current, Is.EqualTo(1), "Current should be the first coefficient");
            actualCoeffs.Add(enumerator.Current);

            Assert.That(enumerator.MoveNext(), Is.True, "MoveNext should return true for the second coefficient");
            Assert.That(enumerator.Current, Is.EqualTo(2), "Current should be the second coefficient");
            actualCoeffs.Add(enumerator.Current);

            Assert.That(enumerator.MoveNext(), Is.True, "MoveNext should return true for the third coefficient");
            Assert.That(enumerator.Current, Is.EqualTo(3), "Current should be the third coefficient");
            actualCoeffs.Add(enumerator.Current);

            Assert.That(enumerator.MoveNext(), Is.False, "MoveNext should return false after the last coefficient");

            Assert.That(actualCoeffs, Is.EqualTo(expectedCoeffs), "Coefficients collected with manual MoveNext should be correct");
        }
    }
}