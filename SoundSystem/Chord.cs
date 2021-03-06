namespace SoundSystem;


public sealed record Chord(string Name, string Symbol, Tone Root, int[] Intervals)
{
    public int ToneCount => Intervals.Length;

    private static Chord Create(string name, string symbol, params int[] intervals) =>
        new (name, symbol, Tone.C, intervals);

    public static Chord Major { get; } = Create(nameof(Major), "M", 0, 4, 7);
    public static Chord Minor { get; } = Create(nameof(Minor), "m", 0, 3, 7);
    public static Chord Diminished { get; } = Create(nameof(Diminished), "dim", 0,3,6);
    public static Chord Augmented { get; } = Create(nameof(Augmented), "+", 0,4,8);
    public static Chord Sus4 { get; } = Create(nameof(Sus4), "sus4", 0,5,7);
    public static Chord Sus2 { get; } = Create(nameof(Sus2), "sus2", 0,2,7);


    public static Chord Major7 { get; } = Create(nameof(Major7), "maj7", 0,4,7,11);
    public static Chord MajorAdd2 { get; } = Create(nameof(MajorAdd2), "2", 0,2,4,7);
    public static Chord Dominant7 { get; } = Create(nameof(Dominant7), "7", 0,4,7,10);


    public static Chord HalfDiminished7 { get; } = Create(nameof(HalfDiminished7), "m7b5", 0,3,6,10);
    public static Chord Diminished7 { get; } = Create(nameof(Diminished7), "dim7", 0,3,6,9);

    public static Chord Minor7 { get; } = Create(nameof(Minor7), "m7", 0,3,7,10);
    public static Chord MinorMajor7 { get; } = Create(nameof(MinorMajor7), "mMaj7", 0,3,7,11);

    public static Chord Augmented7 { get; } = Create(nameof(Augmented7), "+7", 0,4,8,10);

    public static Chord Dominant11 { get; } = Create(nameof(Dominant11), "11", 0,5,7,10);

    public static Chord Dominant7Sharp9 { get; } = Create(nameof(Dominant7Sharp9), "7#9", 0,3,4,7,10);


    public IEnumerable<Pitch> GetPitches(int octave)
    {
        foreach (var interval in Intervals)
        {
            var t = Root + interval;

            if ((int)t >= 12)
                yield return Pitch.Create(t - 12, octave + 1);
            else
            {
                yield return Pitch.Create(t, octave);
            }
        }
    }
    
    public IEnumerable<Tone> Tones
    {
        get
        {
            foreach (var interval in Intervals)
            {
                var t = (interval + (int) Root) % 12;

                yield return (Tone) t;
            }
        }


        
    }

    /// <summary>
    /// Gets a tone at a particular index. Index outside the bounds will be up one octave.
    /// Accepts negative indices (these will be down one octave)
    /// </summary>
    public Pitch GetToneAtIndex(int index, int octave)
    {
        while (index >= Intervals.Length)
        {
            index -= Intervals.Length;
            octave++;
        }
        
        while (index < 0)
        {
            index += Intervals.Length;
            octave--;
        }
        var t = Root + Intervals[index];

        if ((int)t >= 12)
            return Pitch.Create(t - 12, octave + 1);
        else
        {
            return Pitch.Create(t, octave);
        }
    }

    public Chord TransposeToKey(Tone newRoot)
    {
        var diff = newRoot - Root;
        return Transpose(diff);
    }

    public Chord Transpose(int semitones)
    {
        if (semitones % 12 == 0) return this;
        return new(Name, Symbol, Root.Transpose(semitones),
            Intervals
        );
    }

    public string ABCName()
    {
        return Root.ToNameFlat() + Symbol;
    }
}



//public sealed record Chord(string Name, string Symbol,  Tone Root, Tone Fifth, Tone Third, Tone? Seventh = null, Tone? Ninth = null,
//    Tone? Thirteenth = null, Tone? Eleventh = null)
//{

//    public static Chord Major { get; } = new (nameof(Major),"M", Tone.C, Tone.G, Tone.E);
//    public static Chord Minor { get; } = new (nameof(Minor),"m", Tone.C, Tone.G, Tone.Eb);
//    public static Chord Diminished { get; } = new (nameof(Diminished),"dim", Tone.C, Tone.Gb, Tone.Eb);
//    public static Chord Augmented { get; } = new (nameof(Augmented),"+", Tone.C, Tone.Ab, Tone.E);
//    public static Chord Sus4 { get; } = new (nameof(Sus4), "sus4",Tone.C, Tone.G,   Tone.F);
//    public static Chord Sus2 { get; } = new ( nameof(Sus2),"sus2",Tone.C, Tone.G, Tone.D);


//    public static Chord Major7 { get; } = new ( nameof(Major7),"maj7",Tone.C, Tone.G, Tone.E, Tone.B);
//    public static Chord MajorAdd2 { get; } = new ( nameof(MajorAdd2),"2",Tone.C, Tone.G, Tone.E, null, Tone.D);
//    public static Chord Dominant7 { get; } = new (nameof(Dominant7),"7", Tone.C, Tone.G, Tone.E, Tone.Bb);
    

//    public static Chord HalfDiminished7 { get; } = new ( nameof(HalfDiminished7),"m7b5",Tone.C, Tone.Gb, Tone.Eb,Tone.Bb);
//    public static Chord Diminished7 { get; } = new ( nameof(Diminished7),"dim7",Tone.C, Tone.Gb, Tone.Eb, Tone.A);

//    public static Chord Minor7 { get; } = new(nameof(Minor7),"m7",Tone.C, Tone.G, Tone.Eb, Tone.Bb);
//    public static Chord MinorMajor7 { get; } = new(nameof(MinorMajor7),"mMaj7",Tone.C, Tone.G, Tone.Eb, Tone.B);
    
//    public static Chord Augmented7 { get; } = new ( nameof(Augmented7),"+7",Tone.C, Tone.Ab, Tone.E, Tone.Bb);

//    public static Chord Dominant11 { get; } = new ( nameof(Dominant11),"11",Tone.C, Tone.G, Tone.F, Tone.Bb);

//    public static Chord Dominant7Sharp9 { get; } = new (nameof(Dominant7Sharp9),"7#9", Tone.C, Tone.G, Tone.E, Tone.Bb, Tone.Eb);

//    public int ToneCount
//    {
//        get
//        {
//            var c = 3;
//            if(Seventh is not null) c++;
//            if(Ninth is not null) c++;
//            if(Eleventh is not null) c++;
//            if(Thirteenth is not null) c++;
//            return c;
//        }


//    }

//    public IEnumerable<Tone> Tones
//    {
//        get
//        {
//            yield return Root;
//            yield return Third;
//            yield return Fifth;
            
//            if(Seventh is not null) yield return Seventh.Value;
//            if(Ninth is not null) yield return Ninth.Value;
//            if(Eleventh is not null) yield return Eleventh.Value;
//            if(Thirteenth is not null) yield return Thirteenth.Value;
            
//        }
//    }

//    public Chord TransposeToKey(Tone newRoot)
//    {
//        var diff = newRoot - Root;
//        return Transpose(diff);
//    }

//    public Chord Transpose(int semitones)
//    {
//        if (semitones % 12 == 0) return this;
//        return new(Name, Symbol, Root.Transpose(semitones),
//            Fifth.Transpose(semitones),
//            Third.Transpose(semitones),
//            Seventh?.Transpose(semitones),
//            Ninth?.Transpose(semitones),
//            Thirteenth?.Transpose(semitones),
//            Eleventh?.Transpose(semitones)
//        );
//    }

//    public string ABCName()
//    {
//        return Root.ToNameFlat() + Symbol;
//    }
//}