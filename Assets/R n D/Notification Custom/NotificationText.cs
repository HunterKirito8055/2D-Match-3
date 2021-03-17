using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationText : MonoBehaviour
{
    Text text;
    float alpha = 1;


    private void OnEnable()
    {
        text = this.GetComponent<Text>();
        //StartCoroutine(Fade());
    }

    public void NotificationMessage(string message)
    {
        transform.SetAsLastSibling();
        text.text = message;
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        alpha = 1f;
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        yield return new WaitForSeconds(2f);
        while (text.color.a > 0)
        {
            alpha -= Time.unscaledDeltaTime * 5f;
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        this.gameObject.SetActive(false);
    }
}
