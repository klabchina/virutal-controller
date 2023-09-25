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
using System.Collections;

namespace Jigbox.Tween
{
    public static class TweenUtils
    {
#region public methods

#region position utils

#region move to

        /// <summary>
        /// 現在の位置から指定された位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">移動後のx座標</param>
        /// <param name="y">移動後のy座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveTo(Transform transform, float duration, float x, float y)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = transform.localPosition;
            tween.Final = new Vector3(x, y);
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Vector3 position = t.Value;
                position.z = transform.localPosition.z;
                transform.localPosition = position;
            });

            return tween;
        }

        /// <summary>
        /// 現在の位置から指定された位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">移動後のx座標</param>
        /// <param name="y">移動後のy座標</param>
        /// <param name="z">移動後のz座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveTo(Transform transform, float duration, float x, float y, float z)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = transform.localPosition;
            tween.Final = new Vector3(x, y, z);
            tween.Duration = duration;
            tween.OnUpdate(t => transform.localPosition = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在の位置から指定された位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="to">移動後の座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveTo(Transform transform, float duration, Vector2 to)
        {
            return MoveTo(transform, duration, to.x, to.y);
        }

        /// <summary>
        /// 現在の位置から指定された位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="to">移動後の座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveTo(Transform transform, float duration, Vector3 to)
        {
            return MoveTo(transform, duration, to.x, to.y, to.z);
        }

        /// <summary>
        /// 現在の位置から指定された位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenVector3 MoveTo(Transform transform, TweenPositionOrder order)
        {
            TweenVector3 tween = new TweenVector3();
            order.SetProperties(tween);

            tween.Begin = transform.localPosition;
            tween.Final = order.GetToValue();
            tween.OnUpdate(order.CreateOnUpdate(transform));

            return tween;
        }

        /// <summary>
        /// 現在の位置から指定されたx座標位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">移動後のx座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveToX(Transform transform, float duration, float x)
        {
            TweenPositionOrder order = new TweenPositionOrder(duration);
            order.X = x;
            return MoveTo(transform, order);
        }

        /// <summary>
        /// 現在の位置から指定されたy座標位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="y">移動後のy座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveToY(Transform transform, float duration, float y)
        {
            TweenPositionOrder order = new TweenPositionOrder(duration);
            order.Y = y;
            return MoveTo(transform, order);
        }

        /// <summary>
        /// 現在の位置から指定されたz座標位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="z">移動後のz座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveToZ(Transform transform, float duration, float z)
        {
            TweenPositionOrder order = new TweenPositionOrder(duration);
            order.Z = z;
            return MoveTo(transform, order);
        }

#endregion

#region move from

        /// <summary>
        /// 指定された位置から現在の位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">移動前のx座標</param>
        /// <param name="y">移動前のy座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveFrom(Transform transform, float duration, float x, float y)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = new Vector3(x, y);
            tween.Final = transform.localPosition;
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Vector3 position = t.Value;
                position.z = transform.localPosition.z;
                transform.localPosition = position;
            });

            return tween;
        }

        /// <summary>
        /// 指定された位置から現在の位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">移動前のx座標</param>
        /// <param name="y">移動前のy座標</param>
        /// <param name="z">移動前のz座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveFrom(Transform transform, float duration, float x, float y, float z)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = new Vector3(x, y, z);
            tween.Final = transform.localPosition;
            tween.Duration = duration;
            tween.OnUpdate(t => transform.localPosition = t.Value);

            return tween;
        }

        /// <summary>
        /// 指定された位置から現在の位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="from">移動前の座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveFrom(Transform transform, float duration, Vector2 from)
        {
            return MoveFrom(transform, duration, from.x, from.y);
        }

        /// <summary>
        /// 指定された位置から現在の位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="from">移動前の座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveFrom(Transform transform, float duration, Vector3 from)
        {
            return MoveFrom(transform, duration, from.x, from.y, from.z);
        }

        /// <summary>
        /// 指定された位置から現在の位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenVector3 MoveFrom(Transform transform, TweenPositionOrder order)
        {
            TweenVector3 tween = new TweenVector3();
            order.SetProperties(tween);

            tween.Begin = order.GetFromValue();
            tween.Final = transform.localPosition;
            tween.OnUpdate(order.CreateOnUpdate(transform));

            return tween;
        }

        /// <summary>
        /// 指定されたx座標位置から現在の位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">移動前のx座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveFromX(Transform transform, float duration, float x)
        {
            TweenPositionOrder order = new TweenPositionOrder(duration);
            order.X = x;
            return MoveFrom(transform, order);
        }

        /// <summary>
        /// 指定されたy座標位置から現在の位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="y">移動前のy座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveFromY(Transform transform, float duration, float y)
        {
            TweenPositionOrder order = new TweenPositionOrder(duration);
            order.Y = y;
            return MoveFrom(transform, order);
        }

        /// <summary>
        /// 指定されたz座標位置から現在の位置へと移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="z">移動前のz座標</param>
        /// <returns></returns>
        public static TweenVector3 MoveFromZ(Transform transform, float duration, float z)
        {
            TweenPositionOrder order = new TweenPositionOrder(duration);
            order.Z = z;
            return MoveFrom(transform, order);
        }

#endregion

#region move by

        /// <summary>
        /// 現在の位置から指定されたオフセット値分移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x座標のオフセット値</param>
        /// <param name="y">y座標のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 MoveBy(Transform transform, float duration, float x, float y)
        {
            TweenVector3 tween = new TweenVector3();
            Vector3 to = transform.localPosition;
            to.x += x;
            to.y += y;

            tween.Begin = transform.localPosition;
            tween.Final = to;
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Vector3 position = t.Value;
                position.z = transform.localPosition.z;
                transform.localPosition = position;
            });

            return tween;
        }

        /// <summary>
        /// 現在の位置から指定されたオフセット値分移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x座標のオフセット値</param>
        /// <param name="y">y座標のオフセット値</param>
        /// <param name="z">z座標のオフセット</param>
        /// <returns></returns>
        public static TweenVector3 MoveBy(Transform transform, float duration, float x, float y, float z)
        {
            TweenVector3 tween = new TweenVector3();
            Vector3 to = transform.localPosition;
            to.x += x;
            to.y += y;
            to.z += z;

            tween.Begin = transform.localPosition;
            tween.Final = to;
            tween.Duration = duration;
            tween.OnUpdate(t => transform.localPosition = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在の位置から指定されたオフセット値分移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="offset">座標のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 MoveBy(Transform transform, float duration, Vector2 offset)
        {
            return MoveBy(transform, duration, offset.x, offset.y);
        }

        /// <summary>
        /// 現在の位置から指定されたオフセット値分移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="offset">座標のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 MoveBy(Transform transform, float duration, Vector3 offset)
        {
            return MoveBy(transform, duration, offset.x, offset.y, offset.z);
        }

        /// <summary>
        /// 現在の位置から指定されたオフセット値分移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenVector3 MoveBy(Transform transform, TweenPositionOrder order)
        {
            TweenVector3 tween = new TweenVector3();
            order.SetProperties(tween);

            tween.Begin = transform.localPosition;
            tween.Final = order.GetByValue(transform.localPosition);
            tween.OnUpdate(order.CreateOnUpdate(transform));

            return tween;
        }

        /// <summary>
        /// 現在の位置からx座標を指定されたオフセット値分移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x座標のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 MoveByX(Transform transform, float duration, float x)
        {
            TweenPositionOrder order = new TweenPositionOrder(duration);
            order.X = x;
            return MoveBy(transform, order);
        }

        /// <summary>
        /// 現在の位置からy座標を指定されたオフセット値分移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="y">y座標のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 MoveByY(Transform transform, float duration, float y)
        {
            TweenPositionOrder order = new TweenPositionOrder(duration);
            order.Y = y;
            return MoveBy(transform, order);
        }

        /// <summary>
        /// 現在の位置からz座標を指定されたオフセット値分移動するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="z">z座標のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 MoveByZ(Transform transform, float duration, float z)
        {
            TweenPositionOrder order = new TweenPositionOrder(duration);
            order.Z = z;
            return MoveBy(transform, order);
        }

#endregion

#endregion

#region rotation utils

#region rotate to

        /// <summary>
        /// 現在の角度から指定された角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">回転後のx回転角</param>
        /// <param name="y">回転後のy回転角</param>
        /// <param name="z">回転後のz回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateTo(Transform transform, float duration, float x, float y, float z)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = transform.localEulerAngles;
            tween.Final = new Vector3(x, y, z);
            tween.Duration = duration;
            tween.OnUpdate(t => transform.localEulerAngles = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在の角度から指定された角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="to">回転後の回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateTo(Transform transform, float duration, Vector3 to)
        {
            return RotateTo(transform, duration, to.x, to.y, to.z);
        }

        /// <summary>
        /// 現在の角度から指定された角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenVector3 RotateTo(Transform transform, TweenRotationOrder order)
        {
            TweenVector3 tween = new TweenVector3();
            order.SetProperties(tween);

            tween.Begin = transform.localEulerAngles;
            tween.Final = order.GetToValue();
            tween.OnUpdate(order.CreateOnUpdate(transform));

            return tween;
        }

        /// <summary>
        /// 現在の角度からx軸を指定された角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">回転後のx回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateToX(Transform transform, float duration, float x)
        {
            TweenRotationOrder order = new TweenRotationOrder(duration);
            order.X = x;
            return RotateTo(transform, order);
        }

        /// <summary>
        /// 現在の角度からy軸を指定された角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="y">回転後のy回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateToY(Transform transform, float duration, float y)
        {
            TweenRotationOrder order = new TweenRotationOrder(duration);
            order.Y = y;
            return RotateTo(transform, order);
        }

        /// <summary>
        /// 現在の角度からz軸を指定された角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="z">回転後のz回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateToZ(Transform transform, float duration, float z)
        {
            TweenRotationOrder order = new TweenRotationOrder(duration);
            order.Z = z;
            return RotateTo(transform, order);
        }

#endregion

#region rotate from

        /// <summary>
        /// 指定された角度から現在の角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">回転前のx回転角</param>
        /// <param name="y">回転前のy回転角</param>
        /// <param name="z">回転前のz回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateFrom(Transform transform, float duration, float x, float y, float z)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = new Vector3(x, y, z);
            tween.Final = transform.localEulerAngles;
            tween.Duration = duration;
            tween.OnUpdate(t => transform.localEulerAngles = t.Value);

            return tween;
        }

        /// <summary>
        /// 指定された角度から現在の角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="from">回転前の回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateFrom(Transform transform, float duration, Vector3 from)
        {
            return RotateFrom(transform, duration, from.x, from.y, from.z);
        }

        /// <summary>
        /// 指定された角度から現在の角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenVector3 RotateFrom(Transform transform, TweenRotationOrder order)
        {
            TweenVector3 tween = new TweenVector3();
            order.SetProperties(tween);

            tween.Begin = order.GetFromValue();
            tween.Final = transform.localEulerAngles;
            tween.OnUpdate(order.CreateOnUpdate(transform));

            return tween;
        }

        /// <summary>
        /// 指定された角度からx軸を現在の角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">回転前のx回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateFromX(Transform transform, float duration, float x)
        {
            TweenRotationOrder order = new TweenRotationOrder(duration);
            order.X = x;
            return RotateFrom(transform, order);
        }

        /// <summary>
        /// 指定された角度からy軸を現在の角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="y">回転前のy回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateFromY(Transform transform, float duration, float y)
        {
            TweenRotationOrder order = new TweenRotationOrder(duration);
            order.Y = y;
            return RotateFrom(transform, order);
        }

        /// <summary>
        /// 指定された角度からz軸を現在の角度へと回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="z">回転前のz回転角</param>
        /// <returns></returns>
        public static TweenVector3 RotateFromZ(Transform transform, float duration, float z)
        {
            TweenRotationOrder order = new TweenRotationOrder(duration);
            order.Z = z;
            return RotateFrom(transform, order);
        }

#endregion

#region rotate by

        /// <summary>
        /// 現在の角度からオフセット値分回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x回転角のオフセット値</param>
        /// <param name="y">y回転角のオフセット値</param>
        /// <param name="z">z回転角のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 RotateBy(Transform transform, float duration, float x, float y, float z)
        {
            TweenVector3 tween = new TweenVector3();
            Vector3 to = transform.localEulerAngles;
            to.x += x;
            to.y += y;
            to.z += z;

            tween.Begin = transform.localEulerAngles;
            tween.Final = to;
            tween.Duration = duration;
            tween.OnUpdate(t => transform.localEulerAngles = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在の角度からオフセット値分回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="to">回転角のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 RotateBy(Transform transform, float duration, Vector3 offset)
        {
            return RotateBy(transform, duration, offset.x, offset.y, offset.z);
        }

        /// <summary>
        /// 現在の角度からオフセット値分回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenVector3 RotateBy(Transform transform, TweenRotationOrder order)
        {
            TweenVector3 tween = new TweenVector3();
            order.SetProperties(tween);

            tween.Begin = transform.localEulerAngles;
            tween.Final = order.GetByValue(transform.localEulerAngles);
            tween.OnUpdate(order.CreateOnUpdate(transform));

            return tween;
        }

        /// <summary>
        /// 現在の角度からx軸をオフセット値分回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x回転角のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 RotateByX(Transform transform, float duration, float x)
        {
            TweenRotationOrder order = new TweenRotationOrder(duration);
            order.X = x;
            return RotateBy(transform, order);
        }

        /// <summary>
        /// 現在の角度からy軸をオフセット値分回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="y">y回転角のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 RotateByY(Transform transform, float duration, float y)
        {
            TweenRotationOrder order = new TweenRotationOrder(duration);
            order.Y = y;
            return RotateBy(transform, order);
        }

        /// <summary>
        /// 現在の角度からz軸をオフセット値分回転するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="z">z回転角のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 RotateByZ(Transform transform, float duration, float z)
        {
            TweenRotationOrder order = new TweenRotationOrder(duration);
            order.Z = z;
            return RotateBy(transform, order);
        }

#endregion

#endregion

#region scale utils

#region scale to

        /// <summary>
        /// 現在の拡大縮小率から指定された拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">拡縮後のx軸拡大縮小率</param>
        /// <param name="y">拡縮後のy軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleTo(Transform transform, float duration, float x, float y)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = transform.localScale;
            tween.Final = new Vector3(x, y);
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Vector3 scale = t.Value;
                scale.z = transform.localScale.z;
                transform.localScale = scale;
            });

            return tween;
        }

        /// <summary>
        /// 現在の拡大縮小率から指定された拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">拡縮後のx軸拡大縮小率</param>
        /// <param name="y">拡縮後のy軸拡大縮小率</param>
        /// <param name="z">拡縮後のz軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleTo(Transform transform, float duration, float x, float y, float z)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = transform.localScale;
            tween.Final = new Vector3(x, y, z);
            tween.Duration = duration;
            tween.OnUpdate(t => transform.localScale = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在の拡大縮小率から指定された拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="to">拡縮後の拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleTo(Transform transform, float duration, Vector2 to)
        {
            return ScaleTo(transform, duration, to.x, to.y);
        }

        /// <summary>
        /// 現在の拡大縮小率から指定された拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="to">拡縮後の拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleTo(Transform transform, float duration, Vector3 to)
        {
            return ScaleTo(transform, duration, to.x, to.y, to.z);
        }

        /// <summary>
        /// 現在の拡大縮小率から指定された拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenVector3 ScaleTo(Transform transform, TweenScaleOrder order)
        {
            TweenVector3 tween = new TweenVector3();
            order.SetProperties(tween);

            tween.Begin = transform.localScale;
            tween.Final = order.GetToValue();
            tween.OnUpdate(order.CreateOnUpdate(transform));

            return tween;
        }

        /// <summary>
        /// 現在の拡大縮小率からx軸を指定された拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">拡縮後のx軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleToX(Transform transform, float duration, float x)
        {
            TweenScaleOrder order = new TweenScaleOrder(duration);
            order.X = x;
            return ScaleTo(transform, order);
        }

        /// <summary>
        /// 現在の拡大縮小率からy軸を指定された拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="y">拡縮後のy軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleToY(Transform transform, float duration, float y)
        {
            TweenScaleOrder order = new TweenScaleOrder(duration);
            order.Y = y;
            return ScaleTo(transform, order);
        }

        /// <summary>
        /// 現在の拡大縮小率からz軸を指定された拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="z">拡縮後のz軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleToZ(Transform transform, float duration, float z)
        {
            TweenScaleOrder order = new TweenScaleOrder(duration);
            order.Z = z;
            return ScaleTo(transform, order);
        }

#endregion

#region scale from

        /// <summary>
        /// 指定された拡大縮小率から現在の拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">拡縮前のx軸拡大縮小率</param>
        /// <param name="y">拡縮前のy軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleFrom(Transform transform, float duration, float x, float y)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = new Vector3(x, y);
            tween.Final = transform.localScale;
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Vector3 scale = t.Value;
                scale.z = transform.localScale.z;
                transform.localScale = scale;
            });

            return tween;
        }

        /// <summary>
        /// 指定された拡大縮小率から現在の拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">拡縮前のx軸拡大縮小率</param>
        /// <param name="y">拡縮前のy軸拡大縮小率</param>
        /// <param name="z">拡縮前のz軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleFrom(Transform transform, float duration, float x, float y, float z)
        {
            TweenVector3 tween = new TweenVector3();

            tween.Begin = new Vector3(x, y, z);
            tween.Final = transform.localScale;
            tween.Duration = duration;
            tween.OnUpdate(t => transform.localScale = t.Value);

            return tween;
        }

        /// <summary>
        /// 指定された拡大縮小率から現在の拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="from">拡縮前の拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleFrom(Transform transform, float duration, Vector2 from)
        {
            return ScaleFrom(transform, duration, from.x, from.y);
        }

        /// <summary>
        /// 指定された拡大縮小率から現在の拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="from">拡縮前の拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleFrom(Transform transform, float duration, Vector3 from)
        {
            return ScaleFrom(transform, duration, from.x, from.y, from.z);
        }

        /// <summary>
        /// 指定された拡大縮小率から現在の拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenVector3 ScaleFrom(Transform transform, TweenScaleOrder order)
        {
            TweenVector3 tween = new TweenVector3();
            order.SetProperties(tween);

            tween.Begin = order.GetFromValue();
            tween.Final = transform.localScale;
            tween.OnUpdate(order.CreateOnUpdate(transform));

            return tween;
        }

        /// <summary>
        /// 指定されたx軸拡大縮小率から現在の拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">拡縮前のx軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleFromX(Transform transform, float duration, float x)
        {
            TweenScaleOrder order = new TweenScaleOrder(duration);
            order.X = x;
            return ScaleFrom(transform, order);
        }

        /// <summary>
        /// 指定されたy軸拡大縮小率から現在の拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="y">拡縮前のy軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleFromY(Transform transform, float duration, float y)
        {
            TweenScaleOrder order = new TweenScaleOrder(duration);
            order.Y = y;
            return ScaleFrom(transform, order);
        }

        /// <summary>
        /// 指定されたz軸拡大縮小率から現在の拡大縮小率へと拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="z">拡縮前のz軸拡大縮小率</param>
        /// <returns></returns>
        public static TweenVector3 ScaleFromZ(Transform transform, float duration, float z)
        {
            TweenScaleOrder order = new TweenScaleOrder(duration);
            order.Z = z;
            return ScaleFrom(transform, order);
        }

#endregion

#region scale by

        /// <summary>
        /// 現在の拡大縮小率から指定されたオフセット値分拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x軸拡大縮小率のオフセット値</param>
        /// <param name="y">y軸拡大縮小率のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 ScaleBy(Transform transform, float duration, float x, float y)
        {
            TweenVector3 tween = new TweenVector3();
            Vector3 to = transform.localScale;
            to.x += x;
            to.y += y;

            tween.Begin = transform.localScale;
            tween.Final = to;
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Vector3 scale = t.Value;
                scale.z = transform.localScale.z;
                transform.localScale = scale;
            });

            return tween;
        }

        /// <summary>
        /// 現在の拡大縮小率から指定されたオフセット値分拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x軸拡大縮小率のオフセット値</param>
        /// <param name="y">y軸拡大縮小率のオフセット値</param>
        /// <param name="z">z軸拡大縮小率のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 ScaleBy(Transform transform, float duration, float x, float y, float z)
        {
            TweenVector3 tween = new TweenVector3();
            Vector3 to = transform.localScale;
            to.x += x;
            to.y += y;
            to.z += z;

            tween.Begin = transform.localScale;
            tween.Final = to;
            tween.Duration = duration;
            tween.OnUpdate(t => transform.localScale = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在の拡大縮小率から指定されたオフセット値分拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="offset">拡大縮小率のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 ScaleBy(Transform transform, float duration, Vector2 offset)
        {
            return ScaleBy(transform, duration, offset.x, offset.y);
        }

        /// <summary>
        /// 現在の拡大縮小率から指定されたオフセット値分拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="offset">拡大縮小率のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 ScaleBy(Transform transform, float duration, Vector3 offset)
        {
            return ScaleBy(transform, duration, offset.x, offset.y, offset.z);
        }

        /// <summary>
        /// 現在の拡大縮小率から指定されたオフセット値分拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenVector3 ScaleBy(Transform transform, TweenScaleOrder order)
        {
            TweenVector3 tween = new TweenVector3();
            order.SetProperties(tween);

            tween.Begin = transform.localScale;
            tween.Final = order.GetByValue(transform.localScale);
            tween.OnUpdate(order.CreateOnUpdate(transform));

            return tween;
        }

        /// <summary>
        /// 現在の拡大縮小率からx軸を指定されたオフセット値分拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="x">x軸拡大縮小率のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 ScaleByX(Transform transform, float duration, float x)
        {
            TweenScaleOrder order = new TweenScaleOrder(duration);
            order.X = x;
            return ScaleBy(transform, order);
        }

        /// <summary>
        /// 現在の拡大縮小率からy軸を指定されたオフセット値分拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="y">y軸拡大縮小率のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 ScaleByY(Transform transform, float duration, float y)
        {
            TweenScaleOrder order = new TweenScaleOrder(duration);
            order.Y = y;
            return ScaleBy(transform, order);
        }

        /// <summary>
        /// 現在の拡大縮小率からz軸を指定されたオフセット値分拡縮するTweenを返します。
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="z">z軸拡大縮小率のオフセット値</param>
        /// <returns></returns>
        public static TweenVector3 ScaleByZ(Transform transform, float duration, float z)
        {
            TweenScaleOrder order = new TweenScaleOrder(duration);
            order.Z = z;
            return ScaleBy(transform, order);
        }

#endregion

#endregion

#region color utils

#region color to

        /// <summary>
        /// 現在の色から指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化後のR成分</param>
        /// <param name="g">変化後のG成分</param>
        /// <param name="b">変化後のB成分</param>
        /// <returns></returns>
        public static TweenColor ColorTo(Graphic graphic, float duration, float r, float g, float b)
        {
            TweenColor tween = new TweenColor();

            tween.Begin = graphic.color;
            tween.Final = new Color(r, g, b);
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Color color = t.Value;
                color.a = graphic.color.a;
                graphic.color = color;
            });

            return tween;
        }

        /// <summary>
        /// 現在の色から指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化後のR成分</param>
        /// <param name="g">変化後のG成分</param>
        /// <param name="b">変化後のB成分</param>
        /// <param name="a">変化後のA成分</param>
        /// <returns></returns>
        public static TweenColor ColorTo(Graphic graphic, float duration, float r, float g, float b, float a)
        {
            TweenColor tween = new TweenColor();

            tween.Begin = graphic.color;
            tween.Final = new Color(r, g, b, a);
            tween.Duration = duration;
            tween.OnUpdate(t => graphic.color = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在の色から指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化後のR成分</param>
        /// <param name="g">変化後のG成分</param>
        /// <param name="b">変化後のB成分</param>
        /// <returns></returns>
        public static TweenColor ColorTo(Graphic graphic, float duration, int r, int g, int b)
        {
            return ColorTo(graphic, duration, r / 255.0f, g / 255.0f, b / 255.0f);
        }

        /// <summary>
        /// 現在の色から指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化後のR成分</param>
        /// <param name="g">変化後のG成分</param>
        /// <param name="b">変化後のB成分</param>
        /// <param name="a">変化後のA成分</param>
        /// <returns></returns>
        public static TweenColor ColorTo(Graphic graphic, float duration, int r, int g, int b, int a)
        {
            return ColorTo(graphic, duration, r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }

        /// <summary>
        /// 現在の色から指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="to">変化後の色</param>
        /// <returns></returns>
        public static TweenColor ColorTo(Graphic graphic, float duration, Color to)
        {
            return ColorTo(graphic, duration, to.r, to.g, to.b, to.a);
        }

        /// <summary>
        /// 現在の色から指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenColor ColorTo(Graphic graphic, TweenColorOrder order)
        {
            TweenColor tween = new TweenColor();
            order.SetProperties(tween);

            tween.Begin = graphic.color;
            tween.Final = order.GetToValue();
            tween.OnUpdate(order.CreateOnUpdate(graphic));

            return tween;
        }

        /// <summary>
        /// 現在の色からR成分を指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化後のR成分</param>
        /// <returns></returns>
        public static TweenColor ColorToR(Graphic graphic, float duration, float r)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.R = r;
            return ColorTo(graphic, order);
        }

        /// <summary>
        /// 現在の色からR成分を指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化後のR成分</param>
        /// <returns></returns>
        public static TweenColor ColorToR(Graphic graphic, float duration, int r)
        {
            return ColorToR(graphic, duration, r / 255.0f);
        }

        /// <summary>
        /// 現在の色からG成分を指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="g">変化後のG成分</param>
        /// <returns></returns>
        public static TweenColor ColorToG(Graphic graphic, float duration, float g)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.G = g;
            return ColorTo(graphic, order);
        }

        /// <summary>
        /// 現在の色からG成分を指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="g">変化後のG成分</param>
        /// <returns></returns>
        public static TweenColor ColorToG(Graphic graphic, float duration, int g)
        {
            return ColorToG(graphic, duration, g / 255.0f);
        }

        /// <summary>
        /// 現在の色からB成分を指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="b">変化後のB成分</param>
        /// <returns></returns>
        public static TweenColor ColorToB(Graphic graphic, float duration, float b)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.B = b;
            return ColorTo(graphic, order);
        }

        /// <summary>
        /// 現在の色からB成分を指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="b">変化後のB成分</param>
        /// <returns></returns>
        public static TweenColor ColorToB(Graphic graphic, float duration, int b)
        {
            return ColorToB(graphic, duration, b / 255.0f);
        }

        /// <summary>
        /// 現在の色からA成分を指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="a">変化後のA成分</param>
        /// <returns></returns>
        public static TweenColor ColorToA(Graphic graphic, float duration, float a)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.A = a;
            return ColorTo(graphic, order);
        }

        /// <summary>
        /// 現在の色からA成分を指定された色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="a">変化後のA成分</param>
        /// <returns></returns>
        public static TweenColor ColorToA(Graphic graphic, float duration, int a)
        {
            return ColorToA(graphic, duration, a / 255.0f);
        }

#endregion

#region color from

        /// <summary>
        /// 指定された色から現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化前のR成分</param>
        /// <param name="g">変化前のG成分</param>
        /// <param name="b">変化前のB成分</param>
        /// <returns></returns>
        public static TweenColor ColorFrom(Graphic graphic, float duration, float r, float g, float b)
        {
            TweenColor tween = new TweenColor();

            tween.Begin = new Color(r, g, b);
            tween.Final = graphic.color;
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Color color = t.Value;
                color.a = graphic.color.a;
                graphic.color = color;
            });

            return tween;
        }

        /// <summary>
        /// 指定された色から現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化前のR成分</param>
        /// <param name="g">変化前のG成分</param>
        /// <param name="b">変化前のB成分</param>
        /// <param name="a">変化前のA成分</param>
        /// <returns></returns>
        public static TweenColor ColorFrom(Graphic graphic, float duration, float r, float g, float b, float a)
        {
            TweenColor tween = new TweenColor();

            tween.Begin = new Color(r, g, b, a);
            tween.Final = graphic.color;
            tween.Duration = duration;
            tween.OnUpdate(t => graphic.color = t.Value);

            return tween;
        }

        /// <summary>
        /// 指定された色から現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化前のR成分</param>
        /// <param name="g">変化前のG成分</param>
        /// <param name="b">変化前のB成分</param>
        /// <returns></returns>
        public static TweenColor ColorFrom(Graphic graphic, float duration, int r, int g, int b)
        {
            return ColorFrom(graphic, duration, r / 255.0f, g / 255.0f, b / 255.0f);
        }

        /// <summary>
        /// 指定された色から現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化前のR成分</param>
        /// <param name="g">変化前のG成分</param>
        /// <param name="b">変化前のB成分</param>
        /// <param name="a">変化前のA成分</param>
        /// <returns></returns>
        public static TweenColor ColorFrom(Graphic graphic, float duration, int r, int g, int b, int a)
        {
            return ColorFrom(graphic, duration, r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }

        /// <summary>
        /// 指定された色から現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="from">変化前の色</param>
        /// <returns></returns>
        public static TweenColor ColorFrom(Graphic graphic, float duration, Color from)
        {
            return ColorFrom(graphic, duration, from.r, from.g, from.b, from.a);
        }

        /// <summary>
        /// 指定された色から現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenColor ColorFrom(Graphic graphic, TweenColorOrder order)
        {
            TweenColor tween = new TweenColor();
            order.SetProperties(tween);

            tween.Begin = order.GetFromValue();
            tween.Final = graphic.color;
            tween.OnUpdate(order.CreateOnUpdate(graphic));

            return tween;
        }

        /// <summary>
        /// 指定された色からR成分を現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化前のR成分</param>
        /// <returns></returns>
        public static TweenColor ColorFromR(Graphic graphic, float duration, float r)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.R = r;
            return ColorFrom(graphic, order);
        }

        /// <summary>
        /// 指定された色からR成分を現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">変化前のR成分</param>
        /// <returns></returns>
        public static TweenColor ColorFromR(Graphic graphic, float duration, int r)
        {
            return ColorFromR(graphic, duration, r / 255.0f);
        }

        /// <summary>
        /// 指定された色からG成分を現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="g">変化前のG成分</param>
        /// <returns></returns>
        public static TweenColor ColorFromG(Graphic graphic, float duration, float g)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.G = g;
            return ColorFrom(graphic, order);
        }

        /// <summary>
        /// 指定された色からG成分を現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="g">変化前のG成分</param>
        /// <returns></returns>
        public static TweenColor ColorFromG(Graphic graphic, float duration, int g)
        {
            return ColorFromG(graphic, duration, g / 255.0f);
        }

        /// <summary>
        /// 指定された色からB成分を現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="b">変化前のB成分</param>
        /// <returns></returns>
        public static TweenColor ColorFromB(Graphic graphic, float duration, float b)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.B = b;
            return ColorFrom(graphic, order);
        }

        /// <summary>
        /// 指定された色からB成分を現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="b">変化前のB成分</param>
        /// <returns></returns>
        public static TweenColor ColorFromB(Graphic graphic, float duration, int b)
        {
            return ColorFromB(graphic, duration, b / 255.0f);
        }

        /// <summary>
        /// 指定された色からA成分を現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="a">変化前のA成分</param>
        /// <returns></returns>
        public static TweenColor ColorFromA(Graphic graphic, float duration, float a)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.A = a;
            return ColorFrom(graphic, order);
        }

        /// <summary>
        /// 指定された色からA成分を現在の色へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="a">変化前のA成分</param>
        /// <returns></returns>
        public static TweenColor ColorFromA(Graphic graphic, float duration, int a)
        {
            return ColorFromA(graphic, duration, a / 255.0f);
        }

#endregion

#region color by

        /// <summary>
        /// 現在の色から指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分のオフセット</param>
        /// <param name="g">G成分のオフセット</param>
        /// <param name="b">B成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorBy(Graphic graphic, float duration, float r, float g, float b)
        {
            TweenColor tween = new TweenColor();
            Color to = graphic.color;
            to.r += r;
            to.g += g;
            to.b += b;

            tween.Begin = graphic.color;
            tween.Final = to;
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Color color = t.Value;
                color.a = graphic.color.a;
                graphic.color = color;
            });

            return tween;
        }

        /// <summary>
        /// 現在の色から指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分のオフセット</param>
        /// <param name="g">G成分のオフセット</param>
        /// <param name="b">B成分のオフセット</param>
        /// <param name="a">A成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorBy(Graphic graphic, float duration, float r, float g, float b, float a)
        {
            TweenColor tween = new TweenColor();
            Color to = graphic.color;
            to.r += r;
            to.g += g;
            to.b += b;
            to.a += a;

            tween.Begin = graphic.color;
            tween.Final = to;
            tween.Duration = duration;
            tween.OnUpdate(t => graphic.color = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在の色から指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分のオフセット</param>
        /// <param name="g">G成分のオフセット</param>
        /// <param name="b">B成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorBy(Graphic graphic, float duration, int r, int g, int b)
        {
            return ColorBy(graphic, duration, r / 255.0f, g / 255.0f, b / 255.0f);
        }

        /// <summary>
        /// 現在の色から指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分のオフセット</param>
        /// <param name="g">G成分のオフセット</param>
        /// <param name="b">B成分のオフセット</param>
        /// <param name="a">A成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorBy(Graphic graphic, float duration, int r, int g, int b, int a)
        {
            return ColorBy(graphic, duration, r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }

        /// <summary>
        /// 現在の色から指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="offset">色のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorBy(Graphic graphic, float duration, Color offset)
        {
            return ColorBy(graphic, duration, offset.r, offset.g, offset.b, offset.a);
        }

        /// <summary>
        /// 現在の色から指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenColor ColorBy(Graphic graphic, TweenColorOrder order)
        {
            TweenColor tween = new TweenColor();
            order.SetProperties(tween);

            tween.Begin = graphic.color;
            tween.Final = order.GetByValue(graphic.color);
            tween.OnUpdate(order.CreateOnUpdate(graphic));

            return tween;
        }

        /// <summary>
        /// 現在の色からR成分を指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorByR(Graphic graphic, float duration, float r)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.R = r;
            return ColorBy(graphic, order);
        }

        /// <summary>
        /// 現在の色からR成分を指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="r">R成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorByR(Graphic graphic, float duration, int r)
        {
            return ColorByR(graphic, duration, r / 255.0f);
        }

        /// <summary>
        /// 現在の色からB成分を指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="g">G成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorByG(Graphic graphic, float duration, float g)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.G = g;
            return ColorBy(graphic, order);
        }

        /// <summary>
        /// 現在の色からB成分を指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="g">G成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorByG(Graphic graphic, float duration, int g)
        {
            return ColorByG(graphic, duration, g / 255.0f);
        }

        /// <summary>
        /// 現在の色からG成分を指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="b">B成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorByB(Graphic graphic, float duration, float b)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.B = b;
            return ColorBy(graphic, order);
        }

        /// <summary>
        /// 現在の色からG成分を指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="b">B成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorByB(Graphic graphic, float duration, int b)
        {
            return ColorByB(graphic, duration, b / 255.0f);
        }

        /// <summary>
        /// 現在の色からA成分を指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="a">A成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorByA(Graphic graphic, float duration, float a)
        {
            TweenColorOrder order = new TweenColorOrder(duration);
            order.A = a;
            return ColorBy(graphic, order);
        }

        /// <summary>
        /// 現在の色からA成分を指定されたオフセット値分色を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="a">A成分のオフセット</param>
        /// <returns></returns>
        public static TweenColor ColorByA(Graphic graphic, float duration, int a)
        {
            return ColorByA(graphic, duration, a / 255.0f);
        }

#endregion

#endregion

#region alpha utils

#region alpha to

        /// <summary>
        /// 現在のアルファ値から指定されたアルファ値へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="to">変化後のアルファ値</param>
        /// <returns></returns>
        public static TweenSingle AlphaTo(Graphic graphic, float duration, float to)
        {
            TweenSingle tween = new TweenSingle();

            tween.Begin = graphic.color.a;
            tween.Final = to;
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Color color = graphic.color;
                color.a = t.Value;
                graphic.color = color;
            });

            return tween;
        }

        /// <summary>
        /// 現在のアルファ値から指定されたアルファ値へと変化するTweenを返します。
        /// </summary>
        /// <param name="canvasGroup">CanvasGroup</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="to">変化後のアルファ値</param>
        /// <returns></returns>
        public static TweenSingle AlphaTo(CanvasGroup canvasGroup, float duration, float to)
        {
            TweenSingle tween = new TweenSingle();

            tween.Begin = canvasGroup.alpha;
            tween.Final = to;
            tween.Duration = duration;
            tween.OnUpdate(t => canvasGroup.alpha = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在のアルファ値から指定されたアルファ値へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenSingle AlphaTo(Graphic graphic, TweenAlphaOrder order)
        {
            TweenSingle tween = new TweenSingle();
            order.SetProperties(tween);

            tween.Begin = graphic.color.a;
            tween.Final = order.GetToValue();
            tween.OnUpdate(order.CreateOnUpdate(graphic));

            return tween;
        }

        /// <summary>
        /// 現在のアルファ値から指定されたアルファ値へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenSingle AlphaTo(CanvasGroup canvasGroup, TweenAlphaOrder order)
        {
            TweenSingle tween = new TweenSingle();
            order.SetProperties(tween);

            tween.Begin = canvasGroup.alpha;
            tween.Final = order.GetToValue();
            tween.OnUpdate(order.CreateOnUpdate(canvasGroup));

            return tween;
        }

#endregion

#region alpha from

        /// <summary>
        /// 指定されたアルファ値から現在のアルファ値へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="from">変化後のアルファ値</param>
        /// <returns></returns>
        public static TweenSingle AlphaFrom(Graphic graphic, float duration, float from)
        {
            TweenSingle tween = new TweenSingle();

            tween.Begin = from;
            tween.Final = graphic.color.a;
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Color color = graphic.color;
                color.a = t.Value;
                graphic.color = color;
            });

            return tween;
        }

        /// <summary>
        /// 指定されたアルファ値から現在のアルファ値へと変化するTweenを返します。
        /// </summary>
        /// <param name="canvasGroup">CanvasGroup</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="from">変化後のアルファ値</param>
        /// <returns></returns>
        public static TweenSingle AlphaFrom(CanvasGroup canvasGroup, float duration, float from)
        {
            TweenSingle tween = new TweenSingle();

            tween.Begin = from;
            tween.Final = canvasGroup.alpha;
            tween.Duration = duration;
            tween.OnUpdate(t => canvasGroup.alpha = t.Value);

            return tween;
        }

        /// <summary>
        /// 指定されたアルファ値から現在のアルファ値へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenSingle AlphaFrom(Graphic graphic, TweenAlphaOrder order)
        {
            TweenSingle tween = new TweenSingle();
            order.SetProperties(tween);

            tween.Begin = order.GetFromValue();
            tween.Final = graphic.color.a;
            tween.OnUpdate(order.CreateOnUpdate(graphic));

            return tween;
        }

        /// <summary>
        /// 指定されたアルファ値から現在のアルファ値へと変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenSingle AlphaFrom(CanvasGroup canvasGroup, TweenAlphaOrder order)
        {
            TweenSingle tween = new TweenSingle();
            order.SetProperties(tween);

            tween.Begin = order.GetFromValue();
            tween.Final = canvasGroup.alpha;
            tween.OnUpdate(order.CreateOnUpdate(canvasGroup));

            return tween;
        }

#endregion

#region alpha by

        /// <summary>
        /// 現在のアルファ値から指定されたオフセット値分アルファ値を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="offset">アルファ値のオフセット値</param>
        /// <returns></returns>
        public static TweenSingle AlphaBy(Graphic graphic, float duration, float offset)
        {
            TweenSingle tween = new TweenSingle();

            tween.Begin = graphic.color.a;
            tween.Final = graphic.color.a + offset;
            tween.Duration = duration;
            tween.OnUpdate(t =>
            {
                Color color = graphic.color;
                color.a = t.Value;
                graphic.color = color;
            });

            return tween;
        }

        /// <summary>
        /// 現在のアルファ値から指定されたオフセット値分アルファ値を変化するTweenを返します。
        /// </summary>
        /// <param name="canvasGroup">CanvasGroup</param>
        /// <param name="duration">Tweenによる補間が行われる時間(秒)</param>
        /// <param name="offset">アルファ値のオフセット値</param>
        /// <returns></returns>
        public static TweenSingle AlphaBy(CanvasGroup canvasGroup, float duration, float offset)
        {
            TweenSingle tween = new TweenSingle();

            tween.Begin = canvasGroup.alpha;
            tween.Final = canvasGroup.alpha + offset;
            tween.Duration = duration;
            tween.OnUpdate(t => canvasGroup.alpha = t.Value);

            return tween;
        }

        /// <summary>
        /// 現在のアルファ値から指定されたオフセット値分アルファ値を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenSingle AlphaBy(Graphic graphic, TweenAlphaOrder order)
        {
            TweenSingle tween = new TweenSingle();
            order.SetProperties(tween);

            tween.Begin = graphic.color.a;
            tween.Final = order.GetByValue(graphic.color.a);
            tween.OnUpdate(order.CreateOnUpdate(graphic));

            return tween;
        }

        /// <summary>
        /// 現在のアルファ値から指定されたオフセット値分アルファ値を変化するTweenを返します。
        /// </summary>
        /// <param name="graphic">Graphic</param>
        /// <param name="order">Tweenの動作情報</param>
        /// <returns></returns>
        public static TweenSingle AlphaBy(CanvasGroup canvasGroup, TweenAlphaOrder order)
        {
            TweenSingle tween = new TweenSingle();
            order.SetProperties(tween);

            tween.Begin = canvasGroup.alpha;
            tween.Final = order.GetByValue(canvasGroup.alpha);
            tween.OnUpdate(order.CreateOnUpdate(canvasGroup));

            return tween;
        }

#endregion

#endregion

        /// <summary>
        /// AnimationCurveのキー情報を反転させたものを返します。
        /// </summary>
        /// <param name="anim">AnimationCurve</param>
        /// <returns></returns>
        public static AnimationCurve MirrorAnimationCurve(AnimationCurve anim)
        {
            var allKeyframes = anim.keys;
            var length = allKeyframes.Length;

            var times = new float[length];
            var inTangents = new float[length];
            var outTangents = new float[length];

            for (int j=0; j<allKeyframes.Length; j++)
            {
                inTangents[j] = allKeyframes[j].inTangent;
                outTangents[j] = allKeyframes[j].outTangent;
                times[j] = allKeyframes[j].time;
            }

            for (int i=0; i<allKeyframes.Length; i++)
            {
                allKeyframes[i].time = 1f - times[i];//ok
                allKeyframes[i].inTangent = inTangents[i] * -1f;
                allKeyframes[i].outTangent = outTangents[i] * -1f;
            }

            return new AnimationCurve(allKeyframes);
        }

#endregion
    }
}
