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
    private bool isToasting = false;

    public void doToast(string message)
    {

        Debug.Log("Do toast called");
        Color tmp = background.color;
        tmp.a = 0f;
        background.color = tmp;
        textMesh.color = tmp;

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
        Color tmp1 = background.color;
        Color tmp2 = textMesh.color;

        // fade in
        while (background.color.a < .95f)
        {
            // background alpha
            tmp1.a += fadeSpeed;
            background.color = tmp1;

            // text alpha
            tmp2.a += fadeSpeed;
            textMesh.color = tmp2;

            yield return null;
        }

        yield return new WaitForSeconds(visibleTime);

        // fade out
        while (background.color.a > 0f)
        {
            tmp1.a -= fadeSpeed;
            background.color = tmp1;

            // text alpha
            tmp2.a -= fadeSpeed;
            textMesh.color = tmp2;

            yield return null;
        }

        gameObject.SetActive(false);
        isToasting = false;
    }
}
