using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// 虚拟摇杆操作点
/// </summary>
public class Cursor : MonoBehaviour
{
    [Header("px per Second")]
    public float MoveSpeed;

    public KeyCode downPress = KeyCode.J;

    private Vector2 screenAspect;
    private Vector3 moveDir;

    private RectTransform canvasRoot;
    
    // Start is called before the first frame update
    void Start()
    {
        Canvas item = GameObject.Find("Canvas")?.GetComponent<Canvas>();
        canvasRoot = GameObject.Find("Canvas")?.GetComponent<RectTransform>();

        screenAspect = new Vector2(Screen.width / canvasRoot.sizeDelta.x, Screen.height / canvasRoot.sizeDelta.y);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDir.x = Input.GetKey(KeyCode.LeftArrow) ? -1 : 1;
        }
        else
        {
            moveDir.x = 0;
        }


        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            moveDir.y = Input.GetKey(KeyCode.UpArrow) ? 1 : -1;
        }
        else
        {
            moveDir.y = 0;
        }

        if (!moveDir.Equals(Vector3.zero))
        {
            Vector3 postVector = transform.localPosition + moveDir * MoveSpeed * Time.deltaTime;
            if (CanMove(postVector))
            {

                transform.localPosition = postVector;
            }
        }


        //Simple Click
        if (Input.GetKeyDown(downPress))
        {
            CursorPlugin.GetInstance().BoaderCastDownEvent(ScreenPoint);
        }
        else if (Input.GetKeyUp(downPress))
        {
            CursorPlugin.GetInstance().BoaderCastUpEvent(ScreenPoint);
        }
        else
        {
            CursorPlugin.GetInstance().BoaderCastMoveEvent(ScreenPoint);
        }

    }

    Vector3 ScreenPoint
    {
        get
        {
            Vector2 uguiScreenPos = new Vector2(transform.localPosition.x + canvasRoot.sizeDelta.x / 2, transform.localPosition.y + canvasRoot.sizeDelta.y / 2);
            return new Vector3(uguiScreenPos.x * screenAspect.x, uguiScreenPos.y * screenAspect.y, transform.localPosition.z);
        }
    }


    bool CanMove(Vector3 pos)
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
