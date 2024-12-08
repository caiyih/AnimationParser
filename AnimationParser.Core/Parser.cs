using System.Numerics;
using System.Runtime.CompilerServices;
using AnimationParser.Core.Commands;
using AnimationParser.Core.Shapes;
using AnimationParser.Core.Tokens;

namespace AnimationParser.Core;

public class Parser
{
    private IEnumerable<Token> Tokens { get; }

    private IEnumerator<Token> TokenEnumerator { get; }

    public Token CurrentToken => TokenEnumerator.Current;

    private bool hasToken = true;
    public bool HasToken => hasToken;

    private Token? MoveNext()
    {
        if (!HasToken || !(hasToken = TokenEnumerator.MoveNext()))
            return null!;

        return CurrentToken;
    }

    public Parser(IEnumerable<Token> tokens)
    {
        Tokens = tokens;
        TokenEnumerator = Tokens.GetEnumerator();
    }

    /// <summary>
    /// Parses the tokens into a sequence of animation commands. The returned
    /// sequence is lazy evaluated and faithfully represents the input source.
    /// To Get the execution sequence, use <see cref="CommandSequenceExtensions.Flatten"/>,
    /// which flatten the loop commands into a single sequence of commands.
    /// </summary>
    /// <returns>A sequence of animation commands faithfully representing the input source</returns>
    public IEnumerable<IAnimationCommand> Parse()
    {
        while (HasToken)
        {
            MoveNextIsNotNull();

            if (CurrentToken.Type is TokenType.EndOfSource)
                break;

            Assert(CurrentToken.Type is TokenType.LeftParen, "Expected '('");

            yield return VisitCommand();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // Inlining this method to avoid garbage stack trace information
    private void Assert(bool predicate, string message)
    {
        if (!predicate)
        {
            throw new Exception($"{message}. Current token: {CurrentToken}");
        }
    }

    public void MoveNextIsNotNull()
    {
        Assert(HasToken, "Unexpected end of input");
        Assert(MoveNext() is not null, "Unexpected end of input");
    }

    public IAnimationCommand VisitCommand()
    {
        MoveNextIsNotNull();
        var commandToken = CurrentToken;

        Assert(commandToken.Type is TokenType.Keyword, $"Expected command keyword, but got: {commandToken}");

        var command = commandToken.Text switch
        {
            "define" => (IAnimationCommand)VisitDefineCommand(),
            "place" => (IAnimationCommand)VisitPlaceCommand(),
            "shift" => (IAnimationCommand)VisitShiftCommand(),
            "erase" => (IAnimationCommand)VisitEraseCommand(),
            "loop" => (IAnimationCommand)VisitLoopCommand(),
            _ => throw new Exception($"Unexpected command: {commandToken.Text}")
        };

        return command;
    }

    public AnimationObject VisitDrawList()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.LeftParen, "Expected '('");

        var obj = new AnimationObject();

        while (HasToken)
        {
            MoveNextIsNotNull();

            switch (CurrentToken.Type)
            {
                case TokenType.RightParen: // end of draw list
                    return obj;

                case TokenType.LeftParen:
                    obj.AddShape(VisitShapeCommand());
                    break;

                default:
                    throw new Exception($"Expected '(' but got: {CurrentToken}");
            }
        }

        throw new Exception("Unclosed draw list");
    }

    public IShape VisitShapeCommand()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.Keyword, "Expected keyword");

        var shape = CurrentToken.Text! switch
        {
            "circle" => (IShape)VisitCircleShape(),
            "line" => (IShape)VisitLineShape(),
            _ => throw new Exception($"Unexpected shape command: {CurrentToken.Text}")
        };

        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.RightParen, "Expected ')'"); // close shape command

        return shape;
    }

    public float VisitNumber()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.Number, "Expected number");

        if (!float.TryParse(CurrentToken.Text, out var number))
        {
            throw new Exception($"Expected number but got: {CurrentToken}");
        }

        return number;
    }

    public CircleShape VisitCircleShape()
    {
        return new CircleShape(VisitVector2(), VisitNumber());
    }

    public LineShape VisitLineShape()
    {
        return new LineShape(VisitVector2(), VisitVector2());
    }

    public Vector2 VisitVector2()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.LeftParen, "Expected '('");

        var x = VisitNumber();
        var y = VisitNumber();

        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.RightParen, "Expected ')'");

        return new Vector2(x, y);
    }

    public Direction VisitDirection()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.Keyword, "Expected keyword");

        return CurrentToken.Text switch
        {
            "up" => Direction.Up,
            "down" => Direction.Down,
            "left" => Direction.Left,
            "right" => Direction.Right,
            _ => throw new Exception($"Unexpected direction: {CurrentToken.Text}")
        };
    }

    public DefineCommand VisitDefineCommand()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.Identifier, "Expected identifier");

        var objName = CurrentToken.Text;
        Assert(objName.Length > 0, "Name of an object can not be empty");

        var obj = VisitDrawList();

        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.RightParen, "Expected ')'"); // close command

        return new DefineCommand(objName.ToString(), obj);
    }

    public PlaceCommand VisitPlaceCommand()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.Identifier, "Expected identifier");

        var objName = CurrentToken.Text;
        Assert(objName.Length > 0, "Object name can not be empty");

        var position = VisitVector2();

        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.RightParen, "Expected ')'"); // close command

        return new PlaceCommand(objName.ToString(), position);
    }

    public ShiftCommand VisitShiftCommand()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.Identifier, "Expected identifier");

        var objName = CurrentToken.Text;
        Assert(objName.Length > 0, "Name of an object can not be empty");

        var direction = VisitDirection();

        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.RightParen, "Expected ')'"); // close command

        return new ShiftCommand(objName.ToString(), direction);
    }

    public EraseCommand VisitEraseCommand()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.Identifier, "Expected identifier");

        var objName = CurrentToken.Text!;
        Assert(objName.Length > 0, "Name of an object can not be empty");

        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.RightParen, "Expected ')'"); // close command

        return new EraseCommand(objName.ToString());
    }

    public LoopCommand VisitLoopCommand()
    {
        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.Number, "Expected number");

        int count = int.Parse(CurrentToken.Text!);

        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.LeftParen, "Expected '('"); // begin loop commands

        var commands = new List<IAnimationCommand>();

        while (HasToken)
        {
            MoveNextIsNotNull();
            if (CurrentToken.Type is TokenType.RightParen) // close the list of commands
                break;

            commands.Add(VisitCommand());
        }

        MoveNextIsNotNull();
        Assert(CurrentToken.Type is TokenType.RightParen, "Expected ')'"); // close command

        return new LoopCommand(count, commands);
    }
}
