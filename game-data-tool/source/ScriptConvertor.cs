using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ScriptConvertor : IConvertor
{
    string path = String.Empty;

    public ScriptConvertor()
    {
        this.path = "../../projectpl-client/ProjectPL/Assets/Script/Data/";
    }

    public ScriptConvertor(string filePath)
    {
        this.path = filePath;
        this.path = "../../";
    }

    public void Convert(string sheetName, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas)
    {
        _ConvertData(sheetName, columnInfos, rowDatas);
    }

        
    private void _ConvertData(string sheetName, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas)
    {
        if (columnInfos.Count <= 0 || rowDatas.Count <= 0)
        {
            System.Console.WriteLine("Column Info is Null");
            return;
        }

        StringBuilder builder = new StringBuilder(1000, 50000);

        builder.AppendLine("using UnityEngine;");
        builder.AppendLine(" ");
        builder.AppendLine(" ");
        builder.AppendLine("[System.Serializable]");
        builder.AppendLine("public class " + sheetName + "Data");
        builder.AppendLine("{");

        {
            foreach (var column in columnInfos)
            {
                builder.AppendLine("    /// <summary> " + column.Value.GetDesc() + " </summary> ");
                builder.AppendLine(_ConvertMemberValue(column.Value));
                builder.AppendLine("    ");
            }
        }

        builder.Append("}");

        string fullFilePath = this.path + sheetName + "Data" + ".cs";

        File.WriteAllText(fullFilePath, builder.ToString());
    }

    private void _ConvertManager()
    {

    }

    private string _ConvertMemberValue(ColumnInfo columnInfo)
    {
        return "    " + "public " + Utils.ConvertDataTypeToString(columnInfo.GetDataType(), columnInfo.GetObjectDataType()) + " " + columnInfo.GetName() + ";";
    }
}

