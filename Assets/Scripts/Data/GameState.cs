using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameData", menuName = "GameData", order = 1)]
public class GameState : ScriptableObject 
{
    public FieldState FieldState => fieldState;
    public SpawnerState SpawnerState => spawnerState;

    [SerializeField] private FieldState fieldState;
    [SerializeField] private SpawnerState spawnerState;
}
