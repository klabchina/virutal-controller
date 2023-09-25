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
using System;

namespace Jigbox.Tween
{
    public class TweenColorOrder : TweenOrder
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 色用クラス
        /// </summary>
        protected class ColorUpdateCallback
        {
            /// <summary>Graphic</summary>
            protected Graphic graphic;

            /// <summary>R成分を更新するかどうか</summary>
            protected bool updateR;

            /// <summary>G成分を更新するかどうか</summary>
            protected bool updateG;

            /// <summary>B成分を更新するかどうか</summary>
            protected bool updateB;

            /// <summary>A成分を更新するかどうか</summary>
            protected bool updateA;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="graphic">Graphic</param>
            /// <param name="updateR">R成分を更新するかどうか</param>
            /// <param name="updateG">G成分を更新するかどうか</param>
            /// <param name="updateB">B成分を更新するかどうか</param>
            /// <param name="updateA">A成分を更新するかどうか</param>
            public ColorUpdateCallback(Graphic graphic, bool updateR, bool updateG, bool updateB, bool updateA)
            {
                this.graphic = graphic;
                this.updateR = updateR;
                this.updateG = updateG;
                this.updateB = updateB;
                this.updateA = updateA;
            }

            /// <summary>
            /// 色を更新します。
            /// </summary>
            /// <param name="tween">Tween</param>
            public void OnUpdate(ITween<Color> tween)
            {
                Color value = tween.Value;
                Color color = Color.white;

                color.r = updateR ? value.r : graphic.color.r;
                color.g = updateG ? value.g : graphic.color.g;
                color.b = updateB ? value.b : graphic.color.b;
                color.a = updateA ? value.a : graphic.color.a;

                graphic.color = color;
            }
        }

#endregion

#region properties

        /// <summary>R成分 (red)</summary>
        protected float? r;

        /// <summary>R成分 (red)</summary>
        public float? R { get { return r; } set { r = value; } }

        /// <summary>G成分 (green)</summary>
        protected float? g;

        /// <summary>G成分 (green)</summary>
        public float? G { get { return g; } set { g = value; } }

        /// <summary>B成分 (blue)</summary>
        protected float? b;

        /// <summary>B成分 (blue)</summary>
        public float? B { get { return b; } set { b = value; } }

        /// <summary>A成分 (alpha)</summary>
        protected float? a;

        /// <summary>A成分 (alpha)</summary>
        public float? A { get { return a; } set { a = value; } }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        public TweenColorOrder(float duration) : base(duration)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分 (red)</param>
        /// <param name="g">G成分 (green)</param>
        /// <param name="b">B成分 (blue)</param>
        public TweenColorOrder(float duration, float r, float g, float b) : base(duration)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分 (red)</param>
        /// <param name="g">G成分 (green)</param>
        /// <param name="b">B成分 (blue)</param>
        /// <param name="a">A成分 (alpha)</param>
        public TweenColorOrder(float duration, float r, float g, float b, float a) : base(duration)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分 (red)</param>
        /// <param name="g">G成分 (green)</param>
        /// <param name="b">B成分 (blue)</param>
        public TweenColorOrder(float duration, int r, int g, int b)
            : base(duration)
        {
            this.r = r / 255.0f;
            this.g = g / 255.0f;
            this.b = b / 255.0f;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分 (red)</param>
        /// <param name="g">G成分 (green)</param>
        /// <param name="b">B成分 (blue)</param>
        /// <param name="a">A成分 (alpha)</param>
        public TweenColorOrder(float duration, int r, int g, int b, int a) : base(duration)
        {
            this.r = r / 255.0f;
            this.g = g / 255.0f;
            this.b = b / 255.0f;
            this.a = a / 255.0f;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="color">色</param>
        public TweenColorOrder(float duration, Color color) : base(duration)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        /// <summary>
        /// ToメソッドでのTween.Finalに設定する値を取得します。
        /// </summary>
        /// <returns></returns>
        public Color GetToValue()
        {
            Color to = Color.white;
            if (r.HasValue)
            {
                to.r = r.Value;
            }
            if (g.HasValue)
            {
                to.g = g.Value;
            }
            if (b.HasValue)
            {
                to.b = b.Value;
            }
            if (a.HasValue)
            {
                to.a = a.Value;
            }
            return to;
        }

        /// <summary>
        /// FromメソッドでのTween.Beginに設定する値を取得します。
        /// </summary>
        /// <returns></returns>
        public Color GetFromValue()
        {
            Color from = Color.white;
            if (r.HasValue)
            {
                from.r = r.Value;
            }
            if (g.HasValue)
            {
                from.g = g.Value;
            }
            if (b.HasValue)
            {
                from.b = b.Value;
            }
            if (a.HasValue)
            {
                from.a = a.Value;
            }
            return from;
        }

        /// <summary>
        /// ByメソッドでのTween.Finalに設定する値を取得します。
        /// </summary>
        /// <param name="baseColor">元となる色</param>
        /// <returns></returns>
        public Color GetByValue(Color baseColor)
        {
            Color to = baseColor;
            if (r.HasValue)
            {
                to.r += r.Value;
            }
            if (g.HasValue)
            {
                to.g += g.Value;
            }
            if (b.HasValue)
            {
                to.b += b.Value;
            }
            if (a.HasValue)
            {
                to.a += a.Value;
            }
            return to;
        }

        /// <summary>
        /// Tweenの更新時に呼び出されるコールバックを生成します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <returns></returns>
        public Action<ITween<Color>> CreateOnUpdate(Graphic graphic)
        {
            ColorUpdateCallback callback = new ColorUpdateCallback(
                graphic,
                r.HasValue,
                g.HasValue,
                b.HasValue,
                a.HasValue);

            return callback.OnUpdate;
        }

#endregion
    }
}
