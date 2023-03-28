using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class JsonConvertor : IConvertor
{
    string path = String.Empty;

    public JsonConvertor()
    {
        this.path = "../../projectpl-client/ProjectPL/Assets/Resources/Data/";
    }

    public JsonConvertor(string filePath)
    {
        this.path = filePath;
        this.path = "../../";
    }

    public void Convert(string sheetName, Dictionary<string, string> rootNameMap, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas)
    {
        StringBuilder builder = new StringBuilder(1000, 50000);
        builder.AppendLine("{");

        Utils.ToJsonKey(sheetName);

        builder.AppendLine("    " + Utils.ToJsonKey(sheetName) + " [");

        string fullFilePath = this.path + sheetName + ".json";

        if (columnInfos.Count <= 0 || rowDatas.Count <= 0)
        {
            System.Console.WriteLine("Column Info is Null");
            return;
        }

        foreach(var rowData in rowDatas)
        {
            int key = rowData.Key;
            List<string> datas = rowData.Value;

            if (datas.Count != columnInfos.Count)
            {
                System.Console.WriteLine("Column 개수와 데이터의 개수가 일치하지 않습니다");
                return;
            }

            builder.AppendLine("        {");

            for (int i = 0; i < datas.Count; i++)
            {
                string varName = columnInfos[i].GetName();
                builder.Append("            " + Utils.ToJsonKey(varName));
                builder.AppendLine(datas[i] + ",");
            }

            builder.AppendLine("        },");
        }

        builder.AppendLine("    ]");
        builder.Append("}");

        File.WriteAllText(fullFilePath, builder.ToString());   
    }
}

