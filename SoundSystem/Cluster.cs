using Generator.Equals;

namespace SoundSystem;

[Equatable]
public sealed partial record Cluster([property: OrderedEquality] IReadOnlyList<Pitch> Pitches)
{
    public string ABCName()
    {
        if (Pitches.Count == 0)
            return "z";
        if (Pitches.Count == 1)
            return Pitches.Single().ABCName;

        return $"[{string.Join("", Pitches.Select(x => x.ABCName))}]";
    }
    public Cluster Transpose(int amount)
    {
        if (amount == 0) return this;

        var newPitches = Pitches.Select(x => x.Transpose(amount)).ToList();

        return this with { Pitches = newPitches };
    }
}