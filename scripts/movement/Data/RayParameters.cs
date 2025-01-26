using Godot;

namespace PhysicsRays {
    public class RayParameters
    {
        public bool Colliding { get; private set; }
        public float Distance { get; private set; }
        public Vector3 CollisionPoint { get; private set; }

        public RayParameters(bool colliding, float distance, Vector3 collisionPoint)
        {
            Colliding = colliding;
            Distance = distance;
            CollisionPoint = collisionPoint;
        }
    }
}