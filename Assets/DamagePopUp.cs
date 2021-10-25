using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    [SerializeField] private float dissapearTimerMax = 1f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float dissapearSpeed = 3f;
    [SerializeField] private float scaleIncrease = 0.7f;
    [SerializeField] private TextMeshProUGUI text;
    float dissapearTimer = 1f;
    Color textColor;
    private void Start()
    {
        textColor = text.color;
        dissapearTimer = dissapearTimerMax;
    }
    public void SetText(int damage)
    {
        text.SetText(damage.ToString());
    }
    private void Update()
    {
        transform.position += new Vector3(0, moveSpeed) * Time.deltaTime;

        if(dissapearTimer > dissapearTimerMax * 0.5f)
        {
            transform.localScale += Vector3.one * scaleIncrease * Time.deltaTime;
        } else
        {
            transform.localScale -= Vector3.one * scaleIncrease * Time.deltaTime;
        }

        dissapearTimer -= Time.deltaTime;
        if(dissapearTimer < 0)
        {
            textColor.a -= dissapearSpeed * Time.deltaTime;
            text.color = textColor;
            if(textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
