using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // ==========================================
    // === 1. ส่วนตั้งค่า Physics (เดินใต้น้ำ) ===
    // ==========================================
    [Header("Underwater Physics")]
    public float moveForce = 15f;    // แรงขับเคลื่อนปกติ
    public float maxSpeed = 8f;      // ความเร็วสูงสุดปกติ
    public float stopDamping = 1f;   // แรงหน่วงตอนหยุด (ยิ่งเยอะยิ่งหยุดไว)

    // ==========================================
    // === 2. ส่วนตั้งค่า Skill (พุ่งตัว) ===
    // ==========================================
    [Header("Skill Settings")]
    public KeyCode skillKey = KeyCode.Space; // ปุ่มกดสกิล
    public float skillForceMultiplier = 5f;  // แรงส่งตอนพุ่ง (เลขยิ่งเยอะยิ่งพุ่งแรง)
    public float maxSpeedBoost = 2f;         // ขยายเพดานความเร็วสูงสุดตอนกดสกิล (x2)
    public float skillDuration = 0.5f;       // ระยะเวลาที่พุ่ง (สั้นๆ จะเหมือน Dash)
    public float skillCooldown = 3f;         // เวลาพักสกิล (วินาที)

    // ==========================================
    // === 3. ส่วนตั้งค่า Combat (ยิงปืน) ===
    // ==========================================
    [Header("Combat")]
    public GameObject bulletPrefab;  // อย่าลืมลาก Prefab กระสุนใส่ใน Inspector
    public Transform firePoint;      // อย่าลืมลากจุดปล่อยกระสุนใส่ใน Inspector
    public float fireRate = 0.2f;    // ยิงรัวแค่ไหน

    // ตัวแปรภายใน (ไม่ต้องยุ่งกับมัน)
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 mousePos;
    private Camera cam;
    private float nextFireTime = 0f;

    // ตัวแปรเช็คสถานะ Skill
    private bool isSkillActive = false;
    private float skillTimer = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        // ตั้งค่าแรงต้านน้ำอัตโนมัติ
        rb.gravityScale = 0;   // ปิดแรงโน้มถ่วง
        rb.drag = 2f;          // แรงหนืดน้ำ
        rb.angularDrag = 1f;   // แรงหนืดตอนหมุน
    }

    void Update()
    {
        // 1. รับปุ่มเดิน (WASD)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // 2. รับตำแหน่งเมาส์
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // 3. จัดการ Skill (Spacebar)
        HandleSkillLogic();

        // 4. ยิงปืน (คลิกซ้าย)
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void HandleSkillLogic()
    {
        // ลดเวลา Cooldown
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        // ลดเวลาใช้งาน Skill (ตอนกำลังพุ่ง)
        if (isSkillActive)
        {
            skillTimer -= Time.deltaTime;
            if (skillTimer <= 0)
            {
                isSkillActive = false;
                cooldownTimer = skillCooldown;
                // Debug.Log("Skill หมดฤทธิ์");
            }
        }

        // เช็คการกดปุ่ม
        if (Input.GetKeyDown(skillKey) && cooldownTimer <= 0 && !isSkillActive)
        {
            StartCoroutine(PerformDash()); // เรียกฟังก์ชันพุ่ง
        }
    }

    // ฟังก์ชันสั่งพุ่งตัว (Dash)
    System.Collections.IEnumerator PerformDash()
    {
        isSkillActive = true;
        skillTimer = skillDuration;
        
        // ใส่แรงกระแทกทันที (Impulse) ไปในทิศทางที่กำลังกดเดิน
        if(moveInput.magnitude > 0)
        {
            rb.AddForce(moveInput * moveForce * skillForceMultiplier, ForceMode2D.Impulse);
        }
        else
        {
            // ถ้าไม่ได้กดเดิน ให้พุ่งไปข้างหน้าตามที่ตัวหันอยู่
            rb.AddForce(transform.right * moveForce * skillForceMultiplier, ForceMode2D.Impulse);
        }

        Debug.Log("Skill DASH! 💨");
        yield return null;
    }

    void FixedUpdate()
    {
        // ถ้าสกิลทำงาน ให้ปลดล็อคความเร็วสูงสุดเพิ่มขึ้น
        float currentMaxSpeed = isSkillActive ? (maxSpeed * maxSpeedBoost) : maxSpeed;

        // คำนวณแรงเดินปกติ (Force)
        if (moveInput.magnitude > 0)
        {
            rb.AddForce(moveInput * moveForce);
        }
        else
        {
            // ระบบช่วยเบรก (ให้หยุดนิ่งง่ายขึ้นเมื่อปล่อยปุ่ม)
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, stopDamping * Time.fixedDeltaTime);
        }

        // จำกัดความเร็วไม่ให้เกินกำหนด (Clamp Velocity)
        if (rb.velocity.magnitude > currentMaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * currentMaxSpeed;
        }

        // หมุนตัวตามเมาส์
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    void Shoot()
    {
        if (bulletPrefab && firePoint)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}