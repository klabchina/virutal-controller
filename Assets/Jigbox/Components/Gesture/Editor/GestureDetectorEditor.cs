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
using UnityEditor;
using System;
using System.Collections.Generic;
using Jigbox.UIControl;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    [CustomEditor(typeof(GestureDetector), true)]
    public class GestureDetectorEditor : Editor
    {
#region inner classes, enum, and structs

        /// <summary>
        /// 基本となるジェスチャーの判別用クラスの種類
        /// </summary>
        public enum BasicGestureUmpireType
        {
            /// <summary>特定位置への入力によるジェスチャー</summary>
            Pointing,
            /// <summary>移動によるジェスチャー</summary>
            Drag,
            /// <summary>特定方向への移動によるジェスチャー</summary>
            Swipe,
            /// <summary>特定方向への加速度の大きな移動によるジェスチャー</summary>
            Flick,
            /// <summary>2点間の距離の変化によるジェスチャー</summary>
            Pinch,
            /// <summary>2点間の相対角度の変化によるジェスチャー</summary>
            Rotate
        }

#endregion

#region properties

        /// <summary>ジェスチャーの検出用コンポーネント</summary>
        protected GestureDetector detector;
        
        /// <summary>入力の排他制御用クラスのパラメータ</summary>
        protected SerializedProperty colleagueProperty;

        /// <summary>ジェスチャーの判別用クラスのパラメータ</summary>
        protected SerializedProperty umpiresProperty;

        /// <summary>アタッチされていないジェスチャーの判別クラス</summary>
        protected string[] notExistUmpires;

        /// <summary>アタッチされていないジェスチャーの判別クラスを更新する必要があるかどうか</summary>
        protected bool isNeedUpdateNotExistUmpires = false;

#endregion

#region protected methods

        /// <summary>
        /// 追加するジェスチャーの判別クラスを選択して、コンポーネントをアタッチします。
        /// </summary>
        protected void SelectNewUmpire()
        {
            // 追加するものがなければ表示しない
            if (notExistUmpires.Length == 1)
            {
                return;
            }

            int selectIndex = EditorGUILayout.Popup(0, notExistUmpires);
            // 0は初期表示用の固定文言
            if (selectIndex > 0)
            {
                BasicGestureUmpireType umpire = (BasicGestureUmpireType) Enum.Parse(typeof(BasicGestureUmpireType), notExistUmpires[selectIndex]);
                AddUmpireBehaviour(umpire);
                isNeedUpdateNotExistUmpires = true;
            }
        }

        /// <summary>
        /// ジェスチャーの判別用コンポーネントを追加でアタッチします。
        /// </summary>
        /// <param name="type">ジェスチャーの判別クラスの種類</param>
        protected void AddUmpireBehaviour(BasicGestureUmpireType type)
        {
            switch (type)
            {
                case BasicGestureUmpireType.Pointing:
                    detector.gameObject.AddComponent<PointingGestureUmpire>();
                    break;
                case BasicGestureUmpireType.Drag:
                    detector.gameObject.AddComponent<DragGestureUmpire>();
                    break;
                case BasicGestureUmpireType.Swipe:
                    detector.gameObject.AddComponent<SwipeGestureUmpire>();
                    break;
                case BasicGestureUmpireType.Flick:
                    detector.gameObject.AddComponent<FlickGestureUmpire>();
                    break;
                case BasicGestureUmpireType.Pinch:
                    detector.gameObject.AddComponent<PinchGestureUmpire>();
                    break;
                case BasicGestureUmpireType.Rotate:
                    detector.gameObject.AddComponent<RotateGestureUmpire>();
                    break;
            }
        }

        /// <summary>
        /// アタッチされていないジェスチャーの判別クラスを更新します。
        /// </summary>
        /// <param name="ignore">除外する種類</param>
        protected virtual void UpdateNotExistUmpires()
        {
            List<string> notExistUmpires = new List<string>();
            // 先頭は必ず固定文言にしておく
            notExistUmpires.Add("Add Umpire");
            Array types = Enum.GetValues(typeof(BasicGestureUmpireType));

            foreach (BasicGestureUmpireType type in types)
            {
                bool isExist = false;
                for (int i = 0; i < umpiresProperty.arraySize; ++i)
                {
                    SerializedProperty umpireProperty = umpiresProperty.GetArrayElementAtIndex(i);
                    if (CompareTypeToComponent(type, umpireProperty.objectReferenceValue))
                    {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist)
                {
                    notExistUmpires.Add(type.ToString());
                }
            }

            this.notExistUmpires = notExistUmpires.ToArray();
        }

        /// <summary>
        /// ジェスチャーの判別クラスの種類とコンポーネントのクラスを比較します。
        /// </summary>
        /// <param name="type">ジェスチャーの判別クラスの種類</param>
        /// <param name="target">コンポーネント</param>
        /// <returns></returns>
        protected bool CompareTypeToComponent(BasicGestureUmpireType type, UnityEngine.Object target)
        {
            switch (type)
            {
                case BasicGestureUmpireType.Pointing:
                    return target is PointingGestureUmpire;
                case BasicGestureUmpireType.Drag:
                    return target is DragGestureUmpire;
                case BasicGestureUmpireType.Swipe:
                    return target is SwipeGestureUmpire;
                case BasicGestureUmpireType.Flick:
                    return target is FlickGestureUmpire;
                case BasicGestureUmpireType.Pinch:
                    return target is PinchGestureUmpire;
                case BasicGestureUmpireType.Rotate:
                    return target is RotateGestureUmpire;
            }
            return false;
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            detector = target as GestureDetector;

            SerializedProperty raycastAreaProperty = serializedObject.FindProperty("raycastArea");
            colleagueProperty = serializedObject.FindProperty("colleague");
            umpiresProperty = serializedObject.FindProperty("umpires");

            RaycastArea raycastArea = detector.GetComponent<RaycastArea>();
            if (raycastAreaProperty.objectReferenceValue != raycastArea)
            {
                serializedObject.Update();
                raycastAreaProperty.objectReferenceValue = raycastArea;
                serializedObject.ApplyModifiedProperties();
            }

            isNeedUpdateNotExistUmpires = true;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            bool edited = EditorGUILayout.ToggleLeft("Is Enable", detector.IsEnable);
            if (detector.IsEnable != edited)
            {
                detector.IsEnable = edited;
            }

            serializedObject.Update();

            if (isNeedUpdateNotExistUmpires)
            {
                UpdateNotExistUmpires();
                isNeedUpdateNotExistUmpires = false;
            }

            SubmitColleagueEditor.DrawEdit(colleagueProperty);

            serializedObject.ApplyModifiedProperties();

            SelectNewUmpire();

            EditorUtilsTools.RegisterUndo("Edit Gesture Detector", GUI.changed, target);
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
