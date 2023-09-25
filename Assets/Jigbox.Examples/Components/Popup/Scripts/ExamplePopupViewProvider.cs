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
using Jigbox.Components;
using Jigbox.SceneTransition;

namespace Jigbox.Examples
{
    public sealed class ExamplePopupViewProvider : IInstanceProvider<PopupView>
    {
#region public methods

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <returns></returns>
        public PopupView Generate()
        {
            GameObject obj = GameObject.Instantiate(Resources.Load("PopupView")) as GameObject;
            // Modalシーンを使う場合は、SceneControllerを非アクティブにすることで、
            // それ以下にあるオブジェクトも非表示になる状態を作るので、PopupViewもSceneController以下に置く
            string sceneName = SceneManager.Instance.CurrentModalSceneName;
            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = SceneManager.Instance.CurrentSceneName;
                if (string.IsNullOrEmpty(sceneName))
                {
                    sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                }
            }
            if (!string.IsNullOrEmpty(sceneName))
            {
                BaseSceneController sceneController = SceneManager.Instance.GetSceneController(sceneName);
                obj.transform.SetParent(sceneController.transform, false);
            }
            return obj.GetComponent<PopupView>();
        }

#endregion
    }
}
