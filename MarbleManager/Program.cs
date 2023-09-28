using MarbleManager.Lights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleManager
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            // check and act on command line arguments
            if (args.Length > 0)
            {
                // perform commands then end execution
                await PerformCommandLineFunctions(args);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationContext applicationContext = new CustomApplicationContext();
            Application.Run(applicationContext);
        }

        /**
         * Perform tasks from command line
         */
        static async Task PerformCommandLineFunctions(string[] args)
        {
            GlobalLightController lightController = new GlobalLightController();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "on":
                        await lightController.TurnLightsOnOff(true);
                        break;
                    case "off":
                        await lightController.TurnLightsOnOff(false);
                        break;
                    case "sync":
                        await lightController.SyncToWallpaper();
                        break;
                    case "syncon":
                        await lightController.TurnOnAndSyncToWallpaper();
                        break;
                    default:
                        Console.WriteLine($"Invalid command: {args[i]}");
                        return;
                }
            }
        }
    }
}
