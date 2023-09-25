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

namespace Jigbox.SceneTransition
{
    public class SceneTransitionTransientController
    {
#region properties

        /// <summary>シーン遷移時のトランジション制御コンポーネント</summary>
        protected BaseSceneTransitionController transitionController;

        /// <summary>シーンを戻るかどうか</summary>
        public bool IsBack { get; protected set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="transitionController">シーン遷移時のトランジション制御コンポーネント</param>
        /// <param name="isBack">シーンを戻るかどうか</param>
        public SceneTransitionTransientController(BaseSceneTransitionController transitionController, bool isBack = false)
        {
            this.transitionController = transitionController;
            IsBack = isBack;
        }

        /// <summary>
        /// シーン遷移のトランジションを開始します。
        /// </summary>
        public void BeginTransition()
        {
            SceneTransitionManager.Instance.Begin(transitionController);
        }

#endregion
    }
}
