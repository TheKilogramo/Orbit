using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MenuEventSystemHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected List<Selectable> Selectables = new List<Selectable>();

    [Header("Animations")]
    [SerializeField] protected float _selectedAnimationScale = 1.1f;
    [SerializeField] protected float _scaleDuration = 0.25f;
    [Space]
    [SerializeField] AudioClip _selectSound;

    protected AudioSource _source;

    protected Dictionary<Selectable, Vector3> _scales = new Dictionary<Selectable, Vector3>();

    protected Tween _scaleUpTween;
    protected Tween _scaleDownTween;

    [Header("Behaviours")]
    [SerializeField] protected bool _animateOnSelect = false;
    [SerializeField] protected bool _keepSelectedScaled = false;

    protected Selectable _currentSelected;

    public virtual void Awake()
    {
        foreach (var selectable in Selectables)
        {
            AddSelectionListeners(selectable);
            _scales.Add(selectable, selectable.transform.localScale);
        }

        _source = GetComponent<AudioSource>();
    }

    public virtual void OnEnable()
    {
        //ensure all selectables are reset back to original size
        for (int i = 0; i < Selectables.Count; i++)
        {
            Selectables[i].transform.localScale = _scales[Selectables[i]];
        }
    }

    public virtual void OnDisable()
    {
        _scaleUpTween.Kill(true);
        _scaleDownTween.Kill(true);
    }

    public virtual void SelectFromScript(Selectable sel)
    {
        if (!allowSelection) return;
        if (sel == null) return;

        // Create fake event data for consistency
        BaseEventData data = new BaseEventData(EventSystem.current)
        {
            selectedObject = sel.gameObject
        };

        // Manually call your select logic
       // OnSelect(data);

        // Force Unity's EventSystem to actually "select" it
        EventSystem.current.SetSelectedGameObject(sel.gameObject);
    }

    public virtual void AddSelectable(Selectable sel)
    {
        if (Selectables.Contains(sel)) return;

        Selectables.Add(sel);

        _scales.Add(sel, sel.transform.localScale);

        AddSelectionListeners(sel);
    }

    public virtual void RemoveSelectable(Selectable sel)
    {
        if (Selectables.Contains(sel) == false) return;

        _scales.Remove(sel);

        Selectables.Remove(sel);
    }

    protected virtual void AddSelectionListeners(Selectable selectable)
    {
        //add listener
        EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        //add SELECT event
        EventTrigger.Entry SelectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Select
        };
        SelectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(SelectEntry);

        //add DESELECT event
        EventTrigger.Entry DeselectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Deselect
        };
        DeselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(DeselectEntry);

        //add ONPOINTERENTER event
        EventTrigger.Entry PointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        PointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(PointerEnter);

        //add ONPOINTEREXIT event
        EventTrigger.Entry PointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        PointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(PointerExit);
    }
    public bool allowSelection = false;
    public void OnSelect(BaseEventData eventData)
    {
        if (!allowSelection) return; // ignore early selects

        Selectable newSel = eventData.selectedObject.GetComponent<Selectable>();
        if (newSel == null) return;

        if (!_scales.ContainsKey(newSel))
            _scales[newSel] = newSel.transform.localScale;

        if (_currentSelected != null && _currentSelected != newSel)
        {
            _scaleDownTween?.Kill();
            _scaleDownTween = _currentSelected.transform.DOScale(
                _scales[_currentSelected],
                _scaleDuration
            ).SetUpdate(true);
        }

        _scaleUpTween?.Kill();
        if (!_scales.TryGetValue(newSel, out Vector3 originalScale))
        {
            // It's missing — skip ALL scaling safely
            _currentSelected = newSel;
            return;
        }

        Vector3 upScale = originalScale * _selectedAnimationScale;

        _scaleUpTween = newSel.transform.DOScale(upScale, _scaleDuration).SetUpdate(true);

        _currentSelected = newSel;

        if (_selectSound)
            _source.PlayOneShot(_selectSound);
    }



    public void OnDeselect(BaseEventData eventData)
    {
        Selectable sel = eventData.selectedObject.GetComponent<Selectable>();
        if (sel == null) return;

        if (!_scales.ContainsKey(sel))
        {
            _scales[sel] = sel.transform.localScale;
        }

        if (_keepSelectedScaled)
            return;

        _scaleDownTween?.Kill();
        _scaleDownTween = sel.transform.DOScale(
            _scales[sel],
            _scaleDuration
        ).SetUpdate(true);
    }


    public void OnPointerEnter(BaseEventData eventData)
    {
        if (_animateOnSelect) return;

        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            if (sel == null)
            {
                sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }

            pointerEventData.selectedObject = sel.gameObject;
        }

    }

    public void OnPointerExit(BaseEventData eventData)
    {
        if (_animateOnSelect) return;

        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            pointerEventData.selectedObject = null;
        }
    }
}
