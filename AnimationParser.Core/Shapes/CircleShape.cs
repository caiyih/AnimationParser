using System.Numerics;

namespace AnimationParser.Core.Shapes;

public class CircleShape : IShape
{
    public ShapeType Type => ShapeType.Circle;

    public Vector2 Center { get; }

    public float Radius { get; }

    public CircleShape(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }
}
