using UnityEngine;
using TMPro;

class ContinueButton : MonoBehaviour {
    public static ContinueButton s;

    private bool hovered = false, pressed = false, finished = false;
    private float hoverOffset;
    private TMP_Text continueText;

    private void Awake() {
        s = this;
        continueText = GetComponentInChildren<TMP_Text>();
    }
    public void OnPointerEnter() {
        hovered = true;
        hoverOffset = Random.Range(0f, 1f);
        continueText.enabled = true;
    }
    public void OnPointerExit() {
        hovered = false;
        continueText.enabled = false;
    }
    public void OnPointerDown() {
        pressed = !finished;
    }
    public void OnPointerUp() {
        pressed = false;
        if (!finished) { EventManager.s.OnGameLoad?.Invoke(); }
        finished = true;
    }
    void Update() {
        if (hovered && !pressed) {
            HelperManager.s.FloatingHover(transform, 1.1f, hoverOffset, Vector3.zero);
        } else if (pressed) {
            HelperManager.s.FloatingHover(transform, 0.8f, hoverOffset, Vector3.zero);
        } else {
            HelperManager.s.FloatingHover(transform, 1f, hoverOffset, Vector3.zero, 0, 0);
        }
    }
}
