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
    [SerializeField] private Image borderBGBar;
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
        borderBGBar.color = new Color(borderBGBar.color.r, borderBGBar.color.g, borderBGBar.color.b, 0);
    }

    public void StartUpUIHealth()
    {
        StartCoroutine("SetUpUIHealth");
    }
    private IEnumerator SetUpUIHealth()
    {
        if (hasBeenSetUp)
            yield return null;

        hasBeenSetUp = true;
        borderHealthBar.color = new Color(borderHealthBar.color.r, borderHealthBar.color.g, borderHealthBar.color.b, 1);
        borderBGBar.color = new Color(borderBGBar.color.r, borderBGBar.color.g, borderBGBar.color.b, 1);
        healthBar.color = new Color(healthBar.color.r, healthBar.color.g, healthBar.color.b, 0.75f);
        while (theSlider.value < theSlider.maxValue)
        {
            yield return new WaitForSeconds(0.1f);
            healthBar.color += new Color(0, 0, 0, 0.01f);
            theSlider.value += theSlider.maxValue / 25;
        }
        borderBGBar.color = new Color(borderBGBar.color.r, borderBGBar.color.g, borderBGBar.color.b, 0.35f);
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
