using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using Latios;
using Latios.Transforms;

namespace Project.Gameplay
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    public partial struct SpawnRigidBodiesSystem : ISystem, ISystemNewScene
    {
        private LatiosWorldUnmanaged latiosWorld;
        private EntityQuery spawnRequestQuery;

        [BurstCompile]
        private void OnCreate(ref SystemState state)
        {
            this.latiosWorld = state.GetLatiosWorldUnmanaged();
            this.spawnRequestQuery = state.Fluent().With<SpawnRigidBodiesRequestTag>(true).Build();
        }

        public void OnNewScene(ref SystemState state)
        {
            this.latiosWorld.syncPoint.CreateEntityCommandBuffer().AddComponent<SpawnRigidBodiesRequestTag>(
                this.latiosWorld.sceneBlackboardEntity);
        }

        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            ReproConfig reproConfig = this.latiosWorld.sceneBlackboardEntity.GetComponentData<ReproConfig>();

            NativeList<float2> spawnPositions = new(Allocator.TempJob);
            int gap = math.select(2, -2, reproConfig.NaNRepro);
            float cellSize = reproConfig.RigidBodySize + gap;

            int rows = reproConfig.SpawnGridSize;
            int cols = rows;
            int2 bottomLeftCellCoordinates = new(-cols / 2, -rows / 2);

            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    int2 cellCoordinates = bottomLeftCellCoordinates + new int2(row, col);
                    spawnPositions.Add((float2)cellCoordinates * cellSize);
                }
            }

            JobHandle jobHandle = new Job
            {
                ECB = this.latiosWorld.syncPoint.CreateEntityCommandBuffer().AsParallelWriter(),
                SpawnPositions = spawnPositions,
                ReproConfig = reproConfig,
            }.Schedule(spawnPositions.Length, 8, state.Dependency);
            state.Dependency = jobHandle;
            spawnPositions.Dispose(jobHandle);

            this.latiosWorld.syncPoint.CreateEntityCommandBuffer().RemoveComponent<SpawnRigidBodiesRequestTag>(
                this.latiosWorld.sceneBlackboardEntity);
        }

        [BurstCompile]
        private partial struct Job : IJobParallelFor
        {
            public EntityCommandBuffer.ParallelWriter ECB;

            [ReadOnly]
            public NativeList<float2> SpawnPositions;

            [ReadOnly]
            public ReproConfig ReproConfig;

            public void Execute(int index)
            {
                Entity rigidBodyEntity = this.ECB.Instantiate(index, this.ReproConfig.RigidBodyPrefab);
                TransformQvvs transformQvvs = new(this.SpawnPositions[index].x0y(), quaternion.identity);
                this.ECB.SetComponent(index, rigidBodyEntity, new WorldTransform { worldTransform = transformQvvs });
                this.ECB.AddComponent<UpdateColliderTag>(index, rigidBodyEntity);

                if (index == 0)
                {
                    // Tag the bottom left RB for adding impulse
                    this.ECB.AddComponent<ApplyImpulseTag>(index, rigidBodyEntity);
                }
            }
        }
    }
}
