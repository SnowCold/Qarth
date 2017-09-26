//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Qarth
{
    public class ResHolder : TSingleton<ResHolder>
    {
        protected ResLoader m_Loader;
        private Dictionary<string, Shader> m_ShaderMap;

        public override void OnSingletonInit()
        {
            m_Loader = ResLoader.Allocate("ResHolder");
        }

        public ResLoader loader
        {
            get { return m_Loader; }
        }

        public void AddRes(string res)
        {
            m_Loader.Add2Load(res);
        }

        public Shader FindShader(string name)
        {
            if (m_ShaderMap != null)
            {
                Shader result = null;

                if (m_ShaderMap.TryGetValue(name, out result))
                {
                    return result;
                }
            }

            return Shader.Find(name);
        }

        public void LoadAllShader(string[] shaders)
        {
            if (shaders == null || shaders.Length == 0)
            {
                return;
            }

            for (int i = 0; i < shaders.Length; ++i)
            {
                m_Loader.Add2Load(shaders[i], OnShaderLoadFinish);
            }

            m_Loader.LoadAsync();
        }

        private void OnShaderLoadFinish(bool result, IRes res)
        {
            if (!result)
            {
                return;
            }

            Shader shader = res.asset as Shader;
            if (shader == null)
            {
                return;
            }

            AddShader(shader);
        }

        private void AddShader(Shader shader)
        {
            if (m_ShaderMap == null)
            {
                m_ShaderMap = new Dictionary<string, Shader>();
            }

            if (m_ShaderMap.ContainsKey(shader.name))
            {
                Log.w("Already all Shader:" + shader.name);
                return;
            }

            m_ShaderMap.Add(shader.name, shader);
        }

    }
}
