using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;

namespace AdvancedWebBrowser
{
    [ComVisible(true)]
    public class ScriptManager
    {
        private DownloadManager downloadManager;
        private Form hostForm;

        public ScriptManager(DownloadManager downloadManager, Form hostForm)
        {
            this.downloadManager = downloadManager;
            this.hostForm = hostForm;
        }

        // Called from Javascript: window.external.Download(url)
        public void Download(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            try
            {
                // Ensure call is marshalled to UI thread
                if (hostForm != null && hostForm.IsHandleCreated && !hostForm.IsDisposed)
                {
                    hostForm.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            downloadManager?.StartDownload(url);

                            // Open or activate the Downloads window so user sees progress
                            var existing = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.GetType().Name == "DownloadsForm");
                            if (existing != null)
                            {
                                try
                                {
                                    if (existing.WindowState == FormWindowState.Minimized)
                                        existing.WindowState = FormWindowState.Normal;
                                    existing.BringToFront();
                                    existing.Activate();
                                }
                                catch { }
                            }
                            else
                            {
                                try
                                {
                                    var df = new DownloadsForm(downloadManager);
                                    df.Show(hostForm);
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }));
                }
                else
                {
                    downloadManager?.StartDownload(url);
                    try
                    {
                        var df = new DownloadsForm(downloadManager);
                        df.Show();
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
