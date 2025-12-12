// ชื่อไฟล์: CameraFollow.cs

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // ==========================================
    // === Camera Follow Settings ===
    // ==========================================

    // ตัวแปรสำหรับเก็บ GameObject ที่ต้องการติดตาม (ตั้งค่าใน Inspector)
    [Header("Target & Speed")]
    public Transform target;

    // ความเร็วที่กล้องจะเคลื่อนที่ตาม (ค่าที่น้อยกว่าจะทำให้การเคลื่อนที่นุ่มนวลกว่า)
    public float smoothSpeed = 0.125f;

    // ตำแหน่ง offset ของกล้องจากตัวผู้เล่น (เช่น ห่างไปทางแกน Z)
    [Header("Offset")]
    public Vector3 offset;

    // ==========================================
    // === Main Logic ===
    // ==========================================

    // LateUpdate ถูกเรียกหลังจาก Update ทั้งหมด ทำให้การติดตามวัตถุที่เคลื่อนที่ดูลื่นไหลขึ้น
    void LateUpdate()
    {
        // 🚨 สำคัญ: ตรวจสอบว่า target (ผู้เล่น) ยังมีอยู่หรือไม่
        // เพื่อป้องกัน MissingReferenceException เมื่อผู้เล่นถูกทำลาย (Die())
        if (target != null)
        {
            // 1. กำหนดตำแหน่งที่กล้องควรอยู่
            // ตำแหน่งเป้าหมาย = ตำแหน่งผู้เล่น + ตำแหน่ง offset ที่กำหนดไว้
            Vector3 desiredPosition = target.position + offset;

            // 2. ทำให้การเคลื่อนที่นุ่มนวลขึ้น
            // ใช้ Vector3.Lerp เพื่อเคลื่อนกล้องจากตำแหน่งปัจจุบันไปยังตำแหน่งเป้าหมายอย่างช้าๆ
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime * 60f // คูณ Time.deltaTime * 60f เพื่อให้ Smoothness เป็น Independent of frame rate (ทางเลือก)
            );
            // *หมายเหตุ: การใช้ smoothSpeed ตรงๆ ก็ใช้ได้ แต่การปรับแบบนี้ช่วยให้การเคลื่อนที่ดูดีขึ้นเมื่อ Frame Rate ตก*

            // 3. กำหนดตำแหน่งใหม่ให้กับกล้อง
            transform.position = smoothedPosition;

            // (ทางเลือก) ให้กล้องหันไปมองผู้เล่น
            // transform.LookAt(target);
        }
        else
        {
            // Debug สำหรับแจ้งเตือนว่าผู้เล่นถูกทำลายแล้ว
            Debug.LogWarning("Camera target (Player) is null. Camera stopped following.");
            // ณ จุดนี้ กล้องจะหยุดเคลื่อนที่ ไม่ต้องทำอะไรต่อ
        }
    }
}