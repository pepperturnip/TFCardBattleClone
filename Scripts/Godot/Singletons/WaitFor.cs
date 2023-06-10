using System;
using System.Threading.Tasks;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class WaitFor : Node
    {
        private static WaitFor _instance = null;
        private static TaskCompletionSource<double> _nextFrameTcs = new TaskCompletionSource<double>();

        /// <summary>
        /// Waits until the next frame, and returns the delta time.
        /// </summary>
        /// <returns></returns>
        public static Task<double> NextFrame() => _nextFrameTcs.Task;

        public static async Task Seconds(double seconds)
        {
            var timer = _instance.GetTree().CreateTimer(seconds);
            await timer.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        public override void _Ready()
        {
            if (_instance != null)
                throw new Exception("WaitFor is supposed to be a singleton, but there's more than one.");

            _instance = this;
        }

        public override void _Process(double delta)
        {
            var tcs = _nextFrameTcs;
            _nextFrameTcs = new TaskCompletionSource<double>();
            tcs.SetResult(delta);
        }
    }
}