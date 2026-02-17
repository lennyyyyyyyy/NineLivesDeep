using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class Button : MonoBehaviour {
    private bool hovered = false, pressed = false, finished = false;
    private float hoverOffset;

    protected virtual void OnPress() {
        AudioManager.s.PlayEffect(AudioManager.s.press);
    }
    protected virtual void Awake() {
        HelperManager.s.SetupUIEventTriggers(gameObject, 
                                             new EventTriggerType[] { EventTriggerType.PointerEnter, EventTriggerType.PointerExit, EventTriggerType.PointerDown, EventTriggerType.PointerUp },
                                             new Action<PointerEventData>[] { OnPointerEnter, OnPointerExit, OnPointerDown, OnPointerUp });
    }
    protected virtual void OnPointerEnter(PointerEventData data) {
        hovered = true;
        hoverOffset = Random.Range(0f, 1f);
        AudioManager.s.PlayEffect(AudioManager.s.hover);
    }
    protected virtual void OnPointerExit(PointerEventData data) {
        hovered = false;
    }
    protected virtual void OnPointerDown(PointerEventData data) {
        pressed = !finished;
    }
    protected virtual void OnPointerUp(PointerEventData data) {
        pressed = false;
        if (!finished) OnPress();
        finished = true;
    }
    protected virtual void Update() {
        if (hovered && !pressed) {
            HelperManager.s.FloatingHover(transform, 1.1f, hoverOffset, Vector3.zero);
        } else if (pressed) {
            HelperManager.s.FloatingHover(transform, 0.8f, hoverOffset, Vector3.zero);
        } else {
            HelperManager.s.FloatingHover(transform, 1f, hoverOffset, Vector3.zero, 0, 0);
        }
        if (hovered) {
            UIManager.s.cursorInteract = true;
        }
    }
    public virtual void Reset() {
        finished = false;
    }
}
