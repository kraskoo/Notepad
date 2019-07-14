namespace Notepad
{
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using Forms = System.Windows.Forms;
    using Microsoft.Win32;

    internal class FileDialogFilter
    {
        public FileDialogFilter(string name, string extension)
        {
            this.Name = name;
            this.Extension = extension;
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
        private readonly List<FileDialogFilter> fileDialogFilters = new List<FileDialogFilter>
        {
            new FileDialogFilter("Text files", ".txt"),
            new FileDialogFilter("All files", ".*"),
        };
        private readonly string originalTitle;

        private string lastOpenedFilename = string.Empty;
        private bool clickedToFocus;

        public MainWindow()
        {
            this.InitializeComponent();
            this.originalTitle = this.Title;
            this.clickedToFocus = false;
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.clickedToFocus = !((TextBox) sender).IsFocused;
        }

        private void TextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBox) sender;
            if (this.clickedToFocus && tb.SelectedText.Length == 0)
            {
                tb.CaretIndex = tb.Text.Length;
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.TextBox.Text = string.Empty;
            this.Title = this.originalTitle;
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog {Filter = string.Join("|", this.fileDialogFilters)};
            if (ofd.ShowDialog() == true)
            {
                this.lastOpenedFilename = ofd.FileName;
                this.TextBox.Text = File.ReadAllText(ofd.FileName);
                this.Title = $"{this.lastOpenedFilename} - {this.originalTitle}";
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.lastOpenedFilename != string.Empty;

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            File.WriteAllText(this.lastOpenedFilename, this.TextBox.Text);
        }

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = true;

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sfd = new SaveFileDialog {Filter = string.Join("|", this.fileDialogFilters)};
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, this.TextBox.Text);
                this.lastOpenedFilename = sfd.FileName;
                this.Title = $"{this.lastOpenedFilename} - {this.originalTitle}";
            }
        }

        private void CutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.TextBox.SelectionLength > 0;

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(this.TextBox.SelectedText);
            this.TextBox.Text =
                this.TextBox.Text.Remove(this.TextBox.CaretIndex, this.TextBox.SelectedText.Length);
        }

        private void CopyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.TextBox.SelectedText.Length > 0;

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(this.TextBox.SelectedText);
        }

        private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = Clipboard.GetText().Length > 0;

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.TextBox.SelectedText.Length > 0)
            {
                this.TextBox.Text =
                    this.TextBox.Text.Remove(this.TextBox.CaretIndex, this.TextBox.SelectedText.Length);
            }

            this.TextBox.Text = this.TextBox.Text.Insert(this.TextBox.CaretIndex, Clipboard.GetText());
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.TextBox.SelectedText.Length > 0;

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.TextBox.Text =
                this.TextBox.Text.Remove(this.TextBox.CaretIndex, this.TextBox.SelectedText.Length);
        }

        private void SelectAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = this.TextBox.Text.Length > 0;

        private void SelectAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.TextBox.SelectedText = this.TextBox.Text;
        }

        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var doc = new FlowDocument(new Paragraph(new Run(this.TextBox.Text))) {Name = "FlowDocument"};
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, this.Title);
            }
        }

        private void FontClick(object sender, RoutedEventArgs e)
        {
            var fd = new Forms.FontDialog();
            var dr = fd.ShowDialog();
            if (dr != Forms.DialogResult.Cancel)
            {
                this.TextBox.FontFamily = new System.Windows.Media.FontFamily(fd.Font.Name);
                this.TextBox.FontSize = fd.Font.Size * 96.0 / 72.0;
                this.TextBox.FontWeight = fd.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                this.TextBox.FontStyle = fd.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        private void IncrementFontSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.TextBox.FontSize++;
        }

        private void DecrementFontSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.TextBox.FontSize--;
        }
    }
}