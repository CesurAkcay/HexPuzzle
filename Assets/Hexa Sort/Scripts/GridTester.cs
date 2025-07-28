using NaughtyAttributes;
using UnityEngine;

public class GridTester : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Grid grid;
    [Header("Settings")]
    [OnValueChanged("UpdateGridPos")]
    [SerializeField] private Vector3Int gridPos;

    private void UpdateGridPos() => transform.position = grid.CellToWorld(gridPos); // call this whenever we change our grid position
    
}
