namespace AnimationParser.Core.Commands;

public class DefineCommand : IAnimationCommand
{
    public string ObjectName { get; }

    public AnimationObject Object { get; set; }

    public DefineCommand(string objectName, AnimationObject obj)
    {
        ObjectName = objectName;
        Object = obj;
    }

    public void Execute(AnimationContext context)
    {
        context.DeclareObject(ObjectName, Object);
    }
}
