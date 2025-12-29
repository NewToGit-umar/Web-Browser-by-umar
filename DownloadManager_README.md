Download Manager - Feature Documentation
==========================================

## Overview
The Umar Browser includes a comprehensive Download Manager that handles file downloads with advanced features including concurrent downloads, pause/resume, automatic retries, and real-time progress tracking.

## Key Features

### 1. Concurrent Downloads
- Supports up to 3 simultaneous downloads
- Queues additional downloads automatically
- Maximizes bandwidth utilization without overwhelming the system

### 2. Pause & Resume
- Pause any active download
- Resume paused downloads at any time
- Maintain download progress during pause/resume cycles

### 3. Automatic Retry Logic
- Automatic retry on download failure (up to 3 attempts)
- 1-second delay between retries to allow server recovery
- Clear retry status display to user

### 4. Real-Time Progress Tracking
- Live download progress percentage
- Download speed (KB/s)
- Estimated time remaining (HH:mm:ss format)
- Total file size display

### 5. Download Status Colors
- **Light Blue**: Downloading in progress
- **Khaki**: Retrying after failure
- **Light Yellow**: Paused or Cancelled
- **Light Coral**: Failed or Error
- **Light Green**: Completed successfully

### 6. User-Friendly UI
- Detailed ListView with columns:
  - File Name
  - Status
  - Progress %
  - Total Size
  - Current Speed
  - Time Remaining
  - Start Time
- Buttons: Open Folder, Pause/Resume, Cancel, Clear Completed

## Technical Implementation

### DownloadItem Class
```csharp
public class DownloadItem
{
    public string Url { get; set; }
    public string FileName { get; set; }
    public string SavePath { get; set; }
    public long BytesReceived { get; set; }
    public long TotalBytes { get; set; }
    public double ProgressPercentage { get; }
    public string Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsCompleted { get; }
    public double DownloadSpeed { get; set; }
    public string TimeRemaining { get; set; }
    public int RetryCount { get; set; }
}
```

### DownloadManager Class
Main class responsible for:
- Managing download queue
- Handling concurrent downloads (max 3)
- Tracking progress and completion
- Calculating speed and time estimates
- Providing pause/resume/cancel functionality

## How to Use

### Starting a Download
```csharp
downloadManager.StartDownload(url, suggestedFileName);
```

### Pausing a Download
```csharp
downloadManager.PauseDownload(downloadItem);
```

### Resuming a Download
```csharp
downloadManager.ResumeDownload(downloadItem);
```

### Canceling a Download
```csharp
downloadManager.CancelDownload(downloadItem);
```

### Canceling All Downloads
```csharp
downloadManager.CancelAllDownloads();
```

### Opening Download Folder
```csharp
downloadManager.OpenDownloadFolder();
```

## Download Folder
By default, files are saved to:
```
[Application Directory]\Downloads\
```
The directory is created automatically if it doesn't exist.

## Filename Handling
- Automatically extracts filename from URL
- Generates unique filenames if files already exist (appends counter)
- Format: `filename(1).ext`, `filename(2).ext`, etc.

## Event Handling
The manager provides two events:

```csharp
public event EventHandler<DownloadItem> DownloadProgressChanged;
public event EventHandler<DownloadItem> DownloadCompleted;
```

### Progress Event
Fired during download progress:
```csharp
downloadManager.DownloadProgressChanged += (sender, item) =>
{
    Console.WriteLine($"{item.FileName}: {item.ProgressPercentage:F1}% - {item.DownloadSpeed:F2} KB/s");
};
```

### Completion Event
Fired when download completes (success, failure, or cancellation):
```csharp
downloadManager.DownloadCompleted += (sender, item) =>
{
    Console.WriteLine($"{item.FileName}: {item.Status}");
};
```

## Error Handling
- Network errors: Automatic retry (up to 3 times)
- Invalid URLs: Caught and reported to user
- File system errors: Display error message
- Cancelled downloads: File cleaned up automatically

## Network Configuration
The download manager uses:
- User-Agent: Mozilla/5.0 (Chrome-like string)
- Timeout: Default WebClient settings
- Headers: Standard HTTP headers with user agent

## Performance Considerations
- Concurrent downloads limited to 3 to avoid overwhelming network
- Download speed calculated every second
- Time remaining estimates update continuously
- Efficient ListView updates via threading

## Troubleshooting

### Downloads Not Starting
- Check internet connection
- Verify URL is valid
- Check available disk space in Downloads folder

### Download Speed is Slow
- Check network bandwidth
- Ensure no other large transfers in progress
- Try resuming the download

### Download Fails After Retries
- Check if server is online
- Verify file still exists at URL
- Check firewall/proxy settings

## Future Enhancements
- Download resume from last position (partial download support)
- Multi-threaded downloads per file
- Download scheduling
- Bandwidth throttling
- Download history and statistics

