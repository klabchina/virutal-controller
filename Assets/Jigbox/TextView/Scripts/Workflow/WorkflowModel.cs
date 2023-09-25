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
using System.Collections.Generic;

namespace Jigbox.TextView
{
    /// <summary>
    /// ワークフロー用プロセスの定義(中身の値はビットをシフトする数)
    /// </summary>
    public enum WorkflowProcess
    {
        /// <summary>
        /// 初期化用コールバック処理を行うフラグ
        /// </summary>
        Initialize = 16,

        /// <summary>
        /// テキストのパースを行うフラグ
        /// </summary>
        TextParse = 15,

        /// <summary>
        /// 論理行の生成を行うフラグ
        /// </summary>
        LogicalLinesCreate = 14,

        /// <summary>
        /// 要求するグリフの一覧の生成を行うフラグ
        /// </summary>
        RequestGlyphSpecsCreate = 13,

        /// <summary>
        /// グリフの生成を行うフラグ
        /// </summary>
        CreateGlyph = 12,

        /// <summary>
        /// シュリンクを行うフラグ
        /// </summary>
        Squeeze = 11,

        /// <summary>
        /// シュリンクを行うフラグ
        /// </summary>
        Shrink = 10,

        /// <summary>
        /// グリフの配置および改行処理を行うフラグ
        /// </summary>
        GlyphPlacement = 9,
        
        /// <summary>
        /// Ellipsisによる省略文字の追加
        /// </summary>
        EllipsisCalculate = 8,

        /// <summary>
        /// LinesOffsetYCalculate、TextLinesPositionXCalculate、DisplayableElementIndexCalculateのプロセスのみ並列可能なため、ここが全部パスしたかどうかのフラグを定義しておく
        /// </summary>
        Process4AllPassed = 7,

        /// <summary>
        /// 行毎のY軸位置を計算するフラグ
        /// </summary>
        LinesOffsetYCalculate = 6,

        /// <summary>
        /// Alignmentによるテキスト全体に対するX軸のオフセット位置を計算するフラグ
        /// </summary>
        TextLinesPositionXCalculate = 5,

        /// <summary>
        /// HorizontalLayoutの描画するElementのStartIndexとEndIndexを計算するフラグ
        /// </summary>
        DisplayableElementIndexCalculate = 4,

        /// <summary>
        /// VerticalAlignment(pivot)によるテキスト全体に対するY軸のオフセット位置を計算するフラグ
        /// </summary>
        CurrentOffsetYCalculate = 3,

        /// <summary>
        /// インライン画像配置を行うフラグ
        /// </summary>
        InlineImagePlacement = 2,

        /// <summary>
        /// 頂点座標更新を行うフラグ
        /// </summary>
        FontTextureUVUpdate = 1,

        /// <summary>
        /// レンダリングの更新を行うフラグ
        /// </summary>
        Rendering = 0,
    }

    /// <summary>
    /// TextViewのワークフロー用各種フラグを管理するモデルです
    /// </summary>
    [Serializable]
    public sealed class WorkflowModel
    {
        #region Fields

        /// <summary>
        /// 現在のプロセスの状態を保持している変数
        /// </summary>
        /// <remarks>
        /// この変数は各プロセスのビットフラグとして機能します。
        /// フラグが立っているプロセスは特定タイミングで処理されます。
        /// (実際の処理タイミングはTextViewObserverのLateUpdate、TextViewのプロパティを変更した時、テクスチャのリビルドを検知した時などになります)
        /// フラグが立っていないプロセスは無視されます。
        /// フラグ更新用メソッドとして、RequireProcessメソッド(フラグを立てる)、CompleteProcessメソッド(フラグを下ろす)が実装されています。
        /// フラグを立てた時は、特定の条件を除き、該当のフラグよりも下位のもの全てが影響を受けます(フラグが立つようになっています)。
        /// フラグを下ろした場合は該当のフラグのみに影響します。
        /// </remarks>
        int flags;

#endregion

#region Inner Classes

        /// <summary>
        /// TextViewのワークフローを定義、管理するクラス
        /// </summary>
        class WorkflowProcessInfo
        {
            /// <summary>
            /// 上位のプロセスを定義したデータモデルを返します
            /// </summary>
            /// <returns></returns>
            public WorkflowProcessInfo HigherProcess { get; protected set; }

            /// <summary>
            /// 下位のプロセスを定義したデータモデルを返します
            /// </summary>
            /// <returns></returns>
            public WorkflowProcessInfo LowerProcess { get; protected set; }

            /// <summary>
            /// 自身のプロセスのマスク値を返します
            /// </summary>
            /// <returns></returns>
            public int SelfMask { get; protected set; }

            /// <summary>
            /// 自身よりひとつ上位のプロセスのマスク値を返します
            /// </summary>
            public virtual int HigherProcessMask { get { return this.HigherProcess != null ? this.HigherProcess.SelfMask : 0; } }

            /// <summary>
            /// 自身よりひとつ下位のプロセスのマスク値を返します
            /// </summary>
            public virtual int LowerProcessMask { get { return this.LowerProcess != null ? this.LowerProcess.SelfMask : 0; } }

            public WorkflowProcessInfo(WorkflowProcess process)
            {
                // マスクに変換
                this.SelfMask = 1 << (int) process;
            }

            /// <summary>
            /// 初期化処理
            /// </summary>
            /// <param name="higher"></param>
            /// <param name="lower"></param>
            public void Initialize(WorkflowProcessInfo higher, WorkflowProcessInfo lower)
            {
                this.HigherProcess = higher;
                this.LowerProcess = lower;
            }

            /// <summary>
            /// このプロセスが実行可能かを返します
            /// </summary>
            /// <param name="flags"></param>
            /// <returns></returns>
            public bool IsExecutable(int flags)
            {
                // 前のプロセスのフラグが立っておらず、自身のプロセスのフラグが立っている場合は実行可能と判断する
                return (flags & this.HigherProcessMask) == 0 && (flags & this.SelfMask) > 0;
            }
        }

        /// <summary>
        /// TextViewのワークフローを定義、管理するクラス(Higherが複数ある場合)
        /// </summary>
        class WorkflowProcessJoinInfo : WorkflowProcessInfo
        {
            /// <summary>
            /// 前のプロセスのマスク
            /// </summary>
            protected int higherMask;

            /// <summary>
            /// 前の複数プロセスを統合したマスクを返します
            /// </summary>
            public override int HigherProcessMask { get { return this.higherMask; } }

            public WorkflowProcessJoinInfo(WorkflowProcess process) : base(process)
            {
            }

            /// <summary>
            /// 初期化処理
            /// </summary>
            /// <param name="higher"></param>
            /// <param name="lower"></param>
            /// <param name="higherProcesses"></param>
            public void Initialize(WorkflowProcessInfo higher, WorkflowProcessInfo lower, params WorkflowProcess[] higherProcesses)
            {
                this.HigherProcess = higher;
                this.LowerProcess = lower;
                // マスクを統合する
                foreach (var subProcess in higherProcesses)
                {
                    higherMask |= 1 << (int) subProcess;
                }
            }
        }

        /// <summary>
        /// TextViewのワークフローを定義、管理するクラス(Lowerが複数ある場合)
        /// </summary>
        class WorkflowProcessSplitInfo : WorkflowProcessInfo
        {
            /// <summary>
            /// 次のマスク
            /// </summary>
            protected int lowerMask;

            /// <summary>
            /// 下位の複数プロセスを統合したマスクを返します(LowerProcessInfoに設定しているLowerMaskの値も反映させるため、base.LowerMaskも含める)
            /// </summary>
            public override int LowerProcessMask { get { return this.lowerMask | base.LowerProcessMask; } }

            public WorkflowProcessSplitInfo(WorkflowProcess process) : base(process)
            {
            }

            /// <summary>
            /// 初期化処理
            /// </summary>
            /// <param name="higher"></param>
            /// <param name="lower"></param>
            /// <param name="lowerProcesses"></param>
            public void Initialize(WorkflowProcessInfo higher, WorkflowProcessInfo lower, params WorkflowProcess[] lowerProcesses)
            {
                this.HigherProcess = higher;
                this.LowerProcess = lower;
                // マスクを統合する
                foreach (var subProcess in lowerProcesses)
                {
                    lowerMask |= 1 << (int) subProcess;
                }
            }
        }

        /// <summary>
        /// TextViewのワークフロー順定義用データのリスト
        /// </summary>
        static List<WorkflowProcessInfo> TextViewWorkflowDataList;

        static WorkflowModel()
        {
            TextViewWorkflowDataList = new List<WorkflowProcessInfo>();

            // モデルの生成
            var initializeData = new WorkflowProcessInfo(WorkflowProcess.Initialize);
            var textParseData = new WorkflowProcessInfo(WorkflowProcess.TextParse);
            var logicalLinesCreateData = new WorkflowProcessInfo(WorkflowProcess.LogicalLinesCreate);
            var requestGlyphSpecsCreateData = new WorkflowProcessInfo(WorkflowProcess.RequestGlyphSpecsCreate);
            var createGlyphData = new WorkflowProcessInfo(WorkflowProcess.CreateGlyph);
            var squeezeData = new WorkflowProcessInfo(WorkflowProcess.Squeeze);
            var shrinkData = new WorkflowProcessInfo(WorkflowProcess.Shrink);
            var glyphPlacementData = new WorkflowProcessInfo(WorkflowProcess.GlyphPlacement);
            var ellipsisCalculateData = new WorkflowProcessInfo(WorkflowProcess.EllipsisCalculate);
            var process4AllPassedData = new WorkflowProcessSplitInfo(WorkflowProcess.Process4AllPassed);
            var linesOffsetYCalculateData = new WorkflowProcessInfo(WorkflowProcess.LinesOffsetYCalculate);
            var textLinesPositionXCalculateData = new WorkflowProcessInfo(WorkflowProcess.TextLinesPositionXCalculate);
            var displayableElementIndexCalculateData = new WorkflowProcessInfo(WorkflowProcess.DisplayableElementIndexCalculate);
            var currentOffsetYCalculateData = new WorkflowProcessInfo(WorkflowProcess.CurrentOffsetYCalculate);
            var inlineImagePlacementData = new WorkflowProcessJoinInfo(WorkflowProcess.InlineImagePlacement);
            var fontTextureUVUpdateData = new WorkflowProcessInfo(WorkflowProcess.FontTextureUVUpdate);
            var renderingData = new WorkflowProcessInfo(WorkflowProcess.Rendering);

            // Higher、Lowerへのモデル設定
            initializeData.Initialize(null, textParseData);
            textParseData.Initialize(initializeData, logicalLinesCreateData);
            logicalLinesCreateData.Initialize(textParseData, requestGlyphSpecsCreateData);
            requestGlyphSpecsCreateData.Initialize(logicalLinesCreateData, createGlyphData);
            createGlyphData.Initialize(requestGlyphSpecsCreateData, squeezeData);
            squeezeData.Initialize(createGlyphData, shrinkData);
            shrinkData.Initialize(squeezeData, glyphPlacementData);
            glyphPlacementData.Initialize(shrinkData, process4AllPassedData);
            ellipsisCalculateData.Initialize(glyphPlacementData, process4AllPassedData);
            process4AllPassedData.Initialize(glyphPlacementData, inlineImagePlacementData, WorkflowProcess.LinesOffsetYCalculate, WorkflowProcess.TextLinesPositionXCalculate, WorkflowProcess.DisplayableElementIndexCalculate, WorkflowProcess.CurrentOffsetYCalculate);
            linesOffsetYCalculateData.Initialize(glyphPlacementData, inlineImagePlacementData);
            textLinesPositionXCalculateData.Initialize(glyphPlacementData, inlineImagePlacementData);
            displayableElementIndexCalculateData.Initialize(glyphPlacementData, inlineImagePlacementData);
            currentOffsetYCalculateData.Initialize(glyphPlacementData, inlineImagePlacementData);
            inlineImagePlacementData.Initialize(process4AllPassedData, fontTextureUVUpdateData, WorkflowProcess.LinesOffsetYCalculate, WorkflowProcess.TextLinesPositionXCalculate, WorkflowProcess.DisplayableElementIndexCalculate, WorkflowProcess.CurrentOffsetYCalculate);
            fontTextureUVUpdateData.Initialize(glyphPlacementData, renderingData);
            renderingData.Initialize(fontTextureUVUpdateData, null);

            // ワークフロー順を定義するデータリストを生成
            TextViewWorkflowDataList.Add(renderingData);                            // WorkflowProcess.Rendering = 0
            TextViewWorkflowDataList.Add(fontTextureUVUpdateData);                  // WorkflowProcess.FontTextureUVUpdate = 1
            TextViewWorkflowDataList.Add(inlineImagePlacementData);                 // WorkflowProcess.InlineImagePlacement = 2
            TextViewWorkflowDataList.Add(currentOffsetYCalculateData);              // WorkflowProcess.CurrentOffsetYCalculate = 3
            TextViewWorkflowDataList.Add(displayableElementIndexCalculateData);     // WorkflowProcess.DisplayableElementIndexCalculate = 4
            TextViewWorkflowDataList.Add(textLinesPositionXCalculateData);          // WorkflowProcess.TextLinesPositionXCalculate = 5
            TextViewWorkflowDataList.Add(linesOffsetYCalculateData);                // WorkflowProcess.LinesOffsetYCalculate = 6
            TextViewWorkflowDataList.Add(process4AllPassedData);                    // WorkflowProcess.Process4AllPassed = 7
            TextViewWorkflowDataList.Add(ellipsisCalculateData);                    // WorkflowProcess.EllipsisCalculate = 8
            TextViewWorkflowDataList.Add(glyphPlacementData);                       // WorkflowProcess.GlyphPlacement = 9
            TextViewWorkflowDataList.Add(shrinkData);                               // WorkflowProcess.Shrink = 10
            TextViewWorkflowDataList.Add(squeezeData);                              // WorkflowProcess.Squeeze = 11
            TextViewWorkflowDataList.Add(createGlyphData);                          // WorkflowProcess.CreateGlyph = 12
            TextViewWorkflowDataList.Add(requestGlyphSpecsCreateData);              // WorkflowProcess.RequestGlyphSpecsCreate = 13
            TextViewWorkflowDataList.Add(logicalLinesCreateData);                   // WorkflowProcess.LogicalLinesCreate = 14
            TextViewWorkflowDataList.Add(textParseData);                            // WorkflowProcess.TextParse = 15
            TextViewWorkflowDataList.Add(initializeData);                           // WorkflowProcess.Initialize = 16
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WorkflowModel()
        {
            // 初期化処理の最上位プロセスのみフラグを立てる
            InternalRequireProcess(WorkflowProcess.Initialize, false);
        }

#endregion

#region public methods

        /// <summary>
        /// 指定されたプロセスのフラグが立っているか(プロセスを処理させる必要があるか)を返します
        /// </summary>
        /// <remarks>
        /// CanExecuteProcessと違い、自身のフラグが立っているかどうかだけを判断基準としています
        /// </remarks>
        /// <param name="process">取得したいプロセス</param>
        /// <returns></returns>
        public bool IsNecessaryProcess(WorkflowProcess process)
        {
#if UNITY_EDITOR
            if (process == WorkflowProcess.Process4AllPassed)
            {
                throw new ArgumentException("このプロセスは直接参照できません");
            }
#endif
            return (TextViewWorkflowDataList[(int) process].SelfMask & flags) > 0;
        }

        /// <summary>
        /// 指定されたプロセスが、現在実行可能かどうかを返します
        /// </summary>
        /// <remarks>
        /// IsNecessaryProcessと違い、設定されている上位のプロセスのフラグが立っておらず、自身のプロセスのフラグが立っている場合を実行可能と判断しています
        /// (自身の処理は上位のプロセスが完了しているのを前提条件とした実装となっているため)
        /// </remarks>
        /// <param name="process">取得したいプロセス</param>
        /// <returns></returns>
        public bool CanExecuteProcess(WorkflowProcess process)
        {
#if UNITY_EDITOR
            if (process == WorkflowProcess.Process4AllPassed)
            {
                throw new ArgumentException("このプロセスは直接参照できません");
            }
#endif
            return TextViewWorkflowDataList[(int) process].IsExecutable(this.flags);
        }

        /// <summary>
        /// プロセスを要求します(フラグを有効化する)
        /// </summary>
        /// <param name="process">有効にしたいプロセス</param>
        /// <param name="requireLowerProcesses">下位のプロセスも有効化するかどうか</param>
        public void RequireProcess(WorkflowProcess process, bool requireLowerProcesses = true)
        {
#if UNITY_EDITOR
            if (process == WorkflowProcess.Initialize)
            {
                throw new ArgumentException("このプロセスは直接変更できません");
            }
#endif

            InternalRequireProcess(process, requireLowerProcesses);
        }

        /// <summary>
        /// プロセスを完了させます
        /// </summary>
        /// <param name="process">完了させたいプロセス</param>
        public void ProcessExecuted(WorkflowProcess process)
        {
#if UNITY_EDITOR
            if (process == WorkflowProcess.Process4AllPassed)
            {
                throw new ArgumentException("このプロセスは直接変更できません");
            }
#endif
            WorkflowProcessInfo info = TextViewWorkflowDataList[(int) process];

            // フラグを折る
            this.flags &= ~info.SelfMask;
        }

#endregion

#region private methods

        /// <summary>
        /// WorkflowModel内でプロセスを要求する場合に使用されます。
        /// </summary>
        /// <param name="process">有効にしたいプロセス</param>
        /// <param name="requireLowerProcesses">下位のプロセスも有効化するかどうか</param>
        void InternalRequireProcess(WorkflowProcess process, bool requireLowerProcesses = true)
        {
#if UNITY_EDITOR
            if (process == WorkflowProcess.Process4AllPassed)
            {
                throw new ArgumentException("このプロセスは直接変更できません");
            }
#endif

            // 一時保存用
            WorkflowProcessInfo info = TextViewWorkflowDataList[(int) process];

            // 当該のプロセスのフラグを立てる
            this.flags |= info.SelfMask;

            if (!requireLowerProcesses)
            {
                return;
            }

            // 当該プロセスより下位のプロセスに対して全てのフラグを立てる
            while (info != null)
            {
                this.flags |= info.LowerProcessMask;
                info = info.LowerProcess;
            }
        }

#endregion
    }
}
