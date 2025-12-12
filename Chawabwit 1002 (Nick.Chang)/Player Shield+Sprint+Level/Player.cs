using UnityEngine;
using UnityEngine.Video; // เพิ่มบรรทัดนี้เพื่อรองรับ VideoPlayer

// *** อย่าลืมเช็คชื่อไฟล์ให้ตรงกับชื่อ Class ตรงนี้นะครับ ***
public class PlayerMovementUnderwater_Combined_v3 : MonoBehaviour
{
    // ==========================================
    // === 1. Level System & Combat Stats ===
    // ==========================================
    [Header("Level System")]
    public int level = 1;
    public int maxLevel = 4;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    public int attackPower = 10;

    [Header("Combat/Sword")]
    public bool hasSword = false;
    public KeyCode attackKey = KeyCode.Mouse0;
    public GameObject swordHitboxPrefab;
    public float attackCooldown = 0.5f;
    private float attackTimer = 0f;

    // ==========================================
    // === 2. Health & Damage Settings ===
    // ==========================================
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Damage Settings")]
    public float invulnerabilityDuration = 1.0f; // อมตะหลังโดนชน
    private float invulnerabilityTimer = 0f;

    // ==========================================
    // === NEW! 3. Shield Skill (Invincible) ===
    // ==========================================
    [Header("Shield Skill Settings")]
    public KeyCode shieldKey = KeyCode.E;       // ปุ่มกดใช้โล่
    public float shieldDuration = 9.0f;         // ระยะเวลาอมตะ 9 วินาที
    public float shieldCooldown = 15.0f;        // คูลดาวน์ก่อนใช้ใหม่ได้
    
    // *** แก้ตรงนี้ให้ชื่อตรงกับข้างล่างแล้วครับ ***
    public GameObject shieldVisualObject;       // ลาก Game Object ที่มีไฟล์ MP4 หรือ Animation โล่มาใส่ตรงนี้

    private bool isShieldActive = false;        // เช็คว่าโล่ทำงานอยู่ไหม
    private float shieldActiveTimer = 0f;
    private float shieldCooldownTimer = 0f;

    // ==========================================
    // === 4. Movement & Speed Boost Settings ===
    // ==========================================
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float sprintSpeedMultiplier = 1.5f; // ลดลงหน่อยเผื่อเร็วไป

    [Header("Speed Boost Skill")]
    public float speedBoostMultiplier = 2.0f;
    public float speedBoostDuration = 3.0f;
    public float speedBoostCooldown = 5.0f;
    public KeyCode speedBoostKey = KeyCode.Space;
    
    private bool isSpeedBoostActive = false;
    private float speedBoostTimer = 0f;
    private float speedBoostCooldownTimer = 0f;

    // ==========================================
    // === 5. Visuals & References ===
    // ==========================================
    [Header("Visuals")]
    public GameObject visualSwordObject;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isSprinting;

    // ==========================================
    // === Unity Standard Functions ===
    // ==========================================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateStatsByLevel();

        if (visualSwordObject != null) visualSwordObject.SetActive(hasSword);
        
        // ปิดโล่ตอนเริ่มเกม
        if (shieldVisualObject != null) shieldVisualObject.SetActive(false);
    }

    void Update()
    {
        HandleMovementInput();
        
        // จัดการสกิลเพิ่มความเร็ว (Spacebar)
        HandleSpeedBoost();

        // จัดการสกิลโล่อมตะ (E) - **ใหม่**
        HandleShieldSkill();

        HandleTimersAndCombatInput();

        // Cheat
        if (Input.GetKeyDown(KeyCode.P)) GainEXP(50);
    }

    void FixedUpdate()
    {
        float finalSpeed = moveSpeed;
        if (isSpeedBoostActive) finalSpeed *= speedBoostMultiplier;
        else if (isSprinting) finalSpeed *= sprintSpeedMultiplier;

        rb.velocity = movement * finalSpeed;
    }

    // ==========================================
    // === Logic Functions ===
    // ==========================================

    void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        movement = new Vector2(horizontalInput, verticalInput);
        if (movement.magnitude > 1) movement.Normalize();

        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    // --- ส่วนจัดการ Speed Boost (ของเดิม) ---
    void HandleSpeedBoost()
    {
        if (speedBoostCooldownTimer > 0) speedBoostCooldownTimer -= Time.deltaTime;

        if (isSpeedBoostActive)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0)
            {
                isSpeedBoostActive = false;
                speedBoostCooldownTimer = speedBoostCooldown;
            }
        }

        if (Input.GetKeyDown(speedBoostKey) && speedBoostCooldownTimer <= 0 && !isSpeedBoostActive)
        {
            isSpeedBoostActive = true;
            speedBoostTimer = speedBoostDuration;
        }
    }

    // --- ส่วนจัดการ Shield (ของใหม่) ---
    void HandleShieldSkill()
    {
        // ลดเวลา Cooldown
        if (shieldCooldownTimer > 0) shieldCooldownTimer -= Time.deltaTime;

        // ถ้าโล่ทำงานอยู่
        if (isShieldActive)
        {
            shieldActiveTimer -= Time.deltaTime;
            
            // ถ้าเวลาหมด
            if (shieldActiveTimer <= 0)
            {
                DeactivateShield();
            }
        }

        // กดปุ่มเพื่อใช้สกิล
        if (Input.GetKeyDown(shieldKey) && shieldCooldownTimer <= 0 && !isShieldActive)
        {
            ActivateShield();
        }
    }

    void ActivateShield()
    {
        isShieldActive = true;
        shieldActiveTimer = shieldDuration;
        
        // เปิด Animation/Video ของโล่
        if (shieldVisualObject != null)
        {
            shieldVisualObject.SetActive(true);
            
            // ถ้าเป็น MP4 (VideoPlayer) สั่งให้เล่นใหม่ตั้งแต่ต้น
            VideoPlayer vp = shieldVisualObject.GetComponent<VideoPlayer>();
            if (vp != null) 
            {
                vp.Stop();
                vp.Play();
            }
        }

        Debug.Log("Shield Activated! Invincible for 9 seconds.");
    }

    void DeactivateShield()
    {
        isShieldActive = false;
        shieldCooldownTimer = shieldCooldown;

        // ปิด Animation/Video ของโล่
        if (shieldVisualObject != null)
        {
            shieldVisualObject.SetActive(false);
        }
        
        Debug.Log("Shield Expired.");
    }

    // ==========================================
    // === Combat & Damage ===
    // ==========================================

    void HandleTimersAndCombatInput()
    {
        if (invulnerabilityTimer > 0) invulnerabilityTimer -= Time.deltaTime;
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        if (hasSword && Input.GetKeyDown(attackKey) && attackTimer <= 0)
        {
            Attack();
        }
    }

    void Attack()
    {
        attackTimer = attackCooldown;
        if (swordHitboxPrefab != null)
        {
            GameObject hitbox = Instantiate(swordHitboxPrefab, transform.position, Quaternion.identity);
            PlayerSwordDamage damageScript = hitbox.GetComponent<PlayerSwordDamage>();
            if (damageScript != null) damageScript.SetAttacker(this.gameObject, attackPower);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        // 1. เช็คว่ามีสกิลโล่ทำงานอยู่ไหม? ถ้ามี ห้ามลดเลือด
        if (isShieldActive)
        {
            Debug.Log("BLOCKED by Shield Skill!");
            return;
        }

        // 2. เช็คอมตะจากการโดนชนครั้งก่อน
        if (invulnerabilityTimer > 0) return;

        currentHealth -= damageAmount;
        Debug.Log($"Took Damage! HP left: {currentHealth}");
        invulnerabilityTimer = invulnerabilityDuration;

        if (currentHealth <= 0) Die();
    }

    public void PickUpHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void PickUpSword()
    {
        hasSword = true;
        if (visualSwordObject != null) visualSwordObject.SetActive(true);
    }

    void Die()
    {
        Debug.Log("Game Over");
        Destroy(gameObject);
    }

    // ==========================================
    // === Level System ===
    // ==========================================

    public void GainEXP(int amount)
    {
        if (level >= maxLevel) return;
        currentExp += amount;
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (currentExp >= expToNextLevel && level < maxLevel)
        {
            currentExp -= expToNextLevel;
            level++;
            UpdateStatsByLevel();
        }
        if (level >= maxLevel) currentExp = expToNextLevel;
    }

    void UpdateStatsByLevel()
    {
        switch (level)
        {
            case 1: maxHealth = 3; attackPower = 10; break;
            case 2: maxHealth = 5; attackPower = 15; break;
            case 3: maxHealth = 7; attackPower = 20; break;
            case 4: maxHealth = 10; attackPower = 30; break;
        }
        currentHealth = maxHealth;
    }
}