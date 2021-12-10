using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTravel : MonoBehaviour
{
    [SerializeField] MainMenuTravelPoints currentPosition;
    [SerializeField] MainMenuTravelPoints leftPosition;
    [SerializeField] MainMenuTravelPoints upperPosition;
    [SerializeField] MainMenuTravelPoints rightPosition;
    [SerializeField] MainMenuTravelPoints downwardsPosition;
    [SerializeField] MainMenuTravelPoints selectedPosition;
    public bool isInTransition;

    bool doneInputThisUpdate;

    InputManager theInputManager;

    // Start is called before the first frame update
    void Start()
    {
        currentPosition.SetMenuPosition();
        theInputManager = FindObjectOfType<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPosition.state == MainMenuTravelPoints.ButtonState.KeyBinder && isInTransition && !theInputManager.waitingForInput)
        {
            Debug.Log("EXIT");
            isInTransition = false;
            currentPosition.SetMenuPosition();
            doneInputThisUpdate = true;
        }
        if (isInTransition)
            return;
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Down) && downwardsPosition != currentPosition && downwardsPosition != null && !doneInputThisUpdate)
        {
            currentPosition.UnSetMenuPosition();
            currentPosition = downwardsPosition;
            currentPosition.SetMenuPosition();
            doneInputThisUpdate = true;
        }
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Right) && rightPosition != currentPosition && rightPosition != null && !doneInputThisUpdate)
        {
            currentPosition.UnSetMenuPosition();
            currentPosition = rightPosition;
            currentPosition.SetMenuPosition();
            doneInputThisUpdate = true;
        }
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Left) && leftPosition != currentPosition && leftPosition != null && !doneInputThisUpdate)
        {
            currentPosition.UnSetMenuPosition();
            currentPosition = leftPosition;
            currentPosition.SetMenuPosition();
            doneInputThisUpdate = true;
        }
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Up) && upperPosition != currentPosition && upperPosition != null && !doneInputThisUpdate)
        {
            currentPosition.UnSetMenuPosition();
            currentPosition = upperPosition;
            currentPosition.SetMenuPosition();
            doneInputThisUpdate = true;
        }
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Attack) && !doneInputThisUpdate)
        {
            if (selectedPosition != null || !(currentPosition.state == MainMenuTravelPoints.ButtonState.Selectable))
            {
                currentPosition.ActivateMenuPosition();
            }
        }
        doneInputThisUpdate = false;
    }

    public void SetSelectedMenuPosition()
    {
        currentPosition.UnSetMenuPosition();
        currentPosition = selectedPosition;
        currentPosition.SetMenuPosition();
    }
    public void ReturnMenuControl()
    {
        isInTransition = false;
    }

    public void SetMenuPositions(MainMenuTravelPoints left, MainMenuTravelPoints upper, MainMenuTravelPoints right, MainMenuTravelPoints downwards, MainMenuTravelPoints selected)
    {
        leftPosition = left;
        upperPosition = upper;
        rightPosition = right;
        downwardsPosition = downwards;
        selectedPosition = selected;
    }
}
