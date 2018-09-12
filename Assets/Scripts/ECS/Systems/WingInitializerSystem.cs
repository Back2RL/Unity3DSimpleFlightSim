using System;
using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;


public class WingInitializerSystem : ComponentSystem
{
    private int runs = 0;

    struct EntityFilter
    {
        public WingInitializedData WingInitializedData;
        public WingData WingData;
    }

    protected override void OnUpdate()
    {
        foreach (var entry in GetEntities<EntityFilter>())
        {
            // temporary wing area calculation (A = a * b)

            float a = (entry.WingData.edge0.transform.position - entry.WingData.edge1.transform.position).magnitude;
            float b = (entry.WingData.edge1.transform.position - entry.WingData.edge3.transform.position).magnitude;
            entry.WingData.wingArea = a * b;
            Debug.Log("Wingarea calculated by System = " + entry.WingData.wingArea.ToString() + " m²");

            Object.Destroy(entry.WingData.edge0);
            Object.Destroy(entry.WingData.edge1);
            Object.Destroy(entry.WingData.edge3);

            entry.WingData.edge0 = null;
            entry.WingData.edge1 = null;
            entry.WingData.edge3 = null;

            // remove the WingInitializedData dummy from the entity as well as from the gameobject
            GameObjectEntity gameObjectEntity = entry.WingInitializedData.gameObject.GetComponent<GameObjectEntity>();
            // this does not work, it modifies the Entities list!
            // gameObjectEntity.EntityManager.RemoveComponent(gameObjectEntity.Entity,
            //     entry.WingInitializedData.GetType());
            // instead do this:
            PostUpdateCommands.RemoveComponent(gameObjectEntity.Entity, entry.WingInitializedData.GetType());
            Object.Destroy(entry.WingInitializedData);
            
            ++runs;
            Debug.Log("WingInitializerSystem initialized " + runs + " entities.");
        }
    }
}