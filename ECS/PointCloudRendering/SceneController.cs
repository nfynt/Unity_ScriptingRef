using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.Rendering;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [SerializeField] private int size = 10;
    [SerializeField] private int maxXPos = 10;
    [SerializeField] private float pointRad = 1;
    [SerializeField] Mesh objMesh;
    [SerializeField] Material objMat;
    EntityManager entityMgr;
    NativeArray<Entity> entityArray;
    Vector3[] pointPositions;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        pointPositions = new Vector3[size];

        for (int i = 0; i < pointPositions.Length; i++)
            pointPositions[i] = new Vector3(Random.Range(-maxXPos, maxXPos), Random.Range(-maxXPos, maxXPos), Random.Range(-maxXPos, maxXPos));

        entityMgr = World.Active.EntityManager;
        EntityArchetype entityArchetype = entityMgr.CreateArchetype(
            typeof(PointData),
            typeof(PointCollider),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            );

        entityArray = new NativeArray<Entity>(size, Allocator.Persistent);
        entityMgr.CreateEntity(entityArchetype, entityArray);

        for(int i=0;i<entityArray.Length;i++)
        {
            Entity entity = entityArray[i];
            entityMgr.SetComponentData(entity, new PointData
            {
                currPos = pointPositions[i]
            });

            entityMgr.SetComponentData(entity, new PointCollider
            {
                //centerOffset = Vector3.zero,
                radius = pointRad
            });

            entityMgr.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = objMesh,
                material = objMat,
            });
        }
    }

    public void PointClickedEntity(Entity entity)
    {
        Vector3 pos = entityMgr.GetComponentData<PointData>(entity).currPos;

        Debug.Log("Clicked Pos: " + pos);
    }

   // float startTime;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // startTime = Time.realtimeSinceStartup;

            UpdatePoints();
           
            //Debug.Log(((Time.realtimeSinceStartup - startTime)*1000).ToString() + "ms");
        }
    }

    private void UpdatePoints()
    {
        for (int i = 0; i < pointPositions.Length; i++)
        {
            pointPositions[i] = new Vector3(Random.Range(-maxXPos, maxXPos), Random.Range(-maxXPos, maxXPos), Random.Range(-maxXPos, maxXPos));

            Entity entity = entityArray[i];
            entityMgr.SetComponentData(entity, new PointData
            {
                currPos = pointPositions[i]
            });
        }
    }

    private void OnDisable()
    {
        entityArray.Dispose();
    }
}
