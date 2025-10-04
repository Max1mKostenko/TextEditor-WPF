using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FinalPr
{
    public class DocumentManager
    {
        private readonly Window parentWindow;
        private readonly RichTextBox textBox;
        private string currentFilePath = "";
        private bool isDocumentModified = false;

        public DocumentManager(Window parent, RichTextBox richTextBox)
        {
            parentWindow = parent;
            textBox = richTextBox;
        }

        public string CurrentFilePath => currentFilePath;
        public bool IsDocumentModified => isDocumentModified;

        public void SetModified(bool modified)
        {
            isDocumentModified = modified;
        }

        public void NewDocument()
        {
            if (CheckSaveChanges())
            {
                textBox.Document = new FlowDocument();
                currentFilePath = "";
                isDocumentModified = false;

                if (parentWindow is MainWindow mainWindow)
                {
                    var uiManager = GetUIManager();
                    uiManager?.UpdateTitle();
                    uiManager?.SetStatusText("New document created");
                }
            }
        }

        public void OpenDocument()
        {
            if (CheckSaveChanges())
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                    DefaultExt = ".rtf"
                };

                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        LoadDocument(dialog.FileName);
                        currentFilePath = dialog.FileName;
                        isDocumentModified = false;

                        var uiManager = GetUIManager();
                        uiManager?.UpdateTitle();
                        uiManager?.SetStatusText($"Opened: {Path.GetFileName(currentFilePath)}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening file: {ex.Message}", "Error",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void SaveDocument()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveDocumentAs();
            }
            else
            {
                SaveDocumentToFile(currentFilePath);
            }
        }

        public void SaveDocumentAs()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                DefaultExt = ".rtf"
            };

            if (dialog.ShowDialog() == true)
            {
                SaveDocumentToFile(dialog.FileName);
                currentFilePath = dialog.FileName;
            }
        }

        private void LoadDocument(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".rtf")
            {
                TextRange textRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    textRange.Load(fileStream, DataFormats.Rtf);
                }
            }
            else
            {
                string content = File.ReadAllText(filePath);
                textBox.Document = new FlowDocument(new Paragraph(new Run(content)));
            }
        }

        private void SaveDocumentToFile(string filePath)
        {
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension == ".rtf")
                {
                    TextRange textRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        textRange.Save(fileStream, DataFormats.Rtf);
                    }
                }
                else
                {
                    TextRange textRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
                    File.WriteAllText(filePath, textRange.Text);
                }

                isDocumentModified = false;
                var uiManager = GetUIManager();
                uiManager?.UpdateTitle();
                uiManager?.SetStatusText($"Saved: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CheckSaveChanges()
        {
            if (isDocumentModified)
            {
                string fileName = string.IsNullOrEmpty(currentFilePath) ? "Untitled" : Path.GetFileName(currentFilePath);
                MessageBoxResult result = MessageBox.Show($"Do you want to save changes to {fileName}?",
                                                        "Save Changes", MessageBoxButton.YesNoCancel,
                                                        MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveDocument();
                    return !isDocumentModified; 
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private UIManager GetUIManager()
        {
            if (parentWindow is MainWindow mainWindow)
            {
                var field = typeof(MainWindow).GetField("uiManager",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return field?.GetValue(mainWindow) as UIManager;
            }
            return null;
        }
    }
}