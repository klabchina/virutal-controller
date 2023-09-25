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
using System.Collections.Generic;
using Jigbox.UIControl;

namespace Jigbox.Components
{
    [DisallowMultipleComponent]
    public class GraphicGroup : GraphicComponentGroup
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 色の設定状態
        /// </summary>
        protected enum ColorStatus
        {
            /// <summary>なし</summary>
            None = 0,
            /// <summary>加算</summary>
            Additive = 1,
            /// <summary>乗算</summary>
            Multiply = 1 << 1,
            /// <summary>通過</summary>
            Through = 1 << 31,
        }

#endregion

#region properties

        /// <summary>親のグループからの影響を無視するかどうか</summary>
        [SerializeField]
        protected bool ignoreParentGroup = false;
        
        /// <summary>コンポーネント配下のGraphic情報</summary>
        protected GraphicComponentInfo graphicInfo = new GraphicComponentInfo();

        /// <summary>コンポーネント配下のGraphic情報</summary>
        public GraphicComponentInfo GraphicInfo { get { return graphicInfo; } }

        /// <summary>現在設定されている色</summary>
        protected Color currentColor = Color.white;

        /// <summary>現在のアルファ値</summary>
        protected float currentAlpha = 1.0f;
        
        /// <summary>現在の色の設定状態</summary>
        protected int colorStatus = (int) ColorStatus.None;

        /// <summary>親グループから影響を受ける色</summary>
        protected Color affectColor = Color.white;

        /// <summary>親グループから影響を受けるアルファ値</summary>
        protected float affectAlpha = 1.0f;

        /// <summary>親グループからの影響状態</summary>
        protected int affectColorStatus = (int) ColorStatus.None;

        /// <summary>親のGraphicGroup</summary>
        protected GraphicGroup parent;

        /// <summary>子のGraphicGroup</summary>
        protected List<GraphicGroup> children = new List<GraphicGroup>();

        /// <summary>
        /// <para>自身のコンポーネントを対象としないかどうか</para>
        /// <para>基本的にGraphic情報を取得する際の内部処理にのみ使用します。</para>
        /// </summary>
        protected bool isInvalid = false;

        /// <summary>自身のコンポーネントを対象としないかどうか</summary>
        public override bool IsInvalid { get { return isInvalid; } }

        /// <summary>子オブジェクトのコンポーネントを対象としないかどうか</returns>
        public override bool IsInvalidChildren { get { return isInvalid; } }

        /// <summary>親から影響を受けるかどうか</summary>
        protected bool IsAffectParent { get { return parent != null && !ignoreParentGroup; } }

#endregion

#region public methods

        /// <summary>
        /// 色を元に戻します。
        /// </summary>
        public void ResetColor()
        {
            int status = (int) ColorStatus.None;
            if (IsDifference(status, Color.white, currentAlpha))
            {
                currentColor = Color.white;
                currentAlpha = 1.0f;
                colorStatus = status;
            }
            else
            {
                return;
            }
            
            if (!IsAffectParent)
            {
                graphicInfo.ResetColor();
            }
            else
            {
                if (parent != null && parent.colorStatus == (int) ColorStatus.Through)
                {
                    GraphicGroup parentGroup = GetParentGroupNotThrough();

                    affectColorStatus = parentGroup != null ? parentGroup.colorStatus : (int) ColorStatus.None;
                    affectColor = parentGroup != null ? parentGroup.currentColor : Color.white;
                    affectAlpha = parentGroup != null ? parentGroup.currentAlpha : 1.0f;
                }

                switch (affectColorStatus)
                {
                    case (int) ColorStatus.None:
                        graphicInfo.ResetColor();
                        break;
                    case (int) ColorStatus.Additive:
                        graphicInfo.SetColorAdditive(affectColor, affectAlpha);
                        break;
                    case (int) ColorStatus.Multiply:
                        graphicInfo.SetColorMultiply(affectColor, affectAlpha);
                        break;
                }
            }

            AffectColorToChildren(currentColor, currentAlpha);
        }

        /// <summary>
        /// 元の色に指定された色を加算合成して設定します。
        /// </summary>
        /// <param name="color">加算する色</param>
        public void SetColorAdditive(Color color)
        {
            int status = (int) ColorStatus.Additive;
            if (IsDifference(status, color, currentAlpha))
            {
                currentColor = color;
                colorStatus = status;
            }
            else
            {
                return;
            }

            if (IsAffectParent)
            {
                // 親からの影響を計算してから設定
                color = MergeColor(color, affectColor, affectColorStatus);
                float alpha = currentAlpha * affectAlpha;
                graphicInfo.SetColorAdditive(color, alpha);
                AffectColorToChildren(color, alpha);
            }
            else
            {
                graphicInfo.SetColorAdditive(color, currentAlpha);
                AffectColorToChildren(color, currentAlpha);
            }
        }

        /// <summary>
        /// 元の色に指定された色を加算合成して設定します。
        /// </summary>
        /// <param name="color">加算する色</param>
        /// <param name="alpha">設定するアルファ値</param>
        public void SetColorAdditive(Color color, float alpha)
        {
            int status = (int) ColorStatus.Additive;
            if (IsDifference(status, color, alpha))
            {
                currentColor = color;
                currentAlpha = alpha;
                colorStatus = status;
            }
            else
            {
                return;
            }
            
            if (IsAffectParent)
            {
                // 親からの影響を計算してから設定
                color = MergeColor(color, affectColor, affectColorStatus);
                alpha = currentAlpha * affectAlpha;
                graphicInfo.SetColorAdditive(color, alpha);
                AffectColorToChildren(color, alpha);
            }
            else
            {
                graphicInfo.SetColorAdditive(color, currentAlpha);
                AffectColorToChildren(color, currentAlpha);
            }
        }

        /// <summary>
        /// 元の色に指定された色を乗算合成して設定します。
        /// </summary>
        /// <param name="color">乗算する色</param>
        public void SetColorMultiply(Color color)
        {
            int status = (int) ColorStatus.Multiply;
            if (IsDifference(status, color, currentAlpha))
            {
                currentColor = color;
                colorStatus = status;
            }
            else
            {
                return;
            }

            if (IsAffectParent)
            {
                // 親からの影響を計算してから設定
                color = MergeColor(color, affectColor, affectColorStatus);
                float alpha = currentAlpha * affectAlpha;
                graphicInfo.SetColorMultiply(color, alpha);
                AffectColorToChildren(color, alpha);
            }
            else
            {
                graphicInfo.SetColorMultiply(color, currentAlpha);
                AffectColorToChildren(color, currentAlpha);
            }
        }

        /// <summary>
        /// 元の色に指定された色を乗算合成して設定します。
        /// </summary>
        /// <param name="color">乗算する色</param>
        /// <param name="alpha">設定するアルファ値</param>
        public void SetColorMultiply(Color color, float alpha)
        {
            int status = (int) ColorStatus.Multiply;
            if (IsDifference(status, color, alpha))
            {
                currentColor = color;
                currentAlpha = alpha;
                colorStatus = status;
            }
            else
            {
                return;
            }

            if (IsAffectParent)
            {
                // 親からの影響を計算してから設定
                color = MergeColor(color, affectColor, affectColorStatus);
                alpha = currentAlpha * affectAlpha;
                graphicInfo.SetColorMultiply(color, alpha);
                AffectColorToChildren(color, alpha);
            }
            else
            {
                graphicInfo.SetColorMultiply(color, currentAlpha);
                AffectColorToChildren(color, currentAlpha);
            }
        }

        /// <summary>
        /// 自身の設定で合成せず、親の設定を通過させて合成を行います。
        /// </summary>
        public void SetThrough()
        {
            // ルートのグループは通過状態には設定できない
            if (parent == null)
            {
                return;
            }

            int status = (int) ColorStatus.Through;
            if (colorStatus != status)
            {
                currentColor = Color.white;
                currentAlpha = 1.0f;
                colorStatus = status;
            }
            else
            {
                return;
            }

            if (IsAffectParent)
            {
                // 通過設定になっていない親まで遡って状態を取得
                GraphicGroup parentGroup = GetParentGroupNotThrough();

                if (parentGroup != null)
                {
                    // 親の親から更に影響がある場合はそれを含めた色を取得
                    if (parentGroup.IsAffectParent)
                    {
                        affectColorStatus = parentGroup.colorStatus;
                        affectColor = MergeColor(parentGroup.currentColor, parentGroup.affectColor, parent.affectColorStatus);
                        affectAlpha = parentGroup.currentAlpha * parentGroup.affectAlpha;
                    }
                    else
                    {
                        affectColorStatus = parentGroup.colorStatus;
                        affectColor = parentGroup.currentColor;
                        affectAlpha = parentGroup.currentAlpha;
                    }
                }
                else
                {
                    affectColorStatus = (int) ColorStatus.None;
                    affectColor = Color.white;
                    affectAlpha = 1.0f;
                }

                switch (affectColorStatus)
                {
                    case (int) ColorStatus.None:
                    case (int) ColorStatus.Through:
                        graphicInfo.ResetColor();
                        break;
                    case (int) ColorStatus.Additive:
                        graphicInfo.SetColorAdditive(affectColor, affectAlpha);
                        break;
                    case (int) ColorStatus.Multiply:
                        graphicInfo.SetColorMultiply(affectColor, affectAlpha);
                        break;
                }

                AffectColorToChildren(affectColor, affectAlpha);
            }
            else
            {
                graphicInfo.ResetColor();
                AffectColorToChildren(currentColor, currentAlpha);
            }
        }

        /// <summary>
        /// 子のGraphicGroup全ての色の状態を通過状態に設定します。
        /// </summary>
        public void SetThroughInChildren()
        {
            foreach (GraphicGroup child in children)
            {
                child.SetThrough();
                child.SetThroughInChildren();
            }
        }

        /// <summary>
        /// グループ内に含まれるGraphic情報を探索して取得します。
        /// </summary>
        public void FindGraphicComponents()
        {
            FindGraphicComponents(this);
        }

        /// <summary>
        /// 自身以下の子を含めたGraphicGroupに含まれるGraphic情報を探索して取得します。
        /// </summary>
        public void FindGraphicComponentsInChildren()
        {
            List<GraphicGroup> groups = GetAllGroup(this);
            
            // それぞれのグループに紐づくGraphic情報が正しく設定されるようにするために
            // 全ての親子関係が明確化してから情報を設定する
            foreach (GraphicGroup group in groups)
            {
                FindGraphicComponents(group);
            }
        }

#endregion

#region protected methods

        /// <summary>
        /// 親からの色の影響を反映させます。
        /// </summary>
        /// <param name="color">親の色(色の影響反映済み)</param>
        protected virtual void Affect(Color color, float alpha)
        {
            int status = parent.colorStatus;
            if (status == (int) ColorStatus.Through)
            {
                // 自身が透過設定になっている場合、状態が透過でない親まで遡って状態を設定する
                GraphicGroup parentGroup = GetParentGroupNotThrough();
                status = parentGroup != null ? parentGroup.colorStatus : (int) ColorStatus.None;
            }

            if (IsDifferenceAffect(status, color, alpha))
            {
                affectColor = color;
                affectAlpha = alpha;
                affectColorStatus = status;
            }
            else
            {
                return;
            }

            Color affectedColor = MergeColor(currentColor, affectColor, affectColorStatus);
            float affectedAlpha = currentAlpha * affectAlpha;

            // 親からの色影響を反映した上で、
            // 自身の色の状態に合わせて、色を再設定する
            switch (colorStatus)
            {
                case (int) ColorStatus.None:
                case (int) ColorStatus.Through:
                    // 自身の状態が合成なしの場合、
                    // 親の状態に合わせて、色の再設定方法を変更
                    switch (affectColorStatus)
                    {
                        case (int) ColorStatus.None:
                        case (int) ColorStatus.Through:
                            graphicInfo.ResetColor();
                            break;
                        case (int) ColorStatus.Additive:
                            graphicInfo.SetColorAdditive(affectColor, affectAlpha);
                            break;
                        case (int) ColorStatus.Multiply:
                            graphicInfo.SetColorMultiply(affectColor, affectAlpha);
                            break;
                    }
                    break;
                case (int) ColorStatus.Additive:
                    graphicInfo.SetColorAdditive(affectedColor, affectedAlpha);
                    break;
                case (int) ColorStatus.Multiply:
                    graphicInfo.SetColorMultiply(affectedColor, affectedAlpha);
                    break;
            }

            if (colorStatus != (int) ColorStatus.Through)
            {
                AffectColorToChildren(affectedColor, affectedAlpha);
            }
            else
            {
                AffectColorToChildren(affectColor, affectAlpha);
            }
        }

        /// <summary>
        /// 色の設定状態に差異があるかどうかを返します。
        /// </summary>
        /// <param name="status">設定する状態</param>
        /// <param name="color">設定する色</param>
        /// <returns></returns>
        protected virtual bool IsDifference(int status, Color color, float alpha)
        {
            return colorStatus != status || currentColor != color || currentAlpha != alpha;
        }
        
        /// <summary>
        /// 親から影響を受ける色の状態に差異があるかどうかを返します。
        /// </summary>
        /// <param name="status">設定する状態</param>
        /// <param name="color">設定する色</param>
        /// <returns></returns>
        protected virtual bool IsDifferenceAffect(int status, Color color, float alpha)
        {
            return affectColorStatus != status || affectColor != color || affectAlpha != alpha;
        }

        /// <summary>
        /// 親のGraphicGroupを探索します。
        /// </summary>
        protected virtual void SearchParentGroup()
        {
            // 親グループの探索は、GetComponentInParentが有効とするのが
            // アクティブなゲームオブジェクトのみであるため、
            // Transformをたどる形で実行して確実にコンポーネントを取得する
            // 基本はuGUIコンポーネントを前提としたコンポーネントのため、
            // RectTransformにキャストできない場合は、uGUIに関係しないオブジェクトと判断し、
            // その時点で検索を中断する
            RectTransform parent = transform.parent as RectTransform;
            while (parent != null)
            {
                GraphicGroup graphicGroup = parent.GetComponent<GraphicGroup>();
                if (graphicGroup != null)
                {
                    this.parent = graphicGroup;
                    break;
                }
                parent = parent.parent as RectTransform;
            }

            if (this.parent != null)
            {
                this.parent.children.Add(this);
            }
        }

        /// <summary>
        /// 色の状態を子に対して影響させます。
        /// </summary>
        /// <param name="color">影響させる色</param>
        /// <param name="alpha">影響させるアルファ値</param>
        protected virtual void AffectColorToChildren(Color color, float alpha)
        {
            if (children.Count == 0)
            {
                return;
            }

            foreach (GraphicGroup child in children)
            {
                if (child.ignoreParentGroup)
                {
                    continue;
                }

                child.Affect(color, alpha);
            }
        }

        /// <summary>
        /// 色を合成します。
        /// </summary>
        /// <param name="src1">元となる色</param>
        /// <param name="src2">合成する色</param>
        /// <param name="status">合成する色の状態</param>
        /// <returns></returns>
        protected virtual Color MergeColor(Color src1, Color src2, int status)
        {
            switch (status)
            {
                case (int) ColorStatus.None:
                    return src1;
                case (int) ColorStatus.Additive:
                    return GraphicComponentInfo.ColorAdditive(src1, GraphicComponentInfo.ConvertAdditiveColor(src2));
                case (int) ColorStatus.Multiply:
                    return GraphicComponentInfo.ColorMultiply(src1, GraphicComponentInfo.ConvertMultiplyColor(src2));
            }
            return src1;
        }

        /// <summary>
        /// 通過状態ではない親のGraphicGroupを取得します。
        /// </summary>
        /// <returns></returns>
        protected GraphicGroup GetParentGroupNotThrough()
        {
            GraphicGroup parentGroup = parent;
            while (parentGroup != null && parentGroup.colorStatus == (int) ColorStatus.Through)
            {
                if (parentGroup.IsAffectParent)
                {
                    parentGroup = parentGroup.parent;
                }
                else
                {
                    break;
                }
            }

            return parentGroup;
        }

        /// <summary>
        /// 対照グループに含まれるGraphic情報を探索して取得します。
        /// </summary>
        /// <param name="group">探索対象のGraphicGroup</param>
        protected static void FindGraphicComponents(GraphicGroup group)
        {
            // 情報を取得する際に子グループの情報取得時に参照されるフラグ情報を変更する
            // これによって、子グループ以下のGraphic情報が親グループから除外される
            foreach (GraphicGroup child in group.children)
            {
                child.isInvalid = true;
            }

            group.graphicInfo.FindGraphicComponents(group.gameObject);

            // フラグを戻す
            foreach (GraphicGroup child in group.children)
            {
                child.isInvalid = false;
            }
        }

        /// <summary>
        /// グループ内に含まれる全てのGraphicGroupを取得します。
        /// </summary>
        /// <param name="group">GraphicGroup</param>
        /// <returns></returns>
        protected static List<GraphicGroup> GetAllGroup(GraphicGroup group)
        {
            List<GraphicGroup> groups = new List<GraphicGroup>();
            groups.Add(group);

            foreach (GraphicGroup child in group.children)
            {
                List<GraphicGroup> childGroups = GetAllGroup(child);
                groups.AddRange(childGroups);
            }

            return groups;
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            // 親が設定済みの場合、初期化されているものとして以降の処理はしない
            if (parent != null)
            {
                return;
            }

            SearchParentGroup();

            // 親がいない場合、グループのルートして子を含めて初期化を実行
            if (parent == null)
            {
                GraphicGroup[] groups = GetComponentsInChildren<GraphicGroup>(true);
                foreach (GraphicGroup group in groups)
                {
                    // 自身に親が存在していないのは確定なので処理しない
                    if (group == this)
                    {
                        continue;
                    }

                    if (group.parent == null)
                    {
                        group.SearchParentGroup();
                    }
                }

                // それぞれのグループに紐づくGraphic情報が正しく設定されるようにするために
                // 全ての親子関係が明確化してから情報を設定する
                foreach (GraphicGroup group in groups)
                {
                    FindGraphicComponents(group);
                }
            }
        }
        
        protected virtual void OnDestroy()
        {
            if (parent != null)
            {
                parent.children.Remove(this);
            }
        }

#endregion
    }
}
