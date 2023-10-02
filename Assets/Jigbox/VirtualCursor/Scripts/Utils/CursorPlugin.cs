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
                currentObject = findedClickable.gameObject;
                pointerEventData.pointerPress = currentObject;
                var pointerDownHandler = ExecuteEvents.GetEventHandler<IPointerDownHandler>(currentObject);
                ExecuteEvents.ExecuteHierarchy(pointerDownHandler, pointerEventData, ExecuteEvents.pointerDownHandler);

                var initDragHandler = ExecuteEvents.GetEventHandler<IInitializePotentialDragHandler>(currentObject);
                ExecuteEvents.Execute(initDragHandler, pointerEventData, ExecuteEvents.initializePotentialDrag);

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
            else
            {
                //point enter and point exit
                pointerEventData.position = screenVector;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, results);
                RaycastResult findedClickable = FindFirstRaycast(results);
                if (findedClickable.gameObject != currentOverObject)
                {
                    var pointerExitHandler = ExecuteEvents.GetEventHandler<IPointerExitHandler>(currentOverObject);
                    ExecuteEvents.Execute(pointerExitHandler, pointerEventData, ExecuteEvents.pointerExitHandler);
                    currentOverObject = findedClickable.gameObject;
                    if (currentOverObject != null)
                    {
                        var pointerEnterHandler = ExecuteEvents.GetEventHandler<IPointerEnterHandler>(currentOverObject);
                        ExecuteEvents.Execute(pointerEnterHandler, pointerEventData, ExecuteEvents.pointerEnterHandler);
                    }
                }
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
    }


}
