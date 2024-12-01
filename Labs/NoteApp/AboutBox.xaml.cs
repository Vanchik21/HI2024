using System.Windows;

namespace NoteApp
{
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
