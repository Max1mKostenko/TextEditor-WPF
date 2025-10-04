using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FinalPr
{
    public class FormattingManager
    {
        private readonly RichTextBox textBox;

        public FormattingManager(RichTextBox richTextBox)
        {
            textBox = richTextBox;
        }

        public void ToggleBold()
        {
            object currentWeight = textBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            FontWeight newWeight = (currentWeight != DependencyProperty.UnsetValue &&
                                 (FontWeight)currentWeight == FontWeights.Bold) ?
                                 FontWeights.Normal : FontWeights.Bold;

            textBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, newWeight);
        }

        public void ToggleItalic()
        {
            object currentStyle = textBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            FontStyle newStyle = (currentStyle != DependencyProperty.UnsetValue &&
                                (FontStyle)currentStyle == FontStyles.Italic) ?
                                FontStyles.Normal : FontStyles.Italic;

            textBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, newStyle);
        }

        public void ToggleUnderline()
        {
            object currentDecorations = textBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            TextDecorationCollection newDecorations = (currentDecorations != DependencyProperty.UnsetValue &&
                                                     currentDecorations.Equals(TextDecorations.Underline)) ?
                                                     null : TextDecorations.Underline;

            textBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, newDecorations);
        }

        public void SetFontFamily(string fontFamilyName)
        {
            if (!string.IsNullOrEmpty(fontFamilyName))
            {
                FontFamily fontFamily = new FontFamily(fontFamilyName);
                textBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamily);
            }
        }

        public void SetFontSize(double fontSize)
        {
            if (fontSize > 0)
            {
                textBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
            }
        }

        public void SetTextColor(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            textBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
        }

        public void SetBackgroundColor(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            textBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, brush);
        }

        public void ApplyFormatting(string fontFamily, double fontSize, bool isBold, bool isItalic, bool isUnderline)
        {
            if (!string.IsNullOrEmpty(fontFamily))
                SetFontFamily(fontFamily);

            if (fontSize > 0)
                SetFontSize(fontSize);

            textBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty,
                isBold ? FontWeights.Bold : FontWeights.Normal);
            textBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty,
                isItalic ? FontStyles.Italic : FontStyles.Normal);
            textBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty,
                isUnderline ? TextDecorations.Underline : null);
        }

        public bool IsBold
        {
            get
            {
                object fontWeight = textBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
                return fontWeight != DependencyProperty.UnsetValue &&
                       (FontWeight)fontWeight == FontWeights.Bold;
            }
        }

        public bool IsItalic
        {
            get
            {
                object fontStyle = textBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
                return fontStyle != DependencyProperty.UnsetValue &&
                       (FontStyle)fontStyle == FontStyles.Italic;
            }
        }

        public bool IsUnderline
        {
            get
            {
                object textDecorations = textBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                return textDecorations != DependencyProperty.UnsetValue &&
                       textDecorations.Equals(TextDecorations.Underline);
            }
        }

        public string CurrentFontFamily
        {
            get
            {
                object fontFamily = textBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
                return fontFamily != DependencyProperty.UnsetValue ?
                       fontFamily.ToString() : "Arial";
            }
        }

        public double CurrentFontSize
        {
            get
            {
                object fontSize = textBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
                return fontSize != DependencyProperty.UnsetValue ?
                       (double)fontSize : 12.0;
            }
        }
    }
}