using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PunchmanEnemyController : MonoBehaviour
{
    public float detectionRadius = 5f; // 検知範囲の半径
    public float moveSpeed = 3f; // 移動速度
    public float punchForce = 10f; // パンチの力
    private GameObject player; // プレイヤーの参照
    public float punchRange = 1f; // パンチの攻撃範囲の半径
    public LayerMask PlayerLayer; // 敵のレイヤー
    public float punchOffset; // パンチの中心座標
    private Vector2 punchCenter; // パンチの中心座標
    private Vector2 punchDirection;

    public float rotationDuration = 0.5f;
    public GameObject HeartPlayer;
    private int collisionCountPlayer = 0;
    public AudioSource audioSource;

    private float smoothMoveDuration = 0.5f;

    private bool canPerformPunch = true; // パンチを実行できるかどうかのフラグ
    public float punchCooldown = 1f; // パンチのクールダウン時間
    private void Start()
    {
        // プレイヤーをタグで検索して参照を取得
        player = GameObject.FindGameObjectWithTag("Player");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        punchCenter = (Vector2)transform.position + punchOffset * (Vector2)transform.up;
        // プレイヤーの位置と敵の位置の距離を計算
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // プレイヤーが検知範囲内に入った場合
        if (distanceToPlayer <= detectionRadius && canPerformPunch)
        {
            // プレイヤーの方向を向く（滑らかに）
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.DORotateQuaternion(targetRotation, rotationDuration);

            // プレイヤーに滑らかに近づく
            transform.DOMove(player.transform.position, smoothMoveDuration);

            // パンチを実行
            StartCoroutine(PerformPunchWithCooldown());
        }
    }

    private void PerformPunch()
    {
        // 攻撃範囲内の敵を取得する
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(punchCenter, punchRange, PlayerLayer);

        // 敵に対してパンチをする
        foreach (Collider2D player in hitPlayers)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                Vector2 punchDirection = transform.up;

                collisionCountPlayer++;
                string heartObjectName = "Heart_player" + collisionCountPlayer.ToString("00");
                Transform heartObject = HeartPlayer.transform.Find(heartObjectName);

                if (heartObject != null)
                {
                    heartObject.gameObject.SetActive(false);
                    Debug.Log(heartObjectName + "を非アクティブにしました");
                }

                Debug.Log("敵に攻撃");
            }
        }

        // パンチアニメーションの再生
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("LargePunch");
        }
    }

    private void OnDrawGizmosSelected()
    {
        punchCenter = (Vector2)transform.position + punchOffset * (Vector2)transform.up;

        // 攻撃範囲のギズモを描画する
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(punchCenter, punchRange);

        // 検知範囲のギズモを描画する
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void PunchSE()
    {
        audioSource.Play();
    }

    private IEnumerator PerformPunchWithCooldown()
    {
        // パンチを実行
        PerformPunch();

        // パンチのクールダウン中にフラグを無効化
        canPerformPunch = false;

        // クールダウン時間を待つ
        yield return new WaitForSeconds(punchCooldown);

        // クールダウンが終了したらフラグを有効化
        canPerformPunch = true;
    }

    private void RotateCharacter(float targetAngle)
    {

        transform.DORotate(new Vector3(0f, 0f, targetAngle - 90f), rotationDuration)
            .SetEase(Ease.Linear);
    }

}

