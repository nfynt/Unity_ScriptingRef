/// <summary>
	/// Rotates the texture cw/ccw direction by 90deg
	/// </summary>
	void RotateTexture(ref Texture2D tex, bool cw)
	{
		Color32[] src = tex.GetPixels32();
		int srcH = tex.height;
		int srcW = tex.width;
		Color32[] target = new Color32[srcW * srcH];

		if (cw)
		{
			for (int y = 0; y < srcH; y++)
				for (int x = 0; x < srcW; x++)
					target[y + (srcW - 1 - x) * srcH] = src[x + y * srcW]; //CW
		}
		else
		{
			for (int y = 0; y < srcH; y++)
				for (int x = 0; x < srcW; x++)
					target[(srcH - 1) - y + x * srcH] = src[x + y * srcW]; //CCW
		}
		
		tex = new Texture2D(srcH, srcW, tex.format, false);
		tex.SetPixels32(target);
		tex.Apply();
	}
