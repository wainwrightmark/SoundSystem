using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;
using Result = CSharpFunctionalExtensions.Result;

namespace SoundSystem;

public static class Parser
{
    public enum SoundSystemToken
    {
        None,
        Tone,
        Comma,
        Identifier,
        Word,
        Integer,
        Semicolon,
        ArrayOpen,
        ArrayClose,


    }


    public static CSharpFunctionalExtensions. Result<MusicSystem> Parse(string text)
    {
        var t = Tokenizer.TryTokenize(text);
        if(!t.HasValue)return  Result.Failure<MusicSystem>(t.FormatErrorMessageFragment());


        var p  = MusicSystemParser.TryParse(t.Value);

        if(!p.HasValue)return  Result.Failure<MusicSystem>(p.FormatErrorMessageFragment());

        return p.Value;
    }

    private static readonly Tokenizer<SoundSystemToken> Tokenizer = new TokenizerBuilder<SoundSystemToken>()
        .Match(Character.EqualTo(';'), SoundSystemToken.Semicolon)
        .Ignore(Span.WhiteSpace)
        .Ignore(Comment.ShellStyle)
        .Match(Numerics.Integer, SoundSystemToken.Integer, true)
        .Match(Character.EqualTo(','), SoundSystemToken.Comma, false)
        .Match(Character.EqualTo('['), SoundSystemToken.ArrayOpen, false)
        .Match(Character.EqualTo(']'), SoundSystemToken.ArrayClose, false)
        .Match(Span.Regex("[A-G][b]?"), SoundSystemToken.Tone, true)
        .Match(Identifier.CStyle, SoundSystemToken.Identifier, true)
        .Match(QuotedString.SqlStyle, SoundSystemToken.Word, false).Build();

    public static readonly TokenListParser<SoundSystemToken, List<int>> IntList =
            from open in Token.EqualTo(SoundSystemToken.ArrayOpen)
            from l in Token.EqualTo(SoundSystemToken.Integer)
                .Many()
            from close in Token.EqualTo(SoundSystemToken.ArrayClose)
            select l.Select(t => t.ParseAsInt()).ToList()
        ;

    public static readonly TokenListParser<SoundSystemToken, IClusterGenerator> ArpeggioClusterGenerator =
    (
        from _ in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Arp")
        from oct in Token.EqualTo(SoundSystemToken.Integer)
        from asc in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Asc")
            .Or(Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Dsc"))
        from height in Token.EqualTo(SoundSystemToken.Integer)
        from width in Token.EqualTo(SoundSystemToken.Integer)

        select new ArpeggioClusterGenerator(
            oct.ParseAsInt(), 
            asc.ToStringValue().Equals("ASC", StringComparison.OrdinalIgnoreCase),
            height.ParseAsInt(),
            width.ParseAsInt()
        ) as IClusterGenerator
    );
    
    public static readonly TokenListParser<SoundSystemToken, IClusterGenerator> BlockClusterGenerator =
    (
        from _ in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Block")
        from oct in Token.EqualTo(SoundSystemToken.Integer)
        
        from skip in Token.EqualTo(SoundSystemToken.Integer)
        from take in Token.EqualTo(SoundSystemToken.Integer)

        select new BlockClusterGenerator(oct.ParseAsInt(), skip.ParseAsInt(), take.ParseAsInt())  as IClusterGenerator
    );
    
    public static readonly TokenListParser<SoundSystemToken, IRhythmGenerator> RhythmGenerator =
    (
        from _ in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Rhy")
        from ints in IntList

        select FixedRhythmGenerator.CreateFrom(ints.ToArray()) as IRhythmGenerator
    );

    public static readonly TokenListParser<SoundSystemToken, Voice> Voice =
    (
        from _ in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Voice")
        from name in Token.EqualTo(SoundSystemToken.Word)
        from transpose in Token.EqualTo(SoundSystemToken.Integer)
        from gmv in Token.EqualTo(SoundSystemToken.Integer)
        
        select new Voice(name.WordToString(), transpose.ParseAsInt(),  gmv.ParseAsInt())
    );

    public static readonly TokenListParser<SoundSystemToken, (Voice, IMelodyGenerator)> MelodyGenerator =
        (from v in Voice
            from r in RhythmGenerator
            from c in BlockClusterGenerator.Or(ArpeggioClusterGenerator)
            select (v, new BrainMelodyGenerator(r, c) as IMelodyGenerator)
        );

    public static readonly TokenListParser<SoundSystemToken, ChordProgressionStateGenerator> StateGenerator =

        (from _id in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Chords")
            from note in Token.EqualTo(SoundSystemToken.Tone)
            from mode in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Major")
                .Or(Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Minor"))
            from intervals in IntList
            //from _nl in Token.EqualTo(SoundSystemToken.Semicolon) 

            select new ChordProgressionStateGenerator(
                new KeyMode((Tone)Enum.Parse(typeof(Tone), note.ToStringValue(), true),
                    mode.ToStringValue().Equals("Major", StringComparison.OrdinalIgnoreCase)?
                        Mode.Major.Instance : Mode.Minor.Instance),
                intervals
            )
        );

    public static readonly TokenListParser<SoundSystemToken, MusicSystem> MusicSystemParser =
    (
        from titleName in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Title")
        from title in Token.EqualTo(SoundSystemToken.Word)
        
        from barsName in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "Bars")
        from bars in Token.EqualTo(SoundSystemToken.Integer)

        from bpmName in Token.EqualToValueIgnoreCase(SoundSystemToken.Identifier, "BPM")
        from bpm in Token.EqualTo(SoundSystemToken.Integer)
        
        from s in StateGenerator
        from instruments in MelodyGenerator.Many()// .ManyDelimitedBy(Token.EqualTo(SoundSystemToken.Semicolon))

        //from finalSemicolon in Token.EqualTo(SoundSystemToken.Semicolon).Optional()

        select new MusicSystem(title.WordToString(), bars.ParseAsInt(), bpm.ParseAsInt(), s, instruments)
    );

    public static int ParseAsInt(this Token<SoundSystemToken> t)
    {
        return int.Parse(t.ToStringValue());
    }
    
    public static string WordToString(this Token<SoundSystemToken> t)
    {
        return t.ToStringValue().Trim('\'');
    }
}