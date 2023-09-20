using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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
    private PointerEventData pointerEventData;
    public CursorPlugin()
    {
        pointerEventData = new PointerEventData(EventSystem.current);
    }

    public void BoaderCastDownEvent(Vector3 screenVector)
    {
        pointerEventData.position = screenVector;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        RaycastResult findedClickable = FindFirstRaycast(results);
        
        if (findedClickable.gameObject)
        {
            Debug.LogWarning(currentObject);
            currentObject = findedClickable.gameObject;
            var pointerDownHandler = ExecuteEvents.GetEventHandler<IPointerDownHandler>(currentObject);
            ExecuteEvents.ExecuteHierarchy(pointerDownHandler, pointerEventData, ExecuteEvents.pointerDownHandler);
        }
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
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerUpHandler>(currentObject);
            ExecuteEvents.Execute(pointerUpHandler, pointerEventData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(currentObject, pointerEventData, ExecuteEvents.pointerUpHandler);
            currentObject = null;

            EventSystem.current.SetSelectedGameObject(null);
        }
        
    }

    public void BoaderCastMoveEvent(Vector3 screenVector)
    {
        if (currentObject)
        {
            // pointerEventData.position = screenVector;
            // List<RaycastResult> results = new List<RaycastResult>();
            // EventSystem.current.RaycastAll(pointerEventData, results);
            // RaycastResult findedClickable = FindFirstRaycast(results);
            
            // if (findedClickable.gameObject)
            // {
            //     ExecuteEvents.Execute(findedClickable.gameObject, pointerEventData, ExecuteEvents.pointerClickHandler);
            // }
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
