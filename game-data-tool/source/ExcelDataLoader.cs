using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExcelDataReader;

class ExcelDataLoader : DataLoader
{
    string directoryPath;
    Stopwatch timer;

    // key : columIndex, columInfo
    Dictionary<int, ColumnInfo> columnInfos;

    // key : Row Index, datas
    Dictionary<int, List<string>> rowDatasMap;
    
    public ExcelDataLoader(string path)
    {
        this.directoryPath = path;
        this.columnInfos = new Dictionary<int, ColumnInfo>();
        this.rowDatasMap = new Dictionary<int, List<string>>();
        this.timer = new Stopwatch();
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

        List<string> talbeNames = GetTablenames(dataSet.Tables);




        /*        if (tablenames.Count > 0)
                    sheetCombo.SelectedIndex = 0;*/

        timer.Stop();
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

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                //Row 개수를 넘어갈 경우 ColumInfo 초기화 완료
                if (i > (int)EExcelRowType.Max)
                    return true;

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

        return false;
    }

    private bool _InitRowsData(DataTableCollection dataTables)
    {
        if (dataTables == null)
            return false;

        foreach (DataTable dataTable in dataTables)
        {
            if (dataTable.Rows.Count < (int)EExcelRowType.Max)
                continue;

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                //Row 의 개수가 Max 넘어가는 시점부터 Data Load
                if (i < (int)EExcelRowType.Max)
                    continue;

                DataRow dataRow = dataTable.Rows[i];
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    if (!rowDatasMap.ContainsKey(i))
                    {
                        rowDatasMap.Add(i, new List<string>());
                    }

                    List<string> dataList = rowDatasMap[i];
                    dataList.Add(dataRow[j]?.ToString() ?? String.Empty);
                }
            }
        }
        return true;
    }
}
