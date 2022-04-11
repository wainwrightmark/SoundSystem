using CSharpFunctionalExtensions;
using Generator.Equals;

namespace SoundSystem;

public interface IGenerator<TInput, TOutput>
{
    public TOutput Next(TInput input, Maybe<TOutput> previous);


    public IEnumerable<TOutput> Generate(IEnumerable<TInput> inputs)
    {
        var previous = Maybe<TOutput>.None;

        foreach (var input in inputs)
        {
            var next = Next(input, previous);
            yield return next;
            previous = next;
        }
    }
}


public interface IStateGenerator : IGenerator<int, BarMetadata>{}

public interface IMelodyGenerator : IGenerator<BarMetadata, BarMelody>{}


public sealed record BarMetadata(KeyMode Key, Chord Chord, int BarNumber, int BarSeed);


public interface IClusterGenerator : IGenerator<(BarMetadata Metadata, NoteLength Baroffset, int Index), Cluster>{}

public interface IRhythmGenerator : IGenerator<BarMetadata, IReadOnlyList<NoteLength>> { }


public partial record ChordProgressionStateGenerator(KeyMode KeyMode, IReadOnlyList<int> RootIntervals) : IStateGenerator
{
    /// <inheritdoc />
    public BarMetadata Next(int input, Maybe<BarMetadata> previous)
    {
        var rootInterval = RootIntervals[input % RootIntervals.Count];
        var interval = KeyMode.Mode.Intervals[rootInterval - 1];
        var tone = KeyMode.Root.Transpose(interval);

        var chord = KeyMode.GetChordType(new HashSet<Tone>(){tone});

        return new BarMetadata(KeyMode, chord, input, new Random(input).Next());

    }
}


[Equatable]
public partial record FixedRhythmGenerator([property:OrderedEquality] IReadOnlyList<NoteLength> Durations) : IRhythmGenerator
{

    public static FixedRhythmGenerator CreateFrom(params int[] durs)
    {
        if (durs.Length == 0) return new(new[]{ new NoteLength(NoteLength.BarTotal)});

        var total = durs.Sum();

        var mult = NoteLength.BarTotal / total;

        return new FixedRhythmGenerator(durs.Select(x => new NoteLength(x * mult)).ToList());
    }

    /// <inheritdoc />
    public IReadOnlyList<NoteLength> Next(BarMetadata input, Maybe<IReadOnlyList<NoteLength>> previous)
    {
        return Durations;
    }
}

[Equatable]
public partial record ArpeggioClusterGenerator(int Octave, bool Ascending, int MaxHeight, int Width) : IClusterGenerator
{
    /// <inheritdoc />
    public Cluster Next((BarMetadata Metadata, NoteLength Baroffset, int Index) input, Maybe<Cluster> previous)
    {

        var i = input.Index % MaxHeight;
        if (!Ascending) i *= -1;
        i *= Width;

        var pitch = input.Metadata.Chord.GetToneAtIndex(i, Octave);
        return new Cluster(new[] { pitch });
    }
}

public partial record BlockClusterGenerator(int Octave, int Skip, int Take) : IClusterGenerator
{
    /// <inheritdoc />
    public Cluster Next((BarMetadata Metadata, NoteLength Baroffset, int Index) input, Maybe<Cluster> previous)
    {
        return new Cluster(GetChordPitches(Octave, input.Metadata.Chord).Skip(Skip).Take(Take).ToList());
    }

    public static IEnumerable<Pitch> GetChordPitches(int octave, Chord chord)
    {
        while (true)
        {
            foreach (var t in chord.GetPitches(octave))
            {
                yield return t;
            }

            octave++;
        }
    }
}



public record BrainMelodyGenerator(IRhythmGenerator RhythmGenerator, IClusterGenerator ClusterGenerator) : IMelodyGenerator
{
    /// <inheritdoc />
    public BarMelody Next(BarMetadata input, Maybe<BarMelody> previous)
    {
        var prevRhythm = previous.Map(x => x.Lengths);
        var rhythm = RhythmGenerator
            .Next(input, prevRhythm);

        var offset = 0;
        var index = 0;
        var termInputs = rhythm.Select(r =>
        {

            var result =  (input, new NoteLength(offset), index );
            offset += r.NinetySixths;
            index++;
            return result;
        }).ToList();

        var terms = ClusterGenerator.Generate(termInputs).ToList();
        return new BarMelody(terms, rhythm);
    }
}

public record LoopMelodyGenerator(IReadOnlyList<BarMelody> Bars, Maybe<Tone> OriginalKey,  int Offset = 0) : IMelodyGenerator
{ 
    /// <inheritdoc />
    public BarMelody Next(BarMetadata input, Maybe<BarMelody> previous)
    {
        var barNumber = (input.BarNumber + Offset)% Bars.Count;

        var barMelody =Bars[barNumber];

        //TODO allow changing of mode
        int transposeAmount;
        if (OriginalKey.HasNoValue) transposeAmount = 0;
        else
        {
            transposeAmount = OriginalKey.Value.GetTransposeAmount(input.Key.Root);
        }

        if (transposeAmount == 0) return barMelody;

        return barMelody with { Clusters = barMelody.Clusters.Select(x => x.Transpose(transposeAmount)).ToList() };
    }
}