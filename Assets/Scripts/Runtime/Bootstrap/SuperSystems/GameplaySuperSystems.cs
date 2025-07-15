using Latios;

using Project.Gameplay;

namespace Project.Bootstrap
{
    public partial class GameplaySuperSystem : SuperSystem
    {
        protected override void CreateSystems()
        {
            this.GetOrCreateAndAddUnmanagedSystem<SpawnRigidBodiesSystem>();
            this.GetOrCreateAndAddUnmanagedSystem<InitializeRigidBodiesSystem>();
            this.GetOrCreateAndAddManagedSystem<DrawCollidersSystem>();
            this.GetOrCreateAndAddUnmanagedSystem<DetectNaNTransformsSystem>();
        }
    }
}
