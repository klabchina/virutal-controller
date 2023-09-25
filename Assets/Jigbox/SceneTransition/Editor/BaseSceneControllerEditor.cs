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

namespace Jigbox.SceneTransition
{
    [CustomEditor(typeof(BaseSceneController), true)]
    public class BaseSceneControllerEditor : Editor
    {
#region properties

        /// <summary>シーンの制御コンポーネント</summary>
        protected BaseSceneController controller;

        /// <summary>ビルド設定に追加済みのシーン名</summary>
        protected string[] sceneNames;

        /// <summary>現在選択されているシーン名のインデックス</summary>
        protected int sceneIndex;

        /// <summary>シーン名のプロパティ</summary>
        protected SerializedProperty sceneName;

        /// <summary>シーンをアクティブ化しないようにするかどうか</summary>
        protected SerializedProperty isNotActivate;

#endregion

#region protected methods

        /// <summary>
        /// build設定に追加済みのシーンからシーン名の一覧を取得します。
        /// </summary>
        protected void GetScenes()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            sceneNames = new string[scenes.Length];
            int count = 0;
            foreach (EditorBuildSettingsScene scene in scenes)
            {
                int slashIndex = scene.path.LastIndexOf('/') + 1;
                int dotIndex = scene.path.LastIndexOf('.');
                sceneNames[count] = scene.path.Substring(slashIndex, dotIndex - slashIndex);
                if (sceneNames[count] == controller.SceneName)
                {
                    sceneIndex = count;
                }
                ++count;
            }
        }

        /// <summary>
        /// シリアライズ用プロパティを取得します。
        /// </summary>
        protected virtual void GetProperties()
        {
            sceneName = serializedObject.FindProperty("sceneName");
            if (string.IsNullOrEmpty(sceneName.stringValue))
            {
                UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
                serializedObject.Update();
                sceneName.stringValue = scene.name;
                serializedObject.ApplyModifiedProperties();
            }
            isNotActivate = serializedObject.FindProperty("isNotActivate");
        }

        /// <summary>
        /// エディタ表示を行います。
        /// </summary>
        protected virtual void DrawField()
        {
            sceneIndex = EditorGUILayout.Popup("シーン名", sceneIndex, sceneNames);
            sceneName.stringValue = sceneNames[sceneIndex];

            isNotActivate.boolValue = EditorGUILayout.Toggle("シーンをアクティブ化しない", isNotActivate.boolValue);
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            controller = target as BaseSceneController;
            GetProperties();
            GetScenes();
        }

        public override void OnInspectorGUI()
        {
            // 通常のインスペクター表記を行う
            DrawDefaultInspector();

            serializedObject.Update();

            DrawField();

            serializedObject.ApplyModifiedProperties();
        }

#endregion
    }
}
