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
using System.Linq;

namespace Jigbox.Components
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TextViewLongTextExtension : MonoBehaviour
    {
#region constants

        /// <summary>
        /// <para>分割を行う文字数の最大数</para>
        /// <para>VertexHelper内の制約で65000頂点以上が許容されないため、論理的な限界文字数は16249文字</para>
        /// </summary>
        public static readonly int DivideLengthLimit = 16000;

        /// <summary>分割を行う文字数のデフォルト値</summary>
        protected static readonly int DefaultDivideLength = 3000;

#endregion

#region properties

        /// <summary>分割を行う文字数</summary>
        [HideInInspector]
        [SerializeField]
        protected int divideLength = DefaultDivideLength;

        /// <summary>分割を行う文字数</summary>
        public int DivideLength { get { return divideLength; } }

        /// <summary>描画用オブジェクト</summary>
        protected List<TextViewExcessGlyphDrawer> drawers = new List<TextViewExcessGlyphDrawer>();

        /// <summary>MeshModifier</summary>
        protected List<IMeshModifier> modifiers = new List<IMeshModifier>();

        /// <summary>TextViewで使っているマテリアル</summary>
        protected Material cachedTextViewMaterial = null;

        /// <summary>TextViewで使っているテクスチャ</summary>
        protected Texture cachedTextViewTexture = null;

        /// <summary>追加された文字の数</summary>
        protected int characterCount = 0;

        /// <summary>現在の参照してるDrawerのインデックス</summary>
        protected int drawerIndex = 0;

        /// <summary>TextViewに設定されるClipRect</summary>
        protected Rect? cachedClipRect;

        /// <summary>TextViewに設定されるValidClip</summary>
        protected bool? cachedValidClip;

#endregion

#region public methods

        /// <summary>
        /// 描画する文字数を設定します。
        /// </summary>
        /// <param name="length">文字数</param>
        /// <returns>描画用オブジェクトが追加された場合、<c>true</c>を返します。</returns>
        public virtual bool SetLength(int length)
        {
            bool result = false;
            if (length <= divideLength)
            {
                return result;
            }

            // 本体が描画する分は不要なので描画コンポーネントの数+1で計算
            int canDrawLength = (drawers.Count + 1) * divideLength;
            result = canDrawLength < length;
            RectTransform parent = gameObject.transform as RectTransform;
            while (canDrawLength < length)
            {
                var drawer = TextViewExcessGlyphDrawer.Create(parent);

                // 生成後1F目ではdrawerが存在せずClipRectが設定されていないケースがあるため
                // drawerを追加した際にClipRectを設定する
                if (cachedClipRect != null && cachedValidClip != null)
                {
                    drawer.SetClipRect(cachedClipRect.Value, cachedValidClip.Value);
                }

                drawers.Add(drawer);
                canDrawLength += divideLength;
            }

            return result;
        }

        /// <summary>
        /// マテリアルを更新します。
        /// </summary>
        /// <param name="material">マテリアル</param>
        /// <param name="texture"テクスチャ></param>
        public virtual void UpdateMaterial(Material material, Texture texture)
        {
            cachedTextViewMaterial = material;
            cachedTextViewTexture = texture;

            foreach (TextViewExcessGlyphDrawer drawer in drawers)
            {
                drawer.UpdateMaterial(material, texture);
            }
        }

        /// <summary>
        /// カリングの状態を更新します。
        /// </summary>
        /// <param name="cull">カリングを行うかどうか</param>
        public virtual void UpdateCull(bool cull)
        {
            foreach (var drawer in drawers)
            {
                drawer.UpdateCull(cull);
            }
        }

        /// <summary>
        /// クリッピング領域を設定します。
        /// </summary>
        /// <param name="clipRect">クリッピング領域</param>
        /// <param name="validClip">クリッピングが有効かどうか</param>
        public virtual void SetClipRect(Rect clipRect, bool validClip)
        {
            cachedClipRect = clipRect;
            cachedValidClip = validClip;

            foreach (var drawer in drawers)
            {
                drawer.SetClipRect(clipRect, validClip);
            }
        }

        /// <summary>
        /// 頂点の追加を開始します。
        /// </summary>
        public virtual void BeginFill()
        {
            characterCount = 0;
            drawerIndex = 0;
            GetComponents<IMeshModifier>(modifiers);
        }

        /// <summary>
        /// 頂点を追加します。
        /// </summary>
        /// <param name="vertices">追加する頂点情報</param>
        public virtual void AddUIVertexQuad(UIVertex[] vertices)
        {
            // 文字を表示するためのポリゴンの頂点情報が渡される想定
            UnityEngine.Assertions.Assert.AreEqual(4, vertices.Length);

            drawers[drawerIndex].AddVertices(vertices);
            ++characterCount;

            if (characterCount >= divideLength)
            {
                drawers[drawerIndex].GenerateMesh(
                    modifiers,
                    cachedTextViewMaterial,
                    cachedTextViewTexture);

                characterCount = 0;
                ++drawerIndex;
            }
        }

        /// <summary>
        /// 頂点の追加を終了します。
        /// </summary>
        public virtual void EndFill()
        {
            for (int i = drawerIndex; i < drawers.Count; ++i)
            {
                drawers[i].GenerateMesh(
                    modifiers,
                    cachedTextViewMaterial,
                    cachedTextViewTexture);
            }
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            GetComponentsInChildren<TextViewExcessGlyphDrawer>(true, drawers);
        }

#endregion
    }
}
