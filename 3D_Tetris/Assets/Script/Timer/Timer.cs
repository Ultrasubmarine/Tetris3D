using System;


    public class Timer : FiniteStateMachine<TimerState>
    {
        public event Action<TimeSpan> onChanged;

        public event Action onOneSecondPassed;

        public event Action<float> onProgressChanged;

        public TimeSpan timeTotal { get; }

        public TimeSpan timeLeft { get; private set; }

        public bool isUnscaled { get; }

        public float progress { get; private set; }

        private float _oneSecondPassedChecker;


        public Timer(float intervalInSeconds, bool isUnscaled = true)
        {
            timeTotal = timeLeft = TimeSpan.FromSeconds(intervalInSeconds);
            this.isUnscaled = isUnscaled;
        }

        public Timer(TimeSpan interval, bool isUnscaled = true)
        {
            timeTotal = timeLeft = interval;
            this.isUnscaled = isUnscaled;
        }


        public void Start()
        {
            if (currentState != TimerState.Scheduled)
                return;

            SetState(TimerState.Started);
        }

        public void Cancel()
        {
            if (currentState == TimerState.Canceled || currentState == TimerState.Completed)
                return;

            SetState(TimerState.Canceled);
        }

        public void Subtract(TimeSpan timeSpan)
        {
            if (currentState == TimerState.Canceled || currentState == TimerState.Completed)
                return;

            timeLeft = timeLeft.Subtract(timeSpan);
        }

        public void Subtract(float seconds)
        {
            Subtract(TimeSpan.FromSeconds(seconds));
        }

        public void Update(float deltaTime)
        {
            timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(deltaTime));
            if (timeLeft.TotalSeconds < 0)
                timeLeft = TimeSpan.Zero;

            onChanged?.Invoke(timeLeft);

            progress = (float)(timeTotal.Subtract(timeLeft).TotalSeconds / timeTotal.TotalSeconds);
            onProgressChanged?.Invoke(progress);

            _oneSecondPassedChecker += deltaTime;
            if (_oneSecondPassedChecker >= 1f)
            {
                _oneSecondPassedChecker = 0f;
                onOneSecondPassed?.Invoke();
            }

            if (timeLeft.TotalSeconds > 0)
                return;

            SetState(TimerState.Completed);
        }
    }
