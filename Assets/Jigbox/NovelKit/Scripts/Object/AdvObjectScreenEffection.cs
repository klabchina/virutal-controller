/**
 * Jigbox
 * Copyright(c) 2016 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 *
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications 
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */

using Jigbox.Tween;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.NovelKit
{
    public class AdvObjectScreenEffection : AdvObject2DBase
    {
#region properties

        /// <summary>オフスクリーンレンダリング対象のテクスチャ</summary>
        [SerializeField]
        protected RawImage rawImage;

        /// <summary>オフスクリーンレンダリング対象のテクスチャ</summary>
        public RawImage RawImage { get { return rawImage; } }

        /// <summary>画像の色</summary>
        public override Color Color
        {
            get
            {
                return rawImage.color;
            }
            set
            {
                Color color = value;
                color.a = rawImage.color.a;
                rawImage.color = color;
            }
        }

        /// <summary>アルファ値</summary>
        protected override float Alpha
        {
            get
            {
                return rawImage.color.a;
            }
            set
            {
                if (rawImage.color.a != value)
                {
                    Color color = rawImage.color;
                    color.a = value;
                    rawImage.color = color;
                }
            }
        }

        /// <summary>表示順を決めるためのDepth値(使用されていません)</summary>
        public override int Depth { get { return 0; } set { } }

        /// <summary>Material</summary>
        public override Material Material { get { return rawImage.material; } set { rawImage.material = value; } }

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="id">管理用ID(オブジェクトの種類毎)</param>
        /// <param name="type">オブジェクトの種類</param>
        /// <param name="setting">オブジェクトの基礎設定</param>
        public override void Init(int id, ObjectType type, AdvObjectSetting.ObjectSetting setting)
        {
            Id = id;
            Type = type;
            ShowTransitionTime = setting.ShowTransitionTime;
            InitColor();
        }

        /// <summary>
        /// リソースを読み込みます。(使用されていません)
        /// </summary>
        /// <param name="loader">Loader</param>
        /// <param name="resourcePath">リソースのパス</param>
        public override void LoadResource(IAdvResourceLoader loader, string resourcePath)
        {
        }

#endregion
    }
}
