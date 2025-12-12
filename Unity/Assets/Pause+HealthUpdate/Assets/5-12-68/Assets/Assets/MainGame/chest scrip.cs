using UnityEngine;
using System.Collections;
using System.Collections.Generic; // ต้องใช้สำหรับ List

public class ChestScript : MonoBehaviour
{
    // กำหนดจำนวน EXP ที่จะให้ (ตั้งค่าใน Inspector ได้)
    public int expAmount = 50;

    // กำหนดเวลาที่ใช้ในการ Respawn (20 วินาที)
    public float respawnTime = 20f;

    // **[ใหม่]** สร้าง List เพื่อเก็บตำแหน่งที่เป็นไปได้ทั้งหมดที่หีบจะสุ่มไป Respawn
    // คุณจะต้องลาก GameObjects ที่เป็นจุด Respawn มาใส่ใน Inspector
    public List<Transform> respawnPoints;

    // ตัวแปรสำหรับอ้างอิงถึง Renderer และ Collider2D ของหีบ
    private Renderer chestRenderer;
    private Collider2D chestCollider;

    void Start()
    {
        // 1. ดึง Component ที่จำเป็นเมื่อเริ่มต้น
        chestRenderer = GetComponent<Renderer>();
        chestCollider = GetComponent<Collider2D>();

        // ตรวจสอบให้แน่ใจว่ามี Component เหล่านี้อยู่
        if (chestRenderer == null || chestCollider == null)
        {
            Debug.LogError("ChestScript requires a Renderer and a Collider2D component on the same GameObject.");
            enabled = false;
        }

        // **[ใหม่]** ตรวจสอบว่าได้กำหนดจุด Respawn หรือไม่
        if (respawnPoints == null || respawnPoints.Count == 0)
        {
            Debug.LogError("Respawn Points list is empty or null. Chest will not be able to randomly respawn.");
        }
    }

    // ใช้ฟังก์ชันนี้เมื่อวัตถุอื่นชนเข้ามาในขอบเขต (Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovementUnderwater_Combined_v3 player = other.GetComponent<PlayerMovementUnderwater_Combined_v3>();

        if (player != null)
        {
            // 2. ให้ EXP แก่ Player
            player.GainEXP(expAmount);

            // 3. เริ่มกระบวนการซ่อน
            HideChest();

            // 4. เริ่ม Coroutine เพื่อรอเวลาแล้ว Respawn
            StartCoroutine(RespawnAfterTime(respawnTime));
        }
    }

    private void HideChest()
    {
        chestRenderer.enabled = false;
        chestCollider.enabled = false;
    }

    /// <summary>
    /// ทำให้หีบปรากฏขึ้นมาใหม่ที่ตำแหน่งสุ่ม
    /// </summary>
    private void ShowChest()
    {
        if (respawnPoints.Count > 0)
        {
            // **[ใหม่]** 1. สุ่มเลือกตำแหน่งจาก List
            // Random.Range(min, max) จะสุ่มตัวเลขจำนวนเต็มระหว่าง min (รวม) ถึง max (ไม่รวม)
            int randomIndex = Random.Range(0, respawnPoints.Count);
            Transform randomPoint = respawnPoints[randomIndex];

            // **[ใหม่]** 2. ย้ายตำแหน่งของหีบไปที่จุดที่สุ่มได้
            // ใช้ transform.position เพื่อกำหนดตำแหน่งใหม่ของ GameObject นี้
            transform.position = randomPoint.position;
        }

        // 3. เปิดการมองเห็นและ Collider
        chestRenderer.enabled = true;
        chestCollider.enabled = true;
    }

    /// <summary>
    /// Coroutine ที่รอตามเวลาที่กำหนดแล้วเรียก ShowChest
    /// </summary>
    private IEnumerator RespawnAfterTime(float wait)
    {
        yield return new WaitForSeconds(wait);
        ShowChest();
    }
}