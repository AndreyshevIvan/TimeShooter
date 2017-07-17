using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MyGame
{
	public static class GameData
	{
		public static string locale
		{
			get
			{
				string localeName = PlayerPrefs.GetString(LOCALE_KEY, "eng");
				return LOCALE_PATH + localeName + LOCALE_FILE_NAME;
			}
		}
		public static BoundingBox box { get { return new BoundingBox(-12, 12, -18, 20); } }
		public static string LEVELS_PATH = RESOURCES_PATH + "levels/levels";

		public static uint GetNeededExp(ushort level)
		{
			return 1000;
		}
		public static void SaveShip(ShipProperties properties)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(SHIPS_PROPERTIES_PATH, FileMode.Create);
			formatter.Serialize(stream, properties);
			stream.Close();
		}
		public static ShipProperties LoadShip(ShipType type)
		{
			string file = SHIPS_PROPERTIES_PATH;

			if (!File.Exists(file))
			{
				ShipProperties newShip = new ShipProperties();
				SaveShip(newShip);
			}

			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(file, FileMode.Open);
			ShipProperties ship = (ShipProperties)formatter.Deserialize(stream);
			stream.Close();
			return ship;
		}

		static string LOCALE_FILE_NAME = "_locale";
		static string LOCALE_PATH = "locales/";
		static string LOCALE_KEY = "locale";

		static string RESOURCES_PATH = Application.dataPath + "/Resources/";
		static string FTYPE = ".txt";

		static string USER_FILE_NAME = "user" + FTYPE;
		static string USER_FILE = RESOURCES_PATH + USER_FILE_NAME;

		static string SHIPS_PROPERTIES_PATH = RESOURCES_PATH + "ShipProperties";

	}
}
