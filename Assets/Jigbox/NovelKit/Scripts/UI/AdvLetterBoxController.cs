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
using Jigbox.Tween;

namespace Jigbox.NovelKit
{
    public class AdvLetterBoxController : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// ブラインドの種類
        /// </summary>
        public enum BlindType
        {
            /// <summary>レターボックス形式(上下)</summary>
            Letter,
            /// <summary>ピラーボックス形式(左右)</summary>
            Piller,
        }
        
        /// <summary>
        /// ブラインド
        /// </summary>
        protected class Blind
        {
            public enum Position
            {
                None,
                Top,
                Bottom,
                Left,
                Right,
            }

            /// <summary>位置</summary>
            protected Position position = Position.None;

            /// <summary>隠すためのImage</summary>
            public Image BlindImage { get; protected set; }

            /// <summary>RectTransform</summary>
            protected RectTransform rectTransform;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="controller">レターボックスの制御コンポーネント</param>
            /// <param name="position">位置</param>
            /// <param name="size">サイズ</param>
            public Blind(AdvLetterBoxController controller, Position position, Vector2 size)
            {
                GameObject obj = new GameObject(position.ToString());
                obj.transform.SetParent(controller.transform, false);
                BlindImage = obj.AddComponent<Image>();
                BlindImage.color = Color.black;
                rectTransform = BlindImage.rectTransform;
                Set(position, size);
            }

            /// <summary>
            /// 位置を設定します。
            /// </summary>
            /// <param name="position">位置</param>
            /// <param name="size">サイズ</param>
            public void Set(Position position, Vector2 size)
            {
                if (this.position == position)
                {
                    return;
                }

                this.position = position;
                rectTransform.sizeDelta = size;
                Vector2 parentSize = (rectTransform.parent as RectTransform).rect.size;
                Vector3 localPosition = Vector3.zero;

                switch (position)
                {
                    case Position.Top:
                        rectTransform.pivot = new Vector2(0.5f, 1.0f);
                        localPosition.y = parentSize.y / 2.0f;
                        break;
                    case Position.Bottom:
                        rectTransform.pivot = new Vector2(0.5f, 1.0f);
                        rectTransform.localEulerAngles = new Vector3(180.0f, 0.0f, 0.0f);
                        localPosition.y = -parentSize.y / 2.0f;
                        break;
                    case Position.Left:
                        rectTransform.pivot = new Vector2(0.0f, 0.5f);
                        localPosition.x = -parentSize.x / 2.0f;
                        break;
                    case Position.Right:
                        rectTransform.pivot = new Vector2(0.0f, 0.5f);
                        rectTransform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                        localPosition.x = parentSize.x / 2.0f;
                        break;
                }

                rectTransform.localPosition = localPosition;
            }

            /// <summary>
            /// 状態を更新します。
            /// </summary>
            /// <param name="value"></param>
            public void Update(float value)
            {
                Vector3 scale = rectTransform.localScale;
                if (position == Position.Top || position == Position.Bottom)
                {
                    scale.y = value;
                }
                else
                {
                    scale.x = value;
                }
                rectTransform.localScale = scale;

                Color color = BlindImage.color;
                color.a = value;
                BlindImage.color = color;
            }
        }

#endregion

#region properties

        /// <summary>RectTransform</summary>
        protected RectTransform rectTransform;

        /// <summary>ブラインドオブジェクト1</summary>
        protected Blind blind1;

        /// <summary>ブラインドオブジェクト2</summary>
        protected Blind blind2;

        /// <summary>非表示になった際のコールバック</summary>
        protected Action hideCallback;

        /// <summary>表示切替時のトランジション用Tween</summary>
        public TweenSingle Tween { get; protected set; }
        
#endregion

#region public methods

        /// <summary>
        /// ピクセル指定でレターボックスを作成します。
        /// </summary>
        /// <param name="parent">親オブジェクト</param>
        /// <param name="hideCallback">非表示になった際のコールバック</param>
        /// <param name="type">ブラインドの種類</param>
        /// <param name="size">ブラインのサイズ(pixel)</param>
        /// <returns></returns>
        public static AdvLetterBoxController CreateFromPixel(GameObject parent, Action hideCallback, BlindType type, float size)
        {
            AdvLetterBoxController controller = Create(parent, hideCallback, type);

            controller.SetBlind(type, size);

            return controller;
        }

        /// <summary>
        /// 割合指定でレターボックスを作成します。
        /// </summary>
        /// <param name="parent">親オブジェクト</param>
        /// <param name="hideCallback">非表示になった際のコールバック</param>
        /// <param name="type">ブラインドの種類</param>
        /// <param name="ratio">ブラインの画面に対する割合</param>
        /// <returns></returns>
        public static AdvLetterBoxController CreateFromRatio(GameObject parent, Action hideCallback, BlindType type, float ratio)
        {
            AdvLetterBoxController controller = Create(parent, hideCallback, type);

            float size = (type == BlindType.Letter ? controller.rectTransform.rect.height : controller.rectTransform.rect.width) * ratio;
            controller.SetBlind(type, size);

            return controller;
        }

        /// <summary>
        /// ブラインドを表示します。
        /// </summary>
        /// <param name="time">時間</param>
        public void Show(float time)
        {
            if (time == 0.0f)
            {
                blind1.Update(1.0f);
                blind2.Update(1.0f);
                return;
            }

            Tween.Begin = 0.0f;
            Tween.Final = 1.0f;
            Tween.Duration = time;

            Tween.Start();
        }

        /// <summary>
        /// ブラインドを非表示にします。
        /// </summary>
        /// <param name="time">時間</param>
        public void Hide(float time)
        {
            if (time == 0.0f)
            {
                blind1.Update(0.0f);
                blind2.Update(0.0f);

                if (hideCallback != null)
                {
                    hideCallback();
                }
                return;
            }

            Tween.Begin = 1.0f;
            Tween.Final = 0.0f;
            Tween.Duration = time;
            if (hideCallback != null)
            {
                Tween.OnComplete(OnCompleteHide);
            }

            Tween.Start();
        }

        /// <summary>
        /// ブラインドに画像を設定します。
        /// </summary>
        /// <param name="image">画像</param>
        public void SetImage(Sprite image)
        {
            blind1.BlindImage.sprite = image;
            blind1.BlindImage.color = Color.white;
            blind2.BlindImage.sprite = image;
            blind2.BlindImage.color = Color.white;
        }

#endregion

#region protected methods

        /// <summary>
        /// レターボックスを作成します。
        /// </summary>
        /// <param name="parent">親オブジェクト</param>
        /// <param name="hideCallback">非表示になった際のコールバック</param>
        /// <param name="type">ブラインドの種類</param>
        /// <returns></returns>
        protected static AdvLetterBoxController Create(GameObject parent, Action hideCallback, BlindType type)
        {
            GameObject obj = new GameObject(type.ToString(), typeof(RectTransform));
            obj.transform.SetParent(parent.transform, false);
            AdvLetterBoxController controller = obj.AddComponent<AdvLetterBoxController>();
            controller.rectTransform = controller.transform as RectTransform;
            controller.rectTransform.anchorMin = Vector2.zero;
            controller.rectTransform.anchorMax = Vector2.one;
            controller.rectTransform.sizeDelta = Vector2.zero;
            controller.hideCallback = hideCallback;

            return controller;
        }

        /// <summary>
        /// ブラインドを作成して、設定します。
        /// </summary>
        /// <param name="type">ブラインドの種類</param>
        /// <param name="size">ブラインドの大きさ(pixel)</param>
        protected void SetBlind(BlindType type, float size)
        {
            switch (type)
            {
                case BlindType.Letter:
                    Vector2 letterSize = new Vector2(rectTransform.rect.width, size);
                    blind1 = new Blind(this, Blind.Position.Top, letterSize);
                    blind2 = new Blind(this, Blind.Position.Bottom, letterSize);
                    break;
                case BlindType.Piller:
                    Vector2 pillerSize = new Vector2(size, rectTransform.rect.height);
                    blind1 = new Blind(this, Blind.Position.Left, pillerSize);
                    blind2 = new Blind(this, Blind.Position.Right, pillerSize);
                    break;
            }
            blind1.Update(0.0f);
            blind2.Update(0.0f);
        }

        /// <summary>
        /// 非表示化が完了した際に呼び出されます。
        /// </summary>
        /// <param name="tween"></param>
        protected void OnCompleteHide(ITween tween)
        {
            Tween.RemoveOnComplete(OnCompleteHide);
            hideCallback();
        }

#endregion

#region override unity methods

        void Awake()
        {
            Tween = new TweenSingle();
            Tween.OnUpdate(t =>
            {
                blind1.Update(t.Value);
                blind2.Update(t.Value);
            });
        }

        void OnDestroy()
        {
            Tween.Kill();
        }

#endregion
    }
}
