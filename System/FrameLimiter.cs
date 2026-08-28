using System;
using System.Diagnostics;
using System.Threading;
using Dalamud.Plugin.Services;

namespace RenderManager.System
{
    public class FrameLimiter : IDisposable
    {
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public bool IsEnabled {get; set;} = false;
        public double TargetFps {get; set;} = 60.0;

        public FrameLimiter() {
            Service.Framework.Update += OnFrameworkUpdate;
        }
        private void OnFrameworkUpdate(IFramework framework) {
            if (!IsEnabled || TargetFps <= 0) return;

            var targetFrameTime = 1000.0 / TargetFps;
            var delayTime = (int) (targetFrameTime - stopwatch.ElapsedMilliseconds);

            if (delayTime > 1) {
                Thread.Sleep(delayTime - 1);
            }

            stopwatch.Restart();
        }

        public void Dispose() {
            Service.Framework.Update -= OnFrameworkUpdate;
        }

    }
}
