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
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;


namespace DnDDicer
{
	/// <summary>
	/// Interaction logic for AddProfile.xaml
	/// </summary>
	public partial class AddProfile : Window
	{
		public AddProfile()
		{
			InitializeComponent();
		}

		private void okBtn_Click(object sender, RoutedEventArgs e)
		{
			Dice tDice = new Dice();
			DiceSaver tSaver = new DiceSaver(profTxt.Text);
			tSaver.LoadFile(ref tDice);
			tSaver.SaveFile(tDice);
			DicerSettings set = new DicerSettings();
			set.LoadSettings();
			set.Data.ProfileName = profTxt.Text;
			set.SaveSettings();
			this.Close();
		}

		private void profTxt_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				Dice tDice = new Dice();
				DiceSaver tSaver = new DiceSaver(profTxt.Text);
				tSaver.LoadFile(ref tDice);
				tSaver.SaveFile(tDice);
				DicerSettings set = new DicerSettings();
				set.LoadSettings();
				set.Data.ProfileName = profTxt.Text;
				set.SaveSettings();
				this.Close();
			}
		}
	}
}
