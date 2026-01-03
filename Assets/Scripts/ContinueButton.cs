using UnityEngine;

class ContinueButton : MonoBehaviour
{
    public static ContinueButton s;
    private bool hovered = false, pressed = false, finished = false;
    private float hoverOffset;
    private void Awake() {
        s = this;
    }
    public void OnPointerEnter() {
        hovered = true;
        hoverOffset = Random.Range(0f, 1f);
    }
    public void OnPointerExit() {
        hovered = false;
    }
    public void OnPointerDown() {
        pressed = !finished;
    }
    public void OnPointerUp() {
        pressed = false;
        if (!finished) {

        }
        finished = true;
    }
    void Update() {
        if (hovered && !pressed) {
            UIManager.s.floatingHover(transform, 1.1f, hoverOffset, Vector3.zero);
        } else if (pressed) {
            UIManager.s.floatingHover(transform, 0.8f, hoverOffset, Vector3.zero);
        } else {
            UIManager.s.floatingHover(transform, 1f, hoverOffset, Vector3.zero, 0, 0);
        }
    }
}
