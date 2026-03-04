using BezierSurfacesFiller.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Controller.Animations
{
    public class LightAnimationSpiralMovement : IAnimation
    {
        public bool Enabled { get; set; } = false;
        private readonly RenderSettings _settings;

        private float _t = 0f;
        private float _dt = 0.15f;
        private float a = 50f; // starting radius
        private float b = 8f;  // how spiral expans

        public LightAnimationSpiralMovement(RenderSettings seetings)
        {
            _settings = seetings;
        }


        public void Update()
        {
            _t += _dt; // increase angle

            var z = _settings.LightSourceCordinates.Z; // constant from slider

            var r = a + b * _t;
            float x = r * MathF.Cos(_t);
            float y = r * MathF.Sin(_t);

            _settings.LightSourceCordinates = new Vector3(x, y, z);
        }
    }
}
