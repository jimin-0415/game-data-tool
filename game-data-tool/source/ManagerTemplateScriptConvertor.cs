using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

public class ManagerTemplateScriptConvertor : IConvertor
{
    string path = string.Empty;

    public ManagerTemplateScriptConvertor()
    {
        this.path = "../Assets/Scripts/Manager/Data";
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary> @brief [ 변환 합니다. ] </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public void Convert( 
        string                           sheetName, 
        Dictionary<string, List<string>> rootNamesMap, 
        Dictionary<int, ColumnInfo>      columnInfos, 
        Dictionary<int, List<string>>    rowDatas)
    {
        string rootName = string.Empty;
        foreach ( var rootNameEntry in rootNamesMap )
        {
            List<string> sheetNameList = rootNameEntry.Value;

            foreach ( string valus in sheetNameList )
            {
                if ( valus == sheetName )
                {
                    string[] ar = rootNameEntry.Key.Split("/");
                    rootName = ar[ar.Length - 1];
                }
            }
        }

        if ( String.IsNullOrEmpty( rootName ) )
        {
            System.Console.WriteLine( "Faile Script Convert : [ Reason : RootName Is Null ]" );
        }
        //1. 필요 스펙 => 루트 이름 기준으로 Template 클래스. [생성].
        _ConvertToTemplateManagerClass(rootName, sheetName, columnInfos, rowDatas);

        //2. => 루트이름 기준 기본 Manager 클래스 생성 <- 대신.. 이 클래스는 없으면 생성. 있으면 생성x.
        _ConvertToManagerClass( rootName, sheetName, columnInfos, rowDatas );
    }

    private void _ConvertToTemplateManagerClass(
        string                        rootName,
        string                        sheetName,
        Dictionary<int, ColumnInfo>   columnInfos,
        Dictionary<int, List<string>> rowDatas)
    {
        if ( columnInfos.Count <= 0 || rowDatas.Count <= 0 )
        {
            System.Console.WriteLine( "Column Info is Null" );
            return;
        }

        //Write Generate String
        StringBuilder builder = new StringBuilder( 1000, 50000 );

        string sheetDataName = sheetName + "Data";     // NameData;
        string sheetDictName = sheetDataName + "Dict"; // NameDataDict;
        string sheetDatavariable = Utils.LowerFirstChar(sheetDataName);
        string sheetDictvariable = "m_" + Utils.LowerFirstChar(sheetDictName);

        //LowerFirstCharacter

        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Newtonsoft.Json;" );
        builder.AppendLine( "using Newtonsoft.Json.Converters;" );
        builder.AppendLine( "using System.Collections.Generic;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [" + sheetDataName + "ManagerTemplate ]" );
        builder.AppendLine( "/// @brief    [" + sheetDataName + "ManagerTemplate class ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public abstract class " + sheetDataName + "ManagerTemplate" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    /// <summary> " + sheetDictName + " [ Key : id, Value : " + sheetDataName + " ] </summary>" );
        builder.AppendLine( "    private Dictionary< uint, " + sheetDataName + " > " + sheetDictvariable + " = new Dictionary< uint, " + sheetDataName + " >();" );
        
        builder.AppendLine( " ");
        
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 로드 합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Load()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        List< " + sheetDataName + " > " + sheetDatavariable + "s = DataManager.LoadJsonData<" + sheetDataName + ">(\"Json/"+ sheetDataName + "\");" );
        builder.AppendLine( "        foreach ( " + sheetDataName + " " + sheetDatavariable + " in " + sheetDatavariable + "s )" );
        builder.AppendLine( "        {");
        builder.AppendLine( "            if(!" + sheetDictvariable + ".ContainsKey( " + sheetDatavariable + ".id) )");
        builder.AppendLine( "            {" );
        builder.AppendLine( "                " + sheetDictvariable + ".Add(" + sheetDatavariable + ".id, " + sheetDatavariable + " );");
        builder.AppendLine( "            }" );
        builder.AppendLine( "        }" );
        builder.AppendLine( "    }" );

        builder.AppendLine( " " );

        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ @getter "+ sheetDataName + "를 반환 합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public "+ sheetDataName + " Get"+ sheetDataName + "( uint id )" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        if ( "+ sheetDictvariable + ".ContainsKey( id ) )" );
        builder.AppendLine( "            return " + sheetDictvariable + "[ id ];" );
        builder.AppendLine( "    " );
        builder.AppendLine( "        return null;" );
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

        builder.AppendLine( "}" );

        //폴더 있는지 유무 확인 후 생성
        string directoryPath = this.path + "/" + rootName;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        //파일 생성
        string fullFilePath = directoryPath + "/" + sheetName + "ManagerTemplate.cs";
        File.WriteAllText( fullFilePath, builder.ToString() );
    }

    private void _ConvertToManagerClass( 
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
        StringBuilder builder = new StringBuilder( 1000, 50000 );

        string sheetDataName = sheetName + "Data";     // NameData;
        string sheetDictName = sheetDataName + "Dict"; // NameDataDict;

        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Newtonsoft.Json;" );
        builder.AppendLine( "using Newtonsoft.Json.Converters;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [" + sheetDataName + " Manager ]" );
        builder.AppendLine( "/// @brief    [" + sheetDataName + " Manager class ]" );
        builder.AppendLine( "/// @date     [" + DateTime.Now.ToString( "yyyy.MM.dd" ) + "]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public class " + sheetDataName + "Manager : " + sheetDataName + "ManagerTemplate" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public virtual void Initialize01()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        base.Initialize01();" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "}" );

        //폴더 있는지 유무 확인 후 생성
        string directoryPath = this.path + "/" + rootName;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        //파일 생성
        string fullFilePath = directoryPath + "/" + sheetName + "Manager.cs";

        FileInfo fileInfo = new FileInfo( fullFilePath );
        if( !fileInfo.Exists )
            File.WriteAllText( fullFilePath, builder.ToString() );
    }
}

