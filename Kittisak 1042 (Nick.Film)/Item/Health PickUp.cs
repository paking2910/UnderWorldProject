using UnityEngine;
public class HealthPickup : MonoBehaviour
{
    // กำหนดค่าพลังชีวิตที่จะเพิ่ม (ตั้งค่าใน Inspector)
    public float healAmount = 2f;

    // ฟังก์ชันนี้จะทำงานเมื่อเกิดการชนแบบ Trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. ตรวจสอบว่าวัตถุที่ชนมี Tag เป็น "Player" หรือไม่
        if (other.CompareTag("Player"))
        {
            // 2. พยายามดึง Component HealthSystem จากผู้เล่น
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();

            // 3. ป้องกัน NullReferenceException และตรวจสอบว่าผู้เล่นมีระบบเลือด
            if (playerHealth != null)
            {
                // 4. สั่งให้ผู้เล่นเพิ่มพลังชีวิต
                playerHealth.Heal(healAmount);
                
                // 5. ทำลายไอเทมทิ้งหลังจากเก็บแล้ว
                Destroy(gameObject); 
            }
            else
            {
                // Log Error นี้หากเกิดปัญหา
                Debug.LogError("HealthSystem component not found on Player object.");
            }
        }
    }
}