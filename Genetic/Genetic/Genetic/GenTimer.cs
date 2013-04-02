using System;

namespace Genetic
{
    public class GenTimer : GenBasic
    {
        /// <summary>
        /// A flag to determine if the timer is currently running.
        /// </summary>
        public bool IsRunning = false;

        /// <summary>
        /// A flag to determine if the elapsed time should be affected by GenG.TimeScale.
        /// Used for calculating the timer based on fast/slow motion.
        /// </summary>
        public bool UseTimeScale;

        /// <summary>
        /// The total amount of time, in seconds, for the timer to reach.
        /// </summary>
        public float Time;

        /// <summary>
        /// The amount of time, in seconds, that has elpased since the timer has been running.
        /// </summary>
        public float ElapsedTime = 0;

        /// <summary>
        /// The method to invoke when the timer has finished.
        /// </summary>
        public Action Callback;

        /// <summary>
        /// A timer used to invoke a method after a given time has elapsed.
        /// </summary>
        /// <param name="time">The total amount of time, in seconds, for the timer to reach.</param>
        /// <param name="callback">The method to invoke when the timer has finished.</param>
        /// <param name="useTimeScale">Determines if the elapsed time should be affected by GenG.TimeScale. Used for calculating the timer based on fast/slow motion.</param>
        public GenTimer(float time, Action callback = null, bool useTimeScale = true)
        {
            UseTimeScale = useTimeScale;
            Time = time;
            Callback = callback;
        }

        public override void Update()
        {
            if (IsRunning)
            {
                if (UseTimeScale)
                    ElapsedTime += GenG.PhysicsTimeStep;
                else
                    ElapsedTime += GenG.TimeStep;

                if (ElapsedTime > Time)
                {
                    Stop();

                    if (Callback != null)
                        Callback.Invoke();
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
                ElapsedTime = 0;

            IsRunning = true;
        }

        /// <summary>
        /// Stops the timer from running.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
        }
    }
}