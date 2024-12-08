using osu.Framework.Testing;

namespace AnimationParser.Game.Tests.Visual
{
    public abstract partial class AnimationParserTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new AnimationParserTestSceneTestRunner();

        private partial class AnimationParserTestSceneTestRunner : AnimationParserGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
