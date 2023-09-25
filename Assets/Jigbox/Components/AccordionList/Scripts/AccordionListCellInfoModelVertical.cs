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

namespace Jigbox.Components
{
    /// <summary>
    /// AccordionListCellInfoをVerticalで利用するクラス
    /// </summary>
    public class AccordionListCellInfoModelVertical : AccordionListCellInfoModel
    {
#region override methods

        /// <summary>
        /// セル情報の生成
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="padding">Padding</param>
        /// <param name="isFirstNode">兄弟ノードの中で最初か</param>
        /// <param name="isLastNode">兄弟ノードの中で最後か</param>
        /// <returns>セル情報</returns>
        protected override AccordionListCellInfo CreateMainCellInfo(AccordionListNode node, Padding padding, bool isFirstNode, bool isLastNode)
        {
            var expandSpacingFront = isFirstNode ? padding.Top : node.SpacingFront;
            var expandSpacingBack = node.ChildAreaPadding.Top;
            var collapseSpacingFront = expandSpacingFront;
            // オプショナルセルがbackを持っているのでここでは0を指定している
            var collapseSpacingBack = 0.0f;

            var margin = node.Margin;
            var cellPrefab = node.MainCellPrefab;

            return new AccordionListCellInfo(node, AccordionListCellType.Main, cellPrefab, cellPrefab.CellSize, cellPrefab.IsVariable,
                expandSpacingFront, expandSpacingBack, collapseSpacingFront, collapseSpacingBack,
                padding, margin);
        }

        /// <summary>
        /// オプショナルセル情報の生成
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="padding">左Padding</param>
        /// <param name="isFirstNode">兄弟ノードの中で最初か</param>
        /// <param name="isLastNode">兄弟ノードの中で最後か</param>
        /// <returns></returns>
        protected override AccordionListCellInfo CreateOptionalCellInfo(AccordionListNode node, Padding padding, bool isFirstNode, bool isLastNode)
        {
            var expandSpacingFront = node.ChildAreaPadding.Bottom;
            var expandSpacingBack = isLastNode ? padding.Bottom : node.SpacingBack;
            // メインセルがfrontを持っているのでここでは0を指定している
            var collapseSpacingFront = 0;
            var collapseSpacingBack = expandSpacingBack;

            if (node.HasOptionalCell)
            {
                var margin = node.OptionalCellMargin;
                var cellPrefab = node.OptionalCellPrefab;

                return new AccordionListCellInfo(node, AccordionListCellType.Optional, cellPrefab, cellPrefab.CellSize, cellPrefab.IsVariable,
                    expandSpacingFront, expandSpacingBack, collapseSpacingFront, collapseSpacingBack,
                    padding, margin);
            }
            else
            {
                return new AccordionListCellInfo(node, AccordionListCellType.Optional, null, 0, false,
                    expandSpacingFront, expandSpacingBack, collapseSpacingFront, collapseSpacingBack,
                    Padding.zero, Padding.zero);
            }
        }

        /// <summary>
        /// チャイルドエリアセル情報を作成する
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="padding">Padding</param>
        /// <returns>セル情報</returns>
        protected override AccordionListCellInfo CreateChildAreaCellInfo(AccordionListNode node, Padding padding)
        {
            return new AccordionListCellInfo(node, AccordionListCellType.ChildArea, node.ChildAreaCellPrefab, 0, false, 0, 0, 0, 0, padding, Padding.zero);
        }

        /// <summary>
        /// 引数のセル情報の子ノードに適用するPaddingを生成する
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <returns>Padding</returns>
        protected override Padding CreateChildrenPadding(AccordionListCellInfo cellInfo)
        {
            return new Padding(cellInfo.Padding.Left + cellInfo.Node.ChildAreaPadding.Left,
                cellInfo.Padding.Right + cellInfo.Node.ChildAreaPadding.Right,
                cellInfo.Node.ChildAreaPadding.Top,
                cellInfo.Node.ChildAreaPadding.Bottom);
        }

        /// <summary>
        /// セルの末端を返す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <param name="withSpacing">Spacingを含めるか</param>
        /// <returns>座標</returns>
        public override Vector2 CellFrontPosition(AccordionListCellInfo cellInfo, bool withSpacing)
        {
            var result = cellInfo.CellPosition.y + cellInfo.Size / 2;
            if (withSpacing)
            {
                result = result + cellInfo.CurrentSpacingFront;
            }

            return new Vector2(0, result);
        }

        /// <summary>
        /// セルの末端を返す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <param name="withSpacing">Spacingを含めるか</param>
        /// <returns>座標</returns>
        public override Vector2 CellBackPosition(AccordionListCellInfo cellInfo, bool withSpacing)
        {
            var result = cellInfo.CellPosition.y - cellInfo.Size / 2;
            if (withSpacing)
            {
                result = result - cellInfo.CurrentSpacingBack;
            }

            return new Vector2(0, result);
        }

        /// <summary>
        /// セルの上端を返す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <param name="withSpacing">Spacingを含めるか</param>
        /// <returns>値</returns>
        public override float SimplifyCellFrontPosition(AccordionListCellInfo cellInfo, bool withSpacing)
        {
            return CellFrontPosition(cellInfo, withSpacing).y;
        }

        /// <summary>
        /// セルの末端を返す
        /// </summary>
        /// <param name="cellInfo">セル情報</param>
        /// <param name="withSpacing">Spacingを含めるか</param>
        /// <returns>値</returns>
        public override float SimplifyCellBackPosition(AccordionListCellInfo cellInfo, bool withSpacing)
        {
            return CellBackPosition(cellInfo, withSpacing).y;
        }

        /// <summary>
        /// 全てのセルの座標を計算する
        /// </summary>
        /// <param name="padding">AccordionListの外周余白</param>
        /// <param name="cellPivot">セルPivot</param>
        public override void CalculateCellPosition(Padding padding, Vector2 cellPivot)
        {
            if (CellInfos.Count == 0)
            {
                return;
            }

            // 最初のセルだけループ外で設定
            var cellInfo = CellInfos[0];
            var prevSize = cellInfo.Size;
            var y = padding.Top + prevSize * (1.0f - cellPivot.y);
            var x = (padding.Left + cellInfo.SpacingLeft) * (1.0f - cellPivot.x) - (padding.Right + cellInfo.SpacingRight) * cellPivot.x;
            cellInfo.CellPosition = new Vector2(x, -y);
            var spacing = cellInfo.CurrentSpacingBack;

            for (var i = 1; i <= CellInfos.Count - 1; i++)
            {
                cellInfo = CellInfos[i];

                var spacingFront = cellInfo.CurrentSpacingFront;
                if (spacingFront > spacing)
                {
                    spacing = spacingFront;
                }

                var currentSize = cellInfo.Size;
                y += prevSize * (1.0f - cellPivot.y) + currentSize * cellPivot.y + spacing;
                x = (padding.Left + cellInfo.SpacingLeft) * (1.0f - cellPivot.x) - (padding.Right + cellInfo.SpacingRight) * cellPivot.x;
                cellInfo.CellPosition = new Vector2(x, -y);
                spacing = cellInfo.CurrentSpacingBack;
                prevSize = currentSize;
            }

            // チャイルドエリアセルの計算
            foreach (var childAreaCellInfo in ChildAreaCellInfos)
            {
                var main = FindCellInfo(childAreaCellInfo.Node.Id, AccordionListCellType.Main);
                childAreaCellInfo.Size = ChildAreaSize(main);
                x = (padding.Left + childAreaCellInfo.SpacingLeft) * (1.0f - cellPivot.x) - (padding.Right + childAreaCellInfo.SpacingRight) * cellPivot.x;
                childAreaCellInfo.CellPosition = new Vector2(x, SimplifyCellBackPosition(main, false) - childAreaCellInfo.Size * cellPivot.y);
            }
        }

#endregion
    }
}
