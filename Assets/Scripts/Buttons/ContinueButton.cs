using UnityEngine;
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
    protected override void OnPointerEnter() {
        base.OnPointerEnter();
        continueText.enabled = true;
    }
    protected override void OnPointerExit() {
        base.OnPointerExit();
        continueText.enabled = false;
    }
}
