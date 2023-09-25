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

namespace Jigbox.Components
{
    /// <summary>
    /// Carousel用のViewのベースとなる抽象クラス
    /// </summary>
    public abstract class CarouselViewBase
    {
#region fields

        /// <summary>
        /// indexのoffset値
        /// </summary>
        protected int offsetIndex = 0;

        /// <summary>
        /// CarouselViewProperty
        /// </summary>
        protected CarouselViewProperty property;

        /// <summary>
        /// Contentのスタート位置(Cellの個数が偶数の場合は補正をしなければいけないため)
        /// </summary>
        protected Vector3 contentBasePos;

        /// <summary>
        /// ドラッグの一時計算保持用
        /// </summary>
        protected float tempDelta;

        /// <summary>
        /// ドラッグの開始位置保持用
        /// </summary>
        protected float startTempDelta;

#endregion

#region properties

        /// <summary>
        /// ContentのLocalPositionを取得します
        /// </summary>
        public virtual Vector3 ContentLocalPosition
        {
            get
            {
                return this.property.ContentLocalPosition;
            }
        }

        /// <summary>
        /// ContentのBasePositionを取得・設定します
        /// </summary>
        public virtual Vector3 ContentBasePosition
        {
            get
            {
                return this.contentBasePos;
            }
            protected set
            {
                this.contentBasePos = value;
                if (this.property.Layout != null)
                {
                    this.property.Layout.transform.localPosition = contentBasePos;
                }
            }
        }

#endregion

#region public methods

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="property">CarouselのViewで扱うプロパティの参照クラス</param>
        /// <param name="cellCount">Cellの数</param>
        public virtual void Initialize(CarouselViewProperty property)
        {
            this.property = property;
            this.offsetIndex = 0;
            this.tempDelta = 0.0f;
            this.startTempDelta = 0.0f;

            ContentBasePosition = CalculateContentBasePosition(property.CellCount);
        }

        /// <summary>
        /// tempDeltaとcontentBasePosの値を加味したContentのLocalPositionを返します
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 CalculateNewContentLocalPosition();

        /// <summary>
        /// Cellを引数分ずらす時のontentの移動量を返します
        /// </summary>
        /// <param name="offset">ずらしたいCellの個数</param>
        /// <returns>移動量(Vector3)</returns>
        public abstract Vector3 GetAmountOfMovement(int offset);

        /// <summary>
        /// セルの半分のサイズを返します
        /// </summary>
        /// <param name="size">1つのコンテンツ(Cell)のSize</param>
        /// <returns></returns>
        public virtual float GetHalfCellSize(Vector2 size)
        {
            var value = GetValueByAxis(size);
            if (value == 0)
            {
                return value;
            }

            return value / 2;
        }

        /// <summary>
        /// Axisで設定されている情報を基にしてVector2のxもしくはyのどちらかの値を返します
        /// </summary>
        /// <remarks>Axisが取得できない(layoutの情報がない)場合には0を返します</remarks>
        /// <param name="vec"></param>
        /// <returns></returns>
        public virtual float GetValueByAxis(Vector2 vec)
        {
            if (this.property.Axis == GridLayoutGroup.Axis.Horizontal)
            {
                return vec.x;
            }
            else
            {
                return vec.y;
            }
        }

        /// <summary>
        /// Loopなし版に必要な各種データを初期化します
        /// </summary>
        public abstract void InitNoLoopViewData();

        /// <summary>
        /// ドラッグのdelta値を受け取ってContentの位置を移動します
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public abstract int MoveContentByDelta(Vector3 delta);

        /// <summary>
        /// ContentのTransformがセンタリングされるまでの距離を計算して返します
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 DistanceToCenteringPosition();

        /// <summary>
        /// VectorがPrev方向に向いてるかどうかを返します
        /// </summary>
        /// <param name="vec">Vector3</param>
        /// <returns>プラス方向かどうか</returns>
        public abstract bool CheckDeltaPrevVector(Vector3 vec);

        /// <summary>
        /// VectorがNext方向に向いてるかどうかを返します
        /// </summary>
        /// <param name="vec">Vector3</param>
        /// <returns>Next方向かどうか</returns>
        public abstract bool CheckDeltaNextVector(Vector3 vec);

        /// <summary>
        /// 引数がPrev方向に動けるかどうかチェックし、最大量を引数分として動かせる分だけのDelta値を返します
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public abstract Vector3 GetValidDeltaPrev(Vector3 delta);

        /// <summary>
        /// 引数がNext方向に動けるかどうかをチェックし、最大量を引数分として動かせる分だけのDelta値を返します
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public abstract Vector3 GetValidDeltaNext(Vector3 delta);

        /// <summary>
        /// 引数のoffsetIndexを基にして移動量をVector3で返します
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual Vector3 GetContentMoveAmountByOffsetIndex(int offset)
        {
            return DistanceToCenteringPosition() - GetAmountOfMovement(offset);
        }

#endregion

#region protected methods

        /// <summary>
        /// ContentのBaseとなるPositionを計算して返します
        /// </summary>
        /// <param name="cellCount">Cellの個数</param>
        /// <returns></returns>
        protected abstract Vector3 CalculateContentBasePosition(int cellCount);

#endregion
    }
}
