namespace PhysicsUtils
{
    public partial class Direction
    {
        public override string ToString()
        {
            return $"{{Force: {Force}, Limit: {(Limit.HasValue ? Limit.Value.ToString() : "null")}}}";
        }
    }
}