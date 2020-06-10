using System;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Ayay.Properties;
using System.Windows.Controls;

namespace Ayay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon NotifyIcon { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            //load icon
            NotifyIcon = new NotifyIcon()
            {
                Icon = Ayay.Properties.Resources.favicon,
                Visible = false,
                Text = "This is some test right here",
            };

            //Window Event Handler
            StateChanged += StateChangedEvent;

            //Tray Icon Handlers
            NotifyIcon.DoubleClick += TrayIconDoubleClick;

            //Start App minimized
            MinimizeAyAy();
        }

        #region Window State Change Methods
        /// <summary>
        /// Depending on the State of the window, either minimize it or restore it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateChangedEvent(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Minimized:
                    MinimizeAyAy();
                    break;
                default:
                    RestoreAyAy();
                    break;
            }
        }

        /// <summary>
        /// Change Window to Minimized, Remove Icon From Taskbar, and show icon in the tray
        /// </summary>
        private void MinimizeAyAy()
        {
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
            NotifyIcon.Visible = !ShowInTaskbar;
        }

        /// <summary>
        /// Restore the Window, Add Icon to Taskbar, and remove icon from the tray
        /// </summary>
        private void RestoreAyAy()
        {
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            NotifyIcon.Visible = !ShowInTaskbar;
        }
        #endregion

        #region Tray Icon Click Events
        /// <summary>
        /// Restore Window when the Tray Icon is double-clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TrayIconDoubleClick(object sender, EventArgs e) => WindowState = WindowState.Normal;
        #endregion

        /// <summary>
        /// Save Settings Before Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Ayay.Properties.Settings.Default.Save();
        }
    }
}
