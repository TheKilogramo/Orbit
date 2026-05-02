using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System;
using CandyCoded.HapticFeedback;

[RequireComponent(typeof(Image))]
public class BetterButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static event Action<BetterButton> OnAnyButtonSelected;

    [Header("Events")]
    public bool isSelected = false;
    public bool ignoreSelectedEventCall = false;
    public UnityEvent TriggerEvent;
    public bool playSound = true;
    public bool vibrate = true;

    [Header("Scaling")]
    public bool scaleOnSelect = true;
    public bool staySelected = false;
    [Space]
    public float scaleMultiplier = 1.25f;
    public float scaleTime = .1f;

    private Vector3 startScale;

    [Header("Color")]
    public bool changeColorOnSelect;
    public Color selectedColor;

    private Color startColor;
    private Image image;

    public virtual void OnEnable()
    {
        OnAnyButtonSelected += AnyButtonSelected;

    }

    public virtual void OnDisable()
    {
        OnAnyButtonSelected -= AnyButtonSelected;

        DOTween.Kill(transform);
        transform.localScale = startScale;

    }


    public virtual void Start()
    {
        image = GetComponent<Image>();

        startColor = image.color;
        startScale = transform.localScale;
    }

    void AnyButtonSelected(BetterButton source)
    {
        if (source == this) return;

        if(isSelected)
            Deselect(true);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        //pointer still on the button
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            Deselect();
            TriggerEvent?.Invoke();
        }
        else //pointer aint on the button
        {
            Deselect();
        }

        if(playSound)
            AudioPlayer.PlayOneShot(GameManager.Instance.ButtonPressSound, .5f);

        if(vibrate)
        {
            HapticFeedback.MediumFeedback();
        }


    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Select();
    }



    public virtual void Deselect(bool forceDeselect = false)
    {
        if (staySelected == false || forceDeselect == true)
        {
            if (scaleOnSelect)
                transform.DOScale(startScale, scaleTime).SetUpdate(true);

            if (changeColorOnSelect)
                image.color = startColor;

            isSelected = false;
        }
    }

    public virtual void Select(bool triggerSelectEvent = false)
    {
        isSelected = true;

        if(ignoreSelectedEventCall == false)
            OnAnyButtonSelected?.Invoke(this);

        if (triggerSelectEvent)
            TriggerEvent?.Invoke();

        if (scaleOnSelect)
        {
            Vector3 targetScale = startScale * scaleMultiplier;
            transform.DOScale(targetScale, scaleTime).SetUpdate(true);
        }

        if (changeColorOnSelect)
        {
            image.color = selectedColor;
        }
    }
}
