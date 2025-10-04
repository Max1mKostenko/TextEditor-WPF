using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FinalPr
{
    public class SearchManager
    {
        private readonly RichTextBox textBox;
        private readonly Grid searchPanel;
        private readonly TextBox findTextBox;
        private readonly TextBox replaceTextBox;

        public SearchManager(RichTextBox richTextBox, Grid panel, TextBox findBox, TextBox replaceBox)
        {
            textBox = richTextBox;
            searchPanel = panel;
            findTextBox = findBox;
            replaceTextBox = replaceBox;
        }

        public void ShowSearchPanel()
        {
            searchPanel.Visibility = Visibility.Visible;
            findTextBox.Focus();
        }

        public void HideSearchPanel()
        {
            searchPanel.Visibility = Visibility.Collapsed;
            textBox.Focus();
        }

        public string FindNext()
        {
            string searchText = findTextBox.Text;
            if (string.IsNullOrEmpty(searchText))
            {
                return "Enter text to search";
            }

            TextRange fullText = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            string content = fullText.Text;

            TextPointer currentPosition = textBox.CaretPosition;
            TextRange searchRange = new TextRange(currentPosition, textBox.Document.ContentEnd);
            string remainingText = searchRange.Text;

            int foundIndex = remainingText.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase);

            if (foundIndex >= 0)
            {
                TextPointer startPos = currentPosition.GetPositionAtOffset(foundIndex);
                TextPointer endPos = startPos?.GetPositionAtOffset(searchText.Length);

                if (startPos != null && endPos != null)
                {
                    textBox.Selection.Select(startPos, endPos);
                    textBox.Focus();
                    return $"Found: '{searchText}'";
                }
            }
            else
            {
                int firstOccurrence = content.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase);
                if (firstOccurrence >= 0)
                {
                    TextPointer startPos = textBox.Document.ContentStart.GetPositionAtOffset(firstOccurrence);
                    TextPointer endPos = startPos?.GetPositionAtOffset(searchText.Length);

                    if (startPos != null && endPos != null)
                    {
                        textBox.Selection.Select(startPos, endPos);
                        textBox.Focus();
                        return $"Found: '{searchText}' (wrapped to beginning)";
                    }
                }
                else
                {
                    return $"'{searchText}' not found";
                }
            }

            return "Search completed";
        }

        public string ReplaceOne()
        {
            string findText = findTextBox.Text;
            string replaceText = replaceTextBox.Text;

            if (string.IsNullOrEmpty(findText))
            {
                return "Enter text to find";
            }

            if (!textBox.Selection.IsEmpty)
            {
                string selectedText = textBox.Selection.Text;
                if (string.Equals(selectedText, findText, StringComparison.CurrentCultureIgnoreCase))
                {
                    textBox.Selection.Text = replaceText;

                    FindNext();
                    return $"Replaced '{findText}' with '{replaceText}'";
                }
            }

            FindNext();
            return "Found next occurrence";
        }

        public string ReplaceAll()
        {
            string findText = findTextBox.Text;
            string replaceText = replaceTextBox.Text;

            if (string.IsNullOrEmpty(findText))
            {
                return "Enter text to find";
            }

            TextRange fullText = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            string content = fullText.Text;

            int replacementCount = 0;
            int startIndex = 0;

            while (true)
            {
                int foundIndex = content.IndexOf(findText, startIndex, StringComparison.CurrentCultureIgnoreCase);
                if (foundIndex == -1) break;

                replacementCount++;
                content = content.Substring(0, foundIndex) + replaceText + content.Substring(foundIndex + findText.Length);
                startIndex = foundIndex + replaceText.Length;
            }

            if (replacementCount > 0)
            {
                textBox.Document = new FlowDocument(new Paragraph(new Run(content)));
                return $"Replaced {replacementCount} occurrence(s) of '{findText}' with '{replaceText}'";
            }
            else
            {
                return $"'{findText}' not found";
            }
        }

        public bool IsSearchPanelVisible => searchPanel.Visibility == Visibility.Visible;
    }
}