using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Honorbuddy;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Styx.Helpers;
using Styx.Plugins;

namespace WorldQuestSettings
{
    public class Main : HBPlugin
    {
        public static ConfigWindow Config;

        public override string Name => "WorldQuestHelper";
        public override string Author => "Nuok";
        public override Version Version => new Version(1, 1, GetSvnInt());

        public override bool WantButton => true;
        public override string ButtonText => "Settings";
        public static string SvnVersion => "$Rev$";

        private static int GetSvnInt()
        {
            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "(\\d+)";  // Integer Number 1

            Regex r = new Regex(re1 + re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(SvnVersion);
            if (!m.Success) return 0;

            var int1 = m.Groups[1].ToString();

            return int.Parse(int1);
        }

        public override void Pulse()
        {
        }

        public override void OnButtonPress()
        {
            if (Config == null)
                Config = new ConfigWindow("World Quest Helper", Version.ToString(), "Nuok", 400, 910);
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnButtonPress();          
        }

        
    }

    public class Settings : Styx.Helpers.Settings
    {
        public static readonly string _settingsPath = Path.Combine(CharacterSettingsDirectory, "Profiles",
            "NuokWorldQuestSettings.xml");

        private static Settings _instance;

        public Settings() : base(_settingsPath)
        {
        }

        public static Settings Instance => _instance ?? (_instance = new Settings());


        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do Wanted Quests")]
        [Styx.Helpers.DefaultValue(true)]
        public bool DoWanted { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do DANGER Quests")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoDangerQuests { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do Elite Quests")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoElite { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do PvP Quests")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoPvP { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do World Bosses")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoWorldBosses { get; set; }

        [Setting]
        [Category("Professions")]
        [DisplayName("Do Professions Quests")]
        [Styx.Helpers.DefaultValue(true)]
        public bool DoProfessions { get; set; }

        [Setting]
        [Category("Professions")]
        [DisplayName("Do Work Order Quests")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoWorkOrders { get; set; }

        #region zones

        [Setting]
        [Category("Zone")]
        [DisplayName("Azsuna")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Azsuna { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Val'sharah")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Valsharah { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Stormheim")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Stormheim { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Suramar")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Suramar { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Highmountain")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Highmountain { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Eye of Azshara")]
        [Styx.Helpers.DefaultValue(true)]
        public bool EyeofAzshara { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Dalaran")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Dalaran { get; set; }

        #endregion

        #region factions
        [Setting]
        [Category("Faction")]
        [DisplayName("The Wardens")]
        [Styx.Helpers.DefaultValue(true)]
        public bool TheWardens { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Court of Farondis")]
        [Styx.Helpers.DefaultValue(true)]
        public bool CourtOfFarondis { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Kirin Tor")]
        [Styx.Helpers.DefaultValue(true)]
        public bool KirinTor { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Highmountain Tribe")]
        [Styx.Helpers.DefaultValue(true)]
        public bool HighmountainTribe { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Dreamweavers")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Dreamweavers { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("The Nightfallen")]
        [Styx.Helpers.DefaultValue(true)]
        public bool TheNightfallen { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Valarjar")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Valarjar { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Do Emissary First")]
        [Description("Complete the Emissary Quest First")]
        [Styx.Helpers.DefaultValue(true)]
        public bool EmissaryFirst { get; set; }

        #endregion

        #region rewards
        [Setting]
        [Category("Rewards")]
        [DisplayName("Artifact Power")]
        [Description("Rewards that increase Artifact Power")]
        [Styx.Helpers.DefaultValue(true)]
        public bool ArtifactPower { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Equipment")]
        [Description("Gear for your character")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Equipment { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Item's")]
        [Description("Includes Trade Goods & Profession Items")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Items { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Blood of Sargeras")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Blood { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Order Hall Resource")]
        [Styx.Helpers.DefaultValue(true)]
        public bool OrderHall { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Gold")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Gold { get; set; }

        #endregion

        [Setting]
        [Category("Utility")]
        [DisplayName("Skip HB Relog Task")]
        [Styx.Helpers.DefaultValue(true)]
        public bool SkipHBRelog { get; set; }

        [Setting]
        [Category("Utility")]
        [DisplayName("SVN Update")]
        [Description("Runs the updater.bat file to try and update profiles.")]
        [Styx.Helpers.DefaultValue(false)]
        public bool SVNUpdate { get; set; }

        [Setting]
        [Category("Utility")]
        [DisplayName("Loop Profile")]
        [Description("Runs the profile in a loop to try and prevent it from missing quests")]
        [Styx.Helpers.DefaultValue(false)]
        public bool LoopProfile { get; set; }
    }

    public class ConfigWindow : MetroWindow
    {
        private readonly Grid _mainCanvas = new Grid();

        public ConfigWindow(string title, string logo, string logoTagLine, int width, int height)
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            Topmost = true;
            Width = width;
            Height = height;
            Title = title;
            var mainSettingsSource = Settings.Instance;

            // Store _mainCanvas outside this so we don't have to cast and whatnot.
            Content = _mainCanvas;
            _mainCanvas.RowDefinitions.Add(new RowDefinition());
            _mainCanvas.RowDefinitions.Add(new RowDefinition {Height = new GridLength(50)});

            // Add tab controls...
            var tabs = new TabControl();
            Grid.SetRow(tabs, 0);
            _mainCanvas.Children.Add(tabs);

            var mainTab = GenerateTab("General", mainSettingsSource);
            tabs.Items.Add(mainTab);

            #region "Logo"

            var logoPanel = new StackPanel();
            Grid.SetRow(logoPanel, 1);
            _mainCanvas.Children.Add(logoPanel);

            var logoMain = new TextBlock
            {
                Text = logo,
                FontSize = 20,
                FontFamily = new FontFamily("Impact"),
                Padding = new Thickness(5, 0, 0, 0)
            };

            logoPanel.Children.Add(logoMain);

            var logoTag = new TextBox
            {
                Text = logoTagLine,
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(5),
                BorderThickness = new Thickness(0)
            };

            logoPanel.Children.Add(logoTag);

            #endregion
        }

        private TabItem GenerateTab(string tabtitle, object source)
        {
            var tab = new TabItem
            {
                Header = tabtitle
            };
            var tabPanel = new StackPanel();
            var sv = new ScrollViewer
            {
                Content = tabPanel,
                BorderThickness = new Thickness(2)
            };
            tab.Content = sv;

            var settingsProperties =
                source.GetType()
                    .GetProperties()
                    .Where(p => p.GetCustomAttributes(false)
                        .Any(a => a is SettingAttribute));

            BuildSettings(tabPanel, settingsProperties, source);

            return tab;
        }

        private static T GetAttribute<T>(PropertyInfo pi) where T : Attribute
        {
            var attr = pi.GetCustomAttributes(false).FirstOrDefault(a => a is T);
            return attr as T;
        }

        private void BuildSettings(Panel tabPanel, IEnumerable<PropertyInfo> settings, object source)
        {
            var categories = new Dictionary<string, StackPanel>();

            foreach (var pi in settings.OrderBy(p => p.PropertyType.Name))
            {
                // First get some display stuff.
                // By default, any settings w/o a category set, go into "Misc"
                var category = GetAttribute<CategoryAttribute>(pi) != null
                    ? GetAttribute<CategoryAttribute>(pi).Category
                    : "Miscellaneous";
                var displayName = GetAttribute<DisplayNameAttribute>(pi) != null
                    ? GetAttribute<DisplayNameAttribute>(pi).DisplayName
                    // Default to the property name if no display name is given.
                    : pi.Name;
                var description = GetAttribute<DescriptionAttribute>(pi) != null
                    ? GetAttribute<DescriptionAttribute>(pi).Description
                    : null;

                if (!categories.ContainsKey(category))
                    categories.Add(category, new StackPanel());

                var group = categories[category];

                //Logger.Write(displayName + " -> " + description);

                var returnType = pi.PropertyType;

                // Deal with enums in a "special" way. The typecode for any enum will be int32 by default (unless marked as something else)
                // So we really want a dropdown, not a textbox.
                if (returnType.IsEnum)
                {
                    AddComboBoxForEnum(group, source, pi, displayName, description, returnType);
                    continue;
                }

                // Easiest way to blanket-statement a bunch of editable values. (Quite a few will just be textbox editors)
                switch (Type.GetTypeCode(returnType))
                {
                    case TypeCode.Boolean:
                        AddCheckbox(group, source, pi, displayName, description);
                        break;
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        AddSlider(group, source, pi, displayName, description);
                        break;
                    case TypeCode.String:
                        AddEditBox(group, source, pi, displayName, description);
                        break;
                }
            }

            foreach (
                var gb in
                categories.OrderBy(kv => kv.Key)
                    .Select(
                        sp =>
                            new Expander
                            {
                                Content = sp.Value,
                                Header = sp.Key,
                                ExpandDirection = ExpandDirection.Down,
                                IsExpanded = true
                            }))
                tabPanel.Children.Add(gb);
        }

        private void AddBinding(FrameworkElement ctrl, string xpath, object source, PropertyInfo bindTo,
            DependencyProperty depProp)
        {
            var b = new Binding(xpath)
                {Source = source, Path = new PropertyPath(bindTo.Name), Mode = BindingMode.TwoWay};
            ctrl.SetBinding(depProp, b);
        }

        private void AddCheckbox(Panel ctrl, object source, PropertyInfo bindTo, string label, string tooltip)
        {
            var cb = new CheckBox
            {
                Content = label,
                ToolTip = !string.IsNullOrEmpty(tooltip) ? tooltip : null
            };

            // And the binding so we don't have to do a lot of nasty event handling.
            AddBinding(cb, "IsChecked", source, bindTo, ToggleButton.IsCheckedProperty);

            ctrl.Children.Add(cb);
        }

        private void AddSlider(Panel ctrl, object source, PropertyInfo bindTo, string label, string tooltip)
        {
            var display = new StackPanel {Orientation = Orientation.Horizontal, Width = ctrl.Width};
            var l = new Label
            {
                Content = label,
                Margin = new Thickness(5, 5, 5, 3),
                ToolTip = tooltip
            };
            var sldLbl = new Label();

            var s = new Slider();
            // Find min/max
            var attr = GetAttribute<LimitAttribute>(bindTo);
            if (attr != null)
            {
                s.Maximum = attr.High;
                s.Minimum = attr.Low;
                s.TickFrequency = Math.Abs(attr.High - attr.Low)/100;
                s.SmallChange = s.TickFrequency;
                s.LargeChange = s.TickFrequency*10f;
            }
            s.MinWidth = 65;
            s.TickPlacement = TickPlacement.BottomRight;
            s.ToolTip = tooltip;
            AddBinding(s, "Value", source, bindTo, RangeBase.ValueProperty);

            var b = new Binding("Value") {Source = s, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay};
            sldLbl.SetBinding(ContentProperty, b);
            sldLbl.ContentStringFormat = "N2";
            sldLbl.Width = 38;


            display.Children.Add(l);
            display.Children.Add(s);
            display.Children.Add(sldLbl);

            ctrl.Children.Add(display);
        }

        private void AddEditBox(Panel ctrl, object source, PropertyInfo bindTo, string label, string tooltip)
        {
            // This is a bit tricky. We want to stack the edit box to the right of the label.
            // We do this via another stack panel, with a changed "stack" way.

            var display = new StackPanel {Orientation = Orientation.Horizontal, Width = ctrl.Width};
            var l = new Label
            {
                Content = label,
                Margin = new Thickness(5, 5, 5, 3),
                ToolTip = tooltip
            };

            var tb = new TextBox
            {
                ToolTip = tooltip,
                Background = Background,
                Margin = new Thickness(2, 3, 5, 3),
                MinWidth = 50
            };

            // Add the textbox/label to the stack panel so we can have it side-to-side.
            display.Children.Add(l);
            display.Children.Add(tb);


            // Don't forget the damned binding.
            AddBinding(tb, "Text", source, bindTo, TextBox.TextProperty);

            // And add it to the main control.
            ctrl.Children.Add(display);
        }

        private void AddComboBoxForEnum(Panel ctrl, object source, PropertyInfo bindTo, string label, string tooltip,
            Type enumType)
        {
            var display = new StackPanel {Orientation = Orientation.Horizontal, Width = ctrl.Width};
            var l = new Label
            {
                Content = label,
                Margin = new Thickness(5, 5, 5, 3),
                ToolTip = tooltip
            };

            var cb = new ComboBox
            {
                ToolTip = tooltip,
                Background = Background,
                BorderThickness = new Thickness(2)
            };
            foreach (var val in Enum.GetValues(enumType))
                cb.Items.Add(val);

            AddBinding(cb, "SelectedItem", source, bindTo, Selector.SelectedItemProperty);
            display.Children.Add(l);
            display.Children.Add(cb);


            ctrl.Children.Add(display);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class LimitAttribute : Attribute
    {
        public LimitAttribute(double low, double high)
        {
            Low = low;
            High = high;
        }

        public double High { get; set; }
        public double Low { get; set; }
    }
}