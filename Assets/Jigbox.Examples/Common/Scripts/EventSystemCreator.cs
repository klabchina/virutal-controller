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
using Jigbox.SceneTransition;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Jigbox.Examples
{
    /// <summary>
    /// SceneにEventSystemを配置するクラス
    /// </summary>
    public class EventSystemCreator : MonoBehaviour
    {
        [SerializeField] 
        bool isInputSystem = false;
        
        GameObject eventSystem = null;

        string sceneName = "";

#region override unity methods

        protected void Awake()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;

            sceneName = StackableSceneManager.Instance.GetSceneNameOfStackTop();
            string activeSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (this.sceneName == String.Empty || activeSceneName == "Index")
            {
                sceneName = activeSceneName;
            }
        }

        void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            if (eventSystem)
            {
                Destroy(eventSystem);
                eventSystem = null;
            }
        }

        void OnActiveSceneChanged(Scene beforeScene, Scene nextScene)
        {
            if (sceneName == nextScene.name)
            {
                var indexSceneData = StackableSceneManager.Instance.GetPassingData("IndexInputSystem");
                if (indexSceneData != null)
                {
                    isInputSystem = (bool)indexSceneData;
                }
                InputWrapper.IsInputSystem = isInputSystem;

                var eventSystemClassList = FindObjectsOfType<EventSystem>();

                foreach (var eventSystemClass in eventSystemClassList)
                {
                    Destroy(eventSystemClass.gameObject);
                }

                eventSystem = new GameObject("JigboxEventSystem");
                eventSystem.transform.parent = transform;
                eventSystem.AddComponent<EventSystem>();

                if (InputWrapper.IsInputSystem)
                {
#if ENABLE_INPUT_SYSTEM
                    var inputSystem = eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                    inputSystem.AssignDefaultActions();
#endif
                }
                else
                {
                    eventSystem.AddComponent<StandaloneInputModule>();
                }
            }
        }
#endregion
    }
}
