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
using System;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    public static class VirtualPadMenuEditor
    {
#region public methods

        [MenuItem("GameObject/UI/Jigbox/VirtualPad")]
        public static void CreateDetector()
        {
            GameObject detector = new GameObject("GestureDetector");
            detector.AddComponent<GestureDetector>();
            VirtualPadController virtualPad = detector.AddComponent<VirtualPadController>();

            GameObject view = new GameObject("VirtualPadView", typeof(RectTransform));
            VirtualPadView virtualPadView = view.AddComponent<VirtualPadView>();
            RectTransform viewRectTransform = view.transform as RectTransform;
            viewRectTransform.SetParent(detector.transform, false);

            GameObject bg = new GameObject("Bg");
            Image bgImage = bg.AddComponent<Image>();
            bgImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            RectTransform bgRectTransform = bg.transform as RectTransform;
            bgRectTransform.sizeDelta = new Vector2(150.0f, 150.0f);
            bgRectTransform.SetParent(viewRectTransform, false);

            GameObject handle = new GameObject("Handle");
            Image handleImage = handle.AddComponent<Image>();
            handleImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            RectTransform handleRectTransform = handle.transform as RectTransform;
            handleRectTransform.SetParent(viewRectTransform, false);

            {
                SerializedObject serializedObject = new SerializedObject(virtualPad);
                SerializedProperty viewProperty = serializedObject.FindProperty("view");

                serializedObject.Update();
                viewProperty.objectReferenceValue = virtualPadView;
                serializedObject.ApplyModifiedProperties();
            }
            {
                SerializedObject serializedObject = new SerializedObject(virtualPadView);
                SerializedProperty handleProperty = serializedObject.FindProperty("handle");

                serializedObject.Update();
                handleProperty.objectReferenceValue = handleRectTransform;
                serializedObject.ApplyModifiedProperties();
            }

            EditorMenuUtils.CreateUIObject(detector);
        }

#endregion
    }
}
