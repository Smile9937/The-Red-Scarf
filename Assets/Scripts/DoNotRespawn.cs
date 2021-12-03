using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoNotRespawn : MonoBehaviour
{
    private static int IDCounter = 0;
    public int pickUpID = -1;

    private bool pickedUp;
    [SerializeField] private bool checkForPickUpScript = false; // New Bool

    private void Reset()
    {
        pickUpID = IDCounter;
        IDCounter++;
    }

    private void OnEnable()
    {
        GameEvents.Instance.onSaveGame += Save;
        GameEvents.Instance.onLoadGame += Load;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onSaveGame -= Save;
        GameEvents.Instance.onLoadGame -= Load;
    }

    private void Save()
    {
        if(pickedUp == true && !GameManager.Instance.pickupCollectedDatabase.ContainsKey(pickUpID))
        {
            GameManager.Instance.pickupCollectedDatabase.Add(pickUpID, true);
        }
    }

    private void Load()
    {
        if (GameManager.Instance.pickupCollectedDatabase.ContainsKey(pickUpID))
        {
            if (GameManager.Instance.pickupCollectedDatabase[pickUpID])
            {
                if (GetComponent<PickUp>() && checkForPickUpScript) // New code
                {  // New Code
                    GetComponent<PickUp>().DoEvent(); // New code
                }  // New code
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    public void Collected()
    {
        pickedUp = true;
    }
}
