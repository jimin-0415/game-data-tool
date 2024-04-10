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
        this.path = "../Assets/Scripts/Data/";
    }

    public ScriptConvertor(string filePath)
    {
        /*this.path = filePath;
        this.path = "../../";*/
    }

    public void Convert(string sheetName, Dictionary<string, List<string>> rootNamesMap, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas)
    {
        string rootName = string.Empty;
        foreach(var rootNameEntry in rootNamesMap)
        {
            List<string> sheetNameList = rootNameEntry.Value;

            foreach(string valus in sheetNameList)
            {
                if(valus == sheetName)
                {
                    rootName = rootNameEntry.Key;
                }
            }
        }

        if (String.IsNullOrEmpty(rootName))
        {
            System.Console.WriteLine("Faile Script Convert : [ Reason : RootName Is Null ]");
        }
        
        _ConvertData(rootName, sheetName, columnInfos, rowDatas);
    }

        
    private void _ConvertData(string rootName, string sheetName, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas)
    {
        if (columnInfos.Count <= 0 || rowDatas.Count <= 0)
        {
            System.Console.WriteLine("Column Info is Null");
            return;
        }

        //Write Generate String
        StringBuilder builder = new StringBuilder(1000, 50000);

        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// @file    " + sheetName + "Data.cs" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @brief   " + sheetName + " class cs file " );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @date    " + DateTime.Now.ToString( "yyyy.MM.dd" ) );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("using Newtonsoft.Json;" );
        builder.AppendLine("using Newtonsoft.Json.Converters;" );
        builder.AppendLine("");
        builder.AppendLine("");
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    " + sheetName + "Data " );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @brief    " + sheetName + "Data class" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine("[System.Serializable]");
        builder.AppendLine("public class " + sheetName + "Data");
        builder.AppendLine("{");

        {
            foreach (var column in columnInfos)
            {
                if(column.Value.GetDataType() != EDataType.Desc)
                {
                    builder.AppendLine("    /// <summary> " + column.Value.GetDesc() + " </summary> ");
                    if( column.Value.GetDataType() == EDataType.Unique )
                        builder.AppendLine( "    [ JsonConverter( typeof( StringEnumConverter ) ) ]" );
                    builder.AppendLine(_ConvertMemberValue(column.Value));
                    builder.AppendLine("    ");
                }
            }
        }

        builder.Append("}");

        //폴더 있는지 유무 확인 후 생성
        string directoryPath = this.path + "/Generate/" + rootName;
        
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        if (!directoryInfo.Exists)
            directoryInfo.Create();

        //파일 생성
        string fullFilePath = directoryPath + "/" + sheetName + "Data" + ".cs";
        File.WriteAllText(fullFilePath, builder.ToString());
    }

    private string _ConvertMemberValue(ColumnInfo columnInfo)
    {
        return "    " + "public " + Utils.ConvertDataTypeToString(columnInfo.GetDataType(), columnInfo.GetObjectDataType()) + " " + columnInfo.GetName() + ";";
    }
}

