using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlay.Core
{
    public interface ITrayService
    {
        void ShowNotification(string title, string message, int durationMs = 3000);
        void SetContextMenu(Dictionary<string, Action> menuItems);
        void SetIcon(string iconPath);
        void Show();
        void Hide();
    }
}