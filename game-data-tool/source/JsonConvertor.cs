using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class JsonConvertor
{ 
    private string targetData;
    private DataLoadType dataLoadType;
    
    public JsonConvertor(string targetData, DataLoadType dataLoadType) 
    {
        this.dataLoadType = dataLoadType;
        this.targetData = targetData;
    }

    public void ConvertToJson()
    {

    }
}

