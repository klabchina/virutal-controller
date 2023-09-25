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

namespace Jigbox.Components
{
    /// <summary>
    /// AdvancedButtonTransition から通知されるイベントを実行する為のインターフェースです
    /// </summary>
    /// <remarks>
    /// このインターフェースを実装した MonoBehaviour をエフェクトのルートオブジェクトにアタッチすれば
    /// エフェクト側で通知に応じた処理をすることが可能になります
    /// AdvancedButtonTransitionEffectProvider を利用する場合に
    /// このインターフェースを実装することを想定しています
    /// IAdvancedButtonTransitionEffectProvider を実装して新規に Provider を作成する際は
    /// このインターフェースを利用することは任意の想定です
    /// </remarks>
    public interface IAdvancedButtonTransitionEffect
    {
        void OnTransition(AdvancedButtonTransition transition, InputEventType type);
        void OnStopTransition(AdvancedButtonTransition transition);
    }
}
