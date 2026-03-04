using BezierSurfacesFiller.Controller;
using BezierSurfacesFiller.View;

namespace BezierSurfacesFiller
{
    public partial class Form1 : Form
    {
        private CanvasController _controller;
        public Form1()
        {
            InitializeComponent();
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            var controls = new ControlsPanel() { Dock = DockStyle.Right, Width = 220 };
            var canvas = new CanvasView() { Dock = DockStyle.Fill, BackColor = Color.Black };

            Controls.Add(canvas);
            Controls.Add(controls);

            _controller = new CanvasController(canvas);

            controls.LoadedCP += _controller.UpdateControlPoints;

            controls.AlphaChanged += _controller.UpdateAlpha;
            controls.BetaChanged += _controller.UpdateBeta;
            controls.ResolutionChanged += _controller.UpdateResolution;
            controls.RenderModeChanged += _controller.OnRenderModeChanged;

            controls.KdChanged += _controller.UpdateKd;
            controls.KsChanged += _controller.UpdateKs;
            controls.MChanged += _controller.UpdateM;
            controls.LightColorChanged += _controller.UpdateLightColor;
            controls.LightZPositionChanged += _controller.UpdateLightZPosition;
            controls.LightAnimationClicked += _controller.UpdateLightAnimation;

            controls.SurfaceColorChanged += _controller.UpdateSurfaceColor;

            controls.SolidFillChanged += _controller.UpdateSolidFill;
            controls.TextureFillChanged += _controller.UpdateTextureFill;
            controls.TextureLoaded += _controller.UpdateTexture;

            controls.NormalMapChanged += _controller.UpdateNormalMapUsage;
            controls.NormalMapLoaded += _controller.UpdateNormalMap;
        }
    }
}
