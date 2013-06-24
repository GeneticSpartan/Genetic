using System;

namespace Genetic
{
    /// <summary>
    /// A timer that can invoke a given callback method after a specified time has elapsed since the timer was started.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenTimer : GenBasic
    {
        /// <summary>
        /// A flag to determine if the timer is currently running.
        /// </summary>
        public bool IsRunning;

        /// <summary>
        /// A flag to determine if the elapsed time should be affected by GenG.TimeScale.
        /// Used for calculating the timer based on fast/slow motion.
        /// </summary>
        public bool UseTimeScale; // TODO: Find a reason for UseTimeScale in GenTimer, or remove it completely.

        /// <summary>
        /// The total amount of time, in seconds, for the timer to reach.
        /// </summary>
        public float Duration;

        /// <summary>
        /// The amount of time, in seconds, that has elapsed since the timer has been running.
        /// </summary>
        public float Elapsed;

        public bool IsLooping;

        /// <summary>
        /// The method to invoke when the timer has finished.
        /// </summary>
        public Action Callback;

        /// <summary>
        /// Gets the remaining time left, in seconds, before the timer completes its duration.
        /// </summary>
        public float Remaining
        {
            get { return Duration - Elapsed; }
        }

        /// <summary>
        /// A timer used to invoke a method after a given time has elapsed.
        /// </summary>
        /// <param name="duration">The total amount of time, in seconds, for the timer to reach.</param>
        /// <param name="callback">The method to invoke when the timer has finished.</param>
        /// <param name="useTimeScale">Determines if the elapsed time should be affected by <c>GenG.TimeScale</c>. Used for calculating the timer based on faster/slower update calls.</param>
        public GenTimer(float duration, Action callback = null, bool useTimeScale = true)
        {
            IsRunning = false;
            UseTimeScale = useTimeScale;
            Duration = duration;
            Elapsed = 0f;
            IsLooping = false;
            Callback = callback;
        }

        /// <summary>
        /// Updates the timer, and invokes the callback method when the timer has finished.
        /// </summary>
        public override void Update()
        {
            if (IsRunning)
            {
                Elapsed += GenG.TimeStep;

                if (Elapsed >= Duration)
                {
                    if (Callback != null)
                        Callback.Invoke();

                    if (IsLooping)
                        Elapsed -= Duration;
                    else
                        Stop();
                }
            }
        }

        /// <summary>
        /// Starts running the timer.
        /// </summary>
        /// <param name="forceReset">Determines if the elapsed time should be reset to 0 before starting the timer. False will start the timer from the current elapsed time value.</param>
        public void Start(bool forceReset = true)
        {
            if (forceReset)
                Elapsed = 0f;

            IsRunning = true;
        }

        /// <summary>
        /// Stops the timer from running.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
        }

        public override void Reset()
        {
            base.Reset();

            Elapsed = 0f;
            Stop();
        }
    }
}