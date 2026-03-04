using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BezierSurfacesFiller.View
{
    public partial class CanvasView : UserControl
    {
        public event Action<Graphics>? OnRenderScene; // zdarzenie: "narysuj scenę"

        public CanvasView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Ustawienie środka canvasa jako (0,0)
            g.TranslateTransform(Width / 2f, Height / 2f);
            g.ScaleTransform(1, -1);

            OnRenderScene?.Invoke(g); // wywołanie za pomocą controllera odpowiednich funkcji renderujących canvas
        }

        public void RefreshCanvas()
        {
            Invalidate();
        }
    }
}
