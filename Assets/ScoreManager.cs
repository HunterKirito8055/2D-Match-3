using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    bool isStart = false;
    [SerializeField] StringEvent onScoreUpdateEvent;
    [SerializeField] int score;
    public int defaultFontSize;
    public float speed = 4f;
    public IntEvent started;
    public IntEvent ended;
    int maxFontSize;
    public int remainder, quotient;
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            onScoreUpdateEvent.Invoke(score.ToString());

        }
    }
    private void Start()
    {
        //InitializerAnimation();
    }
    public void IncrementScore(int value)
    {
        if (!isStart)
        {
            StartCoroutine(IAddToScore(value));
        }
    }
  

    bool isIncre, isDecre;
    void InitializerAnimation()
    {
        maxFontSize = defaultFontSize + defaultFontSize / 8;
    }

    IEnumerator IAddToScore(int scoreValue)
    {
        isStart = true;
        if (scoreValue > 0)
        {
            isIncre = true;
            isDecre = false;
        }
        else
        {
            isIncre = false;
            isDecre = true;
        }
        quotient = scoreValue / 10;
        remainder = scoreValue % 10;
        int tempCounter = quotient == 0 ? 0 : 10;

        while (tempCounter != 0)
        {
            tempCounter--;
            if (isIncre == true && isDecre == false)
            {
                print("is incrementing");
                Score += quotient;
                yield return StartCoroutine(FontSize());

            }
            else if (isIncre == false && isDecre == true)
            {
                print("is decrementing");
                Score -= quotient;
                yield return StartCoroutine(FontSize());
            }
            //}
        }

        Score += remainder;

        yield return new WaitForSeconds(Time.deltaTime);
        ended.Invoke(defaultFontSize);
        isStart = false;
    }
    IEnumerator FontSize()
    {

        yield return new WaitForSeconds(Time.deltaTime * speed);
        started.Invoke(defaultFontSize);
        yield return new WaitForSeconds(Time.deltaTime * speed);
        started.Invoke(maxFontSize);


    }


}
