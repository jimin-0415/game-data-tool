﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

public class ManagerScriptConvertor : IConvertor
{
    string path = string.Empty;

    public ManagerScriptConvertor()
    {
        this.path = "../../projectpl-client/ProjectPL/Assets/Script/Data/Generate/";
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
                builder.AppendLine("    private Dictionary<int, " + sheetDataName + "> " + mapContainerName + " = new Dictionary<int, " + sheetDataName + ">();");
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
                builder.AppendLine("        List<" + sheetDataName + "> " + sheetDataName + "s = LoadJsonData<" + sheetDataName + ">(\"Data/Json/" + sheetDataName + "\");");
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

        string className = "AbstractDataManager";

        string fullFilePath = this.path + className + "_Generated" + ".cs";

        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine("using System.Text;");
        builder.AppendLine("using System.Threading.Tasks;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("");
        builder.AppendLine("");
        builder.AppendLine("public abstract class " + className + " : IDataManager");

        // >Generate Manager Class Start 
        builder.AppendLine("{");

        //Generate Map Container
        foreach (var sheetList in rootNamesMap)
        {
            foreach (var sheetName in sheetList.Value)
            {
                string sheetDataName = sheetName + "Data";
                string mapContainerName = sheetDataName + "Map";

                builder.AppendLine("    //Key : key, Value : " + sheetDataName);
                builder.AppendLine("    private Dictionary<int, " + sheetDataName + "> " + mapContainerName + " = new Dictionary<int, " + sheetDataName + ">();");
                builder.AppendLine("    ");
            }
        }

        // >Generate Load Func Start
        {
            builder.AppendLine("    public virtual void Load()");
            builder.AppendLine("    {");
        }

        foreach (var sheetList in rootNamesMap)
        {
            foreach (var sheetName in sheetList.Value)
            {
                string sheetDataName = sheetName + "Data";
                string mapContainerName = sheetDataName + "Map";

                builder.AppendLine("        //Load " + sheetDataName);
                builder.AppendLine("        List<" + sheetDataName + "> " + sheetDataName + "s = LoadJsonData<" + sheetDataName + ">(\"Data/Json/" + sheetDataName + "\");");
                builder.AppendLine("        foreach (" + sheetDataName + " " + sheetDataName + " in " + sheetDataName + "s )");
                builder.AppendLine("        {");
                builder.AppendLine("            if(!" + mapContainerName + ".ContainsKey(" + sheetDataName + ".id))");
                builder.AppendLine("            {");
                builder.AppendLine("                " + mapContainerName + ".Add(" + sheetDataName + ".id, " + sheetDataName + ");");
                builder.AppendLine("            }");
                builder.AppendLine("        }");
                builder.AppendLine("    ");
            }
        }

        // >Generate Load Func End
        {
            builder.AppendLine("    }");
            builder.AppendLine("    ");
        }
        
        // >Generate Init();
        builder.AppendLine("    public abstract void Init();");
        builder.AppendLine("    ");

        // >Generate LoadJosnData();
        builder.AppendLine("    protected List<T> LoadJsonData<T>(string path)");
        builder.AppendLine("    {");
        builder.AppendLine("        var loadJson = Resources.Load<TextAsset>(path);");
        builder.AppendLine("        JsonDatas<T> result = JsonUtility.FromJson<JsonDatas<T>>(loadJson.ToString());");
        builder.AppendLine("        if (result != null && result.Datas != null)");
        builder.AppendLine("        {");
        builder.AppendLine("            return result.Datas;");
        builder.AppendLine("        }");
        builder.AppendLine("        return new List<T>();");
        builder.AppendLine("    }");
        builder.AppendLine("    ");

        // >Generate Manager Class End
        builder.AppendLine("}");

        // >Generate JsonData
        builder.AppendLine("public class JsonDatas<T>");
        builder.AppendLine("{");
        builder.AppendLine("    public List<T> Datas;");
        builder.AppendLine("}");

        File.WriteAllText(fullFilePath, builder.ToString());
    }
}

