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

using System.Collections.Generic;
using Jigbox.Tween;

namespace Jigbox.NovelKit
{
    public class AdvMovementManager
    {
#region properties

        /// <summary>実行されている動作</summary>
        protected List<IAdvMovement> movements = new List<IAdvMovement>();

        /// <summary>継続する動作</summary>
        protected Dictionary<string, IAdvMovement> keepMovements = new Dictionary<string, IAdvMovement>();

        /// <summary>継続する動作を記録するための名前</summary>
        protected string keepMovementName = null;

        /// <summary>継続する動作のループ設定</summary>
        protected LoopMode keepMovementLoopMode = LoopMode.NoLoop;

#endregion

#region public methods

        /// <summary>
        /// 実行するTweenを登録します。
        /// </summary>
        /// <param name="tween">Tween</param>
        public void Register(ITween tween)
        {
            AdvTweenMovement movement = new AdvTweenMovement(tween);
            if (string.IsNullOrEmpty(keepMovementName))
            {
                movements.Add(movement);
            }
            else
            {
                keepMovements.Add(keepMovementName, movement);
                tween.LoopMode = keepMovementLoopMode;
                keepMovementName = null;
                keepMovementLoopMode = LoopMode.NoLoop;
            }
        }

        /// <summary>
        /// 動作を登録します。
        /// </summary>
        /// <param name="movement">動作</param>
        public void Register(IAdvMovement movement)
        {
            if (string.IsNullOrEmpty(keepMovementName))
            {
                movements.Add(movement);
            }
            else
            {
                keepMovements.Add(keepMovementName, movement);
                if (movement is IAdvLoopableMovement)
                {
                    IAdvLoopableMovement loopable = movement as IAdvLoopableMovement;
                    loopable.SetLoopMode(keepMovementLoopMode);
                }

                keepMovementName = null;
                keepMovementLoopMode = LoopMode.NoLoop;
            }
        }

        /// <summary>
        /// 継続する動作を記録するための名前を設定します。
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="loopMode">ループ設定</param>
        public void SetKeepMovementName(string name, LoopMode loopMode = LoopMode.NoLoop)
        {
            if (keepMovements.ContainsKey(name))
            {
#if UNITY_EDITOR || NOVELKIT_DEBUG
                AdvLog.Error("同じ名前で記録されている動作があります。"
                    + "\n先に同名で記録されている動作を終了させて下さい。"
                    + "\n動作名 : " + name);
#endif
                return;
            }
            keepMovementName = name;
            keepMovementLoopMode = loopMode;
        }

        /// <summary>
        /// 継続している動作を終了させます。
        /// </summary>
        /// <param name="name">記録されている名前</param>
        public void EndKeepMovement(string name)
        {
            if (keepMovements.ContainsKey(name))
            {
                keepMovements[name].Complete();
                keepMovements.Remove(name);
            }
        }

        /// <summary>
        /// 登録されているTweenを全て終了させます。
        /// </summary>
        /// <param name="isAll">継続する動作も含めて全て終了させるかどうか</param>
        public void EndAll(bool isAll = false)
        {
            if (movements.Count > 0)
            {
                foreach (IAdvMovement movement in movements)
                {
                    if (movement == null)
                    {
                        continue;
                    }
                    movement.Complete();
                }

                movements.Clear();
            }

            if (isAll && keepMovements.Count > 0)
            {
                foreach (IAdvMovement movement in keepMovements.Values)
                {
                    if (movement == null)
                    {
                        continue;
                    }
                    movement.Complete();
                }

                keepMovements.Clear();
            }
        }

#endregion
    }
}
