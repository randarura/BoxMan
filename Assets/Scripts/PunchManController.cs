using UnityEngine;
using DG.Tweening;
using TMPro;
public class PunchManController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 characterDirection; // キャラクターの方向ベクトル
    public float dodgeDistance = 5f; // 回避時の移動距離
    public float dodgeDuration = 0.2f; // 回避時の移動時間
    public float dodgeSpeed = 10f;
    private bool dodging;
    private Vector2 dodgeDirection;
    private float dodgeTimer;
    public AudioSource audioSource;
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
    public GameObject ClickObject;
    public float deleteTime = 1.0f;
    private bool isClickRotationComplete = true;

    public AudioClip PunchingSE;

    public AudioClip HitSE;
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // 重力の影響を制御
        rb.gravityScale = 0f;
    }

    void Update()
    {
        Debug.Log(isClickRotationComplete);
        // 移動処理
        // WASDキーからの入力を取得
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // パンチアニメーションの再生
        Animator animator = GetComponent<Animator>();
        bool punchTriggerValue = animator.GetBool("Punch");

        // 移動方向に合わせてキャラクターの方向転換
        if (movement != Vector2.zero && !punchTriggerValue && isClickRotationComplete)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            RotateCharacterMove(angle);
        }
        // 回避動作
        // スペースキーの押下状態を取得
        bool spaceKeyDown = Input.GetKeyDown(KeyCode.Space);

        // スペースキーの押下状態によって回避動作を実行
        if (spaceKeyDown && !dodging)
        {
            // 回避方向を設定
            if (movement != Vector2.zero)
            {
                dodgeDirection = movement.normalized;
            }
            else
            {
                dodgeDirection = transform.up; // 前方向を回避方向とする
            }

            // 回避開始
            dodging = true;
            PerformDodge();
        }

        if (!dodging)
        {
            // 通常の移動処理
            MoveCharacter();
        }

        //パンチの処理
        punchCenter = (Vector2)transform.position + punchOffset * (Vector2)transform.up;
        if (Input.GetMouseButtonDown(0))
        {
            // パンチ時の処理
            PerformPunch();
        }
    }

    void PunchSE()
    {
        audioSource.PlayOneShot(PunchingSE);
    }


    private void PerformDodge()
    {
        rb.DOMove(rb.position + dodgeDirection * dodgeDistance, dodgeDuration)
            .SetEase(Ease.Linear)
            .OnComplete(EndDodge);
    }

    private void EndDodge()
    {
        dodging = false;
    }
    private void MoveCharacter()
    {
        rb.velocity = movement * moveSpeed;
    }

    private void PerformPunch()
    {
        // パンチの方向をクリックした方向に設定
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 3f;
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        punchDirection = (worldMousePosition - transform.position).normalized;

        // キャラクターの方向転換
        float angle = Mathf.Atan2(punchDirection.y, punchDirection.x) * Mathf.Rad2Deg;
        RotateCharacterClick(angle);

        // パンチアニメーションの再生
        Animator animator = GetComponent<Animator>();
        bool punchTriggerValue = animator.GetBool("Punch");
        if (animator != null && !punchTriggerValue)
        {
            animator.SetTrigger("Punch");
        }

        //Clickしたときのエフェクト生成
        GameObject clone = Instantiate(ClickObject, Camera.main.ScreenToWorldPoint(mousePosition),
                 Quaternion.identity);
        Destroy(clone, deleteTime);

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
                    //パンチヒット時に再生
                    audioSource.PlayOneShot(HitSE);

                    heartObject.gameObject.SetActive(false);
                    Debug.Log(heartObjectName + "を非アクティブにしました");

                    if (collisionCountEnemy == 3)
                    {
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

    }

    private void OnDrawGizmosSelected()
    {
        punchCenter = (Vector2)transform.position + punchOffset * (Vector2)transform.up;

        // 攻撃範囲のギズモを描画する
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(punchCenter, punchRange);
    }

    private void RotateCharacterMove(float targetAngle)
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle - 90f);
        transform.DORotateQuaternion(targetRotation, rotationDuration);
    }

    private void RotateCharacterClick(float targetAngle)
    {
        isClickRotationComplete = false;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle - 90f);
        transform.DORotateQuaternion(targetRotation, rotationDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => isClickRotationComplete = true);
    }

    // アニメーション終了時に呼ばれるコールバック関数
    void AnimationComplete()
    {
        Debug.Log("Animation Complete");
    }

}
