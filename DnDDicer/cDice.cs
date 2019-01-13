using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;

namespace DnDDicer
{
	public enum DiceFileType
	{
		XML,
		JSON
	}
	public class DiceSaver
	{
		private readonly string dataFolder = "./DiceData/";
		private readonly string fileExt = ".dce";
		public string profileName = "";
		private readonly string fileName = "";
		private DiceFileType diceFileType;
		public DiceSaver(string _profile, DiceFileType diceFileType = DiceFileType.XML)
		{
			profileName = _profile;
			fileName = dataFolder + profileName + fileExt;
			this.diceFileType = diceFileType;
			DirectoryInfo di = new DirectoryInfo(dataFolder);
			if(!di.Exists)
			{
				di.Create();
				App.Log.WriteDebug("Data folder created");
			}
			App.Log.WriteDebug("DiceSaver class initialized");
		}
		public void LoadFile(ref Dice diceData)
		{
			if (diceFileType == DiceFileType.XML)
			{
				this.LoadFile_XML(ref diceData);
			}
		}
		public void SaveFile(Dice diceData)
		{
			if (diceFileType == DiceFileType.XML)
			{
				this.SaveFile_XML(diceData);
			}
		}
		public void LoadFile_XML(ref Dice diceData)
		{
			App.Log.WriteDebug("Loading serialized XML file " + fileName);
			if (WaitForFile(fileName))
			{
				Dice tDice;
				XmlSerializer x = new XmlSerializer(typeof(Dice));
				try
				{
					FileStream fs = new FileStream(fileName, FileMode.Open);
					tDice = (Dice)x.Deserialize(fs);
					App.Log.WriteDebug("Read object " + tDice.ToString());
					fs.Close();
				}
				catch (FileNotFoundException ex)
				{
					App.Log.WriteDebug("File not found, using default settings");
					tDice = new Dice();
					Die tDie = new Die("Default Dice", 20, 1, false, 0);
					tDice.Add(tDie);
				}
				diceData = tDice;
			} else
			{
				App.Log.WriteDebug("Couldn't unlock the file + " + fileName); 
				throw new Exception("Couldn't unlock the file...");
			}
		}
		public void SaveFile_XML(Dice diceData)
		{
			App.Log.WriteDebug("Saving serialized XML file " + fileName);
			if (WaitForFile(fileName))
			{
				try { File.Delete(fileName); }
				catch (FileNotFoundException ex)
				{
					//Do nothing, file doesn't exist
				}
				XmlSerializer x = new XmlSerializer(typeof(Dice));
				TextWriter w = new StreamWriter(fileName);
				x.Serialize(w, diceData);
				w.Flush();
				w.Close();
			}
			else
			{
				throw new Exception("Couldn't unlock the file...");
			}
		}
		bool WaitForFile(string fullPath)
		{
			if (File.Exists(fullPath))
			{
				int numTries = 0;
				while (true)
				{
					++numTries;
					try
					{
						// Attempt to open the file exclusively.
						using (FileStream fs = new FileStream(fullPath,
							FileMode.Open, FileAccess.ReadWrite,
							FileShare.None, 100))
						{
							fs.ReadByte();

							// If we got this far the file is ready
							break;
						}
					}
					catch (Exception ex)
					{
						if (numTries > 10)
						{
							return false;
							throw new Exception("Tries exhausted. File is still locked.");
						}

						// Wait for the lock to be released
						System.Threading.Thread.Sleep(500);
					}
				}
				return true;
			}
			else
			{
				return true;
			}
		} 
	}
	
	[XmlInclude(typeof(Die))]
	[Serializable]
	public class Dice : IList<Die>
	{
		public string CollectionName;
		public List<Die> diceArray = new List<Die>();
		public bool IsReadOnly { get; set; }
		public override string ToString()
		{
			string retData = "{ ";
			int i = 0;
			foreach (Die die in diceArray)
			{
				retData += i + ": ";
				retData += die.ToString();
				i = i+1;
			}
			return retData + " }";
		}
		public Die this[int index]
		{
			get { return (Die)diceArray[index]; }
			set { diceArray[index] = value; }
		}
		public int IndexOf(Die iDie)
		{
			return diceArray.IndexOf(iDie);
		}
		public void Insert(int index, Die iDie)
		{
			diceArray.Insert(index, iDie);
		}
		public void RemoveAt(int index)
		{
			diceArray.RemoveAt(index);
		}
		public void Clear()
		{
			diceArray.Clear();
		}
		public bool Contains(Die iDie)
		{
			return diceArray.Contains(iDie);
		}
		public void CopyTo(Die[] iDie, int Index)
		{
			diceArray.CopyTo(iDie, Index);
		}
		public int Count
		{
			get { return diceArray.Count; }
		}
		public object SyncRoot
		{
			get { return this; }
		}
		public bool IsSynchronized
		{
			get { return false; }
		}
		public IEnumerator GetEnumerator()
		{
			return (IEnumerator<Die>)diceArray.GetEnumerator();
		}
		public void Add(Die newDie)
		{
			diceArray.Add(newDie);
			App.Log.WriteDebug("Dice object added " + newDie);
		}
		public void Add(object newDie)
		{
			try
			{
				this.diceArray.Add((Die)newDie);
			}
			catch
			{

			}
		}
		public bool Remove(Die iDie)
		{
			return diceArray.Remove(iDie);
		}
		public bool Remove(Guid diceId)
		{
			var tDie = from d in diceArray
					   where d.UID == diceId
					   select d;
			return diceArray.Remove(tDie.First<Die>());
		}

		IEnumerator<Die> IEnumerable<Die>.GetEnumerator()
		{
			return ((IList<Die>)diceArray).GetEnumerator();
		}
	}

	[Serializable]
	public class Die
	{
		public Guid UID { get; set; }
		public int Size { get; set; }
		public int Count { get; set; }
		public string Name { get; set; }
		public int AddAmount { get; set; }
		public bool IsEmpowered { get; set; }
		public double LastRoll { get; set; }
		public override string ToString()
		{
			string retData = " { UID: " + UID + ", ";
			retData += "Size: " + Size + ", ";
			retData += "Count: " + Count + ", ";
			retData += "Name: " + Name + ", ";
			retData += "AddAmount: " + AddAmount + ", ";
			retData += "IsEmpowered: " + IsEmpowered.ToString() + ", ";
			retData += "LastRoll: " + LastRoll + " }";
			return retData;
		}
		public Die()
		{
			UID = Guid.NewGuid();
		}
		public Die(string Name, int Size, int Count, bool Empowered, int Added)
		{
			this.Name = Name;
			this.Size = Size;
			this.Count = Count;
			IsEmpowered = Empowered;
			AddAmount = Added;
			LastRoll = 0;
			UID = Guid.NewGuid();
		}
	}
}
