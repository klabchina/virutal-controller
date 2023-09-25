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
using System.Collections.Generic;
using System.Collections;
using Jigbox.TextView;
#if UNITY_EDITOR
#if UNITY_2021_1_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif
#endif

namespace Jigbox.Components
{
    /// <summary>
    /// TextViewの処理を統括するためのコンポーネント
    /// </summary>
    [ExecuteAlways]
    public sealed class TextViewObserver : MonoBehaviour
    {
#region inner classes, enum, and structs

        /// <summary>
        /// Unity上でシリアライズ可能なFont毎のTextViewのリスト
        /// </summary>
        [System.Serializable]
        class SerializableTextViewList
        {
            /// <summary>フォント</summary>
            [SerializeField]
            Font font;

            /// <summary>フォント</summary>
            public Font Font { get { return font; } }

            /// <summary>
            /// TextViewの参照
            /// このリストを更新するときはtextViewHashSetも更新するようにしてください
            /// </summary>
            [SerializeField]
            List<TextView> textViewList = new List<TextView>();

            /// <summary>
            /// TextViewの所持判定用参照
            /// ListのContainsでの判定処理に時間がかかるためHashSetで判定を行い高速化を図る
            /// textViewListを更新する前にこのHashSetをみるようにしてください
            /// </summary>
            HashSet<TextView> textViewHashSet = new HashSet<TextView>();

            /// <summary>
            /// TextViewの数を返します。
            /// </summary>
            public int Count { get { return textViewList.Count; } }

            /// <summary>
            /// TextViewを返すIndexer
            /// </summary>
            /// <param name="i"></param>
            public TextView this[int i] { get { return textViewList[i]; } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="font">フォント</param>
            public SerializableTextViewList(Font font)
            {
                this.font = font;
            }

            /// <summary>
            /// TextViewの参照を追加します。
            /// </summary>
            /// <param name="textView">TextView</param>
            public void Add(TextView textView)
            {
#if UNITY_EDITOR
                // Asset Importの後などにtextViewHashSetがNullReferenceになる問題の対応
                // 本来ならTextViewObserverを削除させることで対応させたいが、どうしても削除できないケースがある
                // SerializableTextViewListはSerializeだが、textViewHashSetがSerializeできないためNullReferenceになる
                // そのため、EditorモードのときはtextViewHashSetを使わずにtextViewListだけを使うようにしている
                if (!Application.isPlaying)
                {
                    if (!textViewList.Contains(textView))
                    {
                        textViewList.Add(textView);
                    }
                    return;
                }
#endif
                if (textViewHashSet.Add(textView))
                {
                    textViewList.Add(textView);
                }
            }

            /// <summary>
            /// TextViewの参照を破棄します。
            /// </summary>
            /// <param name="textView">TextView</param>
            public void Remove(TextView textView)
            {
#if UNITY_EDITOR
                // Asset Importの後などにtextViewHashSetがNullReferenceになる問題の対応
                // 本来ならTextViewObserverを削除させることで対応させたいが、どうしても削除できないケースがある
                // SerializableTextViewListはSerializeだが、textViewHashSetがSerializeできないためNullReferenceになる
                // そのため、EditorモードのときはtextViewHashSetを使わずにtextViewListだけを使うようにしている
                if (!Application.isPlaying)
                {
                    if (textViewList.Contains(textView))
                    {
                        textViewList.Remove(textView);
                    }
                    return;
                }
#endif
                if (textViewHashSet.Remove(textView))
                {
                    textViewList.Remove(textView);
                }
            }
            
#if UNITY_EDITOR
            /// <summary>
            /// 管理外で削除されたTextViewの登録を解除します
            /// </summary>
            public void RefreshForPrefab()
            {
                List<TextView> registerTextViewList = new List<TextView>();
                foreach (var textView in textViewList)
                {
                    if (textView)
                    {
                        registerTextViewList.Add(textView);
                    }
                }
                
                textViewList = registerTextViewList;
            }
#endif
        }

#endregion

#region properties

        /// <summary>インスタンス</summary>
        static TextViewObserver instance;

        /// <summary>インスタンス</summary>
        public static TextViewObserver Instance
        {
            get
            {
                // static変数はコンパイル時などにUnityEditor的に保持されないため、
                // コンパイルや実行状態を変化させる度にnullとなる
                if (instance == null)
                {
#if UNITY_EDITOR
                    // 実行状態が変更される際に、OnEnable、OnDisableが複数回呼び出されるが
                    // その際にインスタンスを作っても最後の呼び出し以外で作ったインスタンスは
                    // 正しく参照を保持できない状態となってしまうため、再生状態を切り替える際は呼ばないようにすること
                    if (EditorPlayModeChanging)
                    {
                        UnityEngine.Assertions.Assert.IsTrue(false, "Can't call when changing play mode!");
                        return null;
                    }
#endif

                    GameObject gameObject = new GameObject("TextViewObserver");
#if JIGBOX_DEBUG
                    gameObject.hideFlags = HideFlags.DontSave;
#else
                    gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
#endif

                    instance = gameObject.AddComponent<TextViewObserver>();
                }
                return instance;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// <para>UnityEditor上で再生状態が変化している最中かどうか</para>
        /// <para>
        /// Application.isPlayingとEditorApplication.isPlayingOrWillChangePlaymodeは値の変更タイミングが異なるため、
        /// これを利用して再生状態が変化しているかどうかを判断する。
        /// </para>
        /// </summary>
        public static bool EditorPlayModeChanging { get { return Application.isPlaying != UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode; } }

#endif

        /// <summary>
        /// <para>TextViewの一覧</para>
        /// <para>Inspector上で確認するためにシリアライズしているだけで、実際にシーンには保存されない</para>
        /// </summary>
        [SerializeField]
        List<SerializableTextViewList> allTextViews = new List<SerializableTextViewList>();

        /// <summary>フォントテクスチャのリビルドが行われたかどうか</summary>
        bool hasFontRebuilt = false;

        /// <summary>自身の更新処理中かどうか</summary>
        bool isOwnUpdating = false;

        /// <summary>ライフサイクル上のレンダリングではないレンダリングが発生したかどうか</summary>
        bool hasUnexpectedRender = false;

        /// <summary>このフレームにおけるバッチ処理が既に終わっているかどうか</summary>
        public bool AlreadyBatched { get; private set; }

        /// <summary>フレームの終端監視用コルーチン</summary>
        Coroutine frameEndObserver = null;

        /// <summary>
        /// frameEndObserverで使用される返り値のキャッシュ
        /// OnDisableの際にもキャッシュが破棄されないようにフィールドとして宣言する
        /// </summary>
        readonly WaitForEndOfFrame cachedWaitForEndOfFrame = new WaitForEndOfFrame();

#endregion

#region public methods

        /// <summary>
        /// TextViewの参照を記録します。
        /// </summary>
        /// <param name="textView">TextView</param>
        public static void Register(TextView textView)
        {
            if (textView.Font == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!EditorPlayModeChanging)
#endif
            {
                SerializableTextViewList textViews = Instance.GetTextViewList(textView.Font);
                textViews.Add(textView);
            }
        }

        /// <summary>
        /// 記録されているTextViewの参照を解除します。
        /// </summary>
        /// <param name="textView">TextView</param>
        public static void Unregister(TextView textView)
        {
            if (textView.Font == null)
            {
                return;
            }
            // TextViewObserver自体がDestroyされた後に呼び出された場合は
            // インスタンスが再生成されるとマズいので何もしない
            if (instance == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!EditorPlayModeChanging)
#endif
            {
                SerializableTextViewList textViews = instance.GetTextViewList(textView.Font);
                textViews.Remove(textView);
            }
        }

#endregion

#region private methods

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static TextViewObserver()
        {
            Font.textureRebuilt += OnFontRebuilt;
            Canvas.willRenderCanvases += WillRenderCanvases;
#if UNITY_EDITOR
            PrefabStage.prefabSaved += OnPrefabSaved;
            UnityEditor.PrefabUtility.prefabInstanceUpdated += OnPrefabSaved;
#endif
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// プレハブモードで保存した時の処理
        /// シーンにプレハブがある状態でプレハブモードで設定を変更すると
        /// 各プロパティのSetでやっている再描画の処理が呼ばれずに設定が変更されてしまうので
        /// プレハブモードを保存したタイミングで全てのTextViewを更新するようにする
        /// </summary>
        /// <param name="gameObject">Prefab元のObject</param>
        static void OnPrefabSaved(GameObject gameObject)
        {
            TextViewObserver observer = instance;
            if (observer == null)
            {
                return;
            }

            var textview = gameObject.transform.GetComponentInChildren<TextView>();

            // プレハブにTextViewが含まれていたら再更新させる
            if (textview)
            {
                List<TextView> reRegisterTextViewList = new List<TextView>();
                
                foreach (SerializableTextViewList textViews in observer.allTextViews)
                {
                    textViews.RefreshForPrefab();
                    
                    for (int i = 0; i < textViews.Count; i++)
                    {
                        TextView textView = textViews[i];
                        
                        // ListのFontと違うFontが設定されているTextViewを探す
                        if (textView.Font != textViews.Font)
                        {
                            reRegisterTextViewList.Add(textview);
                        }
                    }
                }

                // 再登録
                foreach (var textView in reRegisterTextViewList)
                {
                    foreach (SerializableTextViewList textViews in observer.allTextViews)
                    {
                        textViews.Remove(textView);
                    }

                    Register(textView);
                }

                // 登録されているTexViewを更新
                foreach (SerializableTextViewList textViews in observer.allTextViews)
                {
                    for (int i = 0; i < textViews.Count; i++)
                    {
                        textViews[i].PrefabSavedInit();
                    }
                }
            }
        }
#endif
        /// <summary>
        /// ダイナミックフォントのリビルドが発生した際に呼び出されます。
        /// </summary>
        /// <param name="font">リビルドが発生したフォント</param>
        static void OnFontRebuilt(Font font)
        {
            TextViewObserver observer = instance;
            if (observer == null)
            {
                return;
            }

            // TextViewで使っていないフォントのリビルドは無視
            if (!observer.IsUseFont(font))
            {
                return;
            }

            GlyphCatalog catalog = GlyphCatalog.GetCatalog(font);
            catalog.Clear();

            SerializableTextViewList textViews = observer.GetTextViewList(font);

            // OnPopulateMeshがすでに始まっているタイミングでリビルドが発生した場合と
            // 前回のフレーム終了〜現在のフレームのTextViewObserver.LateUpdate終了前に発生した場合で
            // 処理すべき内容が違うので処理を分ける
            if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
            {
                if (!observer.CreateGlyphOnFontRebuilt(textViews))
                {
                    return;
                }

                // フォントのリビルドが発生しないでグリフが生成しきれる保証はないので
                // 再生成と再描画は分けて呼び出す
                for (int i = 0; i < textViews.Count; i++)
                {
                    TextView textView = textViews[i];
                    if (textView.isActiveAndEnabled)
                    {
                        textView.Repopulate();
                    }
                }
            }
            else if (!observer.AlreadyBatched)
            {
                // 自身の更新中の場合、実際の処理はUpdate内で行う
                if (observer.isOwnUpdating)
                {
                    observer.hasFontRebuilt = true;
                }
                // OnPopulateMeshでも自身の更新中でもない場合に飛んで来る場合は、
                // Jigbox外の案件コンポーネントがリビルドを発生させているということなので、
                // その場合は再計算の要求を行う
                else
                {
                    observer.RequireCreateGlyph(textViews);
                }
            }
            // TextViewObserver.LateUpdate以降、OnPopulateMesh以前のタイミングで
            // フォントのリビルドが発生した場合
            else
            {
                if (!observer.CreateGlyphOnFontRebuilt(textViews))
                {
                    return;
                }

                for (int i = 0; i < textViews.Count; i++)
                {
                    TextView textView = textViews[i];
                    if (textView.isActiveAndEnabled)
                    {
                        textView.UpdateLayoutedGlyph();
                    }
                }
            }
        }

        /// <summary>
        /// Canvasのレンダリングが行われる前に呼び出されます。
        /// </summary>
        static void WillRenderCanvases()
        {
            TextViewObserver observer = instance;
            if (observer == null)
            {
                return;
            }

            // バッチ処理が終わっている状態でCanvasの更新が走る場合、
            // バッチ処理されたTextViewは問題なく処理できるし、
            // バッチ処理後に更新された場合は、更新によってDirty状態になり、
            // その後ライフサイクル内で正常に処理される
#if UNITY_EDITOR
            // 非実行状態の場合は、AlreadyBatchedは常にfalse
            // また、例外処理しなくてもレンダリングに影響はないはずなので
            // 例外処理自体を行わない
            if (observer.AlreadyBatched || !Application.isPlaying)
#else
            if (observer.AlreadyBatched)
#endif
            {
                foreach (SerializableTextViewList textViews in observer.allTextViews)
                {
                    for (int i = 0; i < textViews.Count; i++)
                    {
                        TextView textView = textViews[i];
                        textView.WillUpdateVertices = false;
                    }
                }
            }
            // Canvas.ForceUpdateCanvasesによって、バッチ処理前に
            // レンダリングが発生する場合はフラグを立てておき、
            // LateUpdate内で例外処理を行う
            else
            {
                observer.hasUnexpectedRender = true;
            }
        }

        /// <summary>
        /// フォント毎のTextViewの一覧を取得します。
        /// </summary>
        /// <param name="font">フォント</param>
        /// <returns></returns>
        SerializableTextViewList GetTextViewList(Font font)
        {
            SerializableTextViewList list = null;

            foreach (SerializableTextViewList textViews in allTextViews)
            {
                if (textViews.Font == font)
                {
                    list = textViews;
                }
            }

            if (list == null)
            {
                list = new SerializableTextViewList(font);
                allTextViews.Add(list);
            }

            return list;
        }

        /// <summary>
        /// 対象のFontを利用しているTextViewが存在するかを返します。
        /// </summary>
        /// <param name="font">フォント</param>
        /// <returns></returns>
        bool IsUseFont(Font font)
        {
            foreach (SerializableTextViewList textViews in allTextViews)
            {
                if (textViews.Font == font)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 特定のフォントを利用しているTextViewに対して、グリフの再生成を行うように要求します。
        /// </summary>
        /// <param name="textViews">TextViewのリスト</param>
        void RequireCreateGlyph(SerializableTextViewList textViews)
        {
            // リビルドが発生した場合は、TextViewの状態に合わせて再計算を要求する
            for (int i = 0; i < textViews.Count; i++)
            {
                TextView textView = textViews[i];
                // すでにレイアウトの計算まで終わっている場合は、
                // グリフ情報を最新にして適用するだけで正しくレンダリングできる
                // そうでない場合は、普通に計算する
                textView.RequireProcess(WorkflowProcess.CreateGlyph, !textView.AlreadyCalculatedLayout);
            }
        }

        /// <summary>
        /// フォントのリビルドが発生した際に、TextViewにGlyphを生成させます。
        /// </summary>
        /// <param name="textViews">TextViewのリスト</param>
        /// <returns>Glyphの生成中にフォントのリビルドが発生した場合、<c>false</c>を返します。</returns>
        bool CreateGlyphOnFontRebuilt(SerializableTextViewList textViews)
        {
            // TextViewの状態に合わせて再計算を要求する
            for (int i = 0; i < textViews.Count; i++)
            {
                TextView textView = textViews[i];
                // TextView自体が有効な状態でない場合は、表示しない文字まで
                // 余計にリクエストしてフォントテクスチャを圧迫しないように処理をスキップ
                if (!textView.isActiveAndEnabled)
                {
                    // 有効になった後のLateUpdate中の処理で正しいグリフ情報を設定できるように
                    // グリフの生成を要求する
                    textView.RequireProcess(WorkflowProcess.CreateGlyph, !textView.AlreadyCalculatedLayout);
                    continue;
                }

                // ここでさらにリビルドが起きた場合は
                // コールバックが多重で呼び出されている状態になるので
                // リビルド発生時点で処理を中断して、あとから呼び出された方に
                // 更新を任せるようにする
                if (!textView.CreateGlyphs())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 記録されている全てのTextViewに対して、再更新を行うように要求します。
        /// </summary>
        void DirtyAllTextView()
        {
            foreach (SerializableTextViewList textViews in allTextViews)
            {
                for (int i = 0; i < textViews.Count; i++)
                {
                    TextView textView = textViews[i];
                    textView.RequireProcess(WorkflowProcess.TextParse);
                }
            }
        }

        /// <summary>
        /// 意図しないタイミングでレンダリングされてしまったTextViewをDirty状態に設定します。
        /// </summary>
        void DirtyHasRenderedUnexpectedly()
        {
            foreach (SerializableTextViewList textViews in allTextViews)
            {
                for (int i = 0; i < textViews.Count; i++)
                {
                    TextView textView = textViews[i];
                    if (textView.WillUpdateVertices)
                    {
                        textView.SetVerticesDirty();
                    }
                }
            }
        }

        /// <summary>
        /// フレームの終わりを監視し、フラグを折ります。
        /// </summary>
        /// <returns></returns>
        IEnumerator FrameEndObservation()
        {
            while (true)
            {
                yield return cachedWaitForEndOfFrame;
                AlreadyBatched = false;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// コンパイル中や実行状態の変更時に呼び出された際には、プロパティの状態がUnity的に保持されないため
        /// 結果としてMissingReferenceExceptionが発生してしまうので、一旦インスタンスを破棄して作り直す
        /// </summary>
        /// <returns>Destroyされたかどうか</returns>
        bool DestroyTextViewObserverIfNeeded()
        {
            if (UnityEditor.EditorApplication.isCompiling ||
                Application.isPlaying != UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }

                if (UnityEditor.EditorApplication.isCompiling)
                {
                    DirtyAllTextView();
                }
                return true;
            }
            return false;
        }
#endif

#endregion

#region override unity methods

        void OnEnable()
        {
#if UNITY_EDITOR
            // 実行状態が変更されるタイミングで存在しているインスタンスは、
            // シングルトンとして見た際に意図通りの挙動をしないので、破棄する
            if (EditorPlayModeChanging)
            {
                DestroyImmediate(gameObject);
                return;
            }

            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
            }
#endif

            // 実行時にはシーン遷移してもインスタンスが破棄されないようにする
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
                // 実行していない間は必要ないはずなのでコルーチンは起動しない
                if (frameEndObserver == null)
                {
                    frameEndObserver = StartCoroutine(FrameEndObservation());
                }
            }
        }

#if UNITY_EDITOR
        void OnApplicationQuit()
        {
            // 再生 -> 一時停止 -> 一時停止を解かずに停止
            // 上記の手順を踏んだ場合LateUpdateの処理が呼ばれずTextViewObsererが削除されない
            // そのためMissingReferenceExceptionが発生してしまう問題を解決するためのコード
            DestroyTextViewObserverIfNeeded();
        }
#endif

        void LateUpdate()
        {
#if UNITY_EDITOR
            DestroyTextViewObserverIfNeeded();
#endif
            isOwnUpdating = true;

            if (hasUnexpectedRender)
            {
                DirtyHasRenderedUnexpectedly();
                hasUnexpectedRender = false;
            }

            // 全てのTextViewにParseTextの処理を行わせる
            // ParseTextで行われるonInitializeの処理によりallTextViewsにAddが行われる可能性があるためforを使う
            // allTextViewsに対してRemoveが行われないことを前提としている
            for (int allCount = 0; allCount < allTextViews.Count; allCount++)
            {
                SerializableTextViewList textViews = allTextViews[allCount];

                // TextViewのonInitializeのタイミングでFontの設定が行われると
                // SerializableTextViewListの中身が変更されParseTextが行われないTextViewがでるため処理をループさせている

                // SerializableTextViewListの中身が変更されているかを検知するためのフラグ
                bool hasChangedTextViews;
                do
                {
                    hasChangedTextViews = false;
                    // ParseTextの実行前のTextViewの数を保持する
                    int count = textViews.Count;

                    for (int textCount = 0; textCount < textViews.Count; textCount++)
                    {
                        TextView textView = textViews[textCount];
                        // TextView自体が有効な状態でない場合は、そもそも表示されないので
                        // 計算や文字のリクエスト自体が無駄になるので、処理をスキップする
                        if (textView.isActiveAndEnabled)
                        {
                            textView.ParseText();
                        }
                    }

                    // ParseText実行前の数と合わない場合はSerializableTextViewListの中身が更新されている
                    if (count != textViews.Count)
                    {
                        // SerializableTextViewListが更新されている場合はParseTextの処理がされていないTextViewがいる可能性があるため再度処理を行う
                        hasChangedTextViews = true;
                    }
                } while (hasChangedTextViews);
            }

            foreach (SerializableTextViewList textViews in allTextViews)
            {
                // Glyphの生成は、フォントテクスチャのリビルドが発生しなくなるまで繰り返す
                do
                {
                    hasFontRebuilt = false;

                    for (int i = 0; i < textViews.Count; i++)
                    {
                        TextView textView = textViews[i];
                        if (!textView.isActiveAndEnabled)
                        {
                            continue;
                        }

                        // シュリンクが行われた場合は再実行するのでループで回す
                        // フォントのリビルドが発生した場合は、グリフを再生成する必要があるので
                        // ループをさせずに再計算を待つ
                        while (textView.RequestFontAndCreateGlyph())
                        {
                        }
                        // リビルドが発生した場合はその時点で中断
                        if (hasFontRebuilt)
                        {
                            break;
                        }
                    }

                    // リビルドが発生していれば、再処理するように設定
                    if (hasFontRebuilt)
                    {
                        RequireCreateGlyph(textViews);
                    }
                } while (hasFontRebuilt);

                for (int i = 0; i < textViews.Count; i++)
                {
                    TextView textView = textViews[i];
                    if (textView.isActiveAndEnabled)
                    {
                        textView.CalculateLayout();
                    }
                }
            }

            isOwnUpdating = false;
            // コルーチンを起動していない状態では、下手にフラグを立てたりすると
            // 正常系の処理に影響がでるので、余計なことをしないようにする
            if (frameEndObserver != null)
            {
                AlreadyBatched = true;
            }
        }

        void OnDisable()
        {
            if (frameEndObserver != null)
            {
                StopCoroutine(frameEndObserver);
                frameEndObserver = null;
            }
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

#endregion
    }
}
