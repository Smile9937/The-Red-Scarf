using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPopup : MonoBehaviour
{
    [SerializeField] SpriteRenderer theSpriteRenderer;
    [SerializeField] GameObject theSpriteRendererObject;
    [SerializeField] KeybindingActions theKeyBind;

    [SerializeField] GameObject theTarget;
    [SerializeField] Vector2 targetOffset;
    [SerializeField] DoNotRespawn theAntiRespawnScript;

    // Start is called before the first frame update
    void Start()
    {
        theSpriteRenderer.color = new Color(1, 1, 1, 0);
        UpdateSpriteOfKeyBinding();
    }

    private void OnEnable()
    {
        GameEvents.Instance.onKeyChange += UpdateSpriteOfKeyBinding;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onKeyChange -= UpdateSpriteOfKeyBinding;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == theTarget)
        {
            theSpriteRenderer.color = new Color(1, 1, 1, 1);
            theSpriteRendererObject.transform.position = new Vector2(theTarget.transform.position.x + targetOffset.x, theTarget.transform.position.y + targetOffset.y);
            StartCoroutine("HandleTrackingTarget");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == theTarget)
        {
            theSpriteRenderer.color = new Color(1, 1, 1, 0);
            theSpriteRendererObject.transform.position = transform.position;
            StopAllCoroutines();
        }
    }
    private IEnumerator HandleTrackingTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            theSpriteRendererObject.transform.position = new Vector2(theTarget.transform.position.x + targetOffset.x, theTarget.transform.position.y + targetOffset.y);
            if (InputManager.Instance.GetKey(theKeyBind))
            {
                if (theAntiRespawnScript != null)
                    theAntiRespawnScript.Collected();
                Destroy(gameObject);
            }
        }
    }

    private void UpdateSpriteOfKeyBinding()
    {
        foreach (var keyCodeManager in InputManager.Instance.keybinds)
        {
            if (keyCodeManager.keybindingAction == theKeyBind)
            {
                foreach (var keycodeImage in InputImages.Instance.keyCodeSprites)
                {
                    if (keyCodeManager.keyCode == keycodeImage.keyCode)
                    {
                        theSpriteRenderer.sprite = keycodeImage.activeSprite;
                        return;
                    }
                }
            }
        }
    }
}
