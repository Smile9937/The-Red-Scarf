using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoNotRespawn : MonoBehaviour
{
    private static int IDCounter = 0;
    public int pickUpID = -1;
    public static Dictionary<int, bool> pickupCollectedDatabase;
    private void Awake()
    {
        if (pickupCollectedDatabase == null)
        {
            pickupCollectedDatabase = new Dictionary<int, bool>();
        }
    }

    private void Reset()
    {
        pickUpID = IDCounter;
        IDCounter++;
    }

    private void Start()
    {
        if (pickupCollectedDatabase.ContainsKey(pickUpID))
        {
            if (pickupCollectedDatabase[pickUpID])
            {
                Destroy(gameObject);
            }
            else
            {
                pickupCollectedDatabase.Add(pickUpID, false);
            }
        }
    }

    public void Collected()
    {
        pickupCollectedDatabase[pickUpID] = true;
    }
}
