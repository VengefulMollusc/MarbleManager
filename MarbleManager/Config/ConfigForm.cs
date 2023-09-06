using MarbleManager.Config;
using MarbleManager.LightScripts;
using PaletteSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleManager
{
    public partial class ConfigForm : Form
    {
        private ConfigHandler configHandler;

        public Bitmap wallpaper;

        public ConfigForm()
        {
            InitializeComponent();

            // init config
            LoadConfig();
        }

        private void LoadConfig()
        {
            // init config handler if needed
            if (configHandler == null)
            {
                configHandler = new ConfigHandler();
            }

            // init general config
            GeneralConfig generalConfig = configHandler.GetGeneralConfig();
            checkBoxSyncOnWallpaperChange.Checked = generalConfig.syncOnWallpaperChange;
            checkBoxAutoTurnOnOff.Checked = generalConfig.turnOnOffWithPc;

            // init nanoleaf config
            NanoleafConfig nanoleafConfig = configHandler.GetNanoleafConfig();
            textBoxNanoleafIP.Text = nanoleafConfig.ipAddress;
            textBoxNanoleafApiKey.Text = nanoleafConfig.apiKey;
            switch (nanoleafConfig.effect)
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
            LifxConfig lifxConfig = configHandler.GetLifxConfig();
            textBoxLifxSelector.Text = lifxConfig.selector;
            textBoxLifxAuthKey.Text = lifxConfig.authKey;

            statusIndicator.Text = "Config loaded";
        }

        private void SaveConfig() { 
            if (configHandler == null) { throw new Exception("Cannot save when no configHandler exists"); }

            configHandler.ApplyChanges(new ConfigObject()
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
            }, statusIndicator);
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

        private void GetCurrentWallpaper ()
        {
            Bitmap currentWallpaper = WallpaperManager.GetWallpaperBitmap();
            if (currentWallpaper != null)
            {
                wallpaper = currentWallpaper;
                pictureBoxWallpaper.Image = currentWallpaper;
            }
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
            GetCurrentWallpaper();
        }

        private void buttonGetPalette_Click(object sender, EventArgs e)
        {
            // sync palette panel colours to palette file
            if (wallpaper == null)
            {
                GetCurrentWallpaper();
            }

            // do palette stuff
            Palette palette = Palette.From(wallpaper).Generate();
            palette.Generate();

            ApplySwatch(paletteCurrentDominant, palette.GetDominantSwatch());
            ApplySwatch(paletteCurrent1, palette.GetVibrantSwatch());
            ApplySwatch(paletteCurrent2, palette.GetLightVibrantSwatch());
            ApplySwatch(paletteCurrent3, palette.GetDarkVibrantSwatch());
            ApplySwatch(paletteCurrent4, palette.GetMutedSwatch());
            ApplySwatch(paletteCurrent5, palette.GetLightMutedSwatch());
            ApplySwatch(paletteCurrent6, palette.GetDarkMutedSwatch());
        }

        private void ApplySwatch (Panel panel, Swatch swatch)
        {
            if (swatch == null)
            {
                panel.BackColor = Color.WhiteSmoke;
                panel.BorderStyle = BorderStyle.FixedSingle;
                return;
            }

            // apply colour if not null
            panel.BackColor = swatch.GetArgb();
            panel.BorderStyle = BorderStyle.None;
        }
    }
}
