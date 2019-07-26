﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace mouseclick_looper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer1 = new DispatcherTimer();
                
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public MainWindow()
        {
            InitializeComponent();
            GetSettings();
            timer1.Tick += timer1_Tick;
            var times = GetSettings();
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.ResizeMode = ResizeMode.CanMinimize;
            txtMins.Text = times.Mins.ToString();
            txtSecs.Text = times.Secs.ToString();
        }

        public void DoMouseClick()
        {
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);
            int X = (int) pointToScreen.X;
            int Y = (int) pointToScreen.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
        private void cmdOK_Click(object sender, EventArgs e)
        {
            float duration = 0;
            int seconds = 0;

            if (txtMins.Text != "")
            {
                duration = (float.Parse(txtMins.Text) * 60) * 1000;
            }

            if (txtSecs.Text != "")
            {
                duration += float.Parse(txtSecs.Text) * 1000;
            }
            duration += 1000;

            seconds = (int)duration / 1000;

            timer1.Interval = TimeSpan.FromSeconds(seconds);

            DisplayStartedTime();

            cmdOk.IsEnabled = false;

            SaveSettings(
                            new AppTimes { Mins = int.Parse(txtMins.Text == "" ? "0" : txtMins.Text), Secs = int.Parse(txtSecs.Text) }
                        );

            timer1.Start();

        }

        private void DisplayStartedTime()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.Title = "Started " + String.Format("{0:T}", DateTime.Now) + " ...";
            grdMain.Background = Brushes.Green;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DoMouseClick();
            DisplayStartedTime();
        }

        private void cmdreset_Click(object sender, EventArgs e)
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.Title = "Stopped";
            grdMain.Background = Brushes.LightGray;
            this.timer1.Stop();
            cmdOk.IsEnabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.Title = "Stopped";

            var times = GetSettings();
            txtMins.Text = times.Mins.ToString();
            txtSecs.Text = times.Secs.ToString();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private AppTimes GetSettings()
        {
            AppTimes times = new AppTimes();
            mouseclick_looper.Mouseclick_looper settings = new Mouseclick_looper();

            times.Mins = int.Parse(settings.Mins);
            times.Secs = int.Parse(settings.Secs);

            return times;
        }

        private void SaveSettings(AppTimes _times)
        {
            mouseclick_looper.Mouseclick_looper settings = new Mouseclick_looper();

            settings.Mins = txtMins.Text;
            settings.Secs = txtSecs.Text;
            settings.Save();
        }

      
        }
    
}