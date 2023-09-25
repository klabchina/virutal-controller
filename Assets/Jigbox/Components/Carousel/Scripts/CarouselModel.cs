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

namespace Jigbox.Components
{
    public class CarouselModel
    {
#region properties

        // Carouselの参照を直接持たせると何でもありクラスになって
        // モデルとして破綻するのでデータクラスを一枚噛ませる
        protected CarouselModelProperty property;

#endregion

#region public methods

        public CarouselModel(CarouselModelProperty property)
        {
            this.property = property;
        }

        /// <summary>
        /// Viewが生成されたかどうかを返します(必要であればviewを生成します)
        /// </summary>
        /// <param name="view"></param>
        /// <returns>新しくviewを生成した場合はtrue、引数のviewの参照がある場合はfalseを返します</returns>
        public bool CreateView(ref CarouselViewBase view)
        {
            if (view != null)
            {
                return false;
            }

            // viewがnullもしくはTypeが異なる場合はviewを再生成する
            switch (property.Axis)
            {
                case GridLayoutGroup.Axis.Horizontal:
                    if (property.IsLoop)
                    {
                        view = new CarouselViewHorizontalLoop();
                    }
                    else
                    {
                        view = new CarouselViewHorizontal();
                    }
                    break;

                case GridLayoutGroup.Axis.Vertical:
                    if (property.IsLoop)
                    {
                        view = new CarouselViewVerticalLoop();
                    }
                    else
                    {
                        view = new CarouselViewVertical();
                    }
                    break;

                default:
                    throw new ArgumentException(property.Axis + " is not defined.");
            }

            return true;
        }

        /// <summary>
        /// スタートさせるCellのindexを返します
        /// </summary>
        /// <returns></returns>
        public int GetStartCellIndex()
        {
            int startIndex = 0;
            int cellCount = property.CellCount;

            // CurrentIndexの決定とCellの数が奇数か偶数かによってContentの初期位置をずらす
            if (cellCount > 0)
            {
                startIndex = Mathf.FloorToInt(cellCount / 2);
                if (cellCount % 2 == 0)
                {
                    // 偶数の場合は中央2つのうちHorizontalは左側、Verticalは上側の方に寄せているのでtempIndexをひとつずらしておく
                    startIndex--;
                }
            }
            return startIndex;
        }

        /// <summary>
        /// 現在の表示中のindexから指定したindexに移動するにあたり、移動距離が近いindexの差分を返します(Loop指定されている場合はLoopも考慮されます)
        /// </summary>
        /// <param name="toIndex">移動したいindex</param>
        /// <returns>移動距離が近いindexの差分</returns>
        public int GetNearOffsetIndex(int toIndex)
        {
            int fromIndex = property.ShowIndex;
            int cellCount = property.CellCount;

            if (cellCount <= 0)
            {
                // Cellの個数が0以下の場合はoffset値を算出することができないため0で返す
                return 0;
            }

            if (property.IsLoop)
            {
                var prevOffsetIndex = (toIndex - cellCount - fromIndex) % cellCount;
                var nextOffsetIndex = (toIndex + cellCount - fromIndex) % cellCount;

                // offsetの値が近いほうを返す
                return Mathf.Abs(prevOffsetIndex) < nextOffsetIndex ? prevOffsetIndex : nextOffsetIndex;
            }

            // Loopモードではない場合はfromIndexとtoIndexの方向を見比べるだけ
            return toIndex - fromIndex;
        }

        /// <summary>
        /// Indexの値を適正範囲でバリデーションして返します
        /// </summary>
        /// <param name="index">移動したいindex</param>
        /// <returns>バリデーションしたあとのindex</returns>
        public int ValidateIndex(int index)
        {
            int cellCount = property.CellCount;
            if (cellCount <= 0)
            {
                return 0;
            }

            // 何周ループしていても問題ないように正規化する
            // 例えばcellCount=3の場合、1, 4, 7は同じ扱いで良い
            int normalizedIndex = index % cellCount;

            // C#では-2 % 3の結果は-2になる(他の言語では1になるものもある)
            // ここでは、1が欲しいのでcellCountを加算することで補正する
            // これで-5, -2, 1, 4, 7は同じ扱いになる
            if (normalizedIndex < 0)
            {
                normalizedIndex += cellCount;
            }

            return normalizedIndex;
        }

#endregion
    }
}
