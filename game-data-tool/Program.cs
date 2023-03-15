using System;
using System.Data;
using System.Text;
using ExcelDataReader;

class Program
{ 
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string folderName = "Data";

        IDataLoader dataLoader = new ExcelDataLoader("../Data", new JsonConvertor(), new ScriptConvertor());
        dataLoader.Init();
        dataLoader.Load();
        dataLoader.Convert();

        System.Console.WriteLine("Sucess");
    }
}