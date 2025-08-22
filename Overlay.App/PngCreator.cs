using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Overlay.App
{
    public class PngCreator
    {
        public static void CreateSamplePng(string filePath)
        {
            // Create a new bitmap with transparency
            using (Bitmap bitmap = new Bitmap(100, 100))
            {
                // Make it transparent
                bitmap.MakeTransparent();
                
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // Draw a semi-transparent red circle
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
                    {
                        g.FillEllipse(brush, 10, 10, 80, 80);
                    }
                    
                    // Draw a blue rectangle with transparency
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 0, 0, 255)))
                    {
                        g.FillRectangle(brush, 20, 20, 60, 60);
                    }
                }
                
                // Save as PNG
                bitmap.Save(filePath, ImageFormat.Png);
            }
        }
    }
}