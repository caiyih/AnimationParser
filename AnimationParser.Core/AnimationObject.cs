using System.Numerics;
using AnimationParser.Core.Shapes;

namespace AnimationParser.Core;

public class AnimationObject
{
    public List<IShape> Shapes { get; } = [];

    public Vector2 Position { get; set; }

    public void AddShape(IShape shape)
    {
        Shapes.Add(shape);
    }
}
