using BezierSurfacesFiller.View.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Model.Utilities
{

    public class ThreadSafeBitmap
    {
        private readonly int minX;
        private readonly int minY;
        private readonly int width;
        private readonly int height;

        private readonly Color[] pixels;   // kolor ARGB
        private readonly float[] depth;    // Z-buffer
        private readonly object[] locks;   // lock per pixel

        public ThreadSafeBitmap(Rectangle r)
        {
            minX = r.X;
            minY = r.Y;
            width = r.Width;
            height = r.Height;

            int size = width * height;

            pixels = new Color[size];
            depth = new float[size];
            locks = new object[size];

            for (int i = 0; i < size; i++)
            {
                pixels[i] = Color.Black;
                depth[i] = float.NegativeInfinity;   // Z-buffer: im większe Z tym bliżej
                locks[i] = new object();
            }
        }

        private int Index(int x, int y)
        {
            return (x - minX) + (y - minY) * width;
        }

        public void PutPixel(int x, int y, float z, Color c)
        {
            int i = Index(x, y);

            lock (locks[i])
            {
                if (z >= depth[i])   // bliżej ekranu
                {
                    depth[i] = z;
                    pixels[i] = c;
                }
            }
        }

        public void PutPixelColor(int x, int y, Triangle t, RenderContext rtx)
        {
            // if solid fill is turned off but there is no texture loaded, do not display
            if (rtx.Settings.IsTextureOptionOn && !rtx.Settings.IsTextureLoaded())
                return;
            var LSPosition = rtx.Settings.LightSourceCordinates; // default = new Vector3(0,0,300);

            var kd = rtx.Settings.kd;
            var ks = rtx.Settings.ks;
            var m = rtx.Settings.m;
            var LightColor = rtx.Settings.LightColor;
            var SurfaceColor = rtx.Settings.SurfaceColor;
            Vector3 Il = new Vector3() // RGB
            {
                X = LightColor.R / 255f,
                Y = LightColor.G / 255f,
                Z = LightColor.B / 255f,
            };

            Vector3 Io = new Vector3() // RGB
            {
                X = SurfaceColor.R / 255f,
                Y = SurfaceColor.G / 255f,
                Z = SurfaceColor.B / 255f,
            };

            // współrzędne barycentryczne:
            Vector3 P = new Vector3(x, y, 0);
            var triangleArea = Area(t.A.PRot, t.B.PRot, t.C.PRot);
            var alpha = Area(P, t.B.PRot, t.C.PRot) / triangleArea;
            var beta = Area(t.A.PRot, P, t.C.PRot) / triangleArea;
            var gamma = Area(t.A.PRot, t.B.PRot, P) / triangleArea;

            // interpolacja z
            var zp = alpha * t.A.PRot.Z + beta * t.B.PRot.Z + gamma * t.C.PRot.Z;

            // interpolacja wektora normalnego N
            var N = alpha * t.A.NRot + beta * t.B.NRot + gamma * t.C.NRot;
            N = Vector3.Normalize(N);

            // interpolacja u i v, jeżeli jest włączona tekstura lub mapa wektorów normalnych
            float u;
            float v;
            if (rtx.Settings.IsTextureOptionOn || rtx.Settings.IsMapOptionOn)
            {
                u = alpha * t.A.U + beta * t.B.U + gamma * t.C.U;
                v = alpha * t.A.V + beta * t.B.V + gamma * t.C.V;
                u = Math.Clamp(u, 0f, 1f);
                v = Math.Clamp(v, 0f, 1f);

                // obliczenie koloru obiektu Io na podstawie wczytanej tekstury
                if (rtx.Settings.IsTextureLoaded() && rtx.Settings.IsTextureOptionOn)
                {
                    var texture = rtx.Settings.Texture!;

                    int w = texture.Width;
                    int h = texture.Height;

                    // zakładamy u,v już w [0,1]
                    // mapujemy na zakres indeksów 0..w-1 / 0..h-1
                    int tx = (int)(u * (w));
                    int ty = (int)((1 - v) * (h)); // 1-v

                    tx = Math.Clamp(tx, 0, w - 1);
                    ty = Math.Clamp(ty, 0, h - 1);

                    Color c = texture.GetPixel(tx, ty);
                    Io = new Vector3(
                        c.R / 255f,
                        c.G / 255f,
                        c.B / 255f);
                }

                if (rtx.Settings.IsMapOptionOn && rtx.Settings.IsMapLoaded())
                {
                    var map = rtx.Settings.Map!;
                    int w = map.Width;
                    int h = map.Height;

                    // zakładamy u,v już w [0,1]
                    // mapujemy na zakres indeksów 0..w-1 / 0..h-1
                    int tx = (int)(u * (w));
                    int ty = (int)(v * (h)); // 1-v

                    tx = Math.Clamp(tx, 0, w - 1);
                    ty = Math.Clamp(ty, 0, h - 1);
                    Color c = map.GetPixel(tx, ty);
                    float nx = (c.R / 255f) * 2f - 1f;  // -1..+1
                    float ny = (c.G / 255f) * 2f - 1f;  // -1..+1
                    float nz = (c.B / 255f) * 2f - 1f;  // -1..+1
                    nz = Math.Abs(nz);

                    var N_texture = new Vector3()
                    {
                        X = nx,
                        Y = ny,
                        Z = nz,
                    };
                    N_texture = Vector3.Normalize(N_texture);

                    var Pu = alpha * t.A.PuRot + beta * t.B.PuRot + gamma * t.C.PuRot;
                    var Pv = alpha * t.A.PvRot + beta * t.B.PvRot + gamma * t.C.PvRot;
                    Pu = Vector3.Normalize(Pu);
                    Pv = Vector3.Normalize(Pv);

                    Matrix3x3 M = new Matrix3x3(Pu, Pv, N);
                    N = M * N_texture;
                    N = Vector3.Normalize(N);

                }
            }

            // wersor do światła L
            var L = LSPosition - P;
            L = Vector3.Normalize(L);

            // cos(<N,L>)
            var cosNL = Math.Max(0, Vector3.Dot(N, L));

            // wartości V i R:
            Vector3 V = new Vector3(0, 0, 1);
            var R = 2 * Vector3.Dot(N, L) * N - L;
            R = Vector3.Normalize(R);

            // cos(<V,R>)
            var cosVR = Math.Max(0, Vector3.Dot(V, R));
            var cosmVR = (float)Math.Pow(cosVR, m);

            // składowa zwierciadła I: 
            var I = kd * Il * Io * cosNL + ks * Il * Io * (float)Math.Pow(cosVR, m);

            I.X = Math.Clamp(I.X, 0f, 1f);
            I.Y = Math.Clamp(I.Y, 0f, 1f);
            I.Z = Math.Clamp(I.Z, 0f, 1f);

            // if I is not possible to be calculated, do not render it
            if (I.X is float.NaN || I.Y is float.NaN || I.Z is float.NaN)
                return;

            Color pixelColor = Color.FromArgb(
                (int)Math.Round(I.X * 255),
                (int)Math.Round(I.Y * 255),
                (int)Math.Round(I.Z * 255)
            );

            this.PutPixel(x, y, zp, pixelColor);
        }

        // konwersja na normalną bitmapę
        public Bitmap ToBitmap()
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, width, height);
            var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr ptr = data.Scan0;
                var bytesPerPixel = sizeof(byte) * 4; // R + G + B + A
                byte[] pixelData = new byte[width * height * bytesPerPixel];

                for (int i = 0; i < pixels.Length; i++)
                {
                    var idx = i * bytesPerPixel;
                    pixelData[idx] = pixels[i].B;
                    pixelData[idx + 1] = pixels[i].G;
                    pixelData[idx + 2] = pixels[i].R;
                    pixelData[idx + 3] = pixels[i].A;
                }

                Marshal.Copy(pixelData, 0, ptr, pixelData.Length);
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            return bmp;
        }
        private float Area(Vector3 A, Vector3 B, Vector3 C)
        {
            return (B.X - A.X) * (C.Y - A.Y) - (B.Y - A.Y) * (C.X - A.X);
        }
    }

    public class TextureBuffer
    {
        public readonly int Width;
        public readonly int Height;
        public readonly Color[] Pixels;

        public TextureBuffer(Bitmap bmp)
        {
            Width = bmp.Width;
            Height = bmp.Height;

            Pixels = new Color[Width * Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Pixels[x + y * Width] = bmp.GetPixel(x, y);
                }
            }
        }

        public Color GetPixel(int x, int y)
            => Pixels[x + y * Width];
    }
}
