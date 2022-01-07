using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class CylindricalTarget : MonoBehaviour
{
    public float Radius = 1;
    public float Height = 2;
    public int Strips = 20;
    public bool Curve = true;
    [Tooltip("Length in meters")]
    public float TexLen = 1f;

    private MeshFilter m_meshFilter;
    private Mesh m_mesh;

    [HideInInspector]
    private int m_hstrips = 5;
    [HideInInspector]
    private float m_hrad, m_hheight;
    private bool m_hcurve;
    private float m_hTexLen;

    private void Start()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_mesh = m_meshFilter.sharedMesh ?? new Mesh();
    }

    private void OnValidate()
    {
        if (m_mesh == null) return;

        if (m_hstrips != Strips || m_hrad == Radius || m_hheight == Height || m_hcurve!=Curve || m_hTexLen!=TexLen)
        {
            Debug.Log("Updating mesh");
            m_mesh.name = "Cylinder_" + (Curve ? "curve" : "flat");
            if (UpdateMesh(m_mesh, Radius, Height, Curve, TexLen, Strips))
            {
                m_hstrips = Strips;
                m_hrad = Radius;
                m_hheight = Height;
                m_hcurve = Curve;
                m_hTexLen = TexLen;
            }
            m_meshFilter.sharedMesh = m_mesh;
        }
    }

    private bool UpdateMesh(Mesh m, float rad, float height, bool curve, float texLen, int vstrips = 5)
    {
        float width = 2f * Mathf.PI * rad;

        if(texLen>width || texLen<=0f)
        {
            Debug.Log("Inavlid texture length");
            return false;
        }

        int vcount = 2 * vstrips + 2;
        int pcount = 2 * vstrips;
        float texScale = width / texLen;

        Vector3[] verts = m.vertices?.Length == vcount ? m.vertices : new Vector3[vcount];
        Vector3[] norms = m.normals?.Length == vcount ? m.normals : new Vector3[vcount];
        int[] tris = m.triangles?.Length == 3 * pcount ? m.triangles : new int[3 * pcount];
        Vector2[] uvs = m.uv?.Length == vcount ? m.uv : new Vector2[vcount];

        float dtheta = Mathf.Deg2Rad * (360f / pcount);
        float theta = 0f;

        for (int i = 0; i < vcount; ++i)
        {
            if (i % 2 == 0)
            {
                //bottom strip
                if (curve)
                {
                    verts[i] = new Vector3(Mathf.Cos(theta) * rad, 0f, Mathf.Sin(theta) * rad);
                    norms[i] = verts[i].normalized;
                    theta += dtheta;
                }
                else
                {
                    verts[i] = new Vector3(width * ((float)i / (vcount - 2f)), 0f, 0.0f);
                    norms[i] = new Vector3(0f, 0f, -1f);
                }
                uvs[i] = new Vector2((i / (vcount - 2f)) * texScale, 0f);
            }
            else
            {
                //top strip
                if (curve)
                {
                    verts[i] = new Vector3(Mathf.Cos(theta - dtheta) * rad, height, Mathf.Sin(theta - dtheta) * rad);
                    norms[i] = verts[i].normalized;
                    theta += dtheta;
                }
                else
                {
                    verts[i] = new Vector3(width * ((float)i - 1f) / (vcount - 2f), height, 0.0f);
                    norms[i] = new Vector3(0f, 0f, -1f);
                }
                uvs[i] = new Vector2(((i - 1f) / (vcount - 2f)) * texScale, 1f);
            }
        }

        int ind = 0;
        for (int i = 0; i < pcount; ++i)
        {
            tris[ind++] = i + 0;
            tris[ind++] = i + (i % 2 == 0 ? 1 : 2);
            tris[ind++] = i + (i % 2 == 0 ? 2 : 1);
        }

        m.vertices = verts;
        m.normals = norms;
        m.triangles = tris;
        m.uv = uvs;

        return true;
    }
}
