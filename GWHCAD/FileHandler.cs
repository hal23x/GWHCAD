/************************************************************************************

	Project:
		GWHCAD

	File:
		FileHandler.cs

	Description:
		This souce file contains a standard file wrapper class.

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

namespace GWHCAD
{
    public class FileHandler
    {
		#region Private Static Variables
		private static string lLastErrorText = "";
		#endregion

		#region Public Static Variables
		public static string LastErrorText
		{
			get { return lLastErrorText; }
		}
		#endregion

		#region Public Static Methods
		/********************************************************************************
			FileHandler.DirectoryCompare()

			Description:
				Performs a fully-recursive directory comparison, comparing all files and
				folders beneath.

			Parameters:
				string frompath
					The full path of the directory to copy.
				string topath
					The full path of the directory to which the directory "frompath"
					will be copied.
				string[] filters
					An array of entries to filter.  These must be exactly how the path
					will end--no wildcards are available at this time.

			Return Value:
				A boolean indicating whether or not an error occurred.
		********************************************************************************/
		public static bool DirectoryCompare(string frompath, string topath, string[] filters)
		{
			bool cancel = false;
			return FileHandler.DirectoryCompare(frompath, topath, filters, ref cancel);
		}


		/********************************************************************************
			FileHandler.DirectoryCompare()

			Description:
				Performs a fully-recursive directory comparison, comparing all files and
				folders beneath.

			Parameters:
				string frompath
					The full path of the directory to copy.
				string topath
					The full path of the directory to which the directory "frompath"
					will be copied.
				string[] filters
					An array of entries to filter.  These must be exactly how the path
					will end--no wildcards are available at this time.
				ref bool cancel
					A reference to a boolean that will allow the operation to be
					stopped without finishing.

			Return Value:
				A boolean indicating whether or not an error occurred.
		********************************************************************************/
		public static bool DirectoryCompare(string frompath, string topath, string[] filters, ref bool cancel)
		{
			bool retval = true;

			try
			{
				string[] fromdircontents = Directory.GetFileSystemEntries(frompath);
				string[] todircontents = Directory.GetFileSystemEntries(frompath);

				if (fromdircontents.Length != todircontents.Length)
				{
					retval = false;
				}
				else if (fromdircontents.Length > 0)
				{
					foreach (string tmpname in fromdircontents)
					{
						if (!cancel)
						{
							bool filterfile = false;

							if ((filters != null) && (filters.Length > 0))
							{
								foreach (string tmpfilter in filters)
								{
									if ((tmpfilter != null) && (tmpfilter.Length > 0))
									{
										if (tmpname.ToLower().EndsWith(tmpfilter.ToLower()))
										{
											filterfile = true;
											break;
										}
									}
								}
							}

							if ((!cancel) && (!filterfile))
							{
								string entryname = FileHandler.GetFileNameFromPath(tmpname);
								string toentry = topath + "\\" + entryname;

								if (Directory.Exists(tmpname))
								{
									if (Directory.Exists(toentry))
									{
										retval = FileHandler.DirectoryCompare(tmpname, toentry, filters, ref cancel);
									}
									else
									{
										retval = false;
									}
								}
								else if (File.Exists(tmpname))
								{
									if (!File.Exists(toentry))
									{
										retval = false;
									}
									else if (FileHandler.GetFileSize(tmpname) != FileHandler.GetFileSize(toentry))
									{
										retval = false;
									}
								}

								if (!retval)
								{
									break;
								}
							}
						}

						if (cancel)
						{
							retval = false;
							break;
						}
					}
				}
			}
			catch (Exception /*ex*/)
			{
				retval = false;
			}

			return retval;
		}


		/********************************************************************************
			FileHandler.DirectoryCopy()

			Description:
				Performs a fully-recursive directory copy.

			Parameters:
				string frompath
					The full path of the directory to copy.
				string topath
					The full path of the directory to which the directory "frompath"
					will be copied.

			Return Value:
				A boolean indicating whether or not an error occurred.
		********************************************************************************/
		public static bool DirectoryCopy(string frompath, string topath)
		{
			bool cancel = false;
			string[] filters = null;
			return FileHandler.DirectoryCopy(frompath, topath, filters, ref cancel);
		}


		/********************************************************************************
			FileHandler.DirectoryCopy()

			Description:
				Performs a fully-recursive directory copy, with optional file/directory
				entry filter.

			Parameters:
				string frompath
					The full path of the directory to copy.
				string topath
					The full path of the directory to which the directory "frompath"
					will be copied.
				string[] filters
					An array of entries to filter.  These must be exactly how the path
					will end--no wildcards are available at this time.

			Return Value:
				A boolean indicating whether or not an error occurred.
		********************************************************************************/
		public static bool DirectoryCopy(string frompath, string topath, string[] filters)
		{
			bool cancel = false;
			return FileHandler.DirectoryCopy(frompath, topath, filters, ref cancel);
		}


		/********************************************************************************
			FileHandler.DirectoryCopy()

			Description:
				Performs a fully-recursive directory copy, with optional file/directory
				entry filter.

			Parameters:
				string frompath
					The full path of the directory to copy.
				string topath
					The full path of the directory to which the directory "frompath"
					will be copied.
				string[] filters
					An array of entries to filter.  These must be exactly how the path
					will end--no wildcards are available at this time.
				ref bool cancel
					A reference to a boolean that will allow the operation to be
					stopped without finishing.

			Return Value:
				A boolean indicating whether or not an error occurred.
		********************************************************************************/
		public static bool DirectoryCopy(string frompath, string topath, string[] filters, ref bool cancel)
		{
			bool retval = true;

			try
			{
				string[] dircontents = Directory.GetFileSystemEntries(frompath);

				if (dircontents.Length > 0)
				{
					foreach (string tmpname in dircontents)
					{
						if (!cancel)
						{
							bool filterfile = false;

							if ((filters != null) && (filters.Length > 0))
							{
								foreach (string tmpfilter in filters)
								{
									if ((tmpfilter != null) && (tmpfilter.Length > 0))
									{
										if (tmpname.ToLower().EndsWith(tmpfilter.ToLower()))
										{
											filterfile = true;
											break;
										}
									}
								}
							}

							if ((!cancel) && (!filterfile))
							{
								string entryname = FileHandler.GetFileNameFromPath(tmpname);
								string toentry = topath + "\\" + entryname;

								if (Directory.Exists(tmpname))
								{
									Directory.CreateDirectory(toentry);
									retval = FileHandler.DirectoryCopy(tmpname, toentry, filters, ref cancel);
								}
								else if (File.Exists(tmpname))
								{
									File.Copy(tmpname, toentry);
								}

								if (!retval)
								{
									break;
								}
							}
						}

						if (cancel)
						{
							retval = false;
							break;
						}
					}
				}
			}
			catch (Exception /*ex*/)
			{
				retval = false;
			}

			return retval;
		}


		/********************************************************************************
			FileHandler.DirectoryDelete()

			Description:
				Performs a fully-recursive directory delete.  Solves some issues that
				System.IO.Directory.Delete() has.

			Parameters:
				string dirpath
					The full path of the directory to delete.

			Return Value:
				A boolean indicating whether or not an error occurred.
		********************************************************************************/
		public static bool DirectoryDelete(string dirpath)
		{
			bool cancel = false;
			return FileHandler.DirectoryDelete(dirpath, ref cancel);
		}


		/********************************************************************************
			FileHandler.DirectoryDelete()

			Description:
				Performs a fully-recursive directory delete.  Solves some issues that
				System.IO.Directory.Delete() has.

			Parameters:
				string dirpath
					The full path of the directory to delete.
				ref bool cancel
					A reference to a boolean that will allow the operation to be
					stopped without finishing.

			Return Value:
				A boolean indicating whether or not an error occurred.
		********************************************************************************/
		public static bool DirectoryDelete(string dirpath, ref bool cancel)
		{
			bool retval = true;

			try
			{
				string[] dircontents = Directory.GetFileSystemEntries(dirpath);

				if (dircontents.Length > 0)
				{
					foreach (string tmpname in dircontents)
					{
						if (!cancel)
						{
							if (Directory.Exists(tmpname))
							{
								retval = FileHandler.DirectoryDelete(tmpname, ref cancel);
							}
							else if (File.Exists(tmpname))
							{
								File.SetAttributes(tmpname, FileAttributes.Normal);
								System.Threading.Thread.Sleep(5);
								File.Delete(tmpname);
								System.Threading.Thread.Sleep(5);
							}

							if (!retval)
							{
								break;
							}
						}

						if (cancel)
						{
							retval = false;
							break;
						}
					}
				}

				if (retval)
				{
					System.Threading.Thread.Sleep(5);
					Directory.Delete(dirpath);
					System.Threading.Thread.Sleep(5);
				}
			}
			catch (Exception /*ex*/)
			{
				retval = false;
			}

			return retval;
		}


		/********************************************************************************
			 FileHandler.GetByteArrayFromFile()

			 Description:
				 Returns the file name (or last directory name) from the given full path.

			 Parameters:
				 string fullpath
					 The full path from which to find the file name.

			 Return Value:
				 A string containing the file name without the path.
		********************************************************************************/
		public static byte[] GetByteArrayFromFile(string filename, uint offset, uint size)
		{
			byte[] bytearray = null;

			if (File.Exists(filename))
			{
				FileInfo fptr = new FileInfo(filename);
				long fsize = fptr.Length;

				if (fsize >= (long)(offset + size))
				{
					BinaryReader srcbin = null;

					try
					{
						bytearray = new byte[size];
						srcbin = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read));
						srcbin.BaseStream.Position = offset;
						srcbin.Read(bytearray, (int)0, (int)size);
					}
					catch (Exception ex)
					{
						bytearray = null;
						lLastErrorText = "Error in FileHandler.GetByteArrayFromFile():  filename=\"" + filename + "\", offset=0x" + offset.ToString("X") + ", size=0x" + size.ToString("X") + ", exception: " + ex.ToString();
					}
					finally
					{
						if (srcbin != null)
						{
							srcbin.Close();
						}
					}
				}
				else
				{
					lLastErrorText = "Error in FileHandler.GetByteArrayFromFile():  filename=\"" + filename + "\", offset=0x" + offset.ToString("X") + ", size=0x" + size.ToString("X") + ", file size is less than the end area to be read (file size reported as 0x" + fsize.ToString("X") + " bytes).";
				}
			}
			else
			{
				lLastErrorText = "Error in FileHandler.GetByteArrayFromFile():  filename=\"" + filename + "\", offset=0x" + offset.ToString("X") + ", size=0x" + size.ToString("X") + ", file does not exist.";
			}

			return bytearray;
		}


		/********************************************************************************
			 FileHandler.GetFileNameFromPath()

			 Description:
				 Returns the file name (or last directory name) from the given full path.

			 Parameters:
				 string fullpath
					 The full path from which to find the file name.

			 Return Value:
				 A string containing the file name without the path.
		 ********************************************************************************/
		public static string GetFileNameFromPath(string fullpath)
        {
            string retstr = fullpath;

            if ((fullpath != null) && (fullpath.Length > 0))
            {
                int lastterm = fullpath.LastIndexOf('\\');

                if (lastterm != -1)
                {
                    retstr = fullpath.Substring(lastterm + 1);
                }
            }

            return (retstr);
        }


        /********************************************************************************
			FileHandler.GetFilePathFromPath()

			Description:
				Returns the directory path from the given full file path.

			Parameters:
				string fullpath
					The full path from which to find the file name.

			Return Value:
				A string containing the file name without the path.
		********************************************************************************/
        public static string GetFilePathFromPath(string fullpath)
        {
            string retstr = fullpath;

            if ((fullpath != null) && (fullpath.Length > 0))
            {
                int lastterm = fullpath.LastIndexOf('\\');

                if (lastterm > 0)
                {
                    retstr = fullpath.Substring(0, lastterm);
                }
            }

            return (retstr);
        }


		/********************************************************************************
			FileHandler.GetFileSize()

			Description:
				Returns the file's size.

			Parameters:
				string filename
					The name of the file.

			Return Value:
				The size of the file, in bytes.
		********************************************************************************/
		public static int GetFileSize(string filename)
		{
			int fsize = 0;

			if (File.Exists(filename))
			{
				FileInfo fptr = new FileInfo(filename);
				fsize = (int)fptr.Length;
			}

			return fsize;
		}


		/********************************************************************************
			FileHandler.GetTemporaryPath()

			Description:
				Returns the user's temporary path.

			Parameters:
				None.

			Return Value:
				The temporary path.
		********************************************************************************/
		public static string GetTemporaryPath()
		{
			return Path.GetTempPath();
		}


		/********************************************************************************
			FileHandler.LoadBinaryFile()

			Description:
				Attempts to load the specified file.

			Parameters:
				string filename
					The name of the local file (may include path, absolute or relative).

			Return Value:
				A managed pointer to the the array of bytes containing the file contents.
				If an error occurs, the value will be nullptr.
		********************************************************************************/
		public static byte[] LoadBinaryFile(string filename)
		{
			byte[] fbuff = null;
			int fsize = FileHandler.GetFileSize(filename);

			if (fsize > 0)
			{
				BinaryReader srcbin = null;

				// attempt to catch any file exceptions
				try
				{
					srcbin = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read));
					fbuff = srcbin.ReadBytes(fsize);
				}
				catch (Exception /*ex*/)
				{
					fbuff = null;
				}
				finally
				{
					if (srcbin != null)
					{
						srcbin.Close();
					}
				}
			}
			else if (fsize == 0)
			{
				fbuff = new byte[0];
			}

			return fbuff;
		}


		/********************************************************************************
			FileHandler.LoadFile()

			Description:
				Attempts to load the specified file.

			Parameters:
				string filename
					The name of the local file (may include path, absolute or relative).

			Return Value:
				A managed array of strings containing the file lines.  If an error
				occurs, the value will be null.
		********************************************************************************/
		public static string[] LoadFile(string filename)
		{
			string[] arrayptr = null;

			// attempt to catch any file exceptions
			try
			{
				using (StreamReader infile = new StreamReader(filename))
				{
					List<string> strlist = new List<string>();
					string nextline;

					// loop through entire file
					while ((nextline = infile.ReadLine()) != null)
					{
						strlist.Add(nextline);
					}

					// create the string array
					arrayptr = new string[strlist.Count];

					// fill in the string array
					int ndx = 0;
					foreach (string stritr in strlist)
					{
						if (stritr != null)
						{
							arrayptr[ndx++] = stritr;
						}
					}
				}
			}
			catch (Exception /*ex*/)
			{
				arrayptr = null;
			}

			// return the array value
			return arrayptr;
		}


		/********************************************************************************
			FileHandler.SaveBinaryFile()

			Description:
				Attempts to save the specified binary file.

			Parameters:
				string filename
					A managed pointer to the name of the file, including path.
				byte[] buffer
					A managed pointer to the the array of bytes containing the file
					buffer.

			Return Value:
				A boolean indicating true if the save completed successfully, or false if
				an error occurred.
		********************************************************************************/
		public static bool SaveBinaryFile(string filename, byte[] buffer)
		{
			bool retval = false;

			if ((filename != null) && (filename.Length > 0) && (buffer != null) && (buffer.Length > 0))
			{
				try
				{
					FileStream outfs = new FileStream(filename, FileMode.Create, FileAccess.Write);
					BinaryWriter outbw = new BinaryWriter(outfs);
					outbw.Write(buffer);
					outbw.Flush();
					outbw.Close();
					retval = true;
				}
				catch (Exception /*ex*/)
				{
					retval = false;
				}
			}

			return retval;
		}


		/********************************************************************************
			FileHandler.SaveFile()

			Description:
				Attempts to save the specified text file.

			Parameters:
				string filename
					The name of the file, including path.
				string[] filetext
					A managed array of strings containing the file text.

			Return Value:
				A boolean indicating true if the save completed successfully, or false if
				an error occurred.
		********************************************************************************/
		public static bool SaveFile(string filename, string[] filetext)
		{
			bool retval = false;

			if ((filename != null) && (filename.Length > 0) && (filetext != null))
			{
				try
				{
					FileStream		fstrm = new FileStream(filename, FileMode.Create, FileAccess.Write);
					StreamWriter	outfile = new StreamWriter(fstrm);
					int				linecnt = 0, linetot = filetext.Length;

					// loop through entire file
					while (linecnt < linetot)
					{
						if (linecnt == (linetot - 1))
							outfile.Write(filetext[linecnt]);
						else
							outfile.WriteLine(filetext[linecnt]);
						linecnt++;
					}

					outfile.Flush();
					outfile.Close();
					retval = true;
				}
				catch (IOException /*ioex*/)
				{
					// do nothing
				}
			}

			return retval;
		}


		/********************************************************************************
			FileHandler.SaveFile()

			Description:
				Attempts to save the specified text file.

			Parameters:
				string filename
					The name of the file, including path.
				List<string> filetext
					A managed list of strings containing the file text.

			Return Value:
				A boolean indicating true if the save completed successfully, or false if
				an error occurred.
		********************************************************************************/
		public static bool SaveFile(string filename, List<string> filetext)
		{
			bool retval = false;

			if ((filename != null) && (filename.Length > 0) && (filetext != null))
			{
				try
				{
					FileStream fstrm = new FileStream(filename, FileMode.Create, FileAccess.Write);
					StreamWriter outfile = new StreamWriter(fstrm);
					int linecnt = 0, linetot = filetext.Count;

					// loop through entire file
					while (linecnt < linetot)
					{
						if (linecnt == (linetot - 1))
							outfile.Write(filetext[linecnt]);
						else
							outfile.WriteLine(filetext[linecnt]);
						linecnt++;
					}

					outfile.Flush();
					outfile.Close();
					retval = true;
				}
				catch (IOException /*ioex*/)
				{
					// do nothing
				}
			}

			return retval;
		}
		#endregion
	}
}
