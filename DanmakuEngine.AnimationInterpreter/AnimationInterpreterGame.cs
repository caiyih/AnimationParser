using DanmakuEngine.Games;
using DanmakuEngine.Games.Screens;

namespace DanmakuEngine.AnimationInterpreter;

public class AnimationInterpreterGame : Game
{
    public override string Name => base.Name;
    public override Screen EntryScreen => new DemoScreen();
}