//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PTGame.Framework
{
    public class NetImageView : MonoBehaviour
    {
        [SerializeField]
        private RawImage    m_Image;
        [SerializeField]
        private string      m_Url;
        [SerializeField]
        private bool        m_Refresh = false;
        [SerializeField]
        private Text        m_ProgressText;
        private IRes        m_Res;

        static string[] Urls =
        {
            //Big
            "http://a.hiphotos.baidu.com/zhidao/pic/item/342ac65c103853438b3c5f8b9613b07ecb8088ad.jpg",
            "http://g.hiphotos.baidu.com/zhidao/pic/item/a2cc7cd98d1001e93f2c4ef6be0e7bec54e79732.jpg",
            //"http://download.pchome.net/wallpaper/pic-4500-3-1024x768.jpg",
            "http://f.hiphotos.baidu.com/zhidao/pic/item/c9fcc3cec3fdfc03dfdfafcad23f8794a4c22618.jpg",
            "http://f.hiphotos.baidu.com/zhidao/pic/item/8b82b9014a90f60326b707453b12b31bb051eda9.jpg",
            "http://image.wangchao.net.cn/baike/1256954282280.jpg",
            "http://b.hiphotos.baidu.com/zhidao/pic/item/a5c27d1ed21b0ef47a3cc0a7dbc451da80cb3e76.jpg",
            //"http://pic40.nipic.com/20140414/18434316_222152084000_2.jpg",
            "http://g.hiphotos.baidu.com/zhidao/pic/item/f31fbe096b63f6249dc041108344ebf81a4ca300.jpg",

            "http://img.pconline.com.cn/images/upload/upc/tx/wallpaper/1304/28/c2/20341260_1367121265370.jpg",
            "http://pic33.nipic.com/20130927/12045420_085644346124_2.jpg",
            "http://imgb.mumayi.com/android/wallpaper/2012/02/02/sl_600_2012020211150937516337.jpg",
            "http://b.zol-img.com.cn/desk/bizhi/image/4/960x600/1390442719906.jpg",


            "http://c.hiphotos.baidu.com/zhidao/pic/item/b3fb43166d224f4a228770fd08f790529922d1c5.jpg",
            "http://pic1.win4000.com/wallpaper/c/5493ddddbd3f3.jpg",
            "http://imgb.mumayi.com/android/wallpaper/2011/11/02/sl_600_2011110202543659127818.jpg",

            "http://cdn.duitang.com/uploads/item/201407/13/20140713234126_yr2MV.jpeg",

            //girl
            "http://e.hiphotos.baidu.com/image/pic/item/c995d143ad4bd113ffe6dd3858afa40f4bfb0594.jpg",
            "http://a.hiphotos.baidu.com/image/pic/item/d439b6003af33a8709414e1ac45c10385343b586.jpg",
            "http://a.hiphotos.baidu.com/image/pic/item/4d086e061d950a7b4afaacdb08d162d9f3d3c9e6.jpg",
            "http://e.hiphotos.baidu.com/image/pic/item/e4dde71190ef76c64baa532b9f16fdfaaf5167b2.jpg",
            "http://h.hiphotos.baidu.com/image/pic/item/aec379310a55b3198cba257541a98226cffc176a.jpg",
            "http://g.hiphotos.baidu.com/image/pic/item/9f2f070828381f30cfbf4a2cab014c086f06f095.jpg",
            "http://e.hiphotos.baidu.com/image/pic/item/c2fdfc039245d688e89d17a4a6c27d1ed21b2416.jpg",
            "http://h.hiphotos.baidu.com/image/pic/item/faf2b2119313b07e7506c0080ed7912397dd8c73.jpg",

            //"http://t-1.tuzhan.com/ad7b4386f7d2/c-2/l/2013/09/27/06/1cf05d93e058443da5988bfee04030ea.jpg",
            //"http://t-1.tuzhan.com/9a88b6a5a1a2/c-2/l/2013/09/17/07/28cebe7fc454495c871ea8c2f10dbecd.jpg",
            "http://h6.86.cc/walls/20141114/mid_dbe47c9b3f0e76b.jpg",
            "http://images.ali213.net/picfile/pic/2012-02-29/927_D-5.jpg",
            //"http://t-1.tuzhan.com/b7d45215b8b3/c-2/l/2013/07/28/00/9746dc98566942f09f37ccfe74629248.jpg",
            //"http://t-1.tuzhan.com/29a174386b62/c-2/l/2013/09/25/05/e419540a0e304359b846dd354a5c7b9c.jpg",
            "http://pic2016.5442.com:82/2016/0304/8/1.jpg!960.jpg",
            "http://img.51ztzj.com/upload/image/20151113/1446619521550_670x419.jpg",
            "http://img.51ztzj.com/upload/image/20140212/dn201402122011_670x419.jpg",
            "http://www.danji8.com/uploadpic/201212/06/2012120613394580531354772385.jpg",

            //Small
            "http://c11.eoemarket.com/app0/96/96040/icon/614849.png",
            "http://img.zcool.cn/community/01237d554bb5b9000001bf7243befd.jpg",
            "http://img.zcool.cn/community/0188b6554bb5b8000001bf72767a9e.jpg",
            "http://img.zcool.cn/community/01d59c554bb5b9000001bf724aeeee.jpg",
            "http://img.zcool.cn/community/01b552554bb5b9000001bf7297d31b.jpg",
            "http://img.zcool.cn/community/0147e6554bb5b9000001bf7291cc0c.jpg",

            "http://p6.qhimg.com/t01bca79f6827cd36f0.png",
            "http://imgsrc.baidu.com/forum/w%3D580/sign=b2877f19b2de9c82a665f9875c8080d2/97f55110b912c8fce3116dedfd039245d78821aa.jpg"
        };

        private void Awake()
        {
            if (m_Image == null)
            {
                m_Image = GetComponent<RawImage>();
            }

            RequestUpdateImage();
        }

        private void OnDestroy()
        {
            if (m_Res != null)
            {
                m_Res.UnRegisteResListener(OnResLoadFinish);
                m_Res.SubRef();
                m_Res = null;
            }
        }

        void Update()
        {
            if (m_ProgressText == null)
            {
                return;
            }

            if (m_Res == null)
            {
                return;
            }

            if (m_Res.progress <= 1)
            {
                m_ProgressText.text = m_Res.progress.ToString();
            }
        }

        public void ScrollCellIndex(int idx)
        {
            idx = Mathf.Abs(idx);
            m_Url = Urls[(idx % Urls.Length)];
            RequestUpdateImage();
        }

        private void RequestUpdateImage()
        {
            if (m_Res != null)
            {
                m_Res.UnRegisteResListener(OnResLoadFinish);
                m_Res.SubRef();
                m_Res = null;
            }

            if (m_Image != null)
            {
                m_Image.enabled = false;
            }

            if (string.IsNullOrEmpty(m_Url))
            {
                return;
            }

            m_Res = ResMgr.S.GetRes(string.Format("NetImage:{0}", m_Url), true);
            m_Res.AddRef();

            m_Res.RegisteResListener(OnResLoadFinish);
            m_Res.LoadAsync();
        }

        private void OnResLoadFinish(bool result, IRes res)
        {
            if (m_Image == null)
            {
                return;
            }

            if (result)
            {
                m_Image.enabled = true;
                m_Image.texture = res.asset as Texture;
            }
        }

        void OnValidate()
        {
            if (m_Refresh)
            {
                m_Refresh = false;
                RequestUpdateImage();
            }
        }
    }
}
