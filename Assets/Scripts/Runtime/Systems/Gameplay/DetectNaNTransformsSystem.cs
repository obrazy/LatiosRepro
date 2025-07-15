using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

using Latios.Anna;
using Latios.Transforms;

namespace Project.Gameplay
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    public partial struct DetectNaNTransformsSystem : ISystem
    {
        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            new Job().ScheduleParallel();
        }

        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            [BurstCompile]
            public void Execute(
                Entity entity,
                in RigidBody rigidBody,
                in WorldTransform worldTransform)
            {
                if (math.any(math.isnan(worldTransform.position)))
                {
                    UnityEngine.Debug.Log(string.Format("Entity {0} has some NaN transform components", entity.Index));
                }
                if (math.any(math.isnan(rigidBody.velocity.linear)))
                {
                    UnityEngine.Debug.Log(string.Format("Entity {0} has some NaN linear velocity components", entity.Index));
                }
            }
        }
    }
}
