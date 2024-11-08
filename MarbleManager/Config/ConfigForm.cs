﻿using MarbleManager.Config;
using MarbleManager.Colours;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MarbleManager.Lights;

namespace MarbleManager
{
    /**
     * Handles the UI window for config and manual light control
     */
    public partial class ConfigForm : Form
    {
        // previewed image or wallpaper
        Bitmap previewImage;

        // light controller
        GlobalLightController lightController;

        // reference to original console log
        TextWriter originalOut;

        // config sections
        GeneralConfigSection generalConfigSection;
        NanoleafConfigSection nanoleafConfigSection;
        LifxConfigSection lifxConfigSection;
        WizConfigSection wizConfigSection;
        PicoConfigSection picoConfigSection;

        public ConfigForm()
        {
            InitializeComponent();
            RedirectConsoleOutput();

            // init config
            LoadConfig();
            LoadLastPalette();
        }

        /**
         * Sets the light controller object
         */
        internal void SetLightController(GlobalLightController _lightController)
        {
            lightController = _lightController;
        }

        /**
         * Redirects Console.WriteLine calls to the 'console' box in the UI when form is open
         */
        private void RedirectConsoleOutput ()
        {
            originalOut = Console.Out;

            // Create a TextWriter to redirect standard output to the TextBox control
            TextWriter writer = new TextBoxStreamWriter(txtConsole);

            // Redirect the standard output
            Console.SetOut(writer);
        }

        /**
         * cleanup values on form close
         */
        protected override void OnClosing(CancelEventArgs e)
        {
            // reset console output
            Console.SetOut(originalOut);
            originalOut = null;

            // clear values to help memory
            if (previewImage != null)
            {
                previewImage.Dispose();
                WallpaperManager.DeleteCopiedWallpaper();
            }
            if (generalConfigSection != null)
                generalConfigSection = null;
            if (lightController != null)
                lightController = null;
            if (nanoleafConfigSection != null)
                nanoleafConfigSection = null;
            if (lifxConfigSection != null)
                lifxConfigSection = null;
            if (wizConfigSection != null)
                wizConfigSection = null;
            if (picoConfigSection != null)
                picoConfigSection = null;

            base.OnClosing(e);
        }

        /**
         * Previews a colour palette on the UI
         */
        private void PreviewPalette (PaletteObject _palette, bool _isPreview = true)
        {
            if (_palette == null) { return; }

            if (_isPreview)
            {
                PreviewSwatch(new List<Panel>() { paletteCurrentD }, new List<Label>() { labelCurrentDPop, labelCurrentDhsl }, _palette.dominant);
                PreviewSwatch(new List<Panel>() { paletteCurrentV, paletteCurrentV2 }, new List<Label>() { labelCurrentVPop, labelCurrentVhsl }, _palette.vibrant);
                PreviewSwatch(new List<Panel>() { paletteCurrentVl, paletteCurrentVl2 }, new List<Label>() { labelCurrentVlPop, labelCurrentVlhsl }, _palette.lightVibrant);
                PreviewSwatch(new List<Panel>() { paletteCurrentVd, paletteCurrentVd2 }, new List<Label>() { labelCurrentVdPop, labelCurrentVdhsl }, _palette.darkVibrant);
                PreviewSwatch(new List<Panel>() { paletteCurrentM, paletteCurrentM2 }, new List<Label>() { labelCurrentMPop, labelCurrentMhsl }, _palette.muted);
                PreviewSwatch(new List<Panel>() { paletteCurrentMl, paletteCurrentMl2 }, new List<Label>() { labelCurrentMlPop, labelCurrentMlhsl }, _palette.lightMuted);
                PreviewSwatch(new List<Panel>() { paletteCurrentMd, paletteCurrentMd2 }, new List<Label>() { labelCurrentMdPop, labelCurrentMdhsl }, _palette.darkMuted);
            }
            else
            {
                PreviewSwatch(new List<Panel>() { paletteLastD }, new List<Label>() { labelLastDPop, labelLastDhsl }, _palette.dominant);
                PreviewSwatch(new List<Panel>() { paletteLastV, paletteLastV2 }, new List<Label>() { labelLastVPop, labelLastVhsl }, _palette.vibrant);
                PreviewSwatch(new List<Panel>() { paletteLastVl, paletteLastVl2 }, new List<Label>() { labelLastVlPop, labelLastVlhsl }, _palette.lightVibrant);
                PreviewSwatch(new List<Panel>() { paletteLastVd, paletteLastVd2 }, new List<Label>() { labelLastVdPop, labelLastVdhsl }, _palette.darkVibrant);
                PreviewSwatch(new List<Panel>() { paletteLastM, paletteLastM2 }, new List<Label>() { labelLastMPop, labelLastMhsl }, _palette.muted);
                PreviewSwatch(new List<Panel>() { paletteLastMl, paletteLastMl2 }, new List<Label>() { labelLastMlPop, labelLastMlhsl }, _palette.lightMuted);
                PreviewSwatch(new List<Panel>() { paletteLastMd, paletteLastMd2 }, new List<Label>() { labelLastMdPop, labelLastMdhsl }, _palette.darkMuted);
            }
        }

        /**
         * Previews a swatch in the UI
         * shows the colour, plus details on labels
         */
        private void PreviewSwatch (List<Panel> _panels, List<Label> _labels, SwatchObject _swatch)
        {
            bool showHsl = true; // toggles previewing rgb or hsl values
            Color bgColour;
            BorderStyle borderStyle;
            string propText, hslText;
            bool isHighlight;

            if (_swatch == null)
            {
                bgColour = Color.WhiteSmoke;
                borderStyle = BorderStyle.FixedSingle;
                propText = "prop: none";
                hslText = showHsl ? "hsl:--- --- ---" : "rgb:--- --- ---";
                isHighlight = false;
            } else
            {
                bgColour = Color.FromArgb(_swatch.r, _swatch.g, _swatch.b);
                borderStyle = BorderStyle.None;
                propText = $"prop:{(int)Math.Round(_swatch.proportion * 100f, 0, MidpointRounding.AwayFromZero)}%";
                hslText = showHsl ?
                    $"hsl:{_swatch.h} {_swatch.s} {_swatch.l}" :
                    $"rgb:{_swatch.r} {_swatch.g} {_swatch.b}";
                isHighlight = _swatch.isHighlight;
            }

            // apply colours etc to the UI
            foreach (Panel panel in _panels)
            {
                panel.BackColor = bgColour;
                panel.BorderStyle = borderStyle;

                if (panel.HasChildren)
                {
                    // toggle highlight indicator
                    panel.Controls[0].Visible = isHighlight;
                }
            }
            _labels[0].Text = propText;
            _labels[1].Text = hslText;
        }

        /**
         * Fetches the current wallpaper and previews on the UI
         */
        private void PreviewCurrentWallpaper()
        {
            // cleanup existing value (stops RAM use increasing with each load)
            if (previewImage != null) { previewImage.Dispose(); }

            Bitmap currentWallpaper = WallpaperManager.GetWallpaperJpg();
            if (currentWallpaper != null)
            {
                previewImage = currentWallpaper;
                pictureBoxWallpaper.Image = previewImage;
            }
            Console.WriteLine("Loaded current wallpaper");
        }

        /**
         * Loads a config object from the manager and populates the UI accordingly
         */
        private void LoadConfig()
        {
            // Clear dynamic UI
            dynamicSettingsPanel.Controls.Clear();

            // Load config
            GlobalConfigObject config = ConfigManager.GetConfig();
            if (config == null)
            {
                LogManager.WriteLog("Config load error - null object returned");
                return;
            }

            // init raspberry pi pico config
            if (picoConfigSection == null)
                picoConfigSection = new PicoConfigSection();
            // init wiz config
            if (wizConfigSection == null)
                wizConfigSection = new WizConfigSection();
            // init lifx config
            if (lifxConfigSection == null)
                lifxConfigSection = new LifxConfigSection();
            // init nanoleaf config
            if (nanoleafConfigSection == null)
                nanoleafConfigSection = new NanoleafConfigSection();
            // init general config
            if (generalConfigSection == null)
                generalConfigSection = new GeneralConfigSection();
            // Add config sections in opposite order to desired UI display order
            dynamicSettingsPanel.Controls.Add(picoConfigSection.GetConfigUI(config.picoConfig));
            dynamicSettingsPanel.Controls.Add(wizConfigSection.GetConfigUI(config.wizConfig));
            dynamicSettingsPanel.Controls.Add(lifxConfigSection.GetConfigUI(config.lifxConfig));
            dynamicSettingsPanel.Controls.Add(nanoleafConfigSection.GetConfigUI(config.nanoleafConfig));
            dynamicSettingsPanel.Controls.Add(generalConfigSection.GetConfigUI(config.generalConfig));

            Console.WriteLine("Config loaded");
        }

        /**
         * Forms a config object from current ui form values and applies the changes
         */
        private void SaveConfig()
        {
            Console.WriteLine("Saving config");

            GlobalConfigObject newConfig = new GlobalConfigObject()
            {
                generalConfig = generalConfigSection.GetConfigObject<GeneralConfig>(),
                nanoleafConfig = nanoleafConfigSection.GetConfigObject<NanoleafConfig>(),
                lifxConfig = lifxConfigSection.GetConfigObject<LifxConfig>(),
                wizConfig = wizConfigSection.GetConfigObject<WizConfig>(),
                picoConfig = picoConfigSection.GetConfigObject<PicoConfig>(),
            };

            // update light controller with new config object
            lightController.UpdateConfig(newConfig);
            
            // save the config
            ConfigManager.ApplyConfig(newConfig, checkBoxForceApply.Checked);

            LogManager.WriteLog("Config saved");
        }

        /**
         * Loads the last sent palette into the relevant preview box
         */
        private void LoadLastPalette()
        {
            PaletteObject palette = PaletteManager.LoadPalette();
            if (palette == null) {
                return; 
            }

            PreviewPalette(palette, false);
        }

        /**
         * show confirmation dialog on exit
         */
        private void exitItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.ExitThread();
            }
        }

        /**
         * reset UI config values to last saved
         */
        private void buttonConfigReset_Click(object sender, EventArgs e)
        {
            LoadConfig();
        }

        /**
         * Apply config changes
         */
        private void buttonConfigApplyChanges_Click(object sender, EventArgs e)
        {
            // save to file and regenerate scripts etc.
            SaveConfig();
        }

        /**
         * Get the current wallpaper
         */
        private void buttonGetWallpaper_Click(object sender, EventArgs e)
        {
            PreviewCurrentWallpaper();
        }

        /**
         * Previews the palette for the current wallpaper
         */
        private void buttonGetPalette_Click(object sender, EventArgs e)
        {
            if (previewImage == null)
            {
                // fetch wallpaper preview if not already
                PreviewCurrentWallpaper();
            }

            PreviewPalette(PaletteManager.GetPaletteFromBitmap(previewImage));
        }

        /**
         * Saves the currently previewed palette to file (as 'last sent')
         */
        private void buttonSavePalette_Click(object sender, EventArgs e)
        {
            // save palette
            if (previewImage == null)
            {
                Console.WriteLine("Preview an image first.");
                return;
            }
            PaletteObject palette = PaletteManager.GetPaletteFromBitmap(previewImage);
            PaletteManager.SavePalette(palette);
            LoadLastPalette();
        }

        /**
         * Turns on the lights
         */
        private async void buttonLightsOn_Click(object sender, EventArgs e)
        {
            if (lightController == null)
            {
                Console.WriteLine("lightcontroller is null");
                return;
            }
            await lightController.TurnLightsOnOff(true);
        }

        /**
         * Turns off the lights
         */
        private async void buttonLightsOff_Click(object sender, EventArgs e)
        {
            if (lightController == null)
            {
                Console.WriteLine("lightcontroller is null");
                return;
            }
            await lightController.TurnLightsOnOff(false);
        }

        /**
         * Syncs light colours to the current wallpaper
         * NOT the currently previewed palette
         */
        private async void buttonSyncLightColours_Click(object sender, EventArgs e)
        {
            await lightController.SyncToWallpaper();

            LoadLastPalette();
        }

        /**
         * Reloads the last saved palette from file
         */
        private void buttonReloadLastSent_Click(object sender, EventArgs e)
        {
            LoadLastPalette();
        }

        /**
         * Loads an image from file into the preview box
         */
        private void buttonLoadImage_Click(object sender, EventArgs e)
        {
            // Show the Open File dialog. If the user clicks OK, load the
            // picture that the user chose.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // cleanup existing value to stop RAM use increasing
                if (previewImage != null) { previewImage.Dispose(); }

                previewImage = new Bitmap(openFileDialog1.FileName);
                pictureBoxWallpaper.Image = previewImage;
            }
        }

        /**
         * Performs a colour sync with the previewed image
         * (rather than the wallpaper)
         */
        private async void buttonSyncPreviewedPalette_Click(object sender, EventArgs e)
        {
            if (previewImage == null)
            {
                Console.WriteLine("Please preview an image first");
                return;
            }

            await lightController.SyncToWallpaper(previewImage);
            LoadLastPalette();

            LogManager.WriteLog("Previewed palette synced to lights");
        }
    }

    /**
     * Custom TextWriter to redirect output to the TextBox control
     */
    internal class TextBoxStreamWriter : TextWriter
    {
        private readonly TextBox textBox;

        public TextBoxStreamWriter(TextBox _textBox)
        {
            this.textBox = _textBox;
        }

        public override void Write(char _value)
        {
            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new Action(() => textBox.AppendText(_value.ToString())));
            }
            else
            {
                textBox.AppendText(_value.ToString());
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
