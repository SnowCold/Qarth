using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class ResPackage
    {
        //上层直接和ResPackage交互
        private string m_PackageName;
        private string m_ZipFileName;

        public ResPackage(string packageName)
        {
            m_PackageName = packageName;
        }


        public string packageName
        {
            get { return m_PackageName; }
        }

        //本地config的相对路径
        public string configFile
        {
            get { return string.Format("{0}{1}/{2}", ProjectPathConfig.abRelativePath ,m_PackageName, ProjectPathConfig.abConfigfileName); }
        }

        //资源包整包下载zip地址
        public string zipUrl
        {
            get
            {
                return string.Format("http://localhost:8080/ptupdate/pailogic/zip/{0}", zipFileName);
            }
        }

        public string zipFileName
        {
            get
            {
                if (string.IsNullOrEmpty(m_ZipFileName))
                {
                    m_ZipFileName = m_PackageName.Replace('/', '_');
                    m_ZipFileName = m_ZipFileName + ".zip";
                }
                return m_ZipFileName;
            }
        }

        //资源包在本地的文件路径
        public string relativeLcalParentFolder
        {
            get
            {
                string parentFolder = m_PackageName.Substring(0, m_PackageName.LastIndexOf('/') + 1);
                return ProjectPathConfig.abRelativePath + parentFolder;
            }
        }

        public string GetABLocalRelativePath(string abName)
        {
            return string.Format("{0}{1}", ProjectPathConfig.abRelativePath, abName);
        }

        public string GetAssetUrl(string assetName)
        {
            return string.Format("http://localhost:8080/ptupdate/pailogic/res/new/{0}", assetName);
        }
    }
}
