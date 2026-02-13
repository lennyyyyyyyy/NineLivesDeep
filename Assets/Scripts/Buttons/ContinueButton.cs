using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ContinueButton : Button {
    public static ContinueButton s;
    private TMP_Text continueText;

    protected override void OnPress() {
        base.OnPress();
        EventManager.s.OnGameLoad?.Invoke();
    }
    protected override void Awake() {
        base.Awake();
        s = this;
        continueText = GetComponentInChildren<TMP_Text>();
    }
    protected override void OnPointerEnter(PointerEventData data) {
        base.OnPointerEnter(data);
        continueText.enabled = true;
    }
    protected override void OnPointerExit(PointerEventData data) {
        base.OnPointerExit(data);
        continueText.enabled = false;
    }
}
