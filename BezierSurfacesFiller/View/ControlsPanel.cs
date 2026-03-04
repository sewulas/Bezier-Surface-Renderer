using BezierSurfacesFiller.Model.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace BezierSurfacesFiller.View
{
    public partial class ControlsPanel : UserControl
    {
        public event Action<List<Vector3>>? LoadedCP;

        public event Action<float>? AlphaChanged;
        public event Action<float>? BetaChanged;
        public event Action<int>? ResolutionChanged;

        public event Action<float>? KdChanged;
        public event Action<float>? KsChanged;
        public event Action<int>? MChanged;
        public event Action<Color>? LightColorChanged;
        public event Action<int>? LightZPositionChanged;
        public event Action<bool>? LightAnimationClicked;
        private bool IsAnimationOn = false;

        public event Action<string, bool>? RenderModeChanged;

        public event Action<Color>? SurfaceColorChanged;

        public event Action<bool>? SolidFillChanged;
        public event Action<bool>? TextureFillChanged;
        public event Action<Bitmap>? TextureLoaded;

        public event Action<bool>? NormalMapChanged;
        public event Action<Bitmap>? NormalMapLoaded;

        // kontrolki
        // wczytanie CP
        private Button bttnLoadCP;

        private TrackBar trackBarAlpha;
        private TrackBar trackBarBeta;
        private TrackBar trackBarResolution;

        private Label labelAlpha;
        private Label labelBeta;
        private Label labelResolution;

        private Label labelRenderTitle;
        private CheckBox checkBoxCP;
        private CheckBox checkBoxWireframe;
        private CheckBox checkBoxFill;

        // lighting
        private TrackBar trackBarKd;
        private Label labelKd;
        private TrackBar trackBarKs;
        private Label labelKs;
        private TrackBar trackBarM;
        private Label labelM;
        private Button bttnChangeLightColor;
        private TrackBar trackBarZPosition;
        private Label labelZPosition;
        private Button bttnAnimateLightSource;

        // texture/solid
        private Label labelTitle;
        private RadioButton RadioBttnSolidFill;
        private RadioButton RadioBttnTextureFill;
        private Button bttnLoadTexture;

        // mapa wektorów normalnych
        private Label labelNormal;
        private CheckBox checkBoxNormalMap;
        private Button bttnLoadNormalMap;

        // coloring
        private Button bttnChangeSurfaceColor;

        public ControlsPanel()
        {
            InitializeComponent();

            Dock = DockStyle.Right;
            Width = 220;
            BackColor = Color.FromArgb(40, 40, 45);
            Padding = new Padding(10);
            AutoScroll = true;

            InitTrackBars();
        }

        private void InitTrackBars()
        {
            bttnLoadCP = new Button()
            {
                Text = "Załaduj punkty kontrolne",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30,
                Width = 50
            };

            // label – kąt α
            labelAlpha = new Label()
            {
                Text = "Kąt α = 0°:",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 20
            };

            trackBarAlpha = new TrackBar()
            {
                Minimum = -90,
                Maximum = 90,
                TickFrequency = 10,
                Value = 0,
                Dock = DockStyle.Top,
                Height = 45
            };

            // label – kąt β
            labelBeta = new Label()
            {
                Text = "Kąt β = 0°:",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 20
            };

            trackBarBeta = new TrackBar()
            {
                Minimum = -90,
                Maximum = 90,
                TickFrequency = 10,
                Value = 0,
                Dock = DockStyle.Top,
                Height = 45
            };

            // label – rozdzielczość
            labelResolution = new Label()
            {
                Text = "Rozdzielczość: 10",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 20
            };

            trackBarResolution = new TrackBar()
            {
                Minimum = 2,
                Maximum = 100,
                TickFrequency = 2,
                Value = 10,
                Dock = DockStyle.Top,
                Height = 45
            };

            // lighting
            labelKd = new Label()
            {
                Text = "kd = 50%",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 20
            };

            trackBarKd = new TrackBar()
            {
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 5,
                Value = 50,
                Dock = DockStyle.Top,
                Height = 45
            };

            labelKs = new Label()
            {
                Text = "ks = 50%",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 20
            };

            trackBarKs = new TrackBar()
            {
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 5,
                Value = 50,
                Dock = DockStyle.Top,
                Height = 45
            };

            labelM = new Label()
            {
                Text = "m = 1",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 20
            };

            trackBarM = new TrackBar()
            {
                Minimum = 1,
                Maximum = 100,
                TickFrequency = 5,
                Value = 1,
                Dock = DockStyle.Top,
                Height = 45
            };

            bttnChangeLightColor = new Button()
            {
                Text = "Wybierz kolor światła",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30,
                Width = 50
            };

            labelZPosition = new Label()
            {
                Text = "'z' światła = 400",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 20
            };

            trackBarZPosition = new TrackBar()
            {
                Minimum = 100,
                Maximum = 2000,
                TickFrequency = 100,
                Value = 400,
                Dock = DockStyle.Top,
                Height = 45
            };

            bttnAnimateLightSource = new Button()
            {
                Text = "WŁĄCZ animację światła",
                ForeColor = Color.Green,
                Dock = DockStyle.Top,
                Height = 30,
                Width = 50
            };

            // rendering
            labelRenderTitle = new Label()
            {
                Text= "Rysowane obiekty na scenie:",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30
            };

            checkBoxCP = new CheckBox()
            {
                Text = "Wielobok Beziera",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25,
                Checked = true
            };

            checkBoxWireframe = new CheckBox()
            {
                Text = "Siatka trójkątów",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25,
                Checked = true
            };

            checkBoxFill = new CheckBox()
            {
                Text = "Wypełnione trójkąty",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25
            };

            // texture/solid
            labelTitle = new Label()
            {
                Text = "Wybór wypełnienia:",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0,15,0,0)
            };

            RadioBttnSolidFill = new RadioButton()
            {
                Text = "Solid",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25,
                Checked = true
            };

            RadioBttnTextureFill = new RadioButton()
            {
                Text = "Tekstura",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25,
                Checked = false
            };

            bttnChangeSurfaceColor = new Button()
            {
                Text = "Wybierz kolor powierzchni",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30,
                Width = 50
            };

            bttnLoadTexture = new Button()
            {
                Text = "Załaduj teksturę",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30,
                Width = 50
            };

            // wektor map
            labelNormal = new Label()
            {
                Text = "Użycie mapy wektorów normalnych:",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 15, 0, 0)
            };

            checkBoxNormalMap = new CheckBox()
            {
                Text = "Użyj mapy wektorów",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25
            };

            bttnLoadNormalMap = new Button()
            {
                Text = "Załaduj mapę wektorów",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30,
                Width = 50
            };

            //button CP
            bttnLoadCP.Click += OnLoadCP_CLick;

            checkBoxCP.Tag = "ControlPolygon";
            checkBoxCP.CheckedChanged += OnRenderModeCheckChanged;

            checkBoxWireframe.Tag = "Wireframe";
            checkBoxWireframe.CheckedChanged += OnRenderModeCheckChanged;

            checkBoxFill.Tag = "FillTriangles";
            checkBoxFill.CheckedChanged += OnRenderModeCheckChanged;

            // podpięcie zdarzeń
            trackBarAlpha.ValueChanged += trackBarAlpha_ValueChanged;

            trackBarBeta.ValueChanged += trackBarBeta_ValueChanged;

            trackBarResolution.ValueChanged += trackBarResolution_ValueChanged;

            // lighting events
            trackBarKd.ValueChanged += trackBarKd_ValueChanged;
            trackBarKs.ValueChanged += trackBarKs_ValueChanged;
            trackBarM.ValueChanged += trackBarM_ValueChanged;
            bttnChangeLightColor.Click += OnChangeLightColor_Click;
            trackBarZPosition.ValueChanged += trackbarZPosition_ValueChanged;
            bttnAnimateLightSource.Click += OnAnimateLightSource_Click;

            // solid/texture
            RadioBttnSolidFill.CheckedChanged += OnSolidFillChanged;
            RadioBttnTextureFill.CheckedChanged += OnTextureFillChanged;

            // button click
            bttnChangeSurfaceColor.Click += btnChangeSurfaceColor_Click;
            bttnLoadTexture.Click += bttnLoadTexture_Click;

            // Normal Map
            checkBoxNormalMap.CheckedChanged += OnNormalMapCheckChanged;
            bttnLoadNormalMap.Click += bttnLoadNormalMap_Click;

            // dodanie w odwrotnej kolejności (ostatni na górze)

            // Normal map
            Controls.Add(bttnLoadNormalMap);
            Controls.Add(checkBoxNormalMap);
            Controls.Add(labelNormal);
            // button surface
            Controls.Add(bttnLoadTexture);
            Controls.Add(bttnChangeSurfaceColor);
            // texture/solid
            Controls.Add(RadioBttnTextureFill);
            Controls.Add(RadioBttnSolidFill);
            Controls.Add(labelTitle);
            // opcje renderowania
            Controls.Add(checkBoxFill);
            Controls.Add(checkBoxWireframe);
            Controls.Add(checkBoxCP);
            Controls.Add(labelRenderTitle);
            // światło
            Controls.Add(bttnAnimateLightSource);
            Controls.Add(trackBarZPosition);
            Controls.Add(labelZPosition);
            Controls.Add(bttnChangeLightColor);
            // m value
            Controls.Add(trackBarM);
            Controls.Add(labelM);
            // ks value
            Controls.Add(trackBarKs);
            Controls.Add(labelKs);
            // kd value
            Controls.Add(trackBarKd);
            Controls.Add(labelKd);
            // resolution
            Controls.Add(trackBarResolution);
            Controls.Add(labelResolution);
            // beta
            Controls.Add(trackBarBeta);
            Controls.Add(labelBeta);
            // alpha
            Controls.Add(trackBarAlpha);
            Controls.Add(labelAlpha);
            // button CP
            Controls.Add(bttnLoadCP);
        }

        private void OnAnimateLightSource_Click(object? sender, EventArgs e)
        {
            if (!IsAnimationOn)
            {
                IsAnimationOn = true;
                bttnAnimateLightSource.Text = "WYŁĄCZ animację światła";
                bttnAnimateLightSource.ForeColor = Color.Red;
                LightAnimationClicked?.Invoke(IsAnimationOn);
            }
            else
            {
                IsAnimationOn = false;
                bttnAnimateLightSource.Text = "WŁĄCZ animację światła";
                bttnAnimateLightSource.ForeColor = Color.Green;
                LightAnimationClicked?.Invoke(IsAnimationOn);
            }
        }

        private void trackbarZPosition_ValueChanged(object? sender, EventArgs e)
        {
            labelZPosition.Text = $"'z' światła = {trackBarZPosition.Value}";
            LightZPositionChanged?.Invoke(trackBarZPosition.Value);
        }

        private void OnChangeLightColor_Click(object? sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.AllowFullOpen = true;
                colorDialog.AnyColor = true;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    LightColorChanged?.Invoke(colorDialog.Color);
                }
            }
        }

        private void OnLoadCP_CLick(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog();
            dialog.Title = "Wczytaj punkty kontrolne";
            dialog.Filter = "Plik tekstowy (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
            dialog.InitialDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                var lines = File.ReadAllLines(dialog.FileName);
                var cps = new List<Vector3>();

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 3)
                        throw new Exception($"Niepoprawny format linii: \"{line}\"");

                    float x = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);

                    cps.Add(new Vector3(x, y, z));
                }

                // wymagana liczba punktów: dokładnie 16
                if (cps.Count != 16)
                    throw new Exception($"Liczba punktów kontrolnych musi wynosić dokładnie 16, a wczytano: {cps.Count}");

                // przekazanie do controllera
                LoadedCP?.Invoke(cps);

                MessageBox.Show("Wczytano punkty kontrolne.", "Sukces",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wczytywania: " + ex.Message, "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bttnLoadNormalMap_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog();
            dialog.Filter = "Obrazy (*.png;*.jpg)|*.png;*.jpg";
            dialog.InitialDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap bmp = new Bitmap(dialog.FileName);

                    NormalMapLoaded?.Invoke(bmp);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nie udało się załadować mapy:\n" + ex.Message);
                }
            }
        }

        private void OnNormalMapCheckChanged(object? sender, EventArgs e)
        {
            NormalMapChanged?.Invoke(checkBoxNormalMap.Checked);
        }

        private void bttnLoadTexture_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Wybierz plik tekstury";
            dialog.Filter = "Obrazy (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            dialog.InitialDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap bmp = new Bitmap(dialog.FileName);

                    TextureLoaded?.Invoke(bmp);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nie udało się załadować tekstury:\n" + ex.Message);
                }
            }
        }

        private void OnTextureFillChanged(object? sender, EventArgs e)
        {
            var btn = (RadioButton)sender!;
            TextureFillChanged?.Invoke(btn.Checked);
        }

        private void OnSolidFillChanged(object? sender, EventArgs e)
        {
            var btn = (RadioButton)sender!;
            SolidFillChanged?.Invoke(btn.Checked);
        }

        private void btnChangeSurfaceColor_Click(object? sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.AllowFullOpen = true;
                colorDialog.AnyColor = true;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    SurfaceColorChanged?.Invoke(colorDialog.Color);
                }
            }
        }

        private void trackBarM_ValueChanged(object? sender, EventArgs e)
        {
            labelM.Text = $"m = {trackBarM.Value}";
            MChanged?.Invoke(trackBarM.Value);
        }

        private void trackBarKs_ValueChanged(object? sender, EventArgs e)
        {
            var ks = trackBarKs.Value / 100f;
            labelKs.Text = $"ks = {trackBarKs.Value}%";
            KsChanged?.Invoke(ks);
        }

        private void trackBarKd_ValueChanged(object? sender, EventArgs e)
        {
            var kd = trackBarKd.Value/100f;
            labelKd.Text = $"kd = {trackBarKd.Value}%";
            KdChanged?.Invoke(kd);
        }

        private void trackBarAlpha_ValueChanged(object sender, EventArgs e)
        {
            labelAlpha.Text = $"Kąt α = {trackBarAlpha.Value}°:";
            AlphaChanged?.Invoke(trackBarAlpha.Value);
        }

        private void trackBarBeta_ValueChanged(object sender, EventArgs e)
        {
            labelBeta.Text = $"Kąt β = {trackBarBeta.Value}°:";
            BetaChanged?.Invoke(trackBarBeta.Value);
        }

        private void trackBarResolution_ValueChanged(object sender, EventArgs e)
        {
            labelResolution.Text = $"Rozdzielczość: {trackBarResolution.Value}";
            ResolutionChanged?.Invoke(trackBarResolution.Value);
        }

        private void OnRenderModeCheckChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox cb && cb.Tag is string key)
                RenderModeChanged?.Invoke(key, cb.Checked);
        }

    }
}
