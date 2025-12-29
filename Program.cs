using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace AdvancedWebBrowser
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Ensure WebBrowser uses a modern IE emulation mode
            try
            {
                SetBrowserFeatureControl();
                EnableModernBrowserFeatures();
            }
            catch
            {
                // ignore registry failures
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (sender, e) =>
            {
                ShowUnhandledException(e.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                ShowUnhandledException(ex);
            };

            Application.Run(new BrowserForm());
        }

        private static void ShowUnhandledException(Exception ex)
        {
            try
            {
                string message = ex != null
                    ? $"Unhandled exception:\n{ex.Message}\n\n{ex.StackTrace}"
                    : "Unhandled non-Exception error occurred.";

                MessageBox.Show(message, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Optionally log to a file for later inspection
                try
                {
                    var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");
                    File.AppendAllText(logPath, DateTime.Now + " - " + message + Environment.NewLine + "---" + Environment.NewLine);
                }
                catch
                {
                    // ignore logging failures
                }
            }
            catch
            {
                // ignore secondary failures
            }
        }

        private static void SetBrowserFeatureControl()
        {
            // Set browser emulation to IE11 for this executable under HKCU so it doesn't need admin
            string feature = "FEATURE_BROWSER_EMULATION";
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey($"Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\{feature}"))
                {
                    if (key != null)
                    {
                        string exeName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
                        // 11001 = IE11 Edge mode
                        int ie11Mode = 11001;
                        key.SetValue(exeName, ie11Mode, RegistryValueKind.DWord);
                    }
                }
            }
            catch { }
        }

        private static void EnableModernBrowserFeatures()
        {
            try
            {
                // Enable various IE features for better compatibility
                string[] features = {
                    "FEATURE_GPU_RENDERING",
                    "FEATURE_BLOCK_LMZ_SCRIPT",
                    "FEATURE_DISABLE_NAVIGATION_SOUNDS",
                    "FEATURE_SCRIPTLET_MARKUP",
                    "FEATURE_MIME_HANDLING",
                    "FEATURE_LOCALMACHINE_LOCKDOWN",
                    "FEATURE_BEHAVIORS",
                    "FEATURE_ACTIVEX_REPURPOSEDETECTION",
                    "FEATURE_ENABLE_SCRIPT_PASTE_URLACTION_IF_PROMPT",
                    "FEATURE_AJAX_CONNECTIONEVENTS",
                    "FEATURE_RESTRICT_ACTIVEXINSTALL",
                    "FEATURE_RESTRICT_FILEDOWNLOAD",
                    "FEATURE_ADDON_MANAGEMENT",
                    "FEATURE_WEBSOCKET",
                    "FEATURE_WINDOW_RESTRICTIONS"
                };

                foreach (string feature in features)
                {
                    try
                    {
                        using (var key = Registry.CurrentUser.CreateSubKey($"Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\{feature}"))
                        {
                            if (key != null)
                            {
                                string exeName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
                                key.SetValue(exeName, 1, RegistryValueKind.DWord);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}