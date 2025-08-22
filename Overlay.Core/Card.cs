using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlay.Core
{
    public class Card
    {
        public string Id { get; set; } = string.Empty;
        public CardType Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Opacity { get; set; } = 1.0;
        public double Rotation { get; set; }
        public int ZIndex { get; set; }
        public FontInfo? Font { get; set; }
        public string? ForegroundColor { get; set; }
        public string? BackgroundColor { get; set; }
        public string? OutlineColor { get; set; }
        public double OutlineThickness { get; set; } = 0;
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;
        public string? ImagePath { get; set; }
        public string? Text { get; set; }
        public bool IsStrobing { get; set; } = false;
        public string? DropShadowColor { get; set; }
        public int DisplayIndex { get; set; } = 0; // 0 = primary, 1 = secondary, etc.
        
        // New properties for text effects
        public bool HasOutline { get; set; } = false;
        public bool HasGlow { get; set; } = true; // Default to true
        public bool UseZoomAnimation { get; set; } = false;
        public bool IsPulsating { get; set; } = false;
        public string? PulseSpeed { get; set; } // "fast", "medium", "slow"
        public TextSize TextSize { get; set; } = TextSize.Medium; // Default to medium
    }

    public enum CardType
    {
        Text,
        Image
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }

    public class FontInfo
    {
        public string Family { get; set; } = "Impact";
        public double Size { get; set; } = 72; // Default to medium size
        public string Weight { get; set; } = "Normal"; // Normal, Bold, etc.
        public string Style { get; set; } = "Normal"; // Normal, Italic, etc.
    }

    public enum TextSize
    {
        Small,
        Medium,
        Large
    }
}