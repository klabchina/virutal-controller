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
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Jigbox.Tween
{
    /// <summary>
    /// トゥイーンを稼働させるクラスです。シングルトンパターンを採用しています
    /// </summary>
    public class TweenWorker : MonoBehaviour
    {
        static volatile TweenWorker instance;

        /// <summary>
        /// トゥイーンを稼働させるワーカーのインスタンスを取得します
        /// </summary>
        /// <remarks>
        /// シーン内に唯一ワーカーが存在するように振る舞います。
        /// しかし、手動で任意のGameObjectに手動でアタッチした場合、
        /// <c>FincObjectOfType</c>で確認された最初のインスタンスのみが選ばれます。
        /// </remarks>
        /// <value>The instance.</value>
        protected static TweenWorker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(typeof(TweenWorker).Name).AddComponent<TweenWorker>();
                }
                return instance;
            }
        }

#region Properties

        /// <summary>ワーカーが管理するトゥイーン</summary>
        readonly HashSet<IMovement> movements = new HashSet<IMovement>();

        /// <summary>
        /// ワーカーが管理するトゥイーンを返します
        /// </summary>
        /// <value>The tweens.</value>
        public IEnumerable<IMovement> Movements
        {
            get { return movements.Union(movementsToBeAdded).Except(movementsToBeRemoved); }
        }

        /// <summary>
        /// 管理しているトゥイーンの総数です
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return Movements.Count(); }
        }

        /// <summary>
        /// このコールバックを設定した場合、<c>IMovement.Update()</c> 内で発生した <c>MissingReferenceException</c> 以外の例外が、例外の発生した <c>IMovement</c> のインスタンスとともに渡ってきます。この変数が <c>null</c> の場合は、<c>MissingReferenceException</c> のみキャッチされます。
        /// </summary>
        public static Action<IMovement, Exception> TweenUpdateExceptionCallback { protected get; set; }

        /// <summary><see cref="Update"/> で <see cref="movements"/> をイテレートしている最中かどうか</summary>
        static bool iteratingOnUpdate;

        /// <summary><see cref="Update"/> での <see cref="movements"/> のイテレート中の <see cref="Add"/> はこちらに反映されます。</summary>
        readonly HashSet<IMovement> movementsToBeAdded = new HashSet<IMovement>();

        /// <summary><see cref="Update"/> での <see cref="movements"/> のイテレート中の削除はこちらに反映されます。</summary>
        readonly HashSet<IMovement> movementsToBeRemoved = new HashSet<IMovement>();

#endregion

#region Public Method

        /// <summary>
        /// 稼働させるトゥイーンを追加します
        /// </summary>
        /// <param name="movement">Tween.</param>
        public static void Add(IMovement movement)
        {
            if (!iteratingOnUpdate)
            {
                Instance.movements.Add(movement);
                return;
            }

            Instance.movementsToBeAdded.Add(movement);
        }

        /// <summary>
        /// 稼働中のすべてのトゥイーンを初期状態に戻します
        /// </summary>
        public static void RewindAll()
        {
            foreach (var movement in Instance.Movements)
            {
                var tween = movement as ITween;
                if (tween != null)
                {
                    tween.Rewind();
                }
            }
        }

        /// <summary>
        /// 稼働中のすべてのトゥイーンを一時停止させます
        /// </summary>
        public static void PauseAll()
        {
            foreach (var movement in Instance.Movements)
            {
                movement.Pause();
            }
        }

        /// <summary>
        /// 稼働中のすべてのトゥイーンを再開させます
        /// </summary>
        public static void ResumeAll()
        {
            foreach (var movement in Instance.Movements)
            {
                movement.Resume();
            }
        }

        /// <summary>
        /// 稼働中のすべてのトゥイーンを強制終了させます
        /// </summary>
        public static void KillAll()
        {
            foreach (var movement in Instance.Movements)
            {
                movement.Kill();
            }
        }

        /// <summary>
        /// 稼働中のすべてのトゥイーンを強制的に完了させます
        /// </summary>
        public static void CompleteAll()
        {
            foreach (var movement in Instance.Movements)
            {
                movement.Complete();
            }
        }

        /// <summary>
        /// ワーカーを破棄する.
        /// 登録されているTweenも破棄されます.
        /// </summary>
        public static void Destroy()
        {
            if (instance != null)
            {
                UnityEngine.Object.Destroy(instance.gameObject);
                instance.ClearTweens();
                instance = null;
            }
        }

        /// <summary>
        /// ワーカーをシーン遷移時も引き継ぐように設定する.
        /// </summary>
        public static void SetDontDestroy()
        {
            DontDestroyOnLoad(Instance);
        }

#endregion

#region Private Method

        void ClearTweens()
        {
            if (movements != null)
            {
                movements.Clear();
            }
            if (movementsToBeAdded != null)
            {
                movementsToBeAdded.Clear();
            }
            if (movementsToBeRemoved != null)
            {
                movementsToBeRemoved.Clear();
            }
        }

#endregion

#region Unity Method

        protected virtual void Update()
        {
            // GC アロケーション（確保するヒープ）が 0 B になるように実装
            // HashSet<T>.RemoveWhere(Func<T, bool>) はヒープを使うため使用しない（Unity 5.5.4p3）
            iteratingOnUpdate = true;
            foreach (var movement in movements)
            {
                try
                {
                    movement.Update(movement.FollowTimeScale ? Time.deltaTime : Time.unscaledDeltaTime);
                }
                catch (Exception e)
                {
                    if (TweenUpdateExceptionCallback == null)
                    {
                        throw;
                    }

                    TweenUpdateExceptionCallback(movement, e);
                }
            }
            iteratingOnUpdate = false;
            foreach (var tween in movements)
            {
                if (tween.State == TweenState.Done)
                {
                    movementsToBeRemoved.Add(tween);
                }
            }
            // HashSet<T>.ExceptWith(IEnumerable<T>) はヒープを使うため使用しない（Unity 5.5.4p3）
            foreach (var tween in movementsToBeRemoved)
            {
                movements.Remove(tween);
            }
            movementsToBeRemoved.Clear();
            // HashSet<T>.UnionWith(IEnumerable<T>) はヒープを使うため使用しない（Unity 5.5.4p3）
            foreach (var tween in movementsToBeAdded)
            {
                movements.Add(tween);
            }
            movementsToBeAdded.Clear();
        }

        protected virtual void OnDestroy()
        {
            ClearTweens();
            instance = null;
        }

#endregion
    }
}
