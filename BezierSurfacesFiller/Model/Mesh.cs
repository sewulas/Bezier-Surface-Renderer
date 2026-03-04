using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Model
{
    public class Mesh
    {
        public List<Triangle> Triangles { get; set; } = new();
        public Mesh(List<Triangle> Triangles = null) 
        {
            this.Triangles = Triangles ?? new List<Triangle>();
        }
        public void ApplyRotations(float alpha, float beta)
        {
            foreach (var t in Triangles)
            {
                t.A.Rotate(alpha, beta);
                t.B.Rotate(alpha, beta);
                t.C.Rotate(alpha, beta);
            }
        }
    }
}
