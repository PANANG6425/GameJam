using UnityEngine;
using UnityEngine.SceneManagement;

// ใส่ที่ Canvas ของซีน MainMenu
// สลับ panel (Main / Settings / Credits) + เริ่มเกม + ออกเกม
// ต่อปุ่มผ่าน OnClick ใน Inspector:
//   Start   -> StartGame
//   Settings-> ShowSettings   Credits-> ShowCredits
//   Quit    -> QuitGame       Back(ทุกอัน)-> ShowMain
public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject creditsPanel;

    [Header("เริ่มเกม")]
    [Tooltip("ชื่อซีนเกม (ต้องเพิ่มใน Build Settings ด้วย)")]
    [SerializeField] string gameSceneName = "Trench&Dugout";

    void Start()
    {
        Time.timeScale = 1f;   // เผื่อกลับมาจากเกมที่ pause ไว้
        ShowMain();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowMain()
    {
        if (mainPanel) mainPanel.SetActive(true);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        if (mainPanel) mainPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(true);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        if (mainPanel) mainPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
