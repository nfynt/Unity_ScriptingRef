using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Accord.Math.Optimization.Losses;

public class ImageProcessor : MonoBehaviour
{
    public Camera mainCamera;
    public CameraBackground bgCam;
    public MeshFilter zImgTrackerPreview;

    public Material bgMat;
    public RawImage bgCopyImg;
    public RawImage bgCorrectedImg;

    private Texture2D bgCopyTex;
    private Matrix4x4 mHomographyMat;
    private RenderTexture bgAffineRT;
    private Material mHomoSampler;

    private Vector2[] mScreenCoord =
    //new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) }; //Model-space
    new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0) }; //Texture-space

    private void Start()
    {
        mHomoSampler = new Material(Shader.Find("Nfynt/HomographySampler"));
        if (mHomoSampler == null)
            Debug.LogError("Unable to find Nfynt/HomographySampler shader!");
    }
    
    public void Capture()
    {
        var bgTex = bgCam.GetCameraTexture;
        bgCopyTex = new Texture2D(bgTex.width,bgTex.height, TextureFormat.ARGB32, false);
        bgAffineRT = new RenderTexture(bgTex.width, bgTex.height, 0, RenderTextureFormat.ARGB32);
        //Debug.LogFormat("{0}x{1}",bgCopyTex.width,bgCopyTex.height);
        Graphics.CopyTexture(bgTex,0,0, bgCopyTex,0,0);
        bgMat.mainTexture = bgCopyTex;
        bgCopyImg.texture = bgCopyTex;

        EstimateHomography();
        bgCorrectedImg.texture = bgAffineRT;
    }

    void EstimateHomography()
    {
        Vector3 minBound = zImgTrackerPreview.mesh.bounds.min;
        Vector3 maxBound = zImgTrackerPreview.mesh.bounds.max;
        //Debug.LogFormat("{0}\t{1}", minBound, maxBound);
        Vector3[] previewBound = new Vector3[4];
        previewBound[0] = zImgTrackerPreview.transform.localToWorldMatrix.MultiplyPoint(minBound);
        previewBound[1] = zImgTrackerPreview.transform.localToWorldMatrix.MultiplyPoint(new Vector3(maxBound.x, minBound.y, minBound.z));
        previewBound[2] = zImgTrackerPreview.transform.localToWorldMatrix.MultiplyPoint(new Vector3(maxBound.x, minBound.y, maxBound.z));
        previewBound[3] = zImgTrackerPreview.transform.localToWorldMatrix.MultiplyPoint(new Vector3(minBound.x, minBound.y, maxBound.z));
        Debug.LogFormat("World corner position:\n{0}\n{1}\n{2}\n{3}", previewBound[0], previewBound[1], previewBound[2], previewBound[3]);
        //Transform world points to view-space
        for (int i = 0; i < 4; ++i)
        {
            previewBound[i] = mainCamera.WorldToViewportPoint(previewBound[i]);
            previewBound[i] = bgCam.GetTextureMatrix * new Vector4(previewBound[i].x, previewBound[i].y, 0, 1);
            //AddDot(ref bgCopyTex,
            //    new Vector2Int((int)(previewBound[i].x * bgCopyTex.width), (int)(previewBound[i].y * bgCopyTex.height)), 25,
            //    new Color(0,(50 * i + 50.0f) / 255.0f, 0));
        }
        Debug.LogFormat("View-space corners:\n{0}\n{1}\n{2}\n{3}", previewBound[0], previewBound[1], previewBound[2], previewBound[3]);
        
        mHomographyMat = Homography.CalcHomography4x4(
            new Vector2[] { previewBound[0], previewBound[1], previewBound[2], previewBound[3] },
            mScreenCoord
            ,true);

        Debug.Log("Homography:\n"+mHomographyMat.ToString());
        var currRT = RenderTexture.active;
        RenderTexture.active = bgAffineRT;
        GL.Clear(true, true, Color.magenta);
        mHomoSampler.SetMatrix("_Homography", mHomographyMat);
        Graphics.Blit(bgCopyTex,bgAffineRT,mHomoSampler);
        RenderTexture.active = currRT;
    }
    
    void AddDot(ref Texture2D tex, Vector2Int pos, int size, Color col)
    {
        int sh = size / 2;
        if (pos.x - sh < 0 || pos.x + sh >= tex.width || pos.y - sh < 0 || pos.y + sh >= tex.height) {
            Debug.Log("Point too close to corner");
            return;
        }
        Color32[] cols = tex.GetPixels32();
        for(int i = pos.x - sh; i < pos.x + sh; ++i)
        {
            for (int j = pos.y - sh; j < pos.y + sh; ++j)
            {
                //tex.SetPixel(i, j, col);
                cols[tex.width*j+i] = col;
            }
        }
        tex.SetPixels32(cols);
        tex.Apply();
    }
}



/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 

*/