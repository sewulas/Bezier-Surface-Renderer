using BezierSurfacesFiller.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.View.Rendering
{
    public class RenderContext
    {
        public BezierSurface Surface { get; init; }
        public Vector3[,] ControlPointsRotated { get; init; }

        public RenderSettings Settings { get; init; }

        // renderowanie z bitmapa
        public int Width = 0;
        public int Height = 0;
        public int MinX = 0;
        public int MaxX = 0;
        public int MinY = 0;
        public int MaxY = 0;
    }
}
