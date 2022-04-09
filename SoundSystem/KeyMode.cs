using System.Collections.Concurrent;
using Generator.Equals;

namespace SoundSystem;

public sealed record KeyMode(Tone Root, Mode Mode)
{
    private readonly ConcurrentDictionary<IReadOnlySet<Tone>, Chord> _chordTypes =
        new(new SetEqualityComparer<Tone>());

    public Chord GetChordType(IReadOnlySet<Tone> tones)
    {
        return _chordTypes.GetOrAdd(tones, CreateChordType);
    }

    private Chord CreateChordType(IReadOnlySet<Tone> tones)
    {
        var chordType = Mode.GetChordTypes(Root)
                .OrderByDescending(x => x.Tones.Count(tones.Contains)) //most matching notes
                .ThenByDescending(x => tones.Contains(x.Root)) 
                .ThenByDescending(x => tones.Contains(x.Fifth))
                .ThenByDescending(x => tones.Contains(x.Third))
                .ThenBy(x=>x.Tones.Count()) //Smaller chords first
            ;

        return chordType.First();
    }
}