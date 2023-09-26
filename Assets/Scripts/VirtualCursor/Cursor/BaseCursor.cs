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
        protected Vector3 moveDir;


        private Vector2 screenAspect;
        private RectTransform canvasRoot;
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            Canvas item = GameObject.Find("Canvas")?.GetComponent<Canvas>();
            canvasRoot = GameObject.Find("Canvas")?.GetComponent<RectTransform>();

            screenAspect = new Vector2(Screen.width / canvasRoot.sizeDelta.x, Screen.height / canvasRoot.sizeDelta.y);
            
        }

        // Update is called once per frame
        void Update()
        {
            CheckCursorMoveDir();
            

            if (!moveDir.Equals(Vector3.zero))
            {
                Vector3 postVector = transform.localPosition + moveDir * MoveSpeed * Time.deltaTime;
                if (CanMove(postVector))
                {

                    transform.localPosition = postVector;
                }
            }

            if (IsCursorDown())
            {
                CursorPlugin.GetInstance().BoaderCastDownEvent(ScreenPoint);
            }
            else if (IsCursorUp())
            {
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

        protected Vector3 ScreenPoint
        {
            get
            {
                Vector2 uguiScreenPos = new Vector2(transform.localPosition.x + canvasRoot.sizeDelta.x / 2, transform.localPosition.y + canvasRoot.sizeDelta.y / 2);
                return new Vector3(uguiScreenPos.x * screenAspect.x, uguiScreenPos.y * screenAspect.y, transform.localPosition.z);
            }
        }


        protected bool CanMove(Vector3 pos)
        {
            if (canvasRoot == null)
            {
                return true;
            }

            Vector2 leftBottom = new Vector2(-canvasRoot.sizeDelta.x / 2, -canvasRoot.sizeDelta.y / 2);
            Vector2 rightTop = new Vector2(canvasRoot.sizeDelta.x / 2, canvasRoot.sizeDelta.y / 2);
            if (pos.x < leftBottom.x || pos.x > rightTop.x)
            {
                return false;
            }

            if (pos.y < leftBottom.y || pos.y > rightTop.y)
            {
                return false;
            }

            return true;
        }
    }
}
