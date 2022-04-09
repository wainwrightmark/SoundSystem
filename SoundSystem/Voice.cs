namespace SoundSystem;

public sealed record Voice(string Id, string Name, string ShortName, string Clef, int GroupNumber, int GeneralMidiVoice)
{
    /// <summary>
    /// Voices will be grouped by their voice number
    /// </summary>
    public int GroupNumber { get;init; } = GroupNumber;


    public string ABCHeader => $"V:{Id} clef={Clef} name=\"{Name}\" snm=\"{ShortName}\"";
}


public static class Voices
{
    public static Voice Bass { get; } = new ("Bs", "Bass", "Bs", "bass-8", 3,33);
    public static Voice Piano { get; } = new ("Pf", "Piano", "Pf", "treble", 2,5);
    public static Voice Tenor { get; } = new ("T1", "Tenor", "T1", "treble", 2,55);
    public static Voice Pad { get; } = new ("Pd", "Pad", "Pd", "treble", 2,91);

    //TODO other voices
}

//public class BasicBassVoice : Voice
//{
//    private BasicBassVoice()
//    {
//    }

//    public static Voice Instance { get; } = new BasicBassVoice();

//    /// <inheritdoc />
//    public override string Id => "Bs";

//    /// <inheritdoc />
//    public override string Name => "Bass";

//    /// <inheritdoc />
//    public override string ShortName => "Bs";

//    /// <inheritdoc />
//    public override string Clef => "bass-8";

//    /// <inheritdoc />
//    public override int GroupNumber => 3;


//    /// <inheritdoc />
//    public override IEnumerable<ChordTuplet> GetTuplets(SyllablePhrase syllablePhrase, KeyMode keyMode)
//    {
//        const int beatLength = 24;
//        //Play the root note on multiples of 24
//        var current = syllablePhrase.BarOffSet.NinetySixths;
//        var end = syllablePhrase.BarOffSet.NinetySixths + syllablePhrase.TotalLength;

//        var chord = keyMode.GetChordType(syllablePhrase.Tones.Value);



//        //var pitches = new[] { phrase.Pitches.MinBy(x => x.MidiValue)!.Transpose(-12) };

//        var pitchIndex = 0;

//        Tone nextTone()
//        {
//            var tone = (pitchIndex % 3) switch
//            {
//                0 => chord.Root,
//                1 => chord.Fifth,
//                2 => chord.Third,
//                _ => throw new Exception("Next Tone Error")
//            };

//            pitchIndex++;
//            return tone;
//        }

//        if (current % beatLength != 0)
//        {
//            var diff = beatLength - (current % beatLength);
//            yield return new Chord(new []{Pitch.Create(nextTone(), 4), }, new NoteLength(diff)).AsTuplet();
//            current += diff;
//        }

//        while (current < end)
//        {
//            var l = Math.Min(beatLength, end - current);
//            yield return new Chord(new []{Pitch.Create(nextTone(), 4)},
//                new NoteLength(l)).AsTuplet();
//            current += l;
//        }
//    }

//    /// <inheritdoc />
//    public override bool UseLyrics => false;

//    /// <inheritdoc />
//    public override int GMMidiVoice => 33;
//}

//public class JazzPianoRhythmVoice : Voice
//{
//    private JazzPianoRhythmVoice()
//    {
//    }

//    public static Voice Instance { get; } = new JazzPianoRhythmVoice();

//    /// <inheritdoc />
//    public override string Id => "JP";

//    /// <inheritdoc />
//    public override string Name => "Jazz Piano";

//    /// <inheritdoc />
//    public override string ShortName => "JP";

//    /// <inheritdoc />
//    public override string Clef => "treble";

//    /// <inheritdoc />
//    public override int GroupNumber => 2;


//    /// <inheritdoc />
//    public override IEnumerable<ChordTuplet> GetTuplets(SyllablePhrase syllablePhrase, KeyMode keyMode)
//    {
//        const int beatLength = 24;
//        //Play the root note on multiples of 24
//        var current = syllablePhrase.BarOffSet.NinetySixths;
//        var end = syllablePhrase.BarOffSet.NinetySixths + syllablePhrase.TotalLength;

//        var chord = keyMode.GetChordType(syllablePhrase.Tones.Value);


//        while (current < end)
//        {
//            if (current % beatLength != 12) //rest
//            {
//                var diff = beatLength - ((current + 12) % beatLength);
//                yield return new Chord(new Pitch[]{}, new NoteLength(diff)).AsTuplet();
//                current += diff;
//            }
//            else
//            {
//                var l = Math.Min(12, end - current);
//                yield return new Chord(chord.Tones.TakeLast(2).Select(x=> Pitch.Create(x, 4)).ToArray(),
//                    new NoteLength(l)).AsTuplet();
//                current += l;
//            }


//        }
//    }

//    /// <inheritdoc />
//    public override bool UseLyrics => false;

//    /// <inheritdoc />
//    public override int GMMidiVoice => 5;
//}

//public class BasicTenor : Voice
//{
//    private BasicTenor()
//    {
//    }

//    public static Voice Instance { get; } = new BasicTenor();

//    /// <inheritdoc />
//    public override string Id => "T1";

//    /// <inheritdoc />
//    public override string Name => "Tenor";

//    /// <inheritdoc />
//    public override string ShortName => "T1";

//    /// <inheritdoc />
//    public override string Clef => "treble";

//    /// <inheritdoc />
//    public override int GroupNumber => 1;

//    /// <inheritdoc />
//    public override IEnumerable<ChordTuplet> GetTuplets(SyllablePhrase syllablePhrase, KeyMode keyMode)
//    {
//        var pitchIndex = 0;

//        foreach (var tuplet in syllablePhrase.Syllables)
//        {
//            var chords = new List<Chord>(tuplet.Length());

//            foreach (var syllable in tuplet.SyllableLengths)
//            {
//                if (syllable.IsRest())
//                    chords.Add(new Chord(ArraySegment<Pitch>.Empty, syllable.NoteLength));
//                else
//                {
//                    var pitch = syllablePhrase.Pitches[pitchIndex % syllablePhrase.Pitches.Count];
//                    if (pitch.Octave == 4) pitch = pitch.Transpose(12);

//                    chords.Add(new Chord(new[] { pitch }, syllable.NoteLength));
//                    pitchIndex++;
//                }
//            }


//            yield return new ChordTuplet(chords);
//        }
//    }


//    /// <inheritdoc />
//    public override bool UseLyrics => true;

//    /// <inheritdoc />
//    public override int GMMidiVoice => 55;
//}



//public class PadVoice : Voice
//{
//    private PadVoice()
//    {
//    }

//    public static Voice Instance { get; } = new PadVoice();

//    /// <inheritdoc />
//    public override string Id => "P";

//    /// <inheritdoc />
//    public override string Name => "Pad";

//    /// <inheritdoc />
//    public override string ShortName => "P";

//    /// <inheritdoc />
//    public override string Clef => "treble";

//    /// <inheritdoc />
//    public override int GroupNumber => 2;

//    /// <inheritdoc />
//    public override IEnumerable<ChordTuplet> GetTuplets(SyllablePhrase syllablePhrase, KeyMode keyMode)
//    {
//        yield return new Chord(syllablePhrase.Pitches, new NoteLength(syllablePhrase.TotalLength)).AsTuplet();
//    }


//    /// <inheritdoc />
//    public override bool UseLyrics => false;

//    /// <inheritdoc />
//    public override int GMMidiVoice => 91;
//}