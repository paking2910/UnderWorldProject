using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
    // ตัวแปรที่ใช้สำหรับควบคุมความเร็ว
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    // อ้างอิงถึง GameObject ของ Player
    public Transform playerTarget;

    // อ้างอิงถึง Rigidbody2D ของ GameObject นี้
    private Rigidbody2D rb;

    void Start()
    {
        // ดึง Component Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // ค้นหา Player ในฉาก (ถ้ายังไม่ได้กำหนดใน Inspector)
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }
    }
    public int health = 3; // เลือดของศัตรู

    // ... (โค้ดอื่นๆ ของคุณ) ...

    // --- เพิ่มฟังก์ชันนี้ลงไปเพื่อให้กระสุนเรียกใช้ได้ ---
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy(gameObject); // ตาย
        }
    }
    void FixedUpdate()
    {
        // FixedUpdate ใช้สำหรับจัดการฟิสิกส์ (การเคลื่อนที่/การชน)

        if (playerTarget == null)
            return; // หยุดถ้าไม่พบ Player

        // 1. คำนวณทิศทางไปยัง Player
        Vector3 direction = playerTarget.position - transform.position;
        direction.Normalize(); // ทำให้เวกเตอร์มีขนาด 1

        // 2. หมุน GameObject เข้าหา Player (ใช้ใน Update ก็ได้ แต่รวมไว้ใน FixedUpdate)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);

        // 3. ใช้ Rigidbody เคลื่อนที่เข้าหา Player
        // เราใช้ rb.velocity แทน transform.position เพื่อให้การชนกับกำแพงทำงาน
        rb.velocity = direction * moveSpeed;
    }
}