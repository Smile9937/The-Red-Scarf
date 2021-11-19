using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator theAnimator;
    [Header("Options Menu")]
    [SerializeField] List<GameObject> objectsToMove = new List<GameObject>();
    [SerializeField] List<Vector2> targetsObjectsMoveToOptions = new List<Vector2>();
    [SerializeField] List<Vector2> targetsObjectsMoveToNormal = new List<Vector2>();

    private void Awake()
    {
        SwitchMenu(0);
    }

    public void SwitchMenu(int theState)
    {
        switch (theState)
        {
            case 0:
                theAnimator.SetInteger("menuValue", 0);
                break;
            case 1:
                theAnimator.SetInteger("menuValue", 1);
                break;
            case 2:
                theAnimator.SetInteger("menuValue", 2);
                break;
            case 3:
                theAnimator.SetInteger("menuValue", 3);
                break;
            case 4:
                theAnimator.SetInteger("menuValue", 4);
                break;
            default:
                break;
        }
    }
