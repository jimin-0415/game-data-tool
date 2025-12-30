using System;
using System.Data;
using System.Text;
using ExcelDataReader;

class Program
{ 
    static void Main(string[] args)
    {
        var pivot = new DateTime(2026, 12, 31);
        var now = DateTime.Now;

        if ( DateTime.Compare( pivot, now ) < 0 )
        {
            System.Console.WriteLine("사용 기간 만료됨");
            return;
        }
        
        if( args.Length < 4)
        {
            Console.WriteLine("사용법: game-data-tool.exe <sourceDataDir> <jsonDir> [Platform]");
            Console.ReadKey();
            Environment.Exit(1);
        }

        string excelDataPath = args[0];
        string jsonDataPath = args[1] + '\\';
        string scriptPath = args[2] + '\\';
        string managerPath = args[3] + '\\';
        string scriptTemplatePath = args[2] + '\\'; 

        Console.WriteLine(managerPath);

        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            List<IConvertor> convertors = new List<IConvertor>();
            convertors.Add(new JsonConvertor(jsonDataPath));
            convertors.Add( new ScriptConvertor(scriptPath) );
            convertors.Add( new ManagerTemplateScriptConvertor(scriptTemplatePath) );

            IDataLoader dataLoader = new ExcelDataLoader(excelDataPath, managerPath, convertors);

            dataLoader.Init();
            dataLoader.Load();
            dataLoader.Convert();

            // Packet Data Loader
            //dataLoader = new PacketDataLoader( "../../Data/" );
            //dataLoader.Init();
            //dataLoader.Load();
            //dataLoader.Convert();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine("아무 키나 누르면 종료됩니다...");
            Console.ReadKey();
            Environment.Exit(1); // 비정상 종료
        }

        System.Console.WriteLine("Sucess");
        Console.ReadKey();
    }
}