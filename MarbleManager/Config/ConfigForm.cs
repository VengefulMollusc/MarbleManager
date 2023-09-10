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
                ApplySwatch(paletteCurrentD, labelCurrentDPop, palette.dominant);
                ApplySwatch(paletteCurrentV, labelCurrentVPop, palette.vibrant);
                ApplySwatch(paletteCurrentVl, labelCurrentVlPop, palette.lightVibrant);
                ApplySwatch(paletteCurrentVd, labelCurrentVdPop, palette.darkVibrant);
                ApplySwatch(paletteCurrentM, labelCurrentMPop, palette.muted);
                ApplySwatch(paletteCurrentMl, labelCurrentMlPop, palette.lightMuted);
                ApplySwatch(paletteCurrentMd, labelCurrentMdPop, palette.darkMuted);
            }
            else
            {
                ApplySwatch(paletteLastD, labelLastDPop, palette.dominant);
                ApplySwatch(paletteLastV, labelLastVPop, palette.vibrant);
                ApplySwatch(paletteLastVl, labelLastVlPop, palette.lightVibrant);
                ApplySwatch(paletteLastVd, labelLastVdPop, palette.darkVibrant);
                ApplySwatch(paletteLastM, labelLastMPop, palette.muted);
                ApplySwatch(paletteLastMl, labelLastMlPop, palette.lightMuted);
                ApplySwatch(paletteLastMd, labelLastMdPop, palette.darkMuted);
            }
        }

        private void ApplySwatch (Panel panel, Label label, SwatchObject swatch)
        {
            if (swatch == null)
            {
                panel.BackColor = Color.WhiteSmoke;
                panel.BorderStyle = BorderStyle.FixedSingle;
                label.Text = "no swatch";
                return;
            }

            // apply colour if not null
            panel.BackColor = Color.FromArgb(swatch.r, swatch.g, swatch.b);
            panel.BorderStyle = BorderStyle.None;
            label.Text = swatch.population.ToString();
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
            ConfigManager.ApplyConfig(new ConfigObject()
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
            });
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
