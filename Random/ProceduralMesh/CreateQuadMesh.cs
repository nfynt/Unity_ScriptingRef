using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CreateQuadMesh : MonoBehaviour
{
    private Mesh m_QuadMesh = null;
    private MeshFilter m_MeshFilter;
    private Renderer m_QuadRenderer;
    [SerializeField]
    private Material m_QuadMaterial;

    [SerializeField]
    private Vector2 m_Scale = new Vector2(1f,1f);

    [SerializeField]
    private bool m_QuadType = true;

    [SerializeField]
    private bool m_RemapUv = false;

    [SerializeField]
    private float ratio = 1f;
    void Awake()
    {
        m_QuadRenderer = GetComponent<MeshRenderer>();
        m_MeshFilter = GetComponent < MeshFilter >();

        if ( m_QuadRenderer == null )
            m_QuadRenderer = gameObject.AddComponent < MeshRenderer >();
        if (m_MeshFilter == null)
            m_MeshFilter = gameObject.AddComponent<MeshFilter>();
    }

    void Start()
    {
        m_QuadMesh = m_QuadType ? GenerateMesh(m_RemapUv) : GeneratePlaneMesh(m_RemapUv);
        m_QuadMaterial = CreateMaterial();

        m_QuadRenderer.material = m_QuadMaterial;
        m_MeshFilter.mesh = m_QuadMesh;
    }

    private void OnEnable()
    {
        if(m_QuadRenderer==null)
            return;

        if ( m_QuadMesh == null )
        {
            m_QuadMesh = m_QuadType ? GenerateMesh(m_RemapUv) : GeneratePlaneMesh(m_RemapUv);
        }

        m_QuadRenderer.enabled = true;
    }

    private void OnDisable()
    {
        if (m_QuadRenderer == null)
            return;

        m_QuadRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        DestroyImmediate( m_MeshFilter );
        DestroyImmediate( m_QuadRenderer );
    }

    private void OnValidate()
    {
        if ( m_QuadRenderer == null || m_MeshFilter == null )
        {
            return;
        }

        m_QuadMesh = m_QuadType ? GenerateMesh(m_RemapUv) : GeneratePlaneMesh(m_RemapUv);
        m_QuadMaterial = CreateMaterial();

        m_QuadRenderer.material = m_QuadMaterial;
        m_MeshFilter.mesh = m_QuadMesh;
    }

    private Mesh GeneratePlaneMesh(bool remapUv = false)
    {
        Vector3[] vert = new Vector3[8];
        Vector3[] norm = new Vector3[8];
        int[] tris = new int[12];
        Vector2[] uvs = new Vector2[8];

        vert[0] = new Vector3(0, 0, 0);
        vert[1] = new Vector3(m_Scale.x, 0, 0);
        vert[2] = new Vector3(m_Scale.x, m_Scale.y, 0);
        vert[3] = new Vector3(0, m_Scale.y, 0);
        vert[4] = new Vector3(0, m_Scale.y, 0);
        vert[5] = new Vector3(m_Scale.x, m_Scale.y, 0);
        vert[6] = new Vector3(m_Scale.x, 2*m_Scale.y, 0);
        vert[7] = new Vector3(0, 2*m_Scale.y, 0);

        tris[0] = 0;
        tris[1] = 2;
        tris[2] = 1;
        tris[3] = 0;
        tris[4] = 3;
        tris[5] = 2;
        tris[6] = 4;
        tris[7] = 6;
        tris[8] = 5;
        tris[9] = 4;
        tris[10] = 7;
        tris[11] = 6;

        Vector3 normal = Vector3.Cross(vert[1] - vert[0], vert[2] - vert[0]).normalized;
        norm[0] = normal;
        norm[1] = normal;
        norm[2] = normal;
        norm[3] = normal;
        norm[4] = normal;
        norm[5] = normal;
        norm[6] = normal;
        norm[7] = normal;

        if (remapUv)
        {
            //using 1:1 ratio for border width/height;
            ratio = Vector3.Distance(vert[0], vert[1]) / Vector3.Distance(vert[1], vert[2]);

            float xOff = 0f, yOff = 0f;

            if (ratio > 1f)
            {
                xOff = 0.1f - 0.1f / ratio;
            }
            else if (ratio < 1f)
            {
                yOff = 0.1f - 0.1f * ratio;
            }

            uvs[0] = new Vector2(xOff, yOff);
            uvs[1] = new Vector2(1.0f - xOff, yOff);
            uvs[2] = new Vector2(1.0f - xOff, 1.0f - yOff);
            uvs[3] = new Vector2(xOff, 1.0f - yOff);
            uvs[4] = new Vector2(0, 0);
            uvs[5] = new Vector2(1, 0);
            uvs[6] = new Vector2(1, 1);
            uvs[7] = new Vector2(0, 1);
        }
        else
        {
            uvs[0] = new Vector2(0, 0);
            uvs[1] = new Vector2(1, 0);
            uvs[2] = new Vector2(1, 1);
            uvs[3] = new Vector2(0, 1);
            uvs[4] = new Vector2(0, 0);
            uvs[5] = new Vector2(1, 0);
            uvs[6] = new Vector2(1, 1);
            uvs[7] = new Vector2(0, 1);
        }

        

        Mesh mesh = new Mesh { vertices = vert, normals = norm, triangles = tris, uv = uvs };

        return mesh;
    }

    private Mesh GenerateMesh(bool remapUv=false)
    {
        Vector3[] vert = new Vector3[4];
        Vector3[] norm = new Vector3[4];
        int[] tris = new int[6];
        Vector2[] uvs = new Vector2[4];

        vert[0] = new Vector3( 0, 0, 0 );
        vert[1] = new Vector3(m_Scale.x, 0, 0);
        vert[2] = new Vector3(m_Scale.x, m_Scale.y, 0);
        vert[3] = new Vector3(0, m_Scale.y, 0);

        tris[0] = 0;
        tris[1] = 2;
        tris[2] = 1;
        tris[3] = 0;
        tris[4] = 3;
        tris[5] = 2;

        Vector3 normal = Vector3.Cross(vert[1] - vert[0], vert[2] - vert[0]).normalized;
        norm[0] = normal;
        norm[1] = normal;
        norm[2] = normal;
        norm[3] = normal;

        if (remapUv)
        {
            //using 1:1 ratio for border width/height;
            ratio = Vector3.Distance( vert[0], vert[1] ) / Vector3.Distance( vert[1], vert[2] );
            
            float xOff=0f, yOff=0f;

            if ( ratio > 1f )
            {
                xOff = 0.1f - 0.1f / ratio ;
            }else if ( ratio < 1f )
            {
                yOff = 0.1f - 0.1f * ratio;
            }

            uvs[0] = new Vector2(xOff , yOff);
            uvs[1] = new Vector2(1.0f - xOff , yOff );
            uvs[2] = new Vector2(1.0f - xOff , 1.0f - yOff);
            uvs[3] = new Vector2(xOff , 1.0f - yOff);
        }
        else
        {
            uvs[0] = new Vector2(0, 0);
            uvs[1] = new Vector2(1, 0);
            uvs[2] = new Vector2(1, 1);
            uvs[3] = new Vector2(0, 1);
        }
        

        Mesh mesh = new Mesh { vertices = vert, normals = norm, triangles = tris, uv = uvs };

        return mesh;
    }
    
    private Material CreateMaterial()
    {
        if (m_QuadMaterial != null)
            return m_QuadMaterial;

        return new Material(Shader.Find("Standard"));
    }

}
