using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GoldPurchaseStatus
{
    failed, noGold, success, processing
}
public class BonusTimeScript : MonoBehaviour
{
    public static BonusTimeScript bonusInstance;
    public StringEvent timeShowEvent;
    public Text timeCount;

    public void AddExtraTime(int _time)
    {
        _time += int.Parse(timeCount.text);
        //timeCount.text = _time.ToString();
        PlayerPrefs.SetInt(timeBonusKey, _time);
        DisplayTimeCount(_time.ToString());
    }
    //    private void Awake()
    //    {
    //        bonusInstance = this;
    //        GOLDAMOUNT = PlayerPrefs.GetInt(goldKey);
    //    }
    void Start()
    {
        RetrieveBonusTime();
    }
    string timeBonusKey = "timeBonus";

    void RetrieveBonusTime()
    {
        timeCount.text = PlayerPrefs.GetInt(timeBonusKey, 15).ToString();
        DisplayTimeCount(timeCount.text);
    }
    void DisplayTimeCount(string val)
    {
        timeShowEvent.Invoke(val);
    }
   

}
