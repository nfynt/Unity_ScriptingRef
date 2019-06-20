using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PointUpdateSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PointData pointData, ref Translation translation) => {
            translation.Value = pointData.currPos;
        });
    }
}
