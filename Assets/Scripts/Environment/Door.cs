using UnityEngine;

// ประตู/กำแพงกั้น · barrier = collider แข็ง (ไม่ใช่ trigger) ที่บล็อกทางเดิน
// visual = sprite ของประตู · เรียก Open()/Close() เพื่อเปิด-ปิด
public class Door : MonoBehaviour
{
    [SerializeField]
    Collider2D barrier; // collider แข็งที่กั้น (ปล่อยว่าง = ใช้ collider บน object นี้)

    [SerializeField]
    GameObject visual; // ภาพประตู (ปล่อยว่างได้)

    [SerializeField]
    bool startClosed = true;

    public bool IsOpen { get; private set; }

    void Awake()
    {
        if (barrier == null)
            barrier = GetComponent<Collider2D>();
        if (startClosed)
            Close();
        else
            Open();
    }

    public void Open()
    {
        IsOpen = true;
        if (barrier != null)
            barrier.enabled = false;
        if (visual != null)
            visual.SetActive(false);
    }

    public void Close()
    {
        IsOpen = false;
        if (barrier != null)
            barrier.enabled = true;
        if (visual != null)
            visual.SetActive(true);
    }
}
