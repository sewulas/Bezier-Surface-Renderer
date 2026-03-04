using Accessibility;
using BezierSurfacesFiller.Controller;
using BezierSurfacesFiller.Model;
using BezierSurfacesFiller.Model.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.View.Rendering
{
    public class ControlPointsRender : IRender
    {
        private const int DotSize = 6;
        private readonly Pen pen = new Pen(Color.Red, 1);
        private readonly Brush brush = new SolidBrush(Color.Yellow);
        public void Render(Graphics g, RenderContext rtx)
        {
            var cps = rtx.ControlPointsRotated;

            // kwadraciki
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    var p = cps[i, j];
                    float x = p.X;
                    float y = p.Y;

                    RectangleF r = new(
                        x - DotSize / 2f,
                        y - DotSize / 2f,
                        DotSize,
                        DotSize
                    );

                    g.FillRectangle(brush, r);
                    g.DrawRectangle(pen, r.X, r.Y, r.Width, r.Height);
                }

            // linie poziome
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 3; j++)
                {
                    var a = cps[i, j];
                    var b = cps[i, j + 1];
                    g.DrawLine(pen, a.X, a.Y, b.X, b.Y);
                }

            // linie pionowe
            for (int j = 0; j < 4; j++)
                for (int i = 0; i < 3; i++)
                {
                    var a = cps[i, j];
                    var b = cps[i + 1, j];
                    g.DrawLine(pen, a.X, a.Y, b.X, b.Y);
                }
        }
    }

    public class WireframeRender : IRender
    {
        private readonly Pen pen = new Pen(Color.LimeGreen, 1);
        public void Render(Graphics g, RenderContext rtx)
        {
            var mesh = rtx.Surface.mesh;
            if (mesh == null)
                return;

            foreach (var tri in mesh.Triangles)
            {
                var A = tri.A.PRot;
                var B = tri.B.PRot;
                var C = tri.C.PRot;

                // krawędzie trójkąta (A-B, B-C, C-A)
                g.DrawLine(pen, A.X, A.Y, B.X, B.Y);
                g.DrawLine(pen, B.X, B.Y, C.X, C.Y);
                g.DrawLine(pen, C.X, C.Y, A.X, A.Y);
            }
        }
    }

// FillTrianglesRender używający vertex-sorted scanline z inkrementalnym Edge (X,M,Z,ZM)
public class FillTrianglesRender : IRender
    {
        public void Render(Graphics g, RenderContext rtx)
        {
            var mesh = rtx.Surface.mesh;
            if (mesh == null) return;

            var rect = new Rectangle(rtx.MinX, rtx.MinY, rtx.Width, rtx.Height);
            var bmp = new ThreadSafeBitmap(rect);

            // równoległe przetwarzanie trójkątów (każdy zapisuje do bmp z per-pixel lock)
            Parallel.ForEach(mesh.Triangles, tri =>
            {
                FillTriangleVertexSorted(tri, bmp, rtx);
            });

            // jedna konwersja i rysowanie
            using var finalBmp = bmp.ToBitmap();
            g.DrawImageUnscaled(finalBmp, rtx.MinX, rtx.MinY);
        }

        private void FillTriangleVertexSorted(Triangle tri, ThreadSafeBitmap bmp, RenderContext rtx)
        {
            // prepare vertices as PointF and Z
            var v0 = tri.A.PRot; var v1 = tri.B.PRot; var v2 = tri.C.PRot;
            var verts = new (float x, float y, float z)[] {
            (v0.X, v0.Y, v0.Z),
            (v1.X, v1.Y, v1.Z),
            (v2.X, v2.Y, v2.Z)
        };

            var n = verts.Length;

            // sort indices by y ascending order
            int[] ind = Enumerable.Range(0, n).OrderBy(i => verts[i].y).ToArray();

            int ymin = (int)Math.Round(verts[ind[0]].y);
            int ymax = (int)Math.Round(verts[ind[n-1]].y);

            if (ymax < ymin) return;

            const float EPS = 1e-6f;
            List<Edge> AET = new();

            // For each scanline
            for (int y = ymin; y < ymax; y++)
            {
                // 1) for each vertex that lay on previous scanline
                for (int k = 0; k < 3; k++)
                {
                    int i = ind[k];

                    int vEnterY = (int)Math.Round(verts[i].y);
                    if (vEnterY != y) continue;

                    int ip = (i - 1 + n) % n;
                    int inx = (i + 1) % n;

                    TryAddOrRemoveEdge(verts[ip], verts[i], y, AET, EPS);
                    TryAddOrRemoveEdge(verts[inx], verts[i], y, AET, EPS);
                }

                // 2) remove finished edges (Ymax < current y)
                AET.RemoveAll(e => e.Ymax < y);

                // 3) sort AET by X
                AET.Sort((a, b) => a.X.CompareTo(b.X));

                // 4) fill pairs (0-1, 2-3,...)
                for (int e = 0; e + 1 < AET.Count; e += 2)
                {
                    var left = AET[e];
                    var right = AET[e + 1];

                    int xStart = (int)Math.Round(left.X);
                    int xEnd = (int)Math.Round(right.X);

                    // clamp to bitmap
                    DrawScanLine(bmp, xStart, xEnd, y, Color.LightBlue, rtx, tri);
                }

                // 5) advance edges by one scanline (incremental)
                foreach (var edge in AET)
                {
                    edge.X += edge.M;
                }
            }
        }

        private void DrawScanLine(ThreadSafeBitmap bitmap ,int x1, int x2, int y, Color c, RenderContext rtx, Triangle t)
        {
            if (x1 > x2)
                (x1, x2) = (x2, x1);

            for (int x = x1; x <= x2; x++)
                bitmap.PutPixelColor(x, y, t, rtx);
        }

        private void TryAddOrRemoveEdge((float x, float y, float z) P, (float x, float y, float z) Q, int scanY, List<Edge> AET, float eps)
        {
            // Edge from P -> Q (note: order important for determining entering/exiting)
            if (Math.Abs(P.y - Q.y) < eps)
                return;

            // if P.y > Q.y then edge goes downward into polygon as scan increases (enter)
            if (P.y > Q.y)
            {
                // add edge; compute its incremental state for the current scanline
                var e = new Edge(P.x, P.y, P.z, Q.x, Q.y, Q.z);
                InsertEdgeSorted(AET, e);
            }
            else // P.y <= Q.y
            {
                RemoveEdgeByEndpoints(AET, P, Q);
            }
        }

        // Insert edge maintaining sorted-by-X order
        // redundant, can just add and do sort, but it maybe more efficient to use this one still because of binarySearch
        private void InsertEdgeSorted(List<Edge> list, Edge e)
        {
            // TO DO: change comparer
            int idx = list.BinarySearch(e, Comparer<Edge>.Create((a, b) =>
            {
                int cmp = a.X.CompareTo(b.X);
                if (cmp != 0) return cmp;
                // jeśli X równe — sortuj po slope M (większa M — bardziej stroma) — to daje stabilne parowanie
                cmp = a.M.CompareTo(b.M);
                if (cmp != 0) return cmp;
                // ostatecznie po x1
                return a.x1.CompareTo(b.x1);
            }));
            if (idx < 0) idx = ~idx;
            list.Insert(idx, e);
        }

        // Remove edge matching endpoints (both orientations)
        private static bool SamePoint(float ax, float ay, float bx, float by, float eps = 1e-4f)
    => Math.Abs(ax - bx) <= eps && Math.Abs(ay - by) <= eps;

        private void RemoveEdgeByEndpoints(List<Edge> list, (float x, float y, float z) A, (float x, float y, float z) B)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var e = list[i];
                if ((SamePoint(e.x1, e.y1, A.x, A.y) && SamePoint(e.x2, e.y2, B.x, B.y)) ||
                    (SamePoint(e.x1, e.y1, B.x, B.y) && SamePoint(e.x2, e.y2, A.x, A.y)))
                {
                    list.RemoveAt(i);
                }
            }
        }

        private class Edge
        {
            // original endpoints (ordered so y1 < y2)
            public readonly float x1, y1, z1;
            public readonly float x2, y2, z2;

            public float X;    // current X at current scanline
            public float M;    // dx/dy
            public int Ymin;   // integer scanline where edge starts 
            public int Ymax;   // integer scanline where edge ends

            public Edge(float ax, float ay, float az, float bx, float by, float bz)
            {
                // ensure y1 < y2
                if (ay < by)
                {
                    x1 = ax; y1 = ay; z1 = az;
                    x2 = bx; y2 = by; z2 = bz;
                }
                else
                {
                    x1 = bx; y1 = by; z1 = bz;
                    x2 = ax; y2 = ay; z2 = az;
                }

                var dy = y2 - y1;
                M   = dy == 0 ? 0f : (x2 - x1) / dy;

                Ymin = (int)Math.Round(y1);
                Ymax = (int)Math.Round(y2);

                
                float yStart = Ymin;
                X = x1;
            }
        }
    }

}
