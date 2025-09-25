using UnityEngine;
using TMPro;

public class Number : MonoBehaviour
{
    public float distance;
    Vector3 origLocal;
    public void init() {
        origLocal = transform.localPosition;
        gameObject.active = false;
    }
    public void setNum(int num) {
        GetComponent<TMP_Text>().text = num.ToString();
        LeanTween.scale(gameObject, Vector3.one, Random.Range(0.5f, 1.5f)).setRepeat(-1).setLoopPingPong().setEase(LeanTweenType.easeInOutCubic);
        transform.eulerAngles = Vector3.forward * Random.Range(-30f, -10f);
        LeanTween.rotate(gameObject, Vector3.forward * Random.Range(10f, 30f), Random.Range(0.5f, 1.5f)).setRepeat(-1).setLoopPingPong().setEase(LeanTweenType.easeInOutCubic);
    }
    public void enter() {
        gameObject.active = true;
    }
    public void exit() {
        gameObject.active = false;
    }
}
