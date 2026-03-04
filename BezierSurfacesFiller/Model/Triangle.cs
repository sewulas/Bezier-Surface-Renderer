using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Model
{
    public class Triangle
    {
        public Vertex A;
        public Vertex B;
        public Vertex C;

        public Triangle(Vertex A, Vertex B, Vertex C) 
        {
            this.A = A; 
            this.B = B;
            this.C = C;
        }
    }
}
