using UnityEngine;
using System.Collections;


public class UIScaler : MonoBehaviour
{
    [SerializeField] protected RectTransform canvas;
    [SerializeField] protected Transform backGround;
    [SerializeField] protected Transform upBorder;
    [SerializeField] protected RectTransform UserInterface;
    [SerializeField] protected RectTransform menuButton;
    [SerializeField] protected RectTransform restartButton;
    [SerializeField] protected RectTransform nextFigureContainer;
    [SerializeField] protected RectTransform nextFigure;
    [SerializeField] protected RectTransform levelDisplay;
    [SerializeField] protected RectTransform scoreDisplay;
    [SerializeField] protected RectTransform godsNameDisplay;
    protected Camera mainCamera;
    protected Vector2 cameraPosition;
    protected float borderSize = (1920 / 1080);
    protected int offset = 1;
    protected float height;
    protected float width;
    protected float scaleY;
    protected float scaleX;
    protected float pixelsPerUnit;
    protected float borderSizeX;
    protected float borderPixelsX;
    protected float borderPixelsY;

    public void Scaling(SceneData sceneData) => StartCoroutine(WaitForCanvasInit(sceneData));

    protected virtual void Scale(SceneData sceneData)
    {
        float resolution = (float)Screen.height / Screen.width - 16f / 9;
        borderSize = resolution > 0 ? 3 + resolution * 9 : 3;

        offset = 1;
        cameraPosition = new Vector2(sceneData.Width / 2f - 0.5f, (sceneData.Height - sceneData.MaxFigureHeight) / 2f - 0.5f - offset);

        mainCamera = Camera.main;
        mainCamera.orthographicSize = sceneData.Width + borderSize;
        mainCamera.transform.position = cameraPosition.AddZ(-10);

        height = mainCamera.orthographicSize * 2;
        width = height / canvas.sizeDelta.y * canvas.sizeDelta.x;
        scaleY = mainCamera.orthographicSize - (sceneData.Height - sceneData.MaxFigureHeight) / 2f;
        scaleX = (width - sceneData.Width) / 2;

        pixelsPerUnit = canvas.sizeDelta.x / width;

        borderSizeX = (height / canvas.sizeDelta.y * UserInterface.rect.width - sceneData.Width) / 2;
        borderPixelsX = borderSizeX * pixelsPerUnit;

        borderPixelsY = (sceneData.Height - sceneData.MaxFigureHeight) * pixelsPerUnit;

        backGround.position = cameraPosition;
        backGround.localScale = new Vector3(width, height, 1);

        upBorder.localScale = new Vector3(1, scaleY / height, 0);
        upBorder.localPosition = new Vector3(0, (mainCamera.orthographicSize - scaleY / 2) / height + (offset / height), 1);

        scoreDisplay.sizeDelta = new Vector3(borderPixelsX, scoreDisplay.sizeDelta.y, 1);
        scoreDisplay.localPosition = new Vector3((-sceneData.Width / 2) * pixelsPerUnit - borderPixelsX / 2, pixelsPerUnit * 5, 1);

        levelDisplay.sizeDelta = new Vector3(borderPixelsX, levelDisplay.sizeDelta.y, 1);
        levelDisplay.localPosition = new Vector3((sceneData.Width / 2) * pixelsPerUnit + borderPixelsX / 2, pixelsPerUnit * 5, 1);

        menuButton.sizeDelta = new Vector3(borderPixelsX, menuButton.sizeDelta.y, 1);
        menuButton.position = scoreDisplay.position;
        menuButton.localPosition += new Vector3(0, -scoreDisplay.sizeDelta.y / 2 - menuButton.sizeDelta.y / 2, 0);

        nextFigureContainer.sizeDelta = new Vector3(borderPixelsX, nextFigureContainer.sizeDelta.y, 1);
        nextFigureContainer.position = levelDisplay.position;
        nextFigureContainer.localPosition += new Vector3(0, -levelDisplay.sizeDelta.y / 2 - nextFigureContainer.sizeDelta.y / 2, 0);
    }

    private IEnumerator WaitForCanvasInit(SceneData sceneData)
    {
        yield return new WaitForEndOfFrame();

        Scale(sceneData);
    }
}