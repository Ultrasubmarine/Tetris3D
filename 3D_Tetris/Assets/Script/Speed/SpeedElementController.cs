using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SpeedElementController : MonoBehaviour
{
    //Listener<int> {

    private float _afterTime;
    [SerializeField] private List<Vector2> ScoreAndTime;

    private void Awake()
    {
//        Messenger<int>.AddListener(GameEvent.CURRENT_SCORE.ToString(), CheckScoreAndTimeDrop);
        ScoreAndTime = ScoreAndTime.OrderByDescending(s => s.x).ToList();
    }

    private void Start()
    {
        //this.CallDelegate = CheckSpeed;
    }

    private void CheckScoreAndTimeDrop(int score)
    {
        var speed = ScoreAndTime.Find(s => s.x < score).y;
        if (_afterTime < speed)
        {
            ChangeTimeDrop(speed);
            _afterTime = speed;
        }
    }

    public void ChangeTimeDrop(float timeDrop)
    {
        //  Messenger<float>.Broadcast(GameEvent.CHANGE_TIME_DROP.ToString(), timeDrop);
    }
}