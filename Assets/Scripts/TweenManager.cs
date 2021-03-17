using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Launchship2DTiles
{
    public class TweenManager : MonoBehaviour
    {
        public static TweenManager instance;
        public List<RectTransform> rectTrans = new List<RectTransform>();
        public List<Vector2> originalPos = new List<Vector2>();
        public PanelsEnum compareEnum;
        public PanelsEnum ActivatePanel
        {
            get { return compareEnum; }
            set
            {
                compareEnum = value;
                OriginalPlaces();
                foreach (var item in rectTrans)
                {
                    if (compareEnum == item.GetComponent<TweenerPanel>().panelType)
                    {
                        TranslateTweenToCentre(item);
                    }

                }
                if (compareEnum == PanelsEnum.hudPanel)
                {
                    TimerScript.instance.TimeMode = TimeMode.run;
                }
                else
                {
                    TimerScript.instance.TimeMode = TimeMode.pause;
                }
            }
        }

        private void Awake()
        {

            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);


            for (int i = 0; i < transform.childCount-1; i++)
            {
                rectTrans.Add(transform.GetChild(i).GetComponent<RectTransform>());
                originalPos.Add(rectTrans[i].localPosition);
            }

        }

        void TranslateTweenToCentre(RectTransform _rect)
        {
            _rect.DOAnchorPos(Vector2.zero, 1f).SetUpdate(true).OnComplete(() => ActivateCanvas(_rect));

        }
        void OriginalPlaces()
        {
            for (int i = 0; i < rectTrans.Count; i++)
            {
                rectTrans[i].DOAnchorPos(originalPos[i], 1f).SetUpdate(true).OnComplete(() => DeactivateCanvas(rectTrans[i]));
            }

        }
        RectTransform currentRect;
        CanvasGroup canvasGroup;
        void DeactivateCanvas(RectTransform _rect)
        {
            canvasGroup = _rect.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;

        }
        void ActivateCanvas(RectTransform _rect)
        {
            canvasGroup = _rect.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.interactable = true;
        }
        Vector3 punchScale = new Vector3(1f, 0.5f, 0.5f);
        public void ButtonTweenPunch(RectTransform _button, PanelsEnum _panel)
        {
            _button.DORewind();
            _button.DOPunchScale(punchScale, 0.2f).SetUpdate(true).OnComplete(() => ActivatePanel = _panel);/*.SetEase(Ease.InOutBounce);*/
        }
        public void ButtonTweenPunch(RectTransform _button)
        {
            _button.DORewind();
            _button.DOPunchScale(punchScale, 0.2f).SetUpdate(true)/*.OnComplete(() => )*/;/*.SetEase(Ease.InOutBounce);*/
        }
        public void ScoreTween(Text _text, string _score)
        {
            _text.DORewind();
            _text.DOText(_score, 0.5f, true, ScrambleMode.Numerals);
        }

        public void ChangePositions(GameObject one, GameObject two)
        {
            if (one == null || two == null)
            {
                return;
            }
            Vector3 onePos = one.transform.position;
            Vector3 twoPos = two.transform.position;

            one.transform.DOMove(twoPos, 0.3f);
            two.transform.DOMove(onePos, 0.3f)/*.OnComplete(() => BoardBuilding.isTransitioning = false)*/;
        }

    }//class
}//namespace