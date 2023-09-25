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
using UnityEngine;

namespace Jigbox.Components
{
    [Serializable]
    public class ScrollSelectModelHorizontal : ScrollSelectModelBase
    {
#region Static Constants

        /// <summary>
        /// Viewportのデフォルトのpivot
        /// </summary>
        public static readonly Vector2 DefaultViewPivot = new Vector2(0.0f, 0.5f);

#endregion

#region Fields & Properties

        /// <summary>Viewportのpivot</summary>
        [SerializeField]
        Vector2 viewPivot = DefaultViewPivot;

        /// <summary>Viewportのpivot</summary>
        public override Vector2 ViewPivot
        {
            get { return viewPivot; }
            set { viewPivot = value; }
        }

        /// <summary>Pivotに合わせてセルをずらす割合</summary>
        protected override float CellPositionOffsetRate { get { return CellPivot.x - 0.5f; } }

        /// <summary>可視領域の長さ</summary>
        public override float ViewportLength { get { return ViewportSize.x; } }

        /// <summary>スクロール量</summary>
        protected override float ScrollValue { get { return ScrollPosition.x; } }

        /// <summary>選択セルの現在位置と選択状態の基準位置の差の大きさ</summary>
        public override float SelectedCellDelta
        {
            get
            {
                return selectedCellPositionDelta.x;
            }
            protected set
            {
                selectedCellPositionDelta.x = value;
            }
        }
        
        /// <summary>
        /// 選択セルをAdjustするときに使用する選択セルとの距離を返します
        /// </summary>
        public override Vector2 SelectedCellPositionDeltaForAdjust
        {
            get { return -selectedCellPositionDelta; }
        }

#endregion

        /// <summary>
        /// LoopType次第で変わる処理を移譲するためのインスタンスを返します
        /// </summary>
        /// <returns></returns>
        protected override IScrollSelectLoopTypeLogic GetLoopTypeLogic()
        {
            if (LoopType == ScrollSelectViewBase.ScrollSelectLoopType.Loop)
            {
                return new ScrollSelectHorizontalLoopLogic(new ScrollSelectLoopTypeLogicProperty(this));
            }
            else
            {
                return new ScrollSelectHorizontalNoneLoopLogic(new ScrollSelectLoopTypeLogicProperty(this));
            }
        }

        /// <summary>
        /// 現在選択中のセルに合わせて、位置を更新します。
        /// </summary>
        /// <returns>更新後の位置</returns>
        public override Vector3 MoveToSelectedCell()
        {
            Vector3 position = LoopTypeLogic.CalculateJumpPositionBySelectedIndex(SelectedCellIndex);
            ScrollPosition = new Vector2(-position.x, 0.0f);
            SelectedCellDelta = 0.0f;
            return position;
        }

        /// <summary>
        /// 無限スクロール状態で端に到達しないようにするためにスクロール位置を調整します。
        /// </summary>
        /// <param name="scrollPosition">スクロール位置</param>
        public override void SlideScrollPosition(Vector2 scrollPosition)
        {
            // Horizontalの場合contentのpositionとScrollPositionの関係は正負が逆になるためマイナスをかける
            ScrollPosition = new Vector2(-ScrollOriginPoint.x, 0f) + ScrollPosition - scrollPosition;
        }

        /// <summary>
        /// スクロールセレクトビューでは指定された位置にあるセルのPositionIndexを返します
        /// ユーザに渡すためのIndexでないことに気をつけてください
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override int CellIndex(Vector2 position)
        {
            return CellIndex(position.x);
        }
    }
}
