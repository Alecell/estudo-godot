namespace PhysicsUtils
{
    public partial class Direction
    {
        public float Force { get; set; }
        public float? Limit { get; set; }
        public Direction(float force, float? limit = null)
        {
            Force = force;
            Limit = limit;
        }
    }
}