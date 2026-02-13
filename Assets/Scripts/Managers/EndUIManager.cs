using UnityEngine;
using TMPro;

public class EndUIManager : MonoBehaviour {
    public GameObject endUIGroup,
                      returnButton;
    public TMP_Text endText;

    private void Update() {
        returnButton.transform.localPosition = new Vector3(returnButton.transform.localPosition.x,
            0.5f * ConstantsManager.s.buttonIdleStrength * Mathf.Sin(ConstantsManager.s.buttonIdleSpeed * 0.8f * Time.time + 3), 0);
    }
    private void OnGameWin() {
        LeanTween.cancel(gameObject);
        endUIGroup.SetActive(true);
        endText.text = "you win :)";
        LeanTween.value(gameObject, (float f) => {
            endUIGroup.GetComponent<CanvasGroup>().alpha = f;
        }, 0, 1, ConstantsManager.s.gameUITransitionDuration).setEase(LeanTweenType.easeInOutCubic);
        ReturnButton.s.Reset();
    }
    private void OnGameLose() {
        LeanTween.cancel(gameObject);
        endUIGroup.SetActive(true);
        endText.text = "you lost :(";
        LeanTween.value(gameObject, (float f) => {
            endUIGroup.GetComponent<CanvasGroup>().alpha = f;
        }, 0, 1, ConstantsManager.s.gameUITransitionDuration).setEase(LeanTweenType.easeInOutCubic);
        ReturnButton.s.Reset();
    }
    private void OnReturnToStart() {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, (float f) => {
            endUIGroup.GetComponent<CanvasGroup>().alpha = f;
        }, 1, 0, ConstantsManager.s.gameUITransitionDuration).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() => {
            endUIGroup.SetActive(false);
        });
    }
    private void OnEnable() {
        EventManager.s.OnGameWin += OnGameWin;
        EventManager.s.OnGameLose += OnGameLose;
        EventManager.s.OnReturnToStart += OnReturnToStart;
    }
    private void OnDisable() {
        EventManager.s.OnGameWin -= OnGameWin;
        EventManager.s.OnGameLose -= OnGameLose;
        EventManager.s.OnReturnToStart -= OnReturnToStart;
    }
}
