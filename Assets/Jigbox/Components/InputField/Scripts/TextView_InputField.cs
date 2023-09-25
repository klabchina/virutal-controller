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

using System;
using Jigbox.Delegatable;
using Jigbox.TextView;
using Jigbox.UIControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jigbox.Components
{
    [RequireComponent(typeof(InputFieldSub))]
    [DisallowMultipleComponent]
    public class TextView_InputField : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        /// <summary>単語単位分割文字一覧</summary>
        protected static readonly char[] Separators = { ' ', '.', ',', '\t', '\r', '\n' };

        /// <summary>
        /// InputFieldイベント用デリゲート型
        /// </summary>
        public class InputFieldEventDelegate : EventDelegate<string>
        {
            public InputFieldEventDelegate(Callback callback) : base(callback)
            {
            }
        }

        /// <summary>uGUIInputField継承コンポーネント</summary>
        [SerializeField, HideInInspector]
        protected InputFieldSub inputFieldSub = null;

        /// <summary>uGUIInputField継承コンポーネントの参照</summary>
        public InputFieldSub InputField
        {
            get { return inputFieldSub; }
            set { inputFieldSub = value; }
        }

        /// <summary>TextView</summary>
        [SerializeField, HideInInspector]
        protected InputTextView textView = null;

        /// <summary>TextViewの参照</summary>
        public InputTextView TextViewComponent
        {
            get { return textView; }
            set { textView = value; }
        }

        /// <summary>テキストの表示領域</summary>
        [SerializeField, HideInInspector]
        protected RectTransform textViewport = null;

        /// <summary>テキストの表示領域の参照</summary>
        public RectTransform TextViewport
        {
            get { return textViewport; }
            set { textViewport = value; }
        }

        /// <summary>入力が変わった時のコールバック</summary>
        [SerializeField, HideInInspector]
        protected DelegatableList onValueChangedDelegates = new DelegatableList();

        /// <summary>入力が変わった時のコールバック</summary>
        public DelegatableList OnValueChangedDelegates
        {
            get { return onValueChangedDelegates; }
        }

        /// <summary>編集終了時のコールバック</summary>
        [SerializeField, HideInInspector]
        protected DelegatableList onEndEditDelegates = new DelegatableList();

        /// <summary>編集終了時のコールバック</summary>
        public DelegatableList OnEndEditDelegates
        {
            get { return onEndEditDelegates; }
        }

        [NonSerialized]
        protected Mesh mesh;

        /// <summary>キャレット描画用のメッシュ</summary>
        protected Mesh Mesh
        {
            get
            {
                if (mesh == null)
                {
                    mesh = new Mesh();
                }

                return mesh;
            }
        }

        /// <summary>入力可能かどうか</summary>
        public bool Interactive
        {
            get { return inputFieldSub.interactable; }
            set
            {
                inputFieldSub.interactable = value;
#if UNITY_EDITOR
                // 実行中以外は色の変更はしない
                if (!Application.isPlaying)
                {
                    if (isSyncColor)
                    {
                        isEnableColor = inputFieldSub.interactable;
                    }

                    return;
                }
#endif
                if (isSyncColor)
                {
                    ChangeColor();
                }

                if (!inputFieldSub.interactable)
                {
                    DeactivateInputField();
                }
            }
        }

        /// <summary>入力文字列</summary>
        public string Text
        {
            get { return inputFieldSub.text; }
            set { inputFieldSub.text = value; }
        }

        /// <summary>パスワードフィールドに使用される文字</summary>
        public char AsteriskChar
        {
            get { return inputFieldSub.asteriskChar; }
            set { inputFieldSub.asteriskChar = value; }
        }

        /// <summary>入力制限文字数(カウントはユニコード文字毎)</summary>
        public int CharacterLimit
        {
            get { return inputFieldSub.characterLimit; }
            set { inputFieldSub.characterLimit = value; }
        }

        /// <summary>文字入力のバリデーション</summary>
        public InputField.CharacterValidation CharacterValidation
        {
            get { return inputFieldSub.characterValidation; }
            set
            {
                inputFieldSub.characterValidation = value;

                CheckCharacterValidation();
            }
        }

        /// <summary>使用されるモバイルキーボードの種類</summary>
        public TouchScreenKeyboardType KeyboardType
        {
            get { return inputFieldSub.keyboardType; }
            set { inputFieldSub.keyboardType = value; }
        }

        /// <summary>入力フィールドが期待するデータのタイプ</summary>
        public InputField.InputType InputType
        {
            get { return inputFieldSub.inputType; }
            set { inputFieldSub.inputType = value; }
        }

        /// <summary>入力の種類</summary>
        public InputField.ContentType ContentType
        {
            get { return inputFieldSub.contentType; }
            set
            {
                inputFieldSub.contentType = value;

                CheckCharacterValidation();

                EnforceTextViewHorizontalOverflow();
            }
        }

        /// <summary>入力行の種類</summary>
        public InputField.LineType LineType
        {
            get { return inputFieldSub.lineType; }
            set
            {
                inputFieldSub.lineType = value;
                EnforceTextViewHorizontalOverflow();
            }
        }

        /// <summary>キャレットの幅</summary>
        public int CaretWidth
        {
            get { return inputFieldSub.caretWidth; }
            set { inputFieldSub.caretWidth = value; }
        }

        /// <summary>キャレットの点滅速度</summary>
        public float CaretBlinkRate
        {
            get { return inputFieldSub.caretBlinkRate; }
            set { inputFieldSub.caretBlinkRate = value; }
        }

        /// <summary>キャレットの色</summary>
        public Color CaretColor
        {
            get { return inputFieldSub.customCaretColor ? inputFieldSub.caretColor : textView.color; }
            set { inputFieldSub.caretColor = value; }
        }

        /// <summary>カスタムキャレットの色</summary>
        public bool CustomCaretColor
        {
            get { return inputFieldSub.customCaretColor; }
            set { inputFieldSub.customCaretColor = value; }
        }

        /// <summary>選択色</summary>
        public Color SelectionColor
        {
            get { return inputFieldSub.selectionColor; }
            set { inputFieldSub.selectionColor = value; }
        }

        /// <summary>プレースホルダー</summary>
        public Graphic Placeholder
        {
            get { return inputFieldSub.placeholder; }
            set { inputFieldSub.placeholder = value; }
        }

        /// <summary>読み取り専用</summary>
        public bool ReadOnly
        {
            get { return inputFieldSub.readOnly; }
            set { inputFieldSub.readOnly = value; }
        }

#if UNITY_2020_3_OR_NEWER
        /// <summary>選択した時に自動で入力を有効化するか</summary>
        public bool ShouldActivateOnSelect
        {
            get { return inputFieldSub.shouldActivateOnSelect; }
            set { inputFieldSub.shouldActivateOnSelect = value; }
        }
#endif

        /// <summary>TextViewの言語モード</summary>
        public TextLanguageType LanguageType
        {
            get { return textView.LanguageType; }
            set { textView.LanguageType = value; }
        }

        /// <summary>フォーカスしているか</summary>
        public bool IsFocused
        {
            get { return inputFieldSub.isFocused; }
        }

        /// <summary>複数行をサポートしているか</summary>
        public bool Multiline
        {
            get { return inputFieldSub.multiLine; }
        }

        /// <summary>ソフトウェアキーボード</summary>
        public TouchScreenKeyboard TouchScreenKeyboard
        {
            get { return inputFieldSub.touchScreenKeyboard; }
        }

        /// <summary>編集がキャンセルされてDeactivateInputFieldで元のテキストを表示したかどうか</summary>
        public bool WasCancel
        {
            get { return inputFieldSub.wasCanceled; }
        }

        /// <summary>
        /// ベースラインより下にはみ出る文字の場合、画面に入り切らない事がある
        /// TextViewのUpperBaseLineとUnderBaseLineの出し方では、タイ語やgなどの文字が入り切らない高さが取得される為
        /// 以下の計算は入るように目視で確認した状態の値と係数になっている
        /// 確認した文字列はタイ語の ฏู็๋ 
        /// </summary>
        protected virtual float AdjustViewPortBottomOffset
        {
            get { return 8f * textView.FontSize / 24.0f; }
        }

        /// <summary>キャレット表示用のオブジェクト</summary>
        protected TextView_InputFieldCaret InputFieldCaret { get; set; }

        /// <summary>
        /// 入力システム
        /// </summary>
        protected BaseInput InputSystem
        {
            get
            {
                if (EventSystem.current && EventSystem.current.currentInputModule)
                {
                    return EventSystem.current.currentInputModule.input;
                }

                return null;
            }
        }

        /// <summary>
        /// 入力中の文字列
        /// </summary>
        protected string CompositionString
        {
            get { return InputSystem != null ? InputSystem.compositionString : Input.compositionString; }
        }

        /// <summary>キャレットの位置</summary>
        protected int CaretPosition { get; set; }

        /// <summary>選択開始位置</summary>
        protected int SelectAnchorPosition { get; set; }

        /// <summary>選択終了位置</summary>
        protected int SelectFocusPosition { get; set; }

        /// <summary>選択中か</summary>
        protected bool HasSelection
        {
            get { return inputFieldSub.selectionAnchorPosition != inputFieldSub.selectionFocusPosition; }
        }

        /// <summary>入力のバリデーション</summary>
        protected InputField.OnValidateInput ValidateInput { get; set; }

        /// <summary>配下の画像情報管理クラス</summary>
        [SerializeField, HideInInspector]
        protected GraphicGroup graphicGroup;

        /// <summary>配下の画像情報</summary>
        protected GraphicComponentInfo imageInfo = null;

        /// <summary>配下の画像情報</summary>
        public GraphicComponentInfo ImageInfo
        {
            get
            {
                if (graphicGroup != null)
                {
                    return graphicGroup.GraphicInfo;
                }

                if (imageInfo == null)
                {
                    imageInfo = new GraphicComponentInfo();
                    imageInfo.FindGraphicComponents(gameObject);
                }

                return imageInfo;
            }
        }

        /// <summary>色の変更を有効にするかどうか</summary>
        [SerializeField, HideInInspector]
        protected bool isEnableColorChange = true;

        /// <summary>色の変更を有効にするかどうか</summary>
        public bool IsEnableColorChange
        {
            get { return isEnableColorChange; }
            set
            {
                if (isEnableColorChange != value)
                {
                    isEnableColorChange = value;

#if UNITY_EDITOR
                    // 実行中以外は色の変更はしない
                    if (!Application.isPlaying)
                    {
                        return;
                    }
#endif
                    // 色の変更が無効の場合はデフォルト色に設定
                    if (!isEnableColorChange)
                    {
                        ImageInfo.ResetColor();
                    }
                    else
                    {
                        ChangeColor();
                    }
                }
            }
        }

        /// <summary>状態と色を同期させるかどうか</summary>
        [SerializeField, HideInInspector]
        protected bool isSyncColor = true;

        /// <summary>状態と色を同期させるかどうか</summary>
        public bool IsSyncColor
        {
            get { return isSyncColor; }
            set
            {
                if (isSyncColor != value)
                {
                    isSyncColor = value;
#if UNITY_EDITOR
                    // 実行中以外は色の変更はしない
                    if (!Application.isPlaying)
                    {
                        return;
                    }
#endif
                    if (isSyncColor)
                    {
                        isEnableColor = inputFieldSub.interactable;
                        ChangeColor();
                    }
                }
            }
        }

        /// <summary>色が有効状態のものかどうか</summary>
        [SerializeField, HideInInspector]
        protected bool isEnableColor = true;

        /// <summary>色が有効状態のものかどうか</summary>
        public bool IsEnableColor
        {
            get { return isEnableColor; }
            set
            {
                // 色を状態と同期している間は外部から変更不可
                if (isSyncColor)
                {
                    return;
                }

                if (isEnableColor != value)
                {
                    isEnableColor = value;
#if UNITY_EDITOR
                    // 実行中以外は色の変更はしない
                    if (!Application.isPlaying)
                    {
                        return;
                    }
#endif
                    ChangeColor();
                }
            }
        }

        /// <summary>無効状態でのデフォルト色</summary>
        protected static readonly Color DisableColorDefault = new Color(0.5f, 0.5f, 0.5f);

        /// <summary>無効状態での色</summary>
        [SerializeField, HideInInspector]
        protected Color disableColor = DisableColorDefault;

        /// <summary>無効状態での色</summary>
        public Color DisableColor
        {
            get { return disableColor; }
            set
            {
                if (disableColor != value)
                {
                    disableColor = value;

#if UNITY_EDITOR
                    // 実行中以外は色の変更はしない
                    if (!Application.isPlaying)
                    {
                        return;
                    }
#endif
                    if (!isEnableColor)
                    {
                        SetColorMultiply(disableColor);
                    }
                }
            }
        }

        /// <summary>キャレット操作クラス</summary>
        protected TextViewCaretController textViewCaretController = null;

        /// <summary>キャレットの頂点データ</summary>
        protected UIVertex[] caretVerts = null;

        protected virtual void Awake()
        {
            inputFieldSub.KeyPressedCallback = KeyPressed;
            inputFieldSub.onValueChanged.AddListener(OnValueChanged);
            inputFieldSub.onEndEdit.AddListener(OnEndEdit);
            inputFieldSub.RebuildCallback = UpdateGeometry;
            CheckCharacterValidation();

            // Awakeで行うと、InputFieldに文字列が設定されている場合、キャレットのデータが正しく取れない
            // TextViewに設定されている文字と同じ為キャレットデータが生成されない
            textView.Text = string.Empty;
            UpdateLabel(Text);
        }

        /// <summary>RectTransformが変更されたイベント</summary>
        protected virtual void OnRectTransformDimensionsChange()
        {
            ForceLabelUpdate();
        }

        protected virtual void OnEnable()
        {
            ChangeColor();
            if (InputFieldCaret == null && inputFieldSub.textComponent != null)
            {
                // キャレットのGameObjectを作成する
                var go = new GameObject(transform.name + " Input Caret ", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextView_InputFieldCaret), typeof(LayoutElement));
                go.hideFlags = HideFlags.DontSave;
                go.transform.SetParent(textViewport);
                go.transform.SetAsFirstSibling();
                go.layer = gameObject.layer;

                InputFieldCaret = go.GetComponent<TextView_InputFieldCaret>();
                InputFieldCaret.raycastTarget = false;
                InputFieldCaret.color = Color.clear;

                InputFieldCaret.canvasRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);

                go.GetComponent<LayoutElement>().ignoreLayout = true;

                AssignPositioningIfNeeded();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(textView.rectTransform, eventData.position, eventData.pressEventCamera, out localMousePos);
            inputFieldSub.selectionFocusPosition = textViewCaretController.GetCaretPositionFromPosition(localMousePos);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsFocused)
            {
                return;
            }

            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(textView.rectTransform, eventData.position, eventData.pressEventCamera, out localMousePos);
            var caretPosition = textViewCaretController.GetCaretPositionFromPosition(localMousePos);
            inputFieldSub.caretPosition = caretPosition;

            eventData.Use();
        }

        protected virtual void OnDestroy()
        {
            inputFieldSub.onValueChanged.RemoveAllListeners();
            inputFieldSub.onEndEdit.RemoveAllListeners();
        }

        /// <summary>
        /// TextViewの表示を更新します
        /// </summary>
        /// <param name="message"></param>
        protected virtual void UpdateLabel(string message)
        {
            // パスワード入力の場合は、パスワード表示にする
            if (InputType == UnityEngine.UI.InputField.InputType.Password)
            {
                message = new string(AsteriskChar, message.Length);
            }

            if (!TextViewComponent.Text.Equals(message) || string.IsNullOrEmpty(message))
            {
                TextViewCaretUnitProxy.Initialize();
            }

            TextViewComponent.SetTextWithEscape(message);
            textViewCaretController = TextViewComponent.GetTextViewCaretController();
        }

        /// <summary>
        /// キャレットを生成する
        /// </summary>
        /// <param name="vbo"></param>
        protected virtual void GenerateCaret(VertexHelper vbo)
        {
            if (!inputFieldSub.CaretVisible)
            {
                return;
            }

            if (caretVerts == null)
            {
                CreateCaretVerts();
            }

            var width = CaretWidth;
            var caretPosition = inputFieldSub.caretPosition;
            var caretInfo = textViewCaretController.GetCaretInfo(caretPosition);
            var startPosition = caretInfo.position;
            var height = caretInfo.caretHeight;

            AdjustRectTransformRelativeToViewport(startPosition, height, true);

            for (int i = 0; i < caretVerts.Length; i++)
            {
                caretVerts[i].color = CaretColor;
            }

            caretVerts[0].position = new Vector3(startPosition.x, startPosition.y, 0.0f);
            caretVerts[1].position = new Vector3(startPosition.x, startPosition.y + height, 0.0f);
            caretVerts[2].position = new Vector3(startPosition.x + width, startPosition.y + height, 0.0f);
            caretVerts[3].position = new Vector3(startPosition.x + width, startPosition.y, 0.0f);

            vbo.AddUIVertexQuad(caretVerts);
        }

        /// <summary>
        /// 文字選択表示を作成する
        /// </summary>
        /// <param name="vbo"></param>
        protected virtual void GenerateHighlight(VertexHelper vbo)
        {
            var startIndex = inputFieldSub.selectionAnchorPosition;
            var endIndex = inputFieldSub.selectionFocusPosition;

            if (startIndex > endIndex)
            {
                var tmp = startIndex;
                startIndex = endIndex;
                endIndex = tmp;
            }

            var caretPosition = inputFieldSub.selectionFocusPosition;
            var caretInfo = textViewCaretController.GetCaretInfo(caretPosition);
            var startPosition = caretInfo.position;
            var height = caretInfo.caretHeight;

            AdjustRectTransformRelativeToViewport(startPosition, height, true);

            var vert = UIVertex.simpleVert;
            vert.uv0 = Vector2.zero;
            vert.color = inputFieldSub.selectionColor;

            foreach (var rect in textViewCaretController.SelectRect(startIndex, endIndex))
            {
                var vertIndex = vbo.currentVertCount;
                vert.position = new Vector3(rect.xMin, rect.yMin, 0.0f);
                vbo.AddVert(vert);

                vert.position = new Vector3(rect.xMin, rect.yMax, 0.0f);
                vbo.AddVert(vert);

                vert.position = new Vector3(rect.xMax, rect.yMax, 0.0f);
                vbo.AddVert(vert);

                vert.position = new Vector3(rect.xMax, rect.yMin, 0.0f);
                vbo.AddVert(vert);

                vbo.AddTriangle(vertIndex, vertIndex + 1, vertIndex + 2);
                vbo.AddTriangle(vertIndex + 2, vertIndex + 3, vertIndex + 0);
            }
        }

        /// <summary>
        /// キャレットの頂点データの配列を作成する
        /// </summary>
        protected virtual void CreateCaretVerts()
        {
            caretVerts = new UIVertex[4];

            for (int i = 0; i < caretVerts.Length; i++)
            {
                caretVerts[i] = UIVertex.simpleVert;
                caretVerts[i].uv0 = Vector2.zero;
            }
        }

        /// <summary>
        /// 行頭へ移動する
        /// </summary>
        protected virtual void MoveToStartOfLine()
        {
            var caretPosition = textViewCaretController.MoveToStartOfLine(CaretPosition);
            CaretPosition = caretPosition;
            inputFieldSub.caretPosition = caretPosition;
        }

        /// <summary>
        /// 行末へ移動する
        /// </summary>
        protected virtual void MoveToEndOfLine()
        {
            var caretPosition = textViewCaretController.MoveToEndOfLine(CaretPosition);
            CaretPosition = caretPosition;
            inputFieldSub.caretPosition = caretPosition;
        }

        /// <summary>
        /// 左移動操作
        /// Ctrl押している場合の単語単位の移動は元のInputFieldの実装を利用している
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="ctrl"></param>
        protected virtual void MoveLeft(bool shift, bool ctrl)
        {
            int caretPosition;
            if (ctrl)
            {
                caretPosition = FindPreviousWordBegin();
            }
            else
            {
                caretPosition = textViewCaretController.MovePrevious(CaretPosition);
            }

            if (shift)
            {
                SelectFocusPosition = caretPosition;
                inputFieldSub.selectionFocusPosition = caretPosition;
            }
            else
            {
                CaretPosition = caretPosition;
                inputFieldSub.caretPosition = caretPosition;
            }
        }

        /// <summary>
        /// 1つ前のワードの開始地点を検索する
        /// </summary>
        /// <returns>文字列のインデックス</returns>
        protected virtual int FindPreviousWordBegin()
        {
            // Separatorsで設定したindexから探すと同じindexが見つかってしまう
            // 1つ前のワードの位置をSeparatorsの文字の1文字横としている為、2引いている
            if (CaretPosition - 2 < 0)
            {
                return 0;
            }

            var spaceLocation = Text.LastIndexOfAny(Separators, CaretPosition - 2);

            // 見つからなかった場合文頭
            if (spaceLocation == -1)
            {
                spaceLocation = 0;
            }
            else
            {
                spaceLocation++;
            }

            return spaceLocation;
        }

        /// <summary>
        /// 次のワードの開始地点を検索する
        /// </summary>
        /// <returns></returns>
        protected virtual int FindNextWordBegin()
        {
            // 同じ位置で次のワードの開始位置を探すと、同じIndexが返ってくる為+1したCaretPositionから探している
            if (CaretPosition + 1 >= Text.Length)
            {
                return Text.Length;
            }

            var spaceLocation = Text.IndexOfAny(Separators, CaretPosition + 1);

            // 見つからなかった場合文末
            if (spaceLocation == -1)
            {
                spaceLocation = Text.Length;
            }
            else
            {
                spaceLocation++;
            }

            return spaceLocation;
        }

        /// <summary>
        /// 右移動操作
        /// Ctrl押している場合の単語単位の移動は元のInputFieldの実装を利用している
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="ctrl"></param>
        protected virtual void MoveRight(bool shift, bool ctrl)
        {
            int caretPosition;
            if (ctrl)
            {
                caretPosition = FindNextWordBegin();
            }
            else
            {
                caretPosition = textViewCaretController.MoveNext(CaretPosition);
            }

            if (shift)
            {
                SelectFocusPosition = caretPosition;
                inputFieldSub.selectionFocusPosition = caretPosition;
            }
            else
            {
                CaretPosition = caretPosition;
                inputFieldSub.caretPosition = caretPosition;
            }
        }

        /// <summary>
        /// 現在のキャレットのX座標を見て、そのX座標がどの文字箇所なのか確認する
        /// </summary>
        /// <param name="shift"></param>
        protected virtual void MoveUp(bool shift)
        {
            var caretPosition = textViewCaretController.MoveUp(CaretPosition);

            if (shift)
            {
                SelectFocusPosition = caretPosition;
                inputFieldSub.selectionFocusPosition = caretPosition;
            }
            else
            {
                CaretPosition = caretPosition;
                inputFieldSub.caretPosition = CaretPosition;
            }
        }

        /// <summary>
        /// 下移動
        /// </summary>
        /// <param name="shift"></param>
        protected virtual void MoveDown(bool shift)
        {
            var caretPosition = textViewCaretController.MoveDown(CaretPosition);

            if (shift)
            {
                SelectFocusPosition = caretPosition;
                inputFieldSub.selectionFocusPosition = caretPosition;
            }
            else
            {
                CaretPosition = caretPosition;
                inputFieldSub.caretPosition = CaretPosition;
            }
        }

        /// <summary>
        /// 文字を削除する
        /// </summary>
        protected virtual void Delete()
        {
            if (ReadOnly)
            {
                return;
            }

            if (!HasSelection)
            {
                return;
            }

            var start = inputFieldSub.selectionAnchorPosition;
            var end = inputFieldSub.selectionFocusPosition;

            if (start > end)
            {
                var tmp = start;
                start = end;
                end = tmp;
            }

            Text = Text.Substring(0, start) + Text.Substring(end, Text.Length - end);
            inputFieldSub.selectionAnchorPosition = start;
            inputFieldSub.selectionFocusPosition = start;
        }

        /// <summary>
        /// 前の文字を削除する
        /// </summary>
        protected virtual void ForwardDelete()
        {
            if (ReadOnly)
            {
                return;
            }

            if (HasSelection)
            {
                Delete();
                return;
            }

            if (CaretPosition < Text.Length)
            {
                var length = textViewCaretController.DeleteLength(CaretPosition);
                // 削除するサイズを取得する
                // 元のInputFieldで1文字消されてるので、ここでは1文字減らした文字を削除している
                Text = Text.Remove(CaretPosition, length);
                inputFieldSub.caretPosition = CaretPosition;
            }
        }

        /// <summary>
        /// キーボードイベントのコールバック
        /// </summary>
        /// <param name="evt"></param>
        protected virtual bool KeyPressed(Event evt)
        {
            var currentEventModifiers = evt.modifiers;
            var ctrl = SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX ? (currentEventModifiers & EventModifiers.Command) != 0 : (currentEventModifiers & EventModifiers.Control) != 0;
            var shift = (currentEventModifiers & EventModifiers.Shift) != 0;

            switch (evt.keyCode)
            {
                case KeyCode.Delete:
                {
                    ForwardDelete();
                    return true;
                }

                case KeyCode.Home:
                {
                    MoveToStartOfLine();
                    return true;
                }

                case KeyCode.End:
                {
                    MoveToEndOfLine();
                    return true;
                }

                case KeyCode.LeftArrow:
                {
                    if (textView.LanguageType == TextLanguageType.Arabic)
                    {
                        MoveRight(shift, ctrl);
                    }
                    else
                    {
                        MoveLeft(shift, ctrl);
                    }

                    return true;
                }

                case KeyCode.RightArrow:
                {
                    if (textView.LanguageType == TextLanguageType.Arabic)
                    {
                        MoveLeft(shift, ctrl);
                    }
                    else
                    {
                        MoveRight(shift, ctrl);
                    }

                    return true;
                }

                case KeyCode.UpArrow:
                {
                    MoveUp(shift);
                    return true;
                }

                case KeyCode.DownArrow:
                {
                    MoveDown(shift);
                    return true;
                }
            }

            if (CompositionString.Length > 0)
            {
                UpdateLabel(Text.Substring(0, CaretPosition) + CompositionString + Text.Substring(CaretPosition));
            }

            return false;
        }

        /// <summary>
        /// キャレットの頂点情報を更新します
        /// </summary>
        protected virtual void UpdateGeometry()
        {
            if (!inputFieldSub.shouldHideMobileInput)
            {
                return;
            }

            if (InputFieldCaret == null)
            {
                return;
            }

            OnFillVBO();
            InputFieldCaret.canvasRenderer.SetMesh(Mesh);

            // uGUIInputFieldのキャレット関連の値と同期
            if (CompositionString.Length == 0)
            {
                CaretPosition = inputFieldSub.caretPosition;
                SelectAnchorPosition = inputFieldSub.selectionAnchorPosition;
                SelectFocusPosition = inputFieldSub.selectionFocusPosition;
            }
        }

        /// <summary>
        /// 頂点情報をメッシュに設定します
        /// </summary>
        protected virtual void OnFillVBO()
        {
            using (var helper = new VertexHelper())
            {
                if (!IsFocused)
                {
                    helper.FillMesh(Mesh);
                    return;
                }

                if (!HasSelection)
                {
                    GenerateCaret(helper);
                }
                else
                {
                    GenerateHighlight(helper);
                }

                helper.FillMesh(Mesh);
            }
        }

        /// <summary>
        /// 座標を同期させます
        /// </summary>
        protected virtual void AssignPositioningIfNeeded()
        {
            if (InputFieldCaret == null || TextViewComponent == null)
            {
                return;
            }

            var caretRectTransform = InputFieldCaret.rectTransform;
            var textViewRectTransform = TextViewComponent.rectTransform;
            if (caretRectTransform.localPosition != textViewRectTransform.localPosition ||
                caretRectTransform.localRotation != textViewRectTransform.localRotation ||
                caretRectTransform.localScale != textViewRectTransform.localScale ||
                caretRectTransform.anchorMin != textViewRectTransform.anchorMin ||
                caretRectTransform.anchorMax != textViewRectTransform.anchorMax ||
                caretRectTransform.anchoredPosition != textViewRectTransform.anchoredPosition ||
                caretRectTransform.sizeDelta != textViewRectTransform.sizeDelta ||
                caretRectTransform.pivot != textViewRectTransform.pivot)
            {
                caretRectTransform.localPosition = textViewRectTransform.localPosition;
                caretRectTransform.localRotation = textViewRectTransform.localRotation;
                caretRectTransform.localScale = textViewRectTransform.localScale;
                caretRectTransform.anchorMin = textViewRectTransform.anchorMin;
                caretRectTransform.anchorMax = textViewRectTransform.anchorMax;
                caretRectTransform.anchoredPosition = textViewRectTransform.anchoredPosition;
                caretRectTransform.sizeDelta = textViewRectTransform.sizeDelta;
                caretRectTransform.pivot = textViewRectTransform.pivot;
            }
        }

        /// <summary>
        /// Viewportに合わせてTextViewのRectTransformをあわせます
        /// </summary>
        protected virtual void AdjustRectTransformRelativeToViewport(Vector2 startPosition, float height, bool isCharVisible)
        {
            if (TextViewport == null)
            {
                return;
            }

            var rect = TextViewport.rect;
            var viewportMin = rect.xMin;
            var viewportMax = rect.xMax;

            var rightOffset = viewportMax - (TextViewComponent.rectTransform.anchoredPosition.x + startPosition.x + CaretWidth);
            if (rightOffset < 0f)
            {
                if (!Multiline || Multiline && isCharVisible)
                {
                    TextViewComponent.rectTransform.anchoredPosition += new Vector2(rightOffset, 0);

                    AssignPositioningIfNeeded();
                }
            }

            var leftOffset = TextViewComponent.rectTransform.anchoredPosition.x + startPosition.x - viewportMin;
            if (leftOffset < 0f)
            {
                TextViewComponent.rectTransform.anchoredPosition += new Vector2(-leftOffset, 0);
                AssignPositioningIfNeeded();
            }

            if (LineType != UnityEngine.UI.InputField.LineType.SingleLine)
            {
                var topOffset = TextViewport.rect.yMax - (TextViewComponent.rectTransform.anchoredPosition.y + startPosition.y + height);
                if (topOffset < -0.0001f)
                {
                    TextViewComponent.rectTransform.anchoredPosition += new Vector2(0, topOffset);
                    AssignPositioningIfNeeded();
                }

                var bottomOffset = TextViewComponent.rectTransform.anchoredPosition.y + startPosition.y - TextViewport.rect.yMin - AdjustViewPortBottomOffset;
                if (bottomOffset < 0f)
                {
                    TextViewComponent.rectTransform.anchoredPosition -= new Vector2(0, bottomOffset);
                    AssignPositioningIfNeeded();
                }
            }
        }

        /// <summary>
        /// InputFieldを有効化します
        /// </summary>
        public virtual void ActivateInputField()
        {
            inputFieldSub.ActivateInputField();
        }

        /// <summary>
        /// InputFieldを無効化します
        /// </summary>
        public virtual void DeactivateInputField()
        {
            inputFieldSub.DeactivateInputField();
        }

        /// <summary>
        /// テキストの先頭に移動する
        /// </summary>
        /// <param name="shift">文字選択有りか</param>
        public virtual void MoveTextStart(bool shift)
        {
            inputFieldSub.MoveTextStart(shift);
        }

        /// <summary>
        /// テキストの最後の移動する
        /// </summary>
        /// <param name="shift">文字選択ありか</param>
        public virtual void MoveTextEnd(bool shift)
        {
            inputFieldSub.MoveTextEnd(shift);
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        public virtual void ForceLabelUpdate()
        {
            inputFieldSub.ForceLabelUpdate();
            UpdateLabel(Text);
        }

        /// <summary>
        /// イベントを処理する
        /// </summary>
        /// <param name="e"></param>
        public virtual void ProcessEvent(Event e)
        {
            inputFieldSub.ProcessEvent(e);
        }

        /// <summary>
        /// CharacterValidationの状態に応じて独自バリデーションを設定するかどうか決定する
        /// 独自バリデーションを設定していると、InputFieldで独自に作成されているContentType=Alphanumericなどが有効にならない
        /// </summary>
        protected virtual void CheckCharacterValidation()
        {
            if (CharacterValidation == UnityEngine.UI.InputField.CharacterValidation.None)
            {
                inputFieldSub.onValidateInput = OnValidateInput;
            }
            else
            {
                inputFieldSub.onValidateInput = null;
            }
        }

        /// <summary>
        /// 色状態を変更します。
        /// </summary>
        protected virtual void ChangeColor()
        {
            // 状態と同期
            if (isSyncColor)
            {
                if (Interactive)
                {
                    ResetColor();
                    isEnableColor = true;
                }
                else
                {
                    SetColorMultiply(disableColor);
                    isEnableColor = false;
                }
            }
            // 色は個別
            else
            {
                if (isEnableColor)
                {
                    ResetColor();
                }
                else
                {
                    SetColorMultiply(disableColor);
                }
            }
        }

        /// <summary>
        /// 色を元に戻します。
        /// </summary>
        protected virtual void ResetColor()
        {
            if (!isEnableColorChange)
            {
                return;
            }

            if (graphicGroup != null)
            {
                graphicGroup.ResetColor();
            }
            else
            {
                ImageInfo.ResetColor();
            }
        }

        /// <summary>
        /// 色を乗算設定します。
        /// </summary>
        /// <param name="color">設定する色</param>
        public virtual void SetColorMultiply(Color color)
        {
            if (!isEnableColorChange)
            {
                return;
            }

            if (graphicGroup != null)
            {
                graphicGroup.SetColorMultiply(color);
            }
            else
            {
                ImageInfo.SetColorMultiply(color);
            }
        }

        /// <summary>
        /// TextViewのHorizontalOverflowの設定を複数行か1行かで挙動を変える
        /// </summary>
        protected virtual void EnforceTextViewHorizontalOverflow()
        {
            if (TextViewComponent != null)
            {
                TextViewComponent.HorizontalOverflow = Multiline ? HorizontalWrapMode.Wrap : HorizontalWrapMode.Overflow;
            }
        }

        /// <summary>
        /// 入力文字が変わった時に通知します
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddOnValueChangedEvent(InputFieldEventDelegate.Callback callback)
        {
            OnValueChangedDelegates.Add(new InputFieldEventDelegate(callback));
        }

        /// <summary>
        /// 文字列変更を実行します
        /// </summary>
        /// <param name="text"></param>
        protected virtual void OnValueChanged(string text)
        {
            UpdateLabel(text);

            if (OnValueChangedDelegates.Count > 0)
            {
                OnValueChangedDelegates.Execute(text);
            }
        }

        /// <summary>
        /// 入力が終了した時のイベントを追加します
        /// </summary>
        /// <param name="callback"></param>
        public virtual void AddOnEndEditEvent(InputFieldEventDelegate.Callback callback)
        {
            OnEndEditDelegates.Add(new InputFieldEventDelegate(callback));
        }

        /// <summary>
        /// 編集完了を実行します
        /// </summary>
        protected virtual void OnEndEdit(string text)
        {
            UpdateLabel(text);

            if (OnEndEditDelegates.Count > 0)
            {
                OnEndEditDelegates.Execute(text);
            }
        }

        /// <summary>
        /// 入力をバリデーションするイベントを追加します
        /// </summary>
        public virtual void AddOnValidateInputEvent(InputField.OnValidateInput validateInput)
        {
            this.ValidateInput = validateInput;
        }

        /// <summary>
        /// 入力のバリデーション
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="index">追加文字のインデックス</param>
        /// <param name="addedChar"></param>
        /// <returns></returns>
        protected virtual char OnValidateInput(string text, int index, char addedChar)
        {
            if (ArabicUtils.ShaklTable.IsShakl(addedChar))
            {
                addedChar = '\0';
            }

            if (ValidateInput != null)
            {
                return ValidateInput.Invoke(text, index, addedChar);
            }

            return addedChar;
        }
    }
}
