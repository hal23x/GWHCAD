/************************************************************************************

	Project:
		GWHCAD

	File:
		StringHandler.cs

	Description:
		This souce file contains a standard string operations class.

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
using System.Text.RegularExpressions;

namespace GWHCAD
{
	public class StringHandler
	{
		#region Public Static Variables
		public static readonly Char[] WhiteSpace = new Char[] { ' ', '\t' };
		public static readonly Char[] LineEndings = new Char[] { '\r', '\n' };
		#endregion

		#region Public Static Methods

		/********************************************************************************
			StringHandler.AddStringToStartOfList()

			Description:
				Adds the given string to the given list, placing it as the first item.
				If the string already exists in the list, the original reference is moved
				to the first position.  If the string is added to the list and the list
				exceeds a predetermined length, the last entry will be removed.

			Parameters:
				List<string> stringlist
					The string list to which to add the string.
				string newstring
					The string to add to the list.
				int maxcount
					The maximum number of items to allow in the list.  If 0 or negative,
					the list will not be shortened.

			Return Value:
				An integer indicating the new length of the list.
		********************************************************************************/
		public static int AddStringToStartOfList(List<string> stringlist, string newstring, int maxcount)
		{
			int retval = 0;

			if (stringlist != null)
			{
				// determine if string needs to be removed
				if (stringlist.Count > 0)
				{
					int strndx = 0;

					foreach (string tmpstr in stringlist)
					{
						if ((tmpstr != null) && (tmpstr.Equals(newstring)))
						{
							break;
						}

						strndx++;
					}

					if (strndx < stringlist.Count)
					{
						stringlist.RemoveAt(strndx);
					}
				}

				stringlist.Insert(0, newstring);

				if ((maxcount > 0) && (maxcount < stringlist.Count))
				{
					int rmcnt = stringlist.Count - maxcount;

					stringlist.RemoveRange(maxcount, rmcnt);
				}

				retval = stringlist.Count;
			}

			return retval;
		}


		/********************************************************************************
			StringHandler.IsStringInArray()

			Description:
				Returns whether or not a specified string value is in the given array.

			Parameters:
				string[] stringarray
					The string array to search within.
				string searchstring
					The string to search for in the array.

			Return Value:
				A boolean indicating true if the string was present in the array, or
				false if not.
		********************************************************************************/
		public static bool IsStringInArray(string[] stringarray, string searchstring)
		{
			bool retval = false;

			if ((stringarray != null) && (stringarray.Length > 0))
			{
				foreach (string tmpstr in stringarray)
				{
					if ((tmpstr != null) && (tmpstr.Equals(searchstring)))
					{
						retval = true;
						break;
					}
				}
			}

			return retval;
		}


		/********************************************************************************
			StringHandler.IsStringInList()

			Description:
				Returns whether or not a specified string value is in the given list.

			Parameters:
				List<string> stringlist
					The string list to search within.
				string searchstring
					The string to search for in the array.

			Return Value:
				A boolean indicating true if the string was present in the array, or
				false if not.
		********************************************************************************/
		public static bool IsStringInList(List<string> stringlist, string searchstring)
		{
			bool retval = false;

			if ((stringlist != null) && (stringlist.Count > 0))
			{
				foreach (string tmpstr in stringlist)
				{
					if ((tmpstr != null) && (tmpstr.Equals(searchstring)))
					{
						retval = true;
						break;
					}
				}
			}

			return retval;
		}


		/********************************************************************************
			StringHandler.RegExMatch()

			Description:
				Returns an array of matches to the given regular expression string found
				in the given input string.

			Parameters:
				string inputstring
					The input string in which to find regular expression matches.
				string regexstring
					The regular expression to be searched for.

			Return Value:
				An array of all matching values.  This array will include all groups, if
				the regular expression string was found multiple times.
		********************************************************************************/
		public static string[] RegExMatch(string inputstring, string regexstring)
		{
			string[] outarray = null;

			if ((inputstring != null) && (regexstring != null))
			{
				MatchCollection matches = Regex.Matches(inputstring, regexstring);

				if (matches.Count > 0)
				{
					List<string> strlist = new List<string>();

					foreach (Match tmpmatch in matches)
					{
						int grpno = 0;

						foreach (Group tmpgrp in tmpmatch.Groups)
						{
							if (grpno > 0)
							{
								strlist.Add(tmpgrp.Value);
							}

							grpno++;
						}
					}

					outarray = new string[strlist.Count];

					if (strlist.Count > 0)
					{
						int listndx = 0;

						foreach (string tmpstr in strlist)
						{
							outarray[listndx] = tmpstr;
							listndx++;
						}
					}
				}
			}

			return outarray;
		}


		/********************************************************************************
			StringHandler.SafeStringToInt32()

			Description:
				Returns the integer value of the given string.

			Parameters:
				string stringval
					The string to convert.

			Return Value:
				The signed integer value of the string.  If the string contained
				non-numeric data, 0 will be returned.
		********************************************************************************/
		public static int SafeStringToInt32(string stringval)
		{
			int valuebase = 10;

			if ((stringval != null) && (stringval.Length > 2) && stringval.StartsWith("0x"))
			{
				stringval = stringval.Substring(2, stringval.Length - 2);
				valuebase = 16;
			}

			return SafeStringToInt32(stringval, valuebase);
		}


		/********************************************************************************
			StringHandler.SafeStringToInt32()

			Description:
				Returns the integer value of the given string.

			Parameters:
				string stringval
					The string to convert.
				int valuebase
					The number base used in the string to represent the number.  Valid
					options are: 2=binary, 8=octal, 10=decimal and 16=hexadecimal.

			Return Value:
				The signed integer value of the string.  If the string contained
				non-numeric data, 0 will be returned.
		********************************************************************************/
		public static int SafeStringToInt32(string stringval, int valuebase)
		{
			int retval = 0;

			if ((stringval != null) && (stringval.Length > 0))
			{
				try
				{
					retval = Convert.ToInt32(stringval, valuebase);
				}
				catch (Exception /*ex*/)
				{
					// do nothing--return value will be 0
				}
			}

			return retval;
		}


		/********************************************************************************
			StringHandler.SafeStringToUInt32()

			Description:
				Returns the unsigned integer value of the given string.  The number is
				assumed to be base-10 (decimal), unless it starts with '0x' indicating it
				is base-16 (hexadecimal).

			Parameters:
				string stringval
					The string to convert.

			Return Value:
				The unsigned integer value of the string.  If the string contained
				non-numeric data, 0 will be returned.
		********************************************************************************/
		public static uint SafeStringToUInt32(string stringval)
		{
			int valuebase = 10;

			if ((stringval != null) && (stringval.Length > 2) && stringval.StartsWith("0x"))
			{
				stringval = stringval.Substring(2, stringval.Length - 2);
				valuebase = 16;
			}

			return SafeStringToUInt32(stringval, valuebase);
		}


		/********************************************************************************
			StringHandler.SafeStringToUInt32()

			Description:
				Returns the unsigned integer value of the given string.

			Parameters:
				string stringval
					The string to convert.
				int valuebase
					The number base used in the string to represent the number.  Valid
					options are: 2=binary, 8=octal, 10=decimal and 16=hexadecimal.

			Return Value:
				The unsigned integer value of the string.  If the string contained
				non-numeric data, 0 will be returned.
		********************************************************************************/
		public static uint SafeStringToUInt32(string stringval, int valuebase)
		{
			uint retval = 0;

			if ((stringval != null) && (stringval.Length > 0))
			{
				try
				{
					retval = Convert.ToUInt32(stringval, valuebase);
				}
				catch (Exception /*ex*/)
				{
					// do nothing--return value will be 0
				}
			}

			return retval;
		}


		/********************************************************************************
			StringHandler.SafeStringToUInt64()

			Description:
				Returns the unsigned integer value of the given string.  The number is
				assumed to be base-10 (decimal), unless it starts with '0x' indicating it
				is base-16 (hexadecimal).

			Parameters:
				string stringval
					The string to convert.

			Return Value:
				The unsigned integer value of the string.  If the string contained
				non-numeric data, 0 will be returned.
		********************************************************************************/
		public static ulong SafeStringToUInt64(string stringval)
		{
			int valuebase = 10;

			if ((stringval != null) && (stringval.Length > 2) && stringval.StartsWith("0x"))
			{
				stringval = stringval.Substring(2, stringval.Length - 2);
				valuebase = 16;
			}

			return SafeStringToUInt64(stringval, valuebase);
		}


		/********************************************************************************
			StringHandler.SafeStringToUInt64()

			Description:
				Returns the unsigned integer value of the given string.

			Parameters:
				string stringval
					The string to convert.
				int valuebase
					The number base used in the string to represent the number.  Valid
					options are: 2=binary, 8=octal, 10=decimal and 16=hexadecimal.

			Return Value:
				The unsigned integer value of the string.  If the string contained
				non-numeric data, 0 will be returned.
		********************************************************************************/
		public static ulong SafeStringToUInt64(string stringval, int valuebase)
		{
			ulong retval = 0;

			if ((stringval != null) && (stringval.Length > 0))
			{
				try
				{
					retval = Convert.ToUInt64(stringval, valuebase);
				}
				catch (Exception /*ex*/)
				{
					// do nothing--return value will be 0
				}
			}

			return retval;
		}


		/********************************************************************************
			StringHandler.SafeStringToDouble()

			Description:
				Returns the double (64-bit floating point) value of the given string.

			Parameters:
				string stringval
					The string to convert.

			Return Value:
				The double (64-bit floating point) value of the string.  If the string
				contained non-numeric data, 0 will be returned.
		********************************************************************************/
		public static double SafeStringToDouble(string stringval)
		{
			double retval = 0;

			if ((stringval != null) && (stringval.Length > 0))
			{
				try
				{
					retval = Convert.ToDouble(stringval);
				}
				catch (Exception /*ex*/)
				{
					// do nothing--return value will be 0
				}
			}

			return retval;
		}
		#endregion
	}
}
