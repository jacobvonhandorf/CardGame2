using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Color damageColor;
    [SerializeField] private Color healColor;

    public void showText(int damageAmount, Vector3 position)
    {
        gameObject.SetActive(true);
        transform.position = position;
        string newText = "" + damageAmount;
        if (damageAmount > 0) // damage - red
        {
            text.text = "-" + damageAmount;
            text.color = damageColor;
        }
        else // healing - green
        {
            damageAmount = Mathf.Abs(damageAmount); // get abs so there is no '-' in text
            text.text = "+" + damageAmount;
            text.color = healColor;
        }
        StopAllCoroutines();
        StartCoroutine(moveAndFade());
    }

    private IEnumerator moveAndFade()
    {
        Color targetColor = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        targetColor.a = 0;
        while (text.color.a > 0f)
        {
            //text.color = Color.Lerp(text.color, targetColor, fadeSpeed * Time.deltaTime);
            Color newColor = text.color;
            newColor.a -= fadeSpeed * Time.deltaTime;
            text.color = newColor;

            Vector3 newTextPosition = transform.position;
            newTextPosition.y += moveSpeed * Time.deltaTime;
            transform.position = newTextPosition;

            yield return null;
        }
    }
}
