using System;

public interface IConvertor
{
    public void Convert(string sheetName, Dictionary<int, ColumnInfo> columnInfos, Dictionary<int, List<string>> rowDatas);
}

