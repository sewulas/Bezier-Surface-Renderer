using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Model
{
    public class BezierSurface
    {
        private int _size = 4;
        private int _deg = 3;
        public Vector3[,] V { get; set; } = new Vector3[4, 4];
        public Mesh mesh = new Mesh();


        public void UpdateControlPoints(List<Vector3> v)
        {
            if (v.Count!=_size*_size)
            {
                throw new Exception();
            }
            for (int i = 0; i < v.Count; i++)
            {
                V[i/_size, i%_size] = v[i];
            }
        }

        public Vector3 Evaluate(float u, float v)
        {
            Vector3 p = Vector3.Zero;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    p += V[i, j] * (B(i, 3, u) * B(j, 3, v));
            return p;
        }

        private float B(int i, int n, float t)
        {
            return Binomial(n, i) * MathF.Pow(1 - t, n - i) * MathF.Pow(t, i);
        }

        private int Binomial(int n, int k)
        {
            if (k == 0 || k == n) return 1;
            int res = 1;
            for (int i = 1; i <= k; i++)
                res = res * (n - (k - i)) / i;
            return res;
        }

        public Vector3 EvaluatePu(float u, float v)
        {
            Vector3 p = Vector3.Zero;
            int n = _deg;

            for (int i = 0; i <= n-1; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    p += (V[i + 1, j] - V[i, j]) * (B(i,n-1,u) * B(j,n,v));
                }
            }
            return p*n;
        }

        public Vector3 EvaluatePv(float u, float v)
        {
            Vector3 p = Vector3.Zero;
            int n = _deg;

            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j <= n - 1; j++)
                {
                    p += (V[i, j + 1] - V[i, j]) * (B(i, n, u) * B(j, n-1, v));
                }
            }
            return p * n;
        }

        private Vertex GenerateVertexOnMesh(float u, float v)
        {
            Vertex vert = new Vertex(u,v);

            vert.P  = Evaluate(u,v);
            vert.Pu = EvaluatePu(u,v); // Pu
            vert.Pv = EvaluatePv(u,v); // Pv
            vert.N  = Vector3.Normalize(Vector3.Cross(vert.Pu, vert.Pv)); // N

            return vert;
        }

        public void UpdateMesh(int resolution)
        {
            this.mesh = new Mesh(GenerateMesh(resolution));
        }

        // Generowanie siatki trójkątów
        public List<Triangle> GenerateMesh(int resolution)
        {
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    float u0 = i / (float)resolution;
                    float v0 = j / (float)resolution;
                    float u1 = (i + 1) / (float)resolution;
                    float v1 = (j + 1) / (float)resolution;

                    Vertex v00 = GenerateVertexOnMesh(u0, v0);
                    Vertex v10 = GenerateVertexOnMesh(u1, v0); 
                    Vertex v01 = GenerateVertexOnMesh(u0, v1);
                    Vertex v11 = GenerateVertexOnMesh(u1, v1);

                    // dwa trójkąty
                    triangles.Add(new Triangle(v00, v10, v11));
                    triangles.Add(new Triangle(v00, v11, v01));
                }
            }

            return triangles;
        }

        public Vector3[,] GetRotatedControlPoints(float alpha, float beta)
        {
            float a = alpha * MathF.PI / 180f;
            float b = beta * MathF.PI / 180f;

            float cosA = MathF.Cos(a);
            float sinA = MathF.Sin(a);
            float cosB = MathF.Cos(b);
            float sinB = MathF.Sin(b);

            var result = new Vector3[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 p = V[i, j];

                    // RZ
                    p = p.RotateZ(cosA, sinA);

                    // RX
                    p = p.RotateX(cosB, sinB);

                    result[i, j] = p;
                }
            }

            return result;
        }

        public Rectangle ComputeBounds(Vector3[,] cpsRot)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    int x = (int)Math.Round(cpsRot[i, j].X);
                    int y = (int)Math.Round(cpsRot[i, j].Y);

                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }

            return new Rectangle(
                minX,
                minY,
                (maxX - minX + 1),
                (maxY - minY + 1)
            );
        }
    }
}
