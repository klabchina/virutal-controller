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
using Jigbox.Tween;

namespace Jigbox.NovelKit
{
    public abstract class AdvObject2DBase : AdvObjectBase, IAdvObjectShowTransition, IAdvObjectColor
    {
#region properties

        /// <summary>自身の画像</summary>
        [SerializeField]
        protected Image image;

        /// <summary>表示座標</summary>
        public Vector2 Position2D
        {
            get
            {
                return new Vector2(LocalTransform.localPosition.x, LocalTransform.localPosition.y);
            }
            set
            {
                LocalTransform.localPosition = new Vector3(value.x, value.y);
            }
        }

        /// <summary>RectTransform</summary>
        protected RectTransform rectTransform;

        /// <summary>RectTransform</summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }
                return rectTransform;
            }
        }

        /// <summary>Material</summary>
        public override Material Material { get { return image.material; } set { image.material = value; } }

        /// <summary>画像の大きさ</summary>
        public virtual Vector2 Size
        {
            get
            {
                if (LocalTransform is RectTransform)
                {
                    return RectTransform.rect.size;
                }
                return Vector2.zero;
            }
            set
            {
                if (LocalTransform is RectTransform)
                {
                    Vector2 anchorMargin = Vector2.zero;
                    RectTransform parent = LocalTransform.parent as RectTransform;
                    if (parent != null)
                    {
                        Rect parentRect = parent.rect;
                        anchorMargin.x = RectTransform.anchorMax.x * parentRect.width - RectTransform.anchorMin.x * parentRect.width;
                        anchorMargin.y = RectTransform.anchorMax.y * parentRect.height - RectTransform.anchorMin.y * parentRect.height;
                    }
                    RectTransform.sizeDelta = value - anchorMargin;
                }
            }
        }

        /// <summary>画像の色</summary>
        public virtual Color Color
        {
            get
            {
                return image.color;
            }
            set
            {
                if (image.color != value)
                {
                    image.color = value;
                }
            }
        }

        /// <summary>アルファ値</summary>
        protected virtual float Alpha 
        {
            get
            {
                return image.color.a; 
            }
            set
            {
                if (image.color.a != value)
                {
                    Color color = image.color;
                    color.a = value;
                    image.color = color;
                }
            }
        }

        /// <summary>表示順を決めるためのDepth値</summary>
        protected int depth = 0;

        /// <summary>表示順を決めるためのDepth値</summary>
        public abstract int Depth { get; set; }

        /// <summary>表示切替時のトランジションの時間</summary>
        public float ShowTransitionTime { get; protected set; }

        /// <summary>表示切替時のアルファのトランジション用Tween</summary>
        protected TweenSingle alphaTween = new TweenSingle();

        /// <summary>表示切替時のアルファのトランジション用Tween</summary>
        public TweenSingle AlphaTween { get { return alphaTween; } }

        /// <summary>トランジション中かどうか</summary>
        protected bool IsTransitioning { get { return alphaTween.State != TweenState.None && alphaTween.State != TweenState.Done; } }

#endregion

#region public methods

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="id">管理用ID(オブジェクトの種類毎)</param>
        /// <param name="type">オブジェクトの種類</param>
        /// <param name="setting">オブジェクトの基礎設定</param>
        public override void Init(int id, ObjectType type, AdvObjectSetting.ObjectSetting setting)
        {
            base.Init(id, type, setting);
            ShowTransitionTime = setting.ShowTransitionTime;
            InitColor();
            Show();
        }

        /// <summary>
        /// 表示切替時のトランジションを設定します。
        /// </summary>
        /// <param name="time">トランジションの時間</param>
        public virtual void SetShowTransitionStatus(float time)
        {
            ShowTransitionTime = time;
        }

        /// <summary>
        /// オブジェクトを表示状態にします。
        /// </summary>
        /// <param name="time">トランジションの時間</param>
        public virtual void Show(float time)
        {
            base.Show();

            if (time > 0.0f)
            {
                alphaTween.Begin = Alpha;
                alphaTween.Final = 1.0f;
                alphaTween.Duration = time;
                alphaTween.Start();
            }
            else
            {
                if (IsTransitioning)
                {
                    alphaTween.Complete();
                }
                if (Alpha == 0.0f)
                {
                    Alpha = 1.0f;
                }
            }
        }

        /// <summary>
        /// オブジェクトを表示状態にします。
        /// </summary>
        public override void Show()
        {
            Show(ShowTransitionTime);
        }

        /// <summary>
        /// オブジェクトを非表示状態にします。
        /// </summary>
        /// <param name="time">トランジションの時間</param>
        public virtual void Hide(float time)
        {
            if (time > 0.0f)
            {
                alphaTween.Begin = Alpha;
                alphaTween.Final = 0.0f;
                alphaTween.Duration = time;
                alphaTween.Start();
            }
            else
            {
                if (IsTransitioning)
                {
                    alphaTween.Complete();
                }
                // 1.0未満のアルファの場合は、そのままで非表示にする
                if (Alpha == 1.0f)
                {
                    Alpha = 0.0f;
                }
                base.Hide();
            }
        }

        /// <summary>
        /// オブジェクトを非表示状態にします。
        /// </summary>
        public override void Hide()
        {
            Hide(ShowTransitionTime);
        }

        /// <summary>
        /// リソースを読み込みます。
        /// </summary>
        /// <param name="loader">Loader</param>
        /// <param name="resourcePath">リソースのパス</param>
        public override void LoadResource(IAdvResourceLoader loader, string resourcePath)
        {
            Sprite sprite = loader.Load<Sprite>(resourcePath);

            if (sprite == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("対象のリソースが存在しない、もしくは、Spriteではありません。"
                    + "\nパス : " + resourcePath);
#endif
                return;
            }

            image.sprite = sprite;
            ResizeToResource();
        }

        /// <summary>
        /// Depth値を更新します。
        /// </summary>
        public virtual void UpdateDepth()
        {
            depth = LocalTransform.GetSiblingIndex();
        }

#endregion

#region protected methods
        
        /// <summary>
        /// リソースのサイズに合わせて画像の大きさを設定します。
        /// </summary>
        protected void ResizeToResource()
        {
            Size = new Vector2(image.sprite.rect.width, image.sprite.rect.height);
        }

        /// <summary>
        /// 色状態を初期化します。
        /// </summary>
        protected virtual void InitColor()
        {
            Alpha = 0.0f;
        }

        /// <summary>
        /// アルファの状態が更新された際に呼び出されます。
        /// </summary>
        /// <param name="tween"></param>
        protected virtual void OnUpdateAlpha(ITween<float> tween)
        {
            Alpha = tween.Value;
        }

#endregion
        
#region override unity methods

        protected override void Awake()
        {
            base.Awake();
            alphaTween.OnUpdate(OnUpdateAlpha);
            alphaTween.Begin = 1.0f;
            alphaTween.Final = 1.0f;
        }

        protected virtual void OnDisable()
        {
            // Tweenが途中だった場合のみ、強制的に完了状態まで進める
            if (IsTransitioning)
            {
                alphaTween.Complete();
            }
        }

#endregion
    }
}
