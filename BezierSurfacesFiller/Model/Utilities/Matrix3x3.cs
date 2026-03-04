using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Model.Utilities
{
    public class Matrix3x3
    {
        public Vector3 col1;
        public Vector3 col2;
        public Vector3 col3;
        public Matrix3x3(Vector3 col1, Vector3 col2, Vector3 col3) 
        {
            this.col1 = col1; this.col2 = col2; this.col3 = col3;
        }

        public static Vector3 operator*(Matrix3x3 M, Vector3 V)
            => M.col1 * V.X + M.col2 * V.Y + M.col3 * V.Z;

    }
}
