using System.Drawing;
using System.Windows.Forms;

namespace AdvancedWebBrowser
{
    partial class DownloadsForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.Text = "Downloads";
            this.Size = new Size(800, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            // ListView
            listView = new ListView
            {
                Dock = DockStyle.Top,
                Height = 300,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            listView.Columns.Add("File Name", 200);
            listView.Columns.Add("Status", 100);
            listView.Columns.Add("Progress", 100);
            listView.Columns.Add("Size", 100);
            listView.Columns.Add("Start Time", 120);
            listView.Columns.Add("URL", 200);

            // Buttons panel
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };

            openFolderButton = new Button
            {
                Text = "Open Download Folder",
                Location = new Point(10, 8),
                Size = new Size(140, 25)
            };

            clearCompletedButton = new Button
            {
                Text = "Clear Completed",
                Location = new Point(160, 8),
                Size = new Size(100, 25)
            };

            statusLabel = new Label
            {
                Text = "Total downloads: 0",
                Location = new Point(270, 12),
                Size = new Size(200, 20)
            };

            buttonPanel.Controls.Add(openFolderButton);
            buttonPanel.Controls.Add(clearCompletedButton);
            buttonPanel.Controls.Add(statusLabel);

            this.Controls.Add(listView);
            this.Controls.Add(buttonPanel);

            // Events
            openFolderButton.Click += (s, e) => downloadManager.OpenDownloadFolder();
            clearCompletedButton.Click += ClearCompletedButton_Click;
        }

        #region Windows Form Designer generated code

        // Designer file intentionally left without InitializeComponent because
        // the main partial implementation provides the UI setup.

        #endregion
    }
}