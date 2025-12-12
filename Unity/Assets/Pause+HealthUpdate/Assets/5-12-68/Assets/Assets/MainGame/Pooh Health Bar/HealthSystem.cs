using UnityEngine;
using UnityEngine.UI; // จำเป็น เพื่อใช้ Image Component

public class HealthSystem : MonoBehaviour
{
    [Header("UI & Effect")]
    // 1. ตัวแปรเก็บหน้าจอ Game Over (ลาก Panel มาใส่)
    public GameObject gameOverPanel;
    // อ้างอิงถึง UI Image ที่เป็นหลอดเลือด (ลาก Image มาใส่)
    public Image healthFillImage;

    [Header("Settings")]
    // 2. ตัวแปรเก็บตัวควบคุมการเดิน (ลากตัว Player มาใส่)
    public PlayerController movementScript;
    // ค่าพลังชีวิตสูงสุด
    public float maxHealth = 10f;

    // ตัวแปรภายใน
    private float currentHealth;
    private bool isDead = false; // ตัวเช็คว่าตายหรือยัง

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
        if (isDead) return; // ถ้าตายแล้ว ไม่ต้องรับดาเมจเพิ่ม

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
        if (isDead) return; // ถ้าตายแล้ว ฮีลไม่ได้

        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // ไม่เกิน MaxHealth

        UpdateHealthBar();
    }

    // ฟังก์ชันหลักในการอัปเดต UI
    private void UpdateHealthBar()
    {
        float fillRatio = currentHealth / maxHealth;

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = fillRatio;
        }
    }

    // ---------------------------------------------------
    // ส่วนที่แก้ไข: ใส่คำสั่งการตายที่สมบูรณ์ตรงนี้
    // ---------------------------------------------------
    private void Die()
    {
        // 1. ป้องกันการเรียกฟังก์ชันซ้ำ
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " has died!");

        // 2. สั่งเล่นอนิเมชั่นตาย (ต้องมี Trigger ชื่อ "Die" ใน Animator)
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // 3. ปิดสคริปต์การเดิน (หยุดรับคำสั่งปุ่มกด)
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // 4. หยุดแรงฟิสิกส์ทั้งหมด (กันตัวละครไถลต่อ)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // หยุดนิ่งสนิท
        }

        // 5. ปิดการชน (เพื่อให้ศัตรูเดินผ่านศพไปได้)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 6. แสดงหน้าจอ Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}