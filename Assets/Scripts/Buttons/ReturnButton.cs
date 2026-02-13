using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ReturnButton : Button {
    public static ReturnButton s;
    private TMP_Text returnText;

    protected override void OnPress() {
        base.OnPress();
        EventManager.s.OnReturnToStart.Invoke();
    }
    protected override void Awake() {
        base.Awake();
        s = this;
        returnText = GetComponentInChildren<TMP_Text>();
    }
    protected override void OnPointerEnter(PointerEventData data) {
        base.OnPointerEnter(data);
        returnText.enabled = true;
    }
    protected override void OnPointerExit(PointerEventData data) {
        base.OnPointerExit(data);
        returnText.enabled = false;
    }
}

