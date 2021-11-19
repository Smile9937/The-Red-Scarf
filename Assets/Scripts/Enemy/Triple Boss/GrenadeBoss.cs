using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBoss : TripleBoss
{
    protected override void StartCurrentPattern()
    {
        StartCoroutine(Wait());
    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        PatternDone();
    }
}
