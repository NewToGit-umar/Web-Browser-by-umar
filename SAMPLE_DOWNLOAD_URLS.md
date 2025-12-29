SAMPLE DOWNLOAD URLS FOR TESTING
=================================

## Quick Test Downloads

### Small Images (Fast Downloads)
```
https://via.placeholder.com/100x100.png
https://via.placeholder.com/200x200.jpg
https://httpbin.org/image/png
https://httpbin.org/image/jpeg
```

### Medium Files (5-50MB)
```
https://file-examples.com/storage/fe76ecf38fa10e3d2e99bfd/2017/10/file_example_MP3_700KB.mp3
https://file-examples.com/storage/fe76ecf38fa10e3d2e99bfd/2017/10/file_example_PDF_1MB.pdf
```

### Official Brand Images
```
https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png
https://www.python.org/static/community_logos/python-logo-master-v3-TM-flattened.png
```

### PDFs (Various Sizes)
```
https://www.w3.org/WAI/WCAG21/Techniques/pdf/PDF1.pdf
https://www.manualslib.com/download/1233322/The-C-Programming-Language-Second-Edition.html
https://ftp.ibiblio.org/pub/docs/books/gutenberg/cache/epub/98/pg98.pdf (Pride and Prejudice)
```

---

## Test Steps

### Test 1: Single Small Image Download
1. Copy URL: `https://via.placeholder.com/100x100.png`
2. Paste in address bar
3. Click "Go" or press Enter
4. Wait for file to download
5. Check Downloads folder for image file
6. Verify file is intact (can open it)

### Test 2: Multiple Concurrent Downloads
1. Open new tabs (Ctrl+T)
2. In each tab, navigate to different download URLs
3. Start downloads simultaneously
4. Observe Download Manager shows max 3 concurrent
5. Others queue automatically
6. Verify all files download successfully

### Test 3: Pause & Resume
1. Start downloading a larger file
2. Select it in Downloads Manager
3. Click "Pause" button
4. Wait 5 seconds
5. Click "Resume" button
6. Verify download continues from pause point

### Test 4: Download Speed Verification
1. Download a file
2. Note the speed shown (KB/s)
3. Speed should update every second
4. Higher speeds = good connection
5. Time remaining should decrease as download progresses

### Test 5: Cancel Download
1. Start downloading
2. Select download in list
3. Click "Cancel" button
4. Confirm action
5. Verify partial file is deleted
6. Status shows "Cancelled"

---

## Expected Results

### Download Success Indicators
? File appears in Downloads folder
? File has correct name from URL
? File size matches source
? File can be opened/used normally
? Download Manager shows "Completed"
? List item background turns light green

### Performance Indicators
? Download speed shows meaningful values
? Time remaining decreases during download
? Progress percentage increases smoothly
? No CPU spikes or freezing
? UI remains responsive

### Error Handling Verification
? Invalid URLs show error message
? Network errors trigger retries
? After 3 retries, shows failed status
? Cancelled downloads clean up partial files
? No crashes or unhandled exceptions

---

## Troubleshooting During Testing

### If Downloads Don't Start
1. Check internet connection (ping google.com)
2. Verify URL is correct and accessible
3. Try a different URL from this list
4. Check firewalls/antivirus isn't blocking
5. Ensure Downloads folder has write permission

### If Download Speed Shows 0
1. This is normal at start, wait 1-2 seconds
2. Speed updates after first second of data
3. If still 0 after 5 seconds, connection may be slow
4. Try a smaller file

### If Download Fails After Retries
1. Server may be down - try different file
2. File may have been deleted from source
3. Network may be unstable
4. Try again after waiting 30 seconds

### If UI Becomes Unresponsive
1. Don't panic - it may be temporary
2. Wait 10 seconds before action
3. Check if large files are downloading
4. Restart application if frozen > 30 seconds

---

## Performance Benchmarks

### Expected Download Times (on typical connection)

| File Size | Type | Expected Time |
|-----------|------|----------------|
| 100 KB | Image | 1-2 seconds |
| 500 KB | Image | 3-5 seconds |
| 1 MB | PDF | 5-10 seconds |
| 5 MB | Audio | 10-20 seconds |
| 10 MB | Video | 20-40 seconds |

**Note:** Actual times depend on:
- Your internet speed
- Server speed
- Network congestion
- File compression

---

## Network Diagnostics

### Check Your Connection Speed
```
Download a known file size and note time:
Speed = File Size (bytes) / Time (seconds) / 1024 = KB/s

Example:
1 MB (1,048,576 bytes) downloaded in 10 seconds
= 1,048,576 / 10 / 1024 = ~102 KB/s
```

### Typical Download Speeds
- Slow (< 100 KB/s): 2G/3G, poor WiFi
- Medium (100-500 KB/s): Standard WiFi, 4G
- Fast (500+ KB/s): Fiber, good WiFi, 5G
- Very Fast (1000+ KB/s): Gigabit connection

---

## Test Results Template

Use this to record your tests:

```
Date: _________
Browser Version: _________
System: _________

Test 1: Single File Download
URL: _________
File: _________
Size: _________ bytes
Time: _________ seconds
Speed: _________ KB/s
Result: [ ] Pass [ ] Fail
Notes: _________

Test 2: Multiple Downloads
Files Started: _________
Concurrent Count: _________
All Completed: [ ] Yes [ ] No
Result: [ ] Pass [ ] Fail
Notes: _________

Test 3: Pause/Resume
File: _________
Paused After: _________ seconds
Resumed: [ ] Yes [ ] No
Completed: [ ] Yes [ ] No
Result: [ ] Pass [ ] Fail
Notes: _________

Overall Result: [ ] Pass [ ] Fail
Issues Found: _________
```

---

## Common File Types & Extensions

### Supported by Download Manager
? Images: .png, .jpg, .jpeg, .gif, .bmp
? Documents: .pdf, .doc, .docx, .xls, .xlsx
? Audio: .mp3, .wav, .m4a, .flac
? Video: .mp4, .avi, .mkv, .webm
? Archives: .zip, .rar, .7z
? Installers: .exe, .msi, .dmg

### Large Test Files
For testing larger downloads, look for files in these ranges:
- 10-50 MB: Medium test
- 50-100 MB: Large test
- 100-500 MB: Very large test

---

## Recommendations for Comprehensive Testing

1. **Test Different File Types**
   - At least one image
   - At least one document
   - At least one archive if possible

2. **Test Different Sizes**
   - Small (< 1 MB)
   - Medium (1-10 MB)  
   - Large (> 10 MB)

3. **Test Different Scenarios**
   - Single download
   - Multiple concurrent
   - Pause/resume
   - Cancel
   - Failed retry

4. **Test Edge Cases**
   - Invalid URL
   - Slow server
   - Interrupted connection (if possible)
   - Very fast server
   - Very slow server

5. **Monitor Performance**
   - CPU usage
   - Memory usage
   - Disk I/O
   - Network usage

---

## Still Having Issues?

1. Check **Download_Testing_Guide.md** for detailed test procedures
2. Check **DownloadManager_README.md** for feature documentation
3. Review **crash.log** in application directory for errors
4. Ensure application rebuilt with latest code
5. Try on different network connection
6. Check Windows Defender isn't blocking downloads

---

**Last Updated:** 2025
**Status:** Ready for Testing
**Download Manager Version:** 1.0

