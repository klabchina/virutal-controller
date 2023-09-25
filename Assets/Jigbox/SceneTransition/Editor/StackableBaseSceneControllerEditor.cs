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

using UnityEditor;
using System.Collections.Generic;

namespace Jigbox.SceneTransition
{
    using StackInfo = StackableSceneManager.StackInfo;

    [CustomEditor(typeof(StackableBaseSceneController), true)]
    public class StackableBaseSceneControllerEditor : BaseSceneControllerEditor
    {
#region properties

        /// <summary>スタック可能シーンの制御コンポーネント</summary>
        protected StackableBaseSceneController StackableController { get { return controller as StackableBaseSceneController; } }

        /// <summary>戻り先シーン名のプロパティ</summary>
        protected SerializedProperty backTargetScene;

        /// <summary>自動的にスタックから取り除くかどうかのプロパティ</summary>
        protected SerializedProperty autoRemoveStack;

        /// <summary>戻り先として設定可能シーン名</summary>
        protected string[] backSceneNames;

        /// <summary>戻り先として選択されているシーン名のインデックス</summary>
        protected int backSceneIndex = 0;

#endregion
        
#region protected methods
        
        /// <summary>
        /// 戻り先のシーン名を取得します。
        /// </summary>
        protected void GetBackScenes()
        {
            backSceneNames = new string[sceneNames.Length + 1];
            // 空文字列だと勝手にプルダウンから隠される
            backSceneNames[0] = " ";
            for (int i = 1; i < backSceneNames.Length; ++i)
            {
                backSceneNames[i] = sceneNames[i - 1];
                if (backSceneNames[i] == backTargetScene.stringValue)
                {
                    backSceneIndex = i;
                }
            }
        }

        /// <summary>
        /// シリアライズ用プロパティを取得します。
        /// </summary>
        protected override void GetProperties()
        {
            base.GetProperties();
            backTargetScene = serializedObject.FindProperty("backTargetSceneName");
            autoRemoveStack = serializedObject.FindProperty("autoRemoveStack");
        }

        /// <summary>
        /// エディタ表示を行います。
        /// </summary>
        protected override void DrawField()
        {
            base.DrawField();

            backSceneIndex = EditorGUILayout.Popup("戻り先のシーン名", backSceneIndex, backSceneNames);
            backTargetScene.stringValue = backSceneIndex == 0 ? string.Empty : backSceneNames[backSceneIndex];
            autoRemoveStack.boolValue = EditorGUILayout.Toggle("シーンをスタックしない", autoRemoveStack.boolValue);

            if (!EditorApplication.isPlaying)
            {
                return;
            }

            EditorGUILayout.BeginVertical(UnityEngine.GUI.skin.box);
            {
                EditorGUILayout.LabelField("シーンのスタック状態(新→旧)");
                List<StackInfo> sceneStack = StackableSceneManager.Instance.GetStackList();
                for (int i = sceneStack.Count - 1; i >= 0; --i)
                {
                    EditorGUILayout.LabelField(sceneStack[i].SceneName);
                }
            }
            EditorGUILayout.EndVertical();
        }

#endregion
        
#region override unity methods

        protected override void OnEnable()
        {
            controller = target as BaseSceneController;
            GetProperties();
            GetScenes();
            GetBackScenes();
        }

#endregion
    }
}
