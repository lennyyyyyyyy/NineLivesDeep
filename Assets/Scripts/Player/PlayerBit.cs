using UnityEngine;
using Random = UnityEngine.Random;
using System;
public class PlayerBit : MonoBehaviour
{
    private Vector2Int coord;
    private Color baseColor;
    private Vector3 basePosition, referencePos;
    private SpriteRenderer sr;
    public float range, variance, noiseScale, noiseStrength, noiseSpeed;
    private float currNoiseStrength = 0;
    public void Init(int x, int y) {
        sr = GetComponent<SpriteRenderer>();
        coord = new Vector2Int(x, y);
        transform.localScale = Vector3.one / (float) Player.s.texWidth;
        transform.parent = Player.s.transform;
        
        basePosition = new Vector3(-0.5f + (2*x + 1f)/(2*Player.s.texWidth), -0.5f + (2*y + 1f)/(2*Player.s.texHeight), 0);
        sr.enabled = false;
    }
    private void OnPlayerDie() {
        sr.enabled = true;
        baseColor = Player.s.sr.sprite.texture.GetPixel((int) Player.s.sr.sprite.rect.x + coord.x, (int) Player.s.sr.sprite.rect.y + coord.y);
        LeanTween.value(gameObject, (Color c) => {
            sr.color = c;
        }, baseColor, new Color(0.85f,0.69f,0.59f,baseColor.a), ConstantsManager.s.playerReviveDuration).setEase(LeanTweenType.easeOutCubic);
        LeanTween.value(gameObject, (float f) => {
            currNoiseStrength = f;
        }, 0, noiseStrength, ConstantsManager.s.playerReviveDuration).setEase(LeanTweenType.easeOutQuint);
        LeanTween.value(gameObject, (Vector3 v) => {
            referencePos = v;
        }, basePosition, basePosition * range + new Vector3(Random.Range(-variance, variance), Random.Range(-variance, variance), 0), ConstantsManager.s.playerReviveDuration)
            .setEase(LeanTweenType.easeOutQuint);
    }
    private void OnPlayerRevive() {
        baseColor = Player.s.sr.sprite.texture.GetPixel((int) Player.s.sr.sprite.rect.x + coord.x, (int) Player.s.sr.sprite.rect.y + coord.y);
        LeanTween.value(gameObject, (Color c) => {
            sr.color = c;
        }, sr.color, baseColor, ConstantsManager.s.playerReviveDuration).setEase(LeanTweenType.easeInCubic);
        LeanTween.value(gameObject, (float f) => {
            currNoiseStrength = f;
        }, noiseStrength, 0, ConstantsManager.s.playerReviveDuration).setEase(LeanTweenType.easeInQuint);
        LeanTween.value(gameObject, (Vector3 v) => {
            referencePos = v;
        }, referencePos, basePosition, ConstantsManager.s.playerReviveDuration).setEase(LeanTweenType.easeInQuint).setOnComplete(()=>{
            sr.enabled = false;
        });
    }
    private void OnEnable() {
        EventManager.s.OnPlayerDie += OnPlayerDie;
        EventManager.s.OnPlayerRevive += OnPlayerRevive;
    }
    private void OnDisable() {
        EventManager.s.OnPlayerDie -= OnPlayerDie;
        EventManager.s.OnPlayerRevive -= OnPlayerRevive;
    }
    private void Update() {
        if (sr.enabled) {
            transform.localPosition = referencePos + currNoiseStrength * new Vector3(Mathf.PerlinNoise(noiseScale * (coord.x + Time.time * noiseSpeed), 
                                                                                                       noiseScale * (coord.y + Time.time * noiseSpeed))-0.5f,
                                                                                     Mathf.PerlinNoise(noiseScale * (coord.x + Time.time * noiseSpeed) + 1000, 
                                                                                                       noiseScale * (coord.y + Time.time * noiseSpeed) + 1000)-0.5f, 0);

        }
    }
}
