using System;
using Godot;

namespace PhysicsUtils
{
    public partial class RelativeDirection
    {
        public float Horizontal { get; set; }
        public float Vertical { get; set; }
        public float Longitudinal { get; set; }

        public RelativeDirection(float horizontal, float vertical, float longitudinal)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            Longitudinal = longitudinal;
        }

        // Soma de dois RelativeDirection
        public static RelativeDirection operator +(RelativeDirection a, RelativeDirection b) =>
            new RelativeDirection(a.Horizontal + b.Horizontal, a.Vertical + b.Vertical, a.Longitudinal + b.Longitudinal);

        // Subtração de dois RelativeDirection
        public static RelativeDirection operator -(RelativeDirection a, RelativeDirection b) =>
            new RelativeDirection(a.Horizontal - b.Horizontal, a.Vertical - b.Vertical, a.Longitudinal - b.Longitudinal);

        // Multiplicação por escalar
        public static RelativeDirection operator *(RelativeDirection a, float scalar) =>
            new RelativeDirection(a.Horizontal * scalar, a.Vertical * scalar, a.Longitudinal * scalar);

        public static RelativeDirection operator *(float scalar, RelativeDirection a) => a * scalar;

        // Divisão por escalar
        public static RelativeDirection operator /(RelativeDirection a, float scalar) =>
            scalar == 0 ? throw new DivideByZeroException() : new RelativeDirection(a.Horizontal / scalar, a.Vertical / scalar, a.Longitudinal / scalar);

        // Módulo (resto da divisão)
        public static RelativeDirection operator %(RelativeDirection a, float scalar) =>
            new RelativeDirection(a.Horizontal % scalar, a.Vertical % scalar, a.Longitudinal % scalar);

        // Operador de igualdade ==
        public static bool operator ==(RelativeDirection a, RelativeDirection b) =>
            a.Horizontal == b.Horizontal && a.Vertical == b.Vertical && a.Longitudinal == b.Longitudinal;

        // Operador de diferença !=
        public static bool operator !=(RelativeDirection a, RelativeDirection b) => !(a == b);

        // Operador unário negativo
        public static RelativeDirection operator -(RelativeDirection a) =>
            new RelativeDirection(-a.Horizontal, -a.Vertical, -a.Longitudinal);

        // Operador unário positivo (redundante, mas útil)
        public static RelativeDirection operator +(RelativeDirection a) => a;

        // Incremento (++)
        public static RelativeDirection operator ++(RelativeDirection a)
        {
            a.Horizontal++;
            a.Vertical++;
            a.Longitudinal++;
            return a;
        }

        // Decremento (--)
        public static RelativeDirection operator --(RelativeDirection a)
        {
            a.Horizontal--;
            a.Vertical--;
            a.Longitudinal--;
            return a;
        }

        // Conversão implícita para Vector3 (Godot)
        public static implicit operator Vector3(RelativeDirection a) =>
            new Vector3(a.Horizontal, a.Vertical, a.Longitudinal);

        // Conversão explícita de Vector3 para RelativeDirection
        public static explicit operator RelativeDirection(Vector3 v) =>
            new RelativeDirection((float)v.X, (float)v.Y, (float)v.Z);

        // Override de Equals e GetHashCode (boa prática para classes com sobrecarga de == e !=)
        public override bool Equals(object obj) =>
            obj is RelativeDirection other && this == other;

        // Override GetHashCode method
        public override int GetHashCode()
        {
            return HashCode.Combine(Horizontal.GetHashCode(), Vertical.GetHashCode(), Longitudinal.GetHashCode());
        }

        // Representação em string
        public override string ToString() =>
            $"RelativeDirection(Horizontal: {Horizontal}, Vertical: {Vertical}, Longitudinal: {Longitudinal})";
    }
}
