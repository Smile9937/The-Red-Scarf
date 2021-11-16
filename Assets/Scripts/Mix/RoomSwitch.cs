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
    private List<GameObject> enemiesToRespawnObj = new List<GameObject>();
    private List<Vector2> enemiesToRespawnPos = new List<Vector2>();
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
                }
                if (!enemiesToRespawnObj.Contains(item.GetComponentInChildren<Enemy>().gameObject))
                {
                    enemiesToRespawnObj.Add(item.GetComponentInChildren<Enemy>().gameObject);
                }
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
                RespawnEnemies();
                CancelInvoke("DeactivateEnemies");
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
        if (collission.tag == "Player" && !useNewMethod)
        {
            Invoke("DeactivateEnemies", Time.deltaTime + 5f);
        }
    }

    private void DeactivateEnemies()
    {
        if (theRoomController.GetInteger("theRoomNumber") != roomNumber)
        {
            foreach (var item in enemiesToRespawn)
            {
                if (item != null)
                {
                    if (item.activeSelf == true)
                    {
                        foreach (var itemObj in enemiesToRespawnObj)
                        {
                            itemObj.GetComponent<Enemy>().currentHealth = itemObj.GetComponent<Enemy>().maxHealth;
                            foreach (var itemLoc in enemiesToRespawnPos)
                            {
                                if (enemiesToRespawnObj.IndexOf(itemObj) == enemiesToRespawnPos.IndexOf(itemLoc))
                                {
                                    itemObj.transform.position = new Vector2(itemLoc.x, itemLoc.y);
                                }
                            }
                        }
                        item.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void RespawnEnemies()
    {
        if (theRoomController.GetInteger("theRoomNumber") != roomNumber)
        {
            foreach (var item in enemiesToRespawn)
            {
                if (item != null)
                {
                    if (item.activeSelf == false)
                    {
                        item.gameObject.SetActive(true);
                        foreach (var itemObj in enemiesToRespawnObj)
                        {
                            itemObj.GetComponent<Enemy>().currentHealth = itemObj.GetComponent<Enemy>().maxHealth;
                            foreach (var itemLoc in enemiesToRespawnPos)
                            {
                                if (enemiesToRespawnObj.IndexOf(itemObj) == enemiesToRespawnPos.IndexOf(itemLoc))
                                {
                                    itemObj.transform.position = new Vector2(itemLoc.x, itemLoc.y);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
