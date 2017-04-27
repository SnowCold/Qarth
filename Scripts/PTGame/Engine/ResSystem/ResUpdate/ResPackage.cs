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
            return string.Format("http://localhost:8080/ptupdate/pailogic/res/new/{0}", assetName);
        }

        public string GetAssetUrl(string assetName)
        {
            return ABName2URL(assetName);
        }

        public string packageName
        {
            get { return m_PackageName; }
        }

        //本地config的相对路径
        public string configFile
        {
            get { return string.Format("{0}{1}/{2}", ProjectPathConfig.abRelativePath ,m_PackageName, "asset_bindle_config.bin"); }
        }

        public string GetABLocalRelativePath(string abName)
        {
            return string.Format("{0}{1}", ProjectPathConfig.abRelativePath, abName);
        }


        //资源包更新地址
        public string updateUrl
        {
            get { return "http://localhost:8080/ptupdate/pailogic/res/"; }
        }

        //资源包整包下载zip地址
        public string zipUrl
        {
            get { return "http://localhost:8080/ptupdate/pailogic/res/"; }
        }

        //资源包在本地的文件路径
        public string localPath
        {
            get { return ProjectPathConfig.abRelativePath + m_PackageName; }
        }
    }
}
