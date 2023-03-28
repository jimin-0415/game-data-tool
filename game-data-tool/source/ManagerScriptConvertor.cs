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
        StringBuilder builder = new StringBuilder(1000, 50000);

        builder.AppendLine("using UnityEngine;");
        builder.AppendLine(" ");
        builder.AppendLine(" ");
        //builder.AppendLine("public class " + sheetName + "Data");
        builder.AppendLine("{");

    }
}

