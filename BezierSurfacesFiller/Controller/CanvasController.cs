using BezierSurfacesFiller.Controller.Animations;
using BezierSurfacesFiller.Model;
using BezierSurfacesFiller.Model.Utilities;
using BezierSurfacesFiller.View;
using BezierSurfacesFiller.View.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Controller
{
    public class CanvasController
    {
        private readonly CanvasView _view;
        private readonly BezierSurface _surface;
        private readonly RenderSettings _settings;
        private Renderer _renderer;

        private readonly AnimationController _animation;
        private readonly LightAnimationSpiralMovement _lightAnimation;

        private List<Vector3> defaultCPs;

    public CanvasController(CanvasView view)
        {
            _view = view;
            _surface = new BezierSurface();
            _settings = new RenderSettings();
            _renderer = new Renderer();

            _animation = new AnimationController(_settings, () => _view.RefreshCanvas());
            _lightAnimation = new LightAnimationSpiralMovement(_settings);
            _animation.AddAnimation(_lightAnimation);

            var defaultRenders = new List<IRender>()
            {
                new ControlPointsRender(),
                new WireframeRender()
            };
            _renderer.InsertRenders(defaultRenders);

            _view.OnRenderScene += RenderScene;
            defaultCPs = new List<Vector3>(){
                new Vector3(-400f, -400f, 225f),
                new Vector3(-400f, -133.33333f, 112.5f),
                new Vector3(-400f,  133.33333f, -112.5f),
                new Vector3(-400f,  400f, -225f),

                new Vector3(-133.33333f, -400f, 112.5f),
                new Vector3(-133.33333f, -133.33333f, 56.25f),
                new Vector3(-133.33333f,  133.33333f, -56.25f),
                new Vector3(-133.33333f,  400f, -112.5f),

                new Vector3(133.33333f, -400f, -112.5f),
                new Vector3(133.33333f, -133.33333f, -56.25f),
                new Vector3(133.33333f,  133.33333f, 56.25f),
                new Vector3(133.33333f,  400f, 112.5f),

                new Vector3(400f, -400f, -225f),
                new Vector3(400f, -133.33333f, -112.5f),
                new Vector3(400f,  133.33333f, 112.5f),
                new Vector3(400f,  400f, 225f),
            };
            _surface.UpdateControlPoints(defaultCPs);
            _surface.UpdateMesh(_settings.Resolution);

        }

        private void RenderScene(Graphics g)
        {
            ApplyTransforms();

            var rtx = PrepareContext();
            _renderer.RenderAll(g, rtx);
        }

        private RenderContext PrepareContext()
        {
            var cpsRot = _surface.GetRotatedControlPoints(_settings.Alpha, _settings.Beta);

            var bounds = _surface.ComputeBounds(cpsRot);

            var rtx = new RenderContext
            {
                Surface = _surface,
                ControlPointsRotated = cpsRot,
                Settings = _settings,
                MinX = bounds.X,
                MinY = bounds.Y,
                Width = bounds.Width,
                Height = bounds.Height
            };

            return rtx;
        }

        private void ApplyTransforms()
        {
            _surface.mesh.ApplyRotations(_settings.Alpha, _settings.Beta);
        }

        public void UpdateAlpha(float alpha)
        {
            _settings.Alpha = alpha;
            _view.RefreshCanvas();
        }

        public void UpdateBeta(float beta)
        {
            _settings.Beta = beta;
            _view.RefreshCanvas();
        }

        public void UpdateResolution(int res)
        {
            _settings.Resolution = res;
            _surface.UpdateMesh(res);
            _view.RefreshCanvas();
        }

        public void UpdateKs(float ks)
        {
            _settings.ks = ks;
            _view.RefreshCanvas();
        }
        public void UpdateKd(float kd)
        {
            _settings.kd = kd;
            _view.RefreshCanvas();
        }
        public void UpdateM(int m)
        {
            _settings.m = m;
            _view.RefreshCanvas();
        }

        public void OnRenderModeChanged(string key, bool enabled)
        {
            if (enabled)
                _renderer.AddRender(RenderFactory.Create(key));
            else
                _renderer.RemoveRender(RenderFactory.Create(key));

            _view.RefreshCanvas();
        }

        public void AddRenderingMethod(IRender render)
        {
            _renderer.AddRender(render);
        }

        public void RemoveRenderingMethod(IRender render) 
        {
            _renderer.RemoveRender(render);
        }

        public void UpdateSurfaceColor(Color color)
        {
            _settings.SurfaceColor = color;
            _view.RefreshCanvas();
        }

        public void UpdateSolidFill(bool check)
        {
            if (check)
                _settings.IsTextureOptionOn = false;
            _view.RefreshCanvas();
        }

        public void UpdateTextureFill(bool check)
        {
            if (check)
                _settings.IsTextureOptionOn = true;
            else
                _settings.IsTextureOptionOn = false;
            _view.RefreshCanvas();
        }

        public void UpdateTexture(Bitmap bitmap)
        {
            _settings.Texture = new TextureBuffer(bitmap);
            _view.RefreshCanvas();
        }

        internal void UpdateNormalMapUsage(bool check)
        {
            _settings.IsMapOptionOn = check;
            _view.RefreshCanvas();
        }

        internal void UpdateNormalMap(Bitmap bitmap)
        {
            _settings.Map = new TextureBuffer(bitmap);
            _view.RefreshCanvas();
        }

        internal void UpdateControlPoints(List<Vector3> list)
        {
            _surface.UpdateControlPoints(list);
            _surface.UpdateMesh(_settings.Resolution);
            _view.RefreshCanvas();
        }

        internal void UpdateLightColor(Color color)
        {
            _settings.LightColor = color;
            _view.RefreshCanvas();
        }

        internal void UpdateLightZPosition(int z)
        { 
            var pos = _settings.LightSourceCordinates;
            _settings.LightSourceCordinates = new Vector3(pos.X, pos.Y, z);
            _view.RefreshCanvas();
        }

        internal void UpdateLightAnimation(bool IsAnimationOn)
        {
            _lightAnimation.Enabled = IsAnimationOn;
        }
    }
}
