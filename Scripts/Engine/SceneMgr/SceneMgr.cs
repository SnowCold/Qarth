//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Qarth
{
    [TMonoSingletonAttribute("[Tools]/SceneMgr")]
    public class SceneMgr : TMonoSingleton<SceneMgr>
    {
        private ResLoader m_CurrentLoader;

        public bool LoadBuildInSceneSync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (m_CurrentLoader != null)
            {
                m_CurrentLoader.ReleaseAllRes();
                m_CurrentLoader.Recycle2Cache();
                m_CurrentLoader = null;
            }

            try
            {
                SceneManager.LoadScene(sceneName, mode);
            }
            catch (Exception e)
            {
                Log.e("SceneManager LoadBuildInSceneSysn Failed! SceneName:" + sceneName);
                Log.e(e);
                return false;
            }

            return true;
        }

        public void LoadBuildInSceneAsync(string sceneName, Action<string, bool> loadCallback = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (m_CurrentLoader != null)
            {
                m_CurrentLoader.ReleaseAllRes();
                m_CurrentLoader.Recycle2Cache();
                m_CurrentLoader = null;
            }

            StartCoroutine(LoadSceneAsync(sceneName, loadCallback, mode, false));
        }

        public bool LoadABSceneFromSync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            ResLoader nextLoader = ResLoader.Allocate("SceneMgr");

            //提前加入可以起到缓存已经加载的资源的作用，防止释放后又加载的重复动作
            if (!AddSceneAB2Loader(sceneName, nextLoader))
            {
                return false;
            }

            if (m_CurrentLoader != null)
            {
                m_CurrentLoader.ReleaseAllRes();
                m_CurrentLoader.Recycle2Cache();
                m_CurrentLoader = null;
            }

            m_CurrentLoader = nextLoader;

            m_CurrentLoader.LoadSync();

            try
            {
                SceneManager.LoadScene(sceneName, mode);
            }
            catch (Exception e)
            {
                Log.e("SceneManager LoadABSceneFromSync Failed! SceneName:" + sceneName);
                Log.e(e);
                UnloadSceneAssetBundle(sceneName);
                return false;
            }

            UnloadSceneAssetBundle(sceneName);
            return true;
        }

        public void LoadABSceneAsync(string sceneName, Action<string, bool> loadCallback = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            ResLoader nextLoader = ResLoader.Allocate("SceneMgr");

            //可以起到缓存已经加载的资源的作用，防止释放后又加载的重复动作
            if (!AddSceneAB2Loader(sceneName, nextLoader))
            {
                if (loadCallback != null)
                {
                    loadCallback(sceneName, false);
                }
                return;
            }

            if (m_CurrentLoader != null)
            {
                m_CurrentLoader.ReleaseAllRes();
                m_CurrentLoader.Recycle2Cache();
                m_CurrentLoader = null;
            }

            m_CurrentLoader = nextLoader;

            m_CurrentLoader.LoadAsync(() =>
            {
                StartCoroutine(LoadSceneAsync(sceneName, loadCallback, mode, true));
            });
        }

        private IEnumerator LoadSceneAsync(string sceneName, Action<string, bool> loadCallback, LoadSceneMode mode, bool abMode)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
            yield return op;

            if (!op.isDone)
            {
                if (abMode)
                {
                    Log.e("SceneManager LoadABSceneAsync Not Done! SceneName:" + sceneName);
                    UnloadSceneAssetBundle(sceneName);
                }
                else
                {
                    Log.e("SceneManager LoadBuindInSceneAsync Not Done! SceneName:" + sceneName);
                }

                if (loadCallback != null)
                {
                    loadCallback(sceneName, false);
                }
                yield break;
            }

            if (abMode)
            {
                UnloadSceneAssetBundle(sceneName);
            }

            if (loadCallback != null)
            {
                loadCallback(sceneName, true);
            }
        }

        private void UnloadSceneAssetBundle(string sceneName)
        {
            string abName = GetSceneAssetBundleName(sceneName);
            if (string.IsNullOrEmpty(abName))
            {
                return;
            }
            m_CurrentLoader.ReleaseRes(abName);
        }

        private bool AddSceneAB2Loader(string sceneName, ResLoader loader)
        {
            string abName = GetSceneAssetBundleName(sceneName);

            if(string.IsNullOrEmpty(abName))
            {
                return false;
            }

            loader.Add2Load(abName);

            return true;
        }

        private string GetSceneAssetBundleName(string sceneName)
        {
            AssetData config = AssetDataTable.S.GetAssetData(sceneName);

            if (config == null)
            {
                Log.e("Not Find AssetData For Asset:" + sceneName);
                return string.Empty;
            }

            string assetBundleName = AssetDataTable.S.GetAssetBundleName(config.assetName, config.assetBundleIndex);

            if (string.IsNullOrEmpty(assetBundleName))
            {
                Log.e("Not Find AssetBundle In Config:" + config.assetBundleIndex);
                return string.Empty;
            }

            return assetBundleName;
        }
    }
}
