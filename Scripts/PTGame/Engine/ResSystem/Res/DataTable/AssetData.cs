using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    [Serializable]
    public class AssetData
    {
        private string  m_AssetName;
        private int     m_AbIndex;
        private short   m_AssetType;

        public string assetName
        {
            get { return m_AssetName; }
            set { m_AssetName = value; }
        }

        public int assetBundleIndex
        {
            get { return m_AbIndex; }
            set { m_AbIndex = value; }
        }

        public short assetType
        {
            get { return m_AssetType; }
            set { m_AssetType = value; }
        }

        public AssetData(string assetName, short assetType, int abIndex)
        {
            m_AssetName = assetName;
            m_AssetType = assetType;
            m_AbIndex = abIndex;
        }

        public AssetData()
        {

        }
    }
}
