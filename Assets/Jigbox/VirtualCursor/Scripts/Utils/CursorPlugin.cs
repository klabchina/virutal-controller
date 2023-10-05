using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Jigbox.VirtualCursor
{
    public class CursorPlugin
    {

        private static CursorPlugin _instance;

        public static CursorPlugin GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CursorPlugin();
            }

            return _instance;
        }

        private GameObject currentObject;
        private DragInfo dragInfo;

        private GameObject currentOverObject;
        private PointerEventData pointerEventData;

        private GameObject hoverObject;
        public CursorPlugin()
        {
            pointerEventData = new PointerEventData(EventSystem.current);
            dragInfo = new();
        }

        public void BoaderCastDownEvent(Vector3 screenVector)
        {
            dragInfo.Reset();
            pointerEventData.position = screenVector;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            RaycastResult findedClickable = FindFirstRaycast(results);
            
            if (findedClickable.gameObject)
            {
                pointerEventData.pressPosition = screenVector;
                pointerEventData.pointerPressRaycast = findedClickable;
                currentObject = findedClickable.gameObject;
                pointerEventData.pointerPress = currentObject;
                var pointerDownHandler = ExecuteEvents.GetEventHandler<IPointerDownHandler>(currentObject);
                ExecuteEvents.ExecuteHierarchy(pointerDownHandler, pointerEventData, ExecuteEvents.pointerDownHandler);

                var initDragHandler = ExecuteEvents.GetEventHandler<IInitializePotentialDragHandler>(currentObject);
                pointerEventData.pointerDrag = initDragHandler;
                ExecuteEvents.Execute(initDragHandler, pointerEventData, ExecuteEvents.initializePotentialDrag);

            }
            else
            {
                pointerEventData.pressPosition = Vector2.zero;
                pointerEventData.pointerPress = null;
                pointerEventData.pointerDrag = null;
            }
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void BoaderCastUpEvent(Vector3 screenVector)
        {
            if (currentObject)
            {
                pointerEventData.position = screenVector;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, results);
                RaycastResult findedClickable = FindFirstRaycast(results);
                if (findedClickable.gameObject == currentObject)
                {
                    var pointerClickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);
                    ExecuteEvents.Execute(pointerClickHandler, pointerEventData, ExecuteEvents.pointerClickHandler);
                }
                pointerEventData.pointerPress = findedClickable.gameObject;

                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerUpHandler>(currentObject);
                ExecuteEvents.ExecuteHierarchy(pointerUpHandler, pointerEventData, ExecuteEvents.pointerUpHandler);
                //end dragging
                var endDragHandler = ExecuteEvents.GetEventHandler<IEndDragHandler>(currentObject);
                ExecuteEvents.ExecuteHierarchy(endDragHandler, pointerEventData, ExecuteEvents.endDragHandler);

                //drop event
                if (dragInfo.IsCurrentDrag)
                {
                    var dropHandler = FindFirstRaycast<IDropHandler>(results);
                    pointerEventData.pointerPress = currentObject;
                    ExecuteEvents.ExecuteHierarchy(dropHandler, pointerEventData, ExecuteEvents.dropHandler);
                }
                
                currentObject = null;
            }
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void BoaderCastMoveEvent(Vector3 screenVector)
        {
            if (currentObject)
            {
                pointerEventData.delta = new Vector2(screenVector.x - pointerEventData.position.x, screenVector.y - pointerEventData.position.y);
                pointerEventData.position = screenVector;

                if (dragInfo.IsCurrentDrag)
                {
                    var dragHandler = ExecuteEvents.GetEventHandler<IDragHandler>(currentObject);
                    ExecuteEvents.Execute(dragHandler, pointerEventData, ExecuteEvents.dragHandler);
                }
                else if (dragInfo.CheckDrag(pointerEventData.delta))
                {
                    var beginDragHandler = ExecuteEvents.GetEventHandler<IBeginDragHandler>(currentObject);
                    ExecuteEvents.ExecuteHierarchy(beginDragHandler, pointerEventData, ExecuteEvents.beginDragHandler);
                }

                var moveHandler = ExecuteEvents.GetEventHandler<IPointerMoveHandler>(currentObject);
                ExecuteEvents.Execute(moveHandler, pointerEventData, ExecuteEvents.moveHandler);
            }

            //point enter and point exit
            pointerEventData.position = screenVector;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            var enterHandlerObject = FindFirstRaycast<IPointerEnterHandler>(results);
            if (enterHandlerObject && hoverObject != enterHandlerObject)
            {
                if (hoverObject)
                {
                    ExecuteEvents.Execute(hoverObject, pointerEventData, ExecuteEvents.pointerExitHandler);
                }

                ExecuteEvents.Execute(enterHandlerObject, pointerEventData, ExecuteEvents.pointerEnterHandler);
                hoverObject = enterHandlerObject;
            }
            else if (hoverObject && !enterHandlerObject)
            {
                ExecuteEvents.Execute(hoverObject, pointerEventData, ExecuteEvents.pointerExitHandler);
                hoverObject = null;
            }
        }
        
        protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            var candidatesCount = candidates.Count;
            for (var i = 0; i < candidatesCount; ++i)
            {
                if (candidates[i].gameObject == null)
                    continue;

                return candidates[i];
            }
            return new RaycastResult();
        }

        protected static GameObject FindFirstRaycast<T>(List<RaycastResult> candidates) where T : IEventSystemHandler
        {
            var candidatesCount = candidates.Count;
            for (var i = 0; i < candidatesCount; ++i)
            {
                if (candidates[i].gameObject == null)
                    continue;

                var hander = ExecuteEvents.GetEventHandler<T>(candidates[i].gameObject);
                if (hander == null)
                {
                    continue;
                }

                return hander;
            }
            return null;
        }
    }


}
