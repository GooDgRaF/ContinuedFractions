# ContinuedFractions

A C# library providing a `CFraction` type to represent finite and potentially infinite continued fractions using `System.Numerics.BigInteger`. Features include lazy evaluation of coefficients, caching, overloaded arithmetic operators, and conversion capabilities.

## Features

* **Representation:**
    * Represents continued fractions with `System.Numerics.BigInteger` coefficients.
    * Supports both finite and potentially infinite fractions (using `IEnumerable<BigInteger>` generators).
    * **Lazy Evaluation:** Coefficients of infinite fractions are computed only when needed.
    * **Caching:** Computed coefficients are cached for efficient reuse.
* **Arithmetic Operations:**
    * Overloaded operators (`+`, `-`, `*`, `/`) for operations between:
        * `CFraction` and `CFraction` (using Gosper's algorithm).
        * `CFraction` and `BigInteger`.
        * `CFraction` and `Frac = (BigInteger p, BigInteger q)` (rational numbers).
    * Unary operators (`+`, `-`) and reciprocal calculation (`.Rcp()`).
* **Creation and Conversion:**
    * Create `CFraction` instances from:
        * Rational numbers (`CFraction.FromRational(p, q)`).
        * Lists of coefficients (`CFraction.FromCoeffs(...)`).
        * Coefficient generators (`CFraction.FromGenerator(...)`).
    * Generate the sequence of convergents (rational approximations) (`.GenerateConvergents()`).
    * Convert to the exact rational value (`.ToFrac()`), if the fraction is finite or fully evaluated.
    * Convert to `double` (`(double)cfraction`) for an approximate value.
* **Comparison:**
    * Overloaded comparison operators (`==`, `!=`, `<`, `>`, `<=`, `>=`).
    * *Note: Comparisons are based on a fixed number of coefficients (`numberOfCoeffs`) and may not be exact for fractions differing only at higher indices.*
* **Constants:**
    * Predefined instances for `Zero`, `One`, `Infinity`, `E`, and `Sqrt2`.
* **Underlying Algorithms:**
    * Uses Linear Fractional Transformations for unary and mixed-type operations.
    * Implements Gosper's algorithm for binary operations between `CFraction` instances.

## Installation

Currently, the library is not available on NuGet. To use it in your project:

1. **Download or Clone:**
    * Download the source code as a ZIP archive from the GitHub repository page and extract it.
    * Alternatively, clone the repository using Git:
      ```bash
      git clone https://github.com/YOUR_USERNAME/ContinuedFractions.NET.git
      ```
2. **Build the Library:**
    * Open the solution file `ContinuedFractions.sln` in IDE and build the `ContinuedFractions` project (usually in Release mode).
    * Or, navigate to the project directory (`ContinuedFractions` folder containing `ContinuedFractions.csproj`) in your terminal and run the .NET CLI build command:
      ```bash
      dotnet build -c Release
      ```
3. **Reference the DLL:**
    * The build process will create a `ContinuedFractions.dll` file in the output directory (e.g., `bin/Release/net9.0/`).
    * In your own project (in Visual Studio or your IDE):
        * Right-click on "Dependencies" (or "References").
        * Select "Add Project Reference..." if you included the `ContinuedFractions` project directly in your solution, OR select "Add Assembly Reference..." (or "Browse...") and navigate to the location of the `ContinuedFractions.dll` file you built in the previous step.
4. **Add Using Directive:**
    * In your C# code files where you want to use the library, add the using directive:
      ```csharp
      using ContinuedFractions;
      ```

## Key Concepts and Implementation Details

* **Arbitrary Precision Coefficients:** Uses `System.Numerics.BigInteger` for coefficients (`a_i`), allowing arbitrary precision for individual terms.
* **Exact Arithmetic:** Internal calculations within arithmetic algorithms use `BigInteger`, ensuring these steps are exact.
* **Lazy Evaluation and Caching:** Coefficients from generators (`CFraction.FromGenerator`) are computed on demand and cached.
* **Canonical Form:** Finite fractions ending in `[..., a, 1]` are automatically stored as `[..., a+1]` (except `[1]`).
* **Limited Precision for Comparisons and Output:**
    * Value comparisons (`==`, `<`, etc.), `GetHashCode()`, `ToString()` are limited by `numberOfCoeffs` (default: 40).
    * Two fractions differing only after this limit compares as equal.
* **Infinity Handling:**
    * `CFraction.Infinity` represents infinity `[]` with no sign.
    * Arithmetic handles `Infinity` appropriately (e.g., `X + Infinity = Infinity`, `X / Infinity = 0`).
* **No Negative Infinity:** No distinct representation for negative infinity. Results like `X - Infinity` will return `CFraction.Infinity`.

## Limitations

* **Comparison Precision Limit:** As mentioned above, comparisons (`==`, `<`, etc.) are fundamentally limited by `numberOfCoeffs`. They test for equality *of the first `numberOfCoeffs` coefficients* (considering the alternating sign logic), not true numerical equality up to infinite precision.
* **(Fundamental) Result Representation vs. Mathematical Identity:**
    * **Example:** Mathematically, `Sqrt2 * Sqrt2` equals `2`. The library represents `Sqrt2` as `[1; 2, 2, 2, ...]`. When multiplying these two sequences using Gosper's algorithm, the result can't be obtained in finite time.
    * **Consequence:** Comparing the *result object* `CFraction.Sqrt2 * CFraction.Sqrt2` directly with `CFraction.FromCoeffs(new[]{2})` using `==` will return `false`.
    * But the expression: `(double)(Sqrt2 * Sqrt2) == 2.0` will return `true`, because any arithmetic operation is as precise as double.
    * A safety fuse (`MAX_CONSUME_WITHOUT_PRODUCE`) is implemented to prevent potential infinite loops in cases of extremely slow convergence or potential issues, terminating the generation with a rational approximation. This is a heuristic and might truncate results in rare valid cases that converge very slowly.

## Usage Examples

Here are a few basic examples to get you started.

**1. Creating Continued Fractions:**

```csharp
using ContinuedFractions;
using System.Numerics;

// From a rational number (355 / 113)
var piApprox = CFraction.FromRational(355, 113);
// Output: [3; 7, 16]
Console.WriteLine($"Pi Approximation: {piApprox}\n");

// From a list of coefficients (Golden Ratio)
var phi = CFraction.FromCoeffs(new BigInteger[] { 1, 1, 1, 1, 1, 1, 1, 1 }); // Finite approximation
// Or using the infinite generator pattern for the true Golden Ratio
var phiInfinite = CFraction.FromGenerator(GeneratePhi());
// Output: [1; 1, 1, 1, 1, 1, 1, 1, ...] (limited by numberOfCoeffs for display)
Console.WriteLine($"Golden Ratio: {phiInfinite}\n");

// Generator for Phi
IEnumerable<BigInteger> GeneratePhi() {
    yield return 1;
    while (true) {
        yield return 1;
    }
}

// Using predefined constants
var sqrt2 = CFraction.Sqrt2;
// Output: [1; 2, 2, 2, 2, ...]
Console.WriteLine($"Square Root of 2: {sqrt2}");
// Get coefficients using indexer (lazy evaluation triggers if needed)
Console.WriteLine($"Third coefficient of Sqrt(2): {CFraction.Sqrt2[2]}\n"); // Output: 2


// Add two fractions
var resultAdd = CFraction.One + piApprox;
// Output: [4; 7, 16] (Result of 1 + [3; 7, 16])
Console.WriteLine($"1 + Pi Approx: {resultAdd}\n");

// Multiply Sqrt2 by itself
// Note: The result sequence converges to 2, but might not be exactly [2]
var sqrt2Squared = CFraction.Sqrt2 * CFraction.Sqrt2;
// Output might be something like: [1;1,138047935912779337726843716653857327224] depending on MAX_CONSUME_WITHOUT_PRODUCE constant
Console.WriteLine($"Sqrt(2) * Sqrt(2) = {sqrt2Squared}");

// Verify the numerical value using double conversion
Console.WriteLine($"(double)(Sqrt(2) * Sqrt(2)) = {(double)sqrt2Squared}\n"); // Should be 2.0

// Work with integers
int five = 5;
var resultDiv = five / CFraction.One; // 5 / [1]
// Output: [5]
Console.WriteLine($"5 / 1 = {resultDiv}\n");

// Get convergents
Console.WriteLine("Convergents of Pi Approximation:");
foreach (var convergent in piApprox.GenerateConvergents())
{
    Console.WriteLine($"  {convergent.numerator} / {convergent.denominator}");
}
// Output:
//   3 / 1
//   22 / 7
//   355 / 113

// Get approximate double value
double phiValue = (double)phiInfinite;
Console.WriteLine($"Phi as double: {phiValue}"); // Output: 1.618033988749895
```

For more examples covering various arithmetic operations, edge cases, and usage of different factory methods, please refer to the unit tests located in the `Tests` directory of the repository.

## References

Here are some resources related to the algorithms and concepts used in this library:

* **Gosper's Algorithm for Continued Fraction Arithmetic:**
  An explanation of the algorithm used for binary operations (`+`, `-`, `*`, `/`) between `CFraction` instances and many other things.
    * [Bill Gosper's Original Notes](https://perl.plover.com/classes/cftalk/INFO/gosper.html)

* **Clear Explanation of Continued Fraction Algorithms:**
  A straightforward explanation of algorithms for continued fraction arithmetic.
    * [Hsin Yao's Notes on Continued Fractions](https://hsinhaoyu.github.io/cont_frac/)

* **Haskell Implementation and Article:**
  A related implementation in Haskell. Author provides an nice article discussing continued fraction arithmetic, the challenges of naive implementations (like potential infinity loops), and possible approaches to address them.
    * [mjcollins10/ContinuedFractions](https://github.com/mjcollins10/ContinuedFractions)

## License

This project is licensed under the **MIT License**.