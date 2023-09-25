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
    public static class PositionTransformUtils
    {
#region public methods

        /// <summary>
        /// ワールド座標系の位置からスクリーン座標系での位置を取得します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <param name="worldPosition">ワールド座標系の位置</param>
        /// <returns></returns>
        public static Vector2 WorldToScreen(Camera camera, Vector3 worldPosition)
        {
            return camera.WorldToScreenPoint(worldPosition);
        }

        /// <summary>
        /// スクリーン座標系の位置からワールド座標系での位置を取得します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <param name="screenPoint">スクリーン座標系の位置</param>
        /// <param name="distanceFromCamera">取得するワールド座標系でのカメラからの距離</param>
        /// <returns></returns>
        public static Vector3 ScreenToWorld(Camera camera, Vector2 screenPoint, float distanceFromCamera)
        {
            Vector3 point = screenPoint;
            point.z = distanceFromCamera;
            return camera.ScreenToWorldPoint(point);
        }

        /// <summary>
        /// <para>3D空間のワールド座標系の位置からRectTransformのローカル座標位置を取得します。</para>
        /// <para>取得できるローカル座標位置は、指定されたRectTransformを親空間としたローカル空間の座標です。</para>
        /// </summary>
        /// <param name="camera3d">3D空間を写しているカメラ</param>
        /// <param name="worldPosition">ワールド座標系での位置</param>
        /// <param name="cameraCanvas">UIを写しているカメラ(CanvasがScreen Space Overlayの場合はnull)</param>
        /// <param name="rectTransform">ローカル座標位置を取得したいRectTransform</param>
        /// <returns></returns>
        public static Vector2 WorldToUILocal(
            Camera camera3d,
            Vector3 worldPosition,
            Camera cameraCanvas,
            RectTransform rectTransform)
        {
            Vector2 screenPoint = WorldToScreen(camera3d, worldPosition);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, cameraCanvas, out localPoint);
            return localPoint;
        }

        /// <summary>
        /// 3D空間のワールド座標系の位置からRectTransformのワールド座標系での位置を取得します。
        /// </summary>
        /// <param name="camera3d">3D空間を写しているカメラ</param>
        /// <param name="worldPosition">ワールド座標系での位置</param>
        /// <param name="cameraCanvas">UIを写しているカメラ(CanvasがScreen Space Overlayの場合はnull)</param>
        /// <param name="rectTransform">ワールド座標系での位置を取得したいRectTransform</param>
        /// <returns></returns>
        public static Vector3 WorldToUIWorld(
            Camera camera3d,
            Vector3 worldPosition,
            Camera cameraCanvas,
            RectTransform rectTransform)
        {
            Vector2 screenPoint = WorldToScreen(camera3d, worldPosition);
            Vector3 worldPoint;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, cameraCanvas, out worldPoint);
            return worldPoint;
        }

        /// <summary>
        /// RectTransformのワールド座標系の位置から3D空間のワールド座標系での位置を取得します。
        /// </summary>
        /// <param name="camera3d">3D空間を写しているカメラ</param>
        /// <param name="distanceFromCamera">取得するワールド座標系でのカメラからの距離</param>
        /// <param name="cameraCanvas">UIを写しているカメラ(CanvasがScreen Space Overlayの場合はnull)</param>
        /// <param name="rectTransformPosition">位置の取得元となるRectTransformのワールド座標系での位置</param>
        /// <returns></returns>
        public static Vector3 UIWorldToWorld(
            Camera camera3d,
            float distanceFromCamera,
            Camera cameraCanvas,
            Vector3 rectTransformPosition)
        {
            Vector3 point;
            if (cameraCanvas != null)
            {
                point = WorldToScreen(cameraCanvas, rectTransformPosition);
            }
            else
            {
                point = rectTransformPosition;
            }
            point.z = distanceFromCamera;
            return camera3d.ScreenToWorldPoint(point);
        }

#endregion
    }
}
