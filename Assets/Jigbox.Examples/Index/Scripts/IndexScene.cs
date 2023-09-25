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
using System.Reflection;
using Jigbox.Components;
using UnityEngine.UI;
#if UNITY_2019_1_OR_NEWER && ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER && UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

namespace Jigbox.Examples
{
    public class IndexScene : ExampleSceneBase
    {
#region properties

        [SerializeField]
        ListViewVertical listView = null;

        SceneListData list;

        static int lastSelectIndex = -1;

        // NovelKitExample側では使わないのでワーニング消し
#pragma warning disable 414
        static FieldInfo sceneManagerInstanceField = null;
#pragma warning restore 414

        /// <summary>
        /// キャッチされない例外やエラーログが発生したときにダイアログを表示するかどうか
        /// </summary>
        [SerializeField]
        bool popupOnError = true;

#endregion

#region private methods

        [AuthorizedAccess]
        void OnValueChanged(bool value)
        {
            InputWrapper.IsInputSystem = value;
        }
        
        [AuthorizedAccess]
        void OnUpdateCell(GameObject cellObject, int index)
        {
            SceneListItemController controller = cellObject.GetComponent<SceneListItemController>();
            controller.SetText(list.Scenes[index], index, index == lastSelectIndex);
            controller.OnClickHandler = i => lastSelectIndex = i;
        }

        void ClearSceneManagerInstance()
        {
            // NovelKitExample側では、SceneManager使わないのでパッケージを入れなくていいように
            // コード上から参照が行われないようにする
#if !JIGBOX_NOVELKIT_EXAMPLE
            // SceneManger.Instance、StackableSceneManager.Instanceは、フィールドを共有しているが、
            // 両立させることは基本的にないため、一度作った後、明示的にインスタンスを消す機能はない
            // ただし、Example上では両方確認できるようにしているため、両立している状態にあり、
            // IndexSceneに帰ってきた時点でインスタンスを消すようにする
            if (sceneManagerInstanceField == null)
            {
                System.Type type = typeof(Jigbox.SceneTransition.SceneManager);
                sceneManagerInstanceField = type.GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
            }
            sceneManagerInstanceField.SetValue(null, null);
#endif
        }

        static void HandleException(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                    MessagePopup.Open(string.Format("{0}\n{1}", condition, stackTrace));
                    break;
            }
        }
        
#endregion

#region override unity methods

        void Start()
        {
            list = Resources.Load<SceneListData>("SceneListData");

            listView.VirtualCellCount = list.Scenes.Count;
            listView.FillCells(Resources.Load<GameObject>("SceneListItem"));

            if (lastSelectIndex >= 0)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += JumpByIndex;
#else
                JumpByIndex();
#endif
            }

            ClearSceneManagerInstance();

            Restarter.EnsureActivation();
            
#if UNITY_2019_1_OR_NEWER && ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            GameObject.Find("Message").transform.Find("InputSystemBase").gameObject.SetActive(true);
            GameObject.Find("InputSystemToggle").GetComponent<GameObjectToggle>().IsOn = InputWrapper.IsInputSystem;
#if UNITY_EDITOR
            if (InputWrapper.IsTouchSimulate())
            {
                GameObject.Find("Message").transform.Find("InputSystemBase").transform.Find("touchSimulate").gameObject.SetActive(true);
            }
            else
            {
                // 起動直後はTouchSimulateとれないのでonDeviceChangeでTouchSimulateが追加されてから反映させる
                InputSystem.onDeviceChange += TouchSimulateActive;
            }
#endif
#endif
        }

        protected override void Awake()
        {
            base.Awake();
            if (this.popupOnError)
            {
                // キャッチされない例外が発生すると HandleException が呼ばれる
                Application.logMessageReceived += HandleException;
            }
        }

#endregion

        void JumpByIndex()
        {
            listView.JumpByIndex(lastSelectIndex);
        }

#if UNITY_2019_1_OR_NEWER && ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER && UNITY_EDITOR
        void TouchSimulateActive(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added)
            {
                if (device == InputSystem.GetDevice("Simulated Touchscreen"))
                {
                    GameObject.Find("Message").transform.Find("InputSystemBase").transform.Find("touchSimulate").gameObject.SetActive(InputWrapper.IsTouchSimulate());
                }
            }
        }
#endif
    }
}
