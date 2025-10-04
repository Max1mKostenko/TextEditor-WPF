using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace FinalPr
{
    public partial class MainWindow : Window
    {
        private readonly DocumentManager documentManager;
        private readonly SearchManager searchManager;
        private readonly FormattingManager formattingManager;
        private readonly UIManager uiManager;

        public MainWindow()
        {
            InitializeComponent();

            documentManager = new DocumentManager(this, MainTextBox);
            searchManager = new SearchManager(MainTextBox, SearchPanel, FindTextBox, ReplaceTextBox);
            formattingManager = new FormattingManager(MainTextBox);
            uiManager = new UIManager(this, StatusBarText, CursorPositionText,
                                    BoldButton, ItalicButton, UnderlineButton,
                                    FontFamilyComboBox, FontSizeComboBox,
                                    TextColorButton, BackgroundColorButton);

            InitializeEditor();
            SetupEventHandlers();
        }

        private void InitializeEditor()
        {
            uiManager.InitializeComboBoxes();
            MainTextBox.Focus();
            uiManager.UpdateTitle();
            uiManager.UpdateCursorPosition();
        }

        private void SetupEventHandlers()
        {
            MainTextBox.TextChanged += (s, e) =>
            {
                documentManager.SetModified(true);
                uiManager.UpdateTitle();
            };

            MainTextBox.SelectionChanged += MainTextBox_SelectionChanged;
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            documentManager.NewDocument();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            documentManager.OpenDocument();
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            documentManager.SaveDocument();
        }

        private void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            documentManager.SaveDocumentAs();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (documentManager.CheckSaveChanges())
            {
                Close();
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (MainTextBox.CanUndo)
            {
                MainTextBox.Undo();
                uiManager.SetStatusText("Undo completed");
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (MainTextBox.CanRedo)
            {
                MainTextBox.Redo();
                uiManager.SetStatusText("Redo completed");
            }
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            MainTextBox.SelectAll();
            uiManager.SetStatusText("All text selected");
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            searchManager.ShowSearchPanel();
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            searchManager.ShowSearchPanel();
        }

        private void FindNext_Click(object sender, RoutedEventArgs e)
        {
            string result = searchManager.FindNext();
            uiManager.SetStatusText(result);
        }

        private void ReplaceOne_Click(object sender, RoutedEventArgs e)
        {
            string result = searchManager.ReplaceOne();
            uiManager.SetStatusText(result);
        }

        private void CloseSearch_Click(object sender, RoutedEventArgs e)
        {
            searchManager.HideSearchPanel();
        }

        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            formattingManager.ToggleBold();
            MainTextBox.Focus();
        }

        private void Italic_Click(object sender, RoutedEventArgs e)
        {
            formattingManager.ToggleItalic();
            MainTextBox.Focus();
        }

        private void Underline_Click(object sender, RoutedEventArgs e)
        {
            formattingManager.ToggleUnderline();
            MainTextBox.Focus();
        }

        private void FontFamily_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null)
            {
                formattingManager.SetFontFamily(FontFamilyComboBox.SelectedItem.ToString());
                MainTextBox.Focus();
            }
        }

        private void FontSize_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem != null &&
                double.TryParse(FontSizeComboBox.SelectedItem.ToString(), out double fontSize))
            {
                formattingManager.SetFontSize(fontSize);
                MainTextBox.Focus();
            }
        }

        private void FontDialog_Click(object sender, RoutedEventArgs e)
        {
            var fontDialog = new FontSelectionDialog(this, formattingManager);
            fontDialog.ShowDialog();
            MainTextBox.Focus();
        }

        private void TextColor_Click(object sender, RoutedEventArgs e)
        {
            var colorDialog = new ColorSelectionDialog(this);
            Color? selectedColor = colorDialog.ShowDialog();
            if (selectedColor.HasValue)
            {
                formattingManager.SetTextColor(selectedColor.Value);
                uiManager.SetTextColorButtonBackground(selectedColor.Value);
                MainTextBox.Focus();
            }
        }

        private void BackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            var colorDialog = new ColorSelectionDialog(this);
            Color? selectedColor = colorDialog.ShowDialog();
            if (selectedColor.HasValue)
            {
                formattingManager.SetBackgroundColor(selectedColor.Value);
                uiManager.SetBackgroundColorButtonBackground(selectedColor.Value);
                MainTextBox.Focus();
            }
        }

        private void InsertImage_Click(object sender, RoutedEventArgs e)
        {
            var imageInserter = new ImageInserter(MainTextBox, uiManager);
            imageInserter.InsertImage();
        }

        private void BulletList_Click(object sender, RoutedEventArgs e)
        {
            uiManager.SetStatusText("Bullet list feature - implementation depends on requirements");
        }

        private void NumberedList_Click(object sender, RoutedEventArgs e)
        {
            uiManager.SetStatusText("Numbered list feature - implementation depends on requirements");
        }

        private void MainTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            uiManager.UpdateCursorPosition();
            uiManager.UpdateFormattingButtons(formattingManager);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            var keyboardHandler = new KeyboardShortcutHandler(this, documentManager, searchManager);
            if (keyboardHandler.HandleKeyPress(e))
            {
                e.Handled = true;
                return;
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!documentManager.CheckSaveChanges())
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        public string CurrentFilePath => documentManager.CurrentFilePath;
        public bool IsDocumentModified => documentManager.IsDocumentModified;
    }
}