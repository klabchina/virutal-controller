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
using UnityEngine.UI;
using System;
using Jigbox.EditorUtils;
using Jigbox.Delegatable;

using FillMethod = UnityEngine.UI.Image.FillMethod;
using OriginHorizontal = UnityEngine.UI.Image.OriginHorizontal;
using OriginVertical = UnityEngine.UI.Image.OriginVertical;
using Origin90 = UnityEngine.UI.Image.Origin90;
using Origin180 = UnityEngine.UI.Image.Origin180;
using Origin360 = UnityEngine.UI.Image.Origin360;
using FillTargetComponent = Jigbox.Components.GaugeModel.FillTargetComponent;

namespace Jigbox.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Gauge), true)]
    public class GaugeEditor : Editor
    {
#region constants

        /// <summary>開閉状態保存用キーの先頭語</summary>
        protected static readonly string KeyHeader = typeof(Gauge).ToString();

        /// <summary>Valueのコントロール名</summary>
        protected static readonly string ValueControlName = "Gauge_Value_Control";

#endregion

#region properties

        /// <summary>Gauge</summary>
        protected Gauge gauge;

        /// <summary>フィリング対象のRectTransformの状態を計算するための矩形領域のプロパティ</summary>
        protected SerializedProperty fillRectProperty;

        /// <summary>フィリング方法のプロパティ</summary>
        protected SerializedProperty fillMethodProperty;

        /// <summary>フィリングを行う際に対象となるコンポーネントのプロパティ</summary>
        protected SerializedProperty fillTargetProperty;

        /// <summary>フィリングを行う際に基準となる点</summary>
        protected SerializedProperty fillOriginProperty;

        /// <summary>フィリング対象となるコンポーネントのプロパティ</summary>
        protected SerializedProperty targetProperty;

        /// <summary>値更新バリデータのプロパティを参照</summary>
        protected SerializedProperty valueLimtTargetProperty;

        /// <summary>値が変更された際のコールバックのプロパティ</summary>
        protected SerializedProperty onValueChangedProperty;

        /// <summary>現在の階調が変更された際のコールバックのプロパティ</summary>
        protected SerializedProperty onStepChangedProperty;

        /// <summary>View</summary>
        protected GaugeViewBase view = null;

        /// <summary>合成したGUI変更フラグ。</summary>
        protected bool compositedGUIChanged;

        /// <summary>Undoの説明文言</summary>
        protected virtual string UndoDescription { get { return "Edit Gauge"; } }
        
#endregion

#region protected methods

        /// <summary>
        /// 値の編集用の表示を行います。
        /// </summary>
        protected virtual void DrawValue()
        {
            float value = gauge.Value;

            bool isMultiple = false;
            foreach (UnityEngine.Object target in targets)
            {
                isMultiple |= (target as Gauge).Value != value;
            }

            EditorGUI.BeginDisabledGroup(isMultiple);
            GUI.SetNextControlName(ValueControlName);
            value = EditorGUILayout.Slider("Value", value, 0.0f, 1.0f);
            EditorGUI.EndDisabledGroup();
            if (gauge.Value != value)
            {
                UpdateProperty(g => g.Value = value);
            }

            // 複数選択状態で、それぞれの値が異なる場合は
            // どの値を表示しても正しい状態を表さなくなるので
            // 表示方法を変更する
            if (isMultiple)
            {
                // 元々のSliderの範囲を取得して、そのうち、
                // 値の入力フィールド部分のみの範囲を抽出する
                Rect rect = GUILayoutUtility.GetLastRect();
                // Sliderの入力フィールドの幅は50
                rect.x += rect.width - 50.0f;
                rect.width = 50.0f;
                // 本来の値の入力領域上に別な入力フィールドを表示して上書きする
                EditorGUI.DrawRect(rect, Color.gray);
                string valueString = EditorGUI.TextField(rect, "-");
                if (float.TryParse(valueString, out value))
                {
                    if (gauge.Value != value)
                    {
                        UpdateProperty(g => g.Value = value);
                        // 入力しているフィールドが本来のフィールドとは異なるので、
                        // 編集された際には、本来のフィールドへフォーカスを戻す
                        GUI.FocusControl(ValueControlName);
                    }
                }
            }
        }

        /// <summary>
        /// フィリング対象のコンポーネントの編集用の表示を行います。
        /// </summary>
        protected virtual void DrawTarget()
        {
            serializedObject.Update();

            // フィリング対象はオブジェクトごとに固有のものなので、
            // 複数同時選択時は編集不可
            EditorGUI.BeginDisabledGroup(targets.Length > 1);
            {
                // フィリング対象によってシリアライズするコンポーネントが異なるので処理を分ける
                if (fillTargetProperty.enumValueIndex == (int) FillTargetComponent.RectTransform)
                {
                    RectTransform target = targetProperty.objectReferenceValue as RectTransform;
                    RectTransform selected = EditorGUILayout.ObjectField(
                        "Target",
                        target,
                        typeof(RectTransform),
                        true) as RectTransform;
                    if (target != selected)
                    {
                        targetProperty.objectReferenceValue = selected;
                        UpdateFillTarget();
                    }
                    compositedGUIChanged |= GUI.changed;
                    GUI.changed = false;
                    EditorGUILayout.PropertyField(fillRectProperty);
                    if (GUI.changed)
                    {
                        UpdateFillTarget();
                    }
                }
                else
                {
                    Image target = targetProperty.objectReferenceValue as Image;
                    Image selected = EditorGUILayout.ObjectField("Target", target, typeof(Image), true) as Image;
                    if (target != selected)
                    {
                        // フィリング対象が更新された場合、元々対象としてシリアライズされていた
                        // コンポーネントの状態を元に戻す必要がある
                        UpdateView(1.0f);
                        targetProperty.objectReferenceValue = selected;
                        UpdateFillTarget();
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawValidator()
        {
            serializedObject.Update();

            var validationTarget = EditorGUILayout.ObjectField(
                "Limiter",
                valueLimtTargetProperty.objectReferenceValue as MonoBehaviour,
                typeof(MonoBehaviour),
                true
            ) as MonoBehaviour;
            if (validationTarget != valueLimtTargetProperty.objectReferenceValue)
            {
                if (validationTarget == null)
                {
                    valueLimtTargetProperty.objectReferenceValue = null;
                }
                else
                {
                    IGaugeLimiter validator = validationTarget.GetComponent<IGaugeLimiter>();
                    valueLimtTargetProperty.objectReferenceValue = validator as MonoBehaviour;
                    if (validator == null)
                    {
                        Debug.LogWarning(string.Format("{0}'s value change validation target need to has IGaugeValueChangeValidator!", KeyHeader));
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawCopyDelegateButton()
        {
            serializedObject.Update();
            if (serializedObject.targetObjects.Length > 1)
            {
                bool isCopy = false;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.HelpBox("イベントの設定は、現在選択しているオブジェクトにのみ適用されます。", MessageType.Info);
                    isCopy = GUILayout.Button("選択中の\nイベントをコピー", GUILayout.Width(100.0f));
                }
                EditorGUILayout.EndHorizontal();

                // DelegatableList の中身のコピーを行う
                if (isCopy)
                {
                    DelegatableObjectEditor.CopyToDelegatableListProperty(gauge.OnValueChanged, onValueChangedProperty);
                    DelegatableObjectEditor.CopyToDelegatableListProperty(gauge.OnStepChanged, onStepChangedProperty);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// モデルの編集用の表示を行います。
        /// </summary>
        protected virtual void DrawModel()
        {
            // Steps
            DrawSteps();

            // ↑の情報は、プロパティを直接編集しているため、
            // SerializedObject.Updateの外でやらなければ行けない
            serializedObject.Update();

            // 実行時には、Viewに関わるModel情報は編集不可
            // 複数同時編集時もViewを再生成できないため、編集不可
            EditorGUI.BeginDisabledGroup(Application.isPlaying || targets.Length > 1);
            {
                // FillMethod
                DrawFillMethod();

                // FillTarget
                EditorGUI.BeginDisabledGroup(fillMethodProperty.intValue != (int) FillMethod.Horizontal 
                    && fillMethodProperty.intValue != (int) FillMethod.Vertical);
                {
                    compositedGUIChanged |= GUI.changed;
                    GUI.changed = false;
                    int lastFillTarget = fillTargetProperty.intValue;
                    EditorGUILayout.PropertyField(fillTargetProperty);
                    if (GUI.changed)
                    {
                        if (lastFillTarget == (int) FillTargetComponent.Image)
                        {
                            UpdateView(1.0f);
                        }
                        UpdateFillTarget();
                    }
                }
                EditorGUI.EndDisabledGroup();

                // FillOrigin
                DrawFillOrigin();
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 階調の編集用の表示を行います。
        /// </summary>
        protected virtual void DrawSteps()
        {
            EditorGUILayout.BeginHorizontal();
            {
                // インスペクタの領域が狭くなってもStepsのIntFieldが見切れないようにLabelサイズを設定する
                EditorGUILayout.LabelField("Step / Steps", GUILayout.MinWidth(100.0f));

                EditorGUILayout.BeginHorizontal();
                {
                    float cachedLabelWidth = EditorGUIUtility.labelWidth;
                    EditorUtilsTools.SetLabelWidth(10.0f);
                    
                    // Steps=0の状態で階調数は指定できない
                    EditorGUI.BeginDisabledGroup(gauge.Steps == 0);
                    {
                        int step = gauge.CurrentStep;
                        // ラベルがないor0文字だとドラッグで値いじれないため
                        // 空白を放り込んでおく
                        if (IntField(" ", ref step, g => g.CurrentStep))
                        {
                            if (step > gauge.Steps)
                            {
                                step = gauge.Steps;
                            }
                            else if (step < 0)
                            {
                                step = 0;
                            }
                            UpdateProperty(g => g.CurrentStep = step);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                    
                    EditorGUILayout.PrefixLabel("/");

                    int steps = gauge.Steps;
                    if (IntField(" ", ref steps, g => g.Steps))
                    {
                        if (steps < 0)
                        {
                            steps = 0;
                        }
                        if (steps != gauge.Steps)
                        {
                            UpdateProperty(g => g.Steps = steps);
                        }
                    }

                    EditorUtilsTools.SetLabelWidth(cachedLabelWidth);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// フィリング方法の編集用の表示を行います。
        /// </summary>
        protected virtual void DrawFillMethod()
        {
            FillMethod fillMethod = GetEnumValue<FillMethod>(fillMethodProperty.intValue);
            fillMethod = (FillMethod) EditorGUILayout.EnumPopup("Fill Method", fillMethod);
            if (fillMethodProperty.intValue != (int) fillMethod)
            {
                fillMethodProperty.intValue = (int) fillMethod;
                // RadialGaugeViewを使用する際には、FillTargetComponentはImageのみ
                switch (fillMethod)
                {
                    case FillMethod.Radial90:
                    case FillMethod.Radial180:
                    case FillMethod.Radial360:
                        if (fillTargetProperty.enumValueIndex == (int) FillTargetComponent.RectTransform)
                        {
                            fillTargetProperty.enumValueIndex = (int) FillTargetComponent.Image;
                        }
                        break;
                }

                fillOriginProperty.intValue = 0;

                UpdateFillTarget();
            }
        }

        /// <summary>
        /// フィリング時の起点の編集用の表示を行います。
        /// </summary>
        protected virtual void DrawFillOrigin()
        {
            int fillOriginSelected = fillOriginProperty.intValue;

            // FillOrigineはFillMethod毎に内容が異なる
            switch (fillMethodProperty.intValue)
            {
                case (int) FillMethod.Horizontal:
                    {
                        OriginHorizontal origin = GetEnumValue<OriginHorizontal>(fillOriginProperty.intValue);
                        origin = (OriginHorizontal) EditorGUILayout.EnumPopup("Fill Origin", origin);
                        fillOriginSelected = (int) origin;
                    }
                    break;
                case (int) FillMethod.Vertical:
                    {
                        OriginVertical origin = GetEnumValue<OriginVertical>(fillOriginProperty.intValue);
                        origin = (OriginVertical) EditorGUILayout.EnumPopup("Fill Origin", origin);
                        fillOriginSelected = (int) origin;
                    }
                    break;
                case (int) FillMethod.Radial90:
                    {
                        Origin90 origin = GetEnumValue<Origin90>(fillOriginProperty.intValue);
                        origin = (Origin90) EditorGUILayout.EnumPopup("Fill Origin", origin);
                        fillOriginSelected = (int) origin;
                    }
                    break;
                case (int) FillMethod.Radial180:
                    {
                        Origin180 origin = GetEnumValue<Origin180>(fillOriginProperty.intValue);
                        origin = (Origin180) EditorGUILayout.EnumPopup("Fill Origin", origin);
                        fillOriginSelected = (int) origin;
                    }
                    break;
                case (int) FillMethod.Radial360:
                    {
                        Origin360 origin = GetEnumValue<Origin360>(fillOriginProperty.intValue);
                        origin = (Origin360) EditorGUILayout.EnumPopup("Fill Origin", origin);
                        fillOriginSelected = (int) origin;
                    }
                    break;
            }

            if (fillOriginProperty.intValue != fillOriginSelected)
            {
                fillOriginProperty.intValue = fillOriginSelected;
                InitView();
            }
        }
        
        /// <summary>
        /// フィリング対象のコンポーネントにフィリングに必要な設定を行います。
        /// </summary>
        protected virtual void UpdateFillTarget()
        {
            // 実行時はコンポーネント自体がViewの更新を行うので、
            // エディタスクリプト側ではViewの操作を行わない
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
            {
                return;
            }

            ValidateTargetType();

            if (targetProperty.objectReferenceValue == null)
            {
                if (fillTargetProperty.enumValueIndex == (int) FillTargetComponent.Image)
                {
                    // フィリング対象が変更になって参照が飛んだ場合、Viewを元に戻してから参照を外す
                    UpdateView(1.0f);
                }
                view = null;
                return;
            }
            if (fillTargetProperty.enumValueIndex == (int) FillTargetComponent.RectTransform)
            {
                if (fillRectProperty.objectReferenceValue == null)
                {
                    view = null;
                    return;
                }
            }

            GaugeViewBase newView = null;

            // フィリング方法、フィリング対象のコンポーネントに合わせてViewを変更
            switch (fillMethodProperty.intValue)
            {
                case (int) FillMethod.Horizontal:
                    if (fillTargetProperty.enumValueIndex == (int) FillTargetComponent.RectTransform)
                    {
                        newView = new HorizontalGaugeView();
                    }
                    else
                    {
                        newView = new FilledHorizontalGaugeView();
                    }
                    break;
                case (int) FillMethod.Vertical:
                    if (fillTargetProperty.enumValueIndex == (int) FillTargetComponent.RectTransform)
                    {
                        newView = new VerticalGaugeView();
                    }
                    else
                    {
                        newView = new FilledVerticalGaugeView();
                    }
                    break;
                case (int) FillMethod.Radial90:
                case (int) FillMethod.Radial180:
                case (int) FillMethod.Radial360:
                    newView = new RadialGaugeView();
                    break;
            }

            if (view == null)
            {
                view = newView;
            }
            else if (view.GetType() != newView.GetType())
            {
                // Viewが切り替わる場合、現在のViewの状態を元に戻してから新しいViewを設定
                if (fillTargetProperty.enumValueIndex == (int) FillTargetComponent.Image)
                {
                    UpdateView(1.0f);
                }
                view = newView;
            }

            InitView();
        }

        /// <summary>
        /// フィリング対象のコンポーネントの型が正しいか判定して編集します。
        /// </summary>
        protected virtual void ValidateTargetType()
        {
            if (fillTargetProperty.enumValueIndex == (int) FillTargetComponent.RectTransform)
            {
                UnityEngine.Object target = targetProperty.objectReferenceValue;
                if (target as RectTransform == null)
                {
                    if (target is Component)
                    {
                        targetProperty.objectReferenceValue = (target as Component).GetComponent<RectTransform>();
                    }
                    else
                    {
                        targetProperty.objectReferenceValue = null;
                    }
                }
            }
            else
            {
                UnityEngine.Object target = targetProperty.objectReferenceValue;
                if (target as Image == null)
                {
                    if (target is Component)
                    {
                        targetProperty.objectReferenceValue = (target as Component).GetComponent<Image>();
                    }
                    else
                    {
                        targetProperty.objectReferenceValue = null;
                    }
                }
            }
        }

        /// <summary>
        /// Viewの状態を初期化します。
        /// </summary>
        protected virtual void InitView()
        {
            if (view != null)
            {
                if (view is ColumnGaugeView)
                {
                    ColumnGaugeView columnView = view as ColumnGaugeView;
                    columnView.InitView(fillRectProperty.objectReferenceValue as RectTransform,
                        targetProperty.objectReferenceValue,
                        fillMethodProperty.intValue,
                        fillOriginProperty.intValue);
                }
                else
                {
                    view.InitView(targetProperty.objectReferenceValue,
                        fillMethodProperty.intValue,
                        fillOriginProperty.intValue);
                }

                UpdateView(gauge.Value);
            }
        }

        /// <summary>
        /// Viewの状態を更新します。
        /// </summary>
        /// <param name="value">ゲージの値</param>
        protected virtual void UpdateView(float value)
        {
            if (view != null)
            {
                view.UpdateView(value);
            }
        }

        /// <summary>
        /// プロパティを更新します。
        /// </summary>
        /// <param name="onUpdate">更新を行うコールバック</param>
        protected void UpdateProperty(Action<Gauge> onUpdate)
        {
            if (targets.Length == 1)
            {
                onUpdate(gauge);
            }
            else
            {
                // 複数選択状態でプロパティアクセスによって値を更新する場合は、
                // 全てのインスタンスに対して更新を行わなければならない
                foreach (UnityEngine.Object target in targets)
                {
                    onUpdate(target as Gauge);
                }
            }
            UpdateView(gauge.Value);
        }

        /// <summary>
        /// int値の編集用フィールドを表示します。
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <param name="value">値</param>
        /// <returns>編集が行われた場合、<c>true</c>を返します。</returns>
        protected bool IntField(string label, ref int value, Func<Gauge, int> getValue)
        {
            compositedGUIChanged |= GUI.changed;
            GUI.changed = false;
            bool isMultiple = false;
            foreach (UnityEngine.Object target in targets)
            {
                isMultiple |= getValue(target as Gauge) != value;
            }

            if (!isMultiple)
            {
                value = EditorGUILayout.IntField(label, value);
            }
            else
            {
                // 複数選択状態における例外表示
                string valueString = EditorGUILayout.TextField(label, "-");
                if (!int.TryParse(valueString, out value))
                {
                    return false;
                }
            }
            return GUI.changed;
        }

        /// <summary>
        /// Enum上での値を取得します。
        /// </summary>
        /// <typeparam name="T">対象Enumの型</typeparam>
        /// <param name="index">Enumのインデックス</param>
        /// <returns></returns>
        protected T GetEnumValue<T>(int index)
        {
            T[] values = (T[]) Enum.GetValues(typeof(T));
            return values[index];
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            if (serializedObject.targetObjects.Length == 1)
            {
                gauge = target as Gauge;
            }
            else
            {
                if (Selection.activeGameObject != null)
                {
                    gauge = Selection.activeGameObject.GetComponent<Gauge>();
                }

                // Gauge以外のコンポーネントを選択してから複数のGaugeを選択した場合にnullになる
                // 異なるコンポーネントが選択された場合はそもそもInspector上で表示されないのでOnEnableの処理もしなくて良い
                if (gauge == null)
                {
                    return;
                }
            }

            targetProperty = serializedObject.FindProperty("target");
            fillRectProperty = serializedObject.FindProperty("fillRect");

            SerializedProperty modelProperty = serializedObject.FindProperty("model");
            fillMethodProperty = modelProperty.FindPropertyRelative("fillMethod");
            fillTargetProperty = modelProperty.FindPropertyRelative("fillTarget");
            fillOriginProperty = modelProperty.FindPropertyRelative("fillOrigin");

            valueLimtTargetProperty = serializedObject.FindProperty("valueLimitTarget");
            onValueChangedProperty = serializedObject.FindProperty("onValueChanged");
            onStepChangedProperty = serializedObject.FindProperty("onStepChanged");

            UpdateFillTarget();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            compositedGUIChanged = false;

            DrawValue();

            DrawTarget();

            DrawValidator();

            DrawCopyDelegateButton();

            EditorGUILayout.Space();

            // Model
            if (EditorUtilsTools.DrawGroupHeader("Model", KeyHeader + "_Model"))
            {
                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawModel();
                }
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space();

            DelegatableObjectEditor.DrawEditFields(
                "On Value Changed",
                target,
                gauge.OnValueChanged,
                typeof(AuthorizedAccessAttribute),
                KeyHeader + "_OnValueChanged");

            EditorGUI.BeginDisabledGroup(gauge.Steps == 0);
            {
                DelegatableObjectEditor.DrawEditFields(
                    "On Step Changed",
                    target,
                    gauge.OnStepChanged,
                    typeof(AuthorizedAccessAttribute),
                    KeyHeader + "_OnStepChanged");
            }
            EditorGUI.EndDisabledGroup();
            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo(UndoDescription, compositedGUIChanged, targets);
            EditorGUI.EndChangeCheck();
        }

#endregion
    }
}
