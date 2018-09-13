using Unity.Entities;
using UnityEngine;

public class WingSystem : ComponentSystem {

    struct EntityFilter
    {
        public Transform Transform;
        public WingData WingData;
    }
	
    protected override void OnUpdate()
    {
        ComponentGroupArray<EntityFilter> filtered = GetEntities<EntityFilter>();
        foreach (var entry in filtered)
        {
            
            Vector3 currVel = entry.WingData.Rigidbody.GetPointVelocity(entry.Transform.position);    // current Velocity in Worldspace

            Vector3 movForwardDir = currVel.normalized;                             // current Movement Direction in Worldspace
            Vector3 movRightDir = Vector3.Cross(entry.Transform.up, movForwardDir);       // Movement Right-Direction Vector (from Movement Direction and Aircraft Up-Direction)
            Vector3 movUpDir = Vector3.Cross(movForwardDir, movRightDir);           // Movement Up-Direction Vector (from Movement Direction and Movement Right-Direction)

            // Wing Lift
            float liftCoefficient = Vector3.Dot(entry.Transform.up, movForwardDir);                                               // cl: temporary lift Coefficient from angle of attack
            float liftForce = -liftCoefficient * entry.WingData.lift * 1.2041f * 0.5f * currVel.sqrMagnitude * entry.WingData.wingArea;                   // cl * density / 2 * v² * A (lift = multiplier * density/2)


            float wingAreaSqrRoot = Mathf.Sqrt(entry.WingData.wingArea);
            float liftCenterOffset = (0.5f * wingAreaSqrRoot) * 0.33f;

            Vector3 moveDirToWing = Vector3.ProjectOnPlane(movForwardDir, entry.Transform.up).normalized;
            
            Vector3 forcePosition =
                Vector3.Lerp(entry.Transform.position, moveDirToWing + entry.Transform.position, liftCenterOffset);
            
            //Debug.DrawRay(entry.Transform.position, moveDirToWing*10f,Color.red);
            
            entry.WingData.Rigidbody.AddForceAtPosition(movUpDir * liftForce, forcePosition, ForceMode.Force);

            // direction of airresistance
            Vector3 dragDir = -movForwardDir;

            // Wing airresistance
            float resistanceCoefficient = Mathf.Abs(Vector3.Dot(entry.Transform.up, movForwardDir));                              // cw: temporary resistance Coefficient from angle of attack
            float resistanceForceMagnitude = resistanceCoefficient * entry.WingData.drag * currVel.sqrMagnitude * entry.WingData.wingArea;                // cw * density / 2 * v² * A (drag = multiplier * density/2)
            entry.WingData.Rigidbody.AddForceAtPosition(dragDir * resistanceForceMagnitude, entry.Transform.position, ForceMode.Force);
        }
    }
}