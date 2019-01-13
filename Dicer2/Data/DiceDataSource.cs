using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Dicer2.Data
{
	public class DiceDataItem
	{
		public string UniqueID { get; private set; }
		public string Size { get; private set; }
		public string Count { get; private set; }
		public string Name { get; private set; }
		public string AddAmount { get; private set; }
		public string IsEmpowered { get; private set; }
		public string LastRoll { get; private set; }
		public DiceDataItem(string _id, string _size, string _count, string _name, string _addAmt, string _isEmp, string _lastRoll)
		{
			this.UniqueID = _id;
			this.Size = _size;
			this.Count = _count;
			this.Name = _name;
			this.AddAmount = _addAmt;
			this.IsEmpowered = _isEmp;
			this.LastRoll = _lastRoll;
		}
		public override string ToString()
		{
			return this.Name;
		}
	}

	public class DiceDataGroup
	{
		public string UniqueID { get; private set; }
		public string CharacterName { get; set; }
		public ObservableCollection<DiceDataItem> Items { get; private set; }
		public DiceDataGroup(string _id, string _charName)
		{
			this.UniqueID = _id;
			this.CharacterName = _charName;
			this.Items = new ObservableCollection<DiceDataItem>();
		}
		public override string ToString()
		{
			return this.CharacterName;
		}
	}

	public class DiceDataSource
	{
		private static DiceDataSource _ddSrc = new DiceDataSource();

		public ObservableCollection<DiceDataGroup> Groups { get; } = new ObservableCollection<DiceDataGroup>();

		public static async Task<IEnumerable<DiceDataGroup>> GetGroupsAsync()
		{
			await _ddSrc.GetDiceDataAsync();

			return _ddSrc.Groups;
		}

		public static async Task<DiceDataGroup> GetGroupAsync(string UniqueID)
		{
			await _ddSrc.GetDiceDataAsync();

			var matches = _ddSrc.Groups.Where((group) => group.UniqueID.Equals(UniqueID));
			if(matches.Count() == 1) { return matches.First(); }
			return null;
		}

		public static async Task<DiceDataItem> GetItemAsync(string UniqueID)
		{
			await _ddSrc.GetDiceDataAsync();

			var matches = _ddSrc.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueID.Equals(UniqueID));
			if (matches.Count() == 1) { return matches.First(); }
			return null;
		}

		private async Task GetDiceDataAsync()
		{
			if (this.Groups.Count != 0)
				return;
			Uri dataUri = new Uri("./DiceData.json");

			//StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
			string jsonText = File.ReadAllText(dataUri.AbsolutePath);
			DiceDataGroup ddg = JsonConvert.DeserializeObject<DiceDataGroup>(jsonText);
			/*
			JsonObject jsonObject = JsonObject.Parse(jsonText);
			JsonArray jsonArray = jsonObject["Groups"].GetArray();

			foreach(JsonValue groupValue in jsonArray)
			{
				JsonObject groupObject = groupValue.GetObject();
				DiceDataGroup group = new DiceDataGroup(
					groupObject["UniqueID"].GetString(),
					groupObject["CharacterName"].GetString()
					);
				foreach (JsonValue itemValue in groupObject["Items"].GetArray())
				{
					JsonObject itemObject = itemValue.GetObject();
					group.Items.Add(new DiceDataItem(
						itemObject["UniqueID"].GetString(),
						itemObject["Size"].GetString(),
						itemObject["Count"].GetString(),
						itemObject["Name"].GetString(),
						itemObject["AddAmount"].GetString(),
						itemObject["IsEmpowered"].GetString(),
						itemObject["LastRoll"].GetString()
						));
				}
				this.Groups.Add(group);
			}*/
		}
	}
}
