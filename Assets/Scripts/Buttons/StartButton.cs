using UnityEngine;
using TMPro;

public class StartButton : Button {
    public static StartButton s;
    private TMP_Text startText;

    protected override void OnPress() {
        base.OnPress();
        EventManager.s.OnGameStart?.Invoke();
    }
    protected override void Awake() {
        base.Awake();
        s = this;
        startText = GetComponentInChildren<TMP_Text>();
    }
    protected override void OnPointerEnter() {
        base.OnPointerEnter();
        startText.enabled = true;
    }
    protected override void OnPointerExit() {
        base.OnPointerExit();
        startText.enabled = false;
    }
}
