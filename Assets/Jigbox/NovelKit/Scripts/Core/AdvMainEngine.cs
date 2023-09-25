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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jigbox.NovelKit
{
    public class AdvMainEngine : MonoBehaviour
    {
#region properties

        /// <summary>シナリオの再生用コンポーネント</summary>
        [SerializeField]
        protected AdvScenarioPlayer player;

        /// <summary>シナリオの再生用コンポーネント</summary>
        public AdvScenarioPlayer Player { get { return player; } }

        /// <summary>UI制御コンポーネント</summary>
        [SerializeField]
        protected AdvUIManager uiManager;

        /// <summary>UI制御コンポーネント</summary>
        public AdvUIManager UIManager { get { return uiManager; } }

        /// <summary>プレーン管理コンポーネント</summary>
        [SerializeField]
        protected AdvPlaneManager planeManager;

        /// <summary>プレーン管理コンポーネント</summary>
        public AdvPlaneManager PlaneManager { get { return planeManager; } }

        /// <summary>カメラ管理コンポーネント</summary>
        [SerializeField]
        protected AdvCameraManager cameraManager;

        /// <summary>カメラ管理コンポーネント</summary>
        public AdvCameraManager CameraManager { get { return cameraManager; } }

        /// <summary>レイヤー管理コンポーネント</summary>
        [SerializeField]
        protected AdvLayerManager layerManager;

        /// <summary>画面効果管理コンポーネント</summary>
        [SerializeField]
        protected AdvScreenEffectionManager screenEffectionManager;

        /// <summary>画面効果管理コンポーネント</summary>
        public AdvScreenEffectionManager ScreenEffectionManager { get { return screenEffectionManager; } }

        /// <summary>フェード管理コンポーネント</summary>
        [SerializeField]
        protected AdvFadeManager fadeManager;

        /// <summary>フェード管理コンポーネント</summary>
        public AdvFadeManager FadeManager { get { return fadeManager; } }

        /// <summary>シナリオの状態管理コンポーネント</summary>
        [SerializeField]
        protected AdvScenarioStatusManager statusManager;

        /// <summary>シナリオの状態管理コンポーネント</summary>
        public AdvScenarioStatusManager StatusManager { get { return statusManager; } }

        /// <summary>設定データ</summary>
        [SerializeField]
        protected AdvSettingDataManager settings;

        /// <summary>設定データ</summary>
        public AdvSettingDataManager Settings { get { return settings; } }
        
        /// <summary>拡張用管理コンポーネント</summary>
        [SerializeField]
        protected AdvSerializedExtendManagerList extendManagers;

        /// <summary>拡張用管理コンポーネント</summary>
        public AdvSerializedExtendManagerList ExtendManagers { get { return extendManagers; } }

        /// <summary>Tweenの管理クラス</summary>
        public AdvMovementManager MovementManager { get; protected set; }

        /// <summary>レターボックスの制御クラス</summary>
        public AdvLetterBoxManager LetterBoxManager { get; protected set; }

        /// <summary>スクリプトのリソース管理クラス</summary>
        protected AdvScriptResourceManager scriptResourceManager;

        /// <summary>シナリオのオブジェクト管理クラス</summary>
        public AdvObjectManager ObjectManager { get; protected set; }

        /// <summary>マクロ管理クラス</summary>
        public AdvMacroManager MacroManager { get; protected set; }

        /// <summary>定数管理クラス</summary>
        public AdvConstantValueManager ConstantValueManager { get; protected set; }

        /// <summary>変数管理クラス</summary>
        public AdvScriptVariableManager VariableManager { get; protected set; }

        /// <summary>引数に設定可能な変数管理クラス</summary>
        public AdvScriptVariableManager VariableParamManager { get; protected set; }

        /// <summary>サウンド管理クラス</summary>
        public IAdvSoundManager SoundManager { get; protected set; }

        /// <summary>リソースの読み込みインタフェース</summary>
        public IAdvResourceLoader Loader { get; protected set; }

        /// <summary>リソースの事前読み込みインタフェース</summary>
        public IAdvResourcePreloader Preloader { get; protected set; }

        /// <summary>スクリプトファイルの読み込みインタフェース</summary>
        public IAdvScriptLoader ScriptLoader { get; protected set; }

        /// <summary>ローカライザ</summary>
        public IAdvLocalizer Localizer { get; protected set; }

        /// <summary>プリロード用の読み込み状態のハンドラ</summary>
        public AdvPreloadHandler PreloadHandler { get; protected set; }

        /// <summary>デバッグ用の状態管理コンポーネント</summary>
        public AdvDebugStatusManager DebugStatusManager { get; protected set; }

        /// <summary>再生しているかどうか</summary>
        public bool IsPlaying { get; protected set; }

        /// <summary>シナリオの終了ハンドラ</summary>
        protected Action endHandler;

#endregion

#region public methods
        
        /// <summary>
        /// 対象のシナリオラベルからシナリオを再生させます。
        /// </summary>
        /// <param name="labelName">再生するシナリオのラベル名</param>
        /// <returns></returns>
        public bool StartScenario(string labelName)
        {
            if (IsPlaying)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvMainEngine.StartScenario : Already started!");
#endif
                return false;
            }

            Init();

            if (DebugStatusManager != null)
            {
                DebugStatusManager.StartDebug();
            }

            bool result = player.StartScene(labelName);
            IsPlaying = result;
            return result;
        }

        /// <summary>
        /// シナリオの再生が終了した際に呼び出されます。
        /// </summary>
        public void EndScenario()
        {
            if (!IsPlaying)
            {
                return;
            }

            if (DebugStatusManager != null)
            {
                DebugStatusManager.EndDebug();
            }

            IsPlaying = false;
            if (endHandler != null)
            {
                endHandler();
            }
        }
        
        /// <summary>
        /// 読み込まれていたスクリプトやオブジェクトを全て破棄します。
        /// </summary>
        public void ReleaseAll()
        {
            if (IsPlaying)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvMainEngine.ClearAll : Can't call when playing scenario!");
#endif
                return;
            }

            if (MovementManager == null && LetterBoxManager == null)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvMainEngine.ClearAll : Already released!");
#endif
                return;
            }

            player.Uninit();
            uiManager.Uninit();
            cameraManager.Uninit();
            layerManager.Uninit();
            screenEffectionManager.Uninit();
            fadeManager.Uninit();
            statusManager.Uninit();

            MovementManager.EndAll(true);
            MovementManager = null;
            LetterBoxManager.Release();
            LetterBoxManager = null;

            ObjectManager.ReleaseAll();
            MacroManager.Clear();
            ConstantValueManager.Clear();
            VariableManager.Clear();
            VariableParamManager.Clear();
            scriptResourceManager.ClearAll();
        }

        /// <summary>
        /// シナリオを読み込みます。
        /// </summary>
        /// <param name="resource">読み込むスクリプトのパス</param>
        /// <returns>読み込みに成功した場合、最初のラベルを返し、失敗した場合、空文字列を返します。</returns>
        public string LoadScenario(string resource)
        {
            // すでに読み込み済みのスクリプトであれば、再読み込みはしない
            if (scriptResourceManager.ContainsScenes(resource))
            {
                return scriptResourceManager.GetScenes(resource)[0];
            }

            string result = string.Empty;

            string[] stream = ScriptLoader.Load(resource);
            if (stream.Length == 0)
            {
                return result;
            }

            Dictionary<string, List<AdvCommandBase>> commands = AdvCommandParser.CreateCommands(this, stream, resource);

            if (commands.Count == 0)
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("AdvMainEngine.LoadScenario : Not exist enable command!"
                    + "\nScript : " + resource);
#endif
                return result;
            }

            scriptResourceManager.AddScene(resource, commands.Keys.ToList());
            List<string> addedScenes = new List<string>();

            foreach (KeyValuePair<string, List<AdvCommandBase>> scene in commands)
            {
                if (string.IsNullOrEmpty(result))
                {
                    result = scene.Key;
                }
                if (!player.AddScene(scene.Key, scene.Value))
                {
                    result = string.Empty;
                    break;
                }
                else
                {
                    addedScenes.Add(scene.Key);
                }
            }

            // 追加に失敗したシーンがあれば、すでに読み込んだシーンを破棄する
            if (string.IsNullOrEmpty(result))
            {
                foreach (string scene in addedScenes)
                {
                    player.RemoveScene(scene);
                }
                scriptResourceManager.RemoveScenes(resource);
            }

            return result;
        }

        /// <summary>
        /// マクロを読み込みます。
        /// </summary>
        /// <param name="resource">読み込むファイルのパス</param>
        public void LoadMacro(string resource)
        {
            // すでに読み込み済みのマクロファイルであれば、再読み込みはしない
            if (scriptResourceManager.ContainsMacro(resource))
            {
                return;
            }

            string[] stream = ScriptLoader.Load(resource);
            if (stream.Length == 0)
            {
                return;
            }

            Dictionary<string, AdvMacroParser.MacroInfo> macro = 
                AdvMacroParser.CreateMacro(stream, resource, ConstantValueManager);
            MacroManager.Add(macro);
            scriptResourceManager.AddMacro(resource, macro.Keys.ToList());
        }

        /// <summary>
        /// 定数を読み込みます。
        /// </summary>
        /// <param name="resource">読み込むファイルのパス</param>
        public void LoadConstantValues(string resource)
        {
            // すでに読み込み済みの定数ファイルであれば、再読み込みはしない
            if (scriptResourceManager.ContainsConstantValues(resource))
            {
                return;
            }

            string[] stream = ScriptLoader.Load(resource);
            if (stream.Length == 0)
            {
                return;
            }

            Dictionary<string, string[]> values = AdvConstantValueParser.CreateConstantValues(stream, resource);
            ConstantValueManager.Add(values);
            scriptResourceManager.AddConstantValues(resource, values.Keys.ToList());
        }

        /// <summary>
        /// Loaderを設定します。
        /// </summary>
        /// <param name="loader">Loader</param>
        public void SetCustomLoader(IAdvResourceLoader loader)
        {
            Loader = loader;
            uiManager.BacklogManager.CreateItems(
                Loader,
                settings.EngineSetting.BacklogResourcePath,
                settings.EngineSetting.BacklogLength);

            if (ScriptLoader != null)
            {
                ScriptLoader.Loader = Loader;
            }
        }

        /// <summary>
        /// リソースの事前読み込み用Loaderを設定します。
        /// </summary>
        /// <param name="preloader">リソースの事前読み込み用Loader</param>
        public void SetPreloader(IAdvResourcePreloader preloader)
        {
            Preloader = preloader;
            if (statusManager.State == null)
            {
                Init();
            }
            PreloadHandler = new AdvPreloadHandler(statusManager.State);
        }

        /// <summary>
        /// ScriptLoaderを設定します。
        /// </summary>
        /// <param name="scriptLoader"></param>
        public void SetCustomScriptLoader(IAdvScriptLoader scriptLoader)
        {
            ScriptLoader = scriptLoader;
            ScriptLoader.Loader = Loader;
        }

        /// <summary>
        /// ローカライザを設定します。
        /// </summary>
        /// <param name="localizer">ローカライザ</param>
        public void SetCustomLocalizer(IAdvLocalizer localizer)
        {
            Localizer = localizer;
        }

        /// <summary>
        /// サウンド管理クラスを設定します。
        /// </summary>
        /// <param name="soundManager"></param>
        public void SetSoundManager(IAdvSoundManager soundManager)
        {
            SoundManager = soundManager;
        }

        /// <summary>
        /// シナリオの再生が終了した際のイベントハンドラを設定します。
        /// </summary>
        /// <param name="handler">ハンドラ</param>
        public void SetEndHandler(Action handler)
        {
            endHandler = handler;
        }

        /// <summary>
        /// デバッグ情報を表示します。
        /// </summary>
        /// <param name="order">デバッグ情報の表示要求</param>
        /// <param name="parent">デバッグ情報の表示構成の親となるCanvasの参照</param>
        public virtual void ShowDebugStatus(AdvDebugStatusViewOrder order = null, Canvas parent = null)
        {
#if !UNITY_EDITOR && !NOVELKIT_DEBUG
            return;
#endif
            if (order == null)
            {
                order = new AdvDebugStatusViewOrder();
            }

            Transform parentTransform = parent != null ? parent.transform : UIManager.GetComponentInParent<Canvas>().transform;

            if (DebugStatusManager == null)
            {
                DebugStatusManager = gameObject.AddComponent<AdvDebugStatusManager>();
                DebugStatusManager.Init(this);
            }

            DebugStatusManager.CreateView(parentTransform, order);
        }

#endregion

#region protected methods

        /// <summary>
        /// 各種モジュールを初期化します。
        /// </summary>
        protected virtual void Init()
        {
            if (MovementManager != null && LetterBoxManager != null)
            {
                return;
            }

            MovementManager = new AdvMovementManager();
            LetterBoxManager = new AdvLetterBoxManager(this);

            player.Init(this);
            statusManager.Init(this);
            uiManager.Init(this);
            cameraManager.Init(this);
            layerManager.Init(this);
            screenEffectionManager.Init(this);
        }

#endregion

#region override unity methods

        protected virtual void Awake()
        {
            AdvEngineSetting engineSetting = settings.EngineSetting;
            engineSetting.Load();

            if (!engineSetting.UseCustomLoader)
            {
                Loader = new AdvResourceLoader();
            }
            if (SoundManager == null)
            {
                SoundManager = new AdvSoundManager();
            }
            if (!engineSetting.UseCustomLocalizer)
            {
                Localizer = new AdvLocalizer();
            }
            if (ScriptLoader == null)
            {
                ScriptLoader = new AdvScriptLoader();
                ScriptLoader.Loader = Loader;
            }

            ObjectManager = new AdvObjectManager();
            MacroManager = new AdvMacroManager();
            ConstantValueManager = new AdvConstantValueManager();
            VariableManager = new AdvScriptVariableManager();
            VariableParamManager = new AdvScriptVariableManager();
            scriptResourceManager = new AdvScriptResourceManager();

            Init();
        }

        protected virtual void OnDisable()
        {
            // MovementManager管理のTweenを止める
            // Tween対象のGameObjectが破棄されてExceptionが投げられるケースへの対応
            if (MovementManager != null)
            {
                MovementManager.EndAll(true);
            }
        }

#endregion
    }
}
