using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FinalPr
{
    public class UIManager
    {
        private readonly MainWindow mainWindow;
        private readonly TextBlock statusBarText;
        private readonly TextBlock cursorPositionText;
        private readonly Button boldButton;
        private readonly Button italicButton;
        private readonly Button underlineButton;
        private readonly ComboBox fontFamilyComboBox;
        private readonly ComboBox fontSizeComboBox;
        private readonly Button textColorButton;
        private readonly Button backgroundColorButton;

        public UIManager(MainWindow window, TextBlock statusText, TextBlock cursorText,
                        Button bold, Button italic, Button underline,
                        ComboBox fontFamily, ComboBox fontSize,
                        Button textColor, Button backgroundColor)
        {
            mainWindow = window;
            statusBarText = statusText;
            cursorPositionText = cursorText;
            boldButton = bold;
            italicButton = italic;
            underlineButton = underline;
            fontFamilyComboBox = fontFamily;
            fontSizeComboBox = fontSize;
            textColorButton = textColor;
            backgroundColorButton = backgroundColor;
        }

        public void InitializeComboBoxes()
        {
            fontFamilyComboBox.Items.Clear();
            foreach (FontFamily font in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
            {
                fontFamilyComboBox.Items.Add(font.Source);
            }
            fontFamilyComboBox.SelectedItem = "Arial";

            fontSizeComboBox.Items.Clear();
            int[] fontSizes = { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            foreach (int size in fontSizes)
            {
                fontSizeComboBox.Items.Add(size.ToString());
            }
            fontSizeComboBox.SelectedItem = "12";
        }

        public void UpdateTitle()
        {
            string fileName = string.IsNullOrEmpty(mainWindow.CurrentFilePath) ?
                             "Untitled" : Path.GetFileName(mainWindow.CurrentFilePath);
            string modified = mainWindow.IsDocumentModified ? "*" : "";
            mainWindow.Title = $"{fileName}{modified} - Text Editor";
        }

        public void SetStatusText(string text)
        {
            statusBarText.Text = text;
        }

        public void UpdateCursorPosition()
        {
            try
            {
                var mainTextBox = GetMainTextBox();
                if (mainTextBox == null) return;

                TextPointer caretLineStart = mainTextBox.CaretPosition.GetLineStartPosition(0);
                TextPointer documentStart = mainTextBox.Document.ContentStart;

                int lineNumber = 1;
                int columnNumber = 1;

                TextPointer linePointer = documentStart.GetLineStartPosition(0);
                while (linePointer != null && linePointer.CompareTo(caretLineStart) < 0)
                {
                    lineNumber++;
                    linePointer = linePointer.GetLineStartPosition(1);
                }

                if (caretLineStart != null)
                {
                    columnNumber = mainTextBox.CaretPosition.GetOffsetToPosition(caretLineStart);
                    columnNumber = Math.Abs(columnNumber) + 1;
                }

                cursorPositionText.Text = $"Line {lineNumber}, Column {columnNumber}";
            }
            catch (Exception)
            {
                cursorPositionText.Text = "Line 1, Column 1";
            }
        }

        public void UpdateFormattingButtons(FormattingManager formattingManager)
        {
            if (formattingManager == null) return;

            try
            {
                boldButton.Opacity = formattingManager.IsBold ? 1.0 : 0.5;
                italicButton.Opacity = formattingManager.IsItalic ? 1.0 : 0.5;
                underlineButton.Opacity = formattingManager.IsUnderline ? 1.0 : 0.5;

                string currentFontFamily = formattingManager.CurrentFontFamily;
                if (fontFamilyComboBox.Items.Contains(currentFontFamily))
                {
                    fontFamilyComboBox.SelectedItem = currentFontFamily;
                }

                double currentFontSize = formattingManager.CurrentFontSize;
                string fontSizeStr = ((int)currentFontSize).ToString();
                if (fontSizeComboBox.Items.Contains(fontSizeStr))
                {
                    fontSizeComboBox.SelectedItem = fontSizeStr;
                }
            }
            catch (Exception)
            {
            }
        }

        public void SetTextColorButtonBackground(Color color)
        {
            textColorButton.Background = new SolidColorBrush(color);
        }

        public void SetBackgroundColorButtonBackground(Color color)
        {
            backgroundColorButton.Background = new SolidColorBrush(color);
        }

        private RichTextBox GetMainTextBox()
        {
            var field = mainWindow.GetType().GetField("MainTextBox",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(mainWindow) as RichTextBox;
        }
    }
}