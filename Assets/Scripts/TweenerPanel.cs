using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Launchship2DTiles
{
    public class TweenerPanel : MonoBehaviour
    {

        public PanelsEnum panelType;

    }




    [System.Serializable]
    public enum PanelsEnum
    {
        startPanel, shopPanel, timesUpPanel, presetPanel, hudPanel, customCreatePanel,gameOverPanel
    }
}