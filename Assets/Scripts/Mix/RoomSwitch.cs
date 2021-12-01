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
            CancelInvoke("DespawnEnemies");
        }
    }

    private void OnTriggerExit2D(Collider2D collission)
    {
        if (collission.tag == "Player")
        {
            CancelInvoke("DespawnEnemies");
            Invoke("DespawnEnemies", Time.deltaTime + 2f);
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
                        item.GetComponentInChildren<Enemy>().currentHealth = item.GetComponentInChildren<Enemy>().maxHealth;
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
        if (theRoomController.GetInteger("theRoomNumber") != roomNumber)
        {
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
