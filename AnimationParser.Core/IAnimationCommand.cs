namespace AnimationParser.Core;

public interface IAnimationCommand
{
    public void Execute(AnimationContext context);
}
