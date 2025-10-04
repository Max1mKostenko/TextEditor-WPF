using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FinalPr
{
    public class ImageInserter
    {
        private readonly RichTextBox textBox;
        private readonly UIManager uiManager;

        public ImageInserter(RichTextBox richTextBox, UIManager ui)
        {
            textBox = richTextBox;
            uiManager = ui;
        }

        public void InsertImage()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files (*.*)|*.*",
                Title = "Select Image"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(dialog.FileName);
                    bitmap.DecodePixelWidth = 400; 
                    bitmap.EndInit();

                    Image image = new Image
                    {
                        Source = bitmap,
                        MaxWidth = 400,
                        MaxHeight = 300,
                        Stretch = System.Windows.Media.Stretch.Uniform
                    };

                    InlineUIContainer container = new InlineUIContainer(image, textBox.CaretPosition);

                    uiManager.SetStatusText("Image inserted successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting image: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    uiManager.SetStatusText("Failed to insert image");
                }
            }
        }
    }

    public class KeyboardShortcutHandler
    {
        private readonly MainWindow mainWindow;
        private readonly DocumentManager documentManager;
        private readonly SearchManager searchManager;

        public KeyboardShortcutHandler(MainWindow window, DocumentManager docManager, SearchManager searchMgr)
        {
            mainWindow = window;
            documentManager = docManager;
            searchManager = searchMgr;
        }

        public bool HandleKeyPress(KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                if (searchManager.IsSearchPanelVisible)
                {
                    string result = searchManager.FindNext();
                    GetUIManager()?.SetStatusText(result);
                }
                return true;
            }

            if (e.Key == Key.Escape)
            {
                if (searchManager.IsSearchPanelVisible)
                {
                    searchManager.HideSearchPanel();
                    return true;
                }
            }

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        documentManager.NewDocument();
                        return true;
                    case Key.O:
                        documentManager.OpenDocument();
                        return true;
                    case Key.S when Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift):
                        documentManager.SaveDocumentAs();
                        return true;
                    case Key.S:
                        documentManager.SaveDocument();
                        return true;
                    case Key.F:
                        searchManager.ShowSearchPanel();
                        return true;
                    case Key.H:
                        searchManager.ShowSearchPanel();
                        return true;
                }
            }

            return false;
        }

        private UIManager GetUIManager()
        {
            var field = mainWindow.GetType().GetField("uiManager",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(mainWindow) as UIManager;
        }
    }
}