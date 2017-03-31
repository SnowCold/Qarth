using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace PTGame.Framework
{
    public class TableExporter
    {
        private static bool IsLinuxSystem()
        {
            PlatformID platformID = System.Environment.OSVersion.Platform;

            if (platformID == PlatformID.MacOSX || platformID == PlatformID.Unix)
            {
                return true;
            }

            return false;
        }

        [MenuItem("Assets/SCEngine/Table/Build C#")]
        public static void BuildCSharpFile()
        {
            string path = ProjectPathConfig.GetProjectToolsFolderPath();
            if (IsLinuxSystem())
            {
                path += ProjectPathConfig.BUILD_CSHARP_LINUX_SHELL;
            }
            else
            {
                path += ProjectPathConfig.BUILD_CSHARP_WIN_SHELL;
            }

            Thread newThread = new Thread(new ThreadStart(() =>
            {
                BuildCSharpThreadStart(path);
            }));
            newThread.Start();
        }

        [MenuItem("Assets/SCEngine/Table/Build Data(txt)")]
        public static void BuildDataTxtMode()
        {
            string path = ProjectPathConfig.GetProjectToolsFolderPath();
            if (IsLinuxSystem())
            {
                path += ProjectPathConfig.BUILD_TXT_DATA_LINUX_SHELL;
            }
            else
            {
                path += ProjectPathConfig.BUILD_TXT_DATA_WIN_SHELL;
            }

            Thread newThread = new Thread(new ThreadStart(() =>
            {
                BuildCSharpThreadStart(path);
            }));
            newThread.Start();
        }

        [MenuItem("Assets/SCEngine/Table/Build Data(lrg)")]
        public static void BuildDataLrgMode()
        {
            string path = ProjectPathConfig.GetProjectToolsFolderPath();
            if (IsLinuxSystem())
            {
                path += ProjectPathConfig.BUILD_LRG_DATA_LINUX_SHELL;
            }
            else
            {
                path += ProjectPathConfig.BUILD_LRG_DATA_WIN_SHELL;
            }

            Thread newThread = new Thread(new ThreadStart(() =>
            {
                BuildCSharpThreadStart(path);
            }));
            newThread.Start();
        }

        private static void BuildCSharpThreadStart(string path)
        {
            if (IsLinuxSystem())
            {
                CommandThreadStartLinux(path);
            }
            else
            {
                CommandThreadStartWin(path);
            }

        }

        private static void CommandThreadStartLinux(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = "/bin/sh";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.Arguments = path + " arg1 arg2";

            process.Start();
            string outPutstr = process.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(outPutstr))
            {
                Log.i(outPutstr);
            }

            process.WaitForExit();
            process.Close();
        }

        private static void CommandThreadStartWin(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            //process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            string outPutstr = process.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(outPutstr))
            {
                Log.i(outPutstr);
            }

            process.WaitForExit();
            process.Close();
        }
    }
}
