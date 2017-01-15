using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Honorbuddy;
using MahApps.Metro;
using Styx.Plugins;
using WorldQuestSettings.GroupFinder;

namespace WorldQuestSettings
{
    public class Main : HBPlugin
    {
        public static ConfigWindow Config;

        public override string Name => "WorldQuestHelper";
        public override string Author => "Nuok";
        public override Version Version => new Version(2, 9, GetSvnInt());

        public override bool WantButton => true;
        public override string ButtonText => "Settings";
        public static string SvnVersion => "$Rev$";

        private static int GetSvnInt()
        {
            var re1 = ".*?"; // Non-greedy match on filler
            var re2 = "(\\d+)"; // Integer Number 1

            var r = new Regex(re1 + re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var m = r.Match(SvnVersion);
            if (!m.Success) return 0;

            var int1 = m.Groups[1].ToString();

            return int.Parse(int1);
        }

        public override void Pulse()
        {
            WQGF.Pulse();
        }

        public override void OnButtonPress()
        {
            if (Config == null)
                Config = new ConfigWindow("World Quest Helper", Version.ToString(), "Nuok", 400, 930);
            ThemeManager.ChangeAppStyle(Config, ThemeManager.GetAccent("Cobalt"), ThemeManager.GetAppTheme("BaseDark"));
            Config.Show();
            Config.Closed += config_Closed;
        }

        private void config_Closed(object sender, EventArgs e)
        {
            Settings.Instance.Save();
            Config = null;
        }

        public override void OnEnable()
        {
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                var grid = (Grid) ((MainWindow) Application.Current.MainWindow).Content;
                var profileConfigButton = grid.Children.OfType<Button>().FirstOrDefault(btn => btn.Name == "btnWQ");
                if (profileConfigButton != null) return;
                var button = new Button
                {
                    Name = "btnWQ",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 8, 55),
                    Content = "World Quest",
                    Height = 20,
                    Width = 120
                };
                button.Click += Button_Click;
                grid.Children.Add(button);
            }));

            WQGF.AttachEvent();
        }

        public override void OnDisable()
        {
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
                {
                    var grid = (Grid) ((MainWindow) Application.Current.MainWindow).Content;
                    var profileConfigButton = grid.Children.OfType<Button>().FirstOrDefault(btn => btn.Name == "btnWQ");
                    if (profileConfigButton != null)
                        grid.Children.Remove(profileConfigButton);
                }
            ));

            WQGF.DetachEvent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnButtonPress();
        }
    }
}