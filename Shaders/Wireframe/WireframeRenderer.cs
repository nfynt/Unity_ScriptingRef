using UnityEngine;
using UnityEngine.Rendering;

namespace Nfynt
{

    [ExecuteAlways]
    public class WireframeRenderer : MonoBehaviour
    {
        public bool ShowBackFaces = true;
        public Color LineColor = Color.black;
        public float LineSize = 0.3f;
        public bool ShowMainRenderer = true;


        private Renderer m_MainRenderer;
        private Mesh m_WireMesh;
        private Renderer m_WireMeshRenderer;
        private Material m_CulledWireMat;
        private Material m_NotCulledWireMat;

        #region Unity Event Functions

        private void OnDestroy()
        {
            if ( m_WireMeshRenderer != null )
            {
                if ( Application.isPlaying )
                {
                    Destroy( m_WireMeshRenderer.gameObject );
                }
                else
                {
                    DestroyImmediate( m_WireMeshRenderer.gameObject );
                }

                m_WireMeshRenderer = null;
            }
        }

        private void OnDisable()
        {
            if ( m_WireMeshRenderer != null )
            {
                m_MainRenderer.enabled = true;
                m_WireMeshRenderer.enabled = false;
            }
        }

        private void OnEnable()
        {
            if ( m_WireMeshRenderer != null )
            {
                m_MainRenderer.enabled = ShowMainRenderer;
                m_WireMeshRenderer.enabled = true;

                OnValidate();
            }
        }

        private void OnValidate()
        {
            if ( m_WireMeshRenderer == null )
            {
                return;
            }

            CreateMaterials();

            // update wireframe material
            m_WireMeshRenderer.material = ShowBackFaces ? m_NotCulledWireMat : m_CulledWireMat;

            // update line color
            m_WireMeshRenderer.sharedMaterial.SetColor( "_LineColor", LineColor );

            // update line size
            m_WireMeshRenderer.sharedMaterial.SetFloat("_LineSize", LineSize);

            // update shade for original renderer
            m_MainRenderer.enabled = ShowMainRenderer;
        }

        private void Start()
        {
            ValidateMeshFilter();
        }

        #endregion

        #region Private

        private void CreateMaterials()
        {
            if ( ShowBackFaces && m_NotCulledWireMat == null )
            {
                Shader shader = Shader.Find("Nfynt/MeshWireframeNotCulled");
                m_NotCulledWireMat = new Material( shader );
            }
            else if ( !ShowBackFaces && m_CulledWireMat == null )
            {
                Shader shader = Shader.Find("Nfynt/MeshWireframeCulled");
                m_CulledWireMat = new Material( shader );
            }
        }

        private void CreateWireframeRenderer()
        {
            GameObject go = new GameObject( "WireframeRenderer" );
            go.transform.SetParent( m_MainRenderer.transform );
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            go.AddComponent < MeshFilter >().mesh = m_WireMesh;
            m_WireMeshRenderer = go.AddComponent < MeshRenderer >();
        }

        private Mesh GetProcessedMesh( Mesh mesh )
        {
            uint maxTotalVertices = mesh.indexFormat == IndexFormat.UInt16 ? 65534 : 4000000000;

            int[] meshTriangles = mesh.triangles;
            Vector3[] meshVertices = mesh.vertices;
            Vector3[] meshNormals = mesh.normals;

            int vertexCount = meshTriangles.Length;

            if ( vertexCount > maxTotalVertices )
            {
                Debug.LogError( "number of vertices surpass unity vertex limit!" );

                return null;
            }

            Mesh processedMesh = new Mesh();

            Vector3[] processedVertices = new Vector3[vertexCount];

            var processedUVs = new Vector2[vertexCount];
            int[] processedTriangles = new int[meshTriangles.Length];
            Vector3[] processedNormals = new Vector3[vertexCount];

            for ( int i = 0; i < meshTriangles.Length; i += 3 )
            {
                processedVertices[i] = meshVertices[meshTriangles[i]];
                processedVertices[i + 1] = meshVertices[meshTriangles[i + 1]];
                processedVertices[i + 2] = meshVertices[meshTriangles[i + 2]];

                processedTriangles[i] = i;
                processedTriangles[i + 1] = i + 1;
                processedTriangles[i + 2] = i + 2;

                processedNormals[i] = meshNormals[meshTriangles[i]];
                processedNormals[i + 1] = meshNormals[meshTriangles[i + 1]];
                processedNormals[i + 2] = meshNormals[meshTriangles[i + 2]];

                processedUVs[i] = new Vector2(0f, 0f);
                processedUVs[i + 1] = new Vector2(1f, 0f);
                processedUVs[i + 2] = new Vector2(0f, 1f);
            }

            processedMesh.vertices = processedVertices;

            processedMesh.uv = processedUVs;
            processedMesh.triangles = processedTriangles;
            processedMesh.normals = processedNormals;

            return processedMesh;
        }

        private void ValidateMeshFilter()
        {
            if ( m_WireMeshRenderer == null )
            {
                Mesh originalMesh = null;

                MeshFilter meshFilter = GetComponentInChildren < MeshFilter >();

                if ( meshFilter != null )
                {
                    originalMesh = meshFilter.sharedMesh;
                    m_MainRenderer = meshFilter.GetComponent < Renderer >();
                }

                if ( originalMesh == null )
                {
                    Debug.LogError( "No mesh filter/renderer found" );
                    enabled = false;

                    return;
                }

                m_WireMesh = GetProcessedMesh( originalMesh );

                if ( m_WireMesh == null )
                {
                    return;
                }

                CreateWireframeRenderer();
                CreateMaterials();
            }

            OnValidate();
        }

        #endregion
    }

}
