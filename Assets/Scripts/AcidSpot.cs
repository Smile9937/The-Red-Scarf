using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidSpot : MonoBehaviour
{
    [SerializeField] private float timeUntilAcidActivation = 0.2f;
    [SerializeField] private float timeUntilAcidDeactivation = 2f;
    private Hazard acid;
    private void Start()
    {
        acid = GetComponentInChildren<Hazard>();
        acid.gameObject.SetActive(false);
    }

    public void ActivateAcid()
    {
        StartCoroutine(Activate());
    }

    private IEnumerator Activate()
    {
        yield return new WaitForSeconds(timeUntilAcidActivation);
        acid.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeUntilAcidDeactivation);
        acid.gameObject.SetActive(false);
    }

}