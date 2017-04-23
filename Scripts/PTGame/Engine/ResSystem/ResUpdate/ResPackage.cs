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
        private string m_ABConfigFile;

        public ResPackage(string packageName)
        {
            m_PackageName = packageName;
        }

        //TODO：处理远程路径，资源包和更新文件地址
        private static string ABName2URL(string assetName)
        {
            return string.Format("http://xczxczxczc/{0}", assetName);
        }

        public string GetAssetUrl(string assetName)
        {
            return ABName2URL(assetName);
        }

        public string packageName
        {
            get { return m_PackageName; }
        }

        //相对路径
        public string configFile
        {
            get { return ""; }
        }

        //资源包更新地址
        public string updateUrl
        {
            get { return ""; }
        }

        //资源包整包下载zip地址
        public string zipUrl
        {
            get { return ""; }
        }

        //资源包在本地的文件路径
        public string localPath
        {
            get { return ""; }
        }
    }
}
