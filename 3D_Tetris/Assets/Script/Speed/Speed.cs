using UnityEngine;

public class Speed : MonoBehaviour
{
    [SerializeField] private float _TimeDrop = 1;
    [SerializeField] private float _TimeDropAfterDestroy = 1;
    [SerializeField] private float _TimeMove = 1;

    public static float timeDrop { get; private set; }
    public static float timeDropAfterDestroy { get; private set; }
    public static float timeMove { get; private set; }

    private void Awake()
    {
        timeDrop = _TimeDrop;
        timeDropAfterDestroy = _TimeDropAfterDestroy;
        timeMove = _TimeMove;

    }

    public static void SetTimeDrop(float time)
    {
        timeDrop = time;
    }
}