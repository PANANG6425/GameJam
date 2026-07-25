using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public RectTransform progressImage;
    float maxWidth;

    void Start()
    {
        maxWidth = progressImage.rect.width;
    }

    public void OnValueChanged(int curValue, int maxValue)
    {
        float ratio = (float)curValue / maxValue;
        Debug.Log("ProgressBar: OnValueChanged: curValue=" + curValue + ", maxValue=" + maxValue + ", ratio=" + ratio);
        progressImage.sizeDelta = new Vector2(maxWidth * ratio, progressImage.sizeDelta.y);
    }
}
