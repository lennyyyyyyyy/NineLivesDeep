using UnityEngine;

public class StartUIManager : MonoBehaviour
{
    public static StartUIManager s;

    public GameObject startUIGroup, 
                      nine,
                      lives,
                      deep,
                      startbutton,
                      continuebutton;

    private float startIdleStrength = 15f, startIdleSpeed = 2f;
    private void Awake() {
        s = this;
    }
    private void Start() {
        continuebutton.SetActive(SaveManager.s.saveDataValid);
    }
    private void Update() {
        if (GameManager.s.gameState == GameManager.GameState.START) {
            nine.transform.localPosition = new Vector3(nine.transform.localPosition.x, startIdleStrength * Mathf.Sin(startIdleSpeed * Time.time), 0);
            lives.transform.localPosition = new Vector3(lives.transform.localPosition.x, -15.0f + startIdleStrength * Mathf.Sin(startIdleSpeed * 0.9f * Time.time + 2), 0);
            deep.transform.localPosition = new Vector3(deep.transform.localPosition.x, startIdleStrength * Mathf.Sin(startIdleSpeed * 1.1f * Time.time + 4), 0);
            startbutton.transform.localPosition = new Vector3(startbutton.transform.localPosition.x, 90.2f + 0.5f * startIdleStrength * Mathf.Sin(startIdleSpeed * 0.8f * Time.time + 3), 0);
            continuebutton.transform.localPosition = new Vector3(continuebutton.transform.localPosition.x, 106.0f + 0.5f * startIdleStrength * Mathf.Sin(startIdleSpeed * 0.8f * Time.time + 1.5f), 0);
        }
    }
    private void OnGameStart() {
        LeanTween.value(gameObject, (float f) => {
            startUIGroup.GetComponent<CanvasGroup>().alpha = f;
        }, 1, 0, ConstantsManager.s.gameStartTransitionDuration).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() => {
            startUIGroup.SetActive(false);
        });
    }
    private void OnEnable() {
        EventManager.s.OnGameStart += OnGameStart;
    }
    private void OnDisable() {
        EventManager.s.OnGameStart -= OnGameStart;
    }
}
