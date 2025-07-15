using Unity.Entities;
using UnityEngine;

using Latios;
using Latios.Psyshock;
using Latios.Transforms;

public partial class DrawCollidersSystem : SubSystem
{
    protected override void OnUpdate()
    {
        this.CompleteDependency();
        foreach ((
                RefRO<Latios.Psyshock.Collider> collider,
                RefRO<WorldTransform> transform)
            in SystemAPI.Query<
                RefRO<Latios.Psyshock.Collider>,
                RefRO<WorldTransform>>())
        {
            PhysicsDebug.DrawCollider(collider.ValueRO, transform.ValueRO.worldTransform, Color.green);
        }
    }
}
