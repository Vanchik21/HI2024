using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace NoteApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NewFile_Click(object sender, RoutedEventArgs e)
    {
        Editor.Document.Blocks.Clear();
    }

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Rich Text Format (*.rtf)|*.rtf"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            TextRange range = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
            using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open))
            {
                range.Load(fs, DataFormats.Rtf);
            }
        }
    }

    private void SaveFile_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Rich Text Format (*.rtf)|*.rtf"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            TextRange range = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
            using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
            {
                range.Save(fs, DataFormats.Rtf);
            }
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void FindSymbol_Click(object sender, RoutedEventArgs e)
    {
        string symbol = Microsoft.VisualBasic.Interaction.InputBox("Enter the symbol to find:", "Find Symbol");
        TextRange documentRange = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
        documentRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);

        if (!string.IsNullOrEmpty(symbol) && symbol.Length == 1)
        {
            char searchChar = symbol[0];
            TextPointer pointer = Editor.Document.ContentStart;

            while (pointer != null)
            {
                TextPointer nextPointer = pointer.GetPositionAtOffset(1, LogicalDirection.Forward);
                if (nextPointer == null) break;

                string textInRange = new TextRange(pointer, nextPointer).Text;
                if (textInRange == symbol)
                {
                    TextRange highlightRange = new TextRange(pointer, nextPointer);
                    highlightRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
                }

                pointer = nextPointer;
            }
        }
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        AboutBox aboutBox = new AboutBox();
        aboutBox.ShowDialog();
    }
}
