using System;
using Godot;

namespace PhysicsUtils
{
    public partial class RelativeDirection
    {
        public static RelativeDirection operator +(RelativeDirection a, RelativeDirection b) =>
            new RelativeDirection(
                new Direction(a.Horizontal.Force + b.Horizontal.Force),
                new Direction(a.Vertical.Force + b.Vertical.Force),
                new Direction(a.Longitudinal.Force + b.Longitudinal.Force)
            );

        public static RelativeDirection operator -(RelativeDirection a, RelativeDirection b) =>
            new RelativeDirection(
                new Direction(a.Horizontal.Force - b.Horizontal.Force),
                new Direction(a.Vertical.Force - b.Vertical.Force),
                new Direction(a.Longitudinal.Force - b.Longitudinal.Force)
            );

        public static RelativeDirection operator *(RelativeDirection a, float scalar) =>
            new RelativeDirection(
                new Direction(a.Horizontal.Force * scalar),
                new Direction(a.Vertical.Force * scalar),
                new Direction(a.Longitudinal.Force * scalar)
            );

        public static RelativeDirection operator *(float scalar, RelativeDirection a) => a * scalar;

        public static RelativeDirection operator *(RelativeDirection a, RelativeDirection b) =>
            new RelativeDirection(
                new Direction(a.Horizontal.Force * b.Horizontal.Force),
                new Direction(a.Vertical.Force * b.Vertical.Force),
                new Direction(a.Longitudinal.Force * b.Longitudinal.Force)
            );

        public static RelativeDirection operator /(RelativeDirection a, float scalar) =>
            scalar == 0 ? throw new DivideByZeroException() : new RelativeDirection(new Direction(a.Horizontal.Force / scalar), new Direction(a.Vertical.Force / scalar), new Direction(a.Longitudinal.Force / scalar));

        public static RelativeDirection operator %(RelativeDirection a, float scalar) =>
            new RelativeDirection(
                new Direction(a.Horizontal.Force % scalar),
                new Direction(a.Vertical.Force % scalar),
                new Direction(a.Longitudinal.Force % scalar)
            );

        public static bool operator ==(RelativeDirection a, RelativeDirection b) =>
            a.Horizontal == b.Horizontal && a.Vertical == b.Vertical && a.Longitudinal == b.Longitudinal;

        public static bool operator !=(RelativeDirection a, RelativeDirection b) => !(a == b);

        public static RelativeDirection operator -(RelativeDirection a) =>
            new RelativeDirection(
                new Direction(-a.Horizontal.Force),
                new Direction(-a.Vertical.Force),
                new Direction(-a.Longitudinal.Force)
            );

        public static RelativeDirection operator +(RelativeDirection a) => a;

        public static RelativeDirection operator ++(RelativeDirection a)
        {
            a.Horizontal.Force++;
            a.Vertical.Force++;
            a.Longitudinal.Force++;
            return a;
        }

        public static RelativeDirection operator --(RelativeDirection a)
        {
            a.Horizontal.Force--;
            a.Vertical.Force--;
            a.Longitudinal.Force--;
            return a;
        }

        public static implicit operator Vector3(RelativeDirection a) =>
            new Vector3(
                a.Horizontal.Force,
                a.Vertical.Force,
                a.Longitudinal.Force
            );

        public static explicit operator RelativeDirection(Vector3 v) =>
            new RelativeDirection(
                new Direction((float)v.X),
                new Direction((float)v.Y),
                new Direction((float)v.Z)
            );
        public override bool Equals(object obj) =>
            obj is RelativeDirection other && this == other;
        public override int GetHashCode()
        {
            return HashCode.Combine(Horizontal.GetHashCode(), Vertical.GetHashCode(), Longitudinal.GetHashCode());
        }
        public override string ToString() =>
            $"RelativeDirection(Horizontal: {Horizontal}, Vertical: {Vertical}, Longitudinal: {Longitudinal})";
    }
}
