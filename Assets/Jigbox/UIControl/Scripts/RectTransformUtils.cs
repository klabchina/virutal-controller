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

namespace Jigbox.UIControl
{
    public static class RectTransformUtils
    {
#region inner classes, enum, and structs

        /// <summary>
        /// Anchorの位置
        /// </summary>
        public enum AnchorPoint
        {
            TopLeft,
            TopCenter,
            TopRight,
            CenterLeft,
            Center,
            CenterRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
            Top,
            VerticalCenter,
            HorizontalCenter,
            Bottom,
            Left,
            Right,
            StretchVertical,
            StretchHorizontal,
            StretchFull,
        }

#endregion

#region public methods

        /// <summary>
        /// <para>RectTransformの位置を設定します。</para>
        /// <para>位置の補正を有効にした場合、中心を基準に位置を変更したように見えるよう、位置を調整します。</para>
        /// </summary>
        /// <param name="transform">対象のRectTransform</param>
        /// <param name="position">位置</param>
        /// <param name="adjustPosition">位置の補正を行うかどうか</param>
        public static void SetPosition(RectTransform transform, Vector2 position, bool adjustPosition = true)
        {
            if (adjustPosition)
            {
                position.x += (transform.pivot.x - 0.5f) * transform.rect.width * transform.localScale.x;
                position.y += (transform.pivot.y - 0.5f) * transform.rect.height * transform.localScale.y;
            }

            transform.localPosition = position;
        }

        /// <summary>
        /// <para>RectTransformのサイズを設定します。</para>
        /// <para>位置の補正を有効にした場合、中心を基準にサイズを変更したように見えるよう、位置を調整します。</para>
        /// </summary>
        /// <param name="transform">対象のRectTransform</param>
        /// <param name="size">サイズ</param>
        /// <param name="adjustPosition">位置の補正を行うかどうか</param>
        public static void SetSize(RectTransform transform, Vector2 size, bool adjustPosition = true)
        {
            size.x = size.x <= 0.0f ? 0.0f : size.x;
            size.y = size.y <= 0.0f ? 0.0f : size.y;

            Vector2 anchorMargin = Vector2.zero;

            // [sizeDelta = RectTransform.rect - Anchor間の距離]からAnchor間の距離を逆算
            anchorMargin.x = transform.rect.width - transform.sizeDelta.x;
            anchorMargin.y = transform.rect.height - transform.sizeDelta.y;
            // 実サイズからsizeDeltaに変換
            Vector2 sizeDelta = size - anchorMargin;
            if (transform.sizeDelta == sizeDelta)
            {
                return;
            }

            Vector2 lastSizeDelta = transform.sizeDelta;
            transform.sizeDelta = sizeDelta;

            // 見かけ上の位置を保持する
            if (adjustPosition)
            {
                Vector3 position = transform.localPosition;
                Vector2 sizeDeltaOffset = lastSizeDelta - sizeDelta;
                position.x -= (transform.pivot.x - 0.5f) * sizeDeltaOffset.x;
                position.y -= (transform.pivot.y - 0.5f) * sizeDeltaOffset.y;
                transform.localPosition = position;
            }
        }

        /// <summary>
        /// <para>RectTransformのAnchorを設定します。</para>
        /// <para>Anchorの状態を変更しても位置は維持されます。</para>
        /// </summary>
        /// <param name="transform">対象のRectTransform</param>
        /// <param name="anchorMin">Anchorの最小値(左上位置)</param>
        /// <param name="anchorMax">Anchorの最大値(右下位置)</param>
        public static void SetAnchor(RectTransform transform, Vector2 anchorMin, Vector2 anchorMax)
        {
            // Anchorの反転等を防ぐために範囲を制限
            anchorMin.x = Mathf.Clamp(anchorMin.x, 0.0f, anchorMax.x);
            anchorMin.y = Mathf.Clamp(anchorMin.y, 0.0f, anchorMax.y);
            anchorMax.x = Mathf.Clamp(anchorMax.x, anchorMin.x, 1.0f);
            anchorMax.y = Mathf.Clamp(anchorMax.y, anchorMin.y, 1.0f);

            if ((anchorMin == transform.anchorMin) && (anchorMax == transform.anchorMax))
            {
                return;
            }

            Vector2 lastAnchorMin = transform.anchorMin;
            Vector2 lastAnchorMax = transform.anchorMax;
            Vector2 lastSizeDelta = transform.sizeDelta;
            transform.anchorMin = anchorMin;
            transform.anchorMax = anchorMax;

            RectTransform parent = transform.parent as RectTransform;

            // Anchorは親のRectTransformに対しての相対値であるため、
            // 親が存在しない場合は影響を計算する必要がない
            if (parent == null)
            {
                return;
            }

            Rect parentRect = parent.rect;
            float offsetMinX = parentRect.width * (lastAnchorMin.x - transform.anchorMin.x);
            float offsetMaxX = parentRect.width * (lastAnchorMax.x - transform.anchorMax.x);
            float offsetMinY = parentRect.height * (lastAnchorMin.y - transform.anchorMin.y);
            float offsetMaxY = parentRect.height * (lastAnchorMax.y - transform.anchorMax.y);

            Vector3 position = transform.localPosition;
            position.x += (offsetMinX + offsetMaxX) * 0.5f;
            position.y += (offsetMinY + offsetMaxY) * 0.5f;

            Vector2 sizeDelta = transform.sizeDelta;
            sizeDelta.x += (-offsetMinX + offsetMaxX);
            sizeDelta.y += (-offsetMinY + offsetMaxY);
            transform.sizeDelta = sizeDelta;

            Vector2 sizeDeltaOffset = lastSizeDelta - sizeDelta;
            position.x -= (transform.pivot.x - 0.5f) * sizeDeltaOffset.x;
            position.y -= (transform.pivot.y - 0.5f) * sizeDeltaOffset.y;

            transform.localPosition = position;
        }

        /// <summary>
        /// <para>RectTransformのAnchorを設定します。</para>
        /// <para>Anchorの状態を変更しても位置は維持されます。</para>
        /// </summary>
        /// <param name="transform">対象のRectTransform</param>
        /// <param name="anchorPoint">Anchorの位置</param>
        public static void SetAnchor(RectTransform transform, AnchorPoint anchorPoint)
        {
            Vector2 anchorMin = new Vector2(0.5f, 0.5f);
            Vector2 anchorMax = new Vector2(0.5f, 0.5f);

            switch (anchorPoint)
            {
                case AnchorPoint.TopLeft:
                    anchorMin = new Vector2(0.0f, 1.0f);
                    anchorMax = new Vector2(0.0f, 1.0f);
                    break;
                case AnchorPoint.TopCenter:
                    anchorMin = new Vector2(0.5f, 1.0f);
                    anchorMax = new Vector2(0.5f, 1.0f);
                    break;
                case AnchorPoint.TopRight:
                    anchorMin = new Vector2(1.0f, 1.0f);
                    anchorMax = new Vector2(1.0f, 1.0f);
                    break;
                case AnchorPoint.CenterLeft:
                    anchorMin = new Vector2(0.0f, 0.5f);
                    anchorMax = new Vector2(0.0f, 0.5f);
                    break;
                case AnchorPoint.Center:
                    anchorMin = new Vector2(0.5f, 0.5f);
                    anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPoint.CenterRight:
                    anchorMin = new Vector2(1.0f, 0.5f);
                    anchorMax = new Vector2(1.0f, 0.5f);
                    break;
                case AnchorPoint.BottomLeft:
                    anchorMin = new Vector2(0.0f, 0.0f);
                    anchorMax = new Vector2(0.0f, 0.0f);
                    break;
                case AnchorPoint.BottomCenter:
                    anchorMin = new Vector2(0.5f, 0.0f);
                    anchorMax = new Vector2(0.5f, 0.0f);
                    break;
                case AnchorPoint.BottomRight:
                    anchorMin = new Vector2(1.0f, 0.0f);
                    anchorMax = new Vector2(1.0f, 0.0f);
                    break;
                case AnchorPoint.Top:
                    anchorMin = new Vector2(transform.anchorMin.x, 1.0f);
                    anchorMax = new Vector2(transform.anchorMax.x, 1.0f);
                    break;
                case AnchorPoint.VerticalCenter:
                    anchorMin = new Vector2(transform.anchorMin.x, 0.5f);
                    anchorMax = new Vector2(transform.anchorMax.x, 0.5f);
                    break;
                case AnchorPoint.HorizontalCenter:
                    anchorMin = new Vector2(0.5f, transform.anchorMin.y);
                    anchorMax = new Vector2(0.5f, transform.anchorMax.y);
                    break;
                case AnchorPoint.Bottom:
                    anchorMin = new Vector2(transform.anchorMin.x, 0.0f);
                    anchorMax = new Vector2(transform.anchorMax.x, 0.0f);
                    break;
                case AnchorPoint.Left:
                    anchorMin = new Vector2(0.0f, transform.anchorMin.y);
                    anchorMax = new Vector2(0.0f, transform.anchorMax.y);
                    break;
                case AnchorPoint.Right:
                    anchorMin = new Vector2(1.0f, transform.anchorMin.y);
                    anchorMax = new Vector2(1.0f, transform.anchorMax.y);
                    break;
                case AnchorPoint.StretchVertical:
                    anchorMin = new Vector2(transform.anchorMin.x, 0.0f);
                    anchorMax = new Vector2(transform.anchorMax.x, 1.0f);
                    break;
                case AnchorPoint.StretchHorizontal:
                    anchorMin = new Vector2(0.0f, transform.anchorMin.y);
                    anchorMax = new Vector2(1.0f, transform.anchorMax.y);
                    break;
                case AnchorPoint.StretchFull:
                    anchorMin = new Vector2(0.0f, 0.0f);
                    anchorMax = new Vector2(1.0f, 1.0f);
                    break;
            }

            SetAnchor(transform, anchorMin, anchorMax);
        }

        /// <summary>
        /// <para>RectTransformのPivotを設定します。</para>
        /// <para>位置の補正を有効にした場合、見た目上の位置を保持したままpivot位置が変わったように見えるよう、位置を調整します。</para>
        /// </summary>
        /// <param name="transform">対象のRectTransform</param>
        /// <param name="pivot">Pivot</param>
        /// <param name="autoAdjustPosition">位置の補正を行うかどうか</param>
        public static void SetPivot(RectTransform transform, Vector2 pivot, bool adjustPosition = true)
        {
            if (transform.pivot == pivot)
            {
                return;
            }

            Vector2 lastPivot = transform.pivot;
            transform.pivot = pivot;

            Vector3 position = transform.localPosition;

            // 見かけ上の位置を保持する
            if (adjustPosition)
            {
                position.x += (pivot.x - lastPivot.x) * transform.rect.width * transform.localScale.x;
                position.y += (pivot.y - lastPivot.y) * transform.rect.height * transform.localScale.y;
                transform.localPosition = position;
            }

            Vector2 pivotOffset = lastPivot - transform.pivot;
            position.x += pivotOffset.x * (transform.rect.width - transform.sizeDelta.x);
            position.y += pivotOffset.y * (transform.rect.height - transform.sizeDelta.y);
            transform.localPosition = position;
        }

        /// <summary>
        /// RectTransformの実描画領域の左下、右上の座標を取得します。
        /// </summary>
        /// <param name="transform">対象のRectTransform</param>
        /// <returns></returns>
        public static Bounds GetBounds(RectTransform transform)
        {
            Vector2 size = transform.rect.size;
            size.x *= transform.lossyScale.x;
            size.y *= transform.lossyScale.y;

            Vector3 leftBottom = transform.position;
            leftBottom.x -= size.x * transform.pivot.x;
            leftBottom.y -= size.y * transform.pivot.y;

            Vector3 rightTop = transform.position;
            rightTop.x += size.x * (1.0f - transform.pivot.x);
            rightTop.y += size.y * (1.0f - transform.pivot.y);

            Bounds bounds = new Bounds();
            bounds.min = leftBottom;
            bounds.max = rightTop;
            return bounds;
        }

        /// <summary>Cornerのキャッシュ</summary>
        static readonly Vector3[] Corners = new Vector3[4];

        /// <summary>
        /// RectTransformのCanvas上のRectを取得します。
        /// https://github.com/Unity-Technologies/uGUI/blob/2019.1/UnityEngine.UI/UI/Core/MaskableGraphic.cs#L204-L232
        /// </summary>
        /// <param name="transform">対象のRectTransform</param>
        /// <param name="canvas">Canvas</param>
        /// <returns></returns>
        public static Rect GetCanvasSpaceRect(RectTransform transform, Canvas canvas)
        {
            transform.GetWorldCorners(Corners);

            if (canvas)
            {
                var mat = canvas.rootCanvas.transform.worldToLocalMatrix;
                for (int i = 0; i < 4; ++i)
                {
                    Corners[i] = mat.MultiplyPoint(Corners[i]);
                }
            }

            var min = Corners[0];
            var max = Corners[0];
            for (int i = 1; i < 4; i++)
            {
                min.x = Mathf.Min(Corners[i].x, min.x);
                min.y = Mathf.Min(Corners[i].y, min.y);
                max.x = Mathf.Max(Corners[i].x, max.x);
                max.y = Mathf.Max(Corners[i].y, max.y);
            }

            return new Rect(min, max - min);
        }

        /// <summary>原点がCanvasの左上になるスクリーン座標を取得する</summary>
        public static Vector2 GetScreenPoint(Canvas canvas, RectTransform rectTransform)
        {
            var localPoint = canvas.transform.worldToLocalMatrix.MultiplyPoint(rectTransform.position);
            var canvasPixelRect = ((RectTransform) canvas.transform).rect;
            // キャンバスの半分のサイズを計算
            var canvasHalfWidth = canvasPixelRect.width * 0.5f;
            var canvasHalfHeight = canvasPixelRect.height * 0.5f;
            // pivotの影響計算
            var rect = rectTransform.rect;
            var localScale = rectTransform.localScale;
            var pivot = rectTransform.pivot;
            var pivotOffsetX = pivot.x * rect.width * localScale.x;
            var pivotOffsetY = (1 - pivot.y) * rect.height * localScale.y;

            return new Vector2(Mathf.RoundToInt(localPoint.x + canvasHalfWidth - pivotOffsetX), Mathf.RoundToInt((localPoint.y - canvasHalfHeight + pivotOffsetY) * -1));
        }

        /// <summary>原点が左上のスクリーン座標からworld座標に変換する</summary>
        public static Vector3 LeftTopScreenPointToWorldPoint(Canvas rootCanvas, RectTransform rectTransform, Vector2 screenPoint)
        {
            // 入力された左上からの座標系からGameObjectのWorld座標を計算する。
            var canvasRectTransform = (RectTransform) rootCanvas.transform;
            var rect = rectTransform.rect;
            // キャンバスの半分のサイズを計算
            var canvasPixelRect = canvasRectTransform.rect;
            var canvasHalfWidth = canvasPixelRect.width * 0.5f;
            var canvasHalfHeight = canvasPixelRect.height * 0.5f;
            // pivotの影響計算
            var localScale = rectTransform.localScale;
            var pivot = rectTransform.pivot;
            // ScreenPointはRectTransformの矩形の左上の座標なので、左上の座標からPivotを考慮した座標に変更する
            var pivotOffsetX = pivot.x * rect.width * localScale.x;
            var pivotOffsetY = (1 - pivot.y) * rect.height * localScale.y;
            var pivotScreenPoint = new Vector3(screenPoint.x + pivotOffsetX - canvasHalfWidth, canvasPixelRect.height - screenPoint.y - pivotOffsetY - canvasHalfHeight, 0f);

            return canvasRectTransform.localToWorldMatrix.MultiplyPoint(pivotScreenPoint);
        }

#endregion
    }
}
