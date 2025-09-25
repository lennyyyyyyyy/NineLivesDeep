using UnityEngine;

class SecondaryCamera : MonoBehaviour
{
    private Camera cam;
    void Start() {
        cam = GetComponent<Camera>();
    }
    void Update() {
        cam.orthographicSize = Camera.main.orthographicSize;
    }
}