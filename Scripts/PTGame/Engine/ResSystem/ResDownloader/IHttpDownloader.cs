//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCEngine
{
    public delegate void OnDownloadFinished(string fileName, int download, int totalFileLenght);
    public delegate void OnDownloadError(string errorMsg);
    public delegate void OnDownloadProgress(int download, int totalFileLenght);
    public delegate void OnDownloadBegin(int totalLength);

    public interface IHttpDownloader
    {
        bool AddDownloadTask(string uri, string localPath, int fileSize, OnDownloadProgress onProgress, OnDownloadError onError, OnDownloadFinished onFinshed, OnDownloadBegin onBegin = null);
    }
}
