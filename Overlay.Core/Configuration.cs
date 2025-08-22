using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlay.Core
{
    public class Configuration
    {
        public string Version { get; set; } = "1.0";
        public Settings Settings { get; set; } = new Settings();
        public List<Card> Cards { get; set; } = new List<Card>();
        public List<Scene> Scenes { get; set; } = new List<Scene>();
    }
}