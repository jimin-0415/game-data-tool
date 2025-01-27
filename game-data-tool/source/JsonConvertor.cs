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
        this.path = "../../Assets/Resources/Json/";
    }

    public JsonConvertor(string filePath)
    {
        this.path = filePath;
        this.path = "../../";
    }

    public void Convert(string sheetName, Dictionary<string, List<string>> rootNamesMap, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas)
    {
        StringBuilder builder = new StringBuilder(1000, 50000);
        builder.AppendLine("{");

        Utils.ToJsonKey(sheetName);

        builder.AppendLine("    " + Utils.ToJsonKey("Datas") + " [");

        string fullFilePath = this.path + sheetName + "Data" + ".json";

        if (columnInfos.Count <= 0 || rowDatas.Count <= 0)
        {
            System.Console.WriteLine("Column Info is Null");
            return;
        }

        int count = 0;
        foreach (var rowData in rowDatas)
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
                //만약 DataType이 Desc 일 경우 무시하고 진행
                if (columnInfos[i].GetDataType() == EDataType.Desc)
                    continue;

                string varName = columnInfos[i].GetName();
                builder.Append("            " + Utils.ToJsonKey(varName));

                string data = datas[i];
                switch (columnInfos[i].GetDataType())
                {
                    case EDataType.String:
                    case EDataType.Unique:
                        data = "\"" + data + "\"";
                        break;
                    default:
                        break;
                }

                if (i == datas.Count - 1)
                    builder.AppendLine(data);
                else
                    builder.AppendLine(data + ",");
            }
            
            if(++count == rowDatas.Count)
                builder.AppendLine("        }");
            else 
                builder.AppendLine("        },");
        }

        builder.AppendLine("    ]");
        builder.Append("}");

        File.WriteAllText(fullFilePath, builder.ToString());   
    }
}

