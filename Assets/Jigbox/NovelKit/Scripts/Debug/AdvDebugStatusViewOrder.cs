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
using Jigbox.UIControl;

namespace Jigbox.NovelKit
{
    public class AdvDebugStatusViewOrder
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 表示位置
        /// </summary>
        public enum ViewPosition
        {
            /// <summary>左上</summary>
            TopLeft,
            /// <summary>右上</summary>
            TopRight,
            /// <summary>左下</summary>
            BottomLeft,
            /// <summary>右下</summary>
            BottomRight,
        }

#endregion

#region constants

        /// <summary>マージンのデフォルト値</summary>
        protected static readonly float DefaultMargin = 10.0f;

        /// <summary>ウィンドウの横幅のデフォルト値</summary>
        protected static readonly float DefaultWidth = 300.0f;

        /// <summary>ウィンドウの縦幅のデフォルト値</summary>
        protected static readonly float DefaultHeight = 200.0f;

#endregion

#region properties

        /// <summary>表示位置</summary>
        protected ViewPosition position = ViewPosition.TopRight;

        /// <summary>表示位置</summary>
        public ViewPosition Position { get { return position; } set { position = value; } }

        /// <summary>マージン</summary>
        protected float margin = DefaultMargin;

        /// <summary>マージン</summary>
        public float Margin { get { return margin; } set { margin = value; } }

        /// <summary>ウィンドウの横幅</summary>
        protected float width = DefaultWidth;

        /// <summary>ウィンドウの横幅</summary>
        public float Width { get { return width; } set { width = value; } }

        /// <summary>ウィンドウの縦幅</summary>
        protected float height = DefaultHeight;

        /// <summary>ウィンドウの縦幅</summary>
        public float Height { get { return height; } set { height = value; } }

        /// <summary>FPSを表示するかどうか</summary>
        protected bool isShowFps = true;

        /// <summary>FPSを表示するかどうか</summary>
        public bool IsShowFps { get { return isShowFps; } set { isShowFps = value; } }

        /// <summary>タイムコードを表示するかどうか</summary>
        protected bool isShowTimecode = true;

        /// <summary>タイムコードを表示するかどうか</summary>
        public bool IsShowTimecode { get { return isShowTimecode; } set { isShowTimecode = value; } }

        /// <summary>Anchorの設定</summary>
        public RectTransformUtils.AnchorPoint AnchorPoint
        {
            get
            {
                switch (position)
                {
                    case ViewPosition.TopLeft:
                        return RectTransformUtils.AnchorPoint.TopLeft;
                    case ViewPosition.TopRight:
                        return RectTransformUtils.AnchorPoint.TopRight;
                    case ViewPosition.BottomLeft:
                        return RectTransformUtils.AnchorPoint.BottomLeft;
                    case ViewPosition.BottomRight:
                        return RectTransformUtils.AnchorPoint.BottomRight;
                    default:
                        UnityEngine.Assertions.Assert.IsTrue(false, "If you support all ViewPosition, you will not reach here.");
                        return RectTransformUtils.AnchorPoint.TopLeft;
                }
            }
        }

        /// <summary>Anchorからのオフセット位置(右上)</summary>
        public Vector2 OffsetMax
        {
            get
            {
                switch (position)
                {
                    case ViewPosition.TopLeft:
                        return new Vector2(Margin + Width, -Margin);
                    case ViewPosition.TopRight:
                        return new Vector2(-Margin, -Margin);
                    case ViewPosition.BottomLeft:
                        return new Vector2(Margin + Width, Margin + Height);
                    case ViewPosition.BottomRight:
                        return new Vector2(-Margin, Margin + Height);
                    default:
                        UnityEngine.Assertions.Assert.IsTrue(false, "If you support all ViewPosition, you will not reach here.");
                        return Vector2.zero;
                }
            }
        }

        /// <summary>Anchorからのオフセット位置(左下)</summary>
        public Vector2 OffsetMin
        {
            get
            {
                switch (position)
                {
                    case ViewPosition.TopLeft:
                        return new Vector2(Margin, -(Margin + Height));
                    case ViewPosition.TopRight:
                        return new Vector2(-(Margin + Width), -(Margin + Height));
                    case ViewPosition.BottomLeft:
                        return new Vector2(Margin, Margin);
                    case ViewPosition.BottomRight:
                        return new Vector2(-(Margin + Width), Margin);
                    default:
                        UnityEngine.Assertions.Assert.IsTrue(false, "If you support all ViewPosition, you will not reach here.");
                        return Vector2.zero;
                }
            }
        }

#endregion

#region public methods

        /// <summary>
        /// 表示用の構成を生成します。
        /// </summary>
        /// <param name="parent">親となるオブジェクトの参照</param>
        /// <returns></returns>
        public virtual AdvDebugStatusView Generate(Transform parent)
        {
            return AdvDebugStatusView.CreateDefaultView(parent, this);
        }

#endregion
    }
}
