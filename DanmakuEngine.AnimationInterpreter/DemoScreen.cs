using DanmakuEngine.DearImgui.Windowing;
using DanmakuEngine.Games.Screens;

namespace DanmakuEngine.AnimationInterpreter;

public class DemoScreen : Screen
{
    private ImguiWindowBase _demoWindow = new AnimationWindow();
    protected override void Start()
    {
        base.Start();

        Scheduler.ScheduleTask(() =>
        {
            _demoWindow.Register();
        }, () => ScreenClock.ElapsedSeconds > 2);
    }
}