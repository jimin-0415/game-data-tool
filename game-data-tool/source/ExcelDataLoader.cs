﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExcelDataReader;

class ExcelDataLoader : IDataLoader
{
    string directoryPath;
    Stopwatch timer;

    // key : sheetName, key : columIndex, columInfo
    Dictionary<string, Dictionary<int, ColumnInfo>> columnInfosMap;

    // key : sheetName, key : Row Index, datas
    Dictionary<string, Dictionary<int, List<string>>> rowDatasMap;

    IConvertor convertor;

    public ExcelDataLoader(string path, IConvertor convertor)
    {
        this.directoryPath = path;

        this.columnInfosMap = new Dictionary<string, Dictionary<int, ColumnInfo>>();
        this.rowDatasMap = new Dictionary<string, Dictionary<int, List<string>>>();
        this.timer = new Stopwatch();
        this.convertor = convertor;
    }

    public virtual void Init()
    {
    }

    private static List<string> GetTablenames(DataTableCollection tables)
    {
        var tableList = new List<string>();
        foreach (var table in tables)
        {
            tableList.Add(table.ToString());
        }

        return tableList;
    }

    public virtual void Load()
    {
        timer.Reset();

        string filePath = "../../../../logSheet.xlsx";

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        timer.Start();

        using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
        var openTining = timer.ElapsedMilliseconds;

        DataSet dataSet;
        // reader.IsFirstRowAsColumnNames = firstRowNamesCheckBox.Checked;
        dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
        {
            UseColumnDataType = false,
        });

        //컬럼 정보를 초기화 한다
        if(!_InitColumInfo(dataSet.Tables))
        {
            System.Console.WriteLine("Fail InitColumInfo");
        }

        //데이터 정보를 초기화 한다
        if (!_InitRowsData(dataSet.Tables))
        {
            System.Console.WriteLine("Fail InitRowsData");
        }

        timer.Stop();
    }

    /// 다른 데이터 형식으로 변환 시킨다 
    public virtual void Convert()
    {
        foreach(var colunmInfo in columnInfosMap)
        {
            string key = colunmInfo.Key;

            if (!rowDatasMap.ContainsKey(key))
            {
                System.Console.WriteLine("Key의 정보가 매칭도지 않습니다");
                return;
            }

            convertor.Convert(key, colunmInfo.Value, rowDatasMap[key]);
        }
    }

    /// 컬럼의 정보를 초기화 합니다 
    private bool _InitColumInfo(DataTableCollection dataTables)
    {
        if (dataTables == null)
            return false;

        foreach (DataTable dataTable in dataTables)
        {
            if (dataTable.Rows.Count < (int)EExcelRowType.Max)
                continue;

            String tableName = _GetTableName(dataTable);
            if (tableName.Equals(String.Empty))
                continue;

            if (!columnInfosMap.ContainsKey(tableName))
            {
                columnInfosMap.Add(tableName, new Dictionary<int, ColumnInfo>());
            }

            Dictionary<int, ColumnInfo> columnInfos = columnInfosMap[tableName];

            for (int i = 0; i < (int)EExcelRowType.Max; i++)
            {
                EExcelRowType excelRowType = (EExcelRowType)i;

                DataRow dataRow = dataTable.Rows[i];
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    //ColumInfo는 열(j)의 개수만큼 만들어진다.
                    if (!columnInfos.ContainsKey(j))
                    {
                        columnInfos.Add(j, new ColumnInfo());
                    }

                    string row = dataRow[j]?.ToString() ?? String.Empty;
                    switch (excelRowType)
                    {
                        case EExcelRowType.Desc:
                            columnInfos[j].SetDesc(row);
                            break;
                        case EExcelRowType.DataLoad:
                            columnInfos[j].SetDataLoadType(row);
                            break;
                        case EExcelRowType.ReferenceTable:
                            columnInfos[j].SetReferenceTableName(row);
                            break;
                        case EExcelRowType.DataType:
                            columnInfos[j].SetDataType(row);
                            break;
                        case EExcelRowType.Name:
                            columnInfos[j].SetName(row);
                            break;
                        default:
                            continue;
                    }
                }
            }
        }

        return true;
    }

    /// Init Row Datas
    private bool _InitRowsData(DataTableCollection dataTables)
    {
        if (dataTables == null)
            return false;

        foreach (DataTable dataTable in dataTables)
        {
            if (dataTable.Rows.Count < (int)EExcelRowType.Max)
                continue;

            String tableName = _GetTableName(dataTable);
            if (tableName.Equals(String.Empty))
                continue;

            if (!rowDatasMap.ContainsKey(tableName))
            {
                rowDatasMap.Add(tableName, new Dictionary<int, List<string>>());
            }

            Dictionary<int, List<String>> rowDatas = rowDatasMap[tableName];

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                //Row 의 개수가 Max 넘어가는 시점부터 Data Load
                if (i < (int)EExcelRowType.Max)
                    continue;

                DataRow dataRow = dataTable.Rows[i];
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    if (!rowDatas.ContainsKey(i))
                    {
                        rowDatas.Add(i, new List<string>());
                    }

                    List<string> dataList = rowDatas[i];
                    dataList.Add(dataRow[j]?.ToString() ?? String.Empty);
                }
            }
        }

        return true;
    }

    /// Read Table Name
    private string _GetTableName(DataTable dataTable)
    {
        if (dataTable == null)
            return String.Empty;

        return dataTable.TableName;
    }

}
