using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleRotObject : MonoBehaviour, IDragHandler
{
    public Transform threeObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        threeObject.Rotate(0, eventData.delta.x, 0);
    }
}
