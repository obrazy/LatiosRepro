using Unity.Entities;
using Unity.Mathematics;

namespace Project.Gameplay
{
    public partial struct ReproConfig : IComponentData
    {
        public bool NaNRepro;
        public int SpawnGridSize;
        public int RigidBodySize;
        public Entity RigidBodyPrefab;
        public float3 BottomLeftRBImpulse;
    }

    public partial struct ApplyImpulseTag : IComponentData, IEnableableComponent
    {
    }

    public partial struct SpawnRigidBodiesRequestTag : IComponentData
    {
    }

    public partial struct UpdateColliderTag : IComponentData, IEnableableComponent
    {
    }
}
