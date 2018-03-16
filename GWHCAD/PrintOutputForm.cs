/************************************************************************************

	Project:
		GWHCAD

	File:
		PrintOutputForm.cs

	Description:
		This souce file contains the class for displaying/printing the output form.

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
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace GWHCAD
{
	public partial class PrintOutputForm : Form
	{
		private DesignObject lDesignObject = null;
		private AppPrefs lAppPrefs = AppPrefs.GAppPrefs;
		private PrintDocument lPrintDocument = new PrintDocument();
		private int lPrintOffsetY = 0;
		private int lPrintPageNo = 1;

		public PrintOutputForm()
		{
			InitializeComponent();

			// set window position prefs
			Location = new Point(lAppPrefs.ReportViewWindowLeft, lAppPrefs.ReportViewWindowTop);
			ClientSize = new Size(lAppPrefs.ReportViewWindowWidth, lAppPrefs.ReportViewWindowHeight);
			WindowState = (lAppPrefs.ReportViewWindowMaximized ? FormWindowState.Maximized : FormWindowState.Normal);

			// set up for printing
			lPrintDocument.BeginPrint += new PrintEventHandler(lPrintDocument_BeginPrint);
			lPrintDocument.PrintPage += new PrintPageEventHandler(lPrintDocument_PrintPage);
		}

		public void SetDesignObject(DesignObject designobject)
		{
			lDesignObject = designobject;
			lDesignOutputCtrl.SetDesignObject(designobject);
		}

		private void PrintOutputForm_Move(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
			{
				lAppPrefs.ReportViewWindowLeft = Left;
				lAppPrefs.ReportViewWindowTop = Top;
			}
		}

		private void PrintOutputForm_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
			{
				lAppPrefs.ReportViewWindowWidth = ClientSize.Width;
				lAppPrefs.ReportViewWindowHeight = ClientSize.Height;
			}
		}

		private void lTSUpdateBtn_Click(object sender, EventArgs e)
		{
			lDesignOutputCtrl.UpdateDocument();
		}

		private void PrintOutputForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			lAppPrefs.ReportViewWindowMaximized = (WindowState == FormWindowState.Maximized ? true : false);
		}

		private void lTSPrintBtn_Click(object sender, EventArgs e)
		{
			PrintDialog prtdlg = new PrintDialog();
			prtdlg.AllowSelection = false;
			prtdlg.AllowSomePages = false;
			prtdlg.AllowCurrentPage = false;
			prtdlg.AllowPrintToFile = true;
			prtdlg.Document = lPrintDocument;

			if (prtdlg.ShowDialog() == DialogResult.OK)
			{
				lPrintDocument.DocumentName = lDesignObject.Name;
				lPrintDocument.Print();
			}
		}

		private void lPrintDocument_BeginPrint(object sender, PrintEventArgs e)
		{
			lPrintOffsetY = 0;
			lPrintPageNo = 1;
		}

		private void lPrintDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			if (lDesignOutputCtrl.Document != null)
			{
				Rectangle pgbounds = e.PageBounds;
				int dispw = pgbounds.Width - (lAppPrefs.PrinterMarginLeft + lAppPrefs.PrinterMarginRight);
				int disph = pgbounds.Height - (lAppPrefs.PrinterMarginTop + lAppPrefs.PrinterMarginBottom);
				Rectangle disprect = new Rectangle(0, lPrintOffsetY, dispw, disph);
				Point outpt = new Point(lAppPrefs.PrinterMarginLeft, lAppPrefs.PrinterMarginTop);

				lPrintOffsetY = lDesignOutputCtrl.Document.PaintElement(e.Graphics, disprect, outpt, true);

				if (lPrintOffsetY < lDesignOutputCtrl.Document.Height)
				{
					lPrintPageNo++;
					e.HasMorePages = true;
				}
			}
		}
	}
}
