//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_5_5
using UnityEngine.Profiling;
#endif

namespace Qarth
{
    [TMonoSingletonAttribute("[App]/MemoryMgr")]
    public class MemoryMgr : TMonoSingleton<MemoryMgr>
    {
        [SerializeField]
        private int m_MaxMemoryUse = 170;
        [SerializeField]
        private int m_MaxHeapMemoryUse = 50;
        [SerializeField]
        private bool m_DisplayOnGUI = true;

        public void Init()
        {
            Log.i("InitMrmoryMgr");
        }

#if !UNITY_EDITOR
        void Update()
        {
        //内存管理
        //MonitorMemorySize();
        }
#endif

        void OnGUI()
        {
            if (m_DisplayOnGUI)
            {
                float totalAllocatedMemory = ByteToM(Profiler.GetTotalAllocatedMemory());
                float reservedMemory = ByteToM(Profiler.GetTotalReservedMemory());

                float monoUsedSize = ByteToM(Profiler.GetMonoUsedSize());
                float monoHeapSize = ByteToM(Profiler.GetMonoHeapSize());
                
                GUILayout.TextField(string.Format("Reserved：{0}M, TotalAllocated:{1}M", reservedMemory, totalAllocatedMemory));
                GUILayout.TextField(string.Format("Heap：{0}M, HeadUsed:{1}M", monoHeapSize, monoUsedSize));
                GUILayout.TextField(string.Format("ResLoader:{0}, Res:{1}, AB:{2}", ResLoader.activeResLoaderCount, ResMgr.S.totalResCount, AssetBundleRes.s_ActiveCount));
            }
        }

        #region 内存监控

        // 字节到兆
        //const float ByteToM = 0.000001f;

        static bool s_isFreeMemory = false;
        static bool s_isFreeMemory2 = false;

        static bool s_isFreeHeapMemory = false;
        static bool s_isFreeHeapMemory2 = false;

        /// <summary>
        /// 用于监控内存
        /// </summary>
        /// <param name="tag"></param>
        void MonitorMemorySize()
        {
            if (ByteToM(Profiler.GetTotalReservedMemory()) > m_MaxMemoryUse * 0.7f)
            {
                if (!s_isFreeMemory)
                {
                    s_isFreeMemory = true;
                }

                if (ByteToM(Profiler.GetMonoHeapSize()) > m_MaxMemoryUse)
                {
                    if (!s_isFreeMemory2)
                    {
                        s_isFreeMemory2 = true;
                        Debug.LogError("总内存超标告警 ！当前总内存使用量： " + ByteToM(Profiler.GetTotalAllocatedMemory()) + "M");
                    }
                }
                else
                {
                    s_isFreeMemory2 = false;
                }
            }
            else
            {
                s_isFreeMemory = false;
            }

            if (ByteToM(Profiler.GetMonoUsedSize()) > m_MaxHeapMemoryUse * 0.7f)
            {
                if (!s_isFreeHeapMemory)
                {
                    s_isFreeHeapMemory = true;
                }

                if (ByteToM(Profiler.GetMonoUsedSize()) > m_MaxHeapMemoryUse)
                {
                    if (!s_isFreeHeapMemory2)
                    {
                        s_isFreeHeapMemory2 = true;
                        Debug.LogError("堆内存超标告警 ！当前堆内存使用量： " + ByteToM(Profiler.GetMonoUsedSize()) + "M");
                    }
                }
                else
                {
                    s_isFreeHeapMemory2 = false;
                }
            }
            else
            {
                s_isFreeHeapMemory = false;
            }
        }

        #endregion

        float ByteToM(uint byteCount)
        {
            return (float)(byteCount / (1024.0f * 1024.0f));
        }
    }
}
