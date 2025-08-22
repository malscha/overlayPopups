using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace Overlay.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private static bool _isCliMode = false;
    private static string[] _cliArgs = new string[0];

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        _cliArgs = e.Args;
        _isCliMode = _cliArgs.Length > 0;
        
        // Create a sample PNG image with transparency
        CreateSamplePng();
        
        if (_isCliMode)
        {
            // Run in CLI mode
            RunCliMode();
        }
        else
        {
            // Run in GUI mode
            var overlayWindow = new OverlayWindow();
            overlayWindow.Show();
        }
    }
    
    private void RunCliMode()
    {
        // Parse CLI arguments and display content
        // For now, we'll just show a simple message
        var overlayWindow = new OverlayWindow(_cliArgs);
        overlayWindow.Show();
        
        // Keep the application running
        System.Windows.Threading.Dispatcher.Run();
    }
    
    private void CreateSamplePng()
    {
        try
        {
            string assetsPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets");
            Directory.CreateDirectory(assetsPath);
            string pngPath = Path.Combine(assetsPath, "sample.png");
            
            // Always create or overwrite the image to ensure it exists
            // Create a larger bitmap (500x500)
            using (Bitmap bitmap = new Bitmap(500, 500))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // Fill with bright red background
                    g.Clear(Color.Red);
                    
                    // Draw a yellow border to make it more visible
                    using (Pen pen = new Pen(Color.Yellow, 20))
                    {
                        g.DrawRectangle(pen, 0, 0, 499, 499);
                    }
                    
                    // Draw some white text
                    using (Font font = new Font("Arial", 48, System.Drawing.FontStyle.Bold))
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        string text = "LARGE OVERLAY";
                        SizeF textSize = g.MeasureString(text, font);
                        float x = (500 - textSize.Width) / 2;
                        float y = (500 - textSize.Height) / 2;
                        g.DrawString(text, font, brush, x, y);
                    }
                }
                
                // Save as PNG
                bitmap.Save(pngPath, ImageFormat.Png);
            }
        }
        catch
        {
            // If we can't create the image, we'll just have to work without it
        }
    }
}

