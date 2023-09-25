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
using Jigbox.EditorUtils;
using Jigbox.UIControl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Jigbox.TextView;
using Jigbox.TextView.HorizontalLayout;
using Jigbox.Delegatable;

namespace Jigbox.Components
{
    [CustomEditor(typeof(TextView), true)]
    public class TextViewEditor : Editor
    {
#region constants

        /// <summary>シーンビュー上で仮想ボディを表示するかどうかを保存しているPrefsKey</summary>
        protected static readonly string IsShowGizmosBodyEditorKey = "isShowGizmosBody";

        /// <summary>PreferredWidthTypeの保存用キー</summary>
        protected static readonly string PreferredWidthTypeKey = "preferredWidthTypeKey";

        /// <summary>PreferredHeightTypeの保存用キー</summary>
        protected static readonly string PreferredHeightTypeKey = "preferredHeightTypeKey";

        /// <summary>アライメントのアイコン画像のディレクトリパス</summary>
        protected static readonly string AlignmentIconDirectory = "Assets/Jigbox/TextView/Editor/Icons/";

        /// <summary>開閉状態保存用キーの先頭語</summary>
        protected static readonly string KeyHeader = typeof(TextView).ToString();

        /// <summary>Inspector上で編集可能な文字列の長さ</summary>
        protected static readonly int EditableTextLength = 5000;

#endregion

#region properties

        /// <summary>横方向のアライメントのアイコン画像</summary>
        protected static Dictionary<TextAlign, Texture> alignIcons;

        /// <summary>縦方向のアライメントのアイコン画像</summary>
        protected static Dictionary<TextVerticalAlign, Texture> verticalAlignIcons;

        /// <summary>TextView</summary>
        protected TextView textView;

        /// <summary>入力判定の対象として有効かどうか</summary>
        protected SerializedProperty raycastTargetProperty;

        /// <summary>フォントサイズ</summary>
        protected SerializedProperty fontSizeProperty;

        /// <summary>シュリンクコンポーネント</summary>
        protected SerializedProperty shrinkComponentProperty;

        /// <summary>高さを自動調整するかどうか</summary>
        protected SerializedProperty heightAdjustmentProperty;

        /// <summary>高さを自動調整する際にルビを含めるかどうか</summary>
        protected SerializedProperty heightAdjustmentWithRubyProperty;

        /// <summary>HorizontalLayoutの参照</summary>
        protected FieldInfo horizontalLayoutField;

        /// <summary>TextViewが実際に描画するテキストのOffsetY(pivotによるY軸)の位置</summary>
        protected FieldInfo offsetYField;

        /// <summary>VisbleStartLineによるTextLineの描画開始補正位置</summary>
        protected FieldInfo visibleStartLineOffsetYField;

        /// <summary></summary>
        protected FieldInfo textSourceField;

        /// <summary>enableMirrorの参照</summary>
        protected FieldInfo enableMirrorField;

        /// <summary>TextSourceの参照</summary>
        protected bool bodyFocus = false;

        /// <summary>直前のフォント</summary>
        protected Font lastFont;

        /// <summary>直前のフォントサイズ</summary>
        protected int lastFontSize;

        /// <summary>直前の高さ</summary>
        protected int adjustmentHeight;

        /// <summary>テキスト編集フィールドの横幅</summary>
        protected float textAreaWidth;

        /// <summary>ボールド</summary>
        protected SerializedProperty isBoldProperty;

        /// <summary>イタリック</summary>
        protected SerializedProperty isItalicProperty;

        /// <summary>言語処理のプロパティ</summary>
        protected SerializedProperty languageTypeProperty;

        /// <summary>RaycastPadding</summary>
        protected RaycastPaddingInspector raycastPadding;

        /// <summary>合成したGUI変更フラグ。</summary>
        protected bool compositedGUIChanged;

        /// <summary>TextViewのテキスト編集フィールドがフォーカスされているかどうか</summary>
        protected bool IsBodyFocus
        {
            get { return bodyFocus; }
            set
            {
                if (bodyFocus != value)
                {
                    if (!value)
                    {
                        var source = (Jigbox.TextView.TextSource) textSourceField.GetValue(textView);

                        if (source != null && source.HasError)
                        {
                            Debug.LogError(source.ErrorMessage);
                        }
                    }
                    bodyFocus = value;
                }
            }
        }
        
        /// <summary>TextViewの設定がシュリンクと矛盾しているかどうか</summary>
        protected bool isInconsistentShrinkSetting = false;

        /// <summary>TextViewの設定がSqueezeと矛盾しているかどうか</summary>
        protected bool isInconsistentEllipsisSetting = false;

        /// <summary>TextViewの設定がEllipsisと矛盾しているかどうか</summary>
        protected bool isInconsistentSqueezeSetting = false;
        
        /// <summary>TextViewに対して即時更新処理を行うためのメソッド情報</summary>
        protected static MethodInfo textViewUpdateMethod = null;
        
        /// <summary>レターケース</summary>
        protected SerializedProperty letterCaseProperty;

#endregion

#region protected methods

        /// <summary>
        /// アイコンを読み込みます
        /// </summary>
        protected static void LoadIcons()
        {
            if (alignIcons == null)
            {
                alignIcons = new Dictionary<TextAlign, Texture> {
                    { TextAlign.Left, LoadIcon("TextAlign_Left_16.png") },
                    { TextAlign.Center, LoadIcon("TextAlign_Center_16.png") },
                    { TextAlign.Right, LoadIcon("TextAlign_Right_16.png") },
                    { TextAlign.Justify, LoadIcon("TextAlign_Justify_16.png") },
                    { TextAlign.JustifyAll, LoadIcon("TextAlign_JustifyAll_16.png") }
                };
            }
            if (verticalAlignIcons == null)
            {
                verticalAlignIcons = new Dictionary<TextVerticalAlign, Texture> {
                    { TextVerticalAlign.Top, LoadIcon("TextVerticalAlign_Top_16.png") },
                    { TextVerticalAlign.Center, LoadIcon("TextVerticalAlign_Center_16.png") },
                    { TextVerticalAlign.Bottom, LoadIcon("TextVerticalAlign_Bottom_16.png") }
                };
            }
        }

        /// <summary>
        /// シリアライズされている変数を取得します。
        /// </summary>
        protected virtual void GetSerializedProperties()
        {
            raycastTargetProperty = serializedObject.FindProperty("m_RaycastTarget");
            fontSizeProperty = serializedObject.FindProperty("fontSize");
            shrinkComponentProperty = serializedObject.FindProperty("overflowExtensionComponent");

            heightAdjustmentProperty = serializedObject.FindProperty("heightAdjustment");
            heightAdjustmentWithRubyProperty = serializedObject.FindProperty("heightAdjustmentWithRuby");
            isBoldProperty = serializedObject.FindProperty("isBold");
            isItalicProperty = serializedObject.FindProperty("isItalic");
            languageTypeProperty = serializedObject.FindProperty("languageType");
            this.letterCaseProperty = serializedObject.FindProperty("letterCase");

            SerializedProperty longTextExtensionProperty = serializedObject.FindProperty("longTextExtension");
            TextViewLongTextExtension longTextExtension = textView.GetComponent<TextViewLongTextExtension>();
            if (longTextExtensionProperty.objectReferenceValue != longTextExtension)
            {
                serializedObject.Update();
                longTextExtensionProperty.objectReferenceValue = longTextExtension;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();

                // TextViewLongTextExtensionをアタッチした時点で全て再計算
                if (longTextExtension != null)
                {
                    textView.RequireProcess(WorkflowProcess.TextParse);
                }
            }
        }

        /// <summary>
        /// シリアライズされていない変数を取得します。
        /// </summary>
        protected virtual void GetNoneSerializedFields()
        {
            BindingFlags bindingFlag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField;

            Type type = typeof(TextView);

            horizontalLayoutField = type.GetField("horizontalLayout", bindingFlag);
            offsetYField = type.GetField("offsetY", bindingFlag);
            visibleStartLineOffsetYField = type.GetField("visibleStartLineOffsetY", bindingFlag);
            textSourceField = type.GetField("textSource", bindingFlag);
            enableMirrorField = type.GetField("enableMirror", bindingFlag);
        }

        /// <summary>
        /// Styleを編集します。
        /// </summary>
        protected virtual void EditStyle()
        {
            GUILayout.Space(2.5f);
            if (!EditorUtilsTools.DrawGroupHeader("Style", KeyHeader + ".Style"))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                this.serializedObject.Update();
                this.textView.Font = EditorGUILayout.ObjectField("Font", this.textView.Font, typeof(Font), true) as Font;
                EditorGUI.BeginChangeCheck();
                // プロパティからgetした値はシュリンク込みの値になってしまうので、直接シリアライズしている値を参照する
                this.textView.FontSize = EditorGUILayout.IntField("Font Size", fontSizeProperty.intValue);
                EditorGUI.EndChangeCheck();

                // シュリンクコンポーネントがアタッチされている場合はシュリンク後のサイズとSqueeze後のサイズも表示
                if (shrinkComponentProperty.objectReferenceValue != null)
                {
                    TextViewOverflowExtension overflowExtensionComponent = shrinkComponentProperty.objectReferenceValue as TextViewOverflowExtension;
                    
                    EditorGUI.BeginDisabledGroup(true);
                    if (overflowExtensionComponent.ShrinkEnabled)
                    {
                        if (overflowExtensionComponent.IsNeedShrink())
                        {
                            EditorGUILayout.LabelField("Shrinked Size", "-");
                        }
                        else
                        {
                            EditorGUILayout.IntField("Shrinked Size", overflowExtensionComponent.ShrinkedSize);    
                        }
                    }
                    
                    if (overflowExtensionComponent.SqueezeEnabled)
                    {
                        if (overflowExtensionComponent.IsNeedSqueeze())
                        {
                            EditorGUILayout.LabelField("Squeezed Size", "-");
                        }
                        else
                        {
                            EditorGUILayout.FloatField("Squeezed Size", overflowExtensionComponent.SqueezedSize);    
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }

                this.textView.color = EditorGUILayout.ColorField("Color", this.textView.color);
                this.textView.IsBold = EditorGUILayout.Toggle("Bold", this.isBoldProperty.boolValue);
                this.textView.IsItalic = EditorGUILayout.Toggle("Italic", this.isItalicProperty.boolValue);
                this.textView.LetterCase = (LetterCase) EditorGUILayout.EnumPopup("LetterCase", (LetterCase) this.letterCaseProperty.enumValueIndex);
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual void EditTextViewOverflowExtension()
        {
            GUILayout.Space(2.5f);
            if (!EditorUtilsTools.DrawGroupHeader("TextViewOverflowExtension", KeyHeader + ".TextViewOverflowExtension"))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            // シュリンクコンポーネントがアタッチされている場合はTextViewOverflowExtensionのプロパティも表示
            if (shrinkComponentProperty.objectReferenceValue != null)
            {
                TextViewOverflowExtension overflowExtensionComponent = shrinkComponentProperty.objectReferenceValue as TextViewOverflowExtension;
                SerializedObject overflowExtensionSerializedObject = new SerializedObject(overflowExtensionComponent);
                    
                var ellipsisEnabledProperty = overflowExtensionSerializedObject.FindProperty("ellipsisEnabled");
                var squeezeEnabledProperty = overflowExtensionSerializedObject.FindProperty("squeezeEnabled");
                var squeezeMaxSizeProperty = overflowExtensionSerializedObject.FindProperty("squeezeMaxSize");
                var squeezeStepSizeProperty = overflowExtensionSerializedObject.FindProperty("squeezeStepSize");
                var shrinkEnabledProperty = overflowExtensionSerializedObject.FindProperty("shrinkEnabled");
                var shrinkMinFontSizeProperty = overflowExtensionSerializedObject.FindProperty("shrinkMinFontSize");
                var criterionSizeProperty = overflowExtensionSerializedObject.FindProperty("criterionSize");

                if (isInconsistentEllipsisSetting && ellipsisEnabledProperty.boolValue)
                {
                    EditorGUILayout.HelpBox(
                        "TextViewのVertical OverflowがOverflowの状態ではEllipsisは行われません。",
                        MessageType.Warning);
                }

                if (isInconsistentSqueezeSetting && squeezeEnabledProperty.boolValue)
                {
                    EditorGUILayout.HelpBox(
                        "TextViewの文字をOverflowする状態でSqueezeを行う場合、WarpかつTruncateを設定している時と結果が異なることがあります。",
                        MessageType.Warning);
                }

                if (isInconsistentShrinkSetting && shrinkEnabledProperty.boolValue)
                {
                    EditorGUILayout.HelpBox(
                        "TextViewの文字をOverflowする状態でShrinkを行う場合、WarpかつTruncateを設定している時と結果が異なることがあります。",
                        MessageType.Warning);
                }
                
                EditorGUI.BeginChangeCheck();
                {
                    overflowExtensionSerializedObject.Update();
                    
                    EditorGUILayout.PropertyField(ellipsisEnabledProperty);
                    EditorGUILayout.PropertyField(squeezeEnabledProperty);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.IntSlider(squeezeMaxSizeProperty, 1, 50);
                    if (EditorGUI.EndChangeCheck())
                    {
                        // SqueezeStepSizeはSqueezeStepMaxの値より大きくならないようにする
                        squeezeStepSizeProperty.intValue = Mathf.Min(squeezeStepSizeProperty.intValue, squeezeMaxSizeProperty.intValue);
                    }
                    EditorGUILayout.IntSlider(squeezeStepSizeProperty, 1, squeezeMaxSizeProperty.intValue);
                    EditorGUILayout.PropertyField(shrinkEnabledProperty);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(shrinkMinFontSizeProperty);
                    if (EditorGUI.EndChangeCheck())
                    {
                        fontSizeProperty.serializedObject.Update();
                        shrinkMinFontSizeProperty.intValue = Mathf.Max(shrinkMinFontSizeProperty.intValue, 1);
                    }
            
                    // フォントサイズ以上にシュリンクの最小サイズが設定されている場合警告文表示
                    if (shrinkEnabledProperty.boolValue && shrinkMinFontSizeProperty.intValue >= fontSizeProperty.intValue)
                    {
                        EditorGUILayout.HelpBox("FontSizeがShrinkMinFontSize以下に設定されています。\nこの状態ではShrinkされません。", MessageType.Warning);
                    }
                    EditorGUILayout.PropertyField(criterionSizeProperty);
                }
                var overflow = EditorGUI.EndChangeCheck();

                // 変更があったらシュリンクをやり直す
                if (overflow)
                {
                    overflowExtensionComponent.RequireCalculate();
                    this.textView.RequireProcess(WorkflowProcess.LogicalLinesCreate);
                }

                overflowExtensionSerializedObject.ApplyModifiedProperties();
                EditorUtilsTools.RegisterUndo("Edit TextView Overflow Extension", overflow, overflowExtensionComponent);
                compositedGUIChanged |= overflow;
            }
            else // TextViewOverflowExtensionがアタッチされていない場合はアタッチ用のボタンを表示
            {
                if (GUILayout.Button("Attach TextViewOverflowExtension"))
                {
                    if (serializedObject.targetObjects.Length == 1)
                    {
                        shrinkComponentProperty.objectReferenceValue = textView.gameObject.AddComponent<TextViewOverflowExtension>();
                    }
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 本文を編集します。
        /// </summary>
        protected virtual void EditBody()
        {
            GUILayout.Space(2.5f);
            if (!EditorUtilsTools.DrawGroupHeader("Body", KeyHeader + ".Body"))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                bool isLongText = this.textView.Text.Length >= EditableTextLength;
                string showText = isLongText ? this.textView.Text.Substring(0, EditableTextLength) : this.textView.Text;

                // 標準のTextAreaのレイアウトを使用するとテキストの入力領域の高さは行数に合わせて可変になるが、それだと空行もしくは数文字程度の文字列だと1行しか表示されず入力しづらく感じる
                // 行数が少ない場合は入力しやすいようにある程度の高さの余白(3行分)を確保し、行数が多い場合はその分の高さになるような挙動にさせる
                GUIStyle style = new GUIStyle(EditorStyles.textArea);
                style.wordWrap = true;
                // CalcHeightを使用すると改行とテキストの折り返しを考慮した高さを返してくれる
                float height = EditorStyles.textArea.CalcHeight(new GUIContent(showText), textAreaWidth);
                height = Mathf.Max(style.lineHeight * 3, height);

                if (isLongText)
                {
                    EditorGUILayout.HelpBox(EditableTextLength + "文字以上のテキストは、Inspector上では編集できません。", MessageType.Warning);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("クリップボードにコピー"))
                        {
                            GUIUtility.systemCopyBuffer = this.textView.Text;
                        }
                        if (GUILayout.Button("クリア"))
                        {
                            this.textView.Text = string.Empty;
                            // フォーカスが残っていると入力途中だった文字列が残ってしまうので
                            // 一度強制的にフォーカスを外す
                            GUI.FocusControl("");
                            Repaint();
                            return;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    // テキストは直接編集出来ない状態で表示
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextArea(showText, style, GUILayout.Height(height));
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    if (Application.isPlaying)
                    {
                        this.textView.Text = EditorGUILayout.TextArea(this.textView.Text, style, GUILayout.Height(height));
                    }
                    else
                    {
                        // IsBodyFocusを実装している意図はTextパースのエラーを通知したいためだが、テキスト入力の度にエラーを吐かれると邪魔になってしまうので
                        // Focusされた時とFocusが外れた時をタイミングとしてチェックを行うようにしている
                        // 実行中には邪魔なだけだと思われるので非実行時のみこの挙動を行う
                        GUI.SetNextControlName("TextViewTextField");
                        this.textView.Text = EditorGUILayout.TextArea(this.textView.Text, style, GUILayout.Height(height));
                        IsBodyFocus = ("TextViewTextField" == GUI.GetNameOfFocusedControl());
                    }
                }

                Rect rect = GUILayoutUtility.GetLastRect();
                // いくつかの条件で、幅、高さが1のRect(デフォルト値っぽい)が返ってくるので
                // 正しい値が返ってきている場合のみ処理
                if (rect.width != 1.0f)
                {
                    // CalcHeightには引数としてwidth(TextAreaの横幅)が必要だが、レイアウト作成時に取得しようとしてもその情報は存在しないので
                    // 今回の描画結果の横幅を保持しておき、次回描画時の計算に使用する(結果1描画分だけ表示が遅れるが特別問題ないと認識している)
                    textAreaWidth = rect.width;
                }

                GUILayout.Space(2.5f);
                this.textView.TreatNewLineAsLineBreak = EditorGUILayout.Toggle("New Line is Line Break", this.textView.TreatNewLineAsLineBreak);
                this.textView.LineBreakRule = (TextLineBreakRule) EditorGUILayout.EnumPopup("Line Break Rule", this.textView.LineBreakRule);
                this.textView.UseBurasage = EditorGUILayout.Toggle("Burasage(ぶら下げ)", this.textView.UseBurasage);
                this.textView.IsHalfPunctuationOfLineHead = EditorGUILayout.Toggle("行頭約物半角",this.textView.IsHalfPunctuationOfLineHead);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// パラグラフを編集します。
        /// </summary>
        protected virtual void EditParagraph()
        {
            GUILayout.Space(2.5f);
            if (!EditorUtilsTools.DrawGroupHeader("Paragraph", KeyHeader + ".Paragraph"))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditAlignment();

                EditorGUILayout.Space();

                this.textView.AlignMode = (TextAlignMode)EditorGUILayout.EnumPopup("Align Mode", this.textView.AlignMode);
                this.textView.CharacterSpacing = EditorGUILayout.FloatField("Character Spacing", this.textView.CharacterSpacing);
                this.textView.SpacingUnit = (SpacingUnit) EditorGUILayout.EnumPopup("Spacing Unit", this.textView.SpacingUnit);
                this.textView.TrimLineTailSpacing = EditorGUILayout.Toggle("Trim Line Tail Spacing", this.textView.TrimLineTailSpacing);
                this.textView.LineHeight = EditorGUILayout.FloatField("Line Height", this.textView.LineHeight);
                this.textView.IsLineHeightFixed = EditorGUILayout.Toggle("Fixed", this.textView.IsLineHeightFixed);
                this.textView.HorizontalOverflow = (HorizontalWrapMode) EditorGUILayout.EnumPopup("Horizontal Overflow", this.textView.HorizontalOverflow);
                this.textView.VerticalOverflow = (VerticalWrapMode) EditorGUILayout.EnumPopup("Vertical Overflow", this.textView.VerticalOverflow);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 表示量を編集します。
        /// </summary>
        protected virtual void EditVisibility()
        {
            GUILayout.Space(2.5f);
            if (!EditorUtilsTools.DrawGroupHeader("Visibility", KeyHeader + ".Visibility"))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                this.textView.VisibleLength = EditorGUILayout.IntField("Visible Length", this.textView.VisibleLength);
                this.textView.VisibleLineStart = EditorGUILayout.IntField("Visible Line Start", this.textView.VisibleLineStart);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// プロバイダを編集します。
        /// </summary>
        protected virtual void EditProvider()
        {
            GUILayout.Space(2.5f);
            if (!EditorUtilsTools.DrawGroupHeader("Extension", KeyHeader + ".Option", false))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                this.textView.ExtensibilityProvider = EditorGUILayout.ObjectField(
                    "Extensibility Provider",
                    this.textView.ExtensibilityProvider,
                    typeof(ExtensibilityProvider),
                    true) as ExtensibilityProvider;

                MonoBehaviour inlineImageProviderTarget = EditorGUILayout.ObjectField(
                    "Inline Image Provider",
                    this.textView.InlineImageProvider,
                    typeof(MonoBehaviour), true) as MonoBehaviour;

                if (inlineImageProviderTarget != this.textView.InlineImageProvider)
                {
                    if (inlineImageProviderTarget == null)
                    {
                        this.textView.InlineImageProvider = null;
                    }
                    else
                    {
                        IInlineImageProvider provider = inlineImageProviderTarget.GetComponent<IInlineImageProvider>();
                        this.textView.InlineImageProvider = provider as MonoBehaviour;
                        if (provider == null)
                        {
                            Debug.LogWarning("TextView's inline image provider target need to has IInlineImageProvider!");
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// コールバックを編集します。
        /// </summary>
        protected virtual void EditCallback()
        {
            GUILayout.Space(2.5f);

            if (this.textView.VerticalOverflow == VerticalWrapMode.Truncate)
            {
                DelegatableObjectEditor.DrawEditFields("On Overflow Text Lines",
                    textView,
                    textView.OnOverflowTextLines,
                    typeof(AuthorizedAccessAttribute),
                    KeyHeader + ".OnOverflowTextLines");

                GUILayout.Space(2.5f);
            }

            // テキストをパースする際のエラーハンドリング設定.
            DelegatableObjectEditor.DrawEditFields("On Text Parse Error",
                textView,
                textView.OnTextParseError,
                typeof(AuthorizedAccessAttribute),
                KeyHeader + ".OnTextParseError",
                false
            );
        }

        /// <summary>
        /// TextViewの主機能の分類に含まれない項目を編集します。
        /// </summary>
        protected virtual void EditEtcetera()
        {
            GUILayout.Space(2.5f);
            if (!EditorUtilsTools.DrawGroupHeader("Etcetera", KeyHeader + ".Etcetera", false))
            {
                return;
            }

            EditorUtilsTools.FitContentToHeader();

            serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(raycastTargetProperty);

                raycastPadding.DrawInspector(targets);
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// アライメントを編集します。
        /// </summary>
        protected void EditAlignment()
        {
            EditorGUILayout.LabelField("Alignment");

            EditorGUILayout.BeginHorizontal();
            {
                TextAlignToggle(TextAlign.Left, EditorStyles.miniButtonLeft);
                TextAlignToggle(TextAlign.Center, EditorStyles.miniButtonMid);
                TextAlignToggle(TextAlign.Right, EditorStyles.miniButtonMid);
                TextAlignToggle(TextAlign.Justify, EditorStyles.miniButtonMid);
                TextAlignToggle(TextAlign.JustifyAll, EditorStyles.miniButtonRight);
            }
            EditorGUILayout.EndHorizontal();

            if (textView.Alignment == TextAlign.Justify || textView.Alignment == TextAlign.JustifyAll)
            {
                EditorGUILayout.Space();
                this.textView.Justify = (TextJustify) EditorGUILayout.EnumPopup("Justify", this.textView.Justify);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Vertical Alignment");

            EditorGUILayout.BeginHorizontal();
            {
                VerticalAlignToggle(TextVerticalAlign.Top, EditorStyles.miniButtonLeft);
                VerticalAlignToggle(TextVerticalAlign.Center, EditorStyles.miniButtonMid);
                VerticalAlignToggle(TextVerticalAlign.Bottom, EditorStyles.miniButtonRight);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 横方向のアライメントを編集するToggleを表示します、。
        /// </summary>
        /// <param name="alignment">横方向のアライメント</param>
        /// <param name="style">スタイル</param>
        protected void TextAlignToggle(TextAlign alignment, GUIStyle style)
        {
            bool selected = textView.Alignment == alignment;
            if (selected != GUILayout.Toggle(selected, alignIcons[alignment], style, GUILayout.Width(32)))
            {
                textView.Alignment = alignment;
            }
        }

        /// <summary>
        /// 横方向のアライメントを編集するToggleを表示します、。
        /// </summary>
        /// <param name="alignment">横方向のアライメント</param>
        /// <param name="style">スタイル</param>
        protected void VerticalAlignToggle(TextVerticalAlign alignment, GUIStyle style)
        {
            bool selected = textView.VerticalAlignment == alignment;
            if (selected != GUILayout.Toggle(selected, verticalAlignIcons[alignment], style, GUILayout.Width(32)))
            {
                textView.VerticalAlignment = alignment;
            }
        }

#region utility

        /// <summary>
        /// 高さを自動設定するかどうかを編集します。
        /// </summary>
        protected virtual void EditHeightAdjustment()
        {
            bool needsRecalculation = false;

            // UI 描画
            GUILayout.Space(2.5f);
            if (EditorUtilsTools.DrawGroupHeader("Height Adjustment", KeyHeader + ".HightAdjustment", false))
            {
                serializedObject.Update();

                EditorUtilsTools.FitContentToHeader();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.PropertyField(heightAdjustmentProperty, new GUIContent("Fit One Line Height"));

                    EditorGUI.BeginDisabledGroup(!heightAdjustmentProperty.boolValue);
                    {
                        compositedGUIChanged |= GUI.changed;
                        GUI.changed = false;
                        EditorGUILayout.PropertyField(heightAdjustmentWithRubyProperty,
                            new GUIContent("Considering Ruby Height"));
                        needsRecalculation = GUI.changed;
                    }
                    EditorGUI.EndDisabledGroup();

                    if (heightAdjustmentProperty.boolValue)
                    {
                        EditorGUILayout.HelpBox(
                            "1行の表示に必要な最小の高さにします。size タグ・属性や offset 属性・インライン画像については考慮されません。",
                            MessageType.Info
                        );
                    }
                }
                EditorGUILayout.EndVertical();

                serializedObject.ApplyModifiedProperties();
            }

            if (!heightAdjustmentProperty.boolValue)
            {
                return;
            }

            if (needsRecalculation || !this.textView.Font.Equals(lastFont) || this.textView.FontSize != lastFontSize)
            {
                CalculateAdjustHeight();
            }

            if (textView.rectTransform.rect.height != adjustmentHeight)
            {
                RectTransformUtils.SetSize(
                    textView.rectTransform,
                    new Vector2(textView.rectTransform.rect.width, adjustmentHeight),
                    adjustPosition: false
                );
            }
        }

        /// <summary>
        /// 必要な高さを計算します。
        /// </summary>
        protected virtual void CalculateAdjustHeight()
        {
            // 1行の表示に必要な最小の高さを計算する。
            // textView.prefferedHeight を使うと入力テキストが空のときに高さが 0 になってしまうのでそれを避けるために、
            // “M” を仮の文字としてテキストのフォーマットをし、行の高さを算出している。
            // size タグ・image タグ・size 属性・offset 属性は考慮しないことになっている。
            // ruby タグは heightAdjustmentWithRuby が true のときのみ考慮する。
            var textRuns = new[] {
                        heightAdjustmentWithRubyProperty.boolValue ?
                            new TextCharactersRubyGroup("M", "M") :
                            new TextCharacters("M")
                    };
            // TODO: 雑にコピペで持ってきたのでリファクタリングすること
            TextSourceCompilerProperty sourceCompilerProperty = new TextSourceCompilerProperty(textView);
            var textSourceCompiler = new TextSourceCompiler(sourceCompilerProperty);
            var logicalLines = textSourceCompiler.Compile(new TextSource(textRuns, null));

            var allGlyphSpecs = new List<FontGlyphSpec>();
            foreach (var g in logicalLines.SelectMany(line => line))
            {
                foreach (var main in g.Groups.SelectMany(gg => gg.Mains))
                {
                    if (main.GlyphSpec is FontGlyphSpec)
                    {
                        allGlyphSpecs.Add((FontGlyphSpec) main.GlyphSpec);
                    }
                    else if (main.GlyphSpec is ImageGlyphSpec)
                    {
                        allGlyphSpecs.Add(((ImageGlyphSpec) main.GlyphSpec).SizeBaseGlyphSpec);
                    }
                }

                foreach (var spec in g.Groups.SelectMany(gg => gg.Rubies))
                {
                    allGlyphSpecs.Add(spec);
                }
            }
            var glyphCatalog = GlyphCatalog.GetCatalog(textView.Font);
            glyphCatalog.CreateGlyphs(allGlyphSpecs);
            var option = new LineRenderingElementsAssembly.AssembleOption(textView);

            var result = new List<TextLine>();

            foreach (var logicalLine in logicalLines)
            {
                var assembledLogicalLine = LineRenderingElementsAssembly.Assemble(logicalLine, glyphCatalog, option);

                var physicalLines = AutoLineBreakRule.Apply(
                    assembledLogicalLine,
                    float.MaxValue,
                    LineBreakRule.GetLineBreakRule(textView.LineBreakRule),
                    false,
                    null,
                    false
                );

                result.AddRange(physicalLines);
            }
            var textLines = result.ToArray();

            lastFont = this.textView.Font;
            lastFontSize = this.textView.FontSize;
            adjustmentHeight = textLines[0].PlacedGlyphs.Aggregate(
                0,
                (max, p) => (int) Math.Max(max, p.Glyph.Height - p.Y)
            );
        }

        protected void EditLanguage()
        {
            GUILayout.Space(2.5f);
            if (EditorUtilsTools.DrawGroupHeader("Language", KeyHeader + ".Language", false))
            {
                this.textView.LanguageType = (TextLanguageType) EditorGUILayout.EnumPopup("LanguageType", this.textView.LanguageType);
            }
        }
        
        /// <summary>
        /// TextViewの設定がシュリンクと矛盾しているかどうかを確認します。
        /// </summary>
        protected void CheckTextViewSetting()
        {
            isInconsistentShrinkSetting = false;
            isInconsistentSqueezeSetting = false;
            isInconsistentEllipsisSetting = false;
            
            if (this.textView.HorizontalOverflow == HorizontalWrapMode.Overflow
                || this.textView.VerticalOverflow == VerticalWrapMode.Overflow)
            {
                isInconsistentShrinkSetting = true;
                isInconsistentSqueezeSetting = true;
            }

            if (this.textView.VerticalOverflow == VerticalWrapMode.Overflow)
            {
                isInconsistentEllipsisSetting = true;
            }
        }

#endregion

#region debug

        /// <summary>
        /// デバッグ用の項目を編集します。
        /// </summary>
        protected void EditDebugOptions()
        {
            GUILayout.Space(2.5f);

            // 初期値は非表示.
            if (!EditorPrefs.HasKey(IsShowGizmosBodyEditorKey))
            {
                EditorPrefs.SetBool(IsShowGizmosBodyEditorKey, false);
            }
            if (!EditorPrefs.HasKey(PreferredWidthTypeKey))
            {
                EditorPrefs.SetInt(PreferredWidthTypeKey, (int) PreferredWidthType.FirstLine);
            }
            if (!EditorPrefs.HasKey(PreferredHeightTypeKey))
            {
                EditorPrefs.SetInt(PreferredHeightTypeKey, (int) PreferredHeightType.AllLine);
            }

            if (!EditorUtilsTools.DrawGroupHeader("Debug", KeyHeader + ".Debug", false))
            {
                return;
            }
            EditorUtilsTools.FitContentToHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                // 仮想ボディの表示
                EditShowGizmosBody();
                EditSizeToPreferred();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 文字毎の表示領域をギズモによって可視化するかどうかを編集します。
        /// </summary>
        protected virtual void EditShowGizmosBody()
        {
            bool currentValue = EditorPrefs.GetBool(IsShowGizmosBodyEditorKey);
            bool changeValue = EditorGUILayout.Toggle("Show Gizmos Body", currentValue);
            if (currentValue != changeValue)
            {
                EditorPrefs.SetBool(IsShowGizmosBodyEditorKey, changeValue);
                SceneView.RepaintAll();
            }
        }

        /// <summary>
        /// 矩形範囲のサイズを設定されているテキストに応じて必要なサイズに設定します。
        /// </summary>
        protected virtual void EditSizeToPreferred()
        {
            EditorGUILayout.BeginHorizontal();
            {
                PreferredWidthType type = (PreferredWidthType) EditorPrefs.GetInt(PreferredWidthTypeKey);
                PreferredWidthType edited = (PreferredWidthType) EditorGUILayout.EnumPopup("Preferred Width", type);
                if (type != edited)
                {
                    type = edited;
                    EditorPrefs.SetInt(PreferredWidthTypeKey, (int) type);
                }
                if (GUILayout.Button("Set Size"))
                {
                    Vector2 size = textView.rectTransform.rect.size;
                    size.x = textView.GetPreferredWidth(type);
                    RectTransformUtils.SetSize(textView.rectTransform, size);
                    Debug.Log("Preferred Width : " + size.x);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                PreferredHeightType type = (PreferredHeightType) EditorPrefs.GetInt(PreferredHeightTypeKey);
                PreferredHeightType edited = (PreferredHeightType) EditorGUILayout.EnumPopup("Preferred Height", type);
                if (type != edited)
                {
                    type = edited;
                    EditorPrefs.SetInt(PreferredHeightTypeKey, (int) type);
                }
                if (GUILayout.Button("Set Size"))
                {
                    Vector2 size = textView.rectTransform.rect.size;
                    size.y = textView.GetPreferredHeight(type);
                    RectTransformUtils.SetSize(textView.rectTransform, size);
                    Debug.Log("Preferred Height : " + size.y);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 仮想ボディの表示領域を表示します。
        /// </summary>
        protected void DrawArea(Vector3 sizeRate, Vector3 origin, object element, float offsetY, bool enableMirror, float mirrorOffsetX)
        {
            if (element is HorizontalLayoutedGlyph)
            {
                HorizontalLayoutedGlyph layoutedGlyph = element as HorizontalLayoutedGlyph;
                if (!layoutedGlyph.GlyphPlacement.Glyph.IsWhiteSpaceOrControl)
                {
                    UIVertex[] vertices = new UIVertex[4];
                    if (!enableMirror)
                    {
                        layoutedGlyph.GetVertices(ref vertices, -offsetY, Color.white, layoutedGlyph.GlyphPlacement.PunctuationHalfType);
                    }
                    else
                    {
                        layoutedGlyph.GetMirrorVertices(ref vertices, -offsetY, Color.white, mirrorOffsetX);
                    }

                    Handles.DrawAAPolyLine(
                        3.0f,
                        origin + Vector3.Scale(sizeRate, vertices[0].position),
                        origin + Vector3.Scale(sizeRate, vertices[1].position),
                        origin + Vector3.Scale(sizeRate, vertices[2].position),
                        origin + Vector3.Scale(sizeRate, vertices[3].position),
                        origin + Vector3.Scale(sizeRate, vertices[0].position)
                    );
                }
            }
            else if (element is HorizontalLayoutedImage)
            {
                var image = (HorizontalLayoutedImage) element;
                var imageGlyph = (InlineImageGlyph) image.GlyphPlacement.Glyph;

                Vector2 pos = new Vector2(
                                  image.OffsetX + image.GlyphPlacement.X,
                                  image.OffsetY - image.GlyphPlacement.Y - offsetY);
                // アラビア語の場合は鏡表示をしているためxの座標位置が変わる
                if (enableMirror)
                {
                    pos.x = -(image.OffsetX + image.GlyphPlacement.X + imageGlyph.Width) + mirrorOffsetX;
                }
                float left = sizeRate.x * pos.x;
                float right = sizeRate.x * (pos.x + imageGlyph.Width);
                float bottom = sizeRate.y * pos.y;
                float top = sizeRate.y * (pos.y + imageGlyph.Height);

                Handles.DrawAAPolyLine(
                    3.0f,
                    origin + new Vector3(left, top),
                    origin + new Vector3(right, top),
                    origin + new Vector3(right, bottom),
                    origin + new Vector3(left, bottom),
                    origin + new Vector3(left, top)
                );
            }
        }

        /// <summary>
        /// TextViewOverflowExtension関連の設定
        /// </summary>
        void SetUpTextViewOverflow()
        {
            TextViewOverflowExtension overflowExtension = textView.GetComponent<TextViewOverflowExtension>();
            
            if (overflowExtension == null)
            {
                return;
            }
            
            if (textViewUpdateMethod == null)
            {
                textViewUpdateMethod = typeof(TextView).GetMethod("ForceUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            serializedObject.Update();
            if (shrinkComponentProperty.objectReferenceValue != overflowExtension)
            {
                shrinkComponentProperty.objectReferenceValue = overflowExtension;
                serializedObject.ApplyModifiedProperties();

                textView.RequireProcess(WorkflowProcess.LogicalLinesCreate);
                overflowExtension.RequireCalculate();
            }
        }

#endregion

#endregion

#region private methods

        /// <summary>
        /// アライメントのアイコン画像を読み込みます。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static Texture LoadIcon(string filename)
        {
            return AssetDatabase.LoadAssetAtPath(AlignmentIconDirectory + filename, typeof(Texture)) as Texture;
        }

        /// <summary>
        /// UndoとRedoの際にTextViewにプロセスの実行を要求します
        /// </summary>
        void OnUndoRedoRequireProcess()
        {
#pragma warning disable 219

            // TextViewのOnPopulateMeshが呼ばれるタイミングがLateUpdate以降のため
            // プロセス処理よりも前にOnPopulateMeshが呼ばれてしまう
            // そのため、フレームを跨いで処理を行う。
            EditorSerialProcessor processor = new EditorSerialProcessor(false,
                (f) =>
                {
                    // プレハブモード時にTextViewのDestroyが呼ばれたあとにEditorのOnOnDisableが呼ばれる前に
                    // OnUndoRedoRequireProcessが呼ばれる可能性があるのでNullチェックを追加
                    if (textView)
                    {
                        // TextSourceの再生成を行うため、別の文字列を入れる
                        var text = textView.Text;
                        textView.Text = text + "(Temp)";
                        textView.Text = text;
                    }
                    return false;
                },
                (f) =>
                {
                    if (textView)
                    {
                        // TextViewObserverで処理が行われた後のフレームで再度Renderingプロセスを呼ぶ
                        textView.SetVerticesDirty();
                    }

                    return false;
                });
            
#pragma warning restore 219
        }

#endregion

#region override unity methods

        protected virtual void OnEnable()
        {
            LoadIcons();

            textView = target as TextView;

            GetSerializedProperties();
            GetNoneSerializedFields();
            SetUpTextViewOverflow();
            Undo.undoRedoPerformed += OnUndoRedoRequireProcess;

            CheckTextViewSetting();
            
            raycastPadding = new RaycastPaddingInspector();
        }

        protected virtual void OnDisable()
        {
            IsBodyFocus = false;
            Undo.undoRedoPerformed -= OnUndoRedoRequireProcess;
        }

        public override void OnInspectorGUI()
        {
            if (textView == null)
            {
                return;
            }
            
            EditorGUI.BeginChangeCheck();

            compositedGUIChanged = false;

            EditStyle();
            EditTextViewOverflowExtension();
            EditBody();
            EditParagraph();
            EditVisibility();
            EditProvider();
            EditCallback();
            EditHeightAdjustment();
            EditLanguage();
            EditEtcetera();
            EditDebugOptions();
            
            compositedGUIChanged |= GUI.changed;
            EditorUtilsTools.RegisterUndo("Edit TextView", compositedGUIChanged, targets);
            if (compositedGUIChanged)
            {
                CheckTextViewSetting();
            }
            EditorGUI.EndChangeCheck();
        }

        protected virtual void OnSceneGUI()
        {
            if (textView == null || Selection.objects.Length > 1)
            {
                return;
            }

            if (!EditorPrefs.HasKey(IsShowGizmosBodyEditorKey)
                || !EditorPrefs.GetBool(IsShowGizmosBodyEditorKey))
            {
                return;
            }

            Color temp = Handles.color;
            Handles.color = Color.green;

            var horizontalLayout = horizontalLayoutField.GetValue(textView) as HorizontalLayout;
            var offsetY = System.Convert.ToSingle(offsetYField.GetValue(textView)) +
                          System.Convert.ToSingle(visibleStartLineOffsetYField.GetValue(textView));
            Vector3 sizeRate = textView.rectTransform.lossyScale;
            Vector3 origin = textView.rectTransform.position;
            bool enableMirror = Convert.ToBoolean(this.enableMirrorField.GetValue(textView));

            // アラビア語対応用ロジック
            // TextViewのFillVerticesメソッド参照
            float mirrorOffsetX = 0f;
            if (enableMirror)
            {
                mirrorOffsetX = textView.rectTransform.rect.width * (0.5f - textView.rectTransform.pivot.x) * 2.0f;
            }

            if (horizontalLayout != null)
            {
                foreach (var element in horizontalLayout)
                {
                    DrawArea(sizeRate, origin, element, offsetY, enableMirror, mirrorOffsetX);
                }
            }

            Handles.color = temp;
        }

        protected virtual void OnDestroy()
        {
            try
            {
                TextViewOverflowExtension overflowExtension = textView.GetComponent<TextViewOverflowExtension>();

                if (overflowExtension != null && textViewUpdateMethod != null)
                {
                    textView.RequireProcess(WorkflowProcess.LogicalLinesCreate);
                    overflowExtension.RequireCalculate();
                    
                    // 非実行状態だと、表示と更新の呼び出し順が逆になって、
                    // 表示情報を計算する前にOnPopulateMeshが呼び出されて、
                    // 表示情報がないままにデータを詰めようとして表示が消えてしまうので、
                    // 処理要求後、すぐに強制的に更新を行うようにする
                    textViewUpdateMethod.Invoke(textView, null);
                }
            }
            catch (MissingReferenceException)
            {
                // 再生状態切り替えタイミングでやるとMissingReferenceExceptionが発生するが、
                // ここで処理しておかないと、コンポーネントを外したタイミングで再計算できないので
                // Exceptionは握りつぶす
            }
        }
        
#endregion
    }

}
