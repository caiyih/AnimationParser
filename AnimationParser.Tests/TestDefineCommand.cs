using AnimationParser.Core;

namespace AnimationParser.Tests;

public class TestDefineCommand
{
    [Test]
    public void TestDefineCommand_WhenDrawableIsDeclared_ShouldAddDrawableToContext()
    {
        const string code = "(define drawable ((line (0 0) (50 50))))";

        var context = new DefineMockedContext();

        var lexer = new Lexer(code);
        var parser = new Parser(lexer.Tokenize());

        var commands = parser.Parse();

        foreach (var command in commands)
        {
            command.Execute(context);
        }

        Assert.That(context.IsDrawableAdded, Is.True);
        Assert.That(context.GetObject("drawable"), Is.Not.Null);
    }


    private class DefineMockedContext : AnimationContext
    {
        public bool IsDrawableAdded { get; private set; }

        protected override void OnObjectAdded(string name, AnimationObject value)
        {
            if (name == "drawable")
            {
                IsDrawableAdded = true;
            }
            else
            {
                throw new Exception($"Unexpected drawable name: {name}");
            }
        }
    }
}
