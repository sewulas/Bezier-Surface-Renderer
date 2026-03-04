using BezierSurfacesFiller.Controller;
using BezierSurfacesFiller.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.View.Rendering
{
    public interface IRender
    {
        void Render(Graphics g, RenderContext rtx);
    }

    public static class RenderFactory
    {
        private static readonly Dictionary<string, Func<IRender>> _renderers =
            new()
            {
                { "ControlPolygon", () => new ControlPointsRender() },
                { "Wireframe", () => new WireframeRender() },
                { "FillTriangles", () => new FillTrianglesRender() }
            };
        public static IRender Create(string key) 
        {
            return _renderers[key]();
        }
    }
}
