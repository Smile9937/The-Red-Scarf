using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomSwitch : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] SpriteRenderer backgroundForRoom;
    [SerializeField] SpriteRenderer oldBackgroundForRoom;
    [Header("Enemy Spawned")]
    [SerializeField] List<GameObject> enemiesToRespawn = new List<GameObject>();
    List<Vector2> enemiesToRespawnPos = new List<Vector2>();
    List<TriggerAreaCheck> theTriggerArea = new List<TriggerAreaCheck>();
    List<HotZoneCheck> theHotZones = new List<HotZoneCheck>();
    [Header("Which Method")]
    [SerializeField] bool useNewMethod = false;
    [Header("Normal Method")]
    [SerializeField] Animator theRoomController;
    [SerializeField] int roomNumber = 0;
    [SerializeField] int fullRoomNumber = 0;
    [Header("New Method")]
    [SerializeField] CinemachineConfiner theConfiner;
    [SerializeField] PolygonCollider2D theRoomCollider;

    RoomMaster theRoomMaster;

    private void Awake()
    {
        theRoomMaster = FindObjectOfType<RoomMaster>();
        foreach (var item in enemiesToRespawn)
        {
            if (item != null)
            {
                theTriggerArea.Add(item.GetComponentInChildren<Enemy>().GetComponentInChildren<TriggerAreaCheck>());
                theHotZones.Add(item.GetComponentInChildren<Enemy>().GetComponentInChildren<HotZoneCheck>());
                if (!enemiesToRespawnPos.Contains(new Vector2(item.GetComponentInChildren<Enemy>().gameObject.transform.position.x, item.GetComponentInChildren<Enemy>().gameObject.transform.position.y)))
                {
                    enemiesToRespawnPos.Add(new Vector2(item.GetComponentInChildren<Enemy>().gameObject.transform.position.x, item.GetComponentInChildren<Enemy>().gameObject.transform.position.y));
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (useNewMethod)
            {
                theConfiner.m_BoundingShape2D = theRoomCollider;
            }
            else
            {
                theRoomController.SetInteger("theRoomNumber", roomNumber);
                theRoomMaster.currentPlayerRoomLoc = fullRoomNumber;
                RespawnEnemies();
                //Invoke("DespawnEnemies", Time.deltaTime + 2f);
            }
            if (GameObject.FindGameObjectWithTag("Background Room"))
            {
                if (GameObject.FindGameObjectWithTag("Background Room").GetComponent<SpriteRenderer>().enabled == true)
                {
                    oldBackgroundForRoom = GameObject.FindGameObjectWithTag("Background Room").GetComponent<SpriteRenderer>();
                }
            }
            if (backgroundForRoom != null && oldBackgroundForRoom != backgroundForRoom)
            {
                if (oldBackgroundForRoom != null)
                {
                    oldBackgroundForRoom.gameObject.SetActive(false);
                }
                backgroundForRoom.gameObject.SetActive(true);
                if (backgroundForRoom.GetComponent<Parallax>())
                {
                    backgroundForRoom.GetComponent<Parallax>().ActivateObject();
                }
            }
        }
    }

    
    private void OnTriggerExit2D(Collider2D collission)
    {
        if (collission.tag == "Player")
        {
            Invoke("DespawnEnemies", Time.deltaTime + 2f);
        }
    }


    private void RespawnEnemies()
    {
        if (theRoomMaster.currentPlayerRoomLoc == fullRoomNumber)
        {
            CancelInvoke("DespawnEnemies");
            foreach (var item in enemiesToRespawn)
            {
                if (item != null)
                {
                    if (item.activeSelf == false)
                    {
                        item.gameObject.SetActive(true);
                        item.GetComponentInChildren<Enemy>().currentHealth = item.GetComponentInChildren<Enemy>().maxHealth;
                        //item.GetComponentInChildren<TriggerAreaCheck>().gameObject.SetActive(true);
                        //item.GetComponentInChildren<Enemy>().gameObject.transform.position = enemiesToRespawnPos[enemiesToRespawn.IndexOf(item)];
                        if (theHotZones[enemiesToRespawn.IndexOf(item)] != null)
                            theHotZones[enemiesToRespawn.IndexOf(item)].enabled = false;
                        if (theTriggerArea[enemiesToRespawn.IndexOf(item)] != null)
                            theTriggerArea[enemiesToRespawn.IndexOf(item)].enabled = true;
                        foreach (var itemLoc in enemiesToRespawnPos)
                        {
                            if (enemiesToRespawn.IndexOf(item) == enemiesToRespawnPos.IndexOf(itemLoc))
                            {
                                item.GetComponentInChildren<Enemy>().gameObject.transform.position = itemLoc;
                            }
                        }
                    }
                }
            }
        }
    }
    private void DespawnEnemies()
    {
        if (theRoomMaster.currentPlayerRoomLoc != fullRoomNumber)
        {
            CancelInvoke("DespawnEnemies");
            foreach (var item in enemiesToRespawn)
            {
                if (item != null)
                {
                    if (item.activeSelf == true)
                    {
                        foreach (var itemLoc in enemiesToRespawnPos)
                        {
                            if (enemiesToRespawn.IndexOf(item) == enemiesToRespawnPos.IndexOf(itemLoc))
                            {
                                item.GetComponentInChildren<Enemy>().gameObject.transform.position = itemLoc;
                            }
                        }
                        item.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
