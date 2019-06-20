using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PointCollisionSystem : ComponentSystem
{
    [BurstCompile]
    public struct CheckRaySpheresIntersection : IJobForEachWithEntity<PointCollider, Translation>
    {
        [ReadOnly] public Ray ray;
        public NativeQueue<Entity>.Concurrent collided;
 
        public void Execute(Entity entity, int index, ref PointCollider collider, ref Translation pos)
        {
            if (CheckIntersection(ray, collider, pos))
            {
                collided.Enqueue(entity);
            }
        }
    }
 
    public static bool CheckIntersection(Ray ray, PointCollider sphere, Translation sphereCenter)
    {
        float3 center = sphereCenter.Value + sphere.centerOffset;
        float3 dir = center - (float3)ray.origin;
 
        float dirSqLen = dir.x * dir.x + dir.y * dir.y + dir.z * dir.z;
        float pointSqRad     = sphere.radius * sphere.radius;
 
        float distanceAlongRay;
 
       if (dirSqLen < pointSqRad)
        {
            //point radius overlap with ray origin
            return true;
        }
 
        distanceAlongRay = math.dot(ray.direction, dir);
     
        if (distanceAlongRay < 0)
        {
            return false;
        }

        float dist = pointSqRad + distanceAlongRay * distanceAlongRay - dirSqLen;
 
        return !(dist < 0);
    }
 
    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            NativeQueue<Entity> collidedEntities = new NativeQueue<Entity>(Allocator.TempJob);
            var checkIntersectionJob = new CheckRaySpheresIntersection()
            {
                collided = collidedEntities.ToConcurrent(),
                ray      = mouseRay
            };
 
            checkIntersectionJob.Schedule(this).Complete();


            while (collidedEntities.Count > 0)
            {
                //Get first point
                if (collidedEntities.Count == 1)
                    SceneController.Instance.PointClickedEntity(collidedEntities.Dequeue());
                else
                {
                    //Debug.Log("Intersected entity: " + collidedEntities.Dequeue());
                    collidedEntities.Dequeue();
                }
            }
 
            collidedEntities.Dispose();
        }
    }
}