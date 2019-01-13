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
using System.Collections.ObjectModel;

namespace DnDDicer
{

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public Dice DiceData;
		public DicerSettings Settings;
		public DiceSaver Saver;
		public MainWindow()
		{
			App.Log.maxLogLevel = vLogger.LogLevel.Debug;
			App.Log.WriteLog("MainWindow initialized at " + DateTime.Now.ToString(), vLogger.LogLevel.Error);
			InitializeComponent();
			Settings = new DicerSettings();
			App.Log.WriteDebug("Initialized Settings variable");
			Saver = new DiceSaver(Settings.Data.ProfileName);
			App.Log.WriteDebug("Initialized Saver variable");
			Saver.LoadFile(ref DiceData);
			App.Log.WriteDebug("Save.LoadFile()");
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			App.Log.WriteDebug("Saving data...");
			Saver.SaveFile(DiceData);
			Settings.SaveSettings();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			PopulateRows();
			this.Title = "D&D Dicer (" + Settings.Data.ProfileName + ")";
			App.Log.WriteDebug("Window loaded");
		}
		private void ReloadSettings()
		{
			Settings.LoadSettings();
			Saver = new DiceSaver(Settings.Data.ProfileName);
			Saver.LoadFile(ref DiceData);
			this.Title = "D&D Dicer (" + Settings.Data.ProfileName + ")";
			App.Log.WriteDebug("Settings reloaded");
		}
		private void addNew_Click(object sender, RoutedEventArgs e)
		{
			Die tDie = new Die("New Dice", 20, 1, false, 0);
			App.Log.WriteDebug("Adding new dice: " + tDie.ToString());
			DiceData.Add(tDie);
			PopulateRows();
		}

		private void NewRow_DiceUpdated(object sender, DiceEventArgs e)
		{
			App.Log.WriteDebug("Dice updated");
			DiceData = e.DiceData;
			if (e.DoRefresh)
				PopulateRows();
		}
		private void SettingsWindow_SettingsUpdated(object sender, SettingsEventArgs e)
		{
			ReloadSettings();
			PopulateRows();
		}
		/*private void SettingsWindow_Closed(object sender, RoutedEventArgs e)
		{
			ReloadSettings();
			PopulateRows();
		}*/
		private void PopulateRows()
		{
			App.Log.WriteDebug("Populating rows...");
			DiceRow newRow;
			int i = 0;
			DiceGrid.Items.Clear();

			App.Log.WriteDebug("Using data: " + DiceData);
			foreach (Die fDie in DiceData)
			{
				App.Log.WriteDebug("Adding row with dice data: " + fDie);
				newRow = new DiceRow
				{
					diceArray = DiceData,
					Name = "DiceRow" + i,
					thisDice = fDie,
					Margin = new Thickness(0, (DiceGrid.Items.Count * 60) + 5, 0, 0),
					VerticalAlignment = VerticalAlignment.Top,
					BorderThickness = new Thickness(0, 1, 0, 1),
					BorderBrush = Brushes.Black
				};

				if (DiceGrid.Items.Count % 2 != 0)
				{
					SolidColorBrush orColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Data.OffRowColor))
					{
						Opacity = 1
					};
					newRow.Background = orColor;
				} else
				{
					SolidColorBrush mrColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Data.MainRowColor))
					{
						Opacity = 1
					};
					newRow.Background = mrColor;
				}
				App.Log.WriteDebug("Adding new row " + newRow.diceArray.ToString());
				DiceGrid.Items.Add(newRow);
				newRow.DiceUpdated += new EventHandler<DiceEventArgs>(NewRow_DiceUpdated);
				newRow.Initialize();
				i = i + 1;
			}
			DiceGrid.InvalidateVisual();
			DiceGrid.Items.Refresh();

		}
		

		private void resetRolls_Click(object sender, RoutedEventArgs e)
		{
			for(int i = 0; i< DiceData.Count;i++)
			{
				DiceData[i].LastRoll = 0;
			}
			App.Log.WriteDebug("Rolls reset");
			PopulateRows();
		}

		private void settingsBtn_Click(object sender, RoutedEventArgs e)
		{
			App.Log.WriteDebug("Clicked settings button");
			SettingsWindow sw = new SettingsWindow();
			sw.SettingsUpdated += new EventHandler<SettingsEventArgs>(SettingsWindow_SettingsUpdated);
			//sw.Unloaded += new RoutedEventHandler(SettingsWindow_Closed);
			sw.ShowDialog();
		}
	}

}
