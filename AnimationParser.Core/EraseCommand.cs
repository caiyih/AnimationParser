namespace AnimationParser.Core;

public class EraseCommand : IAnimationCommand
{
    public string ObjectName { get; }

    public EraseCommand(string objectName)
    {
        ObjectName = objectName;
    }

    public void Execute(AnimationContext context)
    {
        context.EraseObject(ObjectName);
    }
}
