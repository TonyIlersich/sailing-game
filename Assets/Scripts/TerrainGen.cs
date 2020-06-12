using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
	public Vector3Int size = new Vector3Int(20, 5, 20);

	Mesh mesh;

	void Start()
	{
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		Generate();
	}

	void Generate()
	{
		float[,] heightMap = new float[size.x + 1, size.z + 1];
		for (int level = 0, levelCount = 16; level < levelCount; level++)
		{
			int[,,] tiles = new int[size.x + 1, size.z + 1, 2];
			for (int z = 0; z <= size.z; z++)
			{
				for (int x = 0; x <= size.x; x++)
				{
					tiles[x, z, 0] = Random.Range(0, 2);
				}
			}
			for (int i = 0; i < 10; i++)
			{
				for (int z = 0; z <= size.z; z++)
				{
					for (int x = 0; x <= size.x; x++)
					{
						int sum = 0;
						int denom = 9;
						for (int dx = -1; dx <= 1; dx++) {
							for (int dz = -1; dz <= 1; dz++) {
								try {
									sum += tiles[x + dx, z + dz, i % 2];
								} catch(System.IndexOutOfRangeException) {
									denom--;
								}
							}
						}
						tiles[x, z, (i + 1) % 2] = sum > denom / 2 ? 1 : 0;
					}
				}
			}
			for (int z = 0; z <= size.z; z++)
			{
				for (int x = 0; x <= size.x; x++)
				{
					heightMap[x, z] += tiles[x, z, 0] / (float)levelCount;
				}
			}
		}
		Vector3[] vertices = new Vector3[(size.x + 1) * (size.z + 1)];
		for (int i = 0, z = 0; z <= size.z; z++)
		{
			for (int x = 0; x <= size.x; x++)
			{
				float height = heightMap[x, z] * .75f + Mathf.PerlinNoise(x * 10f, z * 10f) * .25f;
				vertices[i++] = new Vector3(x, height * size.y, z);
			}
		}
		int[] triangles = new int[6 * size.x * size.z];
		for (int i = 0, v = 0, z = 0; z < size.z; z++)
		{
			for (int x = 0; x < size.x; x++, v++)
			{
				triangles[i++] = v;
				triangles[i++] = v + size.x + 1;
				triangles[i++] = v + 1;
				triangles[i++] = v + 1;
				triangles[i++] = v + size.x + 1;
				triangles[i++] = v + size.x + 2;
			}
			v++;
		}
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();
	}
}
