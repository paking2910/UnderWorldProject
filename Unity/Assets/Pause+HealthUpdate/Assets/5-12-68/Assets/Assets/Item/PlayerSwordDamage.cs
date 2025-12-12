using UnityEngine;

public class PlayerSwordDamage : MonoBehaviour
{
    // เปลี่ยน swordDamage เป็น private เพื่อเก็บค่า Damage ที่ส่งมาจาก Player
    private int damageAmount;

    public float lifetime = 0.1f; // ระยะเวลาที่ Hitbox อยู่ (0.1 วินาที)

    // ตัวแปรสำหรับเก็บผู้โจมตี (ป้องกันการทำดาเมจตัวเอง)
    private GameObject attacker;

    // ==========================================
    // === แก้ไข: ฟังก์ชัน SetAttacker (รับ 2 ค่า) ===
    // ==========================================
    // ฟังก์ชันนี้ถูกเรียกจาก PlayerMovementUnderwater_Combined_v3.Attack()
    public void SetAttacker(GameObject attackingObject, int power)
    {
        attacker = attackingObject;
        damageAmount = power; // กำหนดค่าพลังโจมตีตาม Level ของผู้เล่น

        Debug.Log("Hitbox received Attack Power: " + damageAmount);
    }

    void Start()
    {
        // ทำลาย Hitbox หลังจากผ่านไปตามค่า lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่า Hitbox ได้รับค่าพลังโจมตีแล้วหรือไม่
        if (damageAmount <= 0)
        {
            Debug.LogError("PlayerSwordDamage: Damage amount is zero or less. Check if Player's SetAttacker ran correctly.");
            return;
        }

        // === 1. ตรวจสอบผู้เล่นอื่น (ป้องกัน Self-Damage) ===
        // ใช้นามสกุลคลาสใหม่ที่คุณตั้งไว้
        PlayerMovementUnderwater_Combined_v3 otherPlayer = other.GetComponent<PlayerMovementUnderwater_Combined_v3>();
        if (otherPlayer != null)
        {
            if (other.gameObject == attacker)
            {
                // Self-Damage check: เพิกเฉยต่อการชนผู้โจมตี
                Debug.Log("Sword hit the Attacker. Damage avoided (Self-Damage Check).");
                return;
            }

            // ทำ Damage กับผู้เล่นคนอื่น
            otherPlayer.TakeDamage(damageAmount);
            Debug.Log("Sword hit another Player! Dealt " + damageAmount + " damage.");
        }

        // === 2. ตรวจสอบศัตรู ===
        // สมมติว่า EnemyHealth เป็นคลาสของศัตรู
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            // ทำ Damage กับศัตรู โดยใช้ค่า damageAmount ที่มาจาก Level ของผู้เล่น
            enemy.TakeDamage(damageAmount);
            Debug.Log("Sword hit Enemy: " + other.gameObject.name + "! Dealt " + damageAmount + " damage.");

            // Optional: ถ้าดาบควรจะหายไปหลังจากโดนเป้าหมายแรก ให้เปิดบรรทัดนี้
            // Destroy(gameObject);
        }
    }
}
