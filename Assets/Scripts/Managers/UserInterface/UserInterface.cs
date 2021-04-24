using System;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "UserInterface", menuName = "Managers/UserInterface", order = 4)]
public class UserInterface : ScriptableObject, IManager, IUserInterface
{
    private SceneData sceneData;
    private Image[] nextFigureBlocks;

    public void BaseAwake() 
    {
        if (ToolBox.GetData(out sceneData))
        {
            CreateGrid();

            CreateFigureDisplay();

            Events.OnLevelChange.AddListener(UpdateLevel);
            Events.OnScoreChanged.Subscribe(UpdateScore);
            Events.NextFigureReady.Subscribe(UpdateFigureDisplay);
        }
    }

    public void BaseStart() => UpdateLevel();

    public void BaseUpdate() { }

    public void ClearState() 
    {
        foreach (Image image in nextFigureBlocks)
            Destroy(image.gameObject);

        UpdateScore(0);
        UpdateLevel();
    }

    private void OnDisable()
    {
        Events.OnLevelChange.RemoveListener(UpdateLevel);
        Events.OnScoreChanged.UnSubscribe(UpdateScore);
        Events.NextFigureReady.UnSubscribe(UpdateFigureDisplay);
    }

    private void CreateFigureDisplay()
    {
        nextFigureBlocks = new Image[sceneData.MaxFigureHeight];

        for (int i = 0; i < sceneData.MaxFigureHeight; i++)
        {
            GameObject newBlock = Instantiate(sceneData.BlockUI.gameObject, sceneData.NextFigure);
            nextFigureBlocks[i] = newBlock.GetComponent<Image>();
        }
    }

    private void UpdateFigureDisplay(BlockState[] blocks)
    {
        if (blocks.Length == nextFigureBlocks.Length && ToolBox.GetData(out SceneData sceneData))
        {
            for (int i = 0; i < nextFigureBlocks.Length; i++)
                nextFigureBlocks[i].color = sceneData.Colors.ToPossibleColor(blocks[i].BlockColor).Color;
        }
        else new Exception("Display blocks number != figure blocks number");
    }

    private void CreateGrid()
    {
        for (int x = 0; x < sceneData.Width; x++)
        {
            for (int y = 0; y < sceneData.Height; y++)
                Instantiate(sceneData.CellPrefab, new Vector3(x, y, 0), Quaternion.identity, sceneData.Grid);
        }
    }

    private void UpdateScore(int score) => sceneData.ScoreDisplay.text = score.ToString();

    private void UpdateLevel() => sceneData.LevelDisplay.text = sceneData.Level.ToString();
}