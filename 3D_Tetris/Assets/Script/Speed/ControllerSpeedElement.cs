using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ControllerSpeedElement: MonoBehaviour { //Listener<int> {

    float _afterTime;
    [SerializeField] List<Vector2> ScoreAndTime;

    private void Awake()
    {
        Messenger<int>.AddListener(GameEvent.CURRENT_SCORE.ToString(), CheckScoreAndTimeDrop);
        ScoreAndTime = ScoreAndTime.OrderByDescending(s => s.x).ToList();

    }

    private void Start()
    {
        //this.CallDelegate = CheckSpeed;

    }
    private void CheckScoreAndTimeDrop( int score)
    {

        var speed = ScoreAndTime.Find(s => s.x < score).y;
        if (_afterTime < speed)
        {
            ChengeTimeDrop(speed);
            _afterTime = speed;
        }
    }

    public void ChengeTimeDrop( float timeDrop)
    {
        Messenger<float>.Broadcast(GameEvent.CHANGE_TIME_DROP.ToString(), timeDrop);
    }
}
