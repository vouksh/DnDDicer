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
using System.IO;

namespace DnDDicer
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		private DicerSettings Settings;
		private bool refreshing = false;
		public SettingsWindow()
		{
			Settings = new DicerSettings();
			InitializeComponent();
		}

		private void GenProfList()
		{
			refreshing = true;
			Settings.LoadSettings();
			profileBox.Items.Clear();
			DirectoryInfo di = new DirectoryInfo("./DiceData");
			foreach(FileInfo prof in di.EnumerateFiles())
			{
				profileBox.Items.Add(prof.Name.Remove(prof.Name.Length - 4, 4));
			}
			profileBox.SelectedItem = Settings.Data.ProfileName;
			refreshing = false;

		}
		public event EventHandler<SettingsEventArgs> SettingsUpdated;
		protected virtual void OnSettingsUpdated(SettingsEventArgs e)
		{
			if(SettingsUpdated != null)
			{
				SettingsUpdated(this, e);
			}
		}

		private void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			GenProfList();
			MainRowColor.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Data.MainRowColor);
			OffRowColor.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Data.OffRowColor);
			alwaysResetBox.IsChecked = Settings.Data.AlwaysReset;
		}

		private void MainRowColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			Settings.Data.MainRowColor = MainRowColor.SelectedColorText;
			mrLabel.Background = new SolidColorBrush((Color)MainRowColor.SelectedColor);
		}

		private void OffRowColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			Settings.Data.OffRowColor = OffRowColor.SelectedColorText;
			orLabel.Background = new SolidColorBrush((Color)OffRowColor.SelectedColor);
		}
		private void AddProfileWindow_Closed(object sender, EventArgs e)
		{
			GenProfList();
		}
		private void addProfBtn_Click(object sender, RoutedEventArgs e)
		{
			AddProfile ap = new AddProfile();
			ap.Closed += new EventHandler(AddProfileWindow_Closed);
			ap.ShowDialog();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Settings.SaveSettings();
			SettingsEventArgs sea = new SettingsEventArgs
			{
				NewSettings = Settings
			};
			OnSettingsUpdated(sea);
		}

		private void profileBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(!refreshing)
				Settings.Data.ProfileName = profileBox.SelectedItem.ToString();
		}

		private void delProfBtn_Click(object sender, RoutedEventArgs e)
		{
			if (profileBox.Items.Count > 1)
			{
				string curItem = profileBox.SelectedItem.ToString();
				if (profileBox.SelectedIndex != 0)
				{
					profileBox.SelectedIndex = profileBox.SelectedIndex - 1;
				}
				else
				{
					profileBox.SelectedIndex = profileBox.SelectedIndex + 1;
				}
				File.Delete("./DiceData/" + curItem + ".dce");
				GenProfList();
			} else
			{
				System.Windows.MessageBox.Show("You can't delete the only profile!\nYou must add a new one first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
	public class SettingsEventArgs : RoutedEventArgs
	{
		public SettingsEventArgs() { }
		public DicerSettings NewSettings { get; set; }
	}
}
