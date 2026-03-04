using BezierSurfacesFiller.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BezierSurfacesFiller.Controller.Animations
{


    public class AnimationController
    {
        private readonly RenderSettings _settings;
        private readonly Action _refresh;
        private readonly System.Windows.Forms.Timer _timer;

        private List<IAnimation> _animations = new();
        public bool IsLightAnimationOn { get; private set; } = false;

        public AnimationController(RenderSettings settings, Action refresh)
        {
            _settings = settings;
            _refresh = refresh;

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 16; // ~60 FPS
            _timer.Tick += OnTick;
            _timer.Start();
        }

        public void AddAnimation(IAnimation animation) => _animations.Add(animation);


        private void OnTick(object? sender, EventArgs e)
        {
            foreach (var animation in _animations)
                if (animation.Enabled)
                {
                    animation.Update();
                    _refresh();
                }
        }
    }
}
