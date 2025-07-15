using Unity.Entities;
using UnityEngine;

namespace Project.Gameplay
{
    public class ReproConfigAuthoring : MonoBehaviour
    {
        public class Baker : Baker<ReproConfigAuthoring>
        {
            public override void Bake(ReproConfigAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.None);
                this.AddComponent(entity, new ReproConfig
                {
                    NaNRepro = authoring.NaNRepro,
                    SpawnGridSize = authoring.SpawnGridSize,
                    RigidBodySize = authoring.RigidBodySize,
                    RigidBodyPrefab = this.GetEntity(authoring.RigidBodyPrefab, TransformUsageFlags.Dynamic),
                    BottomLeftRBImpulse = authoring.BottomLeftRBImpulse,
                });
            }
        }

        public bool NaNRepro;
        public int SpawnGridSize;
        public int RigidBodySize;
        public GameObject RigidBodyPrefab;
        public Vector3 BottomLeftRBImpulse;
    }
}
