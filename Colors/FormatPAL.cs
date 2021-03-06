﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithoutHaste.Drawing.Colors
{
	/// <summary>
	/// Paint Shop Pro *.pal color palette file format.
	/// </summary>
	public class FormatPAL : IPaletteFormat
	{
		private ColorPalette colorPalette;
		/// <summary></summary>
		public ColorPalette ColorPalette { get { return colorPalette; } }

		/// <summary></summary>
		public string Header { get; protected set; }
		/// <summary></summary>
		public string Version { get; protected set; }

		/// <summary>Load color palette from file.</summary>
		/// <param name="fullFilename">Path + filename + extension.</param>
		public FormatPAL(string fullFilename)
		{
			colorPalette = new ColorPalette();
			IO.ValidateFilename(fullFilename, ".pal");
			string[] fileLines = File.ReadAllLines(fullFilename);

			if(fileLines.Length <= 2)
				return;

			colorPalette = new ColorPalette();
			Header = fileLines[0]; //JASC-PAL only known header
			Version = fileLines[1]; //0100 only known version
			int colorCount = Int32.Parse(fileLines[2]); //expected color count
			LoadVersion0100(fileLines);
		}

		/// <summary>Load and return color palette from file.</summary>
		/// <param name="fullFilename">Path + filename + extension.</param>
		public static ColorPalette Load(string fullFilename)
		{
			FormatPAL pal = new FormatPAL(fullFilename);
			return pal.ColorPalette;
		}

		/// <summary>Save color palette in Version 0100 .pal format.</summary>
		/// <param name="fullFilename">Path + filename + extension.</param>
		public static void Save(string fullFilename, ColorPalette palette)
		{
			string[] fileLines = SaveVersion0100(palette);
			File.WriteAllLines(fullFilename, fileLines);
		}

		/// <summary>Save color palette in Version 0100 .pal format.</summary>
		/// <param name="fullFilename">Path + filename + extension.</param>
		public void Save(string fullFilename)
		{
			Save(fullFilename, ColorPalette);
		}

		private void LoadVersion0100(string[] fileLines)
		{
			colorPalette.Clear();
			for(int i = 3; i < fileLines.Length; i++)
			{
				string[] fields = fileLines[i].Split(' ');
				if(fields.Length != 3)
					continue; //just skip badly formatted lines
				int red;
				int green;
				int blue;
				if(!Int32.TryParse(fields[0].Trim(), out red))
					continue; //just skip badly formatted lines
				if(!Int32.TryParse(fields[1].Trim(), out green))
					continue; //just skip badly formatted lines
				if(!Int32.TryParse(fields[2].Trim(), out blue))
					continue; //just skip badly formatted lines
				colorPalette.Add(ConvertColors.RGBToColor(red, green, blue));
			}
		}

		private static string[] SaveVersion0100(ColorPalette palette)
		{
			List<string> fileLines = new List<string>();
			fileLines.Add("JASC-PAL"); //header
			fileLines.Add("0100"); //version
			fileLines.Add(palette.Count.ToString()); //color count
			foreach(Color color in palette.Colors)
			{
				fileLines.Add(String.Format("{0} {1} {2}", color.R, color.G, color.B));
			}
			return fileLines.ToArray();
		}
	}
}
