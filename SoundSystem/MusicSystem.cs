using System.Text;

namespace SoundSystem;

public sealed record MusicSystem(
    string Title,
    int NumberOfBars,
    int BPM,

    IStateGenerator StateGenerator,
    IReadOnlyList<(Voice Voice, IMelodyGenerator MelodyGenerator)> Instruments)
{

    public string ToABC()
    {
        var stringBuilder = new StringBuilder();

        var voiceGroups = string.Join(' ',
            Instruments.Select(x=>x.Voice).GroupBy(x => x.Name)
                .Select(g => $"({string.Join(' ', g.Select(x => x.Name))})")
        );

        stringBuilder.AppendLine("X:1");
        stringBuilder.AppendLine($"Q: 1/4={BPM}");
        stringBuilder.AppendLine($"T: {Title}");
        stringBuilder.AppendLine($"M: 4/4");
        stringBuilder.AppendLine("L:1/4");
        stringBuilder.AppendLine($"%% score {voiceGroups}");
        foreach (var voice in Instruments.Select(x=>x.Voice))
        {
            stringBuilder.AppendLine(voice.ABCHeader);
            stringBuilder.AppendLine($"[I: MIDI=program {voice.GeneralMidiVoice - 1}]");
        }
        

        stringBuilder.AppendLine("%");
        const int barsPerChunk = 4;

        var states = StateGenerator.Generate(Enumerable.Range(0, NumberOfBars)).ToList();

        var instruments = Instruments.Select(instrument =>
            (instrument.Voice, bars: instrument.MelodyGenerator.Generate(states).ToArray())).ToList();

        for (var barNumber = 0; barNumber < NumberOfBars; barNumber+=barsPerChunk)
        {
            stringBuilder.AppendLine($"% {barNumber}");

            var first = true;
            foreach (var (voice, bars ) in instruments)
            {
                stringBuilder.Append($"[V:{voice.Name}]");
                var chunk = bars.AsSpan().Slice(barNumber, barsPerChunk);
                for (var barNum = 0; barNum < chunk.Length; barNum++)
                {
                    var bar = chunk[barNum];
                    if (first)
                    {
                        var barMetadata = states[barNumber + barNum];
                        stringBuilder.Append($"\"{barMetadata.Chord.ABCName()}\"");
                    }
                        

                    stringBuilder.AppendJoin(" ",
                        bar.ToABC()
                    );
                    stringBuilder.Append('|');
                }

                stringBuilder.AppendLine();
                first = false;
            }
        }
        return stringBuilder.ToString();
    }

}