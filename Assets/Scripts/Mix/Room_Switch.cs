using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Switch : MonoBehaviour
{
    [SerializeField] Animator theRoomController;
    [SerializeField] int roomNumber = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            theRoomController.SetInteger("theRoomNumber", roomNumber);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            theRoomController.SetInteger("theRoomNumber", roomNumber);
        }
    }
}
