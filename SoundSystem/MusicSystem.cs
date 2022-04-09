using System.Text;

namespace SoundSystem;

public sealed record MusicSystem(
    IStateGenerator StateGenerator,
    IReadOnlyList<(Voice Voice, IMelodyGenerator MelodyGenerator)> Instruments)
{

    public string ToABC(string title, int numberOfBars, int bpm)
    {
        var stringBuilder = new StringBuilder();

        var voiceGroups = string.Join(' ',
            Instruments.Select(x=>x.Voice).GroupBy(x => x.GroupNumber).OrderBy(x => x.Key)
                .Select(g => $"({string.Join(' ', g.Select(x => x.Id))})")
        );

        stringBuilder.AppendLine("X:1");
        stringBuilder.AppendLine($"Q: 1/4={bpm}");
        stringBuilder.AppendLine($"T: {title}");
        stringBuilder.AppendLine($"M: 4/4");
        stringBuilder.AppendLine("L:1/4");
        stringBuilder.AppendLine($"%% score {voiceGroups}");
        foreach (var voice in Instruments.Select(x=>x.Voice).OrderBy(x => x.GroupNumber))
        {
            stringBuilder.AppendLine(voice.ABCHeader);
            stringBuilder.AppendLine($"[I: MIDI=program {voice.GeneralMidiVoice - 1}]");
        }
        

        stringBuilder.AppendLine("%");
        const int barsPerChunk = 4;

        var states = StateGenerator.Generate(Enumerable.Range(0, numberOfBars)).ToList();

        var instrumentBars = Instruments.Select(instrument =>
            (instrument.Voice, bars: instrument.MelodyGenerator.Generate(states).ToArray())).ToList();

        for (var barNumber = 0; barNumber < numberOfBars; barNumber+=barsPerChunk)
        {
            stringBuilder.AppendLine($"% {barNumber}");

            foreach (var (voice, bars ) in instrumentBars)
            {
                stringBuilder.Append($"[V:{voice.Id}]");
                var chunk = bars.AsSpan().Slice(barNumber, barsPerChunk);
                foreach (var bar in chunk)
                {
                    stringBuilder.AppendJoin(" ",
                        bar.ToABC()
                    );
                    stringBuilder.Append('|');
                }

                stringBuilder.AppendLine();
            }
        }
        return stringBuilder.ToString();
    }

}