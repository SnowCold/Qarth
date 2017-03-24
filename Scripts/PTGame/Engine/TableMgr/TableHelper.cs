// Auto Generate Don't Edit it
using System;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public static class TableHelper
    {
        public static string GetTableFilePath(string fileName)
        {
#if USE_TABLE_XC
            return "config/" + fileName + ".xc";
#else
            return "config/" + fileName + ".txt";
#endif
        }

        public static void CacheNewField(DataStreamReader dataR, string fieldName,  Dictionary<string, TDUniversally.FieldData> fieldCacheDict)
        {
            if (fieldCacheDict.ContainsKey(fieldName) == false)
            {
                switch (dataR.NextFiledType())
                {
                    case DataStreamReader.FieldType.Bool:
                        fieldCacheDict.Add(fieldName, new TDUniversally.FieldData(dataR.ReadBool()));
                        break;
                    case DataStreamReader.FieldType.Int:
                        fieldCacheDict.Add(fieldName, new TDUniversally.FieldData(dataR.ReadInt()));
                        break;
                    case DataStreamReader.FieldType.Float:
                        fieldCacheDict.Add(fieldName, new TDUniversally.FieldData(dataR.ReadFloat()));
                        break;
                    case DataStreamReader.FieldType.String:
                        fieldCacheDict.Add(fieldName, new TDUniversally.FieldData(dataR.ReadString()));
                        break;
                    case DataStreamReader.FieldType.Vector2:
                        fieldCacheDict.Add(fieldName, new TDUniversally.FieldData(dataR.ReadVector2()));
                        break;
                    case DataStreamReader.FieldType.Vector3:
                        fieldCacheDict.Add(fieldName, new TDUniversally.FieldData(dataR.ReadVector3()));
                        break;
                }
            }
        }
    }
}

