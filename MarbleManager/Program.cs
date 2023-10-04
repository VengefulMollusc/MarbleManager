using MarbleManager.Lights;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MarbleManager.Config;

namespace MarbleManager
{
    internal static class Program
    {
        // Define the mutex as a static field so it can be accessed in multiple methods.
        static Mutex appMutex;

        // define a thread for the pipe listener
        static Thread pipeThread;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            LogManager.WriteLog("App starting");
            // Create and acquire the mutex at the beginning of your application.
            bool createdNew;
            appMutex = new Mutex(true, "MyAppMutex", out createdNew);

            if (!createdNew)
            {
                // Another instance of the application is already running.
                LogManager.WriteLog("Another instance of the application is already running.");
                SendCommandLineArguments(args);
                LogManager.WriteLog("Closing app due to existing instance");
                appMutex.Close();
                return;
            }

            // Register an event handler for ApplicationExit.
            Application.ApplicationExit += Application_ApplicationExit;

            // check and act on command line arguments
            if (args.Length > 0)
            {
                // perform commands
                bool bootApp = await ProcessCommandLineArgs(args);
                if (!bootApp)
                    return;
            }

            // start listening for other cmd calls
            pipeThread = new Thread(ListenToNamedPipe);
            pipeThread.IsBackground = true;
            pipeThread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationContext applicationContext = new CustomApplicationContext();
            Application.Run(applicationContext);
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            LogManager.WriteLog("App exit - main instance.");
            // Release the mutex when your application is done.
            if (appMutex != null)
            {
                appMutex.ReleaseMutex();
                appMutex.Close();
            }
        }

        /**
         * Perform tasks from command line
         */
        static async Task<bool> ProcessCommandLineArgs(string[] args)
        {
            LogManager.WriteLog("Processing cmd args: " + args);
            GlobalLightController lightController = GlobalLightController.Instance;
            bool bootApp = false;

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
                    case "bootapp":
                        bootApp = true;
                        break;
                    default:
                        Console.WriteLine($"Invalid command: {args[i]}");
                        return false;
                }
            }

            return bootApp;
        }

        /**
         * Send args to existing running app
         */
        static void SendCommandLineArguments(string[] args)
        {
            try
            {
                LogManager.WriteLog("Pipe: Sending cmd args to existing instance");
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream("MarbleManagerPipe"))
                {
                    LogManager.WriteLog("Pipe: connecting");
                    pipeClient.Connect();

                    using (StreamWriter sw = new StreamWriter(pipeClient))
                    {
                        // Concatenate all command-line arguments into a single string
                        string argumentString = string.Join(" ", args);

                        // Send the concatenated string
                        LogManager.WriteLog("Pipe: sending args");
                        sw.WriteLine(argumentString);
                        LogManager.WriteLog("Pipe: args sent");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error communicating with the existing instance: " + ex.Message);
            }
        }

        /**
         * listen for args from cmd
         */
        static async void ListenToNamedPipe()
        {
            // Set up a named pipe server to listen for incoming data.
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("MarbleManagerPipe"))
            {
                LogManager.WriteLog("Pipe: Waiting for incoming data...");

                try
                {
                    pipeServer.WaitForConnection();

                    using (StreamReader sr = new StreamReader(pipeServer))
                    {
                        while (true) // Continue listening until the application exits.
                        {
                            // Read the concatenated argument string
                            string argumentString = sr.ReadLine();

                            // Split the received string back into individual arguments
                            string[] arguments = argumentString.Split(' ');

                            // Process the received command-line arguments.
                            LogManager.WriteLog("Pipe: Received command-line arguments: " + arguments);

                            await ProcessCommandLineArgs(arguments);
                        }
                    }
                }
                catch (IOException ex)
                {
                    // IOException is thrown when the pipe is closed (e.g., when the application exits).
                    LogManager.WriteLog("Pipe: closed. Exiting pipe thread.");
                }
            }
        }
    }
}
