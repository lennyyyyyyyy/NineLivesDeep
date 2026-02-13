using UnityEngine;
using UnityEngine.Rendering;
public class MainCamera : MonoBehaviour
{   
    public static MainCamera s;
	[System.NonSerialized]
	public bool locked = true;
	[System.NonSerialized]
	public float targetOrthographicSize = 2f;
	private Vector3 cameraShakeOffset = Vector3.zero;
	private float shakeTimer = 0f;

    private void updateZoom(float orthographicSize) {
        Camera.main.orthographicSize = orthographicSize;
    }
	public void ZoomTo(float endZoom, float time) {
		LeanTween.value(gameObject, updateZoom, Camera.main.orthographicSize, endZoom, time).setEase(LeanTweenType.easeInOutCubic);
	}
    public void ExitMotion() {
        LeanTween.cancel(gameObject);
        LeanTween.move(gameObject, Player.s.transform.position, 0.5f).setEase(LeanTweenType.easeInOutCubic);
		ZoomTo(0.01f, 0.5f);
    }
	private void SetupFloorIntro() {
        UIManager.s.ppv.weight = 0;
        HelperManager.s.DelayActionFrames(() => {
            transform.position = Vector3.zero;
            Camera.main.orthographicSize = 300f;
        }, 3);
        HelperManager.s.DelayActionFrames(() => { UIManager.s.ppv.weight = 1; }, 6);
	}	
    private void SetupStartScreen() {
        locked = true;
        transform.position = Vector3.zero;
        Camera.main.orthographicSize = 11.25f;
    }
    void Awake() {
        s = this;
    }
	private void Update() {
        if (GameManager.s.gameState == GameManager.GameState.GAME) {
            if (!locked) {	
                Vector3 playerPos = Floor.s.CoordToPos(Player.s.GetCoord().x, Player.s.GetCoord().y);
                Vector3 targetPos = playerPos - .25f * playerPos.normalized * Camera.main.orthographicSize + cameraShakeOffset;
                transform.position = Vector3.Lerp(transform.position, targetPos, 1-Mathf.Pow(0.65f, Time.deltaTime/.15f));
                updateZoom(Mathf.Lerp(Camera.main.orthographicSize, targetOrthographicSize, 1-Mathf.Pow(0.65f, Time.deltaTime/.15f)));
            }
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize + Input.mouseScrollDelta.y, 0.8f, Player.s.modifiers.vision);
            //shake at varying intervals
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0) {
                cameraShakeOffset = Player.s.modifiers.cameraShakeStrength * Random.insideUnitSphere;
                shakeTimer = Player.s.modifiers.cameraShakePeriod * Random.Range(0.5f, 1.5f);
            }
        }
	}
    private void OnEnable() {
        EventManager.s.OnFloorChangeBeforeIntro += SetupFloorIntro;
        EventManager.s.OnGameExit += SetupStartScreen;
        EventManager.s.OnGameWin += SetupStartScreen;
        EventManager.s.OnGameLose += SetupStartScreen;
        EventManager.s.OnReturnToStart += SetupStartScreen;
    }
    private void OnDisable() {
        EventManager.s.OnFloorChangeBeforeIntro -= SetupFloorIntro;
        EventManager.s.OnGameExit -= SetupStartScreen;
        EventManager.s.OnGameWin -= SetupStartScreen;
        EventManager.s.OnGameLose -= SetupStartScreen;
        EventManager.s.OnReturnToStart -= SetupStartScreen;
    }
}
