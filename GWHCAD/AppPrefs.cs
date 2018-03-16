/************************************************************************************

	Project:
		GWHCAD

	File:
		AppPrefs.cs

	Description:
		This souce file contains the class for storing configuration and preference
		data.

	MIT License

	Copyright (c) 2018 Greg Hall

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.

************************************************************************************/


// Namespaces used in this source
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GWHCAD
{
	public class AppPrefs
	{
		public static AppPrefs GAppPrefs = null;

		public int WindowLeft = 10;
		public int WindowTop = 10;
		public int WindowWidth = 800;
		public int WindowHeight = 600;
		public bool WindowMaximized = false;
		public string LastErrorMessage = "";

		public List<int> PartsListColumnWidths = new List<int>();

		public List<string> LastFiles = new List<string>();

		public bool ShowAxes = true;
		public bool ShowLabels = true;

		public int ReportViewWindowLeft = 10;
		public int ReportViewWindowTop = 10;
		public int ReportViewWindowWidth = 800;
		public int ReportViewWindowHeight = 600;
		public bool ReportViewWindowMaximized = false;

		public string PrinterDefault = "";
		public bool PrinterLayoutLandscape = false;
		public int PrinterMarginTop = 50;
		public int PrinterMarginLeft = 50;
		public int PrinterMarginRight = 50;
		public int PrinterMarginBottom = 50;

		public AppPrefs()
		{
			lLoadPrefs();
			GAppPrefs = this;
		}

		public bool SavePrefs()
		{
			string pathname = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GWHSoftware");
			string filename = Path.Combine(pathname, "gwhcad-v1.xml");
			XmlWriter prefsfile = null;
			bool retval = false;

			// make sure the folder exists
			if (!Directory.Exists(pathname))
				Directory.CreateDirectory(pathname);

			try
			{
				XmlWriterSettings settings = new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "  ",
					NewLineChars = "\r\n",
					NewLineHandling = NewLineHandling.Replace
				};

				prefsfile = XmlWriter.Create(filename, settings);

				prefsfile.WriteStartElement("GWHCADv1");

				prefsfile.WriteElementString("WindowLeft", WindowLeft.ToString("D"));
				prefsfile.WriteElementString("WindowTop", WindowTop.ToString("D"));
				prefsfile.WriteElementString("WindowWidth", WindowWidth.ToString("D"));
				prefsfile.WriteElementString("WindowHeight", WindowHeight.ToString("D"));
				prefsfile.WriteElementString("WindowMaximized", (WindowMaximized ? "1" : "0"));
				if (PartsListColumnWidths.Count > 0)
				{
					string outstr = "";

					foreach (int colwidth in PartsListColumnWidths)
					{
						if (outstr.Length > 0)
						{
							outstr += ",";
						}
						outstr += colwidth.ToString("D");
					}

					prefsfile.WriteElementString("PartsListColumnWidths", outstr);
				}

				if (LastFiles.Count > 0)
				{
					foreach (string tmpstr in LastFiles)
					{
						prefsfile.WriteElementString("LastFiles", tmpstr);
					}
				}

				prefsfile.WriteElementString("ShowAxes", (ShowAxes ? "1" : "0"));
				prefsfile.WriteElementString("ShowLabels", (ShowLabels ? "1" : "0"));

				prefsfile.WriteElementString("ReportViewWindowLeft", ReportViewWindowLeft.ToString("D"));
				prefsfile.WriteElementString("ReportViewWindowTop", ReportViewWindowTop.ToString("D"));
				prefsfile.WriteElementString("ReportViewWindowWidth", ReportViewWindowWidth.ToString("D"));
				prefsfile.WriteElementString("ReportViewWindowHeight", ReportViewWindowHeight.ToString("D"));
				prefsfile.WriteElementString("ReportViewWindowMaximized", (ReportViewWindowMaximized ? "1" : "0"));

				prefsfile.WriteElementString("PrinterDefault", PrinterDefault);
				prefsfile.WriteElementString("PrinterLayoutLandscape", (PrinterLayoutLandscape ? "1" : "0"));
				prefsfile.WriteElementString("PrinterMarginTop", PrinterMarginTop.ToString());
				prefsfile.WriteElementString("PrinterMarginLeft", PrinterMarginLeft.ToString());
				prefsfile.WriteElementString("PrinterMarginRight", PrinterMarginRight.ToString());
				prefsfile.WriteElementString("PrinterMarginBottom", PrinterMarginBottom.ToString());

				prefsfile.WriteEndElement();

				retval = true;
			}
			catch (Exception ex)
			{
				LastErrorMessage = ex.Message;
				// do nothing, will just return false
				retval = false;
			}
			finally
			{
				if (prefsfile != null)
					prefsfile.Close();
			}

			return retval;
		}

		private bool lLoadPrefs()
		{
			string filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GWHSoftware\\gwhcad-v1.xml";
			XmlTextReader prefsfile = null;
			bool retval = false;

			try
			{
				string elementname = null;

				prefsfile = new XmlTextReader(filename);

				while (prefsfile.Read())
				{
					switch (prefsfile.NodeType)
					{
						case XmlNodeType.Element:
							elementname = prefsfile.Name;
							break;
						case XmlNodeType.Text:
							if (elementname.Equals("WindowLeft")) WindowLeft = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("WindowTop")) WindowTop = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("WindowWidth")) WindowWidth = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("WindowHeight")) WindowHeight = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("WindowMaximized")) WindowMaximized = (prefsfile.Value.Equals("0") ? false : true);
							else if (elementname.Equals("PartsListColumnWidths"))
							{
								string[] colwidths = prefsfile.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
								if ((colwidths != null) && (colwidths.Length > 0))
								{
									foreach (string tmpstr in colwidths)
									{
										PartsListColumnWidths.Add(Convert.ToInt32(tmpstr));
									}
								}
							}

							else if (elementname.Equals("LastFiles")) LastFiles.Add(prefsfile.Value);
							else if (elementname.Equals("ShowAxes")) ShowAxes = (prefsfile.Value.Equals("0") ? false : true);
							else if (elementname.Equals("ShowLabels")) ShowLabels = (prefsfile.Value.Equals("0") ? false : true);

							else if (elementname.Equals("ReportViewWindowLeft")) ReportViewWindowLeft = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("ReportViewWindowTop")) ReportViewWindowTop = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("ReportViewWindowWidth")) ReportViewWindowWidth = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("ReportViewWindowHeight")) ReportViewWindowHeight = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("ReportViewWindowMaximized")) ReportViewWindowMaximized = (prefsfile.Value.Equals("0") ? false : true);

							else if (elementname.Equals("PrinterDefault")) PrinterDefault = prefsfile.Value;
							else if (elementname.Equals("PrinterLayoutLandscape")) PrinterLayoutLandscape = (prefsfile.Value.Equals("0") ? false : true);
							else if (elementname.Equals("PrinterMarginTop")) PrinterMarginTop = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("PrinterMarginLeft")) PrinterMarginLeft = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("PrinterMarginRight")) PrinterMarginRight = Convert.ToInt32(prefsfile.Value);
							else if (elementname.Equals("PrinterMarginBottom")) PrinterMarginBottom = Convert.ToInt32(prefsfile.Value);

							break;
						case XmlNodeType.EndElement:
							//elementname = null;
							break;
					}
				}

				retval = true;
			}
			catch (Exception ex)
			{
				LastErrorMessage = ex.Message;
				// do nothing, will just return false
				retval = false;
			}
			finally
			{
				if (prefsfile != null)
					prefsfile.Close();
			}

			return retval;
		}
	}
}
