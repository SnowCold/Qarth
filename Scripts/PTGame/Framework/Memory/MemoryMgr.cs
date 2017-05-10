using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_5_5
using UnityEngine.Profiling;
#endif

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[App]/MemoryMgr")]
    public class MemoryMgr : TMonoSingleton<MemoryMgr>
    {
        /// <summary>
        /// 最大允许的内存使用量
        /// </summary>
        public int s_MaxMemoryUse = 170;

        /// <summary>
        /// 最大允许的堆内存使用量
        /// </summary>
        public int s_MaxHeapMemoryUse = 50;

        public bool s_enable = true;

        public void Init()
        {

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
            if (s_enable)
            {
                float totalAllocatedMemory = ByteToM(Profiler.GetTotalAllocatedMemory());
                float reservedMemory = ByteToM(Profiler.GetTotalReservedMemory());

                float monoUsedSize = ByteToM(Profiler.GetMonoUsedSize());
                float monoHeapSize = ByteToM(Profiler.GetMonoHeapSize());
                
                GUILayout.TextField(string.Format("Reserved：{0}M, TotalAllocated:{1}M", reservedMemory, totalAllocatedMemory));
                GUILayout.TextField(string.Format("Heap：{0}M, HeadUsed:{1}M", monoHeapSize, monoUsedSize));
                GUILayout.TextField(string.Format("ResCount:{0}, ABCount:{1}", ResMgr.S.totalResCount, AssetBundleRes.s_ActiveCount));
                GUILayout.TextField("ResLoaderCount：" + ResLoader.activeResLoaderCount);
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
            if (ByteToM(Profiler.GetTotalReservedMemory()) > s_MaxMemoryUse * 0.7f)
            {
                if (!s_isFreeMemory)
                {
                    s_isFreeMemory = true;
                }

                if (ByteToM(Profiler.GetMonoHeapSize()) > s_MaxMemoryUse)
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

            if (ByteToM(Profiler.GetMonoUsedSize()) > s_MaxHeapMemoryUse * 0.7f)
            {
                if (!s_isFreeHeapMemory)
                {
                    s_isFreeHeapMemory = true;
                }

                if (ByteToM(Profiler.GetMonoUsedSize()) > s_MaxHeapMemoryUse)
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
