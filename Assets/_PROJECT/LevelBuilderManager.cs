using Grid;
using UnityEngine;

public class LevelBuilderManager : MonoBehaviour
{
	[SerializeField] private NodeManager _nodeManager;
	public NodeManager NodeManager => _nodeManager;

	[SerializeField] private int _mapWidth;
	[SerializeField] private int _mapHeight;
	[SerializeField] private float _cellSize;

	public void GenerateNodeMap()
	{
		_nodeManager.GenerateMap(_mapWidth, _mapHeight, _cellSize);

	}

	public void DeleteMap()
	{
		_nodeManager.DeleteAllNodes();
	}

	public void AssignNodeNeighbors()
	{
		_nodeManager.AssignNodeNeighbour();
	}

	public void RebuildNodeGrid()
	{
		_nodeManager.RebuildNodeGrid();
	}
}
