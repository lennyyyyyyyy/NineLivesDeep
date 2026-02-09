using UnityEngine;
using TMPro;

public class Button : MonoBehaviour {
    private bool hovered = false, pressed = false, finished = false;
    private float hoverOffset;

    protected virtual void OnPress() {}
    protected virtual void Awake() {}
    protected virtual void OnPointerEnter() {
        hovered = true;
        hoverOffset = Random.Range(0f, 1f);
    }
    protected virtual void OnPointerExit() {
        hovered = false;
    }
    protected virtual void OnPointerDown() {
        pressed = !finished;
    }
    protected virtual void OnPointerUp() {
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
