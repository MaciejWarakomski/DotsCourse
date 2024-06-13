using Components;
using Unity.Entities;

namespace Systems
{
    public partial class LifetimeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;

        protected override void OnCreate()
        {
            _endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = _endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifetime) =>
            {
                lifetime.Value -= deltaTime;
                
                if (lifetime.Value <= 0f)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
            
            _endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}