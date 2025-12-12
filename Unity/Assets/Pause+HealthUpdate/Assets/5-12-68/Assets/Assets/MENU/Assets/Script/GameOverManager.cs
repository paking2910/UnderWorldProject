using UnityEngine;
using UnityEngine.SceneManagement; // **สำคัญ** ต้องมีบรรทัดนี้ถึงจะเปลี่ยนฉากได้

public class GameOverManager : MonoBehaviour
{
    // ฟังก์ชันสำหรับปุ่ม Retry (เริ่มด่านเดิมใหม่)
    public void RetryGame()
    {
        // สั่งให้เวลาเดินปกติ (เผื่อมีการ Pause เกมไว้)
        Time.timeScale = 1f;

        // โหลดฉากปัจจุบันซ้ำอีกรอบ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ฟังก์ชันสำหรับปุ่ม Main Menu (กลับไปหน้า PlayScreen)
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;

        // โหลดฉากที่ชื่อ "PlayScreen" (ต้องสะกดให้ตรงเป๊ะกับชื่อไฟล์ Scene)
        SceneManager.LoadScene("PlayScreen");
    }
}