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
using Jigbox.Tween;

namespace Jigbox.Shake
{
    public class SpringShakeSingle : SpringShake<float, SpringShakeSingle>
    {
#region constructors

        /// <summary>コンストラクター</summary>
        public SpringShakeSingle() {}

        /// <summary><c>OnUpdate</c>イベントハンドラーを登録するコンストラクター</summary>
        /// <param name="onUpdate">イベントハンドラー</param>
        public SpringShakeSingle(Action<SpringShakeSingle> onUpdate) : base(onUpdate) {}

        /// <summary>コピーコンストラクター</summary>
        /// <param name="other">コピー元のインスタンス</param>
        public SpringShakeSingle(SpringShakeSingle other) : base(other) {}

        /// <summary><c>OnUpdate</c>イベントハンドラーを登録するコピーコンストラクター</summary>
        /// <param name="other">コピー元のインスタンス</param>
        /// <param name="onUpdate">イベントハンドラー</param>
        public SpringShakeSingle(SpringShakeSingle other, Action<SpringShakeSingle> onUpdate) : base(other, onUpdate) {}

#endregion

        /// <summary>
        /// 動作時間における値を返します。
        /// </summary>
        /// <param name="time">0～Durationまでの時間</param>
        /// <returns>値</returns>
        public override float ValueAt(float time)
        {
            return Origin + CalculateWave(time);
        }

        /// <summary>Mono 2.x でビルドするときにコンパイラーに情報を与えるためのコードで、このメソッドは呼ばれません。</summary>
        static void Mono2xFullAotWorkaround()
        {
            new SpringShakeBehaviour<float, SpringShakeSingle>(null).Init();
        }
    }
}
