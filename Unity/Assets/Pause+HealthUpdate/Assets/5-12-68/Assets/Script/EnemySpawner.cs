using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs (ลากไฟล์ Prefab มาใส่ที่นี่)")]
    [SerializeField] private GameObject swarmerPrefab;
    [SerializeField] private GameObject bigSwarmerPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float swarmerInterval = 3.5f;
    [SerializeField] private float bigSwarmerInterval = 10f;

    [Header("Limit Settings")]
    [SerializeField] private int swarmerLimit = 20;     // จำกัดตัวเล็ก 20 ตัว
    [SerializeField] private int bigSwarmerLimit = 5;   // จำกัดตัวใหญ่ 5 ตัว

    [Header("Spawn Area (ระยะขอบเขตการเกิด)")]
    [SerializeField] private float xRange = 5f; // ระยะสุ่มแกน X (ซ้าย-ขวา)
    [SerializeField] private float yRange = 6f; // ระยะสุ่มแกน Y (บน-ล่าง)

    void Start()
    {
        // เริ่มการทำงานแยกกันระหว่างตัวเล็กกับตัวใหญ่
        StartCoroutine(spawnEnemy(swarmerInterval, swarmerPrefab, swarmerLimit));
        StartCoroutine(spawnEnemy(bigSwarmerInterval, bigSwarmerPrefab, bigSwarmerLimit));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemyPrefab, int limit)
    {
        // 1. รอเวลาตามที่กำหนด (Interval)
        yield return new WaitForSeconds(interval);

        // --- เช็กความปลอดภัย ---
        // ถ้าไม่มี Prefab หรือลืมใส่ ให้หยุดทำงานเพื่อป้องกัน Error
        if (enemyPrefab == null)
        {
            Debug.LogError("ไม่พบ Prefab! กรุณาลากไฟล์ Prefab ใส่ในช่อง Inspector ของ EnemySpawner");
            yield break;
        }

        // 2. เช็กจำนวนศัตรูในฉาก (นับเฉพาะตัวที่มี Tag "Enemy")
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        // ถ้าจำนวนยังน้อยกว่า Limit ให้สร้างเพิ่ม
        if (existingEnemies.Length < limit)
        {
            // --- สูตรคำนวณตำแหน่งใหม่ (แก้ให้เกิดรอบๆ Spawner) ---
            // เอาตำแหน่งปัจจุบันของ Spawner (transform.position) + ค่าสุ่ม
            Vector3 randomPos = new Vector3(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange), 0);
            Vector3 spawnPoint = transform.position + randomPos;

            Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        }

        // 3. วนลูปเรียกตัวเองซ้ำ (เพื่อให้ทำงานต่อเนื่อง)
        StartCoroutine(spawnEnemy(interval, enemyPrefab, limit));
    }
}