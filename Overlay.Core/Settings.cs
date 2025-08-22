using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlay.Core
{
    public class Settings
    {
        public double DefaultOpacity { get; set; } = 0.8;
        public bool IgnoreMouse { get; set; } = true;
        public bool SnapToEdges { get; set; } = true;
        public int GridSize { get; set; } = 10;
    }
}