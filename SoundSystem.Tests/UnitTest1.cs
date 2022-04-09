//using System.Collections.Generic;
//using Xunit;
//using Xunit.Abstractions;

//namespace SoundSystem.Tests;
//public class UnitTest1
//{
//    private readonly ITestOutputHelper _testOutputHelper;

//    public UnitTest1(ITestOutputHelper testOutputHelper)
//    {
//        _testOutputHelper = testOutputHelper;
//    }

//    [Theory]
//    [InlineData("Alice", 2)]
//    public void Test1(string w, int expectedCount)
//    {
//        //var r = WordConverter.ConvertWord(w, new PronunciationEngine());

//        //foreach (var chord in r)
//        //{
//        //    _testOutputHelper.WriteLine(chord.ToString());
//        //}

//        //r.Length.Should().Be(expectedCount);
//    }
//    //https://editor.drawthedots.com/

//    [Theory]
//    [InlineData("Alice Began to eat sandwiches")]
//    [InlineData("to do once or twice")]
//    [InlineData("Alice was beginning")]
//    [InlineData("Sandwiches")]
//    [InlineData("Sandwiches Sandwiches Sandwiches")]
//    [InlineData(DefaultText)]
//    public void Test2(string text)
//    {

//        var r = ConvertToAbc.Convert(text, 100, new KeyMode(Tone.C, Mode.Major.Instance), new PronunciationEngine(), Voices);

//        _testOutputHelper.WriteLine(r);
//    }


//    private static readonly List<Voice> Voices = new()
//    {
//        BasicTenor.Instance,
//        PadVoice.Instance,
//        BasicBassVoice.Instance
//    };

//    private const string DefaultText = @"Alice was beginning to get very tired of sitting by her sister on the
//bank, and of having nothing to do. Once or twice she had peeped into the
//book her sister was reading, but it had no pictures or conversations in
//it, ""and what is the use of a book,"" thought Alice, ""without pictures or
//conversations?""";
//}