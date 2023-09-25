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
using UnityEditor;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    public static class ToggleMenuEditor
    {
        [MenuItem("GameObject/UI/Jigbox/Toggle/BasicToggle")]
        public static void CreateBasicToggle()
        {
            GameObject toggle = new GameObject("Basic Toggle", typeof(RectTransform));

#pragma warning disable 219
            BasicToggle basicToggleComponent = toggle.AddComponent<BasicToggle>();
#pragma warning restore 219

            GameObject image = new GameObject("Image", typeof(Image));
            image.transform.SetParent(toggle.transform, false);

            EditorMenuUtils.CreateUIObject(toggle);
        }

        [MenuItem("GameObject/UI/Jigbox/Toggle/ColorToggle")]
        public static void CreateColorToggle()
        {
            GameObject toggle = new GameObject("Color Toggle", typeof(RectTransform));

            ColorToggle colorToggleComponent = toggle.AddComponent<ColorToggle>();

            GameObject image = new GameObject("Image", typeof(Image));
            image.transform.SetParent(toggle.transform, false);

            BasicButton basicButtonComponent = toggle.GetComponent<BasicButton>();
            ButtonBaseEditor.AddEvent(basicButtonComponent, InputEventType.OnClick, colorToggleComponent, "OnClick");

            EditorMenuUtils.CreateUIObject(toggle);
        }

        [MenuItem("GameObject/UI/Jigbox/Toggle/GameObjectToggle")]
        public static void CreateGameObjectToggle()
        {
            GameObject toggle = new GameObject("Game Object Toggle", typeof(RectTransform));

            GameObjectToggle gameObjectToggleComponent = toggle.AddComponent<GameObjectToggle>();

            GameObject imageOn = new GameObject("ImageOn", typeof(Image));
            GameObject imageOff = new GameObject("ImageOff", typeof(Image));

            gameObjectToggleComponent.OnStateGameObject = imageOn;
            gameObjectToggleComponent.OffStateGameObject = imageOff;

            imageOn.transform.SetParent(toggle.transform, false);
            imageOff.transform.SetParent(toggle.transform, false);

            EditorMenuUtils.CreateUIObject(toggle);
        }

        [MenuItem("GameObject/UI/Jigbox/Toggle/ToggleSwitch")]
        public static void CreateToggleSwitch()
        {
            // 120 x 40、OFF状態で生成する

            // ToggleSwitch本体を生成
            GameObject toggleSwitch = new GameObject("ToggleSwitch");
            ToggleSwitch toggle = toggleSwitch.AddComponent<ToggleSwitch>();
            toggle.IsOn = false;
            ToggleSwitchTransitionBase transition = toggleSwitch.AddComponent<BasicToggleSwitchTransition>();
            toggleSwitch.AddComponent<DragBehaviour>();

            // 初期サイズ設定
            RectTransform toggleTransform = toggleSwitch.transform as RectTransform;
            toggleTransform.sizeDelta = new Vector2(120.0f, 40.0f);

            // 背景用画像生成
            GameObject bg = new GameObject("Bg");
            Image bgImage = UIContainerModifier.AddDefaultSprite(bg);
            bgImage.color = Color.gray;
            RectTransform bgTransform = bg.transform as RectTransform;
            bgTransform.sizeDelta = new Vector2(120.0f, 40.0f);


            // つまみ用画像生成
            GameObject knob = new GameObject("Knob");
            UIContainerModifier.AddDefaultSprite(knob);
            RectTransform knobTransform = knob.transform as RectTransform;
            knobTransform.sizeDelta = new Vector2(60.0f, 40.0f);

            // ToggleSwitch以下に画像オブジェクトを配置
            bg.transform.SetParent(toggleTransform, false);
            knob.transform.SetParent(toggleTransform, false);
            knob.transform.localPosition = new Vector3(-30.0f, 0.0f);

            // トランジション用コンポーネントの値を設定
            SerializedObject serializedObject = new SerializedObject(transition);
            
            SerializedProperty knobProperty = serializedObject.FindProperty("knob");
            SerializedProperty positionOnProperty = serializedObject.FindProperty("positionOn");
            SerializedProperty positionOffProperty = serializedObject.FindProperty("positionOff");

            serializedObject.Update();

            knobProperty.objectReferenceValue = knob;
            positionOnProperty.vector3Value = new Vector3(30.0f, 0.0f);
            positionOffProperty.vector3Value = new Vector3(-30.0f, 0.0f);
            
            serializedObject.ApplyModifiedProperties();

            // Hierarchyに配置
            EditorMenuUtils.CreateUIObject(toggleSwitch);
        }
    }
}
