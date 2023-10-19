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
            // Create and acquire the mutex at the beginning of your application.
            bool createdNew;
            appMutex = new Mutex(true, "MyAppMutex", out createdNew);

            if (!createdNew)
            {
                // Another instance of the application is already running.
                LogManager.WriteLog("Starting: Another instance of the application is already running.", true);
                SendCommandLineArguments(args);
                LogManager.WriteLog("Exiting: due to existing instance", true);
                return;
            }

            LogManager.WriteLog("Starting: No existing instance detected");
            // Register an event handler for ApplicationExit.
            Application.ApplicationExit += Application_ApplicationExit;

            // check and act on command line arguments
            if (args.Length > 0)
            {
                // perform commands
                LogManager.WriteLog("Performing initial process of command line arguments");
                bool bootApp = await ProcessCommandLineArgs(args);
                if (!bootApp)
                {
                    LogManager.WriteLog("Exiting: command line only");
                    return;
                }
            }

            LogManager.WriteLog("Booting full app");
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
            LogManager.WriteLog("Exiting: main instance closing");
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
            LogManager.WriteLog("Processing cmd args", string.Join(" ", args));

            bool bootApp = false;
            List<string> filteredArgs = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "on":
                    case "off":
                    case "sync":
                    case "syncon":
                        // standard light commands
                        filteredArgs.Add(args[i]);
                        break;
                    case "bootapp":
                        // whether to boot the full app
                        bootApp = true;
                        break;
                    default:
                        LogManager.WriteLog($"Invalid command: {args[i]}");
                        break;
                }
            }

            // initialise the lightcontroller (with app boot state)
            GlobalLightController lightController = GlobalLightController.GetInstance(bootApp, false);

            // process light commands
            foreach (string arg in filteredArgs)
            {
                switch (arg)
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
                        await lightController.SyncToWallpaper(null, true);
                        break;
                    default:
                        break;
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
                LogManager.WriteLog("Pipe: Sending cmd args to existing instance", true);
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream("MarbleManagerPipe"))
                {
                    pipeClient.Connect();
                    LogManager.WriteLog("Pipe: connected", true);

                    using (StreamWriter sw = new StreamWriter(pipeClient))
                    {
                        // Concatenate all command-line arguments into a single string
                        string argumentString = string.Join(" ", args);

                        // Send the concatenated string
                        sw.WriteLine(argumentString);
                        LogManager.WriteLog("Pipe: args sent: " + argumentString, true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Pipe: Error communicating with the existing instance: " + ex.Message, true);
            }
        }

        /**
         * listen for args from cmd
         */
        static async void ListenToNamedPipe()
        {
            while (true)
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
                            while (sr.Peek() != -1) // Continue listening until the application exits.
                            {
                                // Read the concatenated argument string
                                string argumentString = sr.ReadLine();

                                // Process the received command-line arguments.
                                LogManager.WriteLog("Pipe: Received command-line arguments: " + argumentString);

                                // Split the received string back into individual arguments
                                string[] arguments = argumentString.Split(' ');

                                await ProcessCommandLineArgs(arguments);
                            }
                            sr.Close();
                        }
                    }
                    catch (IOException ex)
                    {
                        // IOException is thrown when the pipe is closed (e.g., when the application exits).
                        LogManager.WriteLog("Pipe: error. Exiting pipe thread: " + ex.Message);
                    }
                }
            }
        }
    }
}
