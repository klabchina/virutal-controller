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

using Jigbox.Delegatable;
using Jigbox.EditorUtils;
using Jigbox.UIControl;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Jigbox.Components
{
    public class BalloonMenuEditor
    {
#region property

        static readonly float balloonLayoutPositionRate = 0.0f;
        static readonly float spacing = 10.0f;

#endregion

#region public methods

        [MenuItem("GameObject/UI/Jigbox/Balloon/Balloon")]
        public static void CreateBalloon()
        {
            // バルーンオブジェクトの作成
            GameObject balloon = new GameObject("Balloon", typeof(RectTransform));
            var balloonComponent = balloon.AddComponent<Balloon>();
            var calculatorComponent = balloon.AddComponent<BalloonLayoutCalculator>();
            var transitionComponent = balloon.AddComponent<BasicBalloonTransition>();

            // バルーンで表示するオブジェクトの作成
            GameObject content = new GameObject("BalloonContent", typeof(Image));
            content.transform.SetParent(balloon.transform);

            var balloonProperty = new SerializedObject(balloonComponent);
            var modelProperty = balloonProperty.FindProperty("balloonModel");
            var calculatorProperty = modelProperty.FindPropertyRelative("calculator");
            var balloonLayoutPositionRateProperty = modelProperty.FindPropertyRelative("balloonLayoutPositionRate");
            var balloonContentProperty = modelProperty.FindPropertyRelative("balloonContent");
            var balloonTransitionProperty = balloonProperty.FindProperty("balloonTransition");
            var balloonSpacingProperty = modelProperty.FindPropertyRelative("spacing");

            balloonProperty.Update();

            calculatorProperty.objectReferenceValue = calculatorComponent;
            balloonLayoutPositionRateProperty.floatValue = balloonLayoutPositionRate;
            balloonContentProperty.objectReferenceValue = balloon.transform as RectTransform;
            balloonTransitionProperty.objectReferenceValue = transitionComponent;
            balloonSpacingProperty.floatValue = spacing;

            balloonProperty.ApplyModifiedProperties();

            EditorMenuUtils.CreateUIObject(balloon);
        }

        [MenuItem("GameObject/UI/Jigbox/Balloon/BalloonLayer")]
        public static void CreateBalloonLayer()
        {
            // バルーンレイヤーオブジェクトの作成
            GameObject balloonLayer = new GameObject("BalloonLayer", typeof(RectTransform));
            var balloonLayerComponent = balloonLayer.AddComponent<BalloonLayer>();
            var layerRectTranform = balloonLayer.transform as RectTransform;

            // BlockerはCanvas全体にかかるようにするためフルストレッチ
            RectTransformUtils.SetAnchor(layerRectTranform, RectTransformUtils.AnchorPoint.StretchFull);
            RectTransformUtils.SetSize(layerRectTranform, Vector2.zero);

            // InputBlockerオブジェクト作成
            GameObject inputBlocker = new GameObject("BalloonInputBlocker", typeof(RectTransform));
            var inputBlockerRectTransform = inputBlocker.transform as RectTransform;
            RectTransformUtils.SetAnchor(inputBlockerRectTransform, RectTransformUtils.AnchorPoint.StretchFull);
            RectTransformUtils.SetSize(inputBlockerRectTransform, Vector2.zero);

            // Clickイベントを受け取るボタンコンポーネントをアタッチする
            var buttonComponent = inputBlocker.AddComponent<ButtonBase>();

            // デリゲートの登録
            var eventEntry = new ButtonBase.EventEntry(InputEventType.OnClick);
            buttonComponent.Entries.Add(eventEntry);

            DelegatableObjectEditor.AddDelegate(eventEntry.Delegates, balloonLayer.GetComponent<MonoBehaviour>(),
                "OnClickBlocker", typeof(AuthorizedAccessAttribute));

            inputBlocker.transform.SetParent(balloonLayer.transform);

            //Blocker用Imageの作成
            GameObject image = new GameObject("Image", typeof(Image));
            var imageComponent = image.GetComponent<Image>();
            imageComponent.color = Color.clear;

            var imageRectTransform = image.transform as RectTransform;
            RectTransformUtils.SetAnchor(imageRectTransform, RectTransformUtils.AnchorPoint.StretchFull);
            RectTransformUtils.SetSize(imageRectTransform, Vector2.zero);

            image.transform.SetParent(inputBlocker.transform);

            // 参照更新
            var balloonLayerProperty = new SerializedObject(balloonLayerComponent);
            var inputBlockerProperty = balloonLayerProperty.FindProperty("inputBlocker");

            balloonLayerProperty.Update();

            inputBlockerProperty.objectReferenceValue = inputBlocker;

            balloonLayerProperty.ApplyModifiedProperties();

            EditorMenuUtils.CreateUIObject(balloonLayer);
        }

#endregion
    }
}
