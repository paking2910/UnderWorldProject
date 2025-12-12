using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    // กำหนดค่าความเสียหายใน Inspector
    public float damageToDeal = 10f;
    
    // **สำคัญ:** ศัตรู/กระสุน ต้องมี Collider2D ที่ตั้งค่า Is Trigger เป็น True
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. ตรวจสอบว่าวัตถุที่ชนมี Tag เป็น "Player" หรือไม่
        if (other.CompareTag("Player"))
        {
            // 2. พยายามดึง Component HealthSystem จากวัตถุที่ชน
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();

            // **3. การป้องกัน NullReferenceException**
            if (playerHealth != null)
            {
                // ถ้าดึงได้: สั่งให้ลดเลือด
                playerHealth.TakeDamage(damageToDeal);
                
                // หากศัตรูตัวนี้คือกระสุน ควรทำลายกระสุนหลังจากชนแล้ว
                // Destroy(gameObject); 
            }
            else
            {
                // แสดงข้อความเตือนถ้าชน Player Tag แต่หา HealthSystem ไม่พบ
                Debug.LogError("Collision with Player Tag, but HealthSystem component is missing on the object: " + other.gameObject.name);
            }
        }
        
        // หากต้องการให้ศัตรู/กระสุนทำลายตัวเองเมื่อชนอย่างอื่นที่ไม่ใช่ผู้เล่น
        // else if (!other.isTrigger) 
        // {
        //     Destroy(gameObject);
        // }
    }
}