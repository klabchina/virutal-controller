using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
