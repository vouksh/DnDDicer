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

namespace Dicer2
{
	/// <summary>
	/// Interaction logic for DiceRow.xaml
	/// </summary>
	public partial class DiceRow : UserControl
	{
		public DiceRow()
		{
			InitializeComponent();
		}

		private void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			var dd = Data.DiceDataSource.GetGroupsAsync();
		}
	}
}
