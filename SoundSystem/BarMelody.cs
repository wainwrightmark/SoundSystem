using System.Text;
using Generator.Equals;

namespace SoundSystem;

/// <summary>
/// The melody of a complete bar of music
/// </summary>
[Equatable]
public partial record BarMelody([property:OrderedEquality]IReadOnlyList<Cluster> Clusters,[property:OrderedEquality] IReadOnlyList<NoteLength> Lengths)
{

    public IEnumerable<Term> Terms => Clusters.Zip(Lengths, (cluster, length) => new Term(cluster, length));

    public string ToABC()
    {
        StringBuilder sb = new();

        var tupletIndex = 0;

        foreach (var term in Terms)
        {
            if (tupletIndex == 0)
            {
                if (term.NoteLength.NinetySixths % 3 != 0)
                {
                    //Enter a triplet
                    tupletIndex++;
                    sb.Append("(3");
                    var nt = term with { NoteLength = new NoteLength(term.NoteLength.NinetySixths * 3) };
                    sb.Append(nt.ABCName());
                }
                else
                {
                    sb.Append(term.ABCName());
                }
            }
            else
            {
                var nt = term with { NoteLength = new NoteLength(term.NoteLength.NinetySixths * 3) };
                sb.Append(nt.ABCName());
                tupletIndex++;
                if (tupletIndex == 3) tupletIndex = 0;
            }

            if (tupletIndex == 0) sb.Append(' ');
        }

        return sb.ToString();
    }
}