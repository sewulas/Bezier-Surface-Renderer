using BezierSurfacesFiller.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.View.Rendering
{
    public class Renderer
    {
        private List<IRender> renders = new();
        private Dictionary<Type, int> renderPriority = new();
        public Renderer() 
        {
            renderPriority[typeof(ControlPointsRender)] = 0;
            renderPriority[typeof(WireframeRender)] = 1;
            renderPriority[typeof(FillTrianglesRender)] = 2;
        }
        public void InsertRenders(List<IRender> renders)
        {
            this.renders.AddRange(renders);
        }
        public void AddRender(IRender render) => renders.Add(render);
        public void RemoveRender(IRender render)
        {
            renders.RemoveAll(r => r.GetType() == render.GetType());
        }
        public void Clear() => renders.Clear();
        public void RenderAll(Graphics g, RenderContext rtx)
        {
            var ordered = renders
             .OrderByDescending(r =>
                 renderPriority.TryGetValue(r.GetType(), out int p)
                 ? p
                 : int.MinValue) // brak priority → najniższy możliwy
             .ToList();

            foreach (var render in ordered)
                render.Render(g, rtx);
        }

    }
}
