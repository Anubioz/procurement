using System.Windows;
using System.Windows.Controls;
using Procurement.ViewModel;

namespace Procurement.Controls
{
    public partial class ForumExport : UserControl
    {
        public ForumExport()
        {
            InitializeComponent();
            this.DataContext = new ForumExportViewModel();
        }

        void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            ForumExportViewModel vm = this.DataContext as ForumExportViewModel;
            CheckBox cb = sender as CheckBox;
            vm.update(int.Parse(cb.Tag.ToString()), cb.IsChecked.Value);
        }

        private void ToggleAll(object sender, RoutedEventArgs e)
        {
            ForumExportViewModel vm = this.DataContext as ForumExportViewModel;
            CheckBox cb = sender as CheckBox;
            vm.ToggleAll(cb.IsChecked.Value);
        }
    }
}
