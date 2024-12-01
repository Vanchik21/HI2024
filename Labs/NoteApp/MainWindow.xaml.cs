using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace NoteApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartAutoSave();
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
                string text = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd).Text;
                int index = 0;
                while ((index = text.IndexOf(symbol, index)) != -1)
                {
                    TextPointer start = GetTextPointerAtOffset(Editor.Document.ContentStart, index);
                    TextPointer end = GetTextPointerAtOffset(Editor.Document.ContentStart, index + 1);
                    if (start != null && end != null)
                    {
                        new TextRange(start, end).ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
                    }
                    index++;
                }
            }
        }

        private TextPointer GetTextPointerAtOffset(TextPointer start, int offset)
        {
            while (offset > 0 && start != null)
            {
                if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    int count = start.GetTextRunLength(LogicalDirection.Forward);
                    if (offset <= count) return start.GetPositionAtOffset(offset);
                    offset -= count;
                }
                start = start.GetPositionAtOffset(1, LogicalDirection.Forward);
            }
            return start;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void AutoSave()
        {
            string backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NoteAppBackup.rtf");
            TextRange range = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
            using (FileStream fs = new FileStream(backupPath, FileMode.Create))
            {
                range.Save(fs, DataFormats.Rtf);
            }
        }

        private void StartAutoSave()
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            timer.Tick += (s, e) => AutoSave();
            timer.Start();
        }
    }
}
