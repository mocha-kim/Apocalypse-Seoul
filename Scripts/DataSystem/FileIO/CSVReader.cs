using System;
using System.Collections.Generic;
using System.IO;
using Alpha;
using Core;
using UnityEngine;

namespace DataSystem.FileIO
{
	public static class CSVReader
	{
		private static WaitForSeconds _wait = new (2f);
		
		public static List<List<string>> ReadFile(string fileName)
		{
			var textAsset = Resources.Load(Path.Combine(Constants.Path.CSVPath, fileName)) as TextAsset;
			try
			{
				var readData = ReadStream(textAsset.text);
				return readData;
			}
			catch (NullReferenceException e)
			{
				Debug.Log("[CSVReader] ReadStream Failed\n" + e.Message);
				return null;
			}
		}
		
		private static List<List<string>> ReadStream(string stream)
		{
			List<List<string>> table = new();
			var reader = new StringReader(stream);
			var trimChar = "\r".ToCharArray();

			// read column name
			reader.ReadLine();

			// read datas
			table.Clear();
			var row = new List<string>();
			var cell = "";
			
			var quoteCount = 0;
			var preChar = 0;
			var curChar = 0;
			while ((curChar = reader.Read()) >= 0)
			{
				switch (curChar)
				{
					case '"':
						quoteCount++;
						break;
					case ',':
					case '\n':
						if (quoteCount == 0 || (preChar == '"' && quoteCount % 2 == 0))
						{
							if (2 <= quoteCount)
							{
								cell = cell[1..];
								cell = cell[..^1];
							}

							if (2 < quoteCount)
							{
								cell = cell.Replace("\"\"", "\"");
							}

							cell = cell.Trim(trimChar);
							row.Add(cell);
							cell = "";
							preChar = 0;
							quoteCount = 0;
							if ('\n' == curChar)
							{
								table.Add(row);
								row = new List<string>();
							}

							continue;
						}
						break;
				}
				preChar = curChar;
				cell += Convert.ToChar(preChar);
			}

			if (cell.Length > 0)
			{
				cell = cell.Trim(trimChar);
			}
			if (row.Count > 0)
			{
				row.Add(cell);
				table.Add(row);
			}

			return table;
		}
	}
}