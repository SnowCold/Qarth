using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class ShaderFinder
    {
        public static Shader FindShaderByFileName(string name)
        {
            Shader result = ResMgr.S.GetRes(name).asset as Shader;

            if (result == null)
            {
                Log.e("Not Find Shader:" + name);
            }

            if (!result.isSupported)
            {
                Log.e("Shader Not Support:" + name);
            }

            return result;
        }

        public static Shader FindShader(string name)
        {
            return ResHolder.S.FindShader(name);
        }

    }
}
