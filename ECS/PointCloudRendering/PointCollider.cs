using Unity.Entities;
using Unity.Mathematics;

public struct PointCollider : IComponentData
{
    public float radius;
    public float3 centerOffset;
}
