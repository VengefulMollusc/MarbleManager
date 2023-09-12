using MarbleManager.Config;
using MarbleManager.Colours;
using PaletteSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MarbleManager.Lights;

namespace MarbleManager
{
    public partial class ConfigForm : Form
    {
        Bitmap wallpaper;
        GlobalLightController lightController;

        public ConfigForm()
        {
            InitializeComponent();
            RedirectConsoleOutput();

            // init config
            LoadConfig();
            LoadLastPalette();
        }

        internal void SetLightController(GlobalLightController _lightController)
        {
            lightController = _lightController;
        }

        private void RedirectConsoleOutput ()
        {
            // Create a TextWriter to redirect standard output to the TextBox control
            TextWriter writer = new TextBoxStreamWriter(txtConsole);

            // Redirect the standard output
            Console.SetOut(writer);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (wallpaper != null)
            {
                wallpaper.Dispose();
                WallpaperManager.DeleteCopiedWallpaper();
            }
            if (lightController != null)
            {
                lightController = null;
            }
            base.OnClosing(e);
        }

        private void PreviewPalette (PaletteObject palette, bool isPreview = true)
        {
            if (palette == null) { return; }

            if (isPreview)
            {
                ApplySwatch(paletteCurrentD, new List<Label>() { labelCurrentDPop, labelCurrentDh, labelCurrentDs, labelCurrentDl }, palette.dominant);
                ApplySwatch(paletteCurrentV, new List<Label>() { labelCurrentVPop, labelCurrentVh, labelCurrentVs, labelCurrentVl }, palette.vibrant);
                ApplySwatch(paletteCurrentVl, new List<Label>() { labelCurrentVlPop, labelCurrentVlh, labelCurrentVls, labelCurrentVll }, palette.lightVibrant);
                ApplySwatch(paletteCurrentVd, new List<Label>() { labelCurrentVdPop, labelCurrentVdh, labelCurrentVds, labelCurrentVdl }, palette.darkVibrant);
                ApplySwatch(paletteCurrentM, new List<Label>() { labelCurrentMPop, labelCurrentMh, labelCurrentMs, labelCurrentMl }, palette.muted);
                ApplySwatch(paletteCurrentMl, new List<Label>() { labelCurrentMlPop, labelCurrentMlh, labelCurrentMls, labelCurrentMll }, palette.lightMuted);
                ApplySwatch(paletteCurrentMd, new List<Label>() { labelCurrentMdPop, labelCurrentMdh, labelCurrentMds, labelCurrentMdl }, palette.darkMuted);
            }
            else
            {
                ApplySwatch(paletteLastD, new List<Label>() { labelLastDPop, labelLastDh, labelLastDs, labelLastDl }, palette.dominant);
                ApplySwatch(paletteLastV, new List<Label>() { labelLastVPop, labelLastVh, labelLastVs, labelLastVl }, palette.vibrant);
                ApplySwatch(paletteLastVl, new List<Label>() { labelLastVlPop, labelLastVlh, labelLastVls, labelLastVll }, palette.lightVibrant);
                ApplySwatch(paletteLastVd, new List<Label>() { labelLastVdPop, labelLastVdh, labelLastVds, labelLastVdl }, palette.darkVibrant);
                ApplySwatch(paletteLastM, new List<Label>() { labelLastMPop, labelLastMh, labelLastMs, labelLastMl }, palette.muted);
                ApplySwatch(paletteLastMl, new List<Label>() { labelLastMlPop, labelLastMlh, labelLastMls, labelLastMll }, palette.lightMuted);
                ApplySwatch(paletteLastMd, new List<Label>() { labelLastMdPop, labelLastMdh, labelLastMds, labelLastMdl }, palette.darkMuted);
            }
        }

        private void ApplySwatch (Panel panel, List<Label> labels, SwatchObject swatch)
        {
            if (swatch == null)
            {
                panel.BackColor = Color.WhiteSmoke;
                panel.BorderStyle = BorderStyle.FixedSingle;
                labels[0].Text = "no swatch";
                labels[1].Text = "0";
                labels[2].Text = "0";
                labels[3].Text = "0";
                return;
            }

            // apply colour if not null
            panel.BackColor = Color.FromArgb(swatch.r, swatch.g, swatch.b);
            panel.BorderStyle = BorderStyle.None;
            labels[0].Text = swatch.population.ToString();
            labels[1].Text = Math.Round(swatch.h, 3).ToString();
            labels[2].Text = Math.Round(swatch.s, 3).ToString();
            labels[3].Text = Math.Round(swatch.l, 3).ToString();
        }

        private void PreviewCurrentWallpaper()
        {
            // cleanup existing value to stop RAM use increasing
            if (wallpaper != null) { wallpaper.Dispose(); }

            Bitmap currentWallpaper = WallpaperManager.GetWallpaperJpg();
            if (currentWallpaper != null)
            {
                wallpaper = currentWallpaper;
                pictureBoxWallpaper.Image = wallpaper;
            }
            Console.WriteLine("Loaded current wallpaper");
        }

        private void LoadConfig()
        {
            // Load config
            ConfigObject config = ConfigManager.GetConfig();

            if (config == null)
            {
                Console.WriteLine("Config load error - null object returned");
                return;
            }

            // init general config
            checkBoxSyncOnWallpaperChange.Checked = config.generalConfig.syncOnWallpaperChange;
            checkBoxAutoTurnOnOff.Checked = config.generalConfig.turnOnOffWithPc;

            // init nanoleaf config
            textBoxNanoleafIP.Text = config.nanoleafConfig.ipAddress;
            textBoxNanoleafApiKey.Text = config.nanoleafConfig.apiKey;
            switch (config.nanoleafConfig.effect)
            {
                case NanoleafEffect.Highlight:
                    radioButtonLightEffectHighlight.Checked = true;
                    break;
                case NanoleafEffect.Random:
                default:
                    radioButtonLightEffectRandom.Checked = true;
                    break;
            }

            // init lifx config
            textBoxLifxSelector.Text = config.lifxConfig.selector;
            textBoxLifxAuthKey.Text = config.lifxConfig.authKey;

            Console.WriteLine("Config loaded");
        }

        private void SaveConfig()
        {
            ConfigObject newConfig = new ConfigObject()
            {
                generalConfig = new GeneralConfig()
                {
                    syncOnWallpaperChange = checkBoxSyncOnWallpaperChange.Checked,
                    turnOnOffWithPc = checkBoxAutoTurnOnOff.Checked,
                },
                nanoleafConfig = new NanoleafConfig()
                {
                    ipAddress = textBoxNanoleafIP.Text,
                    apiKey = textBoxNanoleafApiKey.Text,
                    effect = GetSelectedNanoleafEffect()
                },
                lifxConfig = new LifxConfig()
                {
                    selector = textBoxLifxSelector.Text,
                    authKey = textBoxLifxAuthKey.Text
                }
            };

            lightController.UpdateConfig(newConfig);
            ConfigManager.ApplyConfig(newConfig);
        }

        private void LoadLastPalette()
        {
            PaletteObject palette = PaletteManager.LoadPalette();
            if (palette == null) {
                return; 
            }

            PreviewPalette(palette, false);
        }

        private NanoleafEffect GetSelectedNanoleafEffect()
        {
            if (radioButtonLightEffectHighlight.Checked)
            {
                return NanoleafEffect.Highlight;
            }
            // default return Random
            return NanoleafEffect.Random;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.ExitThread();
            }
        }

        private void buttonConfigReset_Click(object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void buttonConfigApplyChanges_Click(object sender, EventArgs e)
        {
            // save to file and regenerate scripts etc.
            SaveConfig();
        }

        private void buttonGetWallpaper_Click(object sender, EventArgs e)
        {
            PreviewCurrentWallpaper();
        }

        private void buttonGetPalette_Click(object sender, EventArgs e)
        {
            // sync palette panel colours to palette file
            if (wallpaper == null)
            {
                PreviewCurrentWallpaper();
            }

            PreviewPalette(PaletteManager.GetPaletteFromBitmap(wallpaper));
            Console.WriteLine("Palette preview created");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // save palette
            if (wallpaper == null)
            {
                PreviewCurrentWallpaper();
            }
            PaletteObject palette = PaletteManager.GetPaletteFromBitmap(wallpaper);
            PaletteManager.SavePalette(palette);
            LoadLastPalette();
        }

        private void buttonLightsOn_Click(object sender, EventArgs e)
        {
            if (lightController == null)
            {
                Console.WriteLine("lightcontroller is null");
                return;
            }
            Console.WriteLine("Turning lights On");
            lightController.TurnLightsOnOff(true);
        }

        private void buttonLightsOff_Click(object sender, EventArgs e)
        {
            if (lightController == null)
            {
                Console.WriteLine("lightcontroller is null");
                return;
            }
            Console.WriteLine("Turning lights Off");
            lightController.TurnLightsOnOff(false);
        }

        private void buttonSyncLightColours_Click(object sender, EventArgs e)
        {
            lightController.SyncToWallpaper();

            LoadLastPalette();
        }
    }

    // Custom TextWriter to redirect output to the TextBox control
    internal class TextBoxStreamWriter : TextWriter
    {
        private TextBox textBox;

        public TextBoxStreamWriter(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public override void Write(char value)
        {
            // Append the character to the TextBox
            textBox.AppendText(value.ToString());
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
