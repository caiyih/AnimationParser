namespace AnimationParser.Core.Commands;

public interface IAnimationCommand
{
    public void Execute(AnimationContext context);
}
