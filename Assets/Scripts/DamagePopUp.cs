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
    [SerializeField] private Color gradientColor;
    [SerializeField] private Color mainColor;
    private Vector3 startScale;
    float dissapearTimer = 1f;
    Color textColor;
    private void Start()
    {
        startScale = transform.localScale;
        textColor = text.color;
        dissapearTimer = dissapearTimerMax;
    }
    public void SetText(int damage, int damageComparison)
    {
        startScale = transform.localScale;

        if(damage <= damageComparison)
        {
            transform.localScale = startScale;
        }
        else if(damage <= damageComparison + 5)
        {
            transform.localScale = Vector3.Scale(startScale, new Vector3(1.2f, 1.2f, 1));
        }
        else if(damage < damageComparison + 10)
        {
            transform.localScale = Vector3.Scale(startScale, new Vector3(1.4f, 1.4f, 1));
        }
        else if (damage >= damageComparison + 20)
        {
            transform.localScale = Vector3.Scale(startScale, new Vector3(1.6f, 1.6f, 1));
            text.colorGradient = new VertexGradient(gradientColor, mainColor, gradientColor, mainColor);
        }
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
