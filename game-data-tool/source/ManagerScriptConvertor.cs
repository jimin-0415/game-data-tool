using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ManagerScriptConvertor : IConvertor
{
    string path = string.Empty;

    public ManagerScriptConvertor()
    {
        this.path = "../../projectpl-client/ProjectPL/Assets/Script/Data/Generate/";
    }

    public void Convert(string sheetName, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas)
    {

    }

    // key : rootName, value : List<sheetName>
    public void Convert(Dictionary<string, List<string>> rootNamesMap)
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
}

