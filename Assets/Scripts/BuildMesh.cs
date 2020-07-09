using UnityEngine;

public class BuildMesh : MonoBehaviour 
{
	// for flexibility of constructing different meshes (not only pyramid mesh)
	// use cube mesh template and assign its top vertices 
	public Vector3 vertLeftTopFront  = new Vector3(-1,1,1);
	public Vector3 vertRightTopFront = new Vector3(1,1,1);
	public Vector3 vertRightTopBack  = new Vector3(1,1,-1);
	public Vector3 vertLeftTopBack   = new Vector3(-1,1,-1);

	// Use this for initialization
	void Start () 
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;

		// vertices
		Vector3[] vertices = new Vector3[]
		{
			// front 
			vertLeftTopFront,     // 0
			vertRightTopFront,    // 1
			new Vector3(-1,-1,1), // 2
			new Vector3(1,-1,1),  // 3

			// back 
			vertRightTopBack,     // 4
			vertLeftTopBack,      // 5
			new Vector3(1,-1,-1), // 6
			new Vector3(-1,-1,-1),// 7

			// left
			vertLeftTopBack,      // 8
			vertLeftTopFront,     // 9
			new Vector3(-1,-1,-1),// 10
			new Vector3(-1,-1,1), // 11

			// right
			vertRightTopFront,    // 12
			vertRightTopBack,     // 13
			new Vector3(1,-1,1),  // 14
			new Vector3(1,-1,-1), // 15

			// top
			vertLeftTopBack,      // 16
			vertRightTopBack,     // 17
			vertLeftTopFront,     // 18
			vertRightTopFront,    // 19

			// bottom
			new Vector3(-1,-1,1), // 20
			new Vector3(1,-1,1),  // 21
			new Vector3(-1,-1,-1),// 22
			new Vector3(1,-1,-1)  // 23

		};

		// triangles
		int[] triangles = new int[]
		{
			// front 
			0,2,3,
			3,1,0,

			// back 
			4,6,7,
			7,5,4,

			// left 
			8,10,11,
			11,9,8,

			// right
			12,14,15,
			15,13,12,

			// top 
			16,18,19,
			19,17,16,

			// bottom 
			20,22,23,
			23,21,20
		};

		// uvs
		Vector2[] uvs = new Vector2[]
		{
			new Vector2(0,1),
			new Vector2(0,0),
			new Vector2(1,1),
			new Vector2(1,0),

			new Vector2(0,1),
			new Vector2(0,0),
			new Vector2(1,1),
			new Vector2(1,0),

			new Vector2(0,1),
			new Vector2(0,0),
			new Vector2(1,1),
			new Vector2(1,0),

			new Vector2(0,1),
			new Vector2(0,0),
			new Vector2(1,1),
			new Vector2(1,0),

			new Vector2(0,1),
			new Vector2(0,0),
			new Vector2(1,1),
			new Vector2(1,0),

			new Vector2(0,1),
			new Vector2(0,0),
			new Vector2(1,1),
			new Vector2(1,0)
		};

		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.Optimize();
		mesh.RecalculateNormals();
	
	}
	
}















