using UnityEngine;
using DG.Tweening;
using TMPro;

public class AnimationController : MonoBehaviour
{
    public float punchForce = 10f; // パンチ時の力の大きさ
    public float punchRange = 1f; // パンチの攻撃範囲の半径
    public LayerMask enemyLayer; // 敵のレイヤー
    public float punchOffset; // パンチの中心座標
    private Vector2 punchCenter; // パンチの中心座標
    private Vector2 punchDirection;

    public GameObject HeartEnemy;
    private int collisionCountEnemy = 0;
    public float rotationDuration = 0.2f;

    public TextMeshProUGUI[] textObjects;

    public float initialScale = 1f; // 初期スケール
    public float targetScale = 2f;
    public float duration = 1f;
    public GameObject ClearText;
    public AudioClip ClearSE;
    AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        punchCenter = (Vector2)transform.position + punchOffset * (Vector2)transform.up;
        if (Input.GetMouseButtonDown(0))
        {
            // パンチ時の処理
            PerformPunch();
        }
    }

    private void PerformPunch()
    {

        // パンチの方向をクリックした方向に設定
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        punchDirection = (mousePosition - transform.position).normalized;

        // キャラクターの方向転換
        float angle = Mathf.Atan2(punchDirection.y, punchDirection.x) * Mathf.Rad2Deg;
        RotateCharacter(angle);

        // 攻撃範囲内の敵を取得する
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(punchCenter, punchRange, enemyLayer);

        // 敵に対してパンチをする
        foreach (Collider2D enemy in hitEnemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 punchDirection = transform.up;

                collisionCountEnemy++;
                string heartObjectName = "Heart_enemy" + collisionCountEnemy.ToString("00");
                Transform heartObject = HeartEnemy.transform.Find(heartObjectName);

                if (heartObject != null)
                {
                    heartObject.gameObject.SetActive(false);
                    Debug.Log(heartObjectName + "を非アクティブにしました");

                    if (collisionCountEnemy == 3)
                    {
                        audioSource.PlayOneShot(ClearSE);
                        Debug.Log("click ClearSE");

                        ClearText.gameObject.SetActive(true);

                        foreach (TextMeshProUGUI textObject in textObjects)
                        {
                            textObject.transform.localScale = new Vector3(initialScale, initialScale, initialScale); // 初期スケールを設定

                            // テキストを目標のスケールに拡大するTweenを作成
                            textObject.transform.DOScale(targetScale, duration)
                                .SetEase(Ease.OutQuart); // イージングの設定（例: OutQuart）

                            // オプション: アニメーション終了時に何か特定の処理を実行する場合
                            // TweenのOnCompleteコールバックを使用する
                            textObject.transform.DOScale(targetScale, duration)
                                .SetEase(Ease.OutQuart)
                                .OnComplete(AnimationComplete);
                        }
                        enemy.gameObject.SetActive(false);
                    };
                }

            }
        }

        // パンチアニメーションの再生
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Punch");
        }
    }

    private void OnDrawGizmosSelected()
    {
        punchCenter = (Vector2)transform.position + punchOffset * (Vector2)transform.up;

        // 攻撃範囲のギズモを描画する
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(punchCenter, punchRange);
    }

    private void RotateCharacter(float targetAngle)
    {

        transform.DORotate(new Vector3(0f, 0f, targetAngle - 90f), rotationDuration)
            .SetEase(Ease.Linear);
    }

    // アニメーション終了時に呼ばれるコールバック関数
    void AnimationComplete()
    {
        Debug.Log("Animation Complete");
    }
}
