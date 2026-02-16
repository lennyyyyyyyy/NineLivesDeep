using UnityEngine;
using TMPro;

public class Number : MonoBehaviour {
	private Map map;
    [System.NonSerialized]
	public Vector2Int coord;
    [System.NonSerialized]
    public int num;
	private TMP_Text text;
    private Color[] colors;

    private void Awake() {
        colors = new Color[] {
            ConstantsManager.s.white,
            ConstantsManager.s.blue,
            ConstantsManager.s.green,
            ConstantsManager.s.red,
            ConstantsManager.s.darkBlue,
            ConstantsManager.s.darkRed,
            ConstantsManager.s.cyan,
            ConstantsManager.s.black,
            ConstantsManager.s.gray
        };
    }
	private void Update() {
		if (Random.value < 1 - Mathf.Pow(1 - Player.s.modifiers.mapNumberDisappearChancePerSecond, Time.deltaTime)) {
			Destroy(gameObject);
		}
	}
    public void Init(Map map, Vector2Int coord) {
		this.map = map;
		this.coord = coord;
		text = GetComponent<TMP_Text>();
        text.enabled = false;
        gameObject.SetActive(Player.s.mapNumberActive);
    }
    public void SetNum(int num) {
        this.num = num;
        text.text = num.ToString();
        //LeanTween.scale(gameObject, Vector3.one, Random.Range(0.5f, 1.5f)).setRepeat(-1).setLoopPingPong().setEase(LeanTweenType.easeInOutCubic);
        transform.eulerAngles = Vector3.forward * Random.Range(-20f, 20f);
        //LeanTween.rotate(gameObject, Vector3.forward * Random.Range(10f, 30f), Random.Range(0.5f, 1.5f)).setRepeat(-1).setLoopPingPong().setEase(LeanTweenType.easeInOutCubic);
        if (num >= 0 && num < colors.Length) {
            text.color = colors[num];
        } else {
            text.color = ConstantsManager.s.white;
        }
    }
    public void Enter() {
        text.enabled = true;
    }
    public void Exit() {
        text.enabled = false;
    }
	private void OnDestroy() {
		map.RemoveNumber(coord.x, coord.y);
	}
}
