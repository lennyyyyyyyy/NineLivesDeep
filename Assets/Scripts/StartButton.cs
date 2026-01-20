using UnityEngine;
using TMPro;

class StartButton : MonoBehaviour {
    public static StartButton s;

    private bool hovered = false, pressed = false, finished = false;
    private float hoverOffset;
    private TMP_Text startText;

    private void Awake() {
        s = this;
        startText = GetComponentInChildren<TMP_Text>();
    }
    private void OnPointerEnter() {
        hovered = true;
        hoverOffset = Random.Range(0f, 1f);
        startText.enabled = true;
    }
    private void OnPointerExit() {
        hovered = false;
        startText.enabled = false;
    }
    private void OnPointerDown() {
        pressed = !finished;
    }
    private void OnPointerUp() {
        pressed = false;
        if (!finished) { EventManager.s.OnGameStart?.Invoke(); }
        finished = true;
    }
    private void Update() {
        if (hovered && !pressed) {
            HelperManager.s.FloatingHover(transform, 1.1f, hoverOffset, Vector3.zero);
        } else if (pressed) {
            HelperManager.s.FloatingHover(transform, 0.8f, hoverOffset, Vector3.zero);
        } else {
            HelperManager.s.FloatingHover(transform, 1f, hoverOffset, Vector3.zero, 0, 0);
        }
    }
    public void Reset() {
        finished = false;
    }
}
