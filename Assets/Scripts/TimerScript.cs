using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Launchship2DTiles
{
    public class TimerScript : MonoBehaviour
    {
        public class vishal : MonoBehaviour
        {
            public int v = 14;
        }
        public static TimerScript instance;
        public static float maxtime;
        TimeMode timeMode;
        public GameObject timeEndPanel;
        public TimeMode TimeMode
        {
            get { return timeMode; }
            set
            {
                timeMode = value;
                switch (timeMode)
                {
                    case TimeMode.run:
                        isTimerun = true;
                        Time.timeScale = 1;
                        break;
                    case TimeMode.end:
                        isTimerun = false;
                        timer = 0;
                        TweenManager.instance.ActivatePanel = PanelsEnum.timesUpPanel;
                        break;
                    case TimeMode.pause:
                        isTimerun = false;
                        Time.timeScale = 0;
                        break;
                    default:
                        break;
                }
            }
        }
        bool isTimerun;
        float timer;
        public float Timer
        {
            get { return timer; }
            set
            {
                timer = value;
                timerTextEvent?.Invoke(timer.ToString("0.0"));

                if (timer <= 0)
                {
                    BoardBuilding.mouseIns = false;
                    gameLostAudioEvent?.Invoke();
                    TimeMode = TimeMode.end;
                }
            }
        }
        public AudioEvent gameLostAudioEvent;

        public FloatEvent timerEvent;
        public StringEvent timerTextEvent;
        private void Awake()
        {
            if (instance == null)
                instance = this;
        }
        public void StartTimer(float val)
        {
            if (isTimerun)
                StartCoroutine(ITimer(val));
        }
        IEnumerator ITimer(float val)
        {
            Timer = val;
            while (isTimerun)
            {
                yield return !isTimerun;
                Timer -= Time.deltaTime;
                //if (Timer <= 0)
                //{
                //    yield return new WaitForSeconds(Time.deltaTime);
                //    TimeMode = TimeMode.end;
                //}
                timerEvent?.Invoke(Timer / maxtime);
            }
        }

    }
    public enum TimeMode { run, end, pause }
}