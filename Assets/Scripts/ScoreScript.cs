using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Launchship2DTiles
{
    public class ScoreScript : MonoBehaviour
    {
        public Text ScoreText;
        BoardBuilding board;
        public int score;
        public static string highScoreKey = "";
        public int Score
        {
            get
            {
                highScoreTextEvent?.Invoke(PlayerPrefs.GetInt(highScoreKey).ToString());
                return score;
            }
            private set
            {
                score = value;
                TweenManager.instance.ScoreTween(ScoreText, score.ToString());
                //scoreTextEvent?.Invoke(" " + score.ToString());
                if (score >= PlayerPrefs.GetInt(highScoreKey))
                {
                    PlayerPrefs.SetInt(highScoreKey, score);
                    highScoreTextEvent?.Invoke(PlayerPrefs.GetInt(highScoreKey).ToString());
                    board.currentLevelColr.UpdateHighScores(PlayerPrefs.GetInt(highScoreKey));
                }
            }
        }
        //public Text scoreTxt;
        public StringEvent scoreTextEvent;
        public StringEvent highScoreTextEvent;
        public IntEvent scoreSizeEvent;
        int defaultFontSize, maxFontSize;

        public Text textType;
        private void Awake()
        {
            board = FindObjectOfType<BoardBuilding>();
        }
        private void Start()
        {
            defaultFontSize = textType.fontSize;
            maxFontSize = defaultFontSize + 6;
        }
        public void AddScore(int count)
        {
            //Debug.Log(count);
            //score += count;
            ////count /= 2;
            ////score *= count;
            //scoreTxt.text = "Score :" + score;
            Score += count;
            //StartCoroutine(IAddScore(count));
        }
        public int transitionSpeed;
        IEnumerator IAddScore(int _score)
        {
            int t = _score /_score;
            for (int i = 0; i < _score; i++)
            {
                textType.fontSize = maxFontSize;
                if (_score > 0)
                {
                    Score += t;
                    yield return new WaitForSeconds(Time.deltaTime * transitionSpeed);
                }
                else
                {//for Undo or negating score sometimes if needed
                    Score -= t;
                    yield return new WaitForSeconds(Time.deltaTime * transitionSpeed);
                }
                yield return new WaitForSeconds(Time.deltaTime * transitionSpeed);
                textType.fontSize = defaultFontSize;
            }
        }
    }
}