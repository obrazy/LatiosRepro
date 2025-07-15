using Unity.Entities;

using Latios;
using Latios.Anna.Systems;

namespace Project.Bootstrap
{
    [UpdateInGroup(typeof(Latios.Systems.PreSyncPointGroup))]
    public partial class PreSyncRootSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
        }
    }

    [UpdateInGroup(typeof(Latios.Systems.LatiosWorldSyncGroup), OrderLast = true)]
    public partial class SyncPointRootSystems : RootSuperSystem
    {
        protected override void CreateSystems()
        {
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AnnaSuperSystem))]
    public partial class PreAnnaRootSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            this.GetOrCreateAndAddManagedSystem<GameplaySuperSystem>();
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AnnaSuperSystem))]
    [UpdateBefore(typeof(Latios.Transforms.Systems.TransformSuperSystem))]
    public partial class PostAnnaRootSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Latios.Transforms.Systems.TransformSuperSystem))]
    public partial class PostTransformRootSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
    public partial class PresentationRootSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
        }
    }
}
