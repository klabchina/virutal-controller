using UnityEngine;

namespace Jigbox.Examples
{
    public class ExampleSceneBase : MonoBehaviour
    {
        protected virtual int TargetFrameRate
        {
            get { return 30; }
        }

        protected virtual void Awake()
        {
            Application.targetFrameRate = TargetFrameRate;
        }
    }
}
