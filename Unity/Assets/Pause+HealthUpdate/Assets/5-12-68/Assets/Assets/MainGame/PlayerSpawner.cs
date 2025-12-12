using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerSpawner : MonoBehaviour
{
    // กำหนดตัวแปรสำหรับรับ GameObject ของผู้เล่น (player prefab)
    public GameObject playerPrefab;

    // กำหนดตัวแปรสำหรับรับ Transform ของจุดเกิด (playerSpawnPoint)
    public Transform spawnPoint;

    // Start() ถูกเรียกเมื่อ Script เริ่มทำงานครั้งแรก
    void Start()
    {
        // 1. ตรวจสอบว่ามี Player Prefab และ Spawn Point ถูกกำหนดหรือไม่
        if (playerPrefab != null && spawnPoint != null)
        {
            // 2. Instantiate: สร้าง Player Prefab ที่ตำแหน่งของ Spawn Point
            // Quaternion.identity หมายถึงการหมุน (Rotation) ที่เป็นศูนย์ (ไม่มีการหมุน)
            Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

            // หมายเหตุ: ถ้าผู้เล่น (player) ถูกลากเข้ามาใน Scene อยู่แล้ว
            // คุณอาจต้องการแค่กำหนดตำแหน่ง:
            // playerPrefab.transform.position = spawnPoint.position;
            // แต่การใช้ Prefab และ Instantiate เป็นวิธีที่เหมาะสมกว่าในการจัดการ Spawn
        }
        else
        {
            // แสดงข้อความเตือนใน Console ถ้าตัวแปรใดตัวแปรหนึ่งยังไม่ได้กำหนด
            Debug.LogError("Player Prefab or Spawn Point is not assigned in the PlayerSpawner script!");
        }
    }
}