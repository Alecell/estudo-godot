using System;
using Godot;

namespace PhysicsUtils
{
    public partial class Force : RelativeDirection
    {
        public Force(
            object horizontal = null,
            object vertical = null,
            object longitudinal = null
        ) : base(horizontal, vertical, longitudinal) { }

        public RelativeDirection Add(RelativeDirection variant)
        {
            Horizontal.Force += variant.Horizontal.Force;
            Vertical.Force += variant.Vertical.Force;
            Longitudinal.Force += variant.Longitudinal.Force;

            AdjustByLimit();

            return new RelativeDirection(Horizontal, Vertical, Longitudinal);
        }

        public RelativeDirection Set(RelativeDirection variant)
        {
            Horizontal = variant.Horizontal;
            Vertical = variant.Vertical;
            Longitudinal = variant.Longitudinal;

            return new RelativeDirection(Horizontal, Vertical, Longitudinal);
        }

        public RelativeDirection Set(ForceComponent component, float value)
        {
            switch (component)
            {
                case ForceComponent.HORIZONTAL:
                    Horizontal.Force = value;
                    break;
                case ForceComponent.VERTICAL:
                    Vertical.Force = value;
                    break;
                case ForceComponent.LONGITUDINAL:
                    Longitudinal.Force = value;
                    break;
            }

            return new RelativeDirection(Horizontal, Vertical, Longitudinal);
        }

        public void ApplyFriction(RelativeDirection frictionCoefficient)
        {
            Horizontal.Force = ApplyFrictionToAxis(Horizontal.Force, frictionCoefficient.Horizontal.Force);
            Vertical.Force = ApplyFrictionToAxis(Vertical.Force, frictionCoefficient.Vertical.Force);
            Longitudinal.Force = ApplyFrictionToAxis(Longitudinal.Force, frictionCoefficient.Longitudinal.Force);
        }

        private static float ApplyFrictionToAxis(float force, float frictionCoefficient)
        {
            float frictionForce = Math.Abs(force) * frictionCoefficient;
            if (Math.Abs(force) < frictionForce) return 0;
            return force - Math.Sign(force) * frictionForce;
        }

        private void AdjustByLimit()
        {
            if (Horizontal.Limit.HasValue && Math.Abs(Horizontal.Force) > Horizontal.Limit.Value)
                Horizontal.Force = Math.Sign(Horizontal.Force) * Horizontal.Limit.Value;

            if (Vertical.Limit.HasValue && Math.Abs(Vertical.Force) > Vertical.Limit.Value)
                Vertical.Force = Math.Sign(Vertical.Force) * Vertical.Limit.Value;

            if (Longitudinal.Limit.HasValue && Math.Abs(Longitudinal.Force) > Longitudinal.Limit.Value)
                Longitudinal.Force = Math.Sign(Longitudinal.Force) * Longitudinal.Limit.Value;
        }
    }
}
