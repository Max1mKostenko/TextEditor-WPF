using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FinalPr
{
    public class FontSelectionDialog
    {
        private readonly Window parentWindow;
        private readonly FormattingManager formattingManager;

        public FontSelectionDialog(Window parent, FormattingManager formatting)
        {
            parentWindow = parent;
            formattingManager = formatting;
        }

        public bool? ShowDialog()
        {
            Window fontWindow = new Window
            {
                Title = "Font Selection",
                Width = 400,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = parentWindow,
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };

            Label fontFamilyLabel = new Label { Content = "Font Family:" };
            ComboBox fontFamilyCombo = new ComboBox { Height = 25, Margin = new Thickness(0, 0, 0, 10) };
            foreach (FontFamily font in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
            {
                fontFamilyCombo.Items.Add(font.Source);
            }
            fontFamilyCombo.SelectedItem = formattingManager.CurrentFontFamily;

            Label fontSizeLabel = new Label { Content = "Font Size:" };
            ComboBox fontSizeCombo = new ComboBox { Height = 25, Margin = new Thickness(0, 0, 0, 10) };
            int[] fontSizes = { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            foreach (int size in fontSizes)
            {
                fontSizeCombo.Items.Add(size.ToString());
            }
            fontSizeCombo.SelectedItem = ((int)formattingManager.CurrentFontSize).ToString();

            CheckBox boldCheck = new CheckBox { Content = "Bold", Margin = new Thickness(0, 5, 0, 5) };
            CheckBox italicCheck = new CheckBox { Content = "Italic", Margin = new Thickness(0, 0, 0, 5) };
            CheckBox underlineCheck = new CheckBox { Content = "Underline", Margin = new Thickness(0, 0, 0, 10) };

            boldCheck.IsChecked = formattingManager.IsBold;
            italicCheck.IsChecked = formattingManager.IsItalic;
            underlineCheck.IsChecked = formattingManager.IsUnderline;

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            Button okButton = new Button { Content = "OK", Width = 75, Height = 25, Margin = new Thickness(0, 0, 10, 0) };
            Button cancelButton = new Button { Content = "Cancel", Width = 75, Height = 25 };

            okButton.Click += (s, args) =>
            {
                if (fontFamilyCombo.SelectedItem != null && fontSizeCombo.SelectedItem != null &&
                    double.TryParse(fontSizeCombo.SelectedItem.ToString(), out double size))
                {
                    formattingManager.ApplyFormatting(
                        fontFamilyCombo.SelectedItem.ToString(),
                        size,
                        boldCheck.IsChecked == true,
                        italicCheck.IsChecked == true,
                        underlineCheck.IsChecked == true
                    );
                }

                fontWindow.DialogResult = true;
                fontWindow.Close();
            };

            cancelButton.Click += (s, args) => fontWindow.Close();

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            panel.Children.Add(fontFamilyLabel);
            panel.Children.Add(fontFamilyCombo);
            panel.Children.Add(fontSizeLabel);
            panel.Children.Add(fontSizeCombo);
            panel.Children.Add(boldCheck);
            panel.Children.Add(italicCheck);
            panel.Children.Add(underlineCheck);
            panel.Children.Add(buttonPanel);

            fontWindow.Content = panel;
            return fontWindow.ShowDialog();
        }
    }

    public class ColorSelectionDialog
    {
        private readonly Window parentWindow;

        public ColorSelectionDialog(Window parent)
        {
            parentWindow = parent;
        }

        public Color? ShowDialog()
        {
            Window colorWindow = new Window
            {
                Title = "Select Color",
                Width = 320,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = parentWindow,
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };

            Color[] predefinedColors = {
                Colors.Black, Colors.White, Colors.Red, Colors.Green, Colors.Blue,
                Colors.Yellow, Colors.Orange, Colors.Purple, Colors.Pink, Colors.Brown,
                Colors.Gray, Colors.LightGray, Colors.DarkGray, Colors.Cyan, Colors.Magenta,
                Colors.Lime, Colors.Maroon, Colors.Navy, Colors.Olive, Colors.Silver,
                Colors.Teal, Colors.Aqua, Colors.Fuchsia, Colors.DarkRed, Colors.DarkGreen,
                Colors.DarkBlue, Colors.Gold, Colors.IndianRed, Colors.LightBlue, Colors.LightGreen
            };

            Label label = new Label { Content = "Select a color:", FontWeight = FontWeights.Bold };
            panel.Children.Add(label);

            WrapPanel colorPanel = new WrapPanel { Margin = new Thickness(0, 10, 0, 0) };
            Color? selectedColor = null;

            foreach (Color color in predefinedColors)
            {
                Button colorButton = new Button
                {
                    Width = 35,
                    Height = 35,
                    Margin = new Thickness(3),
                    Background = new SolidColorBrush(color),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    ToolTip = color.ToString()
                };

                colorButton.Click += (s, args) =>
                {
                    selectedColor = color;
                    colorWindow.DialogResult = true;
                    colorWindow.Close();
                };

                colorPanel.Children.Add(colorButton);
            }

            ScrollViewer scrollViewer = new ScrollViewer
            {
                Content = colorPanel,
                Height = 250,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            Button cancelButton = new Button { Content = "Cancel", Width = 75, Height = 25 };
            cancelButton.Click += (s, args) => colorWindow.Close();
            buttonPanel.Children.Add(cancelButton);

            panel.Children.Add(scrollViewer);
            panel.Children.Add(buttonPanel);

            colorWindow.Content = panel;
            return colorWindow.ShowDialog() == true ? selectedColor : null;
        }
    }
}