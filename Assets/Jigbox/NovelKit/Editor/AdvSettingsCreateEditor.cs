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

namespace Jigbox.NovelKit
{
    public static class AdvSettingsCreateEditor
    {
#region private methods

        /// <summary>
        /// エンジンの設定用データを生成します。
        /// </summary>
        [MenuItem("Assets/Create/Jigbox/NovelKit/Engine Setting")]
        static void CreateEngineSetting()
        {
            ScriptableObject setting = ScriptableObject.CreateInstance<AdvEngineSetting>();
            AssetDatabase.CreateAsset(setting, GetDirectoryPath() + "/EngineSetting.asset");
            Selection.activeObject = setting;
        }

        /// <summary>
        /// プレーンの設定用データを生成します。
        /// </summary>
        [MenuItem("Assets/Create/Jigbox/NovelKit/Plane Setting")]
        static void CreatePlaneSetting()
        {
            ScriptableObject setting = ScriptableObject.CreateInstance<AdvPlaneSetting>();
            AssetDatabase.CreateAsset(setting, GetDirectoryPath() + "/PlaneSetting.asset");
            Selection.activeObject = setting;
        }

        /// <summary>
        /// オブジェクトの設定用データを生成します。
        /// </summary>
        [MenuItem("Assets/Create/Jigbox/NovelKit/Obejct Setting")]
        static void CreateObjectSetting()
        {
            ScriptableObject setting = ScriptableObject.CreateInstance<AdvObjectSetting>();
            AssetDatabase.CreateAsset(setting, GetDirectoryPath() + "/ObjectSetting.asset");
            Selection.activeObject = setting;
        }

        /// <summary>
        /// レイヤー設定用データを生成します。
        /// </summary>
        [MenuItem("Assets/Create/Jigbox/NovelKit/Layer Setting")]
        static void CreateLayerSetting()
        {
            ScriptableObject setting = ScriptableObject.CreateInstance<AdvLayerSetting>();
            AssetDatabase.CreateAsset(setting, GetDirectoryPath() + "/LayerSetting.asset");
            Selection.activeObject = setting;
        }

        /// <summary>
        /// Assets以下の現在選択されているディレクトリパスを返します。
        /// </summary>
        /// <returns></returns>
        static string GetDirectoryPath()
        {
            if (Selection.assetGUIDs.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            }
            else
            {
                return "Assets";
            }
        }

#endregion
    }
}
