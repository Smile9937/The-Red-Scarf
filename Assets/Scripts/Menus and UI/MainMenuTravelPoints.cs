using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MainMenuTravelPoints : MonoBehaviour
{
    [SerializeField] MainMenuTravel theMainTravel = null;
    public MainMenuTravelPoints leftPosition;
    public MainMenuTravelPoints upperPosition;
    public MainMenuTravelPoints rightPosition;
    public MainMenuTravelPoints downwardsPosition;
    public MainMenuTravelPoints pressedPosition;
    [SerializeField] Sprite deactivatedSprite;
    [SerializeField] Sprite activatedSprite;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] SpriteRenderer theSpriteRenderer;

    [SerializeField] bool isSceneChanging = false;
    [SerializeField] Slider theSlider;

    public ButtonState state;
    public enum ButtonState
    {
        Selectable,
        Slider,
        Button,
        KeyBinder,
        Selected,
    }
    
    public UnityEvent theObjectToInteractWith;

    private void Start()
    {
        theSpriteRenderer = GetComponent<SpriteRenderer>();
        UnSetMenuPosition();
    }

    public void UnSetMenuPosition()
    {
        theSpriteRenderer.sprite = deactivatedSprite;
    }

    public void SetMenuPosition()
    {
        theSpriteRenderer.sprite = activatedSprite;
        theMainTravel.SetMenuPositions(leftPosition, upperPosition, rightPosition, downwardsPosition, pressedPosition);
    }

    public void ActivateMenuPosition()
    {
        switch (state)
        {
            case ButtonState.Selectable:
                StartCoroutine("UpdateSelectablePressed");
                theMainTravel.isInTransition = true;
                break;
            case ButtonState.Button:
                StartCoroutine("UpdateButtonPressed");
                theMainTravel.isInTransition = true;
                break;
            case ButtonState.Slider:
                StartCoroutine("UpdateSliderSelected");
                theMainTravel.isInTransition = true;
                break;
            case ButtonState.KeyBinder:
                theSpriteRenderer.sprite = selectedSprite;
                theMainTravel.isInTransition = true;
                theObjectToInteractWith.Invoke();
                break;
            default:
                break;
        }
    }

    private IEnumerator UpdateSelectablePressed()
    {
        state = ButtonState.Selected;
        theSpriteRenderer.sprite = selectedSprite;
        yield return new WaitForSeconds(0.3f);
        state = ButtonState.Selectable;
        theMainTravel.isInTransition = false;
        theMainTravel.SetSelectedMenuPosition();
    }


    private IEnumerator UpdateButtonPressed()
    {
        state = ButtonState.Selected;
        theSpriteRenderer.sprite = selectedSprite;
        yield return new WaitForSeconds(0.35f);
        theSpriteRenderer.sprite = activatedSprite;
        if (!isSceneChanging)
            theMainTravel.isInTransition = false;
        theObjectToInteractWith.Invoke();
        state = ButtonState.Button;
    }

    private IEnumerator UpdateSliderSelected()
    {
        while (!InputManager.Instance.GetKey(KeybindingActions.Attack) && !InputManager.Instance.GetKey(KeybindingActions.Special))
        {
            if (InputManager.Instance.GetKey(KeybindingActions.Right))
            {
                theSlider.value += 0.01f;
                yield return new WaitForSeconds(Time.deltaTime + 0.1f);
            }
            else if (InputManager.Instance.GetKey(KeybindingActions.Left))
            {
                theSlider.value -= 0.01f;
                yield return new WaitForSeconds(Time.deltaTime + 0.1f);
            }
            else
            {
                yield return new WaitForSeconds(Time.deltaTime + 0.01f);
            }
        }
        theSpriteRenderer.sprite = activatedSprite;
        SetMenuPosition();
        UnSetMenuPosition();
        StopAllCoroutines();
    }
}
