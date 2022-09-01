using UnityEngine;

namespace SayKitInternal
{
    public class SayKitApp
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void init()
        {
            SayKit.init();
        }
    }
}