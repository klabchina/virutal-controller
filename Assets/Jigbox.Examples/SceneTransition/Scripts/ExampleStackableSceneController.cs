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
using UnityEngine.UI;
using Jigbox.SceneTransition;

namespace Jigbox.Examples
{
    public class ExampleStackableSceneController : StackableBaseSceneController
    {
#region properties

        [SerializeField]
        Text sceneNameLable = null;

        [SerializeField]
        GameObject noneStackMessage = null;

        [SerializeField]
        string stackData = "";
        
        static GameObject eventSystem;

#endregion
        
#region protected methods

        protected override void OnAwake(object passingData)
        {
            sceneNameLable.text = SceneName;
            noneStackMessage.SetActive(autoRemoveStack);

            SceneTransitionManager.Instance.DefaultTransitionPrefabPath = "ExsampleFadeTransition";

            if (passingData != null)
            {
                Debug.Log(SceneName + ":" + passingData.ToString());
            }
        }

        [AuthorizedAccess]
        protected void OnClickBack()
        {
            StackableSceneManager.Instance.BackScene(backTargetSceneName);
        }

        [AuthorizedAccess]
        protected void OnClickBackLast()
        {
            StackableSceneManager.Instance.BackLastScene(backTargetSceneName);
        }

        [AuthorizedAccess]
        protected void OnClickTest1()
        {
            StackableSceneManager.Instance.LoadScene("SceneTransitionStackable1");
            if (!string.IsNullOrEmpty(stackData))
            {
                StackableSceneManager.Instance.PushSceneStack("SceneTransitionStackable1", stackData);
            }
        }

        [AuthorizedAccess]
        protected void OnClickTest2()
        {
            StackableSceneManager.Instance.LoadScene("SceneTransitionStackable2");
            if (!string.IsNullOrEmpty(stackData))
            {
                StackableSceneManager.Instance.PushSceneStack("SceneTransitionStackable2", stackData);
            }
            // 遷移先は同じだが、遷移処理を上書きした場合の動作を確認するため
            SceneTransitionManager.Instance.OverrideLoadAction("SceneTransitionStackable2");
        }

        [AuthorizedAccess]
        protected void OnClickTest3()
        {
            StackableSceneManager.Instance.LoadScene("SceneTransitionStackable3");
            if (!string.IsNullOrEmpty(stackData))
            {
                StackableSceneManager.Instance.PushSceneStack("SceneTransitionStackable3", stackData);
            }
        }

        [AuthorizedAccess]
        protected void OnClickTest4()
        {
            StackableSceneManager.Instance.LoadScene("SceneTransitionStackable4");
            if (!string.IsNullOrEmpty(stackData))
            {
                StackableSceneManager.Instance.PushSceneStack("SceneTransitionStackable4", stackData);
            }
        }

        [AuthorizedAccess]
        protected void OnClickModal3()
        {
            if (StackableSceneManager.Instance.GetSceneNames().Contains("SceneTransitionStackable3"))
            {
                return;
            }

            StackableSceneManager.Instance.OpenModalScene("SceneTransitionStackable3");
            if (!string.IsNullOrEmpty(stackData))
            {
                StackableSceneManager.Instance.PushSceneStack("SceneTransitionStackable3", stackData);
            }
        }

        [AuthorizedAccess]
        protected void OnClickModal4()
        {
            if (StackableSceneManager.Instance.GetSceneNames().Contains("SceneTransitionStackable4"))
            {
                return;
            }

            StackableSceneManager.Instance.OpenModalScene("SceneTransitionStackable4");
            if (!string.IsNullOrEmpty(stackData))
            {
                StackableSceneManager.Instance.PushSceneStack("SceneTransitionStackable4", stackData);
            }
        }

        [AuthorizedAccess]
        protected void OnClickClose()
        {
            StackableSceneManager.Instance.CloseModalScene();
        }

        [AuthorizedAccess]
        protected void OnClickCloseAll()
        {
            StackableSceneManager.Instance.CloseModalSceneAll();
        }

#endregion
    }
}
