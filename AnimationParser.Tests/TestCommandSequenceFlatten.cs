using AnimationParser.Core;

namespace AnimationParser.Tests;

public class TestCommandSequenceFlatten
{
    [Test]
    public void TestFlattenNoLoop()
    {
        const string input = "(define drawable ( (line (0 0) (50 50)) ) )";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer.Tokenize());
        var flattened = parser.Parse().Flatten();

        Assert.That(flattened.Count(), Is.EqualTo(1));
    }

    [Test]
    public void TestFlattenSingleDepthLoop()
    {
        const string input = """
                             (define drawable ( (line (0 0) (50 50)) ) )
                             (loop 100 ( (shift drawable right) ) )
                             """;

        var lexer = new Lexer(input);
        var parser = new Parser(lexer.Tokenize());
        var flattened = parser.Parse().Flatten();

        Assert.That(flattened.Count(), Is.EqualTo(100 + 1));
    }

    [Test]
    public void TestFlattenMultipleDepthLoop()
    {
        const string input = """
                             (define drawable ( (line (0 0) (50 50)) ) )
                             (loop 100 ( (loop 100 ( (shift drawable up) ) ) ) )
                             """;

        var lexer = new Lexer(input);
        var parser = new Parser(lexer.Tokenize());
        var flattened = parser.Parse().Flatten();

        Assert.That(flattened.Count(), Is.EqualTo(10000 + 1));
    }

    [Test]
    public void TestFlatten_LoopWithInvalidCountSkipped()
    {
        const string input = """
                             (loop 0 ( (loop 100 ( (shift drawable up) ) ) ) )
                             """;

        var lexer = new Lexer(input);
        var parser = new Parser(lexer.Tokenize());
        var flattened = parser.Parse().Flatten();

        Assert.That(flattened.Count(), Is.EqualTo(0));
    }
}
