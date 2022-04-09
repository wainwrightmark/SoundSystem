using System.Collections.Generic;
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

    [Fact]
    public void Test1()
    {
        var text = 
            "Chords C Major 1, 4, 5, 1;voice 'bass' 33 rhy 3, 3, 2 arp 3 asc 1 1";


         var r = Parser.Parse(text);

         if(!r.IsSuccess)
            r.IsSuccess.Should().BeTrue(r.Error);
    }
}