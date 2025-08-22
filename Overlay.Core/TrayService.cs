using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Overlay.Core
{
    public class TrayService : ITrayService
    {
        private NotifyIcon? _notifyIcon;
        private ApplicationContext? _appContext;

        public void Show()
        {
            if (_notifyIcon == null)
            {
                InitializeTrayIcon();
            }

            _notifyIcon?.ShowBalloonTip(3000, "Overlay App", "Application is running in the background", ToolTipIcon.Info);
        }

        public void Hide()
        {
            // No direct method to hide balloon tip, but we can just not show it
        }

        public void SetIcon(string iconPath)
        {
            if (_notifyIcon == null)
            {
                InitializeTrayIcon();
            }

            try
            {
                _notifyIcon!.Icon = new Icon(iconPath);
            }
            catch
            {
                // If we can't load the icon, use the default
                _notifyIcon!.Icon = SystemIcons.Application;
            }
        }

        public void SetContextMenu(Dictionary<string, Action> menuItems)
        {
            if (_notifyIcon == null)
            {
                InitializeTrayIcon();
            }

            var contextMenu = new ContextMenuStrip();

            foreach (var item in menuItems)
            {
                var menuItem = new ToolStripMenuItem(item.Key);
                menuItem.Click += (sender, e) => item.Value();
                contextMenu.Items.Add(menuItem);
            }

            _notifyIcon!.ContextMenuStrip = contextMenu;
        }

        public void ShowNotification(string title, string message, int durationMs = 3000)
        {
            if (_notifyIcon == null)
            {
                InitializeTrayIcon();
            }

            _notifyIcon!.ShowBalloonTip(durationMs, title, message, ToolTipIcon.Info);
        }

        private void InitializeTrayIcon()
        {
            _appContext = new ApplicationContext();
            _notifyIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "Overlay Application"
            };

            // Add a default context menu with an Exit option
            var contextMenu = new ContextMenuStrip();
            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (sender, e) => System.Windows.Forms.Application.Exit();
            contextMenu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = contextMenu;

            // Handle the balloon tip click to show the application
            _notifyIcon.BalloonTipClicked += (sender, e) => 
            {
                // This would be where we show/hide the overlay window
            };
        }
    }
}