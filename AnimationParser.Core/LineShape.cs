using System.Numerics;

namespace AnimationParser.Core;

public class LineShape : IShape
{
    public ShapeType Type => ShapeType.Line;

    public Vector2 Start { get; }

    public Vector2 End { get; }

    public LineShape(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
    }
}
