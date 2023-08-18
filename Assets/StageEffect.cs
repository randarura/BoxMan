using UnityEngine;

public class StageEffect : MonoBehaviour
{
    public float fadeSpeed = 0.5f; // 色の変化速度

    private SpriteRenderer spriteRenderer;
    private Color originalColor; // オリジナルの色

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        float t = Mathf.PingPong(Time.time * fadeSpeed, 1f); // 0～1の範囲を往復するパラメータ

        // パラメータに基づいて色を補間して設定
        Color newColor = Color.Lerp(Color.white, originalColor, t);
        spriteRenderer.color = newColor;
    }
}
