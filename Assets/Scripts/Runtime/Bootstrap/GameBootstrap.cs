using Unity.Collections;
using Unity.Entities;

using Latios;
using Latios.Anna.Systems;
using Latios.Authoring;
using Latios.Psyshock;

namespace Project.Bootstrap
{
    [UnityEngine.Scripting.Preserve]
    public class LatiosBakingBootstrap : ICustomBakingBootstrap
    {
        public void InitializeBakingForAllWorlds(ref CustomBakingBootstrapContext context)
        {
            Latios.Authoring.CoreBakingBootstrap.ForceRemoveLinkedEntityGroupsOfLength1(ref context);
            Latios.Transforms.Authoring.TransformsBakingBootstrap.InstallLatiosTransformsBakers(ref context);
            Latios.Psyshock.Authoring.PsyshockBakingBootstrap.InstallUnityColliderBakers(ref context);
            Latios.Kinemation.Authoring.KinemationBakingBootstrap.InstallKinemation(ref context);
            // Latios.Unika.Authoring.UnikaBakingBootstrap.InstallUnikaEntitySerialization(ref context);
        }
    }

    [UnityEngine.Scripting.Preserve]
    public class LatiosEditorBootstrap : ICustomEditorBootstrap
    {
        public World Initialize(string defaultEditorWorldName)
        {
            LatiosWorld world = new(defaultEditorWorldName, WorldFlags.Editor)
            {
                useExplicitSystemOrdering = true
            };

            NativeList<SystemTypeIndex> systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default, true);
            BootstrapTools.InjectUnitySystems(systems, world, world.simulationSystemGroup);

            Latios.Transforms.TransformsBootstrap.InstallTransforms(world, world.simulationSystemGroup);
            Latios.Kinemation.KinemationBootstrap.InstallKinemation(world);
            Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphics(world);

            BootstrapTools.InjectRootSuperSystems(systems, world, world.simulationSystemGroup);

            return world;
        }
    }

    [UnityEngine.Scripting.Preserve]
    public class LatiosBootstrap : ICustomBootstrap
    {
        public bool Initialize(string defaultWorldName)
        {
            LatiosWorld world = new(defaultWorldName);
            World.DefaultGameObjectInjectionWorld = world;
            world.useExplicitSystemOrdering = true;

            NativeList<SystemTypeIndex> systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default);

            BootstrapTools.InjectUnitySystems(systems, world, world.simulationSystemGroup);

            Latios.CoreBootstrap.InstallSceneManager(world);
            Latios.Transforms.TransformsBootstrap.InstallTransforms(world, world.simulationSystemGroup);
            Latios.Myri.MyriBootstrap.InstallMyri(world);
            Latios.Kinemation.KinemationBootstrap.InstallKinemation(world);
            Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphics(world);
            Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphicsAnimations(world);
            // Latios.Unika.UnikaBootstrap.InstallUnikaEntitySerialization(world);
            Latios.LifeFX.LifeFXBootstrap.InstallLifeFX(world);

            AnnaSuperSystem anna = Latios.Anna.AnnaBootstrap.InstallAnna(world);
            anna.SetRateManagerCreateAllocator(new SubstepRateManager(0.034f, 8));

            BootstrapTools.InjectRootSuperSystems(systems, world, world.simulationSystemGroup);

            world.initializationSystemGroup.SortSystems();
            world.simulationSystemGroup.SortSystems();
            world.presentationSystemGroup.SortSystems();

            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
            return true;
        }
    }
}
