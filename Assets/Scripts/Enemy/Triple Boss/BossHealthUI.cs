using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public List<TripleBoss> theBossesUsed = new List<TripleBoss>();
    [SerializeField] private Slider theSlider;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image borderHealthBar;
    bool hasBeenSetUp = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (TripleBoss bosses in theBossesUsed)
        {
            theSlider.maxValue += bosses.GetBossHealth();
        }
        theSlider.value = 1;
        healthBar.color = new Color(healthBar.color.r, healthBar.color.g, healthBar.color.b, 0);
        borderHealthBar.color = new Color(borderHealthBar.color.r, borderHealthBar.color.g, borderHealthBar.color.b, 0);
    }

    public void StartUpUIHealth()
    {
        StartCoroutine("SetUpUIHealth");
    }
    private IEnumerator SetUpUIHealth()
    {
        if (hasBeenSetUp)
            yield return null;

        float percentageOfProgress = 0;
        hasBeenSetUp = true;
        while (theSlider.value < theSlider.maxValue)
        {
            theSlider.value += theSlider.maxValue / 20;
            yield return new WaitForSeconds(0.1f);
            healthBar.color += new Color(healthBar.color.r, healthBar.color.g, healthBar.color.b, percentageOfProgress);
            borderHealthBar.color = new Color(borderHealthBar.color.r, borderHealthBar.color.g, borderHealthBar.color.b, percentageOfProgress);
            percentageOfProgress += 0.2f;
            Mathf.Clamp(percentageOfProgress, 0, 1);
        }
    }


    public void SetBossHealthBar()
    {
        int theBossHealth = 0;
        foreach (TripleBoss bosses in theBossesUsed)
        {
            theBossHealth += bosses.GetBossHealth();
        }
        theSlider.value = theBossHealth;
    }
}
