using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

public class ManagerScriptConvertor : IConvertor
{
    string path = string.Empty;

    public ManagerScriptConvertor()
    {
        //this.path = "../../Assets/Scripts/Manager/";
    }

    public ManagerScriptConvertor( string path )
    {
        this.path = path;
    }

    public void Convert(string sheetName, Dictionary<string, List<string>> rootNamesMap, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas)
    {

    }

    public void Convert(Dictionary<string, List<string>> rootNamesMap, ManagerClassType managerClassType = ManagerClassType.AbstractClass)
    {
        switch (managerClassType)
        {
            case ManagerClassType.AbstractClass:
                _ConvertToAbstractClass(rootNamesMap);
                break;
            case ManagerClassType.ExcelName:
                _ConvertToExcelName(rootNamesMap);
                break;
        }
    }

    /// <summary>
    /// 엑셀 시트 이름별로 Manager Class를 생성합니다.
    /// // key : rootName, value : List<sheetName>
    /// </summary>
    /// <param name="rootNamesMap"></param>
    public void _ConvertToExcelName(Dictionary<string, List<string>> rootNamesMap)
    {
        foreach ( var sheetList  in rootNamesMap)
        {
            StringBuilder builder = new StringBuilder(1000, 50000);

            //string className = 

            string fullFilePath = this.path + sheetList.Key + "Data_Generated" + ".cs";

            string managerName = sheetList.Key;

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine("public partial class " + managerName + "DataManager : AbstractDataManager");
            builder.AppendLine("{");

            foreach (var sheetName in sheetList.Value)
            {
                string sheetDataName = sheetName + "Data";
                string mapContainerName = sheetDataName + "Map";

                builder.AppendLine("    //Key : key, Value : " + sheetDataName);
                builder.AppendLine("    private Dictionary< uint, " + sheetDataName + " > " + mapContainerName + " = new Dictionary< uint, " + sheetDataName + " >();");
                builder.AppendLine("    ");
            }

            {
                builder.AppendLine("    public override void Load()");
                builder.AppendLine("    {");
            }
            
            foreach (var sheetName in sheetList.Value)
            {
                string sheetDataName = sheetName + "Data";
                string mapContainerName = sheetDataName + "Map";
                
                builder.AppendLine("        //Load " + sheetDataName);
                builder.AppendLine("        List<" + sheetDataName + "> " + sheetDataName + "s = LoadJsonData<" + sheetDataName + ">(\"Json/" + sheetDataName + "\");");
                builder.AppendLine("        foreach (" + sheetDataName + " " + sheetDataName + " in " + sheetDataName + "s )");
                builder.AppendLine("        {");
                builder.AppendLine("            if(!" + mapContainerName + ".ContainsKey(" + sheetDataName + ".id))");
                builder.AppendLine("            {");
                builder.AppendLine("                " + mapContainerName + ".Add(" + sheetDataName + ".id, " + sheetDataName + ");");
                builder.AppendLine("            }");
                builder.AppendLine("        }");
                builder.AppendLine("    ");
            }

            {
                builder.AppendLine("    }");
            }

            builder.AppendLine("}");

            File.WriteAllText(fullFilePath, builder.ToString());
        }
    }

    /// <summary>
    /// AbstractManager Class 하나로 모든 DataFile을 로드하도록 합니다
    /// </summary>
    /// <param name="rootNamesMap"></param>
    public void _ConvertToAbstractClass(Dictionary<string, List<string>> rootNamesMap)
    {
        //AbstractClass 하나에 모든 로드 파일이 있습니다.
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "DataManager";

        string fullFilePath = this.path + className + ".cs";

        Console.WriteLine("Manager : " + fullFilePath);

        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine("using System.Text;");
        builder.AppendLine("using System.Threading.Tasks;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("using Newtonsoft.Json;" );
        builder.AppendLine("");
        builder.AppendLine("");
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "/// @class: DataManager" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine("public class " + className );

        // >Generate Manager Class Start 
        builder.AppendLine("{");

        // >Generate Manager var
        HashSet<string> visited = new HashSet<string>();
        foreach (var sheetList in rootNamesMap)
        {
            string[] ar = sheetList.Key.Split( "/" );
            var excelRootName = ar[ ar.Length - 1 ];

            if (visited.Contains(excelRootName))
                continue;

            visited.Add(excelRootName);

            //string managerClassName = excelRootName + "DataManager";
            //string managerVarName = "m_" + Utils.LowerFirstChar( managerClassName );

            //builder.AppendLine( "    /// <summary> " + managerClassName + " </summary>" );
            //builder.AppendLine( "    private " + managerClassName + " " + managerVarName + " = new " + managerClassName + "();" );
            //builder.AppendLine( "    public " + managerClassName + " " + managerClassName + " => " + managerVarName + ";" );
            //builder.AppendLine( "" );
        }
        
        // >Generate Load Func Start
        {
            builder.AppendLine( "    /// 로드 합니다." );
            builder.AppendLine( "    public virtual void Load()" );
            builder.AppendLine( "    {" );
        }

        foreach (var sheetList in rootNamesMap)
        {
            foreach ( var excelSheetName in sheetList.Value )
            {
                string managerClassName = excelSheetName + "InfoManager";
                string managerVarName = Utils.UpperFirstChar( managerClassName );
                builder.AppendLine( "        " + managerVarName + ".Instance.Load();" );
            }
        }

        //foreach ( var excelRootName in visited )
        //{
        //    string managerClassName = excelRootName + "InfoManager";
        //    string managerVarName = Utils.UpperFirstChar( managerClassName );
        //    builder.AppendLine( "        " + managerVarName + ".Instance.Load();" );
        //}

        {
            builder.AppendLine("    }");
            builder.AppendLine("    ");
        }

        // >Generate Init();
        builder.AppendLine( "    /// 초기화 합니다." );
        builder.AppendLine( "    public void Initialize()" );
        builder.AppendLine( "    {" );

        foreach ( var sheetList in rootNamesMap )
        {
            foreach ( var excelSheetName in sheetList.Value )
            {
                string managerClassName = excelSheetName + "InfoManager";
                string managerVarName = Utils.UpperFirstChar( managerClassName );
                builder.AppendLine( "        " + managerVarName + ".Instance.Initialize01();" );
            }
        }

        foreach ( var sheetList in rootNamesMap )
        {
            foreach ( var excelSheetName in sheetList.Value )
            {
                string managerClassName = excelSheetName + "InfoManager";
                string managerVarName = Utils.UpperFirstChar( managerClassName );
                builder.AppendLine( "        " + managerVarName + ".Instance.Initialize02();" );
            }
        }

        foreach ( var sheetList in rootNamesMap )
        {
            foreach ( var excelSheetName in sheetList.Value )
            {
                string managerClassName = excelSheetName + "InfoManager";
                string managerVarName = Utils.UpperFirstChar( managerClassName );
                builder.AppendLine( "        " + managerVarName + ".Instance.Initialize03();" );
            }
        }

        {
            builder.AppendLine( "    }" );
            builder.AppendLine( "" );
        }
        
        // >Generate LoadJosnData();
        builder.AppendLine("    /// Json 데이터를 로드 합니다." );
        builder.AppendLine("    public static async Task<List< T >> LoadJsonData< T >( string path )");
        builder.AppendLine("    {");
        builder.AppendLine("        var loadJson = await MainManager.AddressableManager.LoadJsonAsync( path );");
        builder.AppendLine("        JsonDatas< T > result = JsonConvert.DeserializeObject< JsonDatas< T > >( loadJson.ToString() );" );
        builder.AppendLine("        if ( result != null && result.Datas != null )");
        builder.AppendLine("        {");
        builder.AppendLine("            return result.Datas;");
        builder.AppendLine("        }");
        builder.AppendLine("        return new List< T >();");
        builder.AppendLine("    }");

        // >Generate Manager Class End
        builder.AppendLine("}");

        // >Generate JsonData
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "/// @class: " + "JsonDatas< T >" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine("public class JsonDatas< T >");
        builder.AppendLine("{");
        builder.AppendLine("    public List< T > Datas;");
        builder.AppendLine("}");

        File.WriteAllText(fullFilePath, builder.ToString());

        Console.WriteLine( "Manager" + fullFilePath);
    }
}

