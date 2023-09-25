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

using UnityEngine;

namespace Jigbox.NovelKit
{
    using ObjectType = AdvObjectBase.ObjectType;

    public sealed class AdvObjectSetting : ScriptableObject
    {
#region inner classes, enum, and structs

        /// <summary>
        /// オブジェクトの設定
        /// </summary>
        [System.Serializable]
        public class ObjectSetting
        {
            /// <summary>オブジェクトを生成する際のリソースのパス</summary>
            [SerializeField]
            string resourcePath = "AdvObject2D";

            /// <summary>オブジェクトを生成する際のリソースのパス</summary>
            public string ResourcePath { get { return resourcePath; } }

            /// <summary>表示切替時のトランジションの時間</summary>
            [SerializeField]
            float showTransitionTime = 0.15f;

            /// <summary>表示切替時のトランジションの時間</summary>
            public float ShowTransitionTime { get { return showTransitionTime >= 0.0f ? showTransitionTime : 0.0f; } }
        }

        /// <summary>
        /// オブジェクトの設定
        /// </summary>
        [System.Serializable]
        public class ObjectSettings
        {
            /// <summary>キャラクター</summary>
            [SerializeField]
            ObjectSetting character = null;

            /// <summary>キャラクター</summary>
            public ObjectSetting Character { get { return character; } }

            /// <summary>キャラクター以外の画像</summary>
            [SerializeField]
            ObjectSetting sprite = null;

            /// <summary>キャラクター以外の画像</summary>
            public ObjectSetting Sprite { get { return sprite; } }
            
            /// <summary>背景</summary>
            [SerializeField]
            ObjectSetting bg = null;

            /// <summary>背景</summary>
            public ObjectSetting Bg { get { return bg; } }

            /// <summary>CG(一枚絵)</summary>
            [SerializeField]
            ObjectSetting cg = null;

            /// <summary>CG(一枚絵)</summary>
            public ObjectSetting Cg { get { return cg; } }

            /// <summary>感情表現系エモーション</summary>
            [SerializeField]
            ObjectSetting emotional = null;

            /// <summary>感情表現系エモーション</summary>
            public ObjectSetting Emotional { get { return emotional; } }

            /// <summary>演出</summary>
            [SerializeField]
            ObjectSetting effect = null;

            /// <summary>演出</summary>
            public ObjectSetting Effect { get { return effect; } }

            /// <summary>その他</summary>
            [SerializeField]
            ObjectSetting other = null;

            /// <summary>その他</summary>
            public ObjectSetting Other { get { return other; } }

        }

#endregion

#region properties

        /// <summary>オブジェクトの設定</summary>
        [SerializeField]
        ObjectSettings main = null;
        
        /// <summary>差分用オブジェクトの設定</summary>
        [SerializeField]
        ObjectSettings sub = null;

        /// <summary>画面効果</summary>
        [SerializeField]
        ObjectSetting screenEffection = null;

        /// <summary>画面効果</summary>
        public ObjectSetting ScreenEffection { get { return screenEffection; } }

#endregion

#region public methods

        /// <summary>
        /// オブジェクトの種類に応じたオブジェクトの設定を取得します。
        /// </summary>
        /// <param name="type">オブジェクトの種類</param>
        /// <param name="isSub">差分用オブジェクトかどうか</param>
        /// <returns></returns>
        public ObjectSetting GetSetting(ObjectType type, bool isSub = false)
        {
            ObjectSettings settings = isSub ? sub : main;
            switch (type)
            {
                case ObjectType.Character:
                    return settings.Character;
                case ObjectType.Sprite:
                    return settings.Sprite;
                case ObjectType.Bg:
                    return settings.Bg;
                case ObjectType.Cg:
                    return settings.Cg;
                case ObjectType.Emotional:
                    return settings.Emotional;
                case ObjectType.Effect:
                    return settings.Effect;
                case ObjectType.Other:
                    return settings.Other;
            }
            return null;
        }

#endregion
    }
}
