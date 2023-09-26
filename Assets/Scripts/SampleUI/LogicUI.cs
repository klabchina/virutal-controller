using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jigbox;
using Jigbox.VirtualCursor;

public class LogicUI : MonoBehaviour
{
    public VirtualInputSystem usedInputSystem;

    // Start is called before the first frame update
    void Start()
    {
        VirtualCursorMgr.Instance.Launch(usedInputSystem);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBtnClick(int buttonId)
    {
        Debug.LogWarning($"onclick ${buttonId}");
    }

    public void OnScrollChanged(Vector2 vec)
    {
        Debug.LogWarning($"onscroll changed ${vec}");
    }
}
