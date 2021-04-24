using UnityEngine;
using UnityEngine.UI;


public class ChallengeScaler : UIScaler
{
    [SerializeField] protected RectTransform pausePanel;
    [SerializeField] protected RectTransform leftBorder;
    [SerializeField] protected RectTransform leftBorderUp;
    [SerializeField] protected RectTransform rightBorder;
    [SerializeField] protected RectTransform rightBorderUp;
    [SerializeField] private AspectRatioFitter ratioFilter;

    protected override void Scale(SceneData sceneData)
    {
        base.Scale(sceneData);

        float borderUpSize = pixelsPerUnit / leftBorder.sizeDelta.x;

        leftBorder.sizeDelta = new Vector3(borderPixelsX, borderPixelsY, 1);
        leftBorder.localPosition = new Vector3((-sceneData.Width / 2) * pixelsPerUnit - borderPixelsX / 2, pixelsPerUnit * offset, 1);

        leftBorderUp.sizeDelta = new Vector3(leftBorder.sizeDelta.x + 2 * borderUpSize * leftBorder.sizeDelta.x, leftBorderUp.sizeDelta.y, 1);
        leftBorderUp.localPosition = new Vector3(0, pixelsPerUnit * ((sceneData.Height - sceneData.MaxFigureHeight) / 2f) + leftBorderUp.sizeDelta.y * leftBorderUp.localScale.y / 3, 0);

        rightBorder.sizeDelta = new Vector3(borderPixelsX, borderPixelsY, 1);
        rightBorder.localPosition = new Vector3((sceneData.Width / 2) * pixelsPerUnit + borderPixelsX / 2, pixelsPerUnit * offset, 1);

        rightBorderUp.sizeDelta = leftBorderUp.sizeDelta;
        rightBorderUp.localPosition = new Vector3(0, leftBorderUp.localPosition.y, 1);

        godsNameDisplay.sizeDelta = new Vector3((sceneData.Width - 2) * pixelsPerUnit, rightBorderUp.sizeDelta.y, 1);
        godsNameDisplay.localPosition = new Vector3(0, pixelsPerUnit * (height / 2 - scaleY + offset) + godsNameDisplay.sizeDelta.y / 2, 1);

        if (sceneData.PortraitSprite != null)
            ratioFilter.aspectRatio = sceneData.PortraitSprite.rect.width / sceneData.PortraitSprite.rect.height;
    }
}
