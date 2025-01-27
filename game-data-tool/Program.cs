using System;
using System.Data;
using System.Text;
using ExcelDataReader;

class Program
{ 
    static void Main(string[] args)
    {
        var pivot = new DateTime(2025, 12, 31);
        var now = DateTime.Now;

        if ( DateTime.Compare( pivot, now ) < 0 )
        {
            System.Console.WriteLine("사용 기간 만료됨");
            return;
        }
            

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        try
        {
            List<IConvertor> convertors = new List<IConvertor>();
            convertors.Add( new JsonConvertor() );
            convertors.Add( new ScriptConvertor() );
            convertors.Add( new ManagerScriptConvertor() );
            convertors.Add( new ManagerTemplateScriptConvertor() );

            IDataLoader dataLoader = new ExcelDataLoader(
                "../Data/", convertors);

            dataLoader.Init();
            dataLoader.Load();
            dataLoader.Convert();

            dataLoader = new PacketDataLoader( "../Data/" );
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