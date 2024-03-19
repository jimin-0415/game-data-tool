using System;
using System.Data;
using System.Text;
using ExcelDataReader;

class Program
{ 
    static void Main(string[] args)
    {
        /*var pivot = new DateTime(2024, 10, 29);
        var now = DateTime.Now;

         if(DateTime.Compare(pivot, now) < 0)
            return;*/

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        try
        {
            IDataLoader dataLoader = new ExcelDataLoader( "../Assets/Data/", new JsonConvertor(), new ScriptConvertor());
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