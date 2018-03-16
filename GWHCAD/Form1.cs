/************************************************************************************

	Project:
		GWHCAD

	File:
		Form1.cs

	Description:
		This souce file contains the main form class user defined functions and
		variables.

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
using System.Drawing;
using System.Windows.Forms;
using System.IO;


// GWH DEBUG--
//System.Diagnostics.Debug.WriteLine("draw rect element:  trace");
// --GWH DEBUG


namespace GWHCAD
{
	public partial class Form1 : Form
	{
		private AppPrefs lAppPrefs = null;
		private DesignObject lDesignObject = null;
		private bool lChangingFromControl = false;
		private DesignElementRect lCurrDesignElement = null;
		private string lDesignFilePath = "";

		public Form1()
		{
			InitializeComponent();

			// set up event handlers for the 3D projection control
			lDesignCtrl.AngleChangeEvent += new ControlAngleChangeCallback(lDesignControlAngleChangeHandler);
			lDesignCtrl.ScalingChangeEvent += new ControlScalingChangeCallback(lScalingChangeEventHandler);

			// load prefs
			lAppPrefs = new AppPrefs();

			// set window position prefs
			Location = new Point(lAppPrefs.WindowLeft, lAppPrefs.WindowTop);
			ClientSize = new Size(lAppPrefs.WindowWidth, lAppPrefs.WindowHeight);
			WindowState = (lAppPrefs.WindowMaximized ? FormWindowState.Maximized : FormWindowState.Normal);

			// set listview column widths
			if (lAppPrefs.PartsListColumnWidths.Count > 0)
			{
				int colndx = 0;

				foreach (int colwidth in lAppPrefs.PartsListColumnWidths)
				{
					if (colndx < lPartsLvw.Columns.Count)
					{
						lPartsLvw.Columns[colndx].Width = colwidth;
						colndx++;
					}
					else
					{
						break;
					}
				}
			}

			// set the title
			Text = "GWH CAD Application (Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";

			// populate the last files combo box
			lPopulateLastFilesListFromPrefs();
			if (lTSDesignFileCbx.Items.Count > 0)
			{
				lTSDesignFileCbx.SelectedIndex = 0;
			}

			lTSShowAxesCbx.SelectedIndex = (lAppPrefs.ShowAxes ? 0 : 1);
			lTSShowLabelsCbx.SelectedIndex = (lAppPrefs.ShowLabels ? 0 : 1);
		}

		private void lPopulateLastFilesListFromPrefs()
		{
			bool uiupdate = lChangingFromControl;
			lChangingFromControl = true;

			lTSDesignFileCbx.BeginUpdate();
			lTSDesignFileCbx.Items.Clear();

			if (AppPrefs.GAppPrefs.LastFiles.Count > 0)
			{
				foreach (string tmpstr in AppPrefs.GAppPrefs.LastFiles)
				{
					lTSDesignFileCbx.Items.Add(Path.GetFileName(tmpstr));
				}
			}

			lTSDesignFileCbx.EndUpdate();

			lChangingFromControl = uiupdate;
		}

		private void lDesignObjectFileBtn_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.Title = "Select a file to open";
			if ((lDesignFilePath.Length > 0) && Directory.Exists(Path.GetDirectoryName(lDesignFilePath)))
			{
				ofd.InitialDirectory = Path.GetDirectoryName(lDesignFilePath);
				ofd.FileName = Path.GetFileName(lDesignFilePath);
			}
			ofd.Multiselect = false;

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				lDesignFilePath = ofd.FileName;
				lDesignLoad();
			}
		}

		private void lDesignNewBtn_Click(object sender, EventArgs e)
		{
			lDesignFilePath = "";
			lDesignObject = new DesignObject();
			lDesignObject.Name = "";
			lDesignCtrl.SetDesignObject(lDesignObject);
			lDesignCtrl.ShowAxes = lAppPrefs.ShowAxes;
			lPartsLvw.Items.Clear();
		}

		private void lDesignSaveBtn_Click(object sender, EventArgs e)
		{
			if ((lDesignFilePath.Length > 0) && (File.Exists(lDesignFilePath)))
			{
				if (lDesignObject.Name.Length == 0)
				{
					lDesignObject.Name = Path.GetFileNameWithoutExtension(lDesignFilePath);
				}

				if (lDesignObject.SaveDesignObject(lDesignFilePath))
				{
					MessageBox.Show("Saved design.");
				}
				else
				{
					MessageBox.Show("ERROR:  Failed to save design.");
				}
			}
			else
			{
				lTSSaveAsBtn_Click(sender, e);
			}
		}

		private void lTSScalingCbx_TextChanged(object sender, EventArgs e)
		{
			if (!lChangingFromControl)
			{
				try
				{
					lDesignCtrl.Scaling = Convert.ToDouble(lTSScalingCbx.Text);
					lTSScalingCbx.ForeColor = Color.Black;
				}
				catch (System.FormatException /*fex*/)
				{
					lTSScalingCbx.ForeColor = Color.Red;
				}
			}
		}

		public void lDesignControlAngleChangeHandler(object caller, ControlAngleChangeArgs angleargs)
		{
			lChangingFromControl = true;
			lTSThetaLbl.Text = lDesignCtrl.Theta.ToString();
			lTSPhiLbl.Text = lDesignCtrl.Phi.ToString();
			lChangingFromControl = false;
		}

		public void lScalingChangeEventHandler(object caller, ControlScaleChangeArgs scaleargs)
		{
			lChangingFromControl = true;
			lTSScalingCbx.Text = scaleargs.ScaleValue.ToString();
			lChangingFromControl = false;
		}

		private void lTSDesignFileCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!lChangingFromControl)
			{
				int selndx = lTSDesignFileCbx.SelectedIndex;

				if ((selndx >= 0) && (selndx < AppPrefs.GAppPrefs.LastFiles.Count))
				{
					lDesignFilePath = AppPrefs.GAppPrefs.LastFiles[selndx];
					lDesignLoad();
				}
			}
		}

		private void lDesignLoad()
		{
			bool uiupdate = lChangingFromControl;
			lChangingFromControl = true;
			lTSDesignFileCbx.Text = "";
			lChangingFromControl = uiupdate;

			if ((lDesignFilePath.Length > 0) && File.Exists(lDesignFilePath))
			{
				lDesignObject = DesignObject.LoadDesignObject(lDesignFilePath);

				if (lDesignObject != null)
				{
					lDesignCtrl.SetDesignObject(lDesignObject);
					lDesignCtrl.ShowAxes = lAppPrefs.ShowAxes;
					lDesignCtrl.ResetPosition();

					lTSThetaLbl.Text = lDesignCtrl.Theta.ToString("0.000");
					lTSPhiLbl.Text = lDesignCtrl.Phi.ToString("0.000");
					lTSScalingCbx.Text = lDesignCtrl.Scaling.ToString("0.000");

					lPartsLvw.BeginUpdate();
					lPartsLvw.Items.Clear();

					lPrimaryAttachmentCbx.BeginUpdate();
					lPrimaryAttachmentCbx.Items.Clear();

					// fill in parts list view
					if (lDesignObject.Elements.Count > 0)
					{
						foreach (DesignElement tmpelem in lDesignObject.Elements)
						{
							DesignElementRect tmpelemr = (DesignElementRect)tmpelem;
							lAddDesignElement(tmpelemr);
						}
					}

					lPrimaryAttachmentCbx.EndUpdate();

					lPartsLvw.EndUpdate();

					StringHandler.AddStringToStartOfList(lAppPrefs.LastFiles, lDesignFilePath, 10);
					lPopulateLastFilesListFromPrefs();

					lChangingFromControl = true;
					lTSDesignFileCbx.Text = Path.GetFileName(lDesignFilePath);
					lChangingFromControl = uiupdate;
				}
			}
		}

		private void lPartsLvw_SelectedIndexChanged(object sender, EventArgs e)
		{
			lChangingFromControl = true;

			if ((lDesignObject != null) && (lPartsLvw.SelectedItems.Count == 1))
			{
				lCurrDesignElement = (DesignElementRect)lDesignObject.GetDesignElementByName(lPartsLvw.SelectedItems[0].Text);
			}
			else
			{
				lCurrDesignElement = null;
			}

			if (lCurrDesignElement != null)
			{
				lSetPartEntryControlsState(true);

				lPartNameTbx.Text = lCurrDesignElement.Name;
				lPartWidthTbx.Text = lCurrDesignElement.DimensionX.ToString();
				lPartHeightTbx.Text = lCurrDesignElement.DimensionZ.ToString();
				lPartDepthTbx.Text = lCurrDesignElement.DimensionY.ToString();
				if (lCurrDesignElement.AttachmentPoint != null)
				{
					lPrimaryAttachmentRbtn.Checked = true;
					lPlaceByMidpointRbtn.Checked = false;
					lSetPositionByRelationControlsState(false);
					lSetAttachmentPointByAttachmentName(lCurrDesignElement.AttachmentPoint.AttachmentName);
					lSetAttachmentControlsByCurrentDesignElement();
				}
				else
				{
					lPlaceByMidpointRbtn.Checked = true;
					lPrimaryAttachmentRbtn.Checked = false;
					lSetPositionByRelationControlsState(true);

					lPartMidXTbx.Text = lCurrDesignElement.Midpoint.X.ToString();
					lPartMidYTbx.Text = lCurrDesignElement.Midpoint.Y.ToString();
					lPartMidZTbx.Text = lCurrDesignElement.Midpoint.Z.ToString();
				}
				lSecondaryAttachmentsLvw.Items.Clear();
				// GWH UNFINISHED CODE--WILL HANDLE WHEN SECONDARY ATTACHMENTS HAVE BEEN ADDED TO DESIGN ELEMENT
			}
			else
			{
				lPartNameTbx.Text = "";
				lPartWidthTbx.Text = "";
				lPartHeightTbx.Text = "";
				lPartDepthTbx.Text = "";
				lPlaceByMidpointRbtn.Checked = false;
				lPartMidXTbx.Text = "";
				lPartMidYTbx.Text = "";
				lPartMidZTbx.Text = "";
				lPrimaryAttachmentRbtn.Checked = false;
				lPrimaryAttachmentCbx.Text = "";
				lSecondaryAttachmentsLvw.Items.Clear();
				lSetPartEntryControlsState(false);
			}

			lChangingFromControl = false;
		}

		private void lSetAttachmentControlsByCurrentDesignElement()
		{
			lPartMidXTbx.Text = lCurrDesignElement.AttachmentPoint.OwnX.Value.ToString();
			lPartMidYTbx.Text = lCurrDesignElement.AttachmentPoint.OwnY.Value.ToString();
			lPartMidZTbx.Text = lCurrDesignElement.AttachmentPoint.OwnZ.Value.ToString();

			lPartMidXCbx.SelectedIndex = lGetAttPtCbxFromDesignPoint(lCurrDesignElement.AttachmentPoint.OwnX);
			lPartMidYCbx.SelectedIndex = lGetAttPtCbxFromDesignPoint(lCurrDesignElement.AttachmentPoint.OwnY);
			lPartMidZCbx.SelectedIndex = lGetAttPtCbxFromDesignPoint(lCurrDesignElement.AttachmentPoint.OwnZ);

			lPartAttXTbx.Text = lCurrDesignElement.AttachmentPoint.AttX.Value.ToString();
			lPartAttYTbx.Text = lCurrDesignElement.AttachmentPoint.AttY.Value.ToString();
			lPartAttZTbx.Text = lCurrDesignElement.AttachmentPoint.AttZ.Value.ToString();

			lPartAttXCbx.SelectedIndex = lGetAttPtCbxFromDesignPoint(lCurrDesignElement.AttachmentPoint.AttX);
			lPartAttYCbx.SelectedIndex = lGetAttPtCbxFromDesignPoint(lCurrDesignElement.AttachmentPoint.AttY);
			lPartAttZCbx.SelectedIndex = lGetAttPtCbxFromDesignPoint(lCurrDesignElement.AttachmentPoint.AttZ);
		}

		private void lSetPositionByRelationControlsState(bool midpoint)
		{
			bool enable = (midpoint ? false : true);

			lPrimaryAttachmentCbx.Enabled = enable;
			lPartAttXTbx.Enabled = enable;
			lPartAttYTbx.Enabled = enable;
			lPartAttZTbx.Enabled = enable;
			lPartMidXCbx.Enabled = enable;
			lPartMidYCbx.Enabled = enable;
			lPartMidZCbx.Enabled = enable;
			lPartAttXCbx.Enabled = enable;
			lPartAttYCbx.Enabled = enable;
			lPartAttZCbx.Enabled = enable;

			if (midpoint)
			{
				bool uiupdate = lChangingFromControl;
				lChangingFromControl = true;
				lPrimaryAttachmentCbx.Text = "";
				lChangingFromControl = uiupdate;
			}
		}

		private void lSetPartEntryControlsState(bool enabled)
		{
			lPartNameTbx.Enabled = enabled;
			lPartWidthTbx.Enabled = enabled;
			lPartHeightTbx.Enabled = enabled;
			lPartDepthTbx.Enabled = enabled;
			lPlaceByMidpointRbtn.Enabled = enabled;
			lPartMidXTbx.Enabled = enabled;
			lPartMidYTbx.Enabled = enabled;
			lPartMidZTbx.Enabled = enabled;
			lPartAttXTbx.Enabled = enabled;
			lPartAttYTbx.Enabled = enabled;
			lPartAttZTbx.Enabled = enabled;
			lPartMidXCbx.Enabled = enabled;
			lPartMidYCbx.Enabled = enabled;
			lPartMidZCbx.Enabled = enabled;
			lPartAttXCbx.Enabled = enabled;
			lPartAttYCbx.Enabled = enabled;
			lPartAttZCbx.Enabled = enabled;
			lPrimaryAttachmentRbtn.Enabled = enabled;
			lPrimaryAttachmentCbx.Enabled = enabled;
			lSecondaryAttachmentsLvw.Enabled = enabled;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			// GWH UNFINISHED CODE--CHECK FOR UNSAVED CHANGES

			lAppPrefs.WindowMaximized = (WindowState == FormWindowState.Maximized ? true : false);

			lAppPrefs.PartsListColumnWidths.Clear();
			foreach (ColumnHeader colhdr in lPartsLvw.Columns)
			{
				lAppPrefs.PartsListColumnWidths.Add(colhdr.Width);
			}

			lAppPrefs.SavePrefs();
		}

		private void lForm1MoveHandler(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
			{
				lAppPrefs.WindowLeft = Left;
				lAppPrefs.WindowTop = Top;
			}
		}

		private void lForm1ResizeHandler(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
			{
				lAppPrefs.WindowWidth = ClientSize.Width;
				lAppPrefs.WindowHeight = ClientSize.Height;
			}
		}

		private string lGetAttachmentLVStringText(DesignElement delem)
		{
			string atttext = "";

			if (delem.AttachmentPoint != null)
			{
				atttext = delem.AttachmentPoint.AttachmentName + ": at " +
					delem.AttachmentPoint.OwnX.GetTextForParameters() + ", " +
					delem.AttachmentPoint.OwnY.GetTextForParameters() + ", " +
					delem.AttachmentPoint.OwnZ.GetTextForParameters() + ", on own " +
					delem.AttachmentPoint.OwnX.GetTextForParameters() + ", " +
					delem.AttachmentPoint.OwnY.GetTextForParameters() + ", " +
					delem.AttachmentPoint.OwnZ.GetTextForParameters();
			}
			else
			{
				atttext = "Midpoint: " + delem.Midpoint.X.ToString() + ", " + delem.Midpoint.Y.ToString() + ", " + delem.Midpoint.Z.ToString();
			}

			return atttext;
		}

		private ListViewItem lGetLVItemForCurrentDesignElement()
		{
			ListViewItem lvitem = null;

			if ((lCurrDesignElement != null) && (lPartsLvw.SelectedItems.Count == 1) && lCurrDesignElement.Name.Equals(lPartsLvw.SelectedItems[0].Text))
			{
				lvitem = lPartsLvw.SelectedItems[0];
			}

			return lvitem;
		}

		private void lSetAttachmentPointByAttachmentName(string name)
		{
			if (lPrimaryAttachmentCbx.Items.Count > 0)
			{
				int cbxndx = 0;

				foreach (object tmpobj in lPrimaryAttachmentCbx.Items)
				{
					if (((string)tmpobj).Equals(name))
					{
						break;
					}
					cbxndx++;
				}

				if (cbxndx < lPrimaryAttachmentCbx.Items.Count)
				{
					bool uiupdate = lChangingFromControl;
					lChangingFromControl = true;
					lPrimaryAttachmentCbx.SelectedIndex = cbxndx;
					lChangingFromControl = uiupdate;
				}
			}
		}

		private void lTSResetPosBtn_Click(object sender, EventArgs e)
		{
			if (lDesignObject != null)
			{
				lDesignCtrl.ResetPosition();
			}
		}

		private void lPartNameTbx_TextChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lPartNameTbx.Text.Length > 0))
			{
				// GWH UNFINISHED CODE--CHECK TO SEE IF NAME EXISTS ON ANOTHER PART, CHANGE COLOR TO RED AND DON'T UPDATE NAME IF IT DOES
				if (lPartsLvw.SelectedItems.Count == 1)
				{
					lPartsLvw.SelectedItems[0].Text = lPartNameTbx.Text;
				}
				string priorname = lCurrDesignElement.Name;
				lCurrDesignElement.Name = lPartNameTbx.Text;
				lDesignCtrl.ForceRedraw();

				// update attachment combo box
				int tmpndx = 0;

				foreach (object tmpobj in lPrimaryAttachmentCbx.Items)
				{
					if (priorname.Equals((string)tmpobj))
					{
						break;
					}
					tmpndx++;
				}

				if (tmpndx < lPrimaryAttachmentCbx.Items.Count)
				{
					lPrimaryAttachmentCbx.Items[tmpndx] = lCurrDesignElement.Name;
				}
			}
		}

		private void lPartWidthTbx_TextChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lPartWidthTbx.Text.Length > 0))
			{
				if (lPartsLvw.SelectedItems.Count == 1)
				{
					lPartsLvw.SelectedItems[0].SubItems[1].Text = lPartWidthTbx.Text;
				}
				lCurrDesignElement.DimensionX = Convert.ToDouble(lPartWidthTbx.Text);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartHeightTbx_TextChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lPartHeightTbx.Text.Length > 0))
			{
				if (lPartsLvw.SelectedItems.Count == 1)
				{
					lPartsLvw.SelectedItems[0].SubItems[2].Text = lPartHeightTbx.Text;
				}
				lCurrDesignElement.DimensionZ = Convert.ToDouble(lPartHeightTbx.Text);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartDepthTbx_TextChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lPartDepthTbx.Text.Length > 0))
			{
				if (lPartsLvw.SelectedItems.Count == 1)
				{
					lPartsLvw.SelectedItems[0].SubItems[3].Text = lPartDepthTbx.Text;
				}
				lCurrDesignElement.DimensionY = Convert.ToDouble(lPartDepthTbx.Text);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartMidXTbx_TextChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null))
			{
				double midx = StringHandler.SafeStringToDouble(lPartMidXTbx.Text);
				// GWH DEBUG--
				System.Diagnostics.Debug.WriteLine("lPartMidXTbx_TextChanged:  trace");
				// --GWH DEBUG

				if (lCurrDesignElement.AttachmentPoint != null)
					lCurrDesignElement.AttachmentPoint.OwnX.Value = midx;
				else
					lCurrDesignElement.Midpoint.X = midx;

				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartMidYTbx_TextChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null))
			{
				if (lCurrDesignElement.AttachmentPoint != null)
				{
					// GWH DEBUG--
					System.Diagnostics.Debug.WriteLine("lPartMidYTbx_TextChanged:  attachment");
					// --GWH DEBUG
					lCurrDesignElement.AttachmentPoint.OwnY.Value = StringHandler.SafeStringToDouble(lPartMidYTbx.Text);
				}
				else
				{
					// GWH DEBUG--
					System.Diagnostics.Debug.WriteLine("lPartMidYTbx_TextChanged:  midpoint");
					// --GWH DEBUG
					lCurrDesignElement.Midpoint.Y = StringHandler.SafeStringToDouble(lPartMidYTbx.Text);
				}

				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartMidZTbx_TextChanged(object sender, EventArgs e)
		{
			// GWH DEBUG--
			System.Diagnostics.Debug.WriteLine("lPartMidZTbx_TextChanged:  trace");
			// --GWH DEBUG
			if ((!lChangingFromControl) && (lCurrDesignElement != null))
			{
				if (lCurrDesignElement.AttachmentPoint != null)
				{
					lCurrDesignElement.AttachmentPoint.OwnZ.Value = StringHandler.SafeStringToDouble(lPartMidZTbx.Text);
				}
				else
				{
					lCurrDesignElement.Midpoint.Z = StringHandler.SafeStringToDouble(lPartMidZTbx.Text);
				}

				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartAttXTbx_TextChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null))
			{
				lCurrDesignElement.AttachmentPoint.AttX.Value = StringHandler.SafeStringToDouble(lPartAttXTbx.Text);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartAttYTbx_TextChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null))
			{
				lCurrDesignElement.AttachmentPoint.AttY.Value = StringHandler.SafeStringToDouble(lPartAttYTbx.Text);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartAttZTbx_TextChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null))
			{
				lCurrDesignElement.AttachmentPoint.AttZ.Value = StringHandler.SafeStringToDouble(lPartAttZTbx.Text);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPlaceByMidpointRbtn_CheckedChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lPartDepthTbx.Text.Length > 0))
			{
				// GWH DEBUG--
				System.Diagnostics.Debug.WriteLine("lPlaceByMidpointRbtn_CheckedChanged:  trace");
				// --GWH DEBUG
				lChangingFromControl = true;

				if (lPlaceByMidpointRbtn.Checked)
				{
					lSetPositionByRelationControlsState(true);

					lPartMidXTbx.Text = lCurrDesignElement.Midpoint.X.ToString();
					lPartMidYTbx.Text = lCurrDesignElement.Midpoint.Y.ToString();
					lPartMidZTbx.Text = lCurrDesignElement.Midpoint.Z.ToString();

					lCurrDesignElement.AttachmentPoint = null;
				}
				else
				{
					lSetPositionByRelationControlsState(false);

					if (lCurrDesignElement.AttachmentPoint == null)
					{
						if (lPrimaryAttachmentCbx.Items.Count > 0)
						{
							lPrimaryAttachmentCbx.SelectedIndex = 0;
							lCurrDesignElement.AttachmentPoint = new DesignAttachmentParameters(lPrimaryAttachmentCbx.Text);
							lCurrDesignElement.AttachmentPoint.Attachment = lDesignObject.GetDesignElementByName(lPrimaryAttachmentCbx.Text);
						}
						else
						{
							lCurrDesignElement.AttachmentPoint = new DesignAttachmentParameters();
						}
					}

					lSetAttachmentControlsByCurrentDesignElement();
				}

				lChangingFromControl = false;

				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);

				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private int lGetAttPtCbxFromDesignPoint(DesignPoint dpt)
		{
			int selndx = (dpt.Origin == DesignPoint.OriginType.Midpoint ? 2 : 0);

			if (dpt.Direction > 0.0F)
				selndx++;

			return selndx;
		}

		private void lSetDesignPointFromAttPtCbx(DesignPoint dpt, ComboBox cbx)
		{
			dpt.Direction = ((cbx.SelectedIndex & 0x1) == 0x1 ? 1.0F : -1.0F);
			dpt.Origin = (cbx.SelectedIndex < 2 ? DesignPoint.OriginType.Edge : DesignPoint.OriginType.Midpoint);
		}

		private void lPartMidXCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null) && (lPartMidXCbx.SelectedIndex >= 0))
			{
				lSetDesignPointFromAttPtCbx(lCurrDesignElement.AttachmentPoint.OwnX, lPartMidXCbx);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartMidYCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null) && (lPartMidYCbx.SelectedIndex >= 0))
			{
				lSetDesignPointFromAttPtCbx(lCurrDesignElement.AttachmentPoint.OwnY, lPartMidYCbx);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartMidZCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null) && (lPartMidZCbx.SelectedIndex >= 0))
			{
				lSetDesignPointFromAttPtCbx(lCurrDesignElement.AttachmentPoint.OwnZ, lPartMidZCbx);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartAttXCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null) && (lPartAttXCbx.SelectedIndex >= 0))
			{
				lSetDesignPointFromAttPtCbx(lCurrDesignElement.AttachmentPoint.AttX, lPartAttXCbx);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartAttYCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null) && (lPartAttYCbx.SelectedIndex >= 0))
			{
				lSetDesignPointFromAttPtCbx(lCurrDesignElement.AttachmentPoint.AttY, lPartAttYCbx);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lPartAttZCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null) && (lPartAttZCbx.SelectedIndex >= 0))
			{
				lSetDesignPointFromAttPtCbx(lCurrDesignElement.AttachmentPoint.AttZ, lPartAttZCbx);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lNewPartBtn_Click(object sender, EventArgs e)
		{
			if (lDesignObject != null)
			{
				// add generic new part to design object
				DesignElementRect newrect = new DesignElementRect();
				newrect.Name = "New Rect";  // GWH UNFINISHED CODE--FORCE UNIQUE NAME
				newrect.DimensionX = 1.0F;
				newrect.DimensionY = 1.0F;
				newrect.DimensionZ = 1.0F;
				lDesignObject.Elements.Add(newrect);

				lPartsLvw.BeginUpdate();
				lPartsLvwClearSelection();
				ListViewItem newlv = lAddDesignElement(newrect);
				newlv.Selected = true;
				lPartsLvw.EndUpdate();

				// force redraw, add item to listview and set listview selected index to the new item
				newrect.UpdatePoints();
				lDesignCtrl.ForceRedraw();
				lCurrDesignElement = newrect;
			}
		}

		private void lDuplicatePartBtn_Click(object sender, EventArgs e)
		{
			if ((lDesignObject != null) && (lCurrDesignElement != null))
			{
				// add generic new part to design object
				DesignElementRect newrect = (DesignElementRect)lCurrDesignElement.Duplicate();
				newrect.Name = "New Rect";  // GWH UNFINISHED CODE--FORCE UNIQUE NAME
				lDesignObject.Elements.Add(newrect);

				lPartsLvw.BeginUpdate();
				lPartsLvwClearSelection();
				ListViewItem newlv = lAddDesignElement(newrect);
				newlv.Selected = true;
				lPartsLvw.EndUpdate();

				// force redraw, add item to listview and set listview selected index to the new item
				newrect.UpdatePoints();
				lDesignCtrl.ForceRedraw();
				lCurrDesignElement = newrect;
			}
		}

		private void lDeletePartBtn_Click(object sender, EventArgs e)
		{
			if ((lDesignObject != null) && (lCurrDesignElement != null))
			{
				ListViewItem tmplv = lGetLVItemForCurrentDesignElement();
				lDesignObject.Elements.Remove(lCurrDesignElement);

				lPartsLvw.BeginUpdate();
				lPartsLvw.Items.Remove(tmplv);
				lPartsLvw.EndUpdate();

				// force redraw, add item to listview and set listview selected index to the new item
				lDesignCtrl.ForceRedraw();
				lCurrDesignElement = null;
			}
		}

		private ListViewItem lAddDesignElement(DesignElementRect delem)
		{
			ListViewItem tmplv = new ListViewItem(delem.Name);

			tmplv.SubItems.Add(delem.DimensionX.ToString());
			tmplv.SubItems.Add(delem.DimensionY.ToString());
			tmplv.SubItems.Add(delem.DimensionZ.ToString());
			tmplv.SubItems.Add(lGetAttachmentLVStringText(delem));

			lPartsLvw.Items.Add(tmplv);

			lPrimaryAttachmentCbx.Items.Add(delem.Name);

			return tmplv;
		}

		private void lPartsLvwClearSelection()
		{
			if (lPartsLvw.SelectedItems.Count > 0)
			{
				lPartsLvw.SelectedItems[0].Selected = false;

				//foreach (ListViewItem tmplv in lPartsLvw.SelectedItems)
				//{
				//	tmplv.Selected = false;
				//}
			}
		}

		private void lPrimaryAttachmentCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((!lChangingFromControl) && (lCurrDesignElement != null) && (lCurrDesignElement.AttachmentPoint != null) && (lPrimaryAttachmentCbx.SelectedIndex >= 0))
			{
				lCurrDesignElement.AttachmentPoint.Attachment = lDesignObject.GetDesignElementByName(lPrimaryAttachmentCbx.Text);
				ListViewItem lvitem = lGetLVItemForCurrentDesignElement();
				lvitem.SubItems[4].Text = lGetAttachmentLVStringText(lCurrDesignElement);
				lDesignObject.UpdatePointsForElement(lCurrDesignElement);
				lDesignCtrl.ForceRedraw();
			}
		}

		private void lTSPrintBtn_Click(object sender, EventArgs e)
		{

		}

		private void lTSShowAxesCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			lAppPrefs.ShowAxes = (lTSShowAxesCbx.SelectedIndex == 0 ? true : false);
			lDesignCtrl.ShowAxes = lAppPrefs.ShowAxes;
		}

		private void lTSReloadBtn_Click(object sender, EventArgs e)
		{
			if ((lDesignFilePath.Length > 0) && File.Exists(lDesignFilePath))
			{
				lDesignLoad();
			}
			else
			{
				MessageBox.Show("You must first save the current design.");
			}
		}

		private void lTSViewReportBtn_Click(object sender, EventArgs e)
		{
			PrintOutputForm printfrm = new PrintOutputForm();
			printfrm.SetDesignObject(lDesignObject);
			printfrm.Show();
		}

		private void lTSShowLabelsCbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			lAppPrefs.ShowLabels = (lTSShowLabelsCbx.SelectedIndex == 0 ? true : false);
			lDesignCtrl.ShowLabels = lAppPrefs.ShowLabels;
		}

		private void lTSSaveAsBtn_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();

			sfd.Title = "Save design as...";
			if (lDesignFilePath.Length > 0)
			{
				sfd.InitialDirectory = Path.GetDirectoryName(lDesignFilePath);
				sfd.FileName = Path.GetFileName(lDesignFilePath);
			}
			sfd.Filter = "All Files (*.*)|*.*";
			sfd.FilterIndex = 1;

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				lDesignFilePath = sfd.FileName;

				if (lDesignObject.Name.Length == 0)
				{
					lDesignObject.Name = Path.GetFileNameWithoutExtension(lDesignFilePath);
				}

				if (lDesignObject.SaveDesignObject(lDesignFilePath))
				{
					MessageBox.Show("Saved design.");
				}
				else
				{
					MessageBox.Show("ERROR:  Failed to save design.");
				}
			}
		}
	}
}
