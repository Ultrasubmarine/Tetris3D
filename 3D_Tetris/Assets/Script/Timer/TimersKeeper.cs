using System;
using System.Collections.Generic;
using System.Linq;

    public class TimersKeeper : MonoBehaviourSingleton<TimersKeeper>
    {
        public static event Action onUpdate;

        private readonly List<Timer> _timers = new List<Timer>();


        public static Timer Schedule(float intervalInSeconds, bool isUnscaled = false, bool withStarting = true)
        {
            var newTimer = new Timer(intervalInSeconds, isUnscaled);
            instance._timers.Add(newTimer);
            
            if (withStarting)
                newTimer.Start();

            return newTimer;
        }

        public static Timer Schedule(TimeSpan interval, bool isUnscaled = false, bool withStarting = true)
        {
            var newTimer = new Timer(interval, isUnscaled);
            instance._timers.Add(newTimer);

            if (withStarting)
                newTimer.Start();

            return newTimer;
        }


        private void Update()
        {
            onUpdate?.Invoke();
            var isPause = UnityEngine.Time.timeScale <= 0f;
            var timersToRemove = new List<Timer>();

            foreach (var timer in _timers.ToList())
            {
                if (timer.currentState == TimerState.Completed || timer.currentState == TimerState.Canceled)
                {
                    timersToRemove.Add(timer);
                    continue;
                }

                if (timer.currentState != TimerState.Started)
                    continue;

                if (!timer.isUnscaled && isPause)
                    continue;

                timer.Update(UnityEngine.Time.deltaTime);
            }

            foreach (var timer in timersToRemove)
                _timers.Remove(timer);
        }
    }
