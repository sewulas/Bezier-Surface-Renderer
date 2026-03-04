using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Model
{
    public class Vertex
    {
        // parametry powierzchni
        public float U { get; set; }
        public float V { get; set; }

        // przed obrotem
        public Vector3 P { get; set; }
        public Vector3 Pu { get; set; }
        public Vector3 Pv { get; set; }
        public Vector3 N { get; set; }

        // po obrocie
        public Vector3 PRot { get; set; }
        public Vector3 PuRot { get; set; }
        public Vector3 PvRot { get; set; }
        public Vector3 NRot { get; set; }

        public Vertex(float u, float v)
        {
            U = u;
            V = v;
        }

        public void Rotate(float alpha, float beta)
        {
            float a = alpha * MathF.PI / 180f;
            var cos = MathF.Cos(a);
            var sin = MathF.Sin(a);
            PRot = P.RotateZ(cos, sin);
            PuRot = Pu.RotateZ(cos, sin);
            PvRot = Pv.RotateZ(cos, sin);
            NRot = N.RotateZ(cos, sin);

            float b = beta * MathF.PI / 180f;
            cos = MathF.Cos(b);
            sin = MathF.Sin(b);
            PRot = PRot.RotateX(cos, sin);
            PuRot = PuRot.RotateX(cos, sin);
            PvRot = PvRot.RotateX(cos, sin);
            NRot = NRot.RotateX(cos, sin);
        }

        //public void RotateZ(float alpha)
        //{
        //    float a = alpha * MathF.PI / 180f;
        //    var cos = MathF.Cos(a);
        //    var sin = MathF.Sin(a);
        //    PRot   = P.RotateZ(cos, sin);
        //    PuRot  = Pu.RotateZ(cos, sin);
        //    PvRot  = Pv.RotateZ(cos, sin);
        //    NRot   = N.RotateZ(cos, sin);
        //}

        //public void RotateX(float beta)
        //{
        //    float b = beta * MathF.PI / 180f;
        //    var cos = MathF.Cos(b);
        //    var sin = MathF.Sin(b);
        //    PRot   = P.RotateX(cos, sin);
        //    PuRot  = Pu.RotateX(cos, sin);
        //    PvRot  = Pv.RotateX(cos, sin);
        //    NRot   = N.RotateX(cos, sin);
        //}
    }

    public static class Vector3Extensions
    {
        public static Vector3 RotateZ(this Vector3 v, float cos, float sin)
        {
            float x = v.X * cos - v.Y * sin;
            float y = v.X * sin + v.Y * cos;
            return new Vector3(x, y, v.Z);
        }

        public static Vector3 RotateX(this Vector3 v, float cos, float sin)
        {
            float y = v.Y * cos - v.Z * sin;
            float z = v.Y * sin + v.Z * cos;
            return new Vector3(v.X, y, z);
        }
    }
}
