using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toaster : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float visibleTime;
    private float backgroundStartingAlpha;
    private float textStartingAlpha;
    private bool isToasting = false;

    public static Toaster instance;
    public static Toaster Get()
    {
        return instance;
    }

    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
        Debug.Log("in start " + textMesh.color);
        backgroundStartingAlpha = background.color.a;
        textStartingAlpha = textMesh.color.a;
    }

    public void doToast(string message)
    {

        Debug.Log("Do toast called");
        Color tmp = background.color;
        tmp.a = 0f;
        background.color = tmp;
        Color tmp2 = textMesh.color;
        tmp2.a = 0f;
        textMesh.color = tmp2;

        gameObject.SetActive(true);
        textMesh.text = message;

        StartCoroutine(toastCoroutine());
    }

    IEnumerator toastCoroutine()
    {
        while (isToasting)
        {
            yield return null;
        }
        isToasting = true;
        Debug.Log("tmp2 at start: " + textMesh.color);
        Color bgColor = new Color(background.color.r, background.color.g, background.color.b, background.color.a);
        Color textColor = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a);
        Debug.Log("textColor: " + textColor);

        // fade in
        while (background.color.a < backgroundStartingAlpha && textMesh.color.a < textStartingAlpha)
        {
            // background alpha
            if (background.color.a < backgroundStartingAlpha)
            {
                bgColor.a += fadeSpeed * Time.deltaTime;
                background.color = bgColor;
            }

            // text alpha
            if (textMesh.color.a < textStartingAlpha)
            {
                Debug.Log("Tmp 2: " + textMesh.color);
                textColor.a += fadeSpeed * Time.deltaTime;
                textMesh.color = textColor;
            }

            yield return null;
        }

        yield return new WaitForSeconds(visibleTime);

        // fade out
        while (background.color.a > 0f)
        {
            bgColor.a -= fadeSpeed * Time.deltaTime;
            background.color = bgColor;

            // text alpha
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            yield return null;
        }

        gameObject.SetActive(false);
        isToasting = false;
    }
}
