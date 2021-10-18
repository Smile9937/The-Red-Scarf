using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Skills : MonoBehaviour
{
    public float characterSkillMeter = 0;
    [SerializeField] bool isRage;
    [SerializeField] float delayBetweenUpdates = 0.1f;
    bool temporaryStopOfHandling = false;

    // Start is called before the first frame update
    void Start()
    {
        characterSkillMeter = 0.5f;
        StartCoroutine(CharacterHandlingOfSkill());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AdjustCharacterSkillMeter(0.1f, true);
        }
    }

    protected IEnumerator CharacterHandlingOfSkill()
    {
        while (true)
        {
            Debug.Log(characterSkillMeter);
            if (!temporaryStopOfHandling)
            {
                AdjustCharacterSkillMeter(0.01f, false);
            }
            yield return new WaitForSeconds(Time.deltaTime + delayBetweenUpdates);
        }
    }

    public void AdjustCharacterSkillMeter(float adjustment, bool isMajorInterruption)
    {
        if (isRage)
        {
            characterSkillMeter -= adjustment;
        }
        else if (!isRage && characterSkillMeter < 0)
        {
            characterSkillMeter += adjustment;
        }
        characterSkillMeter = Mathf.Clamp(characterSkillMeter, 0, 1);
        InterruptPassiveCharacterSkillGain(isMajorInterruption);
    }
    public void InterruptPassiveCharacterSkillGain(bool isInterrupted)
    {
        temporaryStopOfHandling = isInterrupted;
        Invoke("ReturnPassiveSkillGain", 0.75f);
    }
    private void ReturnPassiveSkillGain()
    {
        temporaryStopOfHandling = false;
    }
}
