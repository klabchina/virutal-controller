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

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using Jigbox.VirtualCursor;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;
#endif

[assembly: InternalsVisibleTo("JigboxExampleRuntime")]
namespace Jigbox
{
    public static class InputWrapper
    {
        static bool isInputSystem = false;

        public static bool IsInputSystem
        {
            get
            {
                // 片方だけが有効の場合強制的に状態を変更
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
                isInputSystem = true;
#elif !ENABLE_INPUT_SYSTEM
                isInputSystem = false;
#endif
                return isInputSystem;
            }

            set
            {
                isInputSystem = value;
            }
        }

        /// <summary>
        /// マウスが押されているか取得
        /// </summary>
        /// <param name="index">どのボタンが押されているか</param>
        /// <returns></returns>
        internal static bool GetMouseButton(int index)
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                if (IsTouchSimulate())
                {
                    // TouchSimulationの場合どのクリックもただのタッチになるのでindexは０
                    returnValue = Touchscreen.current.touches[0].press.isPressed;
                }
                else
                {
                    switch (index)
                    {
                        case 0:
                            returnValue = Mouse.current.leftButton.isPressed;
                            break;
                        case 1:
                            returnValue = Mouse.current.rightButton.isPressed;
                            break;
                        case 2:
                            returnValue = Mouse.current.middleButton.isPressed;
                            break;
                        default:
                            break;
                    }
                }
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.GetMouseButton(index);
#endif
            }
            
            return returnValue || VirtualCursorMgr.Instance.TouchCount > 0;
        }

        /// <summary>
        /// マウスが押された瞬間か取得
        /// </summary>
        /// <param name="index">どのボタンが押されているか</param>
        /// <returns></returns>
        internal static bool GetMouseButtonDown(int index)
        {
            var returnValue = false;
            
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                if (IsTouchSimulate())
                {
                    // TouchSimulationの場合どのクリックもただのタッチになるのでindexは０
                    returnValue = Touchscreen.current.touches[0].press.wasPressedThisFrame;
                }
                else
                {
                    switch (index)
                    {
                        case 0:
                            returnValue = Mouse.current.leftButton.wasPressedThisFrame;
                            break;
                        case 1:
                            returnValue = Mouse.current.rightButton.wasPressedThisFrame;
                            break;
                        case 2:
                            returnValue = Mouse.current.middleButton.wasPressedThisFrame;
                            break;
                        default:
                            returnValue = false;
                            break;
                    }
                }
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.GetMouseButtonDown(index);
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// マウスが離れた瞬間か取得
        /// </summary>
        /// <param name="index">どのボタンが離されたか</param>
        /// <returns></returns>
        internal static bool GetMouseButtonUp(int index)
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                if (IsTouchSimulate())
                {
                    // TouchSimulationの場合どのクリックもただのタッチになるのでindexは０
                    returnValue = Touchscreen.current.touches[0].press.wasReleasedThisFrame;
                }
                else
                {
                    switch (index)
                    {
                        case 0:
                            returnValue = Mouse.current.leftButton.wasReleasedThisFrame;
                            break;
                        case 1:
                            returnValue = Mouse.current.rightButton.wasReleasedThisFrame;
                            break;
                        case 2:
                            returnValue = Mouse.current.middleButton.wasReleasedThisFrame;
                            break;
                        default:
                            returnValue = false;
                            break;
                    }
                }
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.GetMouseButtonUp(index);
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// マウスの座標取得
        /// </summary>
        /// <returns></returns>
        internal static Vector2 GetMousePosition()
        {
            var returnValue = Vector2.zero;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                if (IsTouchSimulate())
                {
                    returnValue = GetTouchPosition(0);
                }
                else
                {
                    returnValue = Mouse.current.position.ReadValue();
                }
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.mousePosition;
#endif
            }

            return returnValue;
        }

        /// <summary>
        /// タッチがサポートされているか
        /// </summary>
        /// <returns></returns>
        internal static bool IsTouchSupported()
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = Touchscreen.current != null;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.touchSupported;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// タッチ数取得
        /// </summary>
        /// <returns></returns>
        internal static int GetTouchCount()
        {
            var returnValue = 0;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                EnableEnhancedTouchSupport();
                returnValue = EnhancedTouch.Touch.activeTouches.Count;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.touchCount;
#endif
            }
            
            return returnValue + VirtualCursorMgr.Instance.TouchCount;
        }

        /// <summary>
        /// キーボードが押されているか取得
        /// </summary>
        /// <param name="key">どのキーか</param>
        /// <returns></returns>
        internal static bool GetKeyDown(KeyCode key)
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                switch (key)
                {
                    case KeyCode.Escape:
                        returnValue = Keyboard.current[Key.Escape].wasPressedThisFrame;
                        break;
                    case KeyCode.Return:
                        returnValue = Keyboard.current[Key.Enter].wasPressedThisFrame;
                        break;
                    case KeyCode.Space:
                        returnValue = Keyboard.current[Key.Space].wasPressedThisFrame;
                        break;
                    case KeyCode.LeftControl:
                        returnValue = Keyboard.current[Key.LeftCtrl].wasPressedThisFrame;
                        break;
                    default:
                        Debug.LogWarning("対応するキーがありません");
                        returnValue = false;
                        break;
                }
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.GetKeyDown(key);
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// キーボードが離れた瞬間か取得
        /// </summary>
        /// <param name="key">どのキーか</param>
        /// <returns></returns>
        internal static bool GetKeyUp(KeyCode key)
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                switch (key)
                {
                    case KeyCode.Escape:
                        returnValue = Keyboard.current[Key.Escape].wasReleasedThisFrame;
                        break;
                    case KeyCode.Return:
                        returnValue = Keyboard.current[Key.Enter].wasReleasedThisFrame;
                        break;
                    case KeyCode.Space:
                        returnValue = Keyboard.current[Key.Space].wasReleasedThisFrame;
                        break;
                    case KeyCode.LeftControl:
                        returnValue = Keyboard.current[Key.LeftCtrl].wasReleasedThisFrame;
                        break;
                    case KeyCode.LeftAlt:
                        returnValue = Keyboard.current[Key.LeftAlt].wasReleasedThisFrame;
                        break;
                    default:
                        Debug.LogWarning("対応するキーがありません");
                        returnValue = false;
                        break;
                }
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.GetKeyUp(key);
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// タッチID取得
        /// </summary>
        /// <param name="index">何番目のタッチか</param>
        /// <returns></returns>
        internal static int GetTouchFingerId(int index)
        {
            var returnValue = 0;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                if (IsTouchSupported())
                {
                    returnValue = Touchscreen.current.touches[index].touchId.ReadValue();
                }
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.touches[index].fingerId;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// タッチ座標取得
        /// </summary>
        /// <param name="index">何番目のタッチか</param>
        /// <returns></returns>
        internal static Vector2 GetTouchPosition(int index)
        {
            var returnValue = Vector2.zero;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                EnableEnhancedTouchSupport();
                returnValue = EnhancedTouch.Touch.activeTouches[index].screenPosition;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.touches[index].position;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// タッチの状態取得
        /// </summary>
        /// <param name="index">何番目のタッチか</param>
        /// <param name="phase">どの状態</param>
        /// <returns></returns>
        internal static bool IsTouchPhase(int index, UnityEngine.TouchPhase phase)
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                EnableEnhancedTouchSupport();
                switch (phase)
                {
                    case UnityEngine.TouchPhase.Began:
                        returnValue = EnhancedTouch.Touch.activeTouches[index].phase == UnityEngine.InputSystem.TouchPhase.Began;
                        break;
                    case UnityEngine.TouchPhase.Canceled:
                        returnValue = EnhancedTouch.Touch.activeTouches[index].phase == UnityEngine.InputSystem.TouchPhase.Canceled;
                        break;
                    case UnityEngine.TouchPhase.Stationary:
                        returnValue = EnhancedTouch.Touch.activeTouches[index].phase == UnityEngine.InputSystem.TouchPhase.Stationary;
                        break;
                    case UnityEngine.TouchPhase.Ended:
                        returnValue = EnhancedTouch.Touch.activeTouches[index].phase == UnityEngine.InputSystem.TouchPhase.Ended;
                        break;
                    case UnityEngine.TouchPhase.Moved:
                        returnValue = EnhancedTouch.Touch.activeTouches[index].phase == UnityEngine.InputSystem.TouchPhase.Moved;
                        break;
                    default:
                        Debug.LogWarning("対応するphaseがありません");
                        break;
                }
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.touches[index].phase == phase;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// 加速度取得
        /// </summary>
        /// <returns></returns>
        internal static Vector3 GetAcceleration()
        {
            var returnValue = Vector3.zero;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = Accelerometer.current.acceleration.ReadValue();
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.acceleration;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// 加速度の数取得
        /// </summary>
        /// <returns></returns>
        internal static int GetAccelerationEventCount()
        {
            var returnValue = 0;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = Input.accelerationEvents.Length;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.accelerationEventCount;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// 加速度の取得(インデックス指定)
        /// </summary>
        /// <param name="index">何番目</param>
        /// <returns></returns>
        internal static AccelerationEvent GetAccelerationEvent(int index)
        {
            var returnValue = new AccelerationEvent();
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = Input.accelerationEvents[index];
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = UnityEngine.Input.GetAccelerationEvent(index);
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// キーボードのどれかのキーが入力されているか取得
        /// </summary>
        /// <returns></returns>
        internal static bool GetAnyKey()
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = Keyboard.current.anyKey.isPressed;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.anyKey;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// キーボードのどれかのキーが入力されたフレームか取得
        /// </summary>
        /// <returns></returns>
        internal static bool GetAnyKeyDown()
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = Keyboard.current.wasUpdatedThisFrame;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.anyKeyDown;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// 入力センサーを補正する必要があるか取得
        /// </summary>
        /// <returns></returns>
        internal static bool GetCompensateSensors()
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = InputSystem.settings.compensateForScreenOrientation;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.compensateSensors;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// IMEで使用されるテキストの位置取得
        /// </summary>
        /// <param name="position"></param>
        internal static void SetCompositionCursorPos(Vector2 position)
        {
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                Keyboard.current.SetIMECursorPosition(position);
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                Input.compositionCursorPos = position;
#endif
            }
        }

        /// <summary>
        /// デバイスの傾き取得
        /// </summary>
        /// <returns></returns>
        internal static Quaternion GetGyroAttitude()
        {
            var returnValue = Quaternion.identity;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = AttitudeSensor.current.attitude.ReadValue();
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.gyro.attitude;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// ジャイロセンサーが有効か取得
        /// </summary>
        /// <returns></returns>
        internal static bool GetGyroEnabled()
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = Gyroscope.current.enabled;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.gyro.enabled;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// ジャイロセンサーが有効か設定
        /// </summary>
        /// <param name="enable"></param>
        internal static void SetGyroEnabled(bool enable)
        {
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                if (enable)
                {
                    InputSystem.EnableDevice(Gyroscope.current);
                }
                else
                {
                    InputSystem.DisableDevice(Gyroscope.current);
                }
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                Input.gyro.enabled = enable;
#endif
            }
        }

        /// <summary>
        /// ジャイロの重力加速度取得
        /// </summary>
        /// <returns></returns>
        internal static Vector3 GetGyroGravity()
        {
            var returnValue = Vector3.zero;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = GravitySensor.current.gravity.ReadValue();
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.gyro.gravity;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// ジャイロの回転率取得
        /// </summary>
        /// <returns></returns>
        internal static Vector3 GetGyroRotationRate()
        {
            var returnValue = Vector3.zero;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = Gyroscope.current.angularVelocity.ReadValue();
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.gyro.rotationRate;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// ジャイロの加速度を取得
        /// </summary>
        /// <returns></returns>
        internal static Vector3 GetGyroUserAcceleration()
        {
            var returnValue = Vector3.zero;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = LinearAccelerationSensor.current.acceleration.ReadValue();
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.gyro.userAcceleration;
#endif
            }
            
            return returnValue;
        }

        /// <summary>
        /// マウスが検出されているか取得
        /// </summary>
        /// <returns></returns>
        internal static bool GetMousePresent()
        {
            var returnValue = false;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = Mouse.current != null;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = Input.mousePresent;
#endif
            }
            
            return returnValue;
        }
        
        /// <summary>
        /// タッチID取得
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        /// <returns></returns>
        internal static int GetEventDataTouchId(PointerEventData eventData)
        {
            var returnValue = 0;
            if (IsInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                returnValue = ((ExtendedPointerEventData) eventData).touchId;
#endif
            }
            else
            {
#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
                returnValue = eventData.pointerId;
#endif
            }
            
            return returnValue;
        }

#if ENABLE_INPUT_SYSTEM
        /// <summary>
        /// EnhancedTouchSupportがDisableならEnableに変更する
        /// </summary>
        static void EnableEnhancedTouchSupport()
        {
            if (!EnhancedTouch.EnhancedTouchSupport.enabled)
            {
                EnhancedTouch.EnhancedTouchSupport.Enable();
            }
        }

        internal static bool IsTouchSimulate()
        {
            return EnhancedTouch.TouchSimulation.instance != null && EnhancedTouch.TouchSimulation.instance.enabled;
        }
#endif
    }
}
