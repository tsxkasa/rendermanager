using System;
using System.Diagnostics;
using System.Threading;
using Dalamud.Plugin.Services;

namespace RenderManager.System
{
    public class FrameLimiter : IDisposable
    {
        private readonly IFramework framework;
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public bool IsEnabled {get; set;} = false;
        public double TargetFps {get; set;} = 60.0;

        public FrameLimiter(IFramework fw) {
            this.framework = fw;

            this.framework.Update += OnFrameworkUpdate;
        }
        private void OnFrameworkUpdate(IFramework framework) {
            if (!IsEnabled || TargetFps <= 0) return;

            double targetFrameTime = 1000.0 / TargetFps;
            var delayTime = (int) (targetFrameTime - stopwatch.ElapsedMilliseconds);

            if (delayTime > 1) {
                Thread.Sleep(delayTime - 1);
            }

            stopwatch.Restart();
        }

        public void Dispose() {
            this.framework.Update -= OnFrameworkUpdate;
        }

    }
}
