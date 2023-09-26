using UnityEngine;
using UnityEngine.InputSystem;


namespace Jigbox.VirtualCursor
{
    /// <summary>
    /// new inputSystem support
    /// </summary>
    public class NewInputCursor : BaseCursor
    {
        private CursorPluginAssets m_ActionAssets;


        protected override void Start()
        {
            base.Start();
            m_ActionAssets = new CursorPluginAssets();
            m_ActionAssets.Enable();
            m_ActionAssets.VirtualCursor.Enable();
            m_ActionAssets.VirtualCursor.MainKey.started += OnMainKey;
            m_ActionAssets.VirtualCursor.MainKey.canceled += OnMainKey;

            m_ActionAssets.VirtualCursor.CrossDir.performed += OnCrossKey;
            m_ActionAssets.VirtualCursor.CrossDir.canceled += OnCrossCancel;
        }

        void OnMainKey(InputAction.CallbackContext context)
        {
            if (context.phase.Equals(InputActionPhase.Started))
            {
                CursorPlugin.GetInstance().BoaderCastDownEvent(ScreenPoint);
            }
            else if (context.phase.Equals(InputActionPhase.Canceled))
            {
                CursorPlugin.GetInstance().BoaderCastUpEvent(ScreenPoint);
            }
        }

        void OnCrossKey(InputAction.CallbackContext context)
        {   
            moveDir = context.ReadValue<Vector2>();
        }

        void OnCrossCancel(InputAction.CallbackContext context)
        {
            moveDir = Vector2.zero;
        }

        void OnDestroy()
        {
            m_ActionAssets.Disable();
            m_ActionAssets.Dispose();
            m_ActionAssets = null;
        }
    }
}