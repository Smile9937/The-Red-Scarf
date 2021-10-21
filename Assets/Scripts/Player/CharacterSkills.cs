using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkills : MonoBehaviour
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
        // For debug purposes
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AdjustCharacterSkillMeter(0.1f);
            InterruptPassiveCharacterSkillGain(true);
        }
    }

    protected IEnumerator CharacterHandlingOfSkill()
    {
        while (true)
        {
            Debug.Log(characterSkillMeter);
            float theDelayBetween = 0.1f;
            if (!temporaryStopOfHandling)
            {
                AdjustCharacterSkillMeter(0.01f);
                theDelayBetween = 1f;
            }
            else
            {
                theDelayBetween = delayBetweenUpdates;
            }
            yield return new WaitForSeconds(Time.deltaTime + theDelayBetween);
        }
    }

    public void SetCharacterSkillMeter(float adjustment)
    {
        characterSkillMeter = adjustment;
        characterSkillMeter = Mathf.Clamp(characterSkillMeter, 0, 1);
    }
    public void AdjustCharacterSkillMeter(float adjustment)
    {
        if (isRage)
        {
            characterSkillMeter -= adjustment;
        }
        else if (!isRage)
        {
            characterSkillMeter += adjustment;
        }
        characterSkillMeter = Mathf.Clamp(characterSkillMeter, 0, 1);
    }
    public void InterruptPassiveCharacterSkillGain(bool isInterrupted)
    {
        temporaryStopOfHandling = isInterrupted;
        CancelInvoke("ReturnPassiveSkillGain");
        Invoke("ReturnPassiveSkillGain", 2f);
    }
    private void ReturnPassiveSkillGain()
    {
        temporaryStopOfHandling = false;
    }
}
