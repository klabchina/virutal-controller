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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Jigbox.Components
{
    /// <summary>
    /// 引数のタイプのプリセットに関する操作や処理を担う 
    /// </summary>
    public class AdvancedButtonTransitionPresetProviderEditor
    {
        protected readonly Type type;
        protected List<Object> AllPresetList;
        protected virtual string DefaultPropertyPath { get { return "isDefault"; } }
        protected virtual string LatestDefaultPresetKey { get { return "AdvancedButtonTransitionPresetProviderEditorDefault"; } }

        /// <summary>最後に記録した既定のプリセット。既定のプリセットが複製された場合にどれを優先するか決定するために使用する。</summary>
        protected virtual Object LatestDefaultPreset
        {
            get
            {
                var guid = EditorUserSettings.GetConfigValue(LatestDefaultPresetKey);
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), type); 
            }
            set
            {
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                EditorUserSettings.SetConfigValue(LatestDefaultPresetKey, guid);
            }
        }

        /// <summary>検索するプリセットが保存されているパス </summary>
        public static string DefaultPresetFolderPath = "Assets";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type">プリセットのタイプ</param>
        public AdvancedButtonTransitionPresetProviderEditor(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public virtual void Initialize()
        {
            AllPresetList = GetAll();
            LatestDefaultPreset = GetDefault();
        }

        /// <summary>
        /// プリセットを新規作成します
        /// </summary>
        public virtual Object Create(string path)
        {
            var preset = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(preset, path);
            AssetDatabase.Refresh();
            AllPresetList = GetAll();
            NormalizeDefaults(preset);
            return preset;
        }

        /// <summary>
        /// 既定のプリセットを設定します
        /// </summary>
        /// <remarks>
        /// 引数のプリセット以外は全て通常のプリセットとして再設定されます
        /// </remarks>
        public virtual void SetDefault(Object target)
        {
            if (target == null)
            {
                return;
            }
            var presets = GetAll();
            foreach (var preset in presets)
            {
                var isDefault = preset == target;
                var obj = new SerializedObject(preset);
                obj.FindProperty(DefaultPropertyPath).boolValue = isDefault;
                obj.ApplyModifiedProperties();
                if (isDefault)
                {
                    LatestDefaultPreset = preset;
                }
            }
        }

        /// <summary>
        /// 既定のプリセットを返します
        /// </summary>
        /// <returns>既定のプリセットを返します。既定のプリセットが一つではない場合、既定のプリセット設定を正常化して結果を返します。プリセット自体が一つも存在しない場合は null を返します</returns>
        public virtual Object GetDefault()
        {
            var defaults = GetDefaults();
            var valid = defaults.Count == 1;
            if (valid)
            {
                return defaults.First();
            }
            NormalizeDefaults(AllPresetList.FirstOrDefault());
            return GetDefaults().FirstOrDefault();
        }

        /// <summary>
        /// 既定のプリセットを複数返します
        /// </summary>
        protected virtual List<Object> GetDefaults()
        {
            var result = new List<Object>(AllPresetList);
            result.RemoveAll(x => !new SerializedObject(x).FindProperty(DefaultPropertyPath).boolValue);
            return result;
        }

        /// <summary>
        /// 既定のプリセット設定を正規化します
        /// </summary>
        /// <remarks>
        /// 既定プリセットの複製や削除、その他InspectorのDebug機能での値変更などを起因として
        /// 既定のプリセットが1つでない場合は1つに変更します
        /// </remarks>
        public virtual void NormalizeDefaults(Object defaultPresetIfNotFound)
        {
            var defaults = GetDefaults();
            // 既定のプリセットが複数ある場合（既定のプリセットを複製した場合などに起こり得る）
            if (defaults.Count > 1)
            {
                SetDefault(LatestDefaultPreset != null ? LatestDefaultPreset : defaults.First());
            }
            // 既定のプリセットが存在しない場合（既定のプリセットを削除した場合などに起こり得る）
            else if (defaults.Count == 0)
            {
                SetDefault(defaultPresetIfNotFound);
            }
        }

        /// <summary>
        /// プリセットを全て取得します
        /// </summary>
        protected virtual List<Object> GetAll()
        {
            var result = new List<Object>();
            var guids = AssetDatabase.FindAssets("t:" + type.Name, new string[]{DefaultPresetFolderPath});
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, type);
                if (asset != null)
                {
                    result.Add(asset);
                }
            }
            return result;
        }
    }
}
