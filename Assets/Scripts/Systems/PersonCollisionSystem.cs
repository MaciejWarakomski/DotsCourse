using Unity.Physics.Systems;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Rendering;
using Unity.Entities;
using Unity.Physics;
using Components;

namespace Systems
{
    public partial class PersonCollisionSystem : SystemBase
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;
        
        protected override void OnCreate()
        {
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        }

        private struct PersonCollisionJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<PersonTag> PersonGroup;
            public ComponentDataFromEntity<URPMaterialPropertyBaseColor> ColorGroup;
            public float Seed;
            
            public void Execute(TriggerEvent triggerEvent)
            {
                var isEntityAPerson = PersonGroup.HasComponent(triggerEvent.EntityA);
                var isEntityBPerson = PersonGroup.HasComponent(triggerEvent.EntityB);
                if (!isEntityAPerson || !isEntityBPerson) return;

                var random = new Random((uint)(1 + Seed + triggerEvent.BodyIndexA * triggerEvent.BodyIndexB));
                ChangeMaterialColor(random, triggerEvent.EntityA);
            }

            private void ChangeMaterialColor(Random random, Entity entity)
            {
                if (!ColorGroup.HasComponent(entity)) return;
                
                var colorComponent = ColorGroup[entity];
                colorComponent.Value.x = random.NextFloat(0, 1);
                colorComponent.Value.y = random.NextFloat(0, 1);
                colorComponent.Value.z = random.NextFloat(0, 1);
                ColorGroup[entity] = colorComponent;
            }
        }

        protected override void OnUpdate()
        {
            Dependency = new PersonCollisionJob
            {
                PersonGroup = GetComponentDataFromEntity<PersonTag>(true),
                ColorGroup = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>(),
                Seed = System.DateTimeOffset.Now.Millisecond
            }.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, Dependency);
        }
    }
}