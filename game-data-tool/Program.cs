using System;
using System.Data;
using System.Text;
using ExcelDataReader;

class Program
{ 
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        DataLoader dataLoader = new ExcelDataLoader("/common/study");
        dataLoader.Init();
        dataLoader.Load();
        System.Console.WriteLine("Hello World !");
    }
}