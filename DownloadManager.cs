using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace AdvancedWebBrowser
{
    public class DownloadItem
    {
        public string Url { get; set; }
        public string FileName { get; set; }
        public string SavePath { get; set; }
        public long BytesReceived { get; set; }
        public long TotalBytes { get; set; }
        public double ProgressPercentage => TotalBytes > 0 ? (BytesReceived * 100.0) / TotalBytes : 0;
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsCompleted => Status == "Completed" || Status == "Failed" || Status.StartsWith("Failed:");
        public double DownloadSpeed { get; set; } // KB/s
        public string TimeRemaining { get; set; }
        public WebClient Client { get; set; }
        public int RetryCount { get; set; } = 0;
        public const int MaxRetries = 3;
    }

    public class DownloadManager
    {
        private List<DownloadItem> downloads;
        private Dictionary<DownloadItem, WebClient> activeDownloads;
        private string downloadFolder;
        private const int MaxConcurrentDownloads = 3;

        public List<DownloadItem> Downloads => downloads;
        public event EventHandler<DownloadItem> DownloadProgressChanged;
        public event EventHandler<DownloadItem> DownloadCompleted;

        public DownloadManager()
        {
            downloads = new List<DownloadItem>();
            activeDownloads = new Dictionary<DownloadItem, WebClient>();
            downloadFolder = Path.Combine(Application.StartupPath, "Downloads");
            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }
        }

        public void StartDownload(string url, string suggestedFileName = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            var downloadItem = new DownloadItem
            {
                Url = url,
                StartTime = DateTime.Now,
                Status = "Pending...",
                RetryCount = 0
            };

            // Determine filename
            if (string.IsNullOrEmpty(suggestedFileName))
            {
                try
                {
                    suggestedFileName = Path.GetFileName(new Uri(url).LocalPath);
                }
                catch
                {
                    suggestedFileName = "download";
                }

                if (string.IsNullOrEmpty(suggestedFileName) || suggestedFileName == "/")
                {
                    suggestedFileName = "download_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                }
            }

            // Ensure unique filename
            string filePath = Path.Combine(downloadFolder, suggestedFileName);
            int counter = 1;
            while (File.Exists(filePath))
            {
                string name = Path.GetFileNameWithoutExtension(suggestedFileName);
                string extension = Path.GetExtension(suggestedFileName);
                filePath = Path.Combine(downloadFolder, $"{name}({counter}){extension}");
                counter++;
            }

            downloadItem.FileName = Path.GetFileName(filePath);
            downloadItem.SavePath = filePath;

            lock (downloads)
            {
                downloads.Add(downloadItem);
            }

            // Notify UI immediately about new pending download so it appears in the list
            try
            {
                DownloadProgressChanged?.Invoke(this, downloadItem);
            }
            catch { }

            // Start download if slots available
            ProcessNextDownload();
        }

        private void ProcessNextDownload()
        {
            lock (activeDownloads)
            {
                if (activeDownloads.Count >= MaxConcurrentDownloads)
                    return;

                var pendingDownload = downloads.FirstOrDefault(d => d.Status == "Pending...");
                if (pendingDownload != null)
                {
                    StartDownloadInternal(pendingDownload);
                }
            }
        }

        private void StartDownloadInternal(DownloadItem downloadItem)
        {
            downloadItem.Status = "Downloading...";

            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.Headers.Add("Accept", "*/*");
            client.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            client.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            client.Headers.Add("Cache-Control", "no-cache");
            client.Headers.Add("Pragma", "no-cache");
            client.Headers.Add("Referer", "https://getintopc.com/");
            client.Headers.Add("Sec-Fetch-Dest", "document");
            client.Headers.Add("Sec-Fetch-Mode", "navigate");
            client.Headers.Add("Sec-Fetch-Site", "none");
            client.Headers.Add("Upgrade-Insecure-Requests", "1");

            long lastBytesReceived = 0;
            DateTime lastSpeedCheck = DateTime.Now;

            client.DownloadProgressChanged += (s, e) =>
            {
                downloadItem.BytesReceived = e.BytesReceived;
                downloadItem.TotalBytes = e.TotalBytesToReceive;
                downloadItem.Status = "Downloading...";

                // Calculate speed
                TimeSpan elapsed = DateTime.Now - lastSpeedCheck;
                if (elapsed.TotalSeconds >= 1)
                {
                    long bytesDifference = e.BytesReceived - lastBytesReceived;
                    downloadItem.DownloadSpeed = bytesDifference / 1024.0; // KB/s
                    lastBytesReceived = e.BytesReceived;
                    lastSpeedCheck = DateTime.Now;

                    // Calculate time remaining
                    if (downloadItem.DownloadSpeed > 0 && downloadItem.TotalBytes > 0)
                    {
                        long bytesRemaining = downloadItem.TotalBytes - downloadItem.BytesReceived;
                        double secondsRemaining = bytesRemaining / (downloadItem.DownloadSpeed * 1024);
                        downloadItem.TimeRemaining = FormatTimeSpan(TimeSpan.FromSeconds(secondsRemaining));
                    }
                }

                try { DownloadProgressChanged?.Invoke(this, downloadItem); } catch { }
            };

            client.DownloadFileCompleted += (s, e) =>
            {
                downloadItem.EndTime = DateTime.Now;

                if (e.Error != null)
                {
                    // Log the actual error for debugging
                    string errorMsg = e.Error?.InnerException?.Message ?? e.Error?.Message ?? "Unknown error";
                    
                    if (downloadItem.RetryCount < DownloadItem.MaxRetries)
                    {
                        downloadItem.RetryCount++;
                        downloadItem.Status = $"Retrying... ({downloadItem.RetryCount}/{DownloadItem.MaxRetries})";
                        downloadItem.BytesReceived = 0;
                        try { DownloadProgressChanged?.Invoke(this, downloadItem); } catch { }

                        // Retry after delay
                        Thread.Sleep(2000);
                        client?.Dispose();
                        lock (activeDownloads)
                        {
                            activeDownloads.Remove(downloadItem);
                        }
                        StartDownloadInternal(downloadItem);
                        return;
                    }
                    else
                    {
                        downloadItem.Status = $"Failed: {errorMsg}";
                    }
                }
                else if (e.Cancelled)
                {
                    downloadItem.Status = "Cancelled";
                    if (File.Exists(downloadItem.SavePath))
                    {
                        try { File.Delete(downloadItem.SavePath); } catch { }
                    }
                }
                else
                {
                    downloadItem.Status = "Completed";
                }

                try { DownloadCompleted?.Invoke(this, downloadItem); } catch { }

                lock (activeDownloads)
                {
                    activeDownloads.Remove(downloadItem);
                }

                client?.Dispose();

                // Process next download
                ProcessNextDownload();
            };

            try
            {
                // Enable automatic redirect handling
                ServicePointManager.DefaultConnectionLimit = 10;
                ServicePointManager.ReusePort = true;
                
                lock (activeDownloads)
                {
                    activeDownloads[downloadItem] = client;
                }

                client.DownloadFileAsync(new Uri(downloadItem.Url), downloadItem.SavePath);
            }
            catch (Exception ex)
            {
                downloadItem.Status = $"Error: {ex.Message}";
                try { DownloadCompleted?.Invoke(this, downloadItem); } catch { }

                lock (activeDownloads)
                {
                    activeDownloads.Remove(downloadItem);
                }

                client?.Dispose();
                ProcessNextDownload();
            }
        }

        public void PauseDownload(DownloadItem item)
        {
            if (item == null) return;
            lock (activeDownloads)
            {
                if (activeDownloads.TryGetValue(item, out var client))
                {
                    client?.CancelAsync();
                    item.Status = "Paused";
                    try { DownloadProgressChanged?.Invoke(this, item); } catch { }
                }
            }
        }

        public void ResumeDownload(DownloadItem item)
        {
            if (item == null || item.IsCompleted) return;
            item.Status = "Pending...";
            try { DownloadProgressChanged?.Invoke(this, item); } catch { }
            ProcessNextDownload();
        }

        public void CancelDownload(DownloadItem item)
        {
            if (item == null) return;
            lock (activeDownloads)
            {
                if (activeDownloads.TryGetValue(item, out var client))
                {
                    client?.CancelAsync();
                }
            }
        }

        public void CancelAllDownloads()
        {
            lock (activeDownloads)
            {
                foreach (var kvp in activeDownloads.ToList())
                {
                    kvp.Value?.CancelAsync();
                }
            }
        }

        public string GetDownloadFolder()
        {
            return downloadFolder;
        }

        public void OpenDownloadFolder()
        {
            try
            {
                Process.Start("explorer.exe", downloadFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening download folder: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
                return string.Format("{0:00}h {1:00}m {2:00}s", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
            else if (ts.TotalMinutes >= 1)
                return string.Format("{0:00}m {1:00}s", (int)ts.TotalMinutes, ts.Seconds);
            else
                return string.Format("{0:00}s", (int)ts.TotalSeconds);
        }
    }
}