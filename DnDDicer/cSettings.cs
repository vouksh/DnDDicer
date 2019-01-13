using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;


namespace DnDDicer
{
	public class SettingsStore
	{
		public bool AlwaysReset;
		public string ProfileName;
		public string MainRowColor;
		public string OffRowColor;
		public bool ShowEmpowered;
	}

	public class DicerSettings
	{
		public SettingsStore Data;
		public DicerSettings()
		{
			LoadSettings();
		}
		public void LoadSettings()
		{
			XmlSerializer x = new XmlSerializer(typeof(SettingsStore));
			try
			{
				FileStream fs = new FileStream("DnDDicerSettings.xml", FileMode.Open);
				Data = (SettingsStore)x.Deserialize(fs);
				fs.Close();
			}
			catch(FileNotFoundException ex)
			{
				Data = new SettingsStore
				{
					AlwaysReset = false,
					ProfileName = "Default",
					MainRowColor = "White",
					OffRowColor = "LightGray",
					ShowEmpowered = true
				};
			}
		}
		public void SaveSettings()
		{
			try { File.Delete("DnDDicerSettings.xml"); }
			catch { }
			XmlSerializer xs = new XmlSerializer(typeof(SettingsStore));
			TextWriter tw = new StreamWriter("DnDDicerSettings.xml");
			xs.Serialize(tw, Data);
			tw.Flush();
			tw.Close();
		}
	}
}
