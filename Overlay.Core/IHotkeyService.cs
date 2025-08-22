using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlay.Core
{
    public interface IHotkeyService
    {
        void RegisterHotkey(string name, HotkeyCombination combination, Action callback);
        void UnregisterHotkey(string name);
        void UnregisterAllHotkeys();
    }

    public class HotkeyCombination
    {
        public HotkeyModifiers Modifiers { get; set; }
        public int Key { get; set; }
    }

    [Flags]
    public enum HotkeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}