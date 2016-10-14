

using System.Data;
using System.IO;

using UnityEngine;
using UnityEditor;

using Excel;

using Locke;


public class Xlsx_To_X
{

	private static string xlsxPath = Application.dataPath + "/Xlsx/";
	private static string txtPath = Application.dataPath + "/Resources/Template/";
	private static string csPath = Application.dataPath + "/Scripts/Template/auto/";


	[MenuItem("Locke_Tools/Excel/xlsx -> txt")]
	static void xlsx_to_txt()
	{
		System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
		if (selectedObjects.Length == 0)
			Debug.LogError("u should select at least one .xlsx file.");
		for (int i = 0; i < selectedObjects.Length; i++)
		{
			Object obj = selectedObjects[i] as Object;

			var sheetData = XlsxReader.Instance.AsStringArray(xlsxPath + obj.name + ".xlsx");
			string txt = _to_txt(sheetData);
			System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(txtPath + obj.name + ".txt", false);
			streamwriter.Write(txt);
			streamwriter.Flush();
			streamwriter.Close();
		}

		watch.Stop();

		Debug.Log(string.Format("xlsx -> txt done, {0} files converted. cost {1} ms.", selectedObjects.Length, watch.ElapsedMilliseconds.ToString()));
	}


	[MenuItem("Locke_Tools/Excel/xlsx -> cs")]
	static void xlsx_to_cs()
	{
		System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
		if (selectedObjects.Length == 0)
			Debug.LogError("u should select at least one .xlsx file.");
		for (int i = 0; i < selectedObjects.Length; i++)
		{
			Object obj = selectedObjects[i] as Object;

			var sheetData = XlsxReader.Instance.AsStringArray(xlsxPath + obj.name + ".xlsx");
			string txt = _to_cs(sheetData, obj.name);
			System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(csPath + obj.name + "_Data.cs", false);
			streamwriter.Write(txt);
			streamwriter.Flush();
			streamwriter.Close();
		}

		watch.Stop();

		Debug.Log(string.Format("xlsx -> cs done, {0} files converted. cost {1} ms.", selectedObjects.Length, watch.ElapsedMilliseconds));
	}

	#region _functions

	private static string _to_txt(XlsxReader.SheetData sheetData)
	{
		try
		{
			int rowCount = sheetData.rowCount;
			int columnCount = sheetData.columnCount;
			
			System.Text.StringBuilder convertedString = new System.Text.StringBuilder();
			for (int row = 0; row < rowCount; ++row)
			{
				for (int col = 0; col < columnCount; ++col)
				{
					string cellText = sheetData.At(row, col).Replace("\n", "\\n");
					convertedString.Append(cellText);
					if (col != columnCount - 1)
						convertedString.Append("\t");
				}
				convertedString.Append("\n");
			}
			// remove last '\n'
			convertedString.Remove(convertedString.Length - 1, 1);
			
			return convertedString.ToString();
		}
		catch (System.IO.IOException ex)
		{
			throw new System.IO.IOException(ex.Message);
		}
	}


	private static string _to_cs(XlsxReader.SheetData sheetData, string fileName)
	{
		try
		{
			string csFile = "";

			csFile += "using System;" + "\n";
			csFile += "using System.Collections.Generic;" + "\n\n\n";

			csFile += "namespace Locke" + "\n";
			csFile += "{" + "\n";
			csFile += "\tpublic class " + fileName + "_Data : IData" + "\n";
			csFile += "\t" + "{" + "\n";

			int columnCount = sheetData.columnCount;

			// get variable names from 1st row.
			string[] variableName = new string[columnCount];
			for (int col = 0; col < columnCount; col++)
			{
				variableName[col] = sheetData.At(0, col);
			}

			// Get variableDescribe array from 2nd row
			string[] variableDescribe = new string[columnCount];
			for (int col = 0; col < columnCount; col++)
			{
				variableDescribe[col] = sheetData.At(1, col);
			}

			// Add variableType Info To CS from 3rd row
			string[] variableLength = new string[columnCount];
			string[] variableType = new string[columnCount];
			for (int col = 0; col < columnCount; col++)
			{
				int cellColumnIndex = col;
				if (cellColumnIndex >= 2)
				{
					string cellInfo = sheetData.At(3, col);
					variableLength[cellColumnIndex] = "";
					variableType[cellColumnIndex] = cellInfo;

					if (cellInfo.EndsWith("]"))
					{
						int startIndex = cellInfo.IndexOf('[');
						variableLength[cellColumnIndex] = cellInfo.Substring(startIndex + 1, cellInfo.Length - startIndex - 2);
						variableType[cellColumnIndex] = cellInfo.Substring(0, startIndex);
					}

					if (variableType[cellColumnIndex].Equals("int") || variableType[cellColumnIndex].Equals("float") ||
						variableType[cellColumnIndex].Equals("double") || variableType[cellColumnIndex].Equals("long") ||
						variableType[cellColumnIndex].Equals("string") || variableType[cellColumnIndex].Equals("bool") ||
						variableType[cellColumnIndex].Equals("JObject"))
					{
						if (variableLength[cellColumnIndex].Equals(""))
						{
							csFile += "\t\t" + variableType[cellColumnIndex] + " _" + variableName[cellColumnIndex] + ";\t//" + variableDescribe[cellColumnIndex] + "\n";
							csFile += "\t\tpublic " + variableType[cellColumnIndex] + " " + variableName[cellColumnIndex] + " { get { return _" + variableName[cellColumnIndex] + ";} }\n";
							csFile += "\n";
						}
						else
						{
							csFile += "\t\t" + variableType[cellColumnIndex] + "[] _" + variableName[cellColumnIndex] + " = new " + variableType[cellColumnIndex] + "[" + variableLength[cellColumnIndex] + "];\t//" + variableDescribe[cellColumnIndex] + "\n";
							csFile += "\t\tpublic " + variableType[cellColumnIndex] + "[] " + variableName[cellColumnIndex] + " { get { return _" + variableName[cellColumnIndex] + ";} }\n";
							csFile += "\n";
						}
					}
				}
			}

			// Add Init() Info To CS
			// Get variableDefaultValue array
			// the fourth row is variableDefaultValue
			string[] variableDefaultValue = new string[columnCount];
			csFile += "\t\tpublic override int init(TabReader reader, int row, int column)" + "\n";
			csFile += "\t\t{" + "\n";
			csFile += "\t\t\tcolumn = base.init(reader, row, column);" + "\n\n";
			for (int col = 0; col < columnCount; col++)
			{
				int cellColumnIndex = col;
				if (cellColumnIndex >= 2)
				{
					variableDefaultValue[cellColumnIndex] = sheetData.At(4, col);

					//special deal with bool
					if (variableType[cellColumnIndex].Equals("bool"))
					{
						if (variableDefaultValue[cellColumnIndex].Equals("0"))
							variableDefaultValue[cellColumnIndex] = "false";
						else
							variableDefaultValue[cellColumnIndex] = "true";
					}

					if (variableType[cellColumnIndex].Equals("int") || variableType[cellColumnIndex].Equals("float") ||
						variableType[cellColumnIndex].Equals("double") || variableType[cellColumnIndex].Equals("long") ||
						variableType[cellColumnIndex].Equals("bool"))
					{
						if (variableLength[cellColumnIndex].Equals(""))
						{
							csFile += "\t\t\t_" + variableName[cellColumnIndex] + " = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t\t" + variableType[cellColumnIndex] + ".TryParse(reader.At(row, column), out _" + variableName[cellColumnIndex] + ");\n";
						}
						else
						{
							// default value
							csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++)" + "\n";
							csFile += "\t\t\t\t_" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";

							csFile += "\t\t\tstring[] " + variableName[cellColumnIndex] + "Array = reader.At(row, column).Split(\',\');" + "\n";
							csFile += "\t\t\tint " + variableName[cellColumnIndex] + "Count = " + variableName[cellColumnIndex] + "Array.Length;" + "\n";
							csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++)\n";
							csFile += "\t\t\t{\n";
							csFile += "\t\t\t\tif(i < " + variableName[cellColumnIndex] + "Count)" + "\n";
							csFile += "\t\t\t\t\t" + variableType[cellColumnIndex] + ".TryParse(" + variableName[cellColumnIndex] + "Array[i], out _" + variableName[cellColumnIndex] + "[i]);" + "\n";
							csFile += "\t\t\t\telse" + "\n";
							csFile += "\t\t\t\t\t_" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t\t}\n";
						}
					}
					if (variableType[cellColumnIndex].Equals("string"))
					{
						if (variableLength[cellColumnIndex].Equals(""))
						{
							csFile += "\t\t\tif(reader.At(row, column) == null)" + "\n";
							csFile += "\t\t\t\t_" + variableName[cellColumnIndex] + " = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t\telse" + "\n";
							csFile += "\t\t\t\t_" + variableName[cellColumnIndex] + " = reader.At(row, column);\n";
						}
						else
						{
							csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++)" + "\n";
							csFile += "\t\t\t\t_" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";

							csFile += "\t\t\tstring[] " + variableName[cellColumnIndex] + "Array = reader.At(row, column).Split(\',\');" + "\n";
							csFile += "\t\t\tint " + variableName[cellColumnIndex] + "Count = " + variableName[cellColumnIndex] + "Array.Length;" + "\n";
							csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++){" + "\n";
							csFile += "\t\t\t\tif(i < " + variableName[cellColumnIndex] + "Count)" + "\n";
							csFile += "\t\t\t\t\t_" + variableName[cellColumnIndex] + "[i] = " + variableName[cellColumnIndex] + "Array[i];\n";
							csFile += "\t\t\t\telse" + "\n";
							csFile += "\t\t\t\t\t_" + variableName[cellColumnIndex] + "[i] = " + variableDefaultValue[cellColumnIndex] + ";\n";
							csFile += "\t\t\t}\n";
						}
					}
					// JObject
					if (variableType[cellColumnIndex].Equals("JObject"))
					{
						csFile += "\t\t\tfor(int i=0; i<" + variableLength[cellColumnIndex] + "; i++){" + "\n";
						csFile += "\t\t\t\tJArray ja = (JArray)JsonConvert.DeserializeObject(reader.At(row, column));\n";
						csFile += "\t\t\t\t_" + variableName[cellColumnIndex] + "[i] = (JObject)ja[i];\n";
						csFile += "\t\t\t}\n";
					}

					csFile += "\t\t\tcolumn++;\n";
					csFile += "\n";

				}
			}
			csFile += "\t\t\treturn column;\n";
			csFile += "\t\t}\n";

			csFile += "\t}" + "\n";
			csFile += "}";
			
			return csFile;
		}
		catch (System.IO.IOException ex)
		{
			throw new System.IO.IOException(ex.Message);
		}
	}

	#endregion

}