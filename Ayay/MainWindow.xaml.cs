using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Ayay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        private bool IsPaused { get; set; }
        private NotifyIcon NotifyIcon { get; set; }
        private Timer ShortBreakIntervalTimer { get; set; }
        private Timer ShortBreakNotificationTimer { get; set; }
        private Timer LongBreakIntervalTimer { get; set; }
        private Timer LongBreakNotificationTimer { get; set; }
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            //load icon
            NotifyIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.favicon,
                Visible = false,
                Text = "This is some test right here",
            };

            IsPaused = Properties.Settings.Default.IsPaused;
            ChangeButtonText();

            //Window Event Handler
            StateChanged += StateChangedEvent;

            //Tray Icon Handlers
            NotifyIcon.DoubleClick += TrayIconDoubleClick;

            //Start App minimized
            MinimizeAyAy();
        }
        #endregion

        #region Startup Methods
        #endregion

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

        /// <summary>
        /// Save Settings Before Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Ayay.Properties.Settings.Default.Save();
        }
        #endregion

        #region Tray Icon Click Events
        /// <summary>
        /// Restore Window when the Tray Icon is double-clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TrayIconDoubleClick(object sender, EventArgs e) => WindowState = WindowState.Normal;

        public void TrayIconSingleClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void ChangeButtonText()
        {
            btn_Timer.Content = IsPaused ? "Start" : "Pause";
        }

        private void Btn_Timer_Click(object sender, RoutedEventArgs e)
        {
            IsPaused = !IsPaused;
            ChangeButtonText();
            (IsPaused ? (Action<Timer>)StopTimer : StartTimer)(ShortBreakIntervalTimer);
        }

        private void ShortBreakNotificationTimerEvent(object sender, EventArgs e)
        {
            SystemSounds.Beep.Play();
            StopTimer(ShortBreakNotificationTimer);
        }

        /// <summary>
        /// Enables a timer
        /// </summary>
        /// <param name="timer"></param>
        private void StartTimer(Timer timer)
        {
            timer.Enabled = true;
        }

        /// <summary>
        /// Disables a timer
        /// </summary>
        /// <param name="timer"></param>
        private void StopTimer(Timer timer)
        {
            timer.Enabled = false;
        }

        #region Timer Creation

        /// <summary>
        /// Create and Return a disabled Timer with a set interval and an event handle added to it 
        /// </summary>
        /// <param name="interval">How often for an event to occur in milliseconds</param>
        /// <param name="timerEvent">The event that occurs when the timer ticks</param>
        /// <returns></returns>
        private Timer CreateTimer(int interval, EventHandler timerEvent)
        {
            Timer timer = new Timer
            {
                Enabled = false,
                Interval = interval,
            };

            timer.Tick += timerEvent;

            return timer;
        }

        private void CreateShortBreakIntervalTimer() => ShortBreakIntervalTimer = CreateTimer();


        private void CreateShortBreakNotification() => ShortBreakNotificationTimer = CreateTimer();


        private void CreateLongBreakInterval() => LongBreakIntervalTimer = CreateTimer();

        private void CreateLongBreakNotification() => LongBreakNotificationTimer => CreateTimer();

        #endregion

        #region Timer Events
        #endregion

        #region Time Converter Methods
        /// <summary>
        /// Converts a Given minute to milliseconds
        /// </summary>
        /// <param name="minutes">Miutes wanting to convert to milliseconds</param>
        /// <returns></returns>
        private int ConvertMinutesToMilli(int minutes) => ConvertSecondsToMilli(minutes * 60);

        /// <summary>
        /// Convert a given amount of seconds to milliseconds
        /// </summary>
        /// <param name="seconds">Seconds wanting to convert to milliseconds</param>
        /// <returns></returns>
        private int ConvertSecondsToMilli(int seconds) => seconds * 1000;
        #endregion

    }
}
