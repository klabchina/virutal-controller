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
using UnityEditor;
using System.Collections.Generic;

namespace Jigbox.AtlasPacker
{
    public class AtlasCustomInfo : ScriptableObject
    {
#region properties

        /// <summary>画像のPaddingの設定</summary>
        [SerializeField]
        int padding = 0;

        /// <summary>画像のPaddingの設定</summary>
        public int Padding { get { return Mathf.Max(0, padding); } set { padding = value; } }

        /// <summary>色を拡張したスプライトのリスト</summary>
        [SerializeField]
        List<string> expandedSprites = new List<string>();

#endregion

#region public methods

        /// <summary>
        /// 設定ファイルを作成します。
        /// </summary>
        /// <param name="atlasName">Atlas名</param>
        /// <param name="directoryPath">ディレクトリのパス</param>
        /// <returns></returns>
        public static AtlasCustomInfo Create(string atlasName, string directoryPath)
        {
            AtlasCustomInfo info = ScriptableObject.CreateInstance<AtlasCustomInfo>();
            string assetPath = directoryPath + "/" + atlasName + "_info.asset";
            AssetDatabase.CreateAsset(info, assetPath);
            return info;
        }

        /// <summary>
        /// 対象Atlasの設定ファイルを読み込みます。
        /// </summary>
        /// <param name="atlas">Atlasとして扱うテクスチャの参照</param>
        /// <returns></returns>
        public static AtlasCustomInfo Load(Texture2D atlas)
        {
            string assetPath = AssetDatabase.GetAssetPath(atlas);
            int index = assetPath.LastIndexOf("/");
            string directoryPath = assetPath.Substring(0, index);

            return AssetDatabase.LoadAssetAtPath(directoryPath + "/" + atlas.name + "_info.asset", typeof(AtlasCustomInfo)) as AtlasCustomInfo;
        }

        /// <summary>
        /// 色を拡張したスプライトのリストを更新します。
        /// </summary>
        /// <param name="sprites">スプライトのリスト</param>
        public void UpdateExpandedSprites(List<string> sprites)
        {
            expandedSprites = sprites;
        }

        /// <summary>
        /// 指定されたスプライトの色が拡張されているかどうかを返します。
        /// </summary>
        /// <param name="spriteName">スプライト名</param>
        /// <returns></returns>
        public bool IsExpanded(string spriteName)
        {
            for (int i = 0; i < expandedSprites.Count; ++i)
            {
                if (expandedSprites[i] == spriteName)
                {
                    return true;
                }
            }

            return false;
        }

#endregion
    }
}
