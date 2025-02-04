using System;
using Godot;

namespace PhysicsUtils
{
    public partial class ForceManager
    {
        public readonly Force internalForce = new();
        public readonly Force externalForce = new();

        public RelativeDirection ComputeForces(RelativeDirection frictionCoefficient)
        {
            var finalResultant = internalForce + externalForce;

            if (finalResultant.IsZero()) return RelativeDirection.Zero();

            var proportionInternal = GetProportion(internalForce, finalResultant);
            var proportionExternal = GetProportion(externalForce, finalResultant);

            internalForce.ApplyFriction(frictionCoefficient * proportionInternal);
            externalForce.ApplyFriction(frictionCoefficient * proportionExternal);

            return internalForce + externalForce;
        }

        private static RelativeDirection GetProportion(RelativeDirection individualForce, RelativeDirection totalForce)
        {
            return new RelativeDirection(
                GetProportionAxis(individualForce.Horizontal.Force, totalForce.Horizontal.Force),
                GetProportionAxis(individualForce.Vertical.Force, totalForce.Vertical.Force),
                GetProportionAxis(individualForce.Longitudinal.Force, totalForce.Longitudinal.Force)
            );
        }

        private static float GetProportionAxis(float individualForce, float totalForce)
        {
            return (Math.Abs(totalForce) < 0.001f) ? 0 : individualForce / totalForce;
        }
    }
}
