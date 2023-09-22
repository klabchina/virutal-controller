using UnityEngine;

/// <summary>
/// new inputSystem support
/// </summary>
public class NewInputCursor : BaseCursor
{
    [Header("MainKey")]
    public KeyCode downPress = KeyCode.J;

    [Header("LeftKey")]
    public KeyCode leftKey = KeyCode.LeftArrow;

    [Header("RightKey")]
    public KeyCode rightkey = KeyCode.RightArrow;

    [Header("UpKey")]
    public KeyCode upKey = KeyCode.UpArrow;
    
    [Header("DownKey")]
    public KeyCode downKey = KeyCode.DownArrow;
    protected override void CheckCursorMoveDir()
    {
        if (Input.GetKey(leftKey) || Input.GetKey(rightkey))
        {
            moveDir.x = Input.GetKey(leftKey) ? -1 : 1;
        }
        else
        {
            moveDir.x = 0;
        }


        if (Input.GetKey(upKey) || Input.GetKey(downKey))
        {
            moveDir.y = Input.GetKey(upKey) ? 1 : -1;
        }
        else
        {
            moveDir.y = 0;
        }
    }



    protected override bool IsCursorDown()
    {
        return Input.GetKeyDown(downPress);
    }

    protected override bool IsCursorUp()
    {
        return Input.GetKeyUp(downPress);
    }
}