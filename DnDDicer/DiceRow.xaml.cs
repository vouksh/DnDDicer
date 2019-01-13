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
using System.Reflection;

namespace DnDDicer
{
	/// <summary>
	/// Interaction logic for DiceRow.xaml
	/// </summary>
	public partial class DiceRow : UserControl
	{
		public Die thisDice;
		public Dice diceArray;
		public bool isLoading = false;
		public bool showEmp = true;
		public bool resetRolls = false;
		public DiceRow()
		{
			App.Log.WriteDebug("Initializing DiceRow component");
			InitializeComponent();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			App.Log.WriteDebug("Roll button clicked");
			List<int> Results = new List<int>();
			int randomRes = 0;
			double result = 0;
			Random rnd = new Random();
			if (thisDice.Count > 1)
			{
				for (int i = 0; i < thisDice.Count; i++)
				{
					randomRes = (rnd.Next(1, thisDice.Size));
					App.Log.WriteDebug("Random result: " + randomRes);
					result += randomRes + thisDice.AddAmount;
					Results.Add(randomRes);
				}
			}
			else
			{
				randomRes = (rnd.Next(1, thisDice.Size + 1));
				App.Log.WriteDebug("Random result: " + randomRes);
				result = randomRes + thisDice.AddAmount;
				Results.Add(randomRes);
			}
			if (thisDice.IsEmpowered)
			{
				result = Math.Ceiling(result * 1.5);
			}
			App.Log.WriteDebug("Completed result: " + result);
			thisDice.LastRoll = result;
			this.diceRoll.Content = result;
			string resText = "";
			for (int a = 0; a < Results.Count; a++)
			{
				if (a + 1 == Results.Count)
				{
					resText += Results[a];
				}
				else
				{
					resText += Results[a] + " + ";
				}
			}
			allRolls.Content = resText;
		}

		private void nameLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			nameLabel.Visibility = Visibility.Hidden;
			nameBox.Visibility = Visibility.Visible;
		}

		private void nameBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				App.Log.WriteDebug("Name updated: " + nameBox.Text);
				this.thisDice.Name = nameBox.Text;
				this.nameLabel.Content = nameBox.Text;
				nameBox.Visibility = Visibility.Hidden;
				nameLabel.Visibility = Visibility.Visible;
				SaveData();
			}
		}
		private void SaveData()
		{
			App.Log.WriteDebug("Saving data: " + diceArray.ToString());
			int i = 0;
			foreach (Die tDie in diceArray)
			{
				if (tDie.UID == thisDice.UID)
				{
					diceArray[i].Name = thisDice.Name;
					diceArray[i].AddAmount = thisDice.AddAmount;
					diceArray[i].Count = thisDice.Count;
					diceArray[i].Size = thisDice.Size;
					diceArray[i].IsEmpowered = thisDice.IsEmpowered;
					diceArray[i].LastRoll = thisDice.LastRoll;
				}
				i++;
			}
		}

		private void Grid_Loaded(object sender, RoutedEventArgs e)
		{
		}
		public void Initialize()
		{
			App.Log.WriteDebug("thisDice for " + this.Name + ": " + thisDice);
			isLoading = true;
			try
			{
				nameLabel.Content = thisDice.Name;
				nameBox.Text = thisDice.Name;
				diceBox.SelectedValue = thisDice.Size;
				diceBoxLabel.Content = thisDice.Size;
				diceCount.Text = thisDice.Count.ToString();
				diceCountLabel.Content = thisDice.Count.ToString();
				diceAdded.Text = thisDice.AddAmount.ToString();
				diceAddedLabel.Content = thisDice.AddAmount.ToString();
				empBox.IsChecked = thisDice.IsEmpowered;
				if (thisDice.LastRoll != 0 || !resetRolls)
				{
					diceRoll.Content = thisDice.LastRoll.ToString();
				}
				if (!showEmp)
				{
					empBox.Visibility = Visibility.Hidden;
				}
			}
			catch (Exception ex)
			{
				App.Log.WriteError(ex.Message);
			}
			isLoading = false;

			DiceEventArgs ddea = new DiceEventArgs
			{
				DiceData = diceArray
			};
			OnDiceUpdated(ddea);
		}

		//Dice Box Start
		private void diceBoxLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			diceBoxLabel.Visibility = Visibility.Hidden;
			diceBox.Visibility = Visibility.Visible;
		}

		private void diceBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				thisDice.Size = Convert.ToInt32((diceBox.SelectedItem as ComboBoxItem).Content);
				diceBoxLabel.Content = Convert.ToInt32((diceBox.SelectedItem as ComboBoxItem).Content);
				diceBoxLabel.Visibility = Visibility.Visible;
				diceBox.Visibility = Visibility.Hidden;
				SaveData();
			}
		}
		private void diceBox_LostFocus(object sender, RoutedEventArgs e)
		{
			/*
			thisDice.diceSize = (int)diceBox.SelectedValue;
			diceBoxLabel.Content = diceBox.SelectedValue;
			diceBoxLabel.Visibility = Visibility.Visible;
			diceBox.Visibility = Visibility.Hidden;
			SaveData();
			*/
		}
		private void diceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			thisDice.Size = Convert.ToInt32((diceBox.SelectedItem as ComboBoxItem).Content);
			diceBoxLabel.Content = Convert.ToInt32((diceBox.SelectedItem as ComboBoxItem).Content);
			diceBoxLabel.Visibility = Visibility.Visible;
			diceBox.Visibility = Visibility.Hidden;
			SaveData();
		}
		//Dice Box Stop

		//Dice Count Start
		private void diceCountLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			diceCountLabel.Visibility = Visibility.Hidden;
			diceCount.Visibility = Visibility.Visible;
		}

		private void diceCount_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				thisDice.Count = Convert.ToInt32(diceCount.Text);
				diceCountLabel.Content = diceCount.Text;
				diceCountLabel.Visibility = Visibility.Visible;
				diceCount.Visibility = Visibility.Hidden;
				SaveData();
			}
		}

		private void diceCount_LostFocus(object sender, RoutedEventArgs e)
		{
			thisDice.Count = Convert.ToInt32(diceCount.Text);
			diceCountLabel.Content = diceCount.Text;
			diceCountLabel.Visibility = Visibility.Visible;
			diceCount.Visibility = Visibility.Hidden;
			SaveData();
		}
		//Dice Count Stop

		//Dice Added Start
		private void diceAddedLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			diceAddedLabel.Visibility = Visibility.Hidden;
			diceAdded.Visibility = Visibility.Visible;
		}

		private void diceAdded_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				thisDice.AddAmount = Convert.ToInt32(diceAdded.Text);
				diceAddedLabel.Content = diceAdded.Text;
				diceAddedLabel.Visibility = Visibility.Visible;
				diceAdded.Visibility = Visibility.Hidden;
				SaveData();
			}
		}

		private void diceAdded_LostFocus(object sender, RoutedEventArgs e)
		{
			thisDice.AddAmount = Convert.ToInt32(diceAdded.Text);
			diceAddedLabel.Content = diceAdded.Text;
			diceAddedLabel.Visibility = Visibility.Visible;
			diceAdded.Visibility = Visibility.Hidden;
			SaveData();
		}

		private void empBox_Checked(object sender, RoutedEventArgs e)
		{
			if (!isLoading)
			{
				thisDice.IsEmpowered = true;
				SaveData();
			}
		}

		private void empBox_Unchecked(object sender, RoutedEventArgs e)
		{
			if (!isLoading)
			{
				thisDice.IsEmpowered = false;
				SaveData();
			}
		}
		//Dice Added Stop
		public event EventHandler<DiceEventArgs> DiceUpdated;
		protected virtual void OnDiceUpdated(DiceEventArgs e)
		{
			DiceUpdated?.Invoke(this, e);
		}
		private void deleteDice_Click(object sender, RoutedEventArgs e)
		{
			diceArray.Remove(thisDice.UID);
			//SaveData();
			DiceEventArgs ddea = new DiceEventArgs
			{
				DiceData = diceArray,
				DoRefresh = true
			};
			OnDiceUpdated(ddea);
		}

		private void diceRoll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			thisDice.LastRoll = 0;
			diceRoll.Content = "--";
			SaveData();
		}
	}

	public class DiceEventArgs : RoutedEventArgs
	{
		public DiceEventArgs ()
		{
			DoRefresh = false;
		}
		public Dice DiceData { get; set;}
		public bool DoRefresh { get; set; }
	}
}
