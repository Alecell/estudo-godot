using System;
using Godot;

namespace PhysicsUtils
{
    public enum ForceComponent
    {
        INVALID = -1,
        HORIZONTAL,
        VERTICAL,
        LONGITUDINAL
    }

    public partial class RelativeDirection
    {
        public Direction Horizontal { get; set; }
        public Direction Vertical { get; set; }
        public Direction Longitudinal { get; set; }

        public RelativeDirection(
            object horizontal = null,
            object vertical = null,
            object longitudinal = null
        )
        {
            Horizontal = ConvertToDirection(horizontal, nameof(horizontal));
            Vertical = ConvertToDirection(vertical, nameof(vertical));
            Longitudinal = ConvertToDirection(longitudinal, nameof(longitudinal));
        }

        public static RelativeDirection Zero()
        {
            return new RelativeDirection(
                new Direction(0),
                new Direction(0),
                new Direction(0)
            );
        }
        public bool IsZero(ForceComponent component = ForceComponent.INVALID)
        {
            if (component == ForceComponent.INVALID)
            {
                return Math.Abs(Horizontal.Force) <= 0.01f &&
                       Math.Abs(Vertical.Force) <= 0.01f &&
                       Math.Abs(Longitudinal.Force) <= 0.01f;
            }

            return component switch
            {
                ForceComponent.HORIZONTAL => Math.Abs(Horizontal.Force) <= 0.01f,
                ForceComponent.VERTICAL => Math.Abs(Vertical.Force) <= 0.01f,
                ForceComponent.LONGITUDINAL => Math.Abs(Longitudinal.Force) <= 0.01f,
                _ => false
            };
        }

        private Direction ConvertToDirection(object value, string paramName)
        {
            if (value == null) return new Direction(0);
            if (value is Direction d) return d;
            if (value is float f) return new Direction(f);

            throw new ArgumentException($"Invalid type for parameter '{paramName}'. Must be 'float' or 'Direction'.");
        }
    }
}
