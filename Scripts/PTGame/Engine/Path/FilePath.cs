using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class FilePath
    {
        private static string           m_PersistentDataPath;
        private static string           m_StreamingAssetsPath;
        private static string           m_PersistentDataPath4Res;
        private static string           m_PersistentDataPath4Photo;

        // 外部目录  
        public static string persistentDataPath
        {
            get
            {
                if (null == m_PersistentDataPath)
                {
                    m_PersistentDataPath = Application.persistentDataPath + "/";
                }

                return m_PersistentDataPath;
            }
        }

        // 内部目录
        public static string streamingAssetsPath
        {
            get
            {
                if (null == m_StreamingAssetsPath)
                {
#if UNITY_IPHONE && !UNITY_EDITOR
                    m_StreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID && !UNITY_EDITOR
                    m_StreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
                    m_StreamingAssetsPath = Application.streamingAssetsPath + "/";//GetParentDir(Application.dataPath, 2) + "/BuildRes/";
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
                    m_StreamingAssetsPath = Application.streamingAssetsPath + "/";
#else
                    //m_StreamingAssetsPath = GetParentDir(Application.dataPath, 1) + "/BuildRes/standalone/";
                    m_StreamingAssetsPath = Application.streamingAssetsPath + "/";
#endif
                }

                return m_StreamingAssetsPath;
            }
        }

        // 外部资源目录
        public static string persistentDataPath4Res
        {
            get
            {
                if (null == m_PersistentDataPath4Res)
                {
                    m_PersistentDataPath4Res = persistentDataPath + "Res/";

                    if (!Directory.Exists(m_PersistentDataPath4Res))
                    {
                        Directory.CreateDirectory(m_PersistentDataPath4Res);
#if UNITY_IPHONE && !UNITY_EDITOR
                        UnityEngine.iOS.Device.SetNoBackupFlag(m_PersistentDataPath4Res);
#endif
                    }
                }

                return m_PersistentDataPath4Res;
            }
        }

        // 外部头像缓存目录
        public static string persistentDataPath4Photo
        {
            get
            {
                if (null == m_PersistentDataPath4Photo)
                {
                    m_PersistentDataPath4Photo = persistentDataPath + "Photos\\";

                    if (!Directory.Exists(m_PersistentDataPath4Photo))
                    {
                        Directory.CreateDirectory(m_PersistentDataPath4Photo);
                    }
                }

                return m_PersistentDataPath4Photo;
            }
        }

        // 资源路径，优先返回外存资源路径
        public static string GetResPathInPersistentOrStream(string relativePath)
        {
            string resPersistentPath = string.Format("{0}{1}", FilePath.persistentDataPath4Res, relativePath);

            if (File.Exists(resPersistentPath))
            {
                return resPersistentPath;
            }
            else
            {
                return FilePath.streamingAssetsPath + relativePath;
            }
        }

        // 上一级目录
        public static string GetParentDir(string dir, int floor = 1)
        {
            string subDir = dir;

            for (int i = 0; i < floor; ++i)
            {
                int last = subDir.LastIndexOf('/');
                subDir = subDir.Substring(0, last);
            }

            return subDir;
        }

        public static void GetFileInFolder(string dirName, string fileName, List<string> outResult)
        {
            if (outResult == null)
            {
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(dirName);

            if (null != dir.Parent && dir.Attributes.ToString().IndexOf("System") > -1)
            {
                return;
            }

            FileInfo[] finfo = dir.GetFiles();
            string fname = string.Empty;
            for (int i = 0; i < finfo.Length; i++)
            {
                fname = finfo[i].Name;

                if (fname == fileName)
                {
                    outResult.Add(finfo[i].FullName);
                }
            }

            DirectoryInfo[] dinfo = dir.GetDirectories();
            for (int i = 0; i < dinfo.Length; i++)
            {
                GetFileInFolder(dinfo[i].FullName, fileName, outResult);
            }
        }

    }
}
