using ECS.Data;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

namespace ECS.Systems
{
//    public class ECS_WingSystem : JobComponentSystem
//    {
//        struct Wings : IJobProcessComponentData<Rotation, Position, ECS_WingData>
//        {
//            public float dt;
//
//
//            public void Execute(ref Rotation data0, ref Position data1, ref ECS_WingData data2)
//            {
//                
//            }
//        }
//
//
//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            Wings wings = new Wings()
//            {
//                dt = Time.deltaTime
//            };
//
//            JobHandle wingJobHandle = wings.Schedule(;
//            return wingJobHandle;
//        }
//    }
}