using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AdvancedWebBrowser
{
    public partial class DownloadsForm : Form
    {
        private DownloadManager downloadManager;
        private ListView listView;
        private Button openFolderButton;
        private Button clearCompletedButton;
        private Button pauseResumeButton;
        private Button cancelButton;
        private Label statusLabel;
        private TextBox urlTextBox; // added for direct URL downloads
        private Button downloadUrlButton; // added

        public DownloadsForm(DownloadManager manager)
        {
            downloadManager = manager;
            InitializeComponent();
            SetupUI();
            SetupEvents();
            RefreshList();
        }

        private void SetupUI()
        {
            // Configure ListView
            this.Text = "Downloads Manager";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // Status Label
            statusLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Text = "Downloads: 0/0 completed",
                Padding = new Padding(10)
            };

            // Button Panel
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                Padding = new Padding(10)
            };

            // URL input for direct downloads
            urlTextBox = new TextBox
            {
                Text = "Paste direct file URL here (e.g. https://example.com/file.zip)",
                ForeColor = Color.Gray,
                Location = new Point(10, 10),
                Size = new Size(520, 30)
            };
            urlTextBox.GotFocus += (s, e) =>
            {
                if (urlTextBox.Text == "Paste direct file URL here (e.g. https://example.com/file.zip)")
                {
                    urlTextBox.Text = string.Empty;
                    urlTextBox.ForeColor = Color.Black;
                }
            };
            urlTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(urlTextBox.Text))
                {
                    urlTextBox.Text = "Paste direct file URL here (e.g. https://example.com/file.zip)";
                    urlTextBox.ForeColor = Color.Gray;
                }
            };

            downloadUrlButton = new Button
            {
                Text = "\u2B07 Download URL",
                Location = new Point(540, 10),
                Size = new Size(120, 30),
                Cursor = Cursors.Hand
            };
            downloadUrlButton.Click += DownloadUrlButton_Click;

            openFolderButton = new Button
            {
                Text = "\uD83D\uDCC1 Open Folder",
                Location = new Point(670, 10),
                Size = new Size(100, 30),
                Cursor = Cursors.Hand
            };
            openFolderButton.Click += (s, e) => downloadManager.OpenDownloadFolder();

            pauseResumeButton = new Button
            {
                Text = "\u23F8 Pause",
                Location = new Point(10, 40),
                Size = new Size(100, 25),
                Cursor = Cursors.Hand
            };
            pauseResumeButton.Click += PauseResumeButton_Click;

            cancelButton = new Button
            {
                Text = "\u2715 Cancel",
                Location = new Point(120, 40),
                Size = new Size(100, 25),
                Cursor = Cursors.Hand
            };
            cancelButton.Click += CancelButton_Click;

            clearCompletedButton = new Button
            {
                Text = "\uD83D\uDDD1 Clear Completed",
                Location = new Point(230, 40),
                Size = new Size(140, 25),
                Cursor = Cursors.Hand
            };
            clearCompletedButton.Click += ClearCompletedButton_Click;

            buttonPanel.Controls.Add(urlTextBox);
            buttonPanel.Controls.Add(downloadUrlButton);
            buttonPanel.Controls.Add(openFolderButton);
            buttonPanel.Controls.Add(pauseResumeButton);
            buttonPanel.Controls.Add(cancelButton);
            buttonPanel.Controls.Add(clearCompletedButton);

            // ListView setup
            listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9)
            };

            listView.Columns.Add("File Name", 200);
            listView.Columns.Add("Status", 120);
            listView.Columns.Add("Progress", 100);
            listView.Columns.Add("Size", 100);
            listView.Columns.Add("Speed", 80);
            listView.Columns.Add("Time Left", 80);
            listView.Columns.Add("Start Time", 100);

            this.Controls.Add(listView);
            this.Controls.Add(statusLabel);
            this.Controls.Add(buttonPanel);
        }

        private void DownloadUrlButton_Click(object sender, EventArgs e)
        {
            var url = urlTextBox?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(url) || url == "Paste direct file URL here (e.g. https://example.com/file.zip)")
            {
                MessageBox.Show("Please paste a direct file URL to download.", "No URL", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Attempt to determine filename from URL
                string suggested = null;
                try
                {
                    suggested = System.IO.Path.GetFileName(new Uri(url).LocalPath);
                }
                catch { /* ignore */ }

                if (string.IsNullOrWhiteSpace(suggested))
                    suggested = "download_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                downloadManager.StartDownload(url, suggested);

                // clear input and update status
                urlTextBox.Text = string.Empty;
                urlTextBox.ForeColor = Color.Black;
                UpdateStatus($"Starting download: {suggested}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start download: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupEvents()
        {
            if (downloadManager != null && listView != null)
            {
                downloadManager.DownloadProgressChanged += (s, item) => RefreshItem(item);
                downloadManager.DownloadCompleted += (s, item) => RefreshItem(item);
            }
        }

        private void RefreshList()
        {
            if (listView != null && downloadManager != null)
            {
                listView.Items.Clear();
                foreach (var download in downloadManager.Downloads)
                {
                    AddOrUpdateItem(download);
                }
                UpdateStatus();
            }
        }

        private void RefreshItem(DownloadItem item)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => RefreshItem(item)));
                return;
            }

            AddOrUpdateItem(item);
            UpdateStatus();
        }

        private void AddOrUpdateItem(DownloadItem item)
        {
            var existingItem = listView.Items.Cast<ListViewItem>()
                .FirstOrDefault(lvi => lvi.Tag == item);

            if (existingItem == null)
            {
                existingItem = new ListViewItem(item.FileName);
                existingItem.Tag = item;
                listView.Items.Add(existingItem);

                existingItem.SubItems.Add(item.Status);
                existingItem.SubItems.Add($"{item.ProgressPercentage:F1}%");
                existingItem.SubItems.Add(FormatBytes(item.TotalBytes));
                existingItem.SubItems.Add($"{item.DownloadSpeed:F2} KB/s");
                existingItem.SubItems.Add(item.TimeRemaining ?? "-");
                existingItem.SubItems.Add(item.StartTime.ToString("HH:mm:ss"));
            }
            else
            {
                existingItem.SubItems[1].Text = item.Status;
                existingItem.SubItems[2].Text = $"{item.ProgressPercentage:F1}%";
                existingItem.SubItems[3].Text = FormatBytes(item.TotalBytes);
                existingItem.SubItems[4].Text = $"{item.DownloadSpeed:F2} KB/s";
                existingItem.SubItems[5].Text = item.TimeRemaining ?? "-";
            }

            // Color coding based on status
            if (item.Status.Contains("Completed"))
                existingItem.BackColor = Color.LightGreen;
            else if (item.Status.Contains("Failed") || item.Status.Contains("Error"))
                existingItem.BackColor = Color.LightCoral;
            else if (item.Status.Contains("Cancelled") || item.Status.Contains("Paused"))
                existingItem.BackColor = Color.LightYellow;
            else if (item.Status.Contains("Retrying"))
                existingItem.BackColor = Color.Khaki;
            else
                existingItem.BackColor = Color.LightBlue;
        }

        private string FormatBytes(long bytes)
        {
            if (bytes == 0) return "0 B";
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void UpdateStatus()
        {
            if (statusLabel != null && downloadManager != null)
            {
                var total = downloadManager.Downloads.Count;
                var completed = downloadManager.Downloads.Count(d => d.IsCompleted);
                var downloading = downloadManager.Downloads.Count(d => d.Status.Contains("Downloading"));
                statusLabel.Text = $"Downloads: {completed}/{total} completed | Currently downloading: {downloading}";
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusLabel != null)
            {
                statusLabel.Text = message;
            }
        }

        private void PauseResumeButton_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                var selectedItem = (DownloadItem)listView.SelectedItems[0].Tag;
                if (selectedItem.Status.Contains("Downloading"))
                {
                    downloadManager.PauseDownload(selectedItem);
                    pauseResumeButton.Text = "\u25B6 Resume";
                }
                else if (selectedItem.Status.Contains("Paused"))
                {
                    downloadManager.ResumeDownload(selectedItem);
                    pauseResumeButton.Text = "\u23F8 Pause";
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                var selectedItem = (DownloadItem)listView.SelectedItems[0].Tag;
                if (DialogResult.Yes == MessageBox.Show(
                    $"Cancel download of '{selectedItem.FileName}'?",
                    "Confirm Cancellation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question))
                {
                    downloadManager.CancelDownload(selectedItem);
                }
            }
        }

        private void ClearCompletedButton_Click(object sender, EventArgs e)
        {
            var completedItems = downloadManager.Downloads.Where(d => d.IsCompleted).ToList();
            foreach (var item in completedItems)
            {
                downloadManager.Downloads.Remove(item);
            }
            RefreshList();
        }
    }
}