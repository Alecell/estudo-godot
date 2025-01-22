using System;
using Godot;

namespace PhysicsUtils
{
    public partial class Force
    {
        public RelativeDirection force { get; set; }
        public string key { get; private set; }

        public Force(RelativeDirection force, string key = null)
        {
            this.force = force;
            this.key = key ?? GenerateKey();
        }

        private string GenerateKey()
        {
            return Guid.NewGuid().ToString();
        }
    } 
}
