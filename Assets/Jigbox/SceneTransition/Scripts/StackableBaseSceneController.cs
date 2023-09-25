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

namespace Jigbox.SceneTransition
{
    public class StackableBaseSceneController : BaseSceneController
    {
#region properties

        /// <summary>戻り先のシーン名</summary>
        [HideInInspector]
        [SerializeField]
        protected string backTargetSceneName = string.Empty;

        /// <summary>このシーンが破棄される際に自動的にスタックから取り除くかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool autoRemoveStack = false;

        /// <summary>このシーンが破棄される際に自動的にスタックから取り除くかどうか</summary>
        public bool AutoRemoveStack { get { return autoRemoveStack; } set { autoRemoveStack = value; } }

#endregion

#region protected methods

        /// <summary>
        /// <para>エスケープキー(Androidのバックキー)が押された際に呼び出されます。</para>
        /// <para>モーダルシーンとして開いている場合、このメソッドは呼び出されません。</para>
        /// </summary>
        protected override void OnEscape()
        {
            BackScene();
        }
        
        /// <summary>
        /// シーンを戻ります。
        /// </summary>
        protected virtual void BackScene()
        {
            // シーンを戻る場合は自動でスタックが破棄されるので自動破棄はしない
            autoRemoveStack = false;
            StackableSceneManager.Instance.BackScene(backTargetSceneName);
        }

#endregion

#region override unity methods

        void Awake()
        {
            StackableSceneManager.Instance.RegisterScene(SceneName, this);

#pragma warning disable 219
            // インスタンスが必ず生成されるように参照を行う
            BackKeyManager instance = BackKeyManager.Instance;
#pragma warning restore 219
            
            OnAwake(StackableSceneManager.Instance.GetPassingData(SceneName));
        }
        
        void OnDestroy()
        {
            OnDestroyed();

            StackableSceneManager.Instance.UnregisterScene(SceneName);
            if (autoRemoveStack)
            {
                StackableSceneManager.Instance.AutoRemoveSceneStack(SceneName);
            }
        }

#endregion
    }
}
