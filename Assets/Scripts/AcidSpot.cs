using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidSpot : MonoBehaviour
{
    [SerializeField] private float timeUntilAcidDeactivation = 2f;
    private Hazard acid;
    private void Start()
    {
        acid = GetComponentInChildren<Hazard>();
        acid.gameObject.SetActive(false);
    }

    public void ActivateAcid()
    {
        acid.gameObject.SetActive(true);
        StartCoroutine(DeactivateAcid());
    }

    private IEnumerator DeactivateAcid()
    {
        yield return new WaitForSeconds(timeUntilAcidDeactivation);
        acid.gameObject.SetActive(false);
    }
}