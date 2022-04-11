using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace SoundSystem.Tests;
public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public const string Text =
        @"Title 'Song' Bars 12 BPM 120 Chords C Major [1 4 5 1]
voice 'bass' -2 33 rhy [3 3 2] arp 3 asc 3 1
voice 'piano' 0 5 rhy [3 1] block 5 1 2";

    [Fact]
    public void TestParser()
    {
        var r = Parser.Parse(Text);

         if(!r.IsSuccess)
            r.IsSuccess.Should().BeTrue(r.Error);
    }
    
}