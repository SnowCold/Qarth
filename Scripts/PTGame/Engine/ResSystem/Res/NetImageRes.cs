using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PTGame.Framework
{
    public class NetImageRes : AbstractRes, IDownloadTask
    {
        private string      m_Url;
        private string      m_HashCode;
        private object      m_RawAsset;
        private WWW         m_WWW;

        public static NetImageRes Allocate(string name)
        {
            NetImageRes res = ObjectPool<NetImageRes>.S.Allocate();
            if (res != null)
            {
                res.name = name;
                res.SetUrl(name.Substring(9));
            }
            return res;
        }

        public void SetDownloadProgress(int totalSize, int download)
        {

        }

        public string localResPath
        {
            get
            {
                return string.Format("{0}{1}", FilePath.persistentDataPath4Photo, m_HashCode);
            }
        }

        public override object rawAsset
        {
            get { return m_RawAsset; }
        }

        public bool needDownload
        {
            get
            {
                return refCount > 0;
            }
        }

        public string url
        {
            get
            {
                return m_Url;
            }
        }

        public int fileSize
        {
            get
            {
                return 1;
            }
        }

        public void SetUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            m_Url = url;
            m_HashCode = string.Format("Photo_{0}", m_Url.GetHashCode());
        }

        public override bool UnloadImage(bool flag)
        {
            return false;
        }

        public override bool LoadSync()
        {
            return false;
        }

        public override void LoadAsync()
        {
            if (!CheckLoadAble())
            {
                return;
            }

            if (string.IsNullOrEmpty(m_Name))
            {
                return;
            }

            DoLoadWork();
        }

        private void DoLoadWork()
        {
            resState = eResState.kLoading;

            OnDownLoadResult(true);

            //检测本地文件是否存在
            /*
            if (!File.Exists(LocalResPath))
            {
                ResDownloader.S.AddDownloadTask(this);
            }
            else
            {
                OnDownLoadResult(true);
            }
            */
        }

        protected override void OnReleaseRes()
        {
            if (m_Asset != null)
            {
                GameObject.Destroy(m_Asset);
                m_Asset = null;
            }

            m_RawAsset = null;
        }

        public override void Recycle2Cache()
        {
            ObjectPool<NetImageRes>.S.Recycle(this);
        }

        public override void OnCacheReset()
        {

        }

        public void DeleteOldResFile()
        {
            //throw new NotImplementedException();
        }

        public void OnDownLoadResult(bool result)
        {
            if (!result)
            {
                OnResLoadFaild();
                return;
            }

            if (refCount <= 0)
            {
                resState = eResState.kWaiting;
                return;
            }

            ResMgr.S.PostIEnumeratorTask(this);
            //ResMgr.S.PostLoadTask(LoadImage());
        }

        //完全的WWW方式,Unity 帮助管理纹理缓存，并且效率貌似更高
        public override IEnumerator StartIEnumeratorTask(Action finishCallback)
        {
            if (refCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            WWW www = new WWW(m_Url);

            m_WWW = www;

            yield return www;

            m_WWW = null;

            if (www.error != null)
            {
                Log.e(string.Format("Res:{0}, WWW Errors:{1}", m_Url, www.error));
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            if (!www.isDone)
            {
                Log.e("NetImageRes WWW Not Done! Url:" + m_Url);
                OnResLoadFaild();
                finishCallback();

                www.Dispose();
                www = null;

                yield break;
            }

            if (refCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();

                www.Dispose();
                www = null;
                yield break;
            }

            //TimeDebugger dt = new TimeDebugger("Tex");
            //dt.Begin("LoadingTask");
            //这里是同步的操作
            m_Asset = www.texture;
            //dt.End();

            www.Dispose();
            www = null;

            //dt.Dump(-1);

            resState = eResState.kReady;

            finishCallback();
        }

        protected override float CalculateProgress()
        {
            if (m_WWW == null)
            {
                return 0;
            }

            return m_WWW.progress;
        }

        /*
        public IEnumerator StartIEnumeratorTask2(Action finishCallback)
        {
            if (refCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            WWW www = new WWW("file://" + LocalResPath);
            yield return www;
            if (www.error != null)
            {
                Log.e("WWW Error:" + www.error);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            if (!www.isDone)
            {
                Log.e("NetImageRes WWW Not Done! Url:" + m_Url);
                OnResLoadFaild();
                finishCallback();

                www.Dispose();
                www = null;

                yield break;
            }

            if (refCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();

                www.Dispose();
                www = null;
                yield break;
            }

            TimeDebugger dt = new TimeDebugger("Tex");
            dt.Begin("LoadingTask");
            Texture2D tex = www.texture;
            tex.Compress(true);
            dt.End();
            dt.Dump(-1);

            m_Asset = tex;
            www.Dispose();
            www = null;

            resState = eResState.kReady;

            finishCallback();
        }
        */
    }
}
