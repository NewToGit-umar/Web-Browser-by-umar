Umar Browser — Lightweight WinForms Browser
==========================================

A small, focused Windows Forms web browser built for learning and experimentation. It combines a simple tabbed UI, an embedded `WebBrowser` control, and a built-in download manager so you can explore embedding web content and handling downloads in a desktop app.

What this project is for
- Educational/demo app to show how to embed a browser control and interact with the page DOM.
- A compact example of a downloads pipeline (queue, progress, pause/resume, cancel).
- Useful starting point if you want to prototype a custom WinForms browser UI.

Key features
- Tabbed browsing: open, close and switch tabs. Basic tab lifecycle handling and a small tab manager.
- Embedded browsing: pages render in the WinForms `WebBrowser` control (IE engine on Windows).
- Download manager:
  - Detects common downloadable links and can start downloads from the page or from a pasted URL.
  - Shows progress, estimated time remaining, download speed and start time.
  - Pause / resume / cancel operations and a simple list view UI.
  - Concurrent downloads with a small queue (configurable in code).
- Page instrumentation: injects a tiny script to capture link clicks and forward them to the native download manager.
- Right-click link download: right-click a link to immediately download and open the Downloads window.
- Search suggestions: a lightweight trie-based suggestions structure for the address/search box.
- Simple preferences persistence (home page, theme, search engine selection).

Technologies
- Language: C# 7.3
- Target: .NET Framework 4.7.2
- UI: Windows Forms (WinForms)
- Embedded browser: `System.Windows.Forms.WebBrowser` control (Internet Explorer engine)
- Networking: `System.Net.WebClient` for downloads

Project scope and limitations
- This project is a demonstration and is not intended to replace modern browsers. The embedded engine is Internet Explorer–based and will not behave like Chromium/Edge on many modern sites.
- Downloads are implemented using `WebClient` and are aimed at direct file URLs. Sites that use complex JavaScript download flows (blobs, XHR-generated content) may not be captured by the heuristic.
- The app is single-user, desktop-only, and not hardened for running untrusted web content. Use caution with unsafe sites.

Getting started (build and run)
1. Open the solution in Visual Studio 2019/2022.
2. Ensure the machine has .NET Framework 4.7.2 installed.
3. Build the solution and run the `Web Browser` project.

Quick usage
- Enter a URL or search term in the address bar and press Enter.
- Use the New Tab button to open additional tabs.
- Right-click any link to start a download immediately — the Downloads window will open automatically.
- Open the Downloads window from the toolbar to monitor progress, pause/resume or cancel downloads.

Files of interest
- `Form1.cs` / `Form1.Designer.cs` — main UI and control logic.
- `DownloadManager.cs` — download queue, progress tracking and controls.
- `DownloadsForm.cs` — UI for download list and controls.
- `TabManager.cs` — lightweight linked structure to track tabs.
- `SearchSuggestionTree.cs` — trie implementation used for suggestions.

How to contribute
- Fork the repository and open a branch for a single, focused change.
- Make small commits with clear messages (feature/fix descriptions).
- Send a pull request describing the change and any manual test steps.

Ideas for improvements
- Replace `WebBrowser` with WebView2 (Chromium) for modern site compatibility.
- Implement true resume for partial downloads and range requests.
- Add bandwidth throttling and download scheduling.
- Improve UI/UX and accessibility.

Contact
programminghub7890@gmail.com

