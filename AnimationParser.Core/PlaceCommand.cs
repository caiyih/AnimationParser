using System.Numerics;

namespace AnimationParser.Core;

public class PlaceCommand : IAnimationCommand
{
    public string ObjectName { get; }
    public Vector2 Position { get; }

    public PlaceCommand(string objectName, Vector2 position)
    {
        ObjectName = objectName;
        Position = position;
    }

    public void Execute(AnimationContext context)
    {
        context.PlaceObject(ObjectName, Position);
    }
}
