using UnityEngine;
using TMPro;

public class Number : MonoBehaviour
{
	protected TMP_Text text;
    public void Init() {
		text = GetComponent<TMP_Text>();
        text.enabled = false;
    }
    public void SetNum(int num) {
        text.text = num.ToString();
        LeanTween.scale(gameObject, Vector3.one, Random.Range(0.5f, 1.5f)).setRepeat(-1).setLoopPingPong().setEase(LeanTweenType.easeInOutCubic);
        transform.eulerAngles = Vector3.forward * Random.Range(-30f, -10f);
        LeanTween.rotate(gameObject, Vector3.forward * Random.Range(10f, 30f), Random.Range(0.5f, 1.5f)).setRepeat(-1).setLoopPingPong().setEase(LeanTweenType.easeInOutCubic);
    }
    public void Enter() {
        text.enabled = true;
    }
    public void Exit() {
        text.enabled = false;
    }
}
