using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExcelDataReader.Log;


public class PacketDataLoader : IDataLoader
{
    //Conver Data Path
    string dataPath;

    // 파일 전체 경로
    List<string> fileFullPath;

    //현재 루트 이름
    string curRootName;

    /// 패킷 정보
    private Dictionary<string, Dictionary<uint, PacketInfo>> m_packetInfoDict;

    public PacketDataLoader( string dataPath )
    {
        this.dataPath = dataPath;

        m_packetInfoDict = new();
    }

    public virtual void Init()
    {
        fileFullPath = Directory.GetFiles( dataPath, "*.md", SearchOption.AllDirectories ).ToList();
    }

    public virtual void Load()
    {
        foreach ( var filePath in fileFullPath )
        {
            var lines = File.ReadAllLines(filePath);

            string lastPacketCategory = String.Empty;
            uint lastPacketIndex = 0;

            foreach (var line in lines)
            {
                string pureLine = line.Replace(" ", String.Empty);
                //string pureLine = temp.Replace("\t-", String.Empty);
                
                if( pureLine.Contains("###") )
                {
                    string packetCategory = pureLine.Replace("###", String.Empty);
                    if (!m_packetInfoDict.ContainsKey(packetCategory))
                        m_packetInfoDict.Add(packetCategory, new Dictionary<uint, PacketInfo>());

                    lastPacketCategory = packetCategory;
                }

                if (pureLine.Contains("-PacketBegin:"))
                {
                    string packetIndex = pureLine.Replace("-PacketBegin:", String.Empty);
                    lastPacketIndex = uint.Parse(packetIndex);
                }

                if (pureLine.Contains("[>>]"))
                {
                    var packetInfoPair = m_packetInfoDict[lastPacketCategory];
                    if (packetInfoPair == null)
                    {
                        Console.WriteLine("Check Packet Data [>>]");
                        return;
                    }

                    if (!packetInfoPair.ContainsKey(lastPacketIndex))
                        packetInfoPair.Add( lastPacketIndex, new PacketInfo() );

                    string packetName = pureLine.Replace("[>>]", String.Empty);
                    
                    var packetInfo = packetInfoPair[lastPacketIndex];
                    packetInfo.packetCategory = lastPacketCategory;
                    packetInfo.packetIndex = lastPacketIndex;
                    packetInfo.packetName = packetName;
                    packetInfo.packetDirectType = EPacketDirectType.ClientToServer;

                    lastPacketIndex++;
                }
                else if ( pureLine.Contains( "[<<]" ) )
                {
                    var packetInfoPair = m_packetInfoDict[lastPacketCategory];
                    if ( packetInfoPair == null )
                    {
                        Console.WriteLine( "Check Packet Data [<<]" );
                        return;
                    }

                    if ( !packetInfoPair.ContainsKey( lastPacketIndex ) )
                        packetInfoPair.Add( lastPacketIndex, new PacketInfo() );

                    string packetName = pureLine.Replace("[<<]", String.Empty);

                    var packetInfo = packetInfoPair[lastPacketIndex];
                    packetInfo.packetCategory = lastPacketCategory;
                    packetInfo.packetIndex = lastPacketIndex;
                    packetInfo.packetName = packetName;
                    packetInfo.packetDirectType = EPacketDirectType.ServerToClient;

                    lastPacketIndex++;
                }

                if (pureLine.Contains("//") && !pureLine.Contains( "\t-" ) )
                {
                    var packetInfoPair = m_packetInfoDict[lastPacketCategory];
                    if ( packetInfoPair == null )
                    {
                        Console.WriteLine( "Check Packet Data [<<]" );
                        return;
                    }

                    string packetDesc = pureLine.Replace("//", String.Empty);
                    var packetInfo = packetInfoPair[lastPacketIndex - 1];
                    packetInfo.packetDesc = packetDesc;
                }

                if (pureLine.Contains("//") && pureLine.Contains("\t-"))
                {
                    PacketElementInfo packetElementInfo = new PacketElementInfo();

                    string packetElement = pureLine.Replace("\t-", String.Empty);
                    string[] packetElementList = packetElement.Split("//");

                    var element = packetElementList[0];

                    //- int m_id;      // 회득한 아이템 아이디
                    string dataType = String.Empty;

                    if( element.Contains( "bool" ) )
                        dataType = "bool";
                    else if( element.Contains( "byte" ) )
                        dataType = "byte";
                    else if ( element.Contains( "sbyte" ) )
                        dataType = "sbyte";
                    else if ( element.Contains( "short" ) )
                        dataType = "short";
                    else if ( element.Contains( "ushort" ) )
                        dataType = "ushort";
                    else if ( element.Contains( "int" ) )
                        dataType = "int";
                    else if ( element.Contains( "uint" ) )
                        dataType = "uint";
                    else if ( element.Contains( "long" ) )
                        dataType = "long";
                    else if ( element.Contains( "ulong" ) )
                        dataType = "ulong";
                    else if ( element.Contains( "float" ) )
                        dataType = "float";
                    else if ( element.Contains( "double" ) )
                        dataType = "double";
                    else if ( element.Contains( "decimal" ) )
                        dataType = "decimal";
                    else if ( element.Contains( "char" ) )
                        dataType = "char";
                    else if ( element.Contains( "string" ) )
                        dataType = "string";

                    var varName = element.Replace(dataType, String.Empty);
                    var varRealName = varName.Replace(";", String.Empty);
                    packetElementInfo.dataType = Utils.ConvertDataType(dataType);
                    packetElementInfo.varName = varRealName;

                    if (packetElementList.Length > 1)
                        packetElementInfo.desc = packetElementList[1];
                    else
                        packetElementInfo.desc = "no Desc";
                    
                    var packetInfoPair = m_packetInfoDict[lastPacketCategory];
                    if ( packetInfoPair == null )
                    {
                        Console.WriteLine( "Check Packet Data [<<]" );
                        return;
                    }

                    var packetInfo = packetInfoPair[lastPacketIndex -1 ];
                    if ( packetInfo.packetElementList == null )
                        packetInfo.packetElementList = new List<PacketElementInfo>();

                    packetInfo.packetElementList.Add( packetElementInfo );
                }
            }
        }
    }

    public virtual void Convert()
    {
        _ConvertToClientMessagReceiver();
        _ConvertToClientMessageSender();

        _ConvertToServerMessageReceiver();
        _ConvertToServerMessageSender();

        _ConvertToClientMessageHandler();
        _ConvertToServerMessageHandler();

        _ConvertToPktPacket();
        _ConvertToPktServerToClientPacket();
        _ConvertToPktClientToServerPacket();

        // 불변.
        _ConvertToPacketHandler();
        _ConvertToPacket();
        _ConverToEunm();
        // packet enum 추가


        // 별도 패킷들.



        //Enum.
    }

    public void _ConvertToClientMessagReceiver()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "ClientMessageReceiver";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [ ClientMessageReceiver ]" );
        builder.AppendLine( "/// @brief    [ 클라이언트 패킷 수신 처리 클래스 ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine("public partial class " + className + " : Singleton< " + className + " >");
        builder.AppendLine( "{" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Initialize()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        NetworkManager.Singleton.OnServerStarted += RegisterMessageHandlers;" );
        builder.AppendLine( "        NetworkManager.Singleton.OnClientStarted += RegisterMessageHandlers;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 개체 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Reset()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        NetworkManager.Singleton.OnServerStarted -= UnRegisterMessageHandlers;" );
        builder.AppendLine( "        NetworkManager.Singleton.OnClientStarted -= UnRegisterMessageHandlers;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 메세지 핸들러에 등록합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void RegisterMessageHandlers()" );
        builder.AppendLine( "    {" );

        // private Dictionary<string, Dictionary<uint, PacketInfo>> m_packetInfoDict;
        foreach ( var packetInfoPair in m_packetInfoDict)
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach( var packetInfo in packetInfoDict)
            {
                if (packetInfo.Value.packetDirectType == EPacketDirectType.ClientToServer)
                    continue;

                string protocolEnumName = "EProtocolType." + packetInfo.Value.packetName;
                string funcName = "Recv_" + packetInfo.Value.packetName;

                builder.AppendLine( "        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler( " + protocolEnumName + ".ToString(), " + funcName + " );" );
            }
        }

        builder.AppendLine( "    }" );
        builder.AppendLine( "" );

        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 메세지 핸들러에 등록을 해제합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void UnRegisterMessageHandlers()" );
        builder.AppendLine( "    {" );

        // private Dictionary<string, Dictionary<uint, PacketInfo>> m_packetInfoDict;
        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ClientToServer )
                    continue;

                string protocolEnumName = "EProtocolType." + packetInfo.Value.packetName;
                string funcName = "Recv_" + packetInfo.Value.packetName;

                builder.AppendLine( "        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler( " + protocolEnumName + ".ToString() );" );
            }
        }

        builder.AppendLine( "    }" );
        builder.AppendLine( "" );

        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ClientToServer )
                    continue;

                string funcName2 = "Recv_" + packetInfo.Value.packetName;

                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    /// <summary> @brief [ "+ packetInfo.Value.packetName + " 패킷 수신 처리를 합니다. ] </summary>" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    public void " + funcName2 + "( ulong senderClientId, FastBufferReader messagePayLoad )" );
                builder.AppendLine( "    {" );
                builder.AppendLine( "        byte[] recvData;" );
                builder.AppendLine( "        messagePayLoad.ReadValueSafe( out recvData );" );
                builder.AppendLine( "        " + packetInfo.Value.packetName + " message = (" + packetInfo.Value.packetName + ")PktUtil.ToObject( recvData );" );
                builder.AppendLine( "        ClientMessageHandler.Process( message );" );
                builder.AppendLine( "    }" );
                builder.AppendLine( "" );
            }
        }

        builder.AppendLine( "}" );
        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString());
    }

    public void _ConvertToClientMessageSender()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "ClientMessageSender";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "using Unity.Collections;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [ ClientMessageSender ]" );
        builder.AppendLine( "/// @brief    [ 클라이언트 패킷 송신 처리 클래스 ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public partial class " + className + " : Singleton< " + className + " >" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Initialize()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        // empty;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 개체 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Reset()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        // empty;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 송신 합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public static void Send<T>( T packet ) where T : PktPacket" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        byte[] serializedData = PktUtil.ToBytes(packet);" );
        builder.AppendLine( "        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(serializedData), Allocator.Temp);" );
        builder.AppendLine( "        using ( writer )" );
        builder.AppendLine( "        {" );
        builder.AppendLine( "            writer.WriteValueSafe( serializedData );" );
        builder.AppendLine( "            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(" );
        builder.AppendLine( "                packet.ProtocolType.ToString(), NetworkManager.ServerClientId, writer );" );
        builder.AppendLine( "        }" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "}" );
        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString() );
    }

    public void _ConvertToServerMessageReceiver()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "ServerMessageReceiver";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [ ServerMessageReceiver ]" );
        builder.AppendLine( "/// @brief    [ 서버 패킷 수신 처리 클래스 ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public partial class " + className + " : Singleton< " + className + " >" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Initialize()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        NetworkManager.Singleton.OnServerStarted += RegisterMessageHandlers;" );
        builder.AppendLine( "        NetworkManager.Singleton.OnClientStarted += RegisterMessageHandlers;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 개체 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Reset()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        NetworkManager.Singleton.OnServerStarted -= UnRegisterMessageHandlers;" );
        builder.AppendLine( "        NetworkManager.Singleton.OnClientStarted -= UnRegisterMessageHandlers;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 메세지 핸들러에 등록합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void RegisterMessageHandlers()" );
        builder.AppendLine( "    {" );

        // private Dictionary<string, Dictionary<uint, PacketInfo>> m_packetInfoDict;
        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ServerToClient )
                    continue;

                string protocolEnumName = "EProtocolType." + packetInfo.Value.packetName;
                string funcName = "Recv_" + packetInfo.Value.packetName;

                builder.AppendLine( "        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler( " + protocolEnumName + ".ToString(), " + funcName + " );" );
            }
        }

        builder.AppendLine( "    }" );
        builder.AppendLine( "" );

        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 메세지 핸들러에 등록을 해제합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void UnRegisterMessageHandlers()" );
        builder.AppendLine( "    {" );

        // private Dictionary<string, Dictionary<uint, PacketInfo>> m_packetInfoDict;
        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ServerToClient )
                    continue;

                string protocolEnumName = "EProtocolType." + packetInfo.Value.packetName;
                string funcName = "Recv_" + packetInfo.Value.packetName;

                builder.AppendLine( "        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler( " + protocolEnumName + ".ToString() );" );
            }
        }

        builder.AppendLine( "    }" );
        builder.AppendLine( "" );

        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ServerToClient )
                    continue;

                string funcName2 = "Recv_" + packetInfo.Value.packetName;

                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    /// <summary> @brief [ " + packetInfo.Value.packetName + " 패킷 수신 처리를 합니다. ] </summary>" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    public void " + funcName2 + "( ulong senderClientId, FastBufferReader messagePayLoad )" );
                builder.AppendLine( "    {" );
                builder.AppendLine( "        byte[] recvData;" );
                builder.AppendLine( "        messagePayLoad.ReadValueSafe( out recvData );" );
                builder.AppendLine( "        " + packetInfo.Value.packetName + " message = (" + packetInfo.Value.packetName + ")PktUtil.ToObject( recvData );" );
                builder.AppendLine( "        ServerMessageHandler.Process( message );" );
                builder.AppendLine( "    }" );
                builder.AppendLine( "" );
            }
        }

        builder.AppendLine( "}" );
        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString() );
    }

    public void _ConvertToServerMessageSender()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "ServerMessageSender";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "using Unity.Collections;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [ ServerMessageSender ]" );
        builder.AppendLine( "/// @brief    [ 서버 패킷 송신 처리 클래스 ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public partial class " + className + " : Singleton< " + className + " >" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Initialize()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        // empty;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 개체 초기화를 진행합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public void Reset()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        // empty;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 송신 합니다. ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public static void Send<T>( T packet ) where T : PktPacket" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        byte[] serializedData = PktUtil.ToBytes(packet);" );
        builder.AppendLine( "        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(serializedData), Allocator.Temp);" );
        builder.AppendLine( "        using ( writer )" );
        builder.AppendLine( "        {" );
        builder.AppendLine( "            writer.WriteValueSafe( serializedData );" );
        builder.AppendLine( "            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(" );
        builder.AppendLine( "                packet.ProtocolType.ToString(), NetworkManager.Singleton.ConnectedClientsIds, writer );" );
        builder.AppendLine( "        }" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "}" );
        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString() );
    }

    public void _ConvertToClientMessageHandler()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "ClientMessageHandler";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [ ClientMessageHandler ]" );
        builder.AppendLine( "/// @brief    [ 클라이언트 패킷 처리 클래스 ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public partial class " + className + " : Singleton< " + className + " >" );
        builder.AppendLine( "{" );

        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ClientToServer )
                    continue;

                string value = Utils.LowerFirstChar(packetInfo.Value.packetName);
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    /// <summary> @brief [ " + packetInfo.Value.packetName + " 을 실행합니다. ] </summary>" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    public static void Process ( " + packetInfo.Value.packetName + " " + value + " )" );
                builder.AppendLine( "    {" );
                builder.AppendLine( "        if( " + value + " == null )" );
                builder.AppendLine( "            return;" );
                builder.AppendLine( "" );
                builder.AppendLine( "        " + packetInfo.Value.packetName + "Handler.Process( " + value + ".ObjId, " + value + " );" );
                builder.AppendLine( "    }" );
                builder.AppendLine( "" );
            }
        }

        builder.AppendLine( "}" );
        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString() );
    }

    public void _ConvertToServerMessageHandler()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "ServerMessageHandler";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [ ServerMessageHandler ]" );
        builder.AppendLine( "/// @brief    [ 서버 패킷 처리 클래스 ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public partial class " + className + " : Singleton< " + className + " >" );
        builder.AppendLine( "{" );

        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ServerToClient )
                    continue;

                string value = Utils.LowerFirstChar(packetInfo.Value.packetName);
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    /// <summary> @brief [ " + packetInfo.Value.packetName + " 을 실행합니다. ] </summary>" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    public static void Process ( " + packetInfo.Value.packetName + " " + value + " )" );
                builder.AppendLine( "    {" );
                builder.AppendLine( "        if( "+ value + " == null )" );
                builder.AppendLine( "            return;" );
                builder.AppendLine( "" );
                builder.AppendLine( "        " + packetInfo.Value.packetName + "Result result = new "+ packetInfo.Value.packetName + "Result();" );
                builder.AppendLine( "        " + packetInfo.Value.packetName + "Handler.Process( "+ value +".ObjId, " + value + ", result );" );
                builder.AppendLine( "        ServerMessageSender.Send( result );" );
                builder.AppendLine( "    }" );
                builder.AppendLine( "" );
            }
        }

        builder.AppendLine( "}" );
        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString() );
    }

    public void _ConvertToPktPacket()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "PktPacket";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "using Unity.Collections;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [ PktPacket ]" );
        builder.AppendLine( "/// @brief    [ 패킷 기본 클래스 ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "[System.Serializable]" );
        builder.AppendLine( "public abstract class PktPacket" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    /// <summary> 프로토콜 타입 </summary>" );
        builder.AppendLine( "    private EProtocolType m_protocolType;" );
        builder.AppendLine( "    public EProtocolType ProtocolType => m_protocolType;" );
        builder.AppendLine( "" );
        builder.AppendLine( "    /// <summary> 오브젝트 아이디 </summary>" );
        builder.AppendLine( "    private long m_objId;" );
        builder.AppendLine( "    public long ObjId => m_objId;" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 생성자 ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public PktPacket( EProtocolType protocolType )" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        m_protocolType = protocolType;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 패킷 전송 방향 타입 ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public abstract EPacketDirectionType GetPacketDirectionType();" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 스트링으로 변환 ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public abstract string ToString();" );
        builder.AppendLine( "}" );
        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString() );
    }

    public void _ConvertToPktServerToClientPacket()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "PktServerToClientPacket";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "using Unity.Collections;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [ PktServerToClientPacket ]" );
        builder.AppendLine( "/// @brief    [ 서버에서 클라이언트 전송 패킷 ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "[System.Serializable]" );
        builder.AppendLine( "public abstract class PktServerToClientPacket : PktPacket" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 생성자 ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public PktServerToClientPacket( EProtocolType protocolType ) : base( protocolType )" );
        builder.AppendLine( "    {");
        builder.AppendLine( "        // empty;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [  패킷 전송 방향 타입 ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public override EPacketDirectionType GetPacketDirectionType()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        return EPacketDirectionType.ServerToClient;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "}" );

        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString() );
    }

    public void _ConvertToPktClientToServerPacket()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "PktClientToServerPacket";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "using Unity.Collections;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary>" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// @class    [ PktClientToServerPacket ]" );
        builder.AppendLine( "/// @brief    [ 서버에서 클라이언트 전송 패킷 ]" );
        builder.AppendLine( "///" );
        builder.AppendLine( "/// </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "[System.Serializable]" );
        builder.AppendLine( "public abstract class PktClientToServerPacket : PktPacket" );
        builder.AppendLine( "{" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [ 생성자 ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public PktClientToServerPacket( EProtocolType protocolType ) : base( protocolType )" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        // empty;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    /// <summary> @brief [  패킷 전송 방향 타입 ] </summary>" );
        builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "    public override EPacketDirectionType GetPacketDirectionType()" );
        builder.AppendLine( "    {" );
        builder.AppendLine( "        return EPacketDirectionType.ClientToServer;" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "" );
        builder.AppendLine( "}" );

        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString() );
    }

    public void _ConvertToPacketHandler()
    {
        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                StringBuilder builder = new StringBuilder(1000, 50000);

                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ServerToClient )
                    continue;

                string handlerNam = packetInfo.Value.packetName + "Handler";

                string className = packetInfo.Value.packetName ;
                string path = "../Assets/Scripts/Network/Packet";

                builder.AppendLine( "using System;" );
                builder.AppendLine( "using UnityEngine;" );
                builder.AppendLine( "using Unity.Netcode;" );
                builder.AppendLine( "" );
                builder.AppendLine( "" );
                builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "/// <summary>" );
                builder.AppendLine( "///" );
                builder.AppendLine( "/// @class    [ " + handlerNam + " ]" );
                builder.AppendLine( "/// @brief    [ " + handlerNam + " 처리 클래스 ]" );
                builder.AppendLine( "///" );
                builder.AppendLine( "/// </summary>" );
                builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "public class " + handlerNam );
                builder.AppendLine( "{" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    /// <summary> @brief [ 실행합니다. ] </summary>" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    public static void Process(" );
                builder.AppendLine( "        long userId," );
                builder.AppendLine( "        " + className + " " + Utils.LowerFirstChar( className ) + "," );
                builder.AppendLine( "        " + className + "Result " + Utils.LowerFirstChar( className ) + "Result )" );
                builder.AppendLine( "    {" );
                builder.AppendLine( "        // 구현이 필요합니다." );
                builder.AppendLine( "    }" );
                builder.AppendLine( "}" );

                //폴더 있는지 유무 확인 후 생성
                string directoryPath = path + "/" + packetInfo.Value.packetCategory;

                DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
                if ( !directoryInfo.Exists )
                    directoryInfo.Create();

                string fullFilePath = directoryPath + "/"+ handlerNam + ".cs";

                FileInfo fileInfo = new FileInfo( fullFilePath );
                if ( !fileInfo.Exists )
                    File.WriteAllText( fullFilePath, builder.ToString() );
            }
        }

        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                StringBuilder builder = new StringBuilder(1000, 50000);

                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ClientToServer )
                    continue;

                string handlerNam = packetInfo.Value.packetName + "Handler";

                string className = packetInfo.Value.packetName ;
                string path = "../Assets/Scripts/Network/Packet";
                
                builder.AppendLine( "using System;" );
                builder.AppendLine( "using UnityEngine;" );
                builder.AppendLine( "using Unity.Netcode;" );
                builder.AppendLine( "" );
                builder.AppendLine( "" );
                builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "/// <summary>" );
                builder.AppendLine( "///" );
                builder.AppendLine( "/// @class    [ " + handlerNam + " ]" );
                builder.AppendLine( "/// @brief    [ " + handlerNam + " 처리 클래스 ]" );
                builder.AppendLine( "///" );
                builder.AppendLine( "/// </summary>" );
                builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "public class " + handlerNam );
                builder.AppendLine( "{" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    /// <summary> @brief [ 실행합니다. ] </summary>" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    public static void Process(" );
                builder.AppendLine( "        long userId," );
                builder.AppendLine( "        " + className + " " + Utils.LowerFirstChar( className ) + " )" );
                builder.AppendLine( "    {" );
                builder.AppendLine( "        // 구현이 필요합니다." );
                builder.AppendLine( "    }" );
                builder.AppendLine( "}" );

                //폴더 있는지 유무 확인 후 생성
                string directoryPath = path + "/" + packetInfo.Value.packetCategory;

                DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
                if ( !directoryInfo.Exists )
                    directoryInfo.Create();

                string fullFilePath = directoryPath + "/"+ handlerNam + ".cs";

                FileInfo fileInfo = new FileInfo( fullFilePath );
                if ( !fileInfo.Exists )
                    File.WriteAllText( fullFilePath, builder.ToString() );
            }
        }
    }

    public void _ConvertToPacket()
    {
        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                StringBuilder builder = new StringBuilder(1000, 50000);

                string className = packetInfo.Value.packetName ;
                string path = "../Assets/Scripts/Network/Packet";
                string protocolEnumName = "EProtocolType." + packetInfo.Value.packetName;

                builder.AppendLine( "using System;" );
                builder.AppendLine( "using UnityEngine;" );
                builder.AppendLine( "using Unity.Netcode;" );
                builder.AppendLine( "" );
                builder.AppendLine( "" );
                builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "/// <summary>" );
                builder.AppendLine( "///" );
                builder.AppendLine( "/// @class    [ " + className + " ]" );
                builder.AppendLine( "/// @brief    [ " + className + " 패킷 클래스 ]" );
                builder.AppendLine( "///" );
                builder.AppendLine( "/// </summary>" );
                builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "[ System.Serializable]" );

                if ( packetInfo.Value.packetDirectType == EPacketDirectType.ClientToServer )
                    builder.AppendLine( "public class " + className + " : PktClientToServerPacket" );
                else if( packetInfo.Value.packetDirectType == EPacketDirectType.ServerToClient )
                    builder.AppendLine( "public class " + className + " : PktServerToClientPacket" );

                builder.AppendLine( "{" );

                foreach (var packetElement in packetInfo.Value.packetElementList)
                {
                    var valNameTemp = packetElement.varName.Replace("m_", String.Empty);
                    var publicVale = Utils.UpperFirstChar(valNameTemp);
                    builder.AppendLine( "" );
                    builder.AppendLine( "    /// <summary> " + packetElement.desc + " </summary>" );
                    builder.AppendLine( "    private " + Utils.ConvertDataTypeToString( packetElement.dataType ) + " " + packetElement.varName + ";" );
                    builder.AppendLine( "    public " + Utils.ConvertDataTypeToString( packetElement.dataType ) + " " + publicVale + " { get; set; }" );
                }

                builder.AppendLine( "" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    /// <summary> @brief [ 생성자 ] </summary>" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    public " + className + "() : base( " + protocolEnumName + " )" );
                builder.AppendLine( "    {" );
                builder.AppendLine( "        // empty; " );
                builder.AppendLine( "    }" );
                builder.AppendLine( "" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    /// <summary> @brief [ 문자열로 변환합니다.(디버깅) ] </summary>" );
                builder.AppendLine( "    ////////////////////////////////////////////////////////////////////////////////////////////////" );
                builder.AppendLine( "    public override string ToString()" );
                builder.AppendLine( "    {" );

                string toStringValue = "return $\"";

                foreach ( var packetElement in packetInfo.Value.packetElementList )
                {
                    var valNameTemp = packetElement.varName.Replace("m_", String.Empty);
                    var publicVale = Utils.UpperFirstChar(valNameTemp);
                    toStringValue += (publicVale + " : {" + packetElement.varName + "},");
                }
                toStringValue += "\";";

                builder.AppendLine( "        " + toStringValue );
                builder.AppendLine( "    }" );
                builder.AppendLine( "}" );

                //폴더 있는지 유무 확인 후 생성
                string directoryPath = path + "/" + packetInfo.Value.packetCategory;

                DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
                if ( !directoryInfo.Exists )
                    directoryInfo.Create();

                string fullFilePath = directoryPath + "/"+ className + ".cs";

                File.WriteAllText( fullFilePath, builder.ToString() );
            }
        }
    }

    public void _ConverToEunm()
    {
        StringBuilder builder = new StringBuilder(1000, 50000);

        string className = "PktProtocolEnum";
        string path = "../Assets/Scripts/Network/Packet";
        string fullFilePath = path + "/"+ className + ".cs";

        builder.AppendLine( "using System;" );
        builder.AppendLine( "using UnityEngine;" );
        builder.AppendLine( "using Unity.Netcode;" );
        builder.AppendLine( "" );
        builder.AppendLine( "" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "/// <summary> 프로토콜 이넘 타입 </summary>" );
        builder.AppendLine( "////////////////////////////////////////////////////////////////////////////////////////////////////" );
        builder.AppendLine( "public enum EProtocolType" );
        builder.AppendLine( "{" );

        foreach ( var packetInfoPair in m_packetInfoDict )
        {
            var packetInfoDict = packetInfoPair.Value;
            foreach ( var packetInfo in packetInfoDict )
            {
                builder.AppendLine("    " + packetInfo.Value.packetName + " = "+ packetInfo.Value.packetIndex + ", //< " + packetInfo.Value.packetDesc );
            }
        }
        builder.AppendLine( "    Max, //< 이 열거자의 최대값" );
        builder.AppendLine( "}" );
        //폴더 있는지 유무 확인 후 생성
        string directoryPath = path;

        DirectoryInfo directoryInfo = new DirectoryInfo( directoryPath );
        if ( !directoryInfo.Exists )
            directoryInfo.Create();

        File.WriteAllText( fullFilePath, builder.ToString() );
    }
}


// 나중에 분리.
public class PacketInfo
{
    /// 패킷 카테고리.[Item, Actor,,,]
    public string packetCategory;

    /// 패킷 넘버링 
    public uint packetIndex;

    /// 패킷 네이밍
    public string packetName;

    /// 패킷 전송 방향
    public EPacketDirectType packetDirectType;

    /// 패킷 디스크립션 
    public string packetDesc;

    /// 패킷 원소 정보 리스트
    public List<PacketElementInfo> packetElementList;
}

// 나중에 분리.
public class PacketElementInfo
{
    /// 데이터 타입
    public EDataType dataType;
    
    /// 변수 이름
    public string varName;

    /// 디스크립션
    public string desc;
}

