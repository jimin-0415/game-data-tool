using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum DataLoadType
{ 
    None,
    Excel,
    Max,
}

/// <summary>
/// 엑셀 테이블 컬럼 타입
/// </summary>
public enum EExcelRowType
{
    Desc,               // Colum Desc - 맨 윗줄은 설명
    DataLoad,           // EDataLoadType - client, server, desc
    ReferenceTable,     // Refernce Table Name - 참조 테이블
    DataType,           // EDataType
    Name,               // var Name - 변수 이름
    Max,                // Max
}

/// <summary>
/// Data Load Type
/// </summary>
public enum EDataLoadType
{
    None,
    Client,
    Server,
    All,
    Max,
}

/// <summary>
/// Data Type
/// </summary>
public enum EDataType
{
    None,
    Bool,
    Byte,
    SByte,
    Short,
    Int,
    Long,
    UShort,
    UInt,
    ULong,
    Float,
    Double,
    Decimal,
    Char,
    String,
    DateTime,
    Object,
    Unique,
    Desc,
    Max,
}

public enum ManagerClassType
{
    ExcelName,
    AbstractClass,   
    Max,
}

