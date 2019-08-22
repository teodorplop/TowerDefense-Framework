using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpotGrid : MonoBehaviour {
	[SerializeField] private Renderer gridRenderer;
	[SerializeField] private float cellSize = 1;

	private Transform thisTransform;
	private Material mat;
	
	private int rows, columns;
	private Tower[][] occupied;
	private Vector2 bottomLeftCellPos;

	public float CellSize { get { return cellSize; } }

	void Awake() {
		thisTransform = transform;
	}
	void Start() {
		mat = gridRenderer.material;
		UpdateGrid(true);
		gridRenderer.enabled = true;
	}

	public void UpdateGrid(bool updateMatrix = false) {
		Vector3 localScale = thisTransform.localScale;
		mat.mainTextureScale = new Vector2(localScale.x / cellSize, localScale.z / cellSize);

		if (updateMatrix) {
			rows = Mathf.RoundToInt(localScale.z / cellSize);
			columns = Mathf.RoundToInt(localScale.x / cellSize);
			occupied = new Tower[rows][];
			for (int i = 0; i < occupied.Length; ++i)
				occupied[i] = new Tower[columns];

			bottomLeftCellPos = new Vector2(
				thisTransform.position.x - (localScale.x - cellSize) / 2,
				thisTransform.position.z - (localScale.z - cellSize) / 2);
		}
	}

	public Vector3 SnapToGrid(Vector3 worldPos) {
		Vector2Int grid = WorldToGrid(worldPos);
		return new Vector3(bottomLeftCellPos.x + cellSize * grid.y, worldPos.y, bottomLeftCellPos.y + cellSize * grid.x);
	}

	public bool IsAvailable(Vector3 worldPos) {
		return IsAvailable(WorldToGrid(worldPos));
	}

	public Vector2Int WorldToGrid(Vector3 worldPos) {
		worldPos.x -= thisTransform.position.x;
		worldPos.z -= thisTransform.position.z;

		int row = Mathf.FloorToInt((worldPos.z + thisTransform.localScale.z / 2) / cellSize);
		int column = Mathf.FloorToInt((worldPos.x + thisTransform.localScale.x / 2) / cellSize);

		return new Vector2Int(row, column);
	}

	public Vector3 GridToWorld(Vector2Int grid) {
		return new Vector3(bottomLeftCellPos.x + cellSize * grid.y, thisTransform.position.y + thisTransform.localScale.y / 2, bottomLeftCellPos.y + cellSize * grid.x);
	}
	public bool IsAvailable(Vector2Int grid) {
		return IsInGrid(grid) ? !occupied[grid.x][grid.y] : false;
	}

	public void Occupy(Vector2Int grid, Tower tower) {
		if (IsInGrid(grid)) occupied[grid.x][grid.y] = tower;
	}
	public void Free(Vector2Int grid) {
		if (IsInGrid(grid)) occupied[grid.x][grid.y] = null;
	}

	public Tower GetTower(Vector3 worldPos) {
		Vector2Int grid = WorldToGrid(worldPos);
		return IsInGrid(grid) ? occupied[grid.x][grid.y] : null;
	}
	public Tower GetTower(Vector2Int grid) {
		return IsInGrid(grid) ? occupied[grid.x][grid.y] : null;
	}

	private bool IsInGrid(Vector2Int grid) {
		return grid.x >= 0 && grid.x < rows && grid.y >= 0 && grid.y < columns;
	}
}
