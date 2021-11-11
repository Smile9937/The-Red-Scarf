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
    List<Transform> enemiesToRespawnPos = new List<Transform>();
    [Header("Which Method")]
    [SerializeField] bool useNewMethod = false;
    [Header("Normal Method")]
    [SerializeField] Animator theRoomController;
    [SerializeField] int roomNumber = 0;
    [Header("New Method")]
    [SerializeField] CinemachineConfiner theConfiner;
    [SerializeField] PolygonCollider2D theRoomCollider;

    private void Awake()
    {
        foreach (var item in enemiesToRespawn)
        {
            if (item != null)
            {
                if (!enemiesToRespawn.Contains(item))
                {
                    enemiesToRespawn.Add(item);
                    enemiesToRespawnPos.Add(item.transform);
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
            Invoke("RespawnEnemies", Time.deltaTime);
        }
    }

    private void RespawnEnemies()
    {
        if (theRoomController.GetInteger("theRoomNumber") == roomNumber)
        {
            foreach (var item in enemiesToRespawn)
            {
                if (item != null)
                {
                    Debug.Log(item + " respawned!");
                    if (item.activeSelf == false)
                    {
                        item.GetComponentInChildren<Enemy>().gameObject.SetActive(true);
                        item.GetComponentInChildren<Enemy>().currentHealth = item.GetComponentInChildren<Enemy>().maxHealth;
                        foreach (var itemLoc in enemiesToRespawnPos)
                        {
                            if (enemiesToRespawn.IndexOf(item) == enemiesToRespawnPos.IndexOf(itemLoc))
                            {
                                item.transform.position = itemLoc.position;
                            }
                        }
                    }
                }
            }
        }
    }
}
