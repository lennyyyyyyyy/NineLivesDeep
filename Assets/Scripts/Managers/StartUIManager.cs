using UnityEngine;

public class StartUIManager : MonoBehaviour {
    public static StartUIManager s;

    public GameObject startUIGroup, 
                      nine,
                      lives,
                      deep,
                      startbutton,
                      continuebutton;

    private void Awake() {
        s = this;
    }
    private void Start() {
        Reset();
    }
    private void Update() {
        if (GameManager.s.gameState == GameManager.GameState.START) {
            nine.transform.localPosition = new Vector3(nine.transform.localPosition.x, 
                ConstantsManager.s.buttonIdleStrength * Mathf.Sin(ConstantsManager.s.buttonIdleSpeed * Time.time), 0);
            lives.transform.localPosition = new Vector3(lives.transform.localPosition.x,
                -15.0f + ConstantsManager.s.buttonIdleStrength * Mathf.Sin(ConstantsManager.s.buttonIdleSpeed * 0.9f * Time.time + 2), 0);
            deep.transform.localPosition = new Vector3(deep.transform.localPosition.x,
                ConstantsManager.s.buttonIdleStrength * Mathf.Sin(ConstantsManager.s.buttonIdleSpeed * 1.1f * Time.time + 4), 0);
            startbutton.transform.localPosition = new Vector3(startbutton.transform.localPosition.x,
                90.2f + 0.5f * ConstantsManager.s.buttonIdleStrength * Mathf.Sin(ConstantsManager.s.buttonIdleSpeed * 0.8f * Time.time + 3), 0);
            continuebutton.transform.localPosition = new Vector3(continuebutton.transform.localPosition.x,
                106.0f + 0.5f * ConstantsManager.s.buttonIdleStrength * Mathf.Sin(ConstantsManager.s.buttonIdleSpeed * 0.8f * Time.time + 1.5f), 0);
        }
    }
    private void Reset() {
        ContinueButton.s.Reset();
        StartButton.s.Reset();
    }
    private void OnGameStart() {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, (float f) => {
            startUIGroup.GetComponent<CanvasGroup>().alpha = f;
        }, 1, 0, ConstantsManager.s.gameUITransitionDuration).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() => {
            startUIGroup.SetActive(false);
        });
    }
    private void OnGameExit() {
        Reset();
        LeanTween.cancel(gameObject);
        startUIGroup.SetActive(true);
        LeanTween.value(gameObject, (float f) => {
            startUIGroup.GetComponent<CanvasGroup>().alpha = f;
        }, 0, 1, ConstantsManager.s.gameUITransitionDuration).setEase(LeanTweenType.easeInOutCubic);
    }
    private void OnReturnToStart() {
        Reset();
        LeanTween.cancel(gameObject);
        startUIGroup.SetActive(true);
        LeanTween.value(gameObject, (float f) => {
            startUIGroup.GetComponent<CanvasGroup>().alpha = f;
        }, 0, 1, ConstantsManager.s.gameUITransitionDuration).setEase(LeanTweenType.easeInOutCubic);
    }
    private void OnEnable() {
        EventManager.s.OnGameStart += OnGameStart;
        EventManager.s.OnGameLoad += OnGameStart;
        EventManager.s.OnGameExit += OnGameExit;
        EventManager.s.OnReturnToStart += OnReturnToStart;
    }
    private void OnDisable() {
        EventManager.s.OnGameStart -= OnGameStart;
        EventManager.s.OnGameLoad -= OnGameStart;
        EventManager.s.OnGameExit -= OnGameExit;
        EventManager.s.OnReturnToStart -= OnReturnToStart;
    }
}
