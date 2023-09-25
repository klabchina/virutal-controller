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
    public interface ISceneUnloadHandler
    {
        /// <summary>
        /// <para>現在のシーンがシーン遷移によって、アンロードされようとした際に呼び出されます。</para>
        /// <para>アンロードのために必要な処理が完了したら、引数に渡されたモジュールを使用して、</para>
        /// <para>シーンの遷移を開始して下さい。</para>
        /// </summary>
        /// <param name="transientController">トランジションの一時的な制御モジュール</param>
        void OnWillUnload(SceneTransitionTransientController transientController);
    }
}
