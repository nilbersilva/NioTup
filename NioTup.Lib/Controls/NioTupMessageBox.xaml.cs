using NioTup.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NioTup.Lib.Controls
{
    /// <summary>
    /// Interaction logic for NioTupMessageBox.xaml
    /// </summary>
    public partial class NioTupMessageBox : Window
    {
        public NioTupMessageBoxVM MyVM = new();
        public NioTupMessageBox()
        {
            InitializeComponent();
            this.DataContext = MyVM;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (btnFirst.IsVisible)
            {
                btnFirst.Focus();
            }
        }
    }
}
