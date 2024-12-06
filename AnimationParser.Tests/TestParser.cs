using System.Numerics;
using AnimationParser.Core;

namespace AnimationParser.Tests;

public class TestParser
{
    private MockedTokenFactory tokenFactory = null!;

    [SetUp]
    public void SetUp()
    {
        tokenFactory = new MockedTokenFactory();
    }

    [TearDown]
    public void TearDown()
    {
        tokenFactory = null!;
    }

    [Test]
    public void TestParseNumber()
    {
        var parser = new Parser([
            tokenFactory.Number("42.0")
        ]);

        Assert.That(parser.VisitNumber(), Is.EqualTo(42.0));
    }

    [Test]
    public void TestParseVector2()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
            tokenFactory.Number("42.0"),
            tokenFactory.Number("24.0"),
            tokenFactory.RightParen,
        ]);

        Assert.That(parser.VisitVector2(), Is.EqualTo(new Vector2(42.0f, 24.0f)));
    }

    public void TestParseDirection()
    {
        var parser = new Parser([
            tokenFactory.Keyword("up"),
            tokenFactory.Keyword("down"),
            tokenFactory.Keyword("left"),
            tokenFactory.Keyword("right"),
        ]);

        Assert.That(parser.VisitDirection(), Is.EqualTo(Direction.Up));
        Assert.That(parser.VisitDirection(), Is.EqualTo(Direction.Down));
        Assert.That(parser.VisitDirection(), Is.EqualTo(Direction.Left));
        Assert.That(parser.VisitDirection(), Is.EqualTo(Direction.Right));
    }

    [Test]
    public void TestParseCircleShape()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
                tokenFactory.Keyword("circle"),
                tokenFactory.LeftParen,
                    tokenFactory.Number("42.0"),
                    tokenFactory.Number("24.0"),
                tokenFactory.RightParen,
                tokenFactory.Number("10.0"),
            tokenFactory.RightParen,
        ]);

        parser.MoveNextIsNotNull(); // skip '('

        var circle = parser.VisitShapeCommand();

        Assert.Multiple(() =>
        {
            Assert.That(circle, Is.InstanceOf<CircleShape>());
            Assert.That(((CircleShape)circle).Center, Is.EqualTo(new Vector2(42.0f, 24.0f)));
            Assert.That(((CircleShape)circle).Radius, Is.EqualTo(10.0f));
        });
    }

    [Test]
    public void TestParseLine()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
                tokenFactory.Keyword("line"),
                tokenFactory.LeftParen,
                    tokenFactory.Number("42.0"),
                    tokenFactory.Number("24.0"),
                tokenFactory.RightParen,
                tokenFactory.LeftParen,
                    tokenFactory.Number("10.0"),
                    tokenFactory.Number("20.0"),
                tokenFactory.RightParen,
            tokenFactory.RightParen,
        ]);

        parser.MoveNextIsNotNull(); // skip '('

        var line = parser.VisitShapeCommand();

        Assert.Multiple(() =>
        {
            Assert.That(line, Is.InstanceOf<LineShape>());
            Assert.That(((LineShape)line).Start, Is.EqualTo(new Vector2(42.0f, 24.0f)));
            Assert.That(((LineShape)line).End, Is.EqualTo(new Vector2(10.0f, 20.0f)));
        });
    }

    [Test]
    public void TestParsePlaceCommand()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
                tokenFactory.Keyword("place"),
                tokenFactory.Identifier("object"),
                tokenFactory.LeftParen,
                    tokenFactory.Number("42.0"),
                    tokenFactory.Number("24.0"),
                tokenFactory.RightParen,
            tokenFactory.RightParen,
        ]);

        parser.MoveNextIsNotNull(); // skip '('

        var place = parser.VisitCommand();

        Assert.Multiple(() =>
        {
            Assert.That(place, Is.InstanceOf<PlaceCommand>());
            Assert.That(((PlaceCommand)place).ObjectName, Is.EqualTo("object"));
            Assert.That(((PlaceCommand)place).Position, Is.EqualTo(new Vector2(42.0f, 24.0f)));
        });
    }

    [Test]
    public void TestParseShiftCommand()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
            tokenFactory.Keyword("shift"),
            tokenFactory.Identifier("object"),
            tokenFactory.Keyword("up"),
            tokenFactory.RightParen,
        ]);

        parser.MoveNextIsNotNull(); // skip '('

        var shift = parser.VisitCommand();

        Assert.Multiple(() =>
        {
            Assert.That(shift, Is.InstanceOf<ShiftCommand>());
            Assert.That(((ShiftCommand)shift).ObjectName, Is.EqualTo("object"));
            Assert.That(((ShiftCommand)shift).Direction, Is.EqualTo(Direction.Up));
        });
    }

    [Test]
    public void TestParseEraseCommand()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
            tokenFactory.Keyword("erase"),
            tokenFactory.Identifier("object"),
            tokenFactory.RightParen,
        ]);

        parser.MoveNextIsNotNull(); // skip '('

        var erase = parser.VisitCommand();

        Assert.Multiple(() =>
        {
            Assert.That(erase, Is.InstanceOf<EraseCommand>());
            Assert.That(((EraseCommand)erase).ObjectName, Is.EqualTo("object"));
        });
    }

    [Test]
    public void TestParseLoopCommand()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
                tokenFactory.Keyword("loop"),
                tokenFactory.Number("42"),
                tokenFactory.LeftParen,
                    tokenFactory.LeftParen,
                        tokenFactory.Keyword("place"),
                        tokenFactory.Identifier("object"),
                        tokenFactory.LeftParen,
                            tokenFactory.Number("42.0"),
                            tokenFactory.Number("24.0"),
                        tokenFactory.RightParen,
                    tokenFactory.RightParen,
                    tokenFactory.LeftParen,
                        tokenFactory.Keyword("erase"),
                        tokenFactory.Identifier("object"),
                    tokenFactory.RightParen,
                    tokenFactory.LeftParen,
                        tokenFactory.Keyword("shift"),
                        tokenFactory.Identifier("object"),
                        tokenFactory.Keyword("up"),
                    tokenFactory.RightParen,
                tokenFactory.RightParen,
            tokenFactory.RightParen,
        ]);

        parser.MoveNextIsNotNull(); // skip '('

        var loop = parser.VisitCommand();

        Assert.Multiple(() =>
        {
            Assert.That(loop, Is.InstanceOf<LoopCommand>());
            Assert.That(((LoopCommand)loop).Count, Is.EqualTo(42));

            var commands = ((LoopCommand)loop).Commands;

            Assert.That(commands.Count(), Is.EqualTo(3));

            Assert.That(commands.ElementAt(0), Is.InstanceOf<PlaceCommand>());
            Assert.That(commands.ElementAt(1), Is.InstanceOf<EraseCommand>());
            Assert.That(commands.ElementAt(2), Is.InstanceOf<ShiftCommand>());
        });
    }

    [Test]
    public void TestParseMultipleStatement()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
                tokenFactory.Keyword("place"),
                tokenFactory.Identifier("object"),
                tokenFactory.LeftParen,
                    tokenFactory.Number("42.0"),
                    tokenFactory.Number("24.0"),
                tokenFactory.RightParen,
            tokenFactory.RightParen,
            tokenFactory.LeftParen,
                tokenFactory.Keyword("erase"),
                tokenFactory.Identifier("object"),
            tokenFactory.RightParen,
            tokenFactory.LeftParen,
                tokenFactory.Keyword("shift"),
                tokenFactory.Identifier("object"),
                tokenFactory.Keyword("up"),
            tokenFactory.RightParen,
            tokenFactory.LeftParen,
                tokenFactory.Keyword("loop"),
                tokenFactory.Number("42"),
                tokenFactory.LeftParen,
                    // Empty loop body
                tokenFactory.RightParen,
            tokenFactory.RightParen,
            tokenFactory.LeftParen,
                tokenFactory.Keyword("place"),
                tokenFactory.Identifier("object"),
                tokenFactory.LeftParen,
                    tokenFactory.Number("42.0"),
                    tokenFactory.Number("24.0"),
                tokenFactory.RightParen,
            tokenFactory.RightParen,
            tokenFactory.EndOfSource,
        ]);

        var commands = parser.Parse().ToList();

        Assert.Multiple(() =>
        {
            Assert.That(commands, Has.Count.EqualTo(5));

            Assert.That(commands.ElementAt(0), Is.InstanceOf<PlaceCommand>());
            Assert.That(commands.ElementAt(1), Is.InstanceOf<EraseCommand>());
            Assert.That(commands.ElementAt(2), Is.InstanceOf<ShiftCommand>());
            Assert.That(commands.ElementAt(3), Is.InstanceOf<LoopCommand>());
            Assert.That(commands.ElementAt(4), Is.InstanceOf<PlaceCommand>());
        });
    }

    [Test]
    public void TestParseDrawList()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
                tokenFactory.LeftParen,
                    tokenFactory.Keyword("circle"),
                    tokenFactory.LeftParen,
                        tokenFactory.Number("42.0"),
                        tokenFactory.Number("24.0"),
                    tokenFactory.RightParen,
                    tokenFactory.Number("10.0"),
                tokenFactory.RightParen,
                tokenFactory.LeftParen,
                    tokenFactory.Keyword("line"),
                    tokenFactory.LeftParen,
                        tokenFactory.Number("42.0"),
                        tokenFactory.Number("24.0"),
                    tokenFactory.RightParen,
                    tokenFactory.LeftParen,
                        tokenFactory.Number("10.0"),
                        tokenFactory.Number("20.0"),
                    tokenFactory.RightParen,
                tokenFactory.RightParen,
                tokenFactory.LeftParen,
                    tokenFactory.Keyword("circle"),
                    tokenFactory.LeftParen,
                        tokenFactory.Number("24.0"),
                        tokenFactory.Number("42.0"),
                    tokenFactory.RightParen,
                    tokenFactory.Number("1.0"),
                tokenFactory.RightParen,
                tokenFactory.LeftParen,
                    tokenFactory.Keyword("line"),
                    tokenFactory.LeftParen,
                        tokenFactory.Number("24.0"),
                        tokenFactory.Number("42.0"),
                    tokenFactory.RightParen,
                    tokenFactory.LeftParen,
                        tokenFactory.Number("20.0"),
                        tokenFactory.Number("10.0"),
                    tokenFactory.RightParen,
                tokenFactory.RightParen,
            tokenFactory.RightParen,
        ]);

        var drawList = parser.VisitDrawList().Shapes;

        Assert.Multiple(() =>
        {
            Assert.That(drawList, Has.Count.EqualTo(4));

            Assert.That(drawList.ElementAt(0), Is.InstanceOf<CircleShape>());
            Assert.That(drawList.ElementAt(1), Is.InstanceOf<LineShape>());
            Assert.That(drawList.ElementAt(2), Is.InstanceOf<CircleShape>());
            Assert.That(drawList.ElementAt(3), Is.InstanceOf<LineShape>());
        });
    }

    [Test]
    public void TestParseDefineCommand()
    {
        var parser = new Parser([
            tokenFactory.LeftParen,
                tokenFactory.Keyword("define"),
                tokenFactory.Identifier("cross"),
                tokenFactory.LeftParen,
                    tokenFactory.LeftParen,
                        tokenFactory.Keyword("line"),
                        tokenFactory.LeftParen,
                            tokenFactory.Number("0"),
                            tokenFactory.Number("0"),
                        tokenFactory.RightParen,
                        tokenFactory.LeftParen,
                            tokenFactory.Number("50.0"),
                            tokenFactory.Number("50.0"),
                        tokenFactory.RightParen,
                    tokenFactory.RightParen,
                tokenFactory.RightParen,
            tokenFactory.RightParen,
        ]);

        parser.MoveNextIsNotNull(); // skip '('

        var define = parser.VisitCommand();

        Assert.Multiple(() =>
        {
            Assert.That(define, Is.InstanceOf<DefineCommand>());
            Assert.That(((DefineCommand)define).ObjectName, Is.EqualTo("cross"));

            var shapes = ((DefineCommand)define).Object.Shapes;

            Assert.That(shapes, Has.Count.EqualTo(1));
            Assert.That(shapes.ElementAt(0), Is.InstanceOf<LineShape>());

            var line = (LineShape)shapes.ElementAt(0);
            Assert.That(line.Start, Is.EqualTo(new Vector2(0, 0)));
            Assert.That(line.End, Is.EqualTo(new Vector2(50, 50)));
        });
    }

    [Test]
    public void TestLexerToParser()
    {
        const string input = """
                             (define cross ( (line (0 0) (50 50)) (line (50 0) (0 50)) (circle (25 25) 25) ) )
                             (place cross (50 50))
                             (loop 100 ( (shift cross right) (shift cross up) ) )
                             (erase cross)
                             """;

        var lexer = new Lexer(input);
        var parser = new Parser(lexer.Tokenize());

        var commands = parser.Parse().ToList();

        Assert.Multiple(() =>
        {
            Assert.That(commands, Has.Count.EqualTo(4));

            Assert.That(commands.ElementAt(0), Is.InstanceOf<DefineCommand>());
            Assert.That(commands.ElementAt(1), Is.InstanceOf<PlaceCommand>());
            Assert.That(commands.ElementAt(2), Is.InstanceOf<LoopCommand>());
            Assert.That(commands.ElementAt(3), Is.InstanceOf<EraseCommand>());

            var cross = (DefineCommand)commands.ElementAt(0);
            var place = (PlaceCommand)commands.ElementAt(1);
            var loop = (LoopCommand)commands.ElementAt(2);
            var erase = (EraseCommand)commands.ElementAt(3);

            Assert.That(cross.ObjectName, Is.EqualTo("cross"));
            Assert.That(place.ObjectName, Is.EqualTo("cross"));
            Assert.That(loop.Count, Is.EqualTo(100));
            Assert.That(erase.ObjectName, Is.EqualTo("cross"));

            var crossShapes = cross.Object.Shapes;
            var loopCommands = loop.Commands;

            Assert.That(crossShapes, Has.Count.EqualTo(3));
            Assert.That(loopCommands.Count(), Is.EqualTo(2));

            Assert.That(crossShapes.ElementAt(0), Is.InstanceOf<LineShape>());
            var line = (LineShape)crossShapes.ElementAt(0);
            Assert.That(line.Start, Is.EqualTo(new Vector2(0, 0)));
            Assert.That(line.End, Is.EqualTo(new Vector2(50, 50)));

            Assert.That(crossShapes.ElementAt(1), Is.InstanceOf<LineShape>());
            line = (LineShape)crossShapes.ElementAt(1);
            Assert.That(line.Start, Is.EqualTo(new Vector2(50, 0)));
            Assert.That(line.End, Is.EqualTo(new Vector2(0, 50)));

            Assert.That(crossShapes.ElementAt(2), Is.InstanceOf<CircleShape>());
            var circle = (CircleShape)crossShapes.ElementAt(2);
            Assert.That(circle.Center, Is.EqualTo(new Vector2(25, 25)));
            Assert.That(circle.Radius, Is.EqualTo(25));

            Assert.That(loopCommands.ElementAt(0), Is.InstanceOf<ShiftCommand>());
            var shift = (ShiftCommand)loopCommands.ElementAt(0);
            Assert.That(shift.ObjectName, Is.EqualTo("cross"));
            Assert.That(shift.Direction, Is.EqualTo(Direction.Right));

            Assert.That(loopCommands.ElementAt(1), Is.InstanceOf<ShiftCommand>());
            shift = (ShiftCommand)loopCommands.ElementAt(1);
            Assert.That(shift.ObjectName, Is.EqualTo("cross"));
            Assert.That(shift.Direction, Is.EqualTo(Direction.Up));
        });
    }
}
