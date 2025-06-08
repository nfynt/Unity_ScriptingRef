using UnityEngine;

namespace Nfynt{
public class TextureBlur : MonoBehaviour
{
	public Camera MainCamera;
	public ComputeShader CompShader;
	[Range(0, 10)]
	public int BlurSize = 1;
	public RenderTexture ResultRT;

	public bool InvokeShader=false;

	private void Start()
	{
		if (ResultRT != null)
		{
			ResultRT.enableRandomWrite = true;
		}
		else
		{
			ResultRT = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
			ResultRT.enableRandomWrite = true;
			ResultRT.wrapMode = TextureWrapMode.Clamp;
			ResultRT.Create();
		}
	}

	public void Update()
	{
		if (InvokeShader && ResultRT!=null && CompShader!=null)
		{
			int kernel = CompShader.FindKernel("CSMain");
			CompShader.SetTexture(kernel,"InputTex",MainCamera.activeTexture);
			CompShader.SetTexture(kernel,"Result",ResultRT);
			CompShader.SetInt("KernelSize",BlurSize);
			CompShader.Dispatch(kernel, ResultRT.width / 8, ResultRT.height / 8, 1);
			//InvokeShader = false;
		}
	}
}
}




/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 

*/
