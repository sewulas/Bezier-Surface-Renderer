using BezierSurfacesFiller.Model.Utilities;
using BezierSurfacesFiller.View.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Model
{
    public class RenderSettings
    {
        public float Alpha { get; set; } = 0;
        public float Beta { get; set; } = 0;
        public int Resolution { get; set; } = 10;
        public float kd { get; set; } = 0.5f;
        public float ks { get; set; } = 0.5f;
        public int m { get; set; } = 1;
        public Color LightColor { get; set; } = Color.FromArgb(255,255,255); // default = white
        public Vector3 LightSourceCordinates { get; set; } = new Vector3(200, 200, 400);
        public Color SurfaceColor { get; set; } = Color.LimeGreen; // default = LimeGreen
        public TextureBuffer? Texture { get; set; } = null;
        public bool IsTextureOptionOn = false;
        public TextureBuffer? Map { get; set; } = null;
        public bool IsMapOptionOn = false;

        public bool IsTextureLoaded()
        {
            return Texture != null;
        }
        public bool IsMapLoaded()
        {
            return Map != null;
        }
    }
}
