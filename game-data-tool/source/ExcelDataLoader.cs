using System;
using System.Collections.Generic;
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

    public ExcelDataLoader(string path)
    {
        this.directoryPath = path;
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

        string filePath = "C:\\Users\\programmer\\Desktop\\git\\pl\\game-data-tool\\logSheet.xlsx";

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

        List<string> talbeNames = GetTablenames(dataSet.Tables);






        /*        if (tablenames.Count > 0)
                    sheetCombo.SelectedIndex = 0;*/

        timer.Stop();
    }
}
