namespace AnimationParser.Core;

public class ShiftCommand : IAnimationCommand
{
    public string ObjectName { get; }

    public Direction Direction { get; }

    public ShiftCommand(string objectName, Direction direction)
    {
        ObjectName = objectName;
        Direction = direction;
    }

    public void Execute(AnimationContext context)
    {
        context.ShiftObject(ObjectName, Direction);
    }
}
