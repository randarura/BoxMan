using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public float bounceForce = 10f;
    public float minBounceForce = 5f;
    public float maxBounceForce = 20f;
    public float maxVelocityMagnitude = 10f;

    public GameObject particlePrefab; // パーティクルのプレハブ

    private Vector3 particlePosition;

    public float offsetDistance;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 collisionNormal = collision.contacts[0].normal;

            // 壁の方向によって法線ベクトルを調整
            if (collision.gameObject.CompareTag("TopWall"))
            {
                collisionNormal = new Vector2(0f, -1f);
            }
            else if (collision.gameObject.CompareTag("BottomWall"))
            {
                collisionNormal = new Vector2(0f, 1f);
            }
            else if (collision.gameObject.CompareTag("LeftWall"))
            {
                collisionNormal = new Vector2(1f, 0f);
            }
            else if (collision.gameObject.CompareTag("RightWall"))
            {
                collisionNormal = new Vector2(-1f, 0f);
            }

            // 速度の速さに応じてBounceForceを計算
            float velocityMagnitude = rb.velocity.magnitude;
            float normalizedMagnitude = Mathf.Clamp01(velocityMagnitude / maxVelocityMagnitude);
            float interpolatedBounceForce = Mathf.Lerp(minBounceForce, maxBounceForce, normalizedMagnitude);

            // BounceForceを適用
            Vector2 reflectVector = Vector2.Reflect(rb.velocity.normalized, collisionNormal);
            rb.velocity = reflectVector * interpolatedBounceForce;

            // パーティクルを生成
            if (particlePrefab != null)
            {
                Vector2 offset = collisionNormal * offsetDistance;
                Vector2 particlePosition = collision.contacts[0].point - offset;
                Quaternion particleRotation = Quaternion.LookRotation(-collisionNormal, Vector3.back);
                Instantiate(particlePrefab, particlePosition, particleRotation);
            }
        }

    }

    private void GenerateCollisionParticle(Vector2 position)
    {
        // パーティクルのプレハブをインスタンス化して生成
        GameObject particleObject = Instantiate(particlePrefab, position, Quaternion.identity);
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

        // パーティクルが再生された後、自動的に破棄する
        Destroy(particleObject, particleSystem.main.duration);
        Debug.Log("パーティクル生成");
    }
}
