using UnityEngine;

public class PsychicEye : MonoBehaviour
{
    public bool placed = false;
    private static Vector3 center = new Vector3(.1249f, .1877f, 0);
    public Vector2Int mineDirection = Vector2Int.zero;
    private float timer = 0;
    private void Start() {
        transform.localScale = .062f * Vector3.one;
    }

    private void Update() {
        timer += Time.deltaTime;
        if (timer > .6f) {
            timer = 0;
            UpdatePosition();
        }
    }

    public void UpdatePosition() {
        if (placed) {
            Vector3 offset = Quaternion.AngleAxis(Random.Range(-10, 11), Vector3.forward) * Vector3.Normalize(new Vector3(mineDirection.x, mineDirection.y, 0)) * .062f;
            LeanTween.moveLocal(gameObject, center + offset, .15f);
        } else {
            LeanTween.moveLocal(gameObject, center + Random.insideUnitSphere * .062f, .15f);
        }
    }
}
