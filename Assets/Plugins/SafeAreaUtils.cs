using UnityEngine;
using System.Runtime.InteropServices;

namespace Jigbox.Examples
{
    public class SafeAreaUtils
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private extern static void GetSafeAreaImpl(out float x, out float y, out float w, out float h);
#endif

        public static Rect GetSafeArea()
        {
            float x, y, w, h;
#if UNITY_IOS
            GetSafeAreaImpl(out x, out y, out w, out h);
#else
            x = 0;
            y = 0;
            w = Screen.width;
            h = Screen.height;
#endif
            return new Rect(x, y, w, h);
        }
    }
}
