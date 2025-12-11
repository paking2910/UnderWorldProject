using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Underwater Physics")]
    public float moveForce = 15f;    
    public float maxSpeed = 8f;      
    public float stopDamping = 1f;   

    [Header("Skill Settings")]
    public KeyCode skillKey = KeyCode.Space; 
    public float skillForceMultiplier = 5f;  
    public float maxSpeedBoost = 2f;         
    public float skillDuration = 0.5f;       
    public float skillCooldown = 3f;         

    [Header("Combat")]
    public GameObject bulletPrefab;  
    public Transform firePoint;      
    public float fireRate = 0.2f;    

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 mousePos;
    private Camera cam;
    private float nextFireTime = 0f;

    private bool isSkillActive = false;
    private float skillTimer = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        rb.gravityScale = 0;   
        rb.drag = 2f;          
        rb.angularDrag = 1f;   
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        HandleSkillLogic();

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void HandleSkillLogic()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        if (isSkillActive)
        {
            skillTimer -= Time.deltaTime;
            if (skillTimer <= 0)
            {
                isSkillActive = false;
                cooldownTimer = skillCooldown;
            }
        }

        if (Input.GetKeyDown(skillKey) && cooldownTimer <= 0 && !isSkillActive)
        {
            StartCoroutine(PerformDash()); 
        }
    }

    System.Collections.IEnumerator PerformDash()
    {
        isSkillActive = true;
        skillTimer = skillDuration;
        
        if(moveInput.magnitude > 0)
        {
            rb.AddForce(moveInput * moveForce * skillForceMultiplier, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(transform.right * moveForce * skillForceMultiplier, ForceMode2D.Impulse);
        }

        Debug.Log("Skill DASH!");
        yield return null;
    }

    void FixedUpdate()
    {
        float currentMaxSpeed = isSkillActive ? (maxSpeed * maxSpeedBoost) : maxSpeed;

        if (moveInput.magnitude > 0)
        {
            rb.AddForce(moveInput * moveForce);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, stopDamping * Time.fixedDeltaTime);
        }

        if (rb.velocity.magnitude > currentMaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * currentMaxSpeed;
        }

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