using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditTextController : MonoBehaviour
{
    [SerializeField] float delayBeforeStart = 2f;
    [SerializeField] float textSpeed = 1;
    [SerializeField] float titleSpeed = 1;
    [SerializeField] float theCreditDuration = 20f;
    [SerializeField] RectTransform theCreditText;
    [SerializeField] GameObject theCreditTitle;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartCredits", delayBeforeStart);
    }

    private void StartCredits()
    {
        StartCoroutine("HandleMovingUpText");
    }

    private IEnumerator HandleMovingUpText()
    {
        while (theCreditDuration >= 0)
        {
            theCreditTitle.transform.position += new Vector3(0, titleSpeed * Time.deltaTime, 0);
            theCreditText.transform.position += new Vector3(0, textSpeed,0);
            yield return new WaitForSeconds(0.01f);
            theCreditDuration -= 0.01f;
        }
        FindObjectOfType<SceneLoader>().Invoke("StartLoadScene",1f);
        StopAllCoroutines();
    }
}
