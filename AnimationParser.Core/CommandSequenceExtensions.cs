using System.Diagnostics;

namespace AnimationParser.Core;

public static class CommandSequenceExtensions
{
    private abstract class LoopFrame
    {
        public LoopFrame(IEnumerable<IAnimationCommand> commandSequence)
        {
            SequenceEnumerator = commandSequence.GetEnumerator();
        }

        /// <summary>
        /// Indicates if the loop has finished.
        /// </summary>
        public abstract bool IsFinished { get; }

        /// <summary>
        /// Marks the end of an iteration of the loop.
        /// </summary>
        /// <returns>If the loop should continue.</returns>
        public abstract bool OnIterationEnd();

        public IEnumerator<IAnimationCommand> SequenceEnumerator { get; private set; } = null!;
    }

    private class InitialFrame : LoopFrame
    {
        public InitialFrame(IEnumerable<IAnimationCommand> commandSequence)
            : base(commandSequence)
        {
        }

        public override bool IsFinished => !FirstIteration;
        private bool FirstIteration { get; set; } = true;
        public override bool OnIterationEnd()
            => FirstIteration = false;
    }

    private class CountedLoopFrame : LoopFrame
    {
        public CountedLoopFrame(int count, IEnumerable<IAnimationCommand> commandSequence)
            : base(commandSequence)
        {
            RemainingIterations = count;
        }

        public override bool IsFinished => RemainingIterations <= 0;

        public int RemainingIterations { get; set; }

        public override bool OnIterationEnd()
            => --RemainingIterations > 0;
    }

    /// <summary>
    /// Flattens the sequence of commands with loops into a single sequence of commands.
    /// </summary>
    /// <param name="commands">The sequence of commands to flatten.</param>
    /// <returns>The flattened sequence of commands.</returns>
    public static IEnumerable<IAnimationCommand> Flatten(this IEnumerable<IAnimationCommand> commands)
    {
        var loopFrames = new Stack<LoopFrame>();
        loopFrames.Push(new InitialFrame(commands));

        while (loopFrames.TryPeek(out var currentFrame))
        {
            Debug.Assert(currentFrame != null);

            if (currentFrame.IsFinished)
            {
                loopFrames.Pop();
                continue;
            }

            if (currentFrame.SequenceEnumerator.MoveNext())
            {
                switch (currentFrame.SequenceEnumerator.Current)
                {
                    case LoopCommand loopCommand:
                        loopFrames.Push(new CountedLoopFrame(loopCommand.Count, loopCommand.Commands));
                        break;

                    case IAnimationCommand animationCommand:
                        yield return animationCommand;
                        break;

                    default:
                        throw new InvalidOperationException("Should never happen.");
                }
            }
            else
            {
                if (currentFrame.OnIterationEnd())
                {
                    // Reset the enumerator to start the next iteration
                    currentFrame.SequenceEnumerator.Reset();
                }
            }
        }
    }

    /// <summary>
    /// Interprets the source code into a sequence of commands.
    /// Ensure that every returned command does not involve implicit multiple commands.
    /// </summary>
    /// <param name="source">The source code to interpret.</param>
    /// <returns>The sequence of commands at the execution order.</returns>
    public static IEnumerable<IAnimationCommand> InterpretExecutable(this IEnumerable<Token> tokenStream)
    {
        var parser = new Parser(tokenStream);
        return parser.Parse().Flatten();
    }

    /// <summary>
    /// Interprets and executes the source code synchronously.
    /// </summary>
    /// <param name="tokenStream">The tokenized source code.</param>
    /// <param name="context">The context to execute the commands.</param>
    public static void InterprettedlyExecuteAll(this IEnumerable<Token> tokenStream, AnimationContext? context = null)
    {
        context ??= new AnimationContext();

        foreach (var command in tokenStream.InterpretExecutable())
        {
            command.Execute(context);
        }
    }

    /// <summary>
    /// Interprets and executes the source code asynchronously. Every time a command is executed, it yields the control to the caller.
    /// </summary>
    /// <param name="tokenStream">The tokenized source code.</param>
    /// <param name="context">The context to execute the commands.</param>
    public static async void InterprettedlyExecuteAllAsync(this IEnumerable<Token> tokenStream, AnimationContext? context = null)
    {
        context ??= new AnimationContext();

        foreach (var command in tokenStream.InterpretExecutable())
        {
            command.Execute(context);
            await Task.Yield();
        }
    }
}
