using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Jigbox.VirtualCursor
{
    public class BaseCursor : MonoBehaviour
    {
        [Header("px per Second")]
        public float MoveSpeed;

        public int TouchCount { get; protected set; }

        protected Vector3 moveDir;


        private Vector2 screenAspect;
        private RectTransform canvasRoot;
        public Vector3 ScreenPoint
        {
            get
            {
                Vector2 uguiScreenPos = new Vector2(transform.localPosition.x + canvasRoot.sizeDelta.x / 2, transform.localPosition.y + canvasRoot.sizeDelta.y / 2);
                return new Vector3(uguiScreenPos.x * screenAspect.x, uguiScreenPos.y * screenAspect.y, transform.localPosition.z);
            }
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {

        }

        public void SwitchRoot(RectTransform rt)
        {
            canvasRoot = rt;
            screenAspect = new Vector2(Screen.width / canvasRoot.sizeDelta.x, Screen.height / canvasRoot.sizeDelta.y);
        }

        // Update is called once per frame
        void Update()
        {
            CheckCursorMoveDir();
            

            if (!moveDir.Equals(Vector3.zero))
            {
                //transform.localPosition + moveDir * MoveSpeed * Time.deltaTime;
                Vector3 postVector = RangeMoveVector(transform.localPosition, moveDir * MoveSpeed * Time.deltaTime);
                transform.localPosition = postVector;
            }

            if (IsCursorDown())
            {
                TouchCount = 1;
                VirtualCursorMgr.Instance.LastVirtualCursor = this;
                CursorPlugin.GetInstance().BoaderCastDownEvent(ScreenPoint);
            }
            else if (IsCursorUp())
            {
                TouchCount = 0;
                CursorPlugin.GetInstance().BoaderCastUpEvent(ScreenPoint);
            }
            else
            {
                CursorPlugin.GetInstance().BoaderCastMoveEvent(ScreenPoint);
            }

        }

        protected virtual void CheckCursorMoveDir()
        {
            
        }

        protected virtual bool IsCursorDown()
        {
            return false;
        }

        protected virtual bool IsCursorUp()
        {
            return false;
        }


        protected Vector3 RangeMoveVector(Vector3 origin, Vector3 offset)
        {
            Vector3 targetPos = origin + offset;
            if (canvasRoot == null)
            {
                return targetPos;
            }

            Vector2 leftBottom = new Vector2(-canvasRoot.sizeDelta.x / 2, -canvasRoot.sizeDelta.y / 2);
            Vector2 rightTop = new Vector2(canvasRoot.sizeDelta.x / 2, canvasRoot.sizeDelta.y / 2);
            
            if (targetPos.x < leftBottom.x || targetPos.x > rightTop.x)
            {
                offset.x = 0;
            }

            if (targetPos.y < leftBottom.y || targetPos.y > rightTop.y)
            {
                offset.y = 0;
            }

            return origin + offset;
        }
    }
}
