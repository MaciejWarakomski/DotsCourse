using Components;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class SetupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject personPrefab;
        [SerializeField] private int gridSize;
        [SerializeField] private int spread;
        [SerializeField] private Vector2 speedRange = new Vector2(4, 7);

        private BlobAssetStore _blob;

        private void Start()
        {
            _blob = new BlobAssetStore();
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blob);
            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(personPrefab, settings);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            for (var x = 0; x < gridSize; x++)
            {
                for (var z = 0; z < gridSize; z++)
                {
                    var instance = entityManager.Instantiate(entity);
                    var position = new float3(x * spread, 0, z * spread);
                    entityManager.SetComponentData(instance, new Translation { Value = position });
                    entityManager.SetComponentData(instance, new Destination { Value = position });
                    var speed = Random.Range(speedRange.x, speedRange.y);
                    entityManager.SetComponentData(instance, new MovementSpeed { Value = speed });
                }
            }
        }

        private void OnDestroy()
        {
            _blob.Dispose();
        }
    }
}