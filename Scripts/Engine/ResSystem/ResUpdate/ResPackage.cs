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

namespace Qarth
{
    public class ResPackage
    {
        //上层直接和ResPackage交互
        private string m_PackageName;
        private string m_RelativePath;
        private string m_ZipFileName;
        private List<string> m_UpdateBlackList;

        public ResPackage(string packageName, string relativePath)
        {
            m_PackageName = packageName;
            m_RelativePath = relativePath;
        }

        public string packageName
        {
            get { return m_PackageName; }
        }

        //本地config的相对路径
        public string configFile
        {
            get { return string.Format("{0}{1}/{2}", m_RelativePath, m_PackageName, ProjectPathConfig.abConfigfileName); }
        }

        //资源包整包下载zip地址
        public string zipUrl
        {
            get
            {
                return ResUpdateConfig.ZIP_WEB_URL + zipFileName;
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
        public string relativeLocalParentFolder
        {
            get
            {
                string parentFolder = m_PackageName.Substring(0, m_PackageName.LastIndexOf('/') + 1);
                return m_RelativePath + parentFolder;
            }
        }

        public string relativeLocalPackageFolder
        {
            get
            {
                return m_RelativePath + m_PackageName;
            }
        }

        public string GetABLocalRelativePath(string abName)
        {
            return string.Format("{0}{1}", m_RelativePath, abName);
        }

        public string GetAssetUrl(string assetName)
        {
            return ResUpdateConfig.RES_WEB_URL + assetName;
        }

        public List<string> updateBlackList
        {
            get
            {
                return m_UpdateBlackList;
            }

            set
            {
                m_UpdateBlackList = value;
            }
        }
    }
}
