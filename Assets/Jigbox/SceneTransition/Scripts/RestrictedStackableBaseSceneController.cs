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

namespace Jigbox.SceneTransition
{
    /// <summary>
    /// PassingDataの型を制約したStackableBaseSceneController
    /// </summary>
    public class RestrictedStackableBaseSceneController<T> : StackableBaseSceneController
    {
#region override methods

        protected sealed override void OnAwake(object passingData)
        {
            if (passingData != null)
            {
                if (passingData is T)
                {
                    OnAwake((T) passingData);
                }
                else
                {
#if UNITY_EDITOR
                    throw new FormatException("PassingData is not a constrained type.");
#endif
                }
            }
            else
            {
                OnAwake(default(T));
            }
        }

        /// <summary>
        /// シーンが生成されたタイミングで呼び出されます
        /// </summary>
        /// <param name="passingData">シーンの受け渡し用データ、存在しない場合はその型のdefault値となります</param>
        protected virtual void OnAwake(T passingData)
        {
        }
        
#endregion
    }
}
