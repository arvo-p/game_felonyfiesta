using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using System.Linq;

public class Pathfinding{
	public class Node {
		public int X, Y;
		public int G, H;
		public int F => G + H;
		public Node? Parent;

		public Node(int x, int y) { X = x; Y = y; }
	}

	Map map;
	public int[,] collision;
	
	float mapWidth;
	float mapHeight;
	float tileSize; 

	public Pathfinding(){
		map = Game.env.map;
		mapWidth = map.mapsize.X; 
		mapHeight = map.mapsize.Y; 
		tileSize = map.tileDimension;
		collision = map.collision;
	}

	public List<Vector2>? FindPath(int startX, int startY, int targetX, int targetY) {
		if (startX < 0 || startX >= collision.GetLength(0) || startY < 0 || startY >= collision.GetLength(1)) return null;
		if (targetX < 0 || targetX >= collision.GetLength(0) || targetY < 0 || targetY >= collision.GetLength(1)) return null;
		if (collision[targetX, targetY] != -1) return null;

		List<Node> openList = new List<Node>();
		Dictionary<string, Node> allNodes = new Dictionary<string, Node>();
		HashSet<string> closedList = new HashSet<string>();

		Node startNode = new Node(startX, startY);
		startNode.G = 0;
		startNode.H = GetDistance(startX, startY, targetX, targetY);
		openList.Add(startNode);
		allNodes[$"{startX},{startY}"] = startNode;

		while (openList.Count > 0) {
			Node currentNode = openList.OrderBy(n => n.F).ThenBy(n => n.H).First();

			openList.Remove(currentNode);
			closedList.Add($"{currentNode.X},{currentNode.Y}");

			if (currentNode.X == targetX && currentNode.Y == targetY) {
				return RetracePath(startNode, currentNode);
			}

			foreach (Node neighbor in GetNeighbors(currentNode)) {
				if (collision[neighbor.X, neighbor.Y] != -1 || closedList.Contains($"{neighbor.X},{neighbor.Y}")) {
					continue;
				}

				// Prevent cutting corners
				if (neighbor.X != currentNode.X && neighbor.Y != currentNode.Y) {
					if (collision[neighbor.X, currentNode.Y] != -1 || collision[currentNode.X, neighbor.Y] != -1) {
						continue;
					}
				}

				int newMovementCostToNeighbor = currentNode.G + GetDistance(currentNode.X, currentNode.Y, neighbor.X, neighbor.Y);
				
				string neighborKey = $"{neighbor.X},{neighbor.Y}";
				if (!allNodes.ContainsKey(neighborKey)) {
					allNodes[neighborKey] = neighbor;
				}
				Node existingNeighbor = allNodes[neighborKey];

				if (newMovementCostToNeighbor < existingNeighbor.G || !openList.Contains(existingNeighbor)) {
					existingNeighbor.G = newMovementCostToNeighbor;
					existingNeighbor.H = GetDistance(existingNeighbor.X, existingNeighbor.Y, targetX, targetY);
					existingNeighbor.Parent = currentNode;

					if (!openList.Contains(existingNeighbor)) {
						openList.Add(existingNeighbor);
					}
				}
			}
		}

		return null;
	}

	private List<Node> GetNeighbors(Node node) {
		List<Node> neighbors = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0) continue;

				int checkX = node.X + x;
				int checkY = node.Y + y;

				if (checkX >= 0 && checkX < collision.GetLength(0) && checkY >= 0 && checkY < collision.GetLength(1)) {
					neighbors.Add(new Node(checkX, checkY));
				}
			}
		}

		return neighbors;
	}

	private int GetDistance(int x1, int y1, int x2, int y2) {
		int dstX = Math.Abs(x1 - x2);
		int dstY = Math.Abs(y1 - y2);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}

	private List<Vector2> RetracePath(Node startNode, Node endNode) {
		List<Vector2> path = new List<Vector2>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(new Vector2(currentNode.X * map.tileRenderDimension + map.tileRenderDimension / 2, currentNode.Y * map.tileRenderDimension + map.tileRenderDimension / 2));
			currentNode = currentNode.Parent;
		}
		path.Reverse();
		return path;
	}
}


