namespace Notepad
{
    using Microsoft.Win32;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using Forms = System.Windows.Forms;
    using System.Windows.Input;

    internal class FileDialogFilter
    {
        public FileDialogFilter(string name, string extesion)
        {
            this.Name = name;
            this.Extension = extesion;
        }

        public string Name { get; }

        public string Extension { get; }

        public override string ToString()
        {
            return $"{this.Name} (*{this.Extension})|*{this.Extension}";
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<FileDialogFilter> FileDialogFilters = new List<FileDialogFilter>
        {
            new FileDialogFilter("Text files", ".txt"),
            new FileDialogFilter("All files", ".*"),
        };
        private string lastOpenedFilename = string.Empty;
        private string originalTitle = string.Empty;
        private bool clickedToFocus = false;

        public MainWindow()
        {
            this.InitializeComponent();
            this.originalTitle = this.Title;
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.clickedToFocus = !((TextBox)sender).IsFocused;
        }

        private void TextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (this.clickedToFocus && tb.SelectedText.Length == 0)
            {
                tb.CaretIndex = tb.Text.Length;
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.txtEditor.Text = string.Empty;
            this.Title = this.originalTitle;
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = string.Join("|", FileDialogFilters) };
            if (ofd.ShowDialog() == true)
            {
                this.lastOpenedFilename = ofd.FileName;
                this.txtEditor.Text = File.ReadAllText(ofd.FileName);
                this.Title = $"{this.lastOpenedFilename} - {this.originalTitle}";
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.lastOpenedFilename != string.Empty;

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            File.WriteAllText(this.lastOpenedFilename, this.txtEditor.Text);
        }

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = true;

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sfd = new SaveFileDialog { Filter = string.Join("|", FileDialogFilters) };
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, this.txtEditor.Text);
                this.lastOpenedFilename = sfd.FileName;
                this.Title = $"{this.lastOpenedFilename} - {this.originalTitle}";
            }
        }

        private void CutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.txtEditor.SelectionLength > 0;

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(this.txtEditor.SelectedText);
            this.txtEditor.Text = this.txtEditor.Text.Remove(this.txtEditor.CaretIndex, this.txtEditor.SelectedText.Length);
        }

        private void CopyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.txtEditor.SelectedText.Length > 0;

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(this.txtEditor.SelectedText);
        }

        private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = Clipboard.GetText().Length > 0;

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.txtEditor.SelectedText.Length > 0)
            {
                this.txtEditor.Text = this.txtEditor.Text.Remove(this.txtEditor.CaretIndex, this.txtEditor.SelectedText.Length);
            }

            this.txtEditor.Text = this.txtEditor.Text.Insert(this.txtEditor.CaretIndex, Clipboard.GetText());
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.txtEditor.SelectedText.Length > 0;

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.txtEditor.Text = this.txtEditor.Text.Remove(this.txtEditor.CaretIndex, this.txtEditor.SelectedText.Length);
        }

        private void SelectAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.txtEditor.Text.Length > 0;

        private void SelectAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.txtEditor.SelectedText = this.txtEditor.Text;
        }

        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var doc = new FlowDocument(new Paragraph(new Run(txtEditor.Text))) { Name = "FlowDocument" };
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, this.Title);
            }
        }

        private void FontClick(object sender, RoutedEventArgs e)
        {
            Forms.FontDialog fd = new Forms.FontDialog();
            Forms.DialogResult dr = fd.ShowDialog();
            if (dr != Forms.DialogResult.Cancel)
            {
                txtEditor.FontFamily = new System.Windows.Media.FontFamily(fd.Font.Name);
                txtEditor.FontSize = fd.Font.Size * 96.0 / 72.0;
                txtEditor.FontWeight = fd.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                txtEditor.FontStyle = fd.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        private void IncrementFontSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            txtEditor.FontSize = txtEditor.FontSize + 1;
        }

        private void DecrementFontSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            txtEditor.FontSize = txtEditor.FontSize - 1;
        }
    }
}