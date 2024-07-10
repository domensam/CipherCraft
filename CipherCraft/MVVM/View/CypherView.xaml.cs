using CipherCraft.MVVM.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CipherCraft.MVVM.View
{
    /// <summary>
    /// Interaction logic for CypherView.xaml
    /// </summary>
    public partial class CypherView : UserControl
    {
        public CypherView()
        {
            InitializeComponent();
            DataContext = new CypherViewModel();
        }
    }
}
