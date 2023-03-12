using System;
using System.Data;
using System.Text;
using ExcelDataReader;

class Program
{ 
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        IDataLoader dataLoader = new ExcelDataLoader("/common/study", new JsonConvertor());
        dataLoader.Init();
        dataLoader.Load();
        dataLoader.Convert();

        System.Console.WriteLine("Sucess");
    }
}