using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidSpot : MonoBehaviour
{
    [SerializeField] private float timeUntilAcidActivation = 0.2f;
    [SerializeField] private float timeUntilAcidDeactivation = 2f;
    private Hazard acid;
    private SoundPlayer soundPlayer;
    private void Start()
    {
        acid = GetComponentInChildren<Hazard>();
        soundPlayer = GetComponent<SoundPlayer>();
        acid.gameObject.SetActive(false);
    }

    public void ActivateAcid()
    {
        StartCoroutine(Activate());
    }

    private IEnumerator Activate()
    {
        soundPlayer.PlaySound(0);
        yield return new WaitForSeconds(timeUntilAcidActivation);
        acid.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeUntilAcidDeactivation);
        soundPlayer.StopSound(0);
        acid.gameObject.SetActive(false);
    }

}