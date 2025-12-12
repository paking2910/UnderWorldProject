using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float lifeTime = 3f;

    void Start()
    {
        // ทำลายตัวเองเมื่อหมดเวลา
        Destroy(gameObject, lifeTime);

        // สั่งกระสุนพุ่งไปข้างหน้า (ตามทิศที่หัวกระสุนหันอยู่)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // กระสุนห้ามตกพื้น
        rb.velocity = transform.right * speed;
    }


    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // ถ้าชนศัตรู
        EnemyFollowPlayer enemy = hitInfo.GetComponent<EnemyFollowPlayer>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // ทำลายกระสุน
        }

        // ถ้าชนกำแพง (สมมติว่ากำแพงมี Tag "Wall")A
        if (hitInfo.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}