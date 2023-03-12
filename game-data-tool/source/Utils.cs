using System;

public static class Utils
{
    /// string 으로 들어온 데이터를 DataLoadType으로 변환시켜준다
    public static EDataLoadType ConvertDataLoadType(string data)
    {
        if (data == null || data.Length <= 0)
            return EDataLoadType.Max;

        switch (data.ToUpper())
        {
            case "C":
            case "CLIENT":
                return EDataLoadType.Client;
            case "S":
            case "SERVER":
                return EDataLoadType.Server;
            case "ALL":
            case "A":
            case "SC":
            case "CS":
                return EDataLoadType.All;
            default:
                return EDataLoadType.Max;
        }
    }

    /// string 으로 들어온 데이터를 DataType으로 변환시켜준다
    public static EDataType ConvertDataType(string data)
    {
        String.Concat(data.Where(t => !Char.IsWhiteSpace(t)));
        if (data == null || data.Length <= 0)
            return EDataType.None;

        switch (data.ToUpper())
        {
            case "BOOL":
                return EDataType.Bool;
            case "BYTE":
                return EDataType.Byte;
            case "SBYTE":
                return EDataType.SByte;
            case "SHORT":
            case "INT16":
                return EDataType.Short;
            case "USHORT":
            case "UINT16":
                return EDataType.UShort;
            case "INT":
            case "INTEGER":
            case "INT32":
                return EDataType.Int;
            case "UINT":
            case "UINT32":
                return EDataType.UInt;
            case "LONG":
            case "INT64":
                return EDataType.Long;
            case "ULONG":
            case "UINT64":
                return EDataType.ULong;
            case "FLOAT":
                return EDataType.Float;
            case "DOUBLE":
                return EDataType.Double;
            case "DECIMAL":
                return EDataType.Decimal;
            case "CHAR":
                return EDataType.Char;
            case "STRING":
                return EDataType.String;
            case "DATETIME":
            case "TIME":
                return EDataType.DateTime;
            case "OBJECT":
                return EDataType.Object;
            default:
                return EDataType.None;
        }
    }

    public static string RemoveWhiteSpace(string target)
    {
        if (target == null)
            return String.Empty;

        return String.Concat(target.Where(t => !Char.IsWhiteSpace(t)));
    }

    public static string ToJsonKey(string name)
    {
        return "\"" + name + "\":";
    }
}


