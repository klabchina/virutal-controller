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
using System.Collections.Generic;

namespace Jigbox.Components
{
#if UNITY_2019_4_OR_NEWER
    [RequireComponent(typeof(CanvasRenderer))]
#endif
    public abstract class BlendMask : MaskableGraphic, ICanvasRaycastFilter
    {
#region constants

        /// <summary>クリッピング時の範囲を計算するためのパラメータの名前</summary>
        protected static readonly string ClipRangePropertyName = "_ClipRange";

#endregion

#region properties

        /// <summary>マスキングが有効かどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isEnable = true;

        /// <summary>マスキングが有効かどうか</summary>
        public virtual bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {
                if (isEnable != value)
                {
                    isEnable = value;
                    SetTargetMaterial();
                    if (isEnable)
                    {
                        UpdateMaskMaterial();
                    }
                }
            }
        }

        /// <summary>マスキングに使用するマテリアル</summary>
        [HideInInspector]
        [SerializeField]
        protected Material maskMaterial = null;

        /// <summary>マスキングに使用するマテリアル</summary>
        public virtual Material MaskMaterial
        {
            get
            {
                return maskMaterial != null ? maskMaterial : DefaultMaskMaterial;
            }
            set
            {
                if (maskMaterial != value)
                {
                    maskMaterial = value;
                    if (isEnable)
                    {
                        UpdateMaskMaterial();
                    }
                }
            }
        }

        /// <summary>マスキングを行う際に基準となるオブジェクト</summary>
        [HideInInspector]
        [SerializeField]
        protected RectTransform maskedTargetRoot = null;

        /// <summary>
        /// <para>マスキングを行う際に基準となるオブジェクト</para>
        /// <para>設定されていない場合は、自身を基準とします。</para>
        /// </summary>
        public virtual RectTransform MaskedTargetRoot
        {
            get
            {
                return maskedTargetRoot != null ? maskedTargetRoot : rectTransform;
            }
            set
            {
                if (maskedTargetRoot != value)
                {
                    maskedTargetRoot = value;
                    // 対象が切り替わったことによって必ず更新が行われるように自然には発生しない-1を設定する
                    lastUpdateChildCount = -1;
                }
            }
        }

        /// <summary>自動的にマスキング対象の更新を行うかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool isAutoUpdateMaskedTargets = true;

        /// <summary>自動的にマスキング対象の更新を行うかどうか</summary>
        public virtual bool IsAutoUpdateMaskedTargets
        {
            get
            {
                return isAutoUpdateMaskedTargets;
            }
            set
            {
                if (isAutoUpdateMaskedTargets != value)
                {
                    isAutoUpdateMaskedTargets = value;
                }
            }
        }

        /// <summary>マスキング用のマテリアルを更新する際に同時に更新するマテリアル</summary>
        [HideInInspector]
        [SerializeField]
        protected List<Material> updateTogetherMaterials = new List<Material>();

        /// <summary>マスキング用のマテリアルを更新する際に同時に更新するマテリアル</summary>
        public virtual List<Material> UpdateTogetherMaterials
        {
            get
            {
                return updateTogetherMaterials;
            }
        }

        /// <summary>最後に更新を行った際の子オブジェクトの数</summary>
        protected int lastUpdateChildCount = -1;

        /// <summary>デフォルトで使用するマスキング用のマテリアル</summary>
        protected abstract string DefaultTargetMaterial { get; }

        /// <summary>マスキングに使用するマテリアルのデフォルト</summary>
        protected Material defaultMaskMaterial = null;

        /// <summary>マスキングに使用するマテリアルのデフォルト</summary>
        public virtual Material DefaultMaskMaterial
        {
            get
            {
                if (defaultMaskMaterial == null)
                {
                    defaultMaskMaterial = Instantiate(Resources.Load(DefaultTargetMaterial)) as Material;
                }
                return defaultMaskMaterial;
            }
        }
 
        /// <summary>マスクを行う対象のコンポーネント</summary>
        protected List<MaskedGraphic> targets = new List<MaskedGraphic>();

        /// <summary>マスク領域の情報</summary>
        protected Vector4 clipRange = Vector4.zero;

#if UNITY_EDITOR

        /// <summary>確認用にマスクを表示するかどうか</summary>
        [HideInInspector]
        [SerializeField]
        protected bool showMask = false;

#endif

#endregion

#region public methods

        /// <summary>
        /// マスキングに使用するマテリアルを更新します。
        /// </summary>
        public abstract void UpdateMaskMaterial();

        /// <summary>
        /// マスキング対象を追加します。
        /// </summary>
        /// <param name="target">対象となるGraphicコンポーネント</param>
        /// <returns>追加に成功すれば<c>true</c>を返します。</returns>
        public virtual bool Add(Graphic target)
        {
            // 自分自身はマスク用なので除外
            if (target == this)
            {
                return false;
            }

            foreach (MaskedGraphic maskedTarget in targets)
            {
                if (maskedTarget.Graphic == target)
                {
                    return false;
                }
            }

            MaskedGraphic maskedGraphic = new MaskedGraphic(target);
            if (IsEnable)
            {
                maskedGraphic.SetMaterial(MaskMaterial);
            }
            targets.Add(maskedGraphic);
            return true;
        }

        /// <summary>
        /// マスキング対象から除外します。
        /// </summary>
        /// <param name="target">対象となるGraphicコンポーネント</param>
        /// <returns>除外に成功すれば<c>true</c>を返します。</returns>
        public virtual bool Remove(Graphic target)
        {
            int index = -1;
            for (int i = 0; i < targets.Count; ++i)
            {
                if (targets[i].Graphic == target)
                {
                    index = i;
                    targets[i].SetDefault();
                    break;
                }
            }

            if (index >= 0)
            {
                targets.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// マスキング対象を全て除外します。
        /// </summary>
        public virtual void RemoveAll()
        {
            foreach (MaskedGraphic target in targets)
            {
                target.SetDefault();
            }

            targets.Clear();
        }

        /// <summary>
        /// 対象以下のマスキング対象を更新します。
        /// </summary>
        /// <param name="target">マスキングの基準となるオブジェクト</param>
        public virtual void UpdateTargetInChildren(RectTransform target)
        {
            Graphic[] graphics = target.GetComponentsInChildren<Graphic>(true);

            if (targets.Count == 0)
            {
                foreach (Graphic graphic in graphics)
                {
                    // 自分自身はマスク用なので除外
                    if (graphic == this)
                    {
                        continue;
                    }

                    targets.Add(new MaskedGraphic(graphic));
                }
            }
            else
            {
                List<MaskedGraphic> newList = new List<MaskedGraphic>();

                foreach (Graphic graphic in graphics)
                {
                    // 自分自身はマスク用なので除外
                    if (graphic == this)
                    {
                        continue;
                    }

                    // すでに参照を持っているか確認
                    int index = -1;
                    for (int i = 0; i < targets.Count; ++i)
                    {
                        if (targets[i].Graphic == graphic)
                        {
                            index = i;
                            break;
                        }
                    }
                    
                    // 参照を持っている場合は、古いリストから移動
                    if (index >= 0)
                    {
                        newList.Add(targets[index]);
                        targets.RemoveAt(index);
                    }
                    else
                    {
                        newList.Add(new MaskedGraphic(graphic));
                    }
                }

                // 古いリストに乗っている参照を解放
                RemoveAll();
                targets = newList;
            }

            SetTargetMaterial();
            UpdateMaskMaterial();
        }

        /// <summary>
        /// 自身、もしくは対象以下のマスキング対象を更新します。
        /// </summary>
        public virtual void UpdateTargetInChildren()
        {
            UpdateTargetInChildren(MaskedTargetRoot);
        }

        /// <summary>
        /// レイキャストが有効かどうかを返します。
        /// </summary>
        /// <param name="screenPoint">入力の画面上における座標</param>
        /// <param name="eventCamera">カメラ</param>
        /// <returns></returns>
        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (!isActiveAndEnabled)
            {
                return true;
            }

            // RectTransformの矩形領域外はレイキャストをブロックする
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, eventCamera);
        }

#endregion

#region protected methods

        /// <summary>
        /// マスキング対象のマテリアルを設定します。
        /// </summary>
        protected virtual void SetTargetMaterial()
        {
            if (isEnable)
            {
                foreach (MaskedGraphic target in targets)
                {
                    target.SetMaterial(MaskMaterial);
                }
            }
            else
            {
                foreach (MaskedGraphic target in targets)
                {
                    target.SetDefault();
                }
            }
        }

        /// <summary>
        /// マスキングに使用するクリッピング情報を更新します。
        /// </summary>
        protected virtual void UpdateClipRange()
        {
            // xy座標は、シェーダー内で処理する際のマスキング用のuv座標のオフセット値
            // zw座標は、マスキング対象の頂点座標をマスキング用のuv座標に変換するための値

            // zw座標に設定した値を用いて、マスキング対象の頂点情報を
            // このGameObjectの矩形範囲(以後、マスク)に対する比率で表現する。
            // マスクの座標をzw座標を用いて変換すると、マスクが原点にあった場合の
            // 相対的な位置を比率で求めることができる。
            // これをxy座標に格納し、始めに作った比率にオフセット値として加えることで
            // マスキング対象のマスクに対する相対的な位置を比率で表すことができ、
            // これをuv座標としてマスク用のテクスチャに適用することでマスキングを実現する。
            
            // 位置情報は絶対値的な位置が必要となるので、Canvasと同じ座標空間に置かれているものとして計算する
            clipRange = transform.InverseTransformPoint(canvas.transform.position);
            
            // Canvasを除いたScale値を計算
            Vector2 scale = new Vector2(transform.lossyScale.x / canvas.transform.lossyScale.x,
                transform.lossyScale.y / canvas.transform.lossyScale.y);

            // 位置の計算をCanvas以下相当にした際に、拡大縮小によってズレる分を補正する
            clipRange.x += clipRange.x * (scale.x - 1.0f);
            clipRange.y += clipRange.y * (scale.y - 1.0f);

            clipRange.x += (rectTransform.pivot.x - 0.5f) * rectTransform.rect.width * scale.x;
            clipRange.y += (rectTransform.pivot.y - 0.5f) * rectTransform.rect.height * scale.y;

            // サイズはスケールの影響を加味したものを利用する
            Vector2 size = GetRenderSize();

            // 座標をクリッピング用のuvに変換するための値
            // 中心からのオフセットで計算するので値は半分にする
            // シェーダー内で乗算で処理できるように逆数化しておく
            clipRange.z = size.x != 0.0f ? 1.0f / (size.x * 0.5f) : 0.0f;
            clipRange.w = size.y != 0.0f ? 1.0f / (size.y * 0.5f) : 0.0f;

            // xy座標をオフセット用の値に変換
            if (clipRange.z != 0.0f)
            {
                clipRange.x = clipRange.x * clipRange.z;
            }

            if (clipRange.w != 0.0f)
            {
                clipRange.y = clipRange.y * clipRange.w;
            }

            // 矩形のサイズが0の場合、その方向のマスキングはuv座標(0.5, 0.5)の状態で計算される

            MaskMaterial.SetVector(ClipRangePropertyName, clipRange);

            foreach (Material material in UpdateTogetherMaterials)
            {
                if (material == null)
                {
                    continue;
                }

                material.SetVector(ClipRangePropertyName, clipRange);
            }
        }

        /// <summary>
        /// レンダリングする矩形領域のサイズを取得します。
        /// </summary>
        /// <returns></returns>
        protected Vector2 GetRenderSize()
        {
            // CanvasのScale値は見た目が正しくなるように自動的に計算されているため、
            // Canvasを除いた状態の拡縮値を求めてサイズ計算に適用する
            Vector2 scale = new Vector2(transform.lossyScale.x / canvas.transform.lossyScale.x,
                    transform.lossyScale.y / canvas.transform.lossyScale.y);

            Vector2 size = rectTransform.rect.size;
            size.x = size.x * scale.x;
            size.y = size.y * scale.y;

            return size;
        }

#endregion

#region override unity methods

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (IsAutoUpdateMaskedTargets)
            {
                UpdateTargetInChildren();
                lastUpdateChildCount = MaskedTargetRoot.childCount;
            }
            else
            {
                UpdateMaskMaterial();
            }

            rectTransform.hasChanged = true;
        }

        protected virtual void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            if (!isEnable)
            {
                return;
            }

            // 対象の子オブジェクト数の変化があれば、
            // 自動的に対象コンポーネントの更新を行う
            if (IsAutoUpdateMaskedTargets && MaskedTargetRoot.childCount != lastUpdateChildCount)
            {
                lastUpdateChildCount = MaskedTargetRoot.childCount;
                UpdateTargetInChildren();
            }

            UpdateClipRange();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // マスキング用のオブジェクト自体はレンダリングする必要がない
            vh.Clear();
        }

#endregion
    }
}
