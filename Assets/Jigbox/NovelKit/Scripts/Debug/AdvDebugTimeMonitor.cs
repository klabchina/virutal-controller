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

namespace Jigbox.NovelKit
{
    public class AdvDebugTimeMonitor : IAdvDebugStatusMonitor
    {
#region constants

        /// <summary>タイムコード表示用のタグ</summary>
        protected static readonly string TimecodeTagName = "DEBUG_TIMECODE";

        /// <summary>タイムコードを表示する際の先頭文字列</summary>
        protected static readonly string TimecodeHeaderString = "Time";

        /// <summary>タイムコード表示のフォーマット</summary>
        protected static readonly string TimecodeFormat = " : {0:D2}:{1:D2}:{2:D2}";

        /// <summary>ラップタイム表示用のタグ</summary>
        protected static readonly string LapTagName = "DEBUG_LAP_";

        /// <summary>ラップタイムを表示する際の先頭文字列</summary>
        protected static readonly string LapHeaderString = "Lap";

#endregion

#region properties

        /// <summary>デバッグ情報のハンドラ</summary>
        protected IAdvDebugStatusHandler handler;

        /// <summary>タイムコードの計測開始時間</summary>
        protected float startTime = 0.0f;

        /// <summary>前回のラップタイムを記録した時間</summary>
        protected float lastLapTime = 0.0f;

        /// <summary>ラップタイムの記録回数</summary>
        protected int lapCount = 0;

        /// <summary>タイムコードを表示するかどうか</summary>
        protected bool isShowTimecode = true;

        /// <summary>現在の時間</summary>
        public float CurrentTime { get { return Time.realtimeSinceStartup - startTime; } }

        /// <summary>現在のラップタイム</summary>
        public float CurrentLap { get { return Time.realtimeSinceStartup - lastLapTime; } }

#endregion

#region public methods

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="handler">デバッグ情報のハンドラ</param>
        public virtual void Init(IAdvDebugStatusHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// タイムコードを関しを開始します。
        /// </summary>
        public virtual void Start()
        {
            startTime = Time.realtimeSinceStartup;
            lastLapTime = startTime;
            lapCount = 1;
        }

        /// <summary>
        /// 状態を更新します。
        /// </summary>
        public virtual void Update()
        {
            // 表示する値は、ズレないように更新時点の値をキャッシュしたものを使う
            float time = Time.realtimeSinceStartup;

            if (isShowTimecode)
            {
                NotifyDebugStatus(TimecodeTagName, TimecodeHeaderString, time - startTime);
            }

            if (lapCount > 1)
            {
                NotifyDebugStatus(LapTagName + lapCount, LapHeaderString + lapCount, time - lastLapTime);
            }
        }

        /// <summary>
        /// タイムコードの表示を変更します。
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowTimecode(bool isShow)
        {
            isShowTimecode = isShow;
        }

        /// <summary>
        /// ラップタイムを記録します。
        /// </summary>
        public void RecodeLap()
        {
            float time = Time.realtimeSinceStartup;
            NotifyDebugStatus(LapTagName + lapCount, LapHeaderString + lapCount, time - lastLapTime);
            ++lapCount;
            lastLapTime = time;
        }

#endregion

#region protected methods

        /// <summary>
        /// デバッグ状態を通知します。
        /// </summary>
        /// <param name="tagName">タグ名</param>
        /// <param name="header">デバッグで表示する情報の先頭文字列</param>
        /// <param name="time">時間</param>
        protected void NotifyDebugStatus(string tagName, string header, float time)
        {
            int minute = Mathf.RoundToInt(time / 60.0f);
            int second = Mathf.RoundToInt(time % 60.0f);
            int millisecond = Mathf.FloorToInt(time % 1.0f * 100.0f);
            handler.Set(tagName, string.Format(header + TimecodeFormat, minute, second, millisecond));
        }

#endregion
    }
}
