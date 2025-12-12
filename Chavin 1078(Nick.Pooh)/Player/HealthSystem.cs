using UnityEngine;
using UnityEngine.UI; // **จำเป็น** เพื่อใช้ Image Component

public class HealthSystem : MonoBehaviour
{
    // ค่าพลังชีวิตสูงสุดที่เราต้องการ (ตั้งค่าใน Inspector)
    public float maxHealth = 100f; 
    
    // ค่าพลังชีวิตปัจจุบัน
    private float currentHealth; 

    // อ้างอิงถึง UI Image ที่เป็นส่วนเติมเต็ม (ตั้งค่าใน Inspector)
    public Image healthFillImage; 

    void Start()
    {
        // เริ่มต้นด้วยพลังชีวิตเต็ม
        currentHealth = maxHealth;
        // อัปเดตการแสดงผลหลอดเลือดตั้งแต่เริ่มต้น
        UpdateHealthBar(); 
    }

    // ฟังก์ชันสำหรับรับความเสียหาย
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0f); // ทำให้แน่ใจว่าค่าไม่ติดลบ
        
        UpdateHealthBar(); // อัปเดตหลอดเลือดทันที
        
        if (currentHealth <= 0)
        {
            Die(); 
        }
    }

    // ฟังก์ชันสำหรับเพิ่มพลังชีวิต
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // ทำให้แน่ใจว่าค่าไม่เกิน MaxHealth

        UpdateHealthBar(); // อัปเดตหลอดเลือดทันที
    }

    // ฟังก์ชันหลักในการอัปเดต UI
    private void UpdateHealthBar()
    {
        // คำนวณอัตราส่วนพลังชีวิต (0 ถึง 1)
        float fillRatio = currentHealth / maxHealth; 

        // ตรวจสอบว่า Image ถูกเชื่อมต่อแล้ว
        if (healthFillImage != null)
        {
            // กำหนดค่า Fill Amount ของ Image Component
            healthFillImage.fillAmount = fillRatio; 
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        // สามารถเพิ่มโค้ด เช่น Animation ตาย, โหลดฉาก Game Over ได้ที่นี่
    }
}