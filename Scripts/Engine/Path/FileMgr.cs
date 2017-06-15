//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;

namespace Qarth
{
    public class FileMgr: TSingleton<FileMgr>
    {
        private List<string>    m_SearchDirList = new List<string>();
        private string          m_StreamingAssetsPath;
        private ZipFile         m_ZipFile = null;

        ~FileMgr()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (m_ZipFile != null)
        {
            m_ZipFile.Close();
            m_ZipFile = null;
        }
#endif
        }


        public override void OnSingletonInit()
        {
            m_SearchDirList.Add(FilePath.persistentDataPath4Res);
            m_StreamingAssetsPath = FilePath.streamingAssetsPath;
#if (UNITY_ANDROID) && !UNITY_EDITOR
            if (m_ZipFile == null)
            {
                m_ZipFile = new ZipFile(File.Open(Application.dataPath, FileMode.Open, FileAccess.Read));
            }
#endif
        }

        public void InitStreamingAssetPath()
        {
            m_StreamingAssetsPath = FilePath.streamingAssetsPath;
        }

        //在包内查找是否有改资源
        private bool FindResInAppInternal(string fileRelativePath)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
        string absoluteFilePath = FindFilePathInternal(fileRelativePath);
        return !string.IsNullOrEmpty(absoluteFilePath);
#elif UNITY_ANDROID && !UNITY_EDITOR
        int entryIndex = m_ZipFile.FindEntry(string.Format("assets/{0}", fileRelativePath), false);
        return entryIndex != -1;
#else
            string absoluteFilePath = string.Format("{0}{1}", m_StreamingAssetsPath, fileRelativePath);
            return File.Exists(absoluteFilePath);
#endif

        }

        private void AddSearchPath(string dir)
        {
            m_SearchDirList.Add(dir);
        }

        public bool FileExists(string fileRelativePath)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
        string absoluteFilePath = FindFilePath(fileRelativePath);
        return (!string.IsNullOrEmpty(absoluteFilePath) && File.Exists(absoluteFilePath));
#elif UNITY_ANDROID && !UNITY_EDITOR
        string absoluteFilePath = FindFilePathInExteral(fileRelativePath);
        //先到外存去找
        if (!string.IsNullOrEmpty(absoluteFilePath))
        {
            return File.Exists(absoluteFilePath);
        }
        else
        {
            if (m_ZipFile == null)
            {
                return false;
            }

            return m_ZipFile.FindEntry(string.Format("assets/{0}", fileRelativePath), true) >= 0;
        }
#else
            string filePathStandalone = string.Format("{0}{1}", m_StreamingAssetsPath, fileRelativePath);
            return (!string.IsNullOrEmpty(filePathStandalone) && File.Exists(filePathStandalone));
#endif
        }

        public Stream OpenReadStream(string absFilePath)
        {
            if (string.IsNullOrEmpty(absFilePath))
            {
                return null;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            //Android 包内
            if (absFilePath.Contains(".apk/"))
            {
                return OpenStreamInZip(absFilePath);
            }
#endif
            FileInfo fileInfo = new FileInfo(absFilePath);

            if (!fileInfo.Exists)
            {
                return null;
            }

            return fileInfo.OpenRead();
        }

        public void GetFileInInner(string fileName, List<string> outResult)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            //Android 包内
            GetFileInZip(m_ZipFile, fileName, outResult);
            return;
#endif
            FilePath.GetFileInFolder(FilePath.streamingAssetsPath, fileName, outResult); 
        }

        public byte[] ReadSync(string fileRelativePath)
        {
            string absoluteFilePath = FindFilePathInExteral(fileRelativePath);
            if (!string.IsNullOrEmpty(absoluteFilePath))
            {
                return ReadSyncExtenal(fileRelativePath);
            }

            return ReadSyncInternal(fileRelativePath);
        }

        public byte[] ReadSyncByAbsoluteFilePath(string absoluteFilePath)
        {

            if (File.Exists(absoluteFilePath))
            {
                FileInfo fileInfo = new FileInfo(absoluteFilePath);
                return ReadFile(fileInfo);
            }
            else
            {
                return null;
            }
        }

        private byte[] ReadSyncExtenal(string fileRelativePath)
        {
            string absoluteFilePath = FindFilePathInExteral(fileRelativePath);

            if (!string.IsNullOrEmpty(absoluteFilePath))
            {
                FileInfo fileInfo = new FileInfo(absoluteFilePath);
                return ReadFile(fileInfo);
            }

            return null;
        }

        private byte[] ReadSyncInternal(string fileRelativePath)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return ReadDataInAndriodApk(fileRelativePath);
#else
            string absoluteFilePath = FindFilePathInternal(fileRelativePath);

            if (!string.IsNullOrEmpty(absoluteFilePath))
            {
                FileInfo fileInfo = new FileInfo(absoluteFilePath);
                return ReadFile(fileInfo);
            }
#endif

            return null;
        }


        private byte[] ReadFile(FileInfo fileInfo)
        {
            using (FileStream fileStream = fileInfo.OpenRead())
            {
                byte[] byteData = new byte[fileStream.Length];
                fileStream.Read(byteData, 0, byteData.Length);
                return byteData;
            }
        }

        private string FindFilePathInExteral(string file)
        {
            string filePath;
            for (int i = 0; i < m_SearchDirList.Count; ++i)
            {
                filePath = string.Format("{0}/{1}", m_SearchDirList[i], file);
                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return string.Empty;
        }

        private string FindFilePath(string file)
        {
            // 先到搜索列表里找
            string filePath = FindFilePathInExteral(file);
            if (!string.IsNullOrEmpty(filePath))
            {
                return filePath;
            }

            // 在包内找
            filePath = FindFilePathInternal(file);
            if (!string.IsNullOrEmpty(filePath))
            {
                return filePath;
            }

            return null;
        }

        private string FindFilePathInternal(string file)
        {
            string filePath = string.Format("{0}{1}", m_StreamingAssetsPath, file);

            if (File.Exists(filePath))
            {
                return filePath;
            }

            return null;
        }


        private Stream OpenStreamInZip(string absPath)
        {
            string tag = "!/assets/";
            string androidFolder = absPath.Substring(0, absPath.IndexOf(tag));

            int startIndex = androidFolder.Length + tag.Length;
            string relativePath = absPath.Substring(startIndex, absPath.Length - startIndex);

            ZipEntry zipEntry = m_ZipFile.GetEntry(string.Format("assets/{0}", relativePath));
            
            if (zipEntry != null)
            {
                return m_ZipFile.GetInputStream(zipEntry);
            }
            else
            {
                Log.e(string.Format("Can't Find File {0}", absPath));
            }

            return null;
        }

        public void GetFileInZip(ZipFile zipFile, string fileName, List<string> outResult)
        {
            int totalCount = 0;

            foreach (var entry in zipFile)
            {
                ++totalCount;
                ICSharpCode.SharpZipLib.Zip.ZipEntry e = entry as ICSharpCode.SharpZipLib.Zip.ZipEntry;
                if (e != null)
                {
                    if (e.IsFile)
                    {
                        if (e.Name.EndsWith(fileName))
                        {
                            outResult.Add(zipFile.Name + "/!/" + e.Name);
                        }
                    }
                }
            }
        }


        private byte[] ReadDataInAndriodApk(string fileRelativePath)
        {
            byte[] byteData = null;
            //Log.i("Read From In App...");
            if (m_ZipFile == null)
            {
                Log.e("can't open apk");
                return null;
            }

            //Log.i("Begin Open Zip...");
            ZipEntry zipEntry = m_ZipFile.GetEntry(string.Format("assets/{0}", fileRelativePath));
            //Log.i("End Open Zip...");
            if (zipEntry != null)
            {
                //Log.i("Begin Read Zip...");
                var stream = m_ZipFile.GetInputStream(zipEntry);
                byteData = new byte[zipEntry.Size];
                //Log.i("Read Zip Length:" + byteData.Length);
                stream.Read(byteData, 0, byteData.Length);
                stream.Close();
            }
            else
            {
                Log.e(string.Format("Can't Find File {0}", fileRelativePath));
            }

            return byteData;
        }
    }
}
