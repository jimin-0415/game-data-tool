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
        this.path = "../../Assets/Scripts/Data/";
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
                    string[] ar = rootNameEntry.Key.Split("/");
                    rootName = ar[ ar.Length - 1 ];
                }
            }
        }

        if (String.IsNullOrEmpty(rootName))
        {
            System.Console.WriteLine("Faile Script Convert : [ Reason : RootName Is Null ]");
        }
        
        _ConvertData(rootName, sheetName, columnInfos, rowDatas);
        _ConverInfoTemplate( rootName, sheetName, columnInfos, rowDatas );
        _ConvertInfo(rootName, sheetName, columnInfos, rowDatas);
    }

    private void _ConvertData(
        string                        rootName, 
        string                        sheetName, 
        Dictionary<int, ColumnInfo>   columnInfos, 
        Dictionary<int, List<string>> rowDatas)
    {
        if (columnInfos.Count <= 0 || rowDatas.Count <= 0)
        {
            System.Console.WriteLine("Column Info is Null");
            return;
        }

        //Write Generate String
        StringBuilder builder = new StringBuilder(1000, 50000);
        
        builder.AppendLine( "using UnityEngine;");
        builder.AppendLine( "using Newtonsoft.Json;" );
        builder.AppendLine( "using Newtonsoft.Json.Converters;" );
        builder.AppendLine( "using System.Collections.Generic;" );
        builder.AppendLine( "");
        builder.AppendLine( "");
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [" + sheetName + "Data ]" );
        builder.AppendLine( "/// @brief    [" + sheetName + "Data class ]" );
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
        string directoryPath = this.path + "/Data/" + rootName;
        
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        if (!directoryInfo.Exists)
            directoryInfo.Create();

        //파일 생성
        string fullFilePath = directoryPath + "/" + sheetName + "Data" + ".cs";
        File.WriteAllText(fullFilePath, builder.ToString());
    }


    private void _ConverInfoTemplate(
        string                        rootName,
        string                        sheetName,
        Dictionary<int, ColumnInfo>   columnInfos,
        Dictionary<int, List<string>> rowDatas )
    {
        if ( columnInfos.Count <= 0 || rowDatas.Count <= 0 )
        {
            System.Console.WriteLine( "Column Info is Null" );
            return;
        }

        //Write Generate String
        StringBuilder builder = new StringBuilder(1000, 50000);

        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Newtonsoft.Json;" );
        builder.AppendLine( "using Newtonsoft.Json.Converters;" );
        builder.AppendLine( "using System.Collections.Generic;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [" + sheetName + "InfoTemplate ]" );
        builder.AppendLine( "/// @brief    [" + sheetName + "InfoTemplate class ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public abstract class " + sheetName + "InfoTemplate" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    /// <summary> 원본 데이터 </summary>" );
        builder.AppendLine( "    private " + sheetName + "Data m_data;"  );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 로드 합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Load( " + sheetName + "Data data )" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        m_data = data;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public virtual void Initialize01() { }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public virtual void Initialize02() { }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public virtual void Initialize03() { }" );
        builder.AppendLine( "" );
        {
            foreach ( var column in columnInfos )
            {
                if ( column.Value.GetDataType() != EDataType.Desc )
                {
                    builder.AppendLine( "    /// <summary> " + column.Value.GetDesc() + " </summary> " );
                    builder.AppendLine( _ConvertPascalMemberValue( column.Value ) );
                    builder.AppendLine( "" );
                }
            }
        }

        builder.AppendLine( "}" );

        //폴더 있는지 유무 확인 후 생성
        string directoryPath = this.path + "/Info/" + rootName;

        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        //파일 생성
        string fullFilePath = directoryPath + "/" + sheetName + "InfoTemplate" + ".cs";
        File.WriteAllText(fullFilePath, builder.ToString());
    }

    private void _ConvertInfo(
        string                        rootName,
        string                        sheetName,
        Dictionary<int, ColumnInfo>   columnInfos,
        Dictionary<int, List<string>> rowDatas )
    {
        if ( columnInfos.Count <= 0 || rowDatas.Count <= 0 )
        {
            System.Console.WriteLine( "Column Info is Null" );
            return;
        }

        //Write Generate String
        StringBuilder builder = new StringBuilder(1000, 50000);

        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Newtonsoft.Json;" );
        builder.AppendLine( "using Newtonsoft.Json.Converters;" );
        builder.AppendLine( "using System.Collections.Generic;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [" + sheetName + "Info ]" );
        builder.AppendLine( "/// @brief    [" + sheetName + "Info class ]" );
        builder.AppendLine( "/// @date     [" + DateTime.Now.ToString( "yyyy.MM.dd" ) + "]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public class " + sheetName + "Info : " + sheetName +"InfoTemplate" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public override void Initialize01()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        base.Initialize01();" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "}" );

        //폴더 있는지 유무 확인 후 생성
        string directoryPath = this.path + "/Info/" + rootName;

        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        //파일 생성
        string fullFilePath = directoryPath + "/" + sheetName + "Info" + ".cs";

        FileInfo fileInfo = new FileInfo( fullFilePath );
        if ( !fileInfo.Exists )
            File.WriteAllText( fullFilePath, builder.ToString() );
    }

    private string _ConvertMemberValue(ColumnInfo columnInfo)
    {
        return "    " + "public " + Utils.ConvertDataTypeToString(columnInfo.GetDataType(), columnInfo.GetObjectDataType()) + " " + columnInfo.GetName() + ";";
    }

    public string _ConvertPascalMemberValue(ColumnInfo columnInfo )
    {
        string columName = columnInfo.GetName();
        var colums = columName.Split("_");

        for (var i = 0; i < colums.Length; i++)
            colums[ i ] = UpperFirstChar(colums[i]);

        string result = "    " + "public " +
                        Utils.ConvertDataTypeToString(columnInfo.GetDataType(), columnInfo.GetObjectDataType()) + " ";

        foreach (var element in colums)
            result += element;

        result += " => m_data." + columName + ";";

        return result;
    }

    private static string UpperFirstChar( string input )
    {
        if ( string.IsNullOrEmpty( input ) )
        {
            return null;
        }

        return char.ToUpper( input[ 0 ] ) + input.Substring( 1 );
    }
}
