using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimitScript : MonoBehaviour
{
    [SerializeField] int targetFrameRate = 60;

    // Start is called before the first frame update
    void Start()
    {
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}
