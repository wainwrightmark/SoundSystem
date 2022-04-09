namespace SoundSystem;


public enum Tone
{
    C = 0, 
    Db = 1,
    D = 2, 
    Eb = 3,
    E = 4, 
    F = 5, 
    Gb = 6, 
    G = 7, 
    Ab = 8, 
    A = 9,
    Bb = 10,
    B = 11
}

public static class ToneExtensions
{
    public static Tone Transpose(this Tone t, int semitones) => (Tone)(( (int)t + semitones) % 12);

    /// <summary>
    /// How many semitones do you need to transpose to the target
    /// </summary>
    public static int GetTransposeAmount(this Tone t, Tone target)
    {
        var diff = target - t;
        if (diff == 0) return 0;
        else if (diff < 0)
        {
            if (Math.Abs(diff + 12) < Math.Abs(diff)) return diff + 12;
            return diff;
        }
        else
        {
            if (Math.Abs(diff - 12) < diff) return diff - 12;
            return diff;
        }
    }


    public static string ToNameSharp(this Tone t)
    {
        return t switch
        {
            Tone.C => "C",
            Tone.Db => "C#",
            Tone.D => "D",
            Tone.Eb => "D#",
            Tone.E => "E",
            Tone.F => "F",
            Tone.Gb => "F#",
            Tone.G => "G",
            Tone.Ab => "G#",
            Tone.A => "A",
            Tone.Bb => "A#",
            Tone.B => "B",
            _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
        };
    }

    public static string ToNameFlat(this Tone t)
    {
        return t switch
        {
            Tone.C => "C",
            Tone.Db => "Db",
            Tone.D => "D",
            Tone.Eb => "Eb",
            Tone.E => "E",
            Tone.F => "F",
            Tone.Gb => "Gb",
            Tone.G => "G",
            Tone.Ab => "Ab",
            Tone.A => "A",
            Tone.Bb => "Bb",
            Tone.B => "B",
            _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
        };
    }
    public static string ToNameABC(this Tone t)
    {
        return t switch
        {
            Tone.C => "C",
            Tone.Db => "^C",
            Tone.D => "D",
            Tone.Eb => "^D",
            Tone.E => "E",
            Tone.F => "F",
            Tone.Gb => "^F",
            Tone.G => "G",
            Tone.Ab => "^G",
            Tone.A => "A",
            Tone.Bb => "^A",
            Tone.B => "B",
            _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
        };
    }

}

public record Pitch(int MidiValue)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return Tone.ToNameSharp() + Octave;
    }

    public static Pitch Create(string tone, int octave)
    {
        var toneValue = ToneNamesDict[tone];
        return new(((octave) * 12) + toneValue);
    }
    
    public static Pitch Create(Tone tone, int octave)
    {
        return new(((octave) * 12) + (int) tone);
    }

    public Pitch Transpose(int amount) => new (MidiValue + amount);

    public Tone Tone => (Tone)(MidiValue % 12);

    public int Octave => (MidiValue / 12) - 1;

    public string ABCName
    {
        get
        {
            var tone = Tone.ToNameABC();

            return Octave switch
            {
                4 => tone,
                5 => tone.ToLowerInvariant(),
                < 4 => tone + new string(',', 4 - Octave),
                > 5 => tone.ToLowerInvariant() + new string(',', Octave - 5)
            };
        }
    }

    private static readonly Dictionary<string, int> ToneNamesDict = new(StringComparer.OrdinalIgnoreCase)
    {

        { "Cb", 11 },
        { "C", 12 },
        { "B#", 12 },
        { "Db", 13 },
        { "C#", 13 },
        { "D", 14 },
        { "Eb", 15 },
        { "D#", 15 },
        { "Fb", 16 },
        { "E", 16 },
        { "F", 17 },
        { "E#", 17 },
        { "Gb", 18 },
        { "F#", 18 },
        { "G", 19 },
        { "Ab", 20 },
        { "G#", 20 },
        { "A", 21 },
        { "A#", 22 },
        { "Bb", 22 },
        { "B", 23 },
    };
}