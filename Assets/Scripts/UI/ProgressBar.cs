using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public RectTransform progressImage;
    float maxWidth;

    void Awake()
    {
        maxWidth = progressImage.sizeDelta.x;
    }

    public void Reset()
    {
        progressImage.sizeDelta = new Vector2(0, progressImage.sizeDelta.y);
    }

    public void OnValueChanged(int curValue, int maxValue)
    {
        float ratio = (float)curValue / maxValue;
        Debug.Log("ProgressBar: OnValueChanged: curValue=" + curValue + ", maxValue=" + maxValue + ", ratio=" + ratio);
        Debug.Log("ProgressBar: OnValueChanged: maxWidth=" + maxWidth + ", newWidth=" + (maxWidth * ratio));
        progressImage.sizeDelta = new Vector2(maxWidth * ratio, progressImage.sizeDelta.y);
    }
}
