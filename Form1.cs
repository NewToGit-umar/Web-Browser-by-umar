using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace AdvancedWebBrowser
{
    public partial class BrowserForm : Form
    {
        private UserPreferences prefs;
        private TabManager tabManager;
        private SearchSuggestionTree suggestionTree;
        private DownloadManager downloadManager;
        private WebBrowser currentBrowser;
        private Timer typingTimer;
        private string homePage = "https://www.google.com";

        // Use a modern user agent string to improve site compatibility
        private const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0 Safari/537.36";

        private bool suppressTabSelectionChanged = false;

        public BrowserForm()
        {
            InitializeComponent();
            // Force handle creation so SafeInvoke/BeginInvoke is safe
            var _ = this.Handle;

            prefs = UserPreferences.Load();

            ApplyTheme(prefs.Theme);

            InitializeBrowser();
            //TestUrlFormatting();

            // Apply prefs
            if (!string.IsNullOrWhiteSpace(prefs.HomePage))
                homePage = prefs.HomePage;
            if (searchEngineComboBox != null && prefs.SearchEngineIndex >= 0 && prefs.SearchEngineIndex < SearchUtilities.SearchEngineCount)
                searchEngineComboBox.SelectedIndex = prefs.SearchEngineIndex;
            if (forceSearchCheckBox != null)
                forceSearchCheckBox.Checked = prefs.ForceSearch;
        }

        private void ApplyTheme(string theme)
        {
            if (string.Equals(theme, "Dark", StringComparison.OrdinalIgnoreCase))
            {
                this.BackColor = Color.FromArgb(34, 34, 34);
                toolbarPanel.BackColor = Color.FromArgb(45, 45, 48);
                statusStrip.BackColor = Color.FromArgb(45, 45, 48);
                this.ForeColor = Color.WhiteSmoke;
            }
            else
            {
                this.BackColor = Color.WhiteSmoke;
                toolbarPanel.BackColor = Color.FromArgb(245, 247, 250);
                statusStrip.BackColor = SystemColors.Control;
                this.ForeColor = Color.Black;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Save preferences
            if (prefs == null) prefs = new UserPreferences();
            prefs.HomePage = homePage;
            if (searchEngineComboBox != null) prefs.SearchEngineIndex = searchEngineComboBox.SelectedIndex;
            if (forceSearchCheckBox != null) prefs.ForceSearch = forceSearchCheckBox.Checked;
            prefs.Save();

            base.OnFormClosing(e);
        }

        private void InitializeBrowser()
        {
            // Initialize data structures
            tabManager = new TabManager();
            suggestionTree = new SearchSuggestionTree();
            downloadManager = new DownloadManager();

            // Initialize typing timer for delayed search
            typingTimer = new Timer();
            typingTimer.Interval = 300; // 300ms delay
            typingTimer.Tick += TypingTimer_Tick;

            // Subscribe to tab change event ONCE here (not in CreateNewTab!)
            tabManager.TabChanged += TabManager_TabChanged;

            // Pre-populate with some suggestions
            PrepopulateSuggestions();

            // Ensure default search engine selection
            if (searchEngineComboBox != null && searchEngineComboBox.Items.Count > 0)
                searchEngineComboBox.SelectedIndex = 0;

            // Create initial tab
            CreateNewTab("Home", homePage);
        }

        // small hover effect helper for toolbar buttons
        private void AddHoverEffect(Control c)
        {
            c.MouseEnter += (s, e) => c.BackColor = Color.FromArgb(230, 230, 230);
            c.MouseLeave += (s, e) => c.BackColor = SystemColors.Control;
        }

        private void CreateNewTab(string title, string url = "about:blank")
        {
            var tabPage = new TabPage(title);
            var browser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true,
                AllowNavigation = true
            };

            // Browser events
            browser.DocumentTitleChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(browser.DocumentTitle))
                {
                    tabPage.Text = browser.DocumentTitle.Length > 15 ?
                        browser.DocumentTitle.Substring(0, 15) + "..." :
                        browser.DocumentTitle;
                }
                UpdateCurrentTabNode();
                UpdateNavigationButtons();
            };

            browser.Navigated += (s, e) =>
            {
                SafeInvoke(() =>
                {
                    if (e.Url != null)
                    {
                        addressBar.Text = e.Url.ToString();
                        UpdateCurrentTabNode();
                        if (!string.IsNullOrEmpty(e.Url.Host))
                            suggestionTree.IncrementSearchCount(e.Url.Host);
                    }
                    UpdateNavigationButtons();
                    UpdateStatus("Ready");
                });
            };

            browser.Navigating += (s, e) =>
            {
                SafeInvoke(() =>
                {
                    var host = e.Url != null ? e.Url.Host : string.Empty;
                    UpdateStatus($"Loading {host}...");
                });
            };

            browser.ProgressChanged += (s, e) =>
            {
                SafeInvoke(() =>
                {
                    if (e.CurrentProgress > 0 && e.MaximumProgress > 0)
                    {
                        int progress = (int)((e.CurrentProgress * 100) / e.MaximumProgress);
                        UpdateStatus($"Loading... {progress}%");
                    }
                });
            };

            browser.DocumentCompleted += (s, e) =>
            {
                SafeInvoke(() =>
                {
                    UpdateStatus("Page loaded");

                    // Inject script bridge to capture download clicks and forward to host
                    try
                    {
                        // expose C# object to JS
                        browser.ObjectForScripting = new ScriptManager(downloadManager, this);

                        if (browser.Document != null)
                        {
                            try
                            {
                                var head = browser.Document.GetElementsByTagName("head").OfType<HtmlElement>().FirstOrDefault();
                                if (head != null)
                                {
                                    var scriptEl = browser.Document.CreateElement("script");
                                    var scriptElem = (HtmlElement)scriptEl;
                                    scriptElem.SetAttribute("type", "text/javascript");
                                    string scriptText = @"(function(){
                                        document.addEventListener('click', function(e){
                                            var target = e.target || e.srcElement;
                                            while(target && target.tagName !== 'A'){
                                                target = target.parentElement;
                                            }
                                            if(target && target.tagName === 'A'){
                                                var href = target.getAttribute('href');
                                                if(href){
                                                    var parts = href.split('?')[0].split('.');
                                                    var ext = parts.length>1 ? parts.pop().toLowerCase() : '';
                                                    var downloadable = ['exe','zip','rar','7z','msi','pdf','doc','docx','xls','xlsx','ppt','pptx','mp3','wav','mp4','avi','mkv','jpg','jpeg','png','gif','bmp','torrent','iso','dmg'];
                                                    if(downloadable.indexOf(ext) !== -1){
                                                        try{ e.preventDefault(); }catch(e){}
                                                        try{ window.external.Download(href); }catch(e){}
                                                    }
                                                }
                                            }
                                        }, true);
                                    })();";
                                    scriptElem.InnerText = scriptText;
                                    head.AppendChild(scriptElem);
                                }
                            }
                            catch { /* ignore DOM injection errors */ }

                            // Attach right-click handler once per browser to support "Download link" via context click
                            try
                            {
                                // Use Tag to mark attached handlers and avoid duplicates
                                var tag = browser.Tag as string ?? string.Empty;
                                if (!tag.Contains("ctx-attached"))
                                {
                                    // disable default context menu so our action is used
                                    browser.IsWebBrowserContextMenuEnabled = false;

                                    HtmlElementEventHandler rightClickHandler = null;
                                    rightClickHandler = (snd, evArgs) =>
                                    {
                                        try
                                        {
                                            if (evArgs.MouseButtonsPressed == MouseButtons.Right)
                                            {
                                                var pt = evArgs.ClientMousePosition;
                                                var el = browser.Document.GetElementFromPoint(pt);
                                                HtmlElement anchor = null;
                                                var cur = el;
                                                while (cur != null)
                                                {
                                                    if (string.Equals(cur.TagName, "A", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        anchor = cur;
                                                        break;
                                                    }
                                                    cur = cur.Parent;
                                                }

                                                var href = anchor?.GetAttribute("href");
                                                if (!string.IsNullOrWhiteSpace(href))
                                                {
                                                    try
                                                    {
                                                        // start download
                                                        string suggested = null;
                                                        try { suggested = Path.GetFileName(new Uri(href).LocalPath); } catch { }
                                                        if (string.IsNullOrWhiteSpace(suggested))
                                                            suggested = "download_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                                                        downloadManager.StartDownload(href, suggested);
                                                    }
                                                    catch { }

                                                    try
                                                    {
                                                        // show downloads form
                                                        var existing = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f is DownloadsForm);
                                                        if (existing != null)
                                                        {
                                                            if (existing.WindowState == FormWindowState.Minimized)
                                                                existing.WindowState = FormWindowState.Normal;
                                                            existing.BringToFront();
                                                            existing.Activate();
                                                        }
                                                        else
                                                        {
                                                            var df = new DownloadsForm(downloadManager);
                                                            df.Show(this);
                                                        }
                                                    }
                                                    catch { }

                                                    // prevent default context action/navigation
                                                    evArgs.ReturnValue = false;
                                                }
                                            }
                                        }
                                        catch { }
                                    };

                                    browser.Document.MouseDown += rightClickHandler;

                                    // mark as attached
                                    browser.Tag = (tag + ";ctx-attached");
                                }
                            }
                            catch { /* ignore mouse hook errors */ }
                        }
                    }
                    catch { }

                    try
                    {
                        var html = browser.DocumentText ?? string.Empty;
                        // Log URL and HTML length for debugging
                        try
                        {
                            var logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug_search.log");
                            var curUrl = browser.Url != null ? browser.Url.ToString() : string.Empty;
                            var entry = DateTime.Now.ToString("o") + " URL:" + curUrl + " Length:" + html.Length + Environment.NewLine;
                            System.IO.File.AppendAllText(logPath, entry);
                        }
                        catch { }

                        // consider page blank if too small or title empty
                        bool isBlankLike = string.IsNullOrWhiteSpace(browser.DocumentTitle) || html.Trim().Length < 500;

                        // detect simple security/certificate related words
                        bool hasSecurityWarning = html.IndexOf("certificate", StringComparison.OrdinalIgnoreCase) >= 0
                            || html.IndexOf("untrusted", StringComparison.OrdinalIgnoreCase) >= 0
                            || html.IndexOf("not secure", StringComparison.OrdinalIgnoreCase) >= 0
                            || html.IndexOf("security", StringComparison.OrdinalIgnoreCase) >= 0;

                        var hostName = browser.Url?.Host ?? string.Empty;

                        // Host-specific handling: YouTube often fails in old IE engine; open externally
                        if (hostName.IndexOf("youtube.com", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            // If page HTML is small or contains a message indicating unsupported browser, open externally
                            if (isBlankLike || html.IndexOf("To view this video", StringComparison.OrdinalIgnoreCase) >= 0 || html.IndexOf("unsupported", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                try
                                {
                                    UpdateStatus("Opening YouTube in external browser for full compatibility...");
                                    Process.Start(new ProcessStartInfo(browser.Url.ToString()) { UseShellExecute = true });
                                }
                                catch { }
                                return;
                            }
                        }

                        // Internet Archive (archive.org) sometimes shows mixed-content or TLS warnings in old engines
                        if (hostName.IndexOf("archive.org", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (isBlankLike || hasSecurityWarning)
                            {
                                try
                                {
                                    UpdateStatus("Opening Internet Archive in external browser due to compatibility/security issues...");
                                    Process.Start(new ProcessStartInfo(browser.Url.ToString()) { UseShellExecute = true });
                                }
                                catch { }
                                return;
                            }
                        }

                        // Handle modern sites that need external browser
                        if (SearchUtilities.IsModernSite(browser.Url?.ToString() ?? "") && isBlankLike)
                        {
                            try
                            {
                                UpdateStatus("Opening modern site in external browser for better compatibility...");
                                Process.Start(new ProcessStartInfo(browser.Url.ToString()) { UseShellExecute = true });
                                return;
                            }
                            catch { }
                        }

                        if (isBlankLike)
                        {
                            var u = browser.Url != null ? browser.Url.ToString() : string.Empty;
                            // attempt to extract search query
                            var q = GetQueryParameter(u, "q") ?? GetQueryParameter(u, "p") ?? string.Empty;
                            if (!string.IsNullOrWhiteSpace(q))
                            {
                                // If we haven't tried duckduckgo yet for this browser
                                if (browser.Tag == null || browser.Tag.ToString() != "ddg-fallback")
                                {
                                    browser.Tag = "ddg-fallback";
                                    var ddgUrl = "https://duckduckgo.com/html/?q=" + Uri.EscapeDataString(q);
                                    BrowserNavigate(browser, ddgUrl);
                                    return;
                                }

                                // If we haven't tried bing yet
                                if (browser.Tag == null || browser.Tag.ToString() != "bing-fallback")
                                {
                                    browser.Tag = "bing-fallback";
                                    var bingUrl = "https://www.bing.com/search?q=" + Uri.EscapeDataString(q);
                                    BrowserNavigate(browser, bingUrl);
                                    return;
                                }

                                // Already tried fallbacks, open external browser as last resort
                                try
                                {
                                    var finalUrl = browser.Url != null ? browser.Url.ToString() : addressBar.Text;
                                    if (!string.IsNullOrWhiteSpace(finalUrl))
                                    {
                                        UpdateStatus("Opening in external browser...");
                                        Process.Start(new ProcessStartInfo(finalUrl) { UseShellExecute = true });
                                    }
                                }
                                catch
                                {
                                    // ignore
                                }
                            }
                        }
                    }
                    catch
                    {
                        // ignore extraction errors
                    }
                });
            };

            // Handle file downloads
            browser.NewWindow += (s, e) =>
            {
                e.Cancel = true; // Prevent new windows
                try
                {
                    // Try to get the clicked/active element's href and navigate within the same browser
                    var doc = browser.Document;
                    if (doc != null)
                    {
                        var active = doc.ActiveElement;
                        string href = null;
                        if (active != null)
                        {
                            try
                            {
                                if (string.Equals(active.TagName, "A", StringComparison.OrdinalIgnoreCase))
                                {
                                    href = active.GetAttribute("href");
                                }
                                else
                                {
                                    var parent = active.Parent;
                                    while (parent != null && !string.Equals(parent.TagName, "A", StringComparison.OrdinalIgnoreCase))
                                    {
                                        parent = parent.Parent;
                                    }
                                    if (parent != null)
                                        href = parent.GetAttribute("href");
                                }
                            }
                            catch { href = null; }
                        }

                        if (!string.IsNullOrWhiteSpace(href))
                        {
                            // If it's a downloadable URL, use internal download manager
                            if (IsDownloadableUrl(href))
                            {
                                try { HandleFileDownload(href); }
                                catch { /* ignore download start failures */ }
                            }
                            else
                            {
                                // Navigate the same browser to keep navigation inside the tab
                                BrowserNavigate(browser, href);
                            }
                        }
                        else
                        {
                            // As a fallback, try to use status text or current URL
                            try
                            {
                                var fallback = browser.StatusText;
                                if (string.IsNullOrWhiteSpace(fallback))
                                    fallback = browser.Url?.ToString() ?? string.Empty;

                                if (!string.IsNullOrWhiteSpace(fallback))
                                {
                                    if (IsDownloadableUrl(fallback))
                                    {
                                        try { HandleFileDownload(fallback); } catch { }
                                    }
                                    else
                                    {
                                        BrowserNavigate(browser, fallback);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            };

            tabPage.Controls.Add(browser);
            tabControl.TabPages.Add(tabPage);
            // avoid triggering SelectedIndexChanged handling when programmatically selecting
            try
            {
                suppressTabSelectionChanged = true;
                tabControl.SelectedTab = tabPage;
            }
            finally
            {
                suppressTabSelectionChanged = false;
            }

            var tabNode = tabManager.AddTab(title, url, browser, tabPage);
            currentBrowser = browser;

            // Navigate to URL
            if (url != "about:blank")
            {
                try
                {
                    BrowserNavigate(browser, url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error navigating to {url}: {ex.Message}", "Navigation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // after creating browser and tab, add hover effects to its toolbar buttons
            AddHoverEffect(backButton);
            AddHoverEffect(forwardButton);
            AddHoverEffect(refreshButton);
            AddHoverEffect(homeButton);
            AddHoverEffect(goButton);
            AddHoverEffect(newTabButton);
            AddHoverEffect(closeTabButton);
            AddHoverEffect(downloadsButton);
        }

        private void TabManager_TabChanged(object sender, TabEventArgs e)
        {
            if (e.Tab != null)
            {
                SafeInvoke(() =>
                {
                    try
                    {
                        suppressTabSelectionChanged = true;
                        tabControl.SelectedTab = e.Tab.TabPage;
                    }
                    finally
                    {
                        suppressTabSelectionChanged = false;
                    }

                    currentBrowser = e.Tab.Browser;
                    addressBar.Text = e.Tab.Url ?? string.Empty;
                    UpdateNavigationButtons();
                });
            }
        }

        private void BrowserNavigate(WebBrowser browser, string url)
        {
            if (browser == null || string.IsNullOrWhiteSpace(url)) return;

            try
            {
                // Check if it's a downloadable file
                if (IsDownloadableUrl(url))
                {
                    HandleFileDownload(url);
                    return;
                }

                // Use default Navigate; adding custom headers (User-Agent) can break some sites in the WebBrowser control.
                browser.Navigate(url);
            }
            catch
            {
                // Fallback to default navigate again
                try { browser.Navigate(url); } catch { }
            }
        }

        private void HandleFileDownload(string url)
        {
            try
            {
                string fileName = Path.GetFileName(new Uri(url).LocalPath);
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = "download_" + DateTime.Now.ToString("yyyyMMdd_HHmms");
                }

                downloadManager.StartDownload(url, fileName);
                UpdateStatus($"Downloading: {fileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting download: {ex.Message}", "Download Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsDownloadableUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            string[] downloadExtensions = {
                ".exe", ".zip", ".rar", ".7z", ".msi",
                ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
                ".mp3", ".wav", ".mp4", ".avi", ".mkv",
                ".jpg", ".jpeg", ".png", ".gif", ".bmp",
                ".torrent", ".iso", ".dmg"
            };

            string lowerUrl = url.ToLower();
            return downloadExtensions.Any(ext => lowerUrl.EndsWith(ext));
        }

        private static string GetQueryParameter(string url, string param)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(param)) return null;
            try
            {
                var idx = url.IndexOf('?');
                if (idx < 0) return null;
                var query = url.Substring(idx + 1);
                var pairs = query.Split('&');
                foreach (var p in pairs)
                {
                    var parts = p.Split('=');
                    if (parts.Length >= 2 && parts[0].Equals(param, StringComparison.OrdinalIgnoreCase))
                    {
                        return Uri.UnescapeDataString(parts[1]);
                    }
                }
            }
            catch { }
            return null;
        }

        private void UpdateNavigationButtons()
        {
            if (currentBrowser != null)
            {
                if (backButton != null) backButton.Enabled = currentBrowser.CanGoBack;
                if (forwardButton != null) forwardButton.Enabled = currentBrowser.CanGoForward;
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusLabel != null && !statusLabel.IsDisposed)
            {
                statusLabel.Text = message;
            }
        }

        private void UpdateCurrentTabNode()
        {
            if (tabManager.CurrentTab != null && currentBrowser != null)
            {
                tabManager.CurrentTab.Title = currentBrowser.DocumentTitle ?? "New Tab";
                tabManager.CurrentTab.Url = currentBrowser.Url?.ToString() ?? "about:blank";
            }
        }

        private void SafeInvoke(Action action)
        {
            if (action == null) return;
            try
            {
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.BeginInvoke(action);
                }
                else
                {
                    // If handle not created yet, run action directly to avoid InvalidOperationException.
                    action();
                }
            }
            catch
            {
                // If BeginInvoke fails for any reason, fallback to direct invocation.
                try { action(); } catch { }
            }
        }

        private void NavigateToUrl(string url)
        {
            if (currentBrowser != null && !string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    int engineIndex = 0;
                    if (searchEngineComboBox != null && searchEngineComboBox.SelectedIndex >= 0)
                        engineIndex = searchEngineComboBox.SelectedIndex;

                    string searchUrl;

                    // If forceSearchCheckBox is checked, always perform a search using the selected search engine
                    if (forceSearchCheckBox != null && forceSearchCheckBox.Checked)
                    {
                        var clean = (url ?? string.Empty).Trim();
                        searchUrl = SearchUtilities.GetSearchEngineBase(engineIndex) + Uri.EscapeDataString(clean);
                    }
                    else
                    {
                        searchUrl = SearchUtilities.FormatSearchUrl(url, engineIndex);
                    }

                    UpdateStatus($"Navigating to {searchUrl}...");
                    BrowserNavigate(currentBrowser, searchUrl);
                    HideSuggestions();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error navigating: {ex.Message}", "Navigation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus("Navigation failed");
                }
            }
        }

        private void ShowSuggestions(List<string> suggestions)
        {
            if (suggestionsListBox.InvokeRequired)
            {
                suggestionsListBox.Invoke(new Action<List<string>>(ShowSuggestions), suggestions);
                return;
            }

            suggestionsListBox.Items.Clear();
            foreach (var suggestion in suggestions)
            {
                suggestionsListBox.Items.Add(suggestion);
            }

            suggestionsPanel.Visible = suggestions.Count > 0;
            suggestionsPanel.BringToFront();
        }

        private void HideSuggestions()
        {
            if (suggestionsPanel.InvokeRequired)
            {
                suggestionsPanel.Invoke(new Action(HideSuggestions));
                return;
            }

            suggestionsPanel.Visible = false;
        }

        // Enhanced Event Handlers
        private void NewTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewTab("New Tab");
            addressBar.Focus();
        }

        private void CloseTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl.TabPages.Count > 1 && tabControl.SelectedTab != null)
            {
                var currentPage = tabControl.SelectedTab;
                var tabNode = tabManager.FindByTabPage(currentPage);
                if (tabNode != null)
                {
                    tabManager.RemoveTab(tabNode);
                }

                // Dispose embedded WebBrowser to avoid leftover handles
                try
                {
                    var browser = currentPage.Controls.OfType<WebBrowser>().FirstOrDefault();
                    if (browser != null)
                    {
                        // detach events if needed
                        currentPage.Controls.Remove(browser);
                        browser.Dispose();
                    }
                }
                catch { }

                int removeIndex = tabControl.TabPages.IndexOf(currentPage);
                tabControl.TabPages.Remove(currentPage);

                // Select a sane tab after removal
                if (tabControl.TabPages.Count > 0)
                {
                    int newIndex = Math.Min(removeIndex, tabControl.TabPages.Count - 1);
                    tabControl.SelectedIndex = newIndex;
                    var newNode = tabManager.FindByTabPage(tabControl.SelectedTab);
                    if (newNode != null)
                        tabManager.SwitchToTab(newNode);
                }
            }
            else if (tabControl.TabPages.Count == 1)
            {
                Application.Exit();
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (currentBrowser != null && currentBrowser.CanGoBack)
            {
                try
                {
                    currentBrowser.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error going back: {ex.Message}", "Navigation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            if (currentBrowser != null && currentBrowser.CanGoForward)
            {
                try
                {
                    currentBrowser.GoForward();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error going forward: {ex.Message}", "Navigation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (currentBrowser != null)
            {
                try
                {
                    currentBrowser.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error refreshing: {ex.Message}", "Refresh Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            NavigateToUrl(homePage);
        }

        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            typingTimer.Stop();
            if (!string.IsNullOrEmpty(addressBar.Text))
            {
                var suggestions = suggestionTree.GetSuggestions(addressBar.Text);
                ShowSuggestions(suggestions);
            }
            else
            {
                HideSuggestions();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NextTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabManager.SwitchToNextTab();
        }

        private void PreviousTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabManager.SwitchToPreviousTab();
        }

        private void NewTabButton_Click(object sender, EventArgs e)
        {
            CreateNewTab("New Tab");
        }

        private void CloseTabButton_Click(object sender, EventArgs e)
        {
            CloseTabToolStripMenuItem_Click(sender, e);
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(addressBar.Text))
            {
                NavigateToUrl(addressBar.Text);
            }
        }

        private void DownloadsButton_Click(object sender, EventArgs e)
        {
            var downloadsForm = new DownloadsForm(downloadManager);
            downloadsForm.Show();
        }

        private void DownloadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadsButton_Click(sender, e);
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(addressBar.Text))
                {
                    NavigateToUrl(addressBar.Text);
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                HideSuggestions();
            }
            else if (e.KeyCode == Keys.Down && suggestionsPanel.Visible)
            {
                suggestionsListBox.Focus();
                if (suggestionsListBox.Items.Count > 0)
                    suggestionsListBox.SelectedIndex = 0;
            }
        }

        private void AddressBar_TextChanged(object sender, EventArgs e)
        {
            // Restart the timer on each keystroke
            typingTimer.Stop();
            typingTimer.Start();
        }

        private void SuggestionsListBox_Click(object sender, EventArgs e)
        {
            if (suggestionsListBox.SelectedItem != null)
            {
                addressBar.Text = suggestionsListBox.SelectedItem.ToString();
                NavigateToUrl(addressBar.Text);
                HideSuggestions();
            }
        }

        private void SuggestionsListBox_DoubleClick(object sender, EventArgs e)
        {
            SuggestionsListBox_Click(sender, e);
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!suppressTabSelectionChanged && tabControl.SelectedTab != null)
            {
                var selectedTabNode = tabManager.FindByTabPage(tabControl.SelectedTab);
                if (selectedTabNode != null)
                {
                    tabManager.SwitchToTab(selectedTabNode);
                }
            }
        }

        private void TabControl_MouseClick(object sender, MouseEventArgs e)
        {
            // Close tab on middle click
            if (e.Button == MouseButtons.Middle)
            {
                for (int i = 0; i < tabControl.TabPages.Count; i++)
                {
                    var rect = tabControl.GetTabRect(i);
                    if (rect.Contains(e.Location))
                    {
                        var tabPage = tabControl.TabPages[i];
                        var tabNode = tabManager.FindByTabPage(tabPage);
                        if (tabNode != null)
                        {
                            tabManager.RemoveTab(tabNode);
                        }

                        try
                        {
                            var browser = tabPage.Controls.OfType<WebBrowser>().FirstOrDefault();
                            if (browser != null)
                            {
                                tabPage.Controls.Remove(browser);
                                browser.Dispose();
                            }
                        }
                        catch { }

                        int removeIndex = i;
                        tabControl.TabPages.Remove(tabPage);

                        // select previous or first
                        if (tabControl.TabPages.Count > 0)
                        {
                            int newIndex = Math.Min(removeIndex, tabControl.TabPages.Count - 1);
                            tabControl.SelectedIndex = newIndex;
                            var newNode = tabManager.FindByTabPage(tabControl.SelectedTab);
                            if (newNode != null)
                                tabManager.SwitchToTab(newNode);
                        }
                        break;
                    }
                }
            }
        }

        private void PrepopulateSuggestions()
        {
            string[] commonSites = {
                "google.com", "github.com", "stackoverflow.com", "microsoft.com",
                "youtube.com", "facebook.com", "twitter.com", "linkedin.com",
                "wikipedia.org", "amazon.com", "reddit.com", "netflix.com",
                "chat.openai.com", "web.whatsapp.com", "gemini.google.com",
                "archive.org", "discord.com", "instagram.com", "spotify.com"
            };

            foreach (var site in commonSites)
            {
                suggestionTree.AddSuggestion(site);
            }
        }
    }
}