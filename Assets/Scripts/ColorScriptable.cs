using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color Set", menuName = "New Color Set", order = 1)]
public class ColorScriptable : ScriptableObject
{
    public List<Color> colors;
    public int highScore;

    [HideInInspector] public string lowLevelKey = "lowlevelKey";
    [HideInInspector] public string midLevelKey = "midLevelKey";
    [HideInInspector] public string semiHighLevelKey = "semiHighLevelKey";
    [HideInInspector] public string highLevelKey = "highLevelKey";
    public void UpdateHighScores(int _score)
    {
        highScore = _score;
    }
}
