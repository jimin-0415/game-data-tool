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
        this.path = "../../Assets/Scripts/Data/Info";
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
        string sheetInfoName = sheetName + "Info";     // NameInfo;
        string sheetDictName = sheetDataName + "";     // NameDataDict;

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
        builder.AppendLine( "/// @class    [" + sheetInfoName + "ManagerTemplate ]" );
        builder.AppendLine( "/// @brief    [" + sheetInfoName + "ManagerTemplate class ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public abstract class " + sheetInfoName + "ManagerTemplate : Singleton< " + sheetInfoName + "Manager >" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    /// <summary> [ Key : id, Value : " + sheetInfoName + " ] </summary>" );
        builder.AppendLine( "    protected Dictionary< uint, " + sheetInfoName + " > m_infos " + " = new Dictionary< uint, " + sheetInfoName + " >();" );
        builder.AppendLine( "    public Dictionary< uint, " + sheetInfoName + " > Infos => m_infos;" );
        builder.AppendLine( " ");
        
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 로드 합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Load()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        List< " + sheetDataName + " > datas = DataManager.LoadJsonData<" + sheetDataName + ">(\"Json/"+ sheetDataName + "\");" );
        builder.AppendLine( "        foreach ( var data in datas )" );
        builder.AppendLine( "        {");
        builder.AppendLine( "            if( !m_infos.ContainsKey( data.id ) )");
        builder.AppendLine( "            {" );
        builder.AppendLine( "                " + sheetInfoName + " info = new " + sheetInfoName + "();" );
        builder.AppendLine( "                info.Load( data );" );
        builder.AppendLine( "                m_infos.Add( data.id, info );");
        builder.AppendLine( "            }" );
        builder.AppendLine( "        }" );
        builder.AppendLine( "    }" );

        builder.AppendLine( " " );

        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ @getter "+ sheetInfoName + "를 반환 합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public "+ sheetInfoName + " Get"+ sheetInfoName + "( uint id )" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        if ( m_infos.ContainsKey( id ) )" );
        builder.AppendLine( "            return m_infos[ id ];" );
        builder.AppendLine( "    " );
        builder.AppendLine( "        return null;" );
        builder.AppendLine( "    }" );

        builder.AppendLine( "" );

        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public virtual void Initialize01()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        foreach( var info in m_infos )" );
        builder.AppendLine( "        {" );
        builder.AppendLine( "            info.Value.Initialize01();" );
        builder.AppendLine( "        }" );
        builder.AppendLine( "    }" );

        builder.AppendLine( "" );

        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public virtual void Initialize02()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        foreach( var info in m_infos )" );
        builder.AppendLine( "        {" );
        builder.AppendLine( "            info.Value.Initialize02();" );
        builder.AppendLine( "        }" );
        builder.AppendLine( "    }" );

        builder.AppendLine( "" );

        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public virtual void Initialize03()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        foreach( var info in m_infos )" );
        builder.AppendLine( "        {" );
        builder.AppendLine( "            info.Value.Initialize03();" );
        builder.AppendLine( "        }" );
        builder.AppendLine( "    }" );

        builder.AppendLine( "}" );

        //폴더 있는지 유무 확인 후 생성
        string directoryPath = this.path + "/" + rootName;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        //파일 생성
        string fullFilePath = directoryPath + "/" + sheetInfoName + "InfoManagerTemplate.cs";
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
        string sheetInfoName = sheetName + "Info";     // NameInfo;
        string sheetDictName = sheetDataName + "Dict"; // NameDataDict;

        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Newtonsoft.Json;" );
        builder.AppendLine( "using Newtonsoft.Json.Converters;" );
        builder.AppendLine( "using System.Collections.Generic;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [" + sheetInfoName + " Manager ]" );
        builder.AppendLine( "/// @brief    [" + sheetInfoName + " Manager class ]" );
        builder.AppendLine( "/// @date     [" + DateTime.Now.ToString( "yyyy.MM.dd" ) + "]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public class " + sheetInfoName + "Manager : " + sheetInfoName + "ManagerTemplate" );
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
        string directoryPath = this.path + "/" + rootName;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        //파일 생성
        string fullFilePath = directoryPath + "/" + sheetInfoName + "Manager.cs";

        FileInfo fileInfo = new FileInfo( fullFilePath );
        if( !fileInfo.Exists )
            File.WriteAllText( fullFilePath, builder.ToString() );
    }
}

