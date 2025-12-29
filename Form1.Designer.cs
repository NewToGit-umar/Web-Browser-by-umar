using System;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedWebBrowser
{
    partial class BrowserForm
    {
        private System.ComponentModel.IContainer components = null;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newTabToolStripMenuItem;
        private ToolStripMenuItem closeTabToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private TabControl tabControl;
        private TextBox addressBar;
        private Button goButton;
        private Button newTabButton;
        private Button closeTabButton;
        private ListBox suggestionsListBox;
        private Panel suggestionsPanel;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem nextTabToolStripMenuItem;
        private ToolStripMenuItem previousTabToolStripMenuItem;

        // New controls
        private Panel toolbarPanel;
        private Button backButton;
        private Button forwardButton;
        private Button refreshButton;
        private Button homeButton;
        private ComboBox searchEngineComboBox;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private CheckBox forceSearchCheckBox;
        private Button downloadsButton;
        private ToolStripMenuItem downloadsToolStripMenuItem;

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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbarPanel = new System.Windows.Forms.Panel();
            this.backButton = new System.Windows.Forms.Button();
            this.forwardButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.homeButton = new System.Windows.Forms.Button();
            this.searchEngineComboBox = new System.Windows.Forms.ComboBox();
            this.forceSearchCheckBox = new System.Windows.Forms.CheckBox();
            this.addressBar = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.newTabButton = new System.Windows.Forms.Button();
            this.closeTabButton = new System.Windows.Forms.Button();
            this.downloadsButton = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.suggestionsListBox = new System.Windows.Forms.ListBox();
            this.suggestionsPanel = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.toolbarPanel.SuspendLayout();
            this.suggestionsPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 36);
            this.menuStrip.TabIndex = 6;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTabToolStripMenuItem,
            this.closeTabToolStripMenuItem,
            this.downloadsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 30);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newTabToolStripMenuItem
            // 
            this.newTabToolStripMenuItem.Name = "newTabToolStripMenuItem";
            this.newTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.newTabToolStripMenuItem.Size = new System.Drawing.Size(257, 34);
            this.newTabToolStripMenuItem.Text = "New Tab";
            this.newTabToolStripMenuItem.Click += new System.EventHandler(this.NewTabToolStripMenuItem_Click);
            // 
            // closeTabToolStripMenuItem
            // 
            this.closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            this.closeTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeTabToolStripMenuItem.Size = new System.Drawing.Size(257, 34);
            this.closeTabToolStripMenuItem.Text = "Close Tab";
            this.closeTabToolStripMenuItem.Click += new System.EventHandler(this.CloseTabToolStripMenuItem_Click);
            // 
            // downloadsToolStripMenuItem
            // 
            this.downloadsToolStripMenuItem.Name = "downloadsToolStripMenuItem";
            this.downloadsToolStripMenuItem.Size = new System.Drawing.Size(257, 34);
            this.downloadsToolStripMenuItem.Text = "Downloads";
            this.downloadsToolStripMenuItem.Click += new System.EventHandler(this.DownloadsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(257, 34);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nextTabToolStripMenuItem,
            this.previousTabToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(65, 30);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // nextTabToolStripMenuItem
            // 
            this.nextTabToolStripMenuItem.Name = "nextTabToolStripMenuItem";
            this.nextTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Tab)));
            this.nextTabToolStripMenuItem.Size = new System.Drawing.Size(339, 34);
            this.nextTabToolStripMenuItem.Text = "Next Tab";
            this.nextTabToolStripMenuItem.Click += new System.EventHandler(this.NextTabToolStripMenuItem_Click);
            // 
            // previousTabToolStripMenuItem
            // 
            this.previousTabToolStripMenuItem.Name = "previousTabToolStripMenuItem";
            this.previousTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
            | System.Windows.Forms.Keys.Tab)));
            this.previousTabToolStripMenuItem.Size = new System.Drawing.Size(339, 34);
            this.previousTabToolStripMenuItem.Text = "Previous Tab";
            this.previousTabToolStripMenuItem.Click += new System.EventHandler(this.PreviousTabToolStripMenuItem_Click);
            // 
            // toolbarPanel
            // 
            this.toolbarPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.toolbarPanel.Controls.Add(this.backButton);
            this.toolbarPanel.Controls.Add(this.forwardButton);
            this.toolbarPanel.Controls.Add(this.refreshButton);
            this.toolbarPanel.Controls.Add(this.homeButton);
            this.toolbarPanel.Controls.Add(this.searchEngineComboBox);
            this.toolbarPanel.Controls.Add(this.forceSearchCheckBox);
            this.toolbarPanel.Controls.Add(this.addressBar);
            this.toolbarPanel.Controls.Add(this.goButton);
            this.toolbarPanel.Controls.Add(this.newTabButton);
            this.toolbarPanel.Controls.Add(this.closeTabButton);
            this.toolbarPanel.Controls.Add(this.downloadsButton);
            this.toolbarPanel.Location = new System.Drawing.Point(0, 33);
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Size = new System.Drawing.Size(1200, 40);
            this.toolbarPanel.TabIndex = 7;
            // 
            // backButton
            // 
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.backButton.Location = new System.Drawing.Point(8, 6);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(36, 28);
            this.backButton.TabIndex = 7;
            this.backButton.Text = "←";
            this.backButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // forwardButton
            // 
            this.forwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.forwardButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.forwardButton.Location = new System.Drawing.Point(50, 6);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(36, 28);
            this.forwardButton.TabIndex = 8;
            this.forwardButton.Text = "→";
            this.forwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.refreshButton.Location = new System.Drawing.Point(92, 6);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(36, 28);
            this.refreshButton.TabIndex = 9;
            this.refreshButton.Text = "↻";
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // homeButton
            // 
            this.homeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.homeButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.homeButton.Location = new System.Drawing.Point(134, 6);
            this.homeButton.Name = "homeButton";
            this.homeButton.Size = new System.Drawing.Size(36, 28);
            this.homeButton.TabIndex = 10;
            this.homeButton.Text = "⌂";
            this.homeButton.Click += new System.EventHandler(this.HomeButton_Click);
            // 
            // searchEngineComboBox
            // 
            this.searchEngineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchEngineComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchEngineComboBox.FormattingEnabled = true;
            this.searchEngineComboBox.Items.AddRange(new object[] {
            "Google",
            "Bing",
            "Yahoo",
            "DuckDuckGo"});
            this.searchEngineComboBox.Location = new System.Drawing.Point(178, 8);
            this.searchEngineComboBox.Name = "searchEngineComboBox";
            this.searchEngineComboBox.Size = new System.Drawing.Size(120, 33);
            this.searchEngineComboBox.TabIndex = 11;
            // 
            // forceSearchCheckBox
            // 
            this.forceSearchCheckBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.forceSearchCheckBox.Location = new System.Drawing.Point(306, 11);
            this.forceSearchCheckBox.Name = "forceSearchCheckBox";
            this.forceSearchCheckBox.Size = new System.Drawing.Size(110, 24);
            this.forceSearchCheckBox.TabIndex = 12;
            this.forceSearchCheckBox.Text = "Force search";
            // 
            // addressBar
            // 
            this.addressBar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.addressBar.Location = new System.Drawing.Point(422, 8);
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new System.Drawing.Size(560, 34);
            this.addressBar.TabIndex = 4;
            this.addressBar.TextChanged += new System.EventHandler(this.AddressBar_TextChanged);
            this.addressBar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AddressBar_KeyDown);
            // 
            // goButton
            // 
            this.goButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.goButton.Location = new System.Drawing.Point(988, 8);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(60, 28);
            this.goButton.TabIndex = 3;
            this.goButton.Text = "Go";
            this.goButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // newTabButton
            // 
            this.newTabButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newTabButton.Location = new System.Drawing.Point(1056, 8);
            this.newTabButton.Name = "newTabButton";
            this.newTabButton.Size = new System.Drawing.Size(36, 28);
            this.newTabButton.TabIndex = 2;
            this.newTabButton.Text = "+";
            this.newTabButton.Click += new System.EventHandler(this.NewTabButton_Click);
            // 
            // closeTabButton
            // 
            this.closeTabButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeTabButton.Location = new System.Drawing.Point(1098, 8);
            this.closeTabButton.Name = "closeTabButton";
            this.closeTabButton.Size = new System.Drawing.Size(36, 28);
            this.closeTabButton.TabIndex = 1;
            this.closeTabButton.Text = "×";
            this.closeTabButton.Click += new System.EventHandler(this.CloseTabButton_Click);
            // 
            // downloadsButton
            // 
            this.downloadsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadsButton.Location = new System.Drawing.Point(1140, 8);
            this.downloadsButton.Name = "downloadsButton";
            this.downloadsButton.Size = new System.Drawing.Size(36, 28);
            this.downloadsButton.TabIndex = 13;
            this.downloadsButton.Text = "↓";
            this.downloadsButton.Click += new System.EventHandler(this.DownloadsButton_Click);
            // 
            // tabControl
            // 
            this.tabControl.Location = new System.Drawing.Point(0, 139);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1200, 534);
            this.tabControl.TabIndex = 5;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            this.tabControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TabControl_MouseClick);
            // 
            // suggestionsListBox
            // 
            this.suggestionsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.suggestionsListBox.ItemHeight = 25;
            this.suggestionsListBox.Location = new System.Drawing.Point(0, 0);
            this.suggestionsListBox.Name = "suggestionsListBox";
            this.suggestionsListBox.Size = new System.Drawing.Size(558, 138);
            this.suggestionsListBox.TabIndex = 0;
            this.suggestionsListBox.Click += new System.EventHandler(this.SuggestionsListBox_Click);
            this.suggestionsListBox.DoubleClick += new System.EventHandler(this.SuggestionsListBox_DoubleClick);
            // 
            // suggestionsPanel
            // 
            this.suggestionsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.suggestionsPanel.Controls.Add(this.suggestionsListBox);
            this.suggestionsPanel.Location = new System.Drawing.Point(422, 73);
            this.suggestionsPanel.Name = "suggestionsPanel";
            this.suggestionsPanel.Size = new System.Drawing.Size(560, 140);
            this.suggestionsPanel.TabIndex = 0;
            this.suggestionsPanel.Visible = false;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 668);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 32);
            this.statusStrip.TabIndex = 12;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(60, 25);
            this.statusLabel.Text = "Ready";
            // 
            // BrowserForm
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.suggestionsPanel);
            this.Controls.Add(this.toolbarPanel);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "BrowserForm";
            this.Text = "Umar Browser";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarPanel.PerformLayout();
            this.suggestionsPanel.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}