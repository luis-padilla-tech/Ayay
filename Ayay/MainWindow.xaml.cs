using System;
using System.Media;
using System.Windows;
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
        private Timer ShortBreakTimer { get; set; }
        private Timer LongBreakTimer { get; set; }
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            IsPaused = Properties.Settings.Default.IsPaused;

            StartupAyay();
        }
        #endregion

        #region Startup Methods

        private void StartupAyay()
        {
            //load icon
            CreateNotifyIcon();
            ChangeButtonText();
            //Window Event Handler
            StateChanged += StateChangedEvent;
            //Start App minimized
            MinimizeAyAy();

            CreateShortBreakIntervalTimer();
            Create
        }
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

        #region Tray Icon Dispolay Methods

        
        private void CreateNotifyIcon()
        {
            NotifyIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.favicon,
                Visible = false,
                BalloonTipTitle = "Break Notification",
                Text = "This is some test right here",
            };

            NotifyIcon.DoubleClick += TrayIconDoubleClick;
            NotifyIcon.BalloonTipClosed += NotifyIcon_BalloonTipClosed;
        }
        private void ShowNotification(string balloonText)
        {
            NotifyIcon.BalloonTipText = balloonText;
            NotifyIcon.Visible = true;
            NotifyIcon.ShowBalloonTip(1000);
        }

        private void NotifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            NotifyIcon.Visible = WindowState == WindowState.Minimized;
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
            (IsPaused ? (Action<Timer>)StopTimer : StartTimer)(ShortBreakTimer);
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

        private void CreateShortBreakIntervalTimer() => ShortBreakTimer = CreateTimer(ConvertMinutesToMilliseconds(Properties.Settings.Default.ShortBreakInterval), ShortBreakNotificationEventStart);

        private void CreateShortBreakNotification() => ShortBreakTimer = CreateTimer(ConvertSecondsToMilliseconds(Properties.Settings.Default.ShortBreakNotification), ShortBreakNotificationEventEnd);

        private void CreateLongBreakInterval() => LongBreakTimer = CreateTimer(ConvertMinutesToMilliseconds(Properties.Settings.Default.LongBreakInterval), LongBreakNotificationEventStart);

        private void CreateLongBreakNotification() => LongBreakTimer = CreateTimer(ConvertMinutesToMilliseconds(Properties.Settings.Default.LongBreakNotification, ));

        #endregion

        #region Timer Events

        private void ShortBreakNotificationEventStart(object sender, EventArgs e)
        {
            StopTimer(ShortBreakTimer);
            CreateShortBreakNotification();
            ShowNotification("Short Break has started");
            StartTimer(ShortBreakTimer);
        }


        private void ShortBreakNotificationEventEnd(object sender, EventArgs e)
        {
            StopTimer(ShortBreakTimer);
            ShowNotification("Short Break has ended");
            CreateShortBreakIntervalTimer();
            StartTimer(ShortBreakTimer);
        }


        private void LongBreakNotificationEventStart(object sender, EventArgs e)
        {

        }


        private void LongBreakNotificationEventEnd(object sender, EventArgs e)
        {

        }

        #endregion

        #region Time Converter Methods
        /// <summary>
        /// Converts a Given minute to milliseconds
        /// </summary>
        /// <param name="minutes">Miutes wanting to convert to milliseconds</param>
        /// <returns></returns>
        private int ConvertMinutesToMilliseconds(int minutes) => ConvertSecondsToMilliseconds(minutes * 60);

        /// <summary>
        /// Convert a given amount of seconds to milliseconds
        /// </summary>
        /// <param name="seconds">Seconds wanting to convert to milliseconds</param>
        /// <returns></returns>
        private int ConvertSecondsToMilliseconds(int seconds) => seconds * 1000;
        #endregion

    }
}
