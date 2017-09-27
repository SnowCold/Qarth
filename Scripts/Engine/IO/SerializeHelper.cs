//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace Qarth
{
    public class SerializeHelper
    {
        public static bool SerializeJson(string path, object obj)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.w("SerializeJson Without Valid Path.");
                return false;
            }

            if (obj == null)
            {
                Log.w("SerializeJson obj is Null.");
                return false;
            }

            string jsonValue = null;
            try
            {
                jsonValue = JsonUtility.ToJson(obj);
            }
            catch (Exception e)
            {
                Log.e(e);
                return false;
            }

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] writeDataArray = UTF8Encoding.UTF8.GetBytes(jsonValue);
                fs.Write(writeDataArray, 0, writeDataArray.Length);
                fs.Flush();
            }

            return true;
        }

        public static T DeserializeJson<T>(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.w("DeserializeJson Without Valid Path.");
                return default(T);
            }

            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                return default(T);
            }

            using (FileStream stream = fileInfo.OpenRead())
            {
                try
                {
                    if (stream.Length <= 0)
                    {
                        stream.Close();
                        return default(T);
                    }

                    byte[] byteData = new byte[stream.Length];

                    stream.Read(byteData, 0, byteData.Length);

                    string context = UTF8Encoding.UTF8.GetString(byteData);

                    stream.Close();

                    if (string.IsNullOrEmpty(context))
                    {
                        return default(T);
                    }

                    return JsonUtility.FromJson<T>(context);
                }
                catch (Exception e)
                {
                    Log.e(e);
                }
            }

            Log.w("DeserializeJson Failed!");
            return default(T);
        }

        public static bool SerializeBinary(string path, object obj)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.w("SerializeBinary Without Valid Path.");
                return false;
            }

            if (obj == null)
            {
                Log.w("SerializeBinary obj is Null.");
                return false;
            }

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(fs, obj);
                return true;
            }
        }

        public static object DeserializeBinary(Stream stream)
        {
            if (stream == null)
            {
                Log.w("DeserializeBinary Failed!");
                return null;
            }

            using (stream)
            {
                try
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    object data = bf.Deserialize(stream);

                    stream.Close();

                    if (data != null)
                    {
                        return data;
                    }
                }
                catch (Exception e)
                {
                    Log.e(e);
                }
            }

            Log.w("DeserializeBinary Failed!");
            return null;
        }

        public static object DeserializeBinary(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.w("DeserializeBinary Without Valid Path.");
                return null;
            }

            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                Log.w("DeserializeBinary File Not Exit.");
                return null;
            }

            using (FileStream fs = fileInfo.OpenRead())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                object data = bf.Deserialize(fs);

                if (data != null)
                {
                    return data;
                }
            }

            Log.w("DeserializeBinary Failed:" + path);
            return null;
        }

        public static bool SerializeXML(string path, object obj)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.w("SerializeBinary Without Valid Path.");
                return false;
            }

            if (obj == null)
            {
                Log.w("SerializeBinary obj is Null.");
                return false;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                XmlSerializer xmlserializer = new XmlSerializer(obj.GetType());
                xmlserializer.Serialize(fs, obj);
                return true;
            }
        }

        public static object DeserializeXML<T>(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.w("DeserializeBinary Without Valid Path.");
                return null;
            }

            FileInfo fileInfo = new FileInfo(path);

            using (FileStream fs = fileInfo.OpenRead())
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                object data = xmlserializer.Deserialize(fs);

                if (data != null)
                {
                    return data;
                }
            }

            Log.w("DeserializeBinary Failed:" + path);
            return null;
        }
    }
}
