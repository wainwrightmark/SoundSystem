using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace PoemTone;

public class ConvertToJson
{

    public static NoteListBuilder SetNotes(NoteListBuilder noteListBuilder, string text)
    {
        var words = text.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var pitchClusters = words.Select(CreatePitches);

        var r = pitchClusters.Aggregate(noteListBuilder,
            (builder, pitches) => builder.AddNoteQuarters(1, 1, pitches.ToArray()));

        return r;
    }

    public static IEnumerable<Pitch> CreatePitches(string word)
    {
        var pitches = word.ToUpperInvariant().Where(char.IsLetter).Select(x => LetterPitchDict[x]).Distinct();
        return pitches;
    }

    private static readonly IReadOnlyDictionary<char, Pitch> LetterPitchDict = new Dictionary<char, Pitch>()
    {
        {'E', Pitch.Create("C", 3)},
        {'T', Pitch.Create("G", 3)},
        {'A', Pitch.Create("E", 3)},
        {'I', Pitch.Create("F", 3)},
        {'N', Pitch.Create("B", 3)},
        {'O', Pitch.Create("D", 3)},
        {'S', Pitch.Create("A", 3)},
        {'H', Pitch.Create("C", 4)},
        {'R', Pitch.Create("G", 4)},
        {'D', Pitch.Create("E", 4)},
        {'L', Pitch.Create("F", 4)},
        {'U', Pitch.Create("B", 4)},
        {'C', Pitch.Create("D", 4)},
        {'M', Pitch.Create("A", 4)},
        {'F', Pitch.Create("C", 4)},
        {'W', Pitch.Create("G", 4)},
        {'Y', Pitch.Create("E", 4)},
        {'G', Pitch.Create("F", 4)},
        {'P', Pitch.Create("Bb", 4)},
        {'B', Pitch.Create("Bb", 4)},
        {'V', Pitch.Create("Bb", 4)},
        {'K', Pitch.Create("Bb", 4)},//Dom VII
        {'Q', Pitch.Create("Gb", 4)},
        {'J', Pitch.Create("Ab", 4)},
        {'X', Pitch.Create("Eb", 4)},
        {'Z', Pitch.Create("Db", 4)},

    };


    public static JsonFiles.ToneMidiDocument MakeJson(Func<NoteListBuilder, NoteListBuilder> setNotes, int ticksPerSecond, int bpm)
    {
        var header = new JsonFiles.Header(ArraySegment<object>.Empty, ArraySegment<object>.Empty,
            "My File", Ppq: (ticksPerSecond * 60) / bpm,
            Tempos: new[] { new JsonFiles.Tempos(bpm, 0) },
            TimeSignatures: new[] { new JsonFiles.TimeSignature(0, new[] { 4, 4 }, 0) }
        );


        var notes = setNotes(new NoteListBuilder(0, ticksPerSecond, bpm)).Build();

        var tracks = new List<JsonFiles.Track>()
        {
            new JsonFiles.Track(0,
                ImmutableDictionary<int, JsonFiles.ControlChange>.Empty, ArraySegment<object>.Empty, 
                new JsonFiles.Instrument("piano", 0, "acoustic grand piano"),
                "Piano",
                notes, 
                EndOfTrackTicks:notes.Max(x=>x.Ticks + x.DurationTicks)
            )
        };

        return new JsonFiles.ToneMidiDocument(header, tracks);
    }

}

public  class NoteListBuilder
{
    public NoteListBuilder(int currentTick, int ticksPerSecond, int bpm)
    {
        _currentTick = currentTick;
        _ticksPerSecond = ticksPerSecond;
        _bpm = bpm;
    }

    public NoteListBuilder AddRest(int ninetySixthNotes) => AddNoteNinetySixths(ninetySixthNotes, 0);

    public NoteListBuilder AddNoteQuarters(int quarterNotes, double vel,params Pitch[] pitches) =>
        AddNoteNinetySixths(quarterNotes * 24, vel, pitches);
    
    public NoteListBuilder AddNoteEighths(int eighthNotes, double vel,params Pitch[] pitches) =>
        AddNoteNinetySixths(eighthNotes * 12, vel, pitches);

    public NoteListBuilder AddNoteTwelfths(int twelfthNotes, double vel,params Pitch[] pitches) =>
        AddNoteNinetySixths(twelfthNotes * 8, vel, pitches);
    
    public NoteListBuilder AddNoteNinetySixths(int ninetySixthNotes, double vel,params Pitch[] pitches)
    {
        var durTicks = (ninetySixthNotes * _ticksPerSecond * 60) / (_bpm * 24); //24 = 96 notes in a bar / 4 beats per bar

        foreach (var pitch in pitches)
        {
            _notes.Add(JsonFiles.Note.Create(pitch, _currentTick, durTicks, _ticksPerSecond, vel));    
        }

        _currentTick+= durTicks;

        return this;
    }


    private readonly int _ticksPerSecond;

    private readonly int _bpm;

    private int _currentTick;

    private readonly List<JsonFiles.Note> _notes = new();

    

    public IReadOnlyList<JsonFiles.Note> Build() => _notes;
}


public class JsonFiles
{

    public record ToneMidiDocument(
        [property: JsonPropertyName("header")]Header Header,
        [property: JsonPropertyName("tracks")]IReadOnlyList<Track> Tracks);

        public record Tempos(
        [property: JsonPropertyName("bpm")] int Bpm,
        [property: JsonPropertyName("ticks")] int Ticks
    );

    public record TimeSignature(
        [property: JsonPropertyName("ticks")] int Ticks,
        [property: JsonPropertyName("timeSignature")] IReadOnlyList<int> TimeSignatureNumbers,
        [property: JsonPropertyName("measures")] int Measures
    );

    public record Header(
        [property: JsonPropertyName("keySignatures")] IReadOnlyList<object> KeySignatures,
        [property: JsonPropertyName("meta")] IReadOnlyList<object> Meta,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("ppq")] int Ppq,
        [property: JsonPropertyName("tempos")] IReadOnlyList<Tempos> Tempos,
        [property: JsonPropertyName("timeSignatures")] IReadOnlyList<TimeSignature> TimeSignatures
    );

    public record ControlChange(
        [property: JsonPropertyName("number")] int Number,
        [property: JsonPropertyName("ticks")] int Ticks,
        [property: JsonPropertyName("time")] int Time,
        [property: JsonPropertyName("value")] double Value
    );

    public record Instrument(
        [property: JsonPropertyName("family")] string Family,
        [property: JsonPropertyName("number")] int Number,
        [property: JsonPropertyName("name")] string Name
    );

    public record Note(
        [property: JsonPropertyName("duration")]
        double Duration,
        [property: JsonPropertyName("durationTicks")]
        int DurationTicks,
        [property: JsonPropertyName("midi")] int Midi,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("ticks")] int Ticks,
        [property: JsonPropertyName("time")] double Time,
        [property: JsonPropertyName("velocity")]
        double Velocity
    )
    {
        public static Note Create(Pitch pitch, int startTicks, int durationTicks, int ticksPerSecond, double vel = 1)
        {
            var duration = ((double) durationTicks) / ticksPerSecond;
            var startTime = ((double)startTicks / ticksPerSecond);

            return new Note(duration, durationTicks, pitch.MidiValue, pitch.Tone + pitch.Octave, startTicks, startTime,
                vel);
        }
    }

    public record Track(
        [property: JsonPropertyName("channel")] int Channel,
        [property: JsonPropertyName("controlChanges")] IReadOnlyDictionary<int, ControlChange> ControlChanges ,
        [property: JsonPropertyName("pitchBends")] IReadOnlyList<object> PitchBends,
        [property: JsonPropertyName("instrument")] Instrument Instrument,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("notes")] IReadOnlyList<Note> Notes,
        [property: JsonPropertyName("endOfTrackTicks")] int EndOfTrackTicks
    );

    public record Root(
        [property: JsonPropertyName("header")] Header Header,
        [property: JsonPropertyName("tracks")] IReadOnlyList<Track> Tracks
    );



}