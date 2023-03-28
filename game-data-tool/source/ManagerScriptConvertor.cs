using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ManagerScriptConvertor : IConvertor
{
    public void Convert(string sheetName, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas)
    {

    }

    // key : rootName, value : List<sheetName>
    public void Convert(Dictionary<string, List<string>> rootNamesMap)
    {
        foreach ( var sheetList  in rootNamesMap)
        {
            StringBuilder builder = new StringBuilder(1000, 50000);

            string managerName = sheetList.Key;

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine("public partial class " + managerName + "Manager : AbstractDataManager");
            builder.AppendLine("{");

            foreach(var sheetName in sheetList.Value)
            {
                string sheetDataName = sheetName + "Data";
                builder.AppendLine("    //Key : key, Value : " + sheetDataName);
                builder.AppendLine("    private Dictionary<int, " + sheetDataName + "> " + sheetDataName + "Map = new Dictionary<int, " + sheetDataName + ">();");
                builder.AppendLine("    ");
                builder.AppendLine("    public override void Load()");
                builder.AppendLine("    {");

                builder.AppendLine("    }");
            }


            builder.AppendLine("}");    
        }
    }
}

