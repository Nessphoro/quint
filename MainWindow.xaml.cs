using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quintessence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new Logic.ContactsModel(this);
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            FileView.Drop += FileView_Drop;
            TheView.SelectionChanged += TheView_SelectionChanged;
            base.OnInitialized(e);
        }

        void PathView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as Logic.ContactsModel).SelectPath.Execute(PathView.SelectedItem);
            PathView.SelectedIndex = -1;
        }


        void TheView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as Logic.ContactsModel).SelectContact.Execute(TheView.SelectedItem);
        }

        void FileView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                (DataContext as Logic.ContactsModel).DropFile.Execute(files);
               
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Focus();
        }

        private void FileView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
          if(e.OriginalSource.GetType() == typeof(Border) || 
              e.OriginalSource.GetType()==typeof(Image)||
              e.OriginalSource.GetType()==typeof(TextBlock))
          {
              (DataContext as Logic.ContactsModel).FileDoubleClick.Execute(FileView.SelectedItem);
          }
        }

    }
}
