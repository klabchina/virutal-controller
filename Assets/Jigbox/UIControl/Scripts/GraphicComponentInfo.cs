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
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Jigbox.Components;

namespace Jigbox.UIControl
{
    public class GraphicComponentInfo
    {
#region inner classes, enum, and structs

        /// <summary>
        /// UIBehaivourの情報
        /// </summary>
        public abstract class UIBehaviourInfo
        {
            // 参照はUIBehaviourで保持できるが参照するのに毎回キャストが必要だと
            // 余計なコストを食うので、一旦個別で参照できるためのプロパティを用意
            // 今後増えるようなら対応を考える

            /// <summary>Graphicコンポーネント</summary>
            public Graphic Graphic { get; protected set; }

            /// <summary>元々の色</summary>
            public Color DefaultColor { get; set; }

            /// <summary>現在の色</summary>
            public virtual Color Color { get; set; }

            /// <summary>色の変更が有効化どうか</summary>
            public bool IsValid { get; set; }

            /// <summary>アルファの変更が有効かどうか</summary>
            public bool IsValidAlpha { get; set; }
        }

        /// <summary>
        /// Graphicの情報
        /// </summary>
        public sealed class GraphicInfo : UIBehaviourInfo
        {
            /// <summary>現在の色</summary>
            public override Color Color
            {
                get { return Graphic.color; }
                set { Graphic.color = value; }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="graphic">Graphicコンポーネント</param>
            public GraphicInfo(Graphic graphic)
            {
                Graphic = graphic;
                DefaultColor = graphic.color;
                IsValid = true;
                IsValidAlpha = true;
            }
        }

        /// <summary>
        /// Shadowの情報
        /// </summary>
        public sealed class ShadowInfo : UIBehaviourInfo
        {
            /// <summary>Shadowコンポーネント</summary>
            public Shadow Shadow { get; private set; }

            /// <summary>現在の色</summary>
            public override Color Color
            {
                get { return Shadow.effectColor; }
                set { Shadow.effectColor = value; }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="shadow">Shadowコンポーネント(Shadow、Outline)</param>
            public ShadowInfo(Shadow shadow)
            {
                this.Shadow = shadow;
                DefaultColor = shadow.effectColor;
                IsValid = true;
                IsValidAlpha = true;
            }
        }

        /// <summary>
        /// Gradation情報用のベースクラス
        /// </summary>
        public abstract class GradationInfo : UIBehaviourInfo
        {
            /// <summary>Gradationコンポーネント</summary>
            public IGradation Gradation { get; private set; }

            /// <summary>現在の色</summary>
            public override Color Color
            {
                get { return EffectColor; }
                set
                {
                    // 色を合成する設定の場合、アルファ値の合成は行わない
                    if (Gradation.Type != ColorEffectType.None)
                    {
                        value.a = DefaultColor.a;
                    }

                    EffectColor = value;
                }
            }

            /// <summary>グラデーションの色</summary>
            protected abstract Color EffectColor { get; set; }

            /// <summary>色の合成方法が加算かどうか</summary>
            public bool IsAdditive
            {
                get { return Gradation.Type == ColorEffectType.Additive; }
            }

            /// <summary>色の合成方法が乗算かどうか</summary>
            public bool IsMultiple
            {
                get { return Gradation.Type == ColorEffectType.Multiple; }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="gradation">Gradationコンポーネント</param>
            public GradationInfo(IGradation gradation)
            {
                this.Gradation = gradation;
                IsValid = true;
                IsValidAlpha = true;
            }
        }

        /// <summary>
        /// Gradationの開始色の情報
        /// </summary>
        public sealed class GradationStartColorInfo : GradationInfo
        {
            /// <summary>グラデーションの色</summary>
            protected override Color EffectColor
            {
                get { return Gradation.StartColor; }
                set { Gradation.StartColor = value; }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="gradation">Gradationコンポーネント</param>
            public GradationStartColorInfo(IGradation gradation) : base(gradation)
            {
                DefaultColor = gradation.StartColor;
            }
        }

        /// <summary>
        /// Gradationの終了色の情報
        /// </summary>
        public sealed class GradationEndColorInfo : GradationInfo
        {
            /// <summary>グラデーションの色</summary>
            protected override Color EffectColor
            {
                get { return Gradation.EndColor; }
                set { Gradation.EndColor = value; }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="gradation">Gradationコンポーネント</param>
            public GradationEndColorInfo(IGradation gradation) : base(gradation)
            {
                DefaultColor = gradation.EndColor;
            }
        }

#endregion

#region properties

        /// <summary>
        /// ルートになる親オブジェクトにアタッチされている監視コンポーネント
        /// アタッチされていない場合は常にnull
        /// </summary>
        protected HierarchyChangeDetector hierarchyChangeDetector = null;

        /// <summary>UIBehaivourを持つコンポーネントの情報</summary>
        protected List<UIBehaviourInfo> behavioursInfo;

        /// <summary>
        /// FindGraphicComponentsで渡される引数のキャッシュ
        /// </summary>
        protected bool cachedValidOwn = false;

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GraphicComponentInfo()
        {
            behavioursInfo = new List<UIBehaviourInfo>();
        }

        /// <summary>
        /// 対象オブジェクト以下に存在する全てのGraphicコンポーネントを取得します。
        /// </summary>
        /// <param name="target">対象のゲームオブジェクト</param>
        /// <param name="isValidOwn">自身にアタッチされたコンポーネントを含むかどうか</param>
        public void FindGraphicComponents(GameObject target, bool isValidOwn = true)
        {
            cachedValidOwn = isValidOwn;

            if (hierarchyChangeDetector == null)
            {
                hierarchyChangeDetector = target.GetComponent<HierarchyChangeDetector>();

                if (hierarchyChangeDetector != null)
                {
                    hierarchyChangeDetector.AddHierarchyChangedCallback(UpdateBehavioursInfoIfNeeded);
                }
            }

            List<UIBehaviour> uiComponents = new List<UIBehaviour>();
            target.GetComponentsInChildren(true, uiComponents);

            if (!isValidOwn)
            {
                // 自身のコンポーネントを含まない場合、自身を除外
                for (int i = 0; i < uiComponents.Count; ++i)
                {
                    if (target == uiComponents[i].gameObject)
                    {
                        uiComponents.RemoveAt(i);
                        break;
                    }
                }
            }

            // Graphicは、1つのGameObjectに複数設定できない
            // Shodowは、Graphicコンポーネントを持たない場合機能しない
            // 上記性質上、色を変更しないコンポーネントを抽出する場合、
            // 対象となるGameObjectにアタッチされているかで判断できる
            List<GameObject> invalidList = new List<GameObject>();
            List<GameObject> invalidAlphaList = new List<GameObject>();

            GetInvalidGraphics(target, invalidList, invalidAlphaList);

            foreach (UIBehaviour component in uiComponents)
            {
                UIBehaviourInfo info = null;
                UIBehaviourInfo subInfo = null;

                // 既に登録されているコンポーネントは処理から除外
                if (IsExistsInfo(component))
                {
                    continue;
                }

                if (component is Graphic)
                {
                    info = new GraphicInfo(component as Graphic);
                }
                else if (component is Shadow)
                {
                    info = new ShadowInfo(component as Shadow);
                }

                // Gradationコンポーネントは、グラデーションの開始色と終了色の2色を扱うので、
                // 1コンポーネントを2つに分けて記録しておく
                else if (component is IGradation)
                {
                    info = new GradationStartColorInfo(component as IGradation);
                    subInfo = new GradationEndColorInfo(component as IGradation);
                }

                if (info == null)
                {
                    continue;
                }

                for (int i = 0; i < invalidList.Count; ++i)
                {
                    if (component.gameObject == invalidList[i])
                    {
                        info.IsValid = false;
                        if (subInfo != null)
                        {
                            subInfo.IsValid = false;
                        }

                        break;
                    }
                }

                for (int i = 0; i < invalidAlphaList.Count; ++i)
                {
                    if (component.gameObject == invalidAlphaList[i])
                    {
                        info.IsValidAlpha = false;
                        if (subInfo != null)
                        {
                            subInfo.IsValidAlpha = false;
                        }

                        break;
                    }
                }

                behavioursInfo.Add(info);
                if (subInfo != null)
                {
                    behavioursInfo.Add(subInfo);
                }
            }

            // オブジェクトが削除されている場合、リストを更新する
            var exceptList = new List<UIBehaviourInfo>();

            foreach (var behaviourInfo in behavioursInfo)
            {
                if (behaviourInfo is GraphicInfo)
                {
                    var graphicInfo = behaviourInfo as GraphicInfo;
                    if (!uiComponents.Contains(graphicInfo.Graphic))
                    {
                        exceptList.Add(graphicInfo);
                    }
                }

                if (behaviourInfo is ShadowInfo)
                {
                    var shadowInfo = behaviourInfo as ShadowInfo;
                    if (!uiComponents.Contains(shadowInfo.Shadow))
                    {
                        exceptList.Add(shadowInfo);
                    }
                }

                if (behaviourInfo is GradationInfo)
                {
                    var gradationInfo = behaviourInfo as GradationInfo;
                    bool isExcept = true;
                    foreach (var component in uiComponents)
                    {
                        if (component is IGradation && (IGradation)component == gradationInfo.Gradation)
                        {
                            isExcept = false;
                        }
                    }

                    if(isExcept)
                    {
                        exceptList.Add(gradationInfo);
                    }
                }
            }

            foreach (var except in exceptList)
            {
                behavioursInfo.Remove(except);
            }
        }

        /// <summary>
        /// 画像の色情報を記録します。
        /// </summary>
        public void RegisterColor()
        {
            UpdateBehavioursInfoIfNeeded();

            foreach (UIBehaviourInfo info in behavioursInfo)
            {
                info.DefaultColor = info.Color;
            }
        }

        /// <summary>
        /// 色を元に戻します。
        /// </summary>
        public void ResetColor()
        {
            UpdateBehavioursInfoIfNeeded();

            foreach (UIBehaviourInfo info in behavioursInfo)
            {
                if (info.IsValid)
                {
                    info.Color = ConvertValidAlpha(info.Color, info.DefaultColor, info.IsValidAlpha);
                }
            }
        }

        /// <summary>
        /// 色を設定します。
        /// </summary>
        /// <param name="color">設定する色</param>
        public void SetColor(Color color)
        {
            UpdateBehavioursInfoIfNeeded();

            foreach (UIBehaviourInfo info in behavioursInfo)
            {
                // 単純な色変更はGraphic本体のみとし、
                // Shadowには適用しないようにする
                if (info.IsValid && info.Graphic != null)
                {
                    info.Color = ConvertValidAlpha(info.Color, color, info.IsValidAlpha);
                }
            }
        }

        /// <summary>
        /// 元の色に指定された色を加算合成して設定します。
        /// </summary>
        /// <param name="color">加算する色</param>
        public void SetColorAdditive(Color color)
        {
            UpdateBehavioursInfoIfNeeded();

            color = ConvertAdditiveColor(color);

            foreach (UIBehaviourInfo info in behavioursInfo)
            {
                if (info.IsValid)
                {
                    // 加算合成をする際に、Gradationが加算合成の場合は、
                    // 本体となるGraphicコンポーネントにのみ、色を適用する
                    if (info is GradationInfo)
                    {
                        if ((info as GradationInfo).IsAdditive)
                        {
                            info.Color = ConvertValidAlpha(info.Color, info.DefaultColor, info.IsValidAlpha);
                            continue;
                        }
                    }

                    var destinationColor = ColorAdditive(info.DefaultColor, color);
                    info.Color = ConvertValidAlpha(info.Color, destinationColor, info.IsValidAlpha);
                }
            }
        }

        /// <summary>
        /// 元の色に指定された色を加算合成して設定します。
        /// </summary>
        /// <param name="color">加算する色</param>
        /// <param name="alpha">設定するアルファ値</param>
        public void SetColorAdditive(Color color, float alpha)
        {
            UpdateBehavioursInfoIfNeeded();

            color = ConvertAdditiveColor(color);

            foreach (UIBehaviourInfo info in behavioursInfo)
            {
                if (info.IsValid)
                {
                    // 加算合成をする際に、Gradationが加算合成の場合は、
                    // 本体となるGraphicコンポーネントにのみ、色を適用する
                    if (info is GradationInfo)
                    {
                        if ((info as GradationInfo).IsAdditive)
                        {
                            info.Color = ConvertValidAlpha(info.Color, info.DefaultColor, info.IsValidAlpha);
                            continue;
                        }
                    }

                    var destinationColor = ColorAdditive(info.DefaultColor, color, alpha);
                    info.Color = ConvertValidAlpha(info.Color, destinationColor, info.IsValidAlpha);
                }
            }
        }

        /// <summary>
        /// 元の色に指定された色を乗算合成して設定します。
        /// </summary>
        /// <param name="color">乗算する色</param>
        public void SetColorMultiply(Color color)
        {
            UpdateBehavioursInfoIfNeeded();

            color = ConvertMultiplyColor(color);

            foreach (UIBehaviourInfo info in behavioursInfo)
            {
                if (info.IsValid)
                {
                    // 乗算合成をする際に、Gradationが乗算合成の場合は、
                    // 本体となるGraphicコンポーネントにのみ、色を適用する
                    if (info is GradationInfo)
                    {
                        if ((info as GradationInfo).IsMultiple)
                        {
                            info.Color = ConvertValidAlpha(info.Color, info.DefaultColor, info.IsValidAlpha);
                            continue;
                        }
                    }

                    var destinationColor = ColorMultiply(info.DefaultColor, color);
                    info.Color = ConvertValidAlpha(info.Color, destinationColor, info.IsValidAlpha);
                }
            }
        }

        /// <summary>
        /// 元の色に指定された色を乗算合成して設定します。
        /// </summary>
        /// <param name="color">乗算する色</param>
        /// <param name="alpha">設定するアルファ値</param>
        public void SetColorMultiply(Color color, float alpha)
        {
            UpdateBehavioursInfoIfNeeded();

            color = ConvertMultiplyColor(color);

            foreach (UIBehaviourInfo info in behavioursInfo)
            {
                if (info.IsValid)
                {
                    // 乗算合成をする際に、Gradationが乗算合成の場合は、
                    // 本体となるGraphicコンポーネントにのみ、色を適用する
                    if (info is GradationInfo)
                    {
                        if ((info as GradationInfo).IsMultiple)
                        {
                            info.Color = ConvertValidAlpha(info.Color, info.DefaultColor, info.IsValidAlpha);
                            continue;
                        }
                    }

                    var destinationColor = ColorMultiply(info.DefaultColor, color, alpha);
                    info.Color = ConvertValidAlpha(info.Color, destinationColor, info.IsValidAlpha);
                }
            }
        }

        /// <summary>
        /// Graphicコンポーネントを取得します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Graphic> GetGraphics()
        {
            UpdateBehavioursInfoIfNeeded();

            for (int i = 0; i < behavioursInfo.Count; ++i)
            {
                if (behavioursInfo[i].Graphic != null)
                {
                    yield return behavioursInfo[i].Graphic;
                }
            }
        }

        /// <summary>
        /// UIBehaviourInfo情報を取得します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UIBehaviourInfo> GetGraphicInfo()
        {
            UpdateBehavioursInfoIfNeeded();

            for (int i = 0; i < behavioursInfo.Count; ++i)
            {
                yield return behavioursInfo[i];
            }
        }

        /// <summary>
        /// 加算合成で実際に使用する色情報に変換します。
        /// </summary>
        /// <param name="color">加算合成する色</param>
        /// <returns></returns>
        public static Color ConvertAdditiveColor(Color color)
        {
            color.r = color.r * color.a;
            color.g = color.g * color.a;
            color.b = color.b * color.a;
            color.a = 0.0f;
            return color;
        }

        /// <summary>
        /// 乗算合成で実際に使用する色情報に変換します。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ConvertMultiplyColor(Color color)
        {
            color.r = (1.0f - color.r) * color.a;
            color.g = (1.0f - color.g) * color.a;
            color.b = (1.0f - color.b) * color.a;
            color.a = 0.0f;
            return color;
        }

        /// <summary>
        /// 加算合成した色を返します。
        /// </summary>
        /// <param name="src1">元の色</param>
        /// <param name="src2">アルファ値反映済みの合成色</param>
        /// <returns></returns>
        public static Color ColorAdditive(Color src1, Color src2)
        {
            Color dst = src1 + src2;
            dst.r = dst.r > 1.0f ? 1.0f : dst.r;
            dst.g = dst.g > 1.0f ? 1.0f : dst.g;
            dst.b = dst.b > 1.0f ? 1.0f : dst.b;
            return dst;
        }

        /// <summary>
        /// 加算合成した色を返します。
        /// </summary>
        /// <param name="src1">元の色</param>
        /// <param name="src2">アルファ値反映済みの合成色</param>
        /// <param name="alpha">設定するアルファ値</param>
        /// <returns></returns>
        public static Color ColorAdditive(Color src1, Color src2, float alpha)
        {
            Color dst = src1 + src2;
            dst.r = dst.r > 1.0f ? 1.0f : dst.r;
            dst.g = dst.g > 1.0f ? 1.0f : dst.g;
            dst.b = dst.b > 1.0f ? 1.0f : dst.b;
            dst.a = alpha;
            return dst;
        }

        /// <summary>
        /// 乗算合成した色を返します。
        /// </summary>
        /// <param name="src1">元の色</param>
        /// <param name="src2">アルファ値反映済みの合成色</param>
        /// <returns></returns>
        public static Color ColorMultiply(Color src1, Color src2)
        {
            Color dst = src1 - (src1 * src2);
            return dst;
        }

        /// <summary>
        /// 乗算合成した色を返します。
        /// </summary>
        /// <param name="src1">元の色</param>
        /// <param name="src2">アルファ値反映済みの合成色</param>
        /// <param name="alpha">設定するアルファ値</param>
        /// <returns></returns>
        public static Color ColorMultiply(Color src1, Color src2, float alpha)
        {
            Color dst = src1 - (src1 * src2);
            dst.a = alpha;
            return dst;
        }

#endregion

#region protected methods

        /// <summary>
        /// 色の変更を行わないGraphicコンポーネントを持つGameObject取得します。
        /// </summary>
        /// <param name="target">対象のゲームオブジェクト</param>
        /// <returns></returns>
        protected static void GetInvalidGraphics(
            GameObject target,
            List<GameObject> invalidObjects,
            List<GameObject> invalidAlphaObjects)
        {
            GraphicComponentGroup[] filters = target.GetComponentsInChildren<GraphicComponentGroup>(true);
            if (filters.Length == 0)
            {
                return;
            }

            List<Graphic> invalidGraphics = new List<Graphic>();
            foreach (GraphicComponentGroup filter in filters)
            {
                invalidGraphics.Clear();

                // 子オブジェクトが対象外の場合
                if (filter.IsInvalidChildren)
                {
                    filter.GetComponentsInChildren(true, invalidGraphics);

                    // 自身は対象となっている場合、除外するリストから自身を削除
                    if (!filter.IsInvalid)
                    {
                        for (int i = 0; i < invalidGraphics.Count; ++i)
                        {
                            if (filter.gameObject == invalidGraphics[i].gameObject)
                            {
                                invalidGraphics.RemoveAt(i);
                                break;
                            }
                        }
                    }

                    foreach (Graphic graphic in invalidGraphics)
                    {
                        if (!invalidObjects.Contains(graphic.gameObject))
                        {
                            invalidObjects.Add(graphic.gameObject);
                        }
                    }
                }

                // 自身が対象の場合
                else if (filter.IsInvalid)
                {
                    if (!invalidObjects.Contains(filter.gameObject))
                    {
                        invalidObjects.Add(filter.gameObject);
                    }
                }

                // アルファを操作可能にするか
                if (filter.IsInvalidControlAlpha && !invalidAlphaObjects.Contains(filter.gameObject))
                {
                    invalidAlphaObjects.Add(filter.gameObject);
                }
            }
        }

        /// <summary>
        /// BehavioursInfoに既に対象のコンポーネントが登録されているかを返します
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        protected bool IsExistsInfo(UIBehaviour component)
        {
            bool isExists = false;

            foreach (var behaviourInfo in behavioursInfo)
            {
                if (behaviourInfo is GraphicInfo && component is Graphic)
                {
                    var graphicInfo = behaviourInfo as GraphicInfo;
                    if (graphicInfo.Graphic == component)
                    {
                        isExists = true;
                        break;
                    }
                }

                if (behaviourInfo is ShadowInfo && component is Shadow)
                {
                    var shadowInfo = behaviourInfo as ShadowInfo;
                    if (shadowInfo.Shadow == component)
                    {
                        isExists = true;
                        break;
                    }
                }

                if (behaviourInfo is GradationInfo && component is IGradation)
                {
                    var gradationInfo = behaviourInfo as GradationInfo;
                    if (gradationInfo.Gradation == (IGradation)component)
                    {
                        isExists = true;
                    }
                }
            }

            return isExists;
        }

        /// <summary>
        /// 現在のフレームでHierarchyが更新されている場合、BehavioursInfoを更新します
        /// </summary>
        protected void UpdateBehavioursInfoIfNeeded()
        {
            if (hierarchyChangeDetector != null && hierarchyChangeDetector.IsHierarchyDirty)
            {
                FindGraphicComponents(hierarchyChangeDetector.gameObject, cachedValidOwn);
            }
        }

        /// <summary>
        /// Alphaが操作可能かどうかに応じてColorを調整します
        /// </summary>
        /// <param name="color"></param>
        /// <param name="setColor"></param>
        /// <param name="validAlpha"></param>
        /// <returns></returns>
        protected virtual Color ConvertValidAlpha(Color color, Color setColor, bool validAlpha)
        {
            if (!validAlpha)
            {
                setColor.a = color.a;
            }

            color = setColor;
            return color;
        }

#endregion
    }
}
