using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Styx.Helpers;

namespace WorldQuestSettings
{
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
                s.TickFrequency = Math.Abs(attr.High - attr.Low) / 100;
                s.SmallChange = s.TickFrequency;
                s.LargeChange = s.TickFrequency * 10f;
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
}