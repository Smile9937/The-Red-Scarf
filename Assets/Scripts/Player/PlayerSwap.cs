using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwap : MonoBehaviour
{
    public RedScarfPlayer redScarf;
    public DressPlayer dress;
    void Start()
    {
        SetCurrentCharacter();
    }

    void Update()
    {
        //Swap Character
        if (InputManager.Instance.GetKeyDown(KeybindingActions.SwapCharacter))
        {
            GameManager.Instance.SwapCharacter();
            SetCurrentCharacter();
            //SetAnimator();
        }
    }

    public void SetCurrentCharacter()
    {
        if (GameManager.Instance.redScarf)
        {
            redScarf.gameObject.SetActive(true);
            dress.gameObject.SetActive(false);
            redScarf.transform.position = dress.transform.position;
        }
        else
        {
            dress.gameObject.SetActive(true);
            redScarf.gameObject.SetActive(false);
            dress.transform.position = redScarf.transform.position;
        }
    }
}
