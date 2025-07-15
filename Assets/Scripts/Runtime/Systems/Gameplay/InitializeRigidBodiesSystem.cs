using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

using Latios;
using Latios.Anna;
using Latios.Psyshock;

namespace Project.Gameplay
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    public partial struct InitializeRigidBodiesSystem : ISystem
    {
        private LatiosWorldUnmanaged latiosWorld;

        [BurstCompile]
        private void OnCreate(ref SystemState state)
        {
            this.latiosWorld = state.GetLatiosWorldUnmanaged();
        }

        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            ReproConfig reproConfig = this.latiosWorld.sceneBlackboardEntity.GetComponentData<ReproConfig>();
            state.Dependency = new GenerateColliderJob
            {
                ECB = this.latiosWorld.syncPoint.CreateEntityCommandBuffer().AsParallelWriter(),
                ReproConfig = reproConfig,
            }.ScheduleParallel(state.Dependency);
            state.Dependency = new SetInitialVelocityJob
            {
                ECB = this.latiosWorld.syncPoint.CreateEntityCommandBuffer().AsParallelWriter(),
                ReproConfig = reproConfig,
            }.Schedule(state.Dependency);
        }

        [BurstCompile]
        private partial struct GenerateColliderJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ECB;

            [ReadOnly]
            public ReproConfig ReproConfig;

            [BurstCompile]
            public void Execute(
                Entity entity,
                [ChunkIndexInQuery] int chunkIndexInQuery,
                ref RenderBounds renderBounds,
                ref Collider collider,
                EnabledRefRW<UpdateColliderTag> updateColliderTag)
            {
                updateColliderTag.ValueRW = false;

                NativeArray<float3> vertices = new(8, Allocator.Temp); // 8 vertices in a cube
                NativeArray<int3> indices = new(8, Allocator.Temp); // 2 triangles for each lateral face
                float halfSize = this.ReproConfig.RigidBodySize / 2f;

                vertices[0] = new float3(-halfSize, 0.5f, -halfSize);
                vertices[1] = new float3(-halfSize, 0.5f, halfSize);
                vertices[2] = new float3(halfSize, 0.5f, halfSize);
                vertices[3] = new float3(halfSize, 0.5f, -halfSize);
                vertices[4] = new float3(-halfSize, -0.5f, -halfSize);
                vertices[5] = new float3(-halfSize, -0.5f, halfSize);
                vertices[6] = new float3(halfSize, -0.5f, halfSize);
                vertices[7] = new float3(halfSize, -0.5f, -halfSize);

                indices[0] = new int3(0, 4, 5);
                indices[1] = new int3(0, 5, 1);
                indices[2] = new int3(1, 5, 6);
                indices[3] = new int3(1, 6, 2);
                indices[4] = new int3(2, 6, 7);
                indices[5] = new int3(2, 7, 3);
                indices[6] = new int3(3, 7, 4);
                indices[7] = new int3(3, 4, 0);

                renderBounds.Value = new AABB
                {
                    Center = float3.zero,
                    Extents = new float3(halfSize, 0.5f, halfSize)
                };

                TriMeshCollider triMeshCollider = collider;
                BlobBuilder blobBuilder = new(Allocator.Temp);
                triMeshCollider.triMeshColliderBlob = Latios.Psyshock.TriMeshColliderBlob.BuildBlob(
                    ref blobBuilder,
                    vertices.AsReadOnlySpan(),
                    indices.AsReadOnlySpan(),
                    "TriMeshCollider",
                    Allocator.Persistent
                );

                this.ECB.SetComponent<Collider>(chunkIndexInQuery, entity, triMeshCollider);
            }
        }

        [BurstCompile]
        private partial struct SetInitialVelocityJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ECB;

            [ReadOnly]
            public ReproConfig ReproConfig;

            [BurstCompile]
            public void Execute(
                ref DynamicBuffer<AddImpulse> addImpulses,
                EnabledRefRW<ApplyImpulseTag> _)
            {
                addImpulses.Add(new AddImpulse(this.ReproConfig.BottomLeftRBImpulse));
            }
        }
    }
}
