using System;
using Unity.Entities;
using UnityEngine;

namespace ECS.Data
{
    [Serializable]
    public struct ECS_WingData : IComponentData
    {
        // Helper opbjects to calculate area of lift/drag
        public GameObject edge0;
        public GameObject edge1;
        public GameObject edge3;


        public AnimationCurve liftCurve;
        public AnimationCurve dragCurve;

        public Rigidbody Rigidbody;

        // temporary solution for lift/drag coefficients
        public float lift; // liftmultiplier * density/2

        public float drag; // dragmultiplier * density/2

        // area of lift/drag
        public float wingArea;
    }

    public class ECS_WingDataComponent : ComponentDataWrapper<ECS_WingData>
    {
    }
}