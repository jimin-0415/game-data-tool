using System;
using System.Data;
using System.Text;
using ExcelDataReader;

class Program
{ 
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        try
        {
            IDataLoader dataLoader = new ExcelDataLoader("../Data", new JsonConvertor(), new ScriptConvertor());
            dataLoader.Init();
            dataLoader.Load();
            dataLoader.Convert();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.ReadLine();
        }

        System.Console.WriteLine("Sucess");
        Console.ReadLine();
    }
}