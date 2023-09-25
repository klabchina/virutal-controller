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
using UnityEngine.SceneManagement;

namespace Jigbox.Examples
{
    public class Restarter : MonoBehaviour
    {
        static GameObject instance;

        bool isRequestLoadIndex = false;

        public static void EnsureActivation()
        {
            if (instance != null)
            {
                return;
            }
            instance = new GameObject("Restarter");
            DontDestroyOnLoad(instance);
            instance.AddComponent<Restarter>();
        }

        void Update()
        {
            if (InputWrapper.GetTouchCount() > 2)
            {
                if (InputWrapper.IsTouchPhase(2, TouchPhase.Ended))
                {
                    Scene current = SceneManager.GetActiveScene();
                    if (current.name != "Index")
                    {
                        isRequestLoadIndex = true;
                    }
                }
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            bool isBack = false;
#if ENABLE_INPUT_SYSTEM
            if (InputWrapper.IsTouchSimulate())
            {
                // TouchSimulation中は右クリックがとれないのでLeftAltで戻る
                isBack = InputWrapper.GetKeyUp(KeyCode.LeftAlt);
            }
            else
#endif
            {
                isBack = InputWrapper.GetMouseButtonUp(1);
            }

            if (isBack)
            {
                Scene current = SceneManager.GetActiveScene();
                if (current.name != "Index")
                {
                    isRequestLoadIndex = true;
                }
            }
#endif
            if (isRequestLoadIndex)
            {
                LoadIndexScene();
            }
        }

        void LoadIndexScene()
        {
            if (!SceneTransition.SceneTransitionManager.Instance.IsTransition)
            {
                isRequestLoadIndex = false;
                SceneManager.LoadScene(0);
            }
        }

        void OnDestroy()
        {
            if (instance == null)
            {
                return;
            }
            instance = null;
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
