using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jigbox;
using Jigbox.VirtualCursor;

public class JigBoxStarter : MonoBehaviour
{
    public VirtualInputSystem usedInputSystem;

    // Start is called before the first frame update
    void Start()
    {
        VirtualCursorMgr.Instance.Launch(usedInputSystem);
    }
}
