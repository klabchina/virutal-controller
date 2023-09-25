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

using UnityEditor;
using UnityEngine;
using Jigbox.EditorUtils;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ToggleSwitchTransitionBase), true)]
    public class ToggleSwitchTransitionEditor : Editor
    {
#region properties

        /// <summary>ToggleSwitchのトランジションの基礎クラス</summary>
        protected ToggleSwitchTransitionBase transition;

        /// <summary>トランジションの対象となるGameObjectのプロパティ</summary>
        protected SerializedProperty knobProperty;

        /// <summary>ON状態の座標のプロパティ</summary>
        protected SerializedProperty positionOnProperty;

        /// <summary>OFF状態の座標のプロパティ</summary>
        protected SerializedProperty positionOffProperty;

        /// <summary>トランジションの時間のプロパティ</summary>
        protected SerializedProperty durationProperty;

        /// <summary>トランジション対象のGameObjectのRectTransform</summary>
        protected RectTransform knobRectTransform;

        /// <summary>ON状態でのワールド座標</summary>
        protected Vector3 positionOnWorld;

        /// <summary>OFF状態でのワールド座標</summary>
        protected Vector3 positionOffWorld;
        
        /// <summary>ToggleSwitchがついているGameObjectの前のPosition</summary>
        protected Vector3 lastPosition;

        /// <summary> 選択中のターゲット数 </summary>
        protected int targetsLength;

#endregion

#region protected methods

        /// <summary>
        /// 座標を編集します。
        /// </summary>
        /// <param name="property">SerializedProperty</param>
        /// <param name="label">ラベル</param>
        protected void EditPosition(SerializedProperty property, string label)
        {
            EditorGUILayout.BeginHorizontal();
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorUtilsTools.SetLabelWidth(35.0f);

                EditorGUILayout.LabelField(label);

                bool isReset = GUILayout.Button("P", GUILayout.Width(25));

                EditorUtilsTools.SetLabelWidth(15.0f);

                EditorGUILayout.PropertyField(property.FindPropertyRelative("x"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("y"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("z"));

                EditorUtilsTools.SetLabelWidth(labelWidth);

                if (isReset)
                {
                    property.vector3Value = Vector3.zero;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// トランジション対象の仮想位置を表示します。
        /// </summary>
        /// <param name="position">表示座標</param>
        /// <param name="color">描画色</param>
        /// <param name="label">ラベル</param>
        protected void DrawRect(Vector3 position, Color color, string label)
        {
            Vector2 size = knobRectTransform.rect.size;
            size.x *= knobRectTransform.lossyScale.x;
            size.y *= knobRectTransform.lossyScale.y;
            
            Vector3 leftBottom = position;
            leftBottom.x -= size.x * knobRectTransform.pivot.x;
            leftBottom.y -= size.y * knobRectTransform.pivot.y;
            Rect rect = new Rect(leftBottom.x, leftBottom.y, size.x, size.y);

            // rectのままでもレンダリングできるがz軸方向の位置が飛ぶので頂点に変換する
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(rect.xMin, rect.yMax, position.z),
                new Vector3(rect.xMax, rect.yMax, position.z),
                new Vector3(rect.xMax, rect.yMin, position.z),
                new Vector3(rect.xMin, rect.yMin, position.z)
            };

            Color faceColor = color;
            faceColor.a = 0.05f;

            Handles.DrawSolidRectangleWithOutline(vertices, faceColor, color);
            Handles.Label(position, label);
        }

        /// <summary>
        /// トランジション位置の操作用ハンドルを表示します。
        /// </summary>
        /// <param name="position">ハンドルの表示位置</param>
        /// <returns>編集後の位置</returns>
        protected Vector3 DrawHandle(Vector3 position)
        {
            Handles.color = Handles.xAxisColor;
            position = Handles.Slider(position, knobRectTransform.right);

            Handles.color = Handles.yAxisColor;
            position = Handles.Slider(position, knobRectTransform.up);

            Handles.color = Handles.zAxisColor;
            position = Handles.Slider(position, knobRectTransform.forward);

            Handles.color = Color.white;

            return position;
        }

        /// <summary>
        /// トランジション対象のGameObjectのワールド座標を取得します。
        /// </summary>
        /// <param name="localPosition">ローカル座標</param>
        /// <returns></returns>
        protected Vector3 GetWorldPosition(Vector3 localPosition)
        {
            Vector3 worldPosition;
            Vector3 cachedPosition = knobRectTransform.localPosition;

            knobRectTransform.localPosition = Vector3.zero;
            worldPosition = knobRectTransform.TransformPoint(localPosition);

            knobRectTransform.localPosition = cachedPosition;
            return worldPosition;
        }

        /// <summary>
        /// トランジション対象のGameObjectのローカル座標を取得します。
        /// </summary>
        /// <param name="worldPosition">ワールド座標</param>
        /// <returns></returns>
        protected Vector3 GetLocalPosition(Vector3 worldPosition)
        {
            Vector3 localPosition;
            Vector3 cachedPosition = knobRectTransform.localPosition;

            knobRectTransform.position = worldPosition;
            localPosition = knobRectTransform.localPosition;

            knobRectTransform.localPosition = cachedPosition;
            return localPosition;
        }

#endregion
        
#region override unity methods

        protected virtual void OnEnable()
        {
            transition = target as ToggleSwitchTransitionBase;

            knobProperty = serializedObject.FindProperty("knob");
            positionOnProperty = serializedObject.FindProperty("positionOn");
            positionOffProperty = serializedObject.FindProperty("positionOff");
            durationProperty = serializedObject.FindProperty("duration");
            // OnSceneGUI で targets へのアクセスがエラーを吐かれる為、ここで数値をとっておく
            targetsLength = targets.Length;
            
            GameObject knob = knobProperty.objectReferenceValue as GameObject;
            if (knob != null)
            {
                knobRectTransform = knob.transform as RectTransform;
            }

            if (knobRectTransform != null)
            {
                positionOnWorld = GetWorldPosition(positionOnProperty.vector3Value);
                positionOffWorld = GetWorldPosition(positionOffProperty.vector3Value);
            }

            // ToggleSwitchが同一GameObjectにアタッチされている場合、自身の参照を設定する
            foreach (Object obj in targets)
            {
                ToggleSwitchTransitionBase transitionBase = obj as ToggleSwitchTransitionBase;
                ToggleSwitch toggleSwitch = transitionBase.GetComponent<ToggleSwitch>();
                if (toggleSwitch == null)
                {
                    continue;
                }

                SerializedObject toggleSwitchObject = new SerializedObject(toggleSwitch);
                toggleSwitchObject.Update();
                SerializedProperty transitionProperty = toggleSwitchObject.FindProperty("transition");
                transitionProperty.objectReferenceValue = transitionBase;
                toggleSwitchObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            bool isRefresh = false;

            serializedObject.Update();

            // 複数選択時には設定させない
            EditorGUI.BeginDisabledGroup(targets.Length > 1);
            GameObject knob = EditorGUILayout.ObjectField("Knob", knobProperty.objectReferenceValue, typeof(GameObject), true) as GameObject;
            if (knob == transition.gameObject)
            {
                Debug.LogError("Can't use own gameObject!");
                knob = null;
            }
            if (knob != knobProperty.objectReferenceValue)
            {
                knobProperty.objectReferenceValue = knob;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Position");
                EditPosition(positionOnProperty, "ON");
                EditPosition(positionOffProperty, "OFF");
                isRefresh = GUI.changed;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(durationProperty);

            serializedObject.ApplyModifiedProperties();

            // transitionのpositionが変更された場合は表示位置を追従させるために更新する
            isRefresh |= lastPosition != transition.transform.position;

            // シリアライズ情報操作中にプロパティ変更とかやると処理がバッティングするので、
            // 編集が終わってから計算する
            if (isRefresh)
            {
                if (knob != null)
                {
                    knobRectTransform = knob.transform as RectTransform;
                }
                else
                {
                    knobRectTransform = null;
                }

                if (knobRectTransform != null)
                {
                    positionOnWorld = GetWorldPosition(positionOnProperty.vector3Value);
                    positionOffWorld = GetWorldPosition(positionOffProperty.vector3Value);
                }
            }

            EditorUtilsTools.RegisterUndo("Edit ToggleSwitch", GUI.changed, targets);

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

        protected virtual void OnSceneGUI()
        {
            if (knobRectTransform == null)
            {
                return;
            }

            if (targetsLength > 1)
            {
                return;
            }

            // GUI.changed push.
            EditorGUI.BeginChangeCheck();

            DrawRect(positionOnWorld, Color.blue, "ON");
            Vector3 editedPositionOn = DrawHandle(positionOnWorld);

            DrawRect(positionOffWorld, Color.red, "OFF");
            Vector3 editedPositionOff = DrawHandle(positionOffWorld);
            
            Handles.DrawAAPolyLine(5.0f, editedPositionOn, editedPositionOff);

            // 値が編集済みの場合、座標を変換してシリアライズ情報を更新する
            if (editedPositionOn != positionOnWorld)
            {
                positionOnWorld = editedPositionOn;
                editedPositionOn = GetLocalPosition(editedPositionOn);
                transition.PositionOn = editedPositionOn;
            }
            if (editedPositionOff != positionOffWorld)
            {
                positionOffWorld = editedPositionOff;
                editedPositionOff = GetLocalPosition(editedPositionOff);
                transition.PositionOff = editedPositionOff;
            }

            EditorUtilsTools.RegisterUndo("Edit ToggleSwitch", GUI.changed, target);

            lastPosition = transition.transform.position;

            // GUI.changed pop.
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
