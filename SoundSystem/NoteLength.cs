namespace SoundSystem;

public readonly record struct NoteLength(int NinetySixths)
{
    public const int BarTotal = 96;

    public string ToABCString()
    {
        return GetFractionText(NinetySixths, BarTotal / 4);
    }

    //TODO rewrite. Only allow multiples of two
    private static string GetFractionText(int numerator, int denominator)
    {
        if (numerator == denominator) return "";

        var gcd = GCD(numerator, denominator);

        var newNumerator = numerator / gcd;
        var newDenominator = denominator / gcd;

        if (newDenominator == 1) return newNumerator.ToString();
        if (newNumerator == 1) return $"/{newDenominator}";

        return $"{newNumerator}/{newDenominator}";
    }

    private static int GCD(int a, int b)
    {
        while (b > 0)
        {
            var rem = a % b;
            a = b;
            b = rem;
        }
        return a;
    }
}