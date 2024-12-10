using AnimationParser.Core;

namespace AnimationParser.Tests;

public class TestCommandExtensions
{
    [Test]
    public async Task TestAsyncExecution_ConcurrentlyRun()
    {
        const string code = """
                            (define drawable ( (line (0 0) (50 50)) ) )
                            (loop 1 ( (shift drawable right) ))
                            """;

        var lexer = new Lexer(code);
        var timeoutTask = Task.Delay(1000);
        var executionTask = lexer.Tokenize().InterprettedlyExecuteAllAsync(new MockedAnimationContext());

        Assert.That(await Task.WhenAny(timeoutTask, executionTask), Is.EqualTo(executionTask));
    }

    [Test]
    public async Task TestAsyncExecution_NonBlocking()
    {
        const string code = """
                            (define drawable ( (line (0 0) (50 50)) ) )
                            (loop 100 ((shift drawable right)))
                            """;

        var lexer = new Lexer(code);
        var timeoutTask = Task.Delay(100);
        var executionTask = lexer.Tokenize().InterprettedlyExecuteAllAsync(new MockedAnimationContext());

        Assert.That(await Task.WhenAny(timeoutTask, executionTask), Is.EqualTo(timeoutTask));
    }

    private class MockedAnimationContext : AnimationContext
    {
        protected override void OnObjectShifting(string name, AnimationObject obj, Direction direction)
        {
            Thread.Sleep(100);

            base.OnObjectShifting(name, obj, direction);
        }
    }
}