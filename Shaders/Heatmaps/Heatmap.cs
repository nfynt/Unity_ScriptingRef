using UnityEngine;
using System.Collections;

namespace Nfynt
{
	public class Heatmap : MonoBehaviour
	{
		public Vector4[] positions;
		public Vector4[] properties;

		public Material material;

		public int count = 10;

		void Start()
		{
			positions = new Vector4[count];
			properties = new Vector4[count];

			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = new Vector4(Random.Range(-5f, +5f), Random.Range(-5f, +5f), 0, 0);
				properties[i] = new Vector4(0.5f, Random.Range(-0.25f, 1f), 0, 0);
			}
		}

		void FixedUpdate()
		{
			for (int i = 0; i < positions.Length; i++)
			{
				//positions[i] += new Vector4(Random.Range(-01f, +01f), Random.Range(-01f, +01f), 0, 0) * Time.deltaTime;
				properties[i] = new Vector4(1f, Random.Range(-0.25f, 1f), 0, 0);
			}

			material.SetInt("_Points_Length", count);
			material.SetVectorArray("_Points", positions);
			material.SetVectorArray("_Properties", properties);
		}
	}
}