using System;
using UnityEngine;


namespace Jigbox.VirtualCursor
{
    public class DragInfo
    {
        /// <summary>
        /// Seconds
        /// </summary>
        const float DragInterval = 0.08f;

        public bool IsCurrentDrag;
        float dragTimeCount;

        public bool CheckDrag(Vector2 moveDelta)
        {
            dragTimeCount += Time.deltaTime;
            if (dragTimeCount > DragInterval && !moveDelta.Equals(Vector2.zero))
            {
                IsCurrentDrag = true;
            }
            return IsCurrentDrag;
        }

        public void Reset()
        {
            IsCurrentDrag = false;
            dragTimeCount = 0;
        }

    }
}
