using System;

public class ColumnInfo
{
    string desc;                //column Desc
    string referenceTableName;  //reference Table Name
    EDataLoadType dataLoadType; //data Load Type
    EDataType dataType;
    string objectDataType;
    string name;

    public ColumnInfo()
    {
        desc = String.Empty;     
        referenceTableName = String.Empty;
        name = String.Empty;
        dataLoadType = EDataLoadType.Max;
        dataType = EDataType.Max;
        objectDataType = String.Empty;
    }

    public void SetDesc(string desc)
    {
        if (desc == null)
        {
            this.desc = " ";
            return;
        }
        this.desc = desc;
    }

    public string GetDesc()
    {
        return this.desc;
    }

    public void SetReferenceTableName(string tableName)
    {
        this.referenceTableName = tableName;
    }

    public string GetReferenceTableName()
    {
        return this.referenceTableName;
    }

    public void SetDataLoadType(EDataLoadType dataLoadType)
    {
        this.dataLoadType = dataLoadType;
    }

    public void SetDataLoadType(string dataLoadType)
    {
        this.dataLoadType = Utils.ConvertDataLoadType(dataLoadType);
    }

    public EDataLoadType GetDataLoadType()
    {
        return dataLoadType;
    }

    public void SetDataType(string dataType)
    {
        if(Utils.ConvertDataType(dataType) == EDataType.Unique)
        {
            this.dataType = EDataType.Unique;
            this.objectDataType = dataType;
        }
        else
        {
            this.dataType = Utils.ConvertDataType(dataType);
        }
    }

    public EDataType GetDataType()
    {
        return this.dataType;
    }

    public string GetObjectDataType()
    {
        return this.objectDataType;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public string GetName()
    {
        return this.name;
    }
}
