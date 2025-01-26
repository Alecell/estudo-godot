using System;
using System.Collections.Generic;
using Godot;

namespace PhysicsUtils
    {
    public partial class ForceManager
    {
        private readonly Dictionary<string, Force> forces = new();

        public string AddForce(RelativeDirection forces, string key = null)
        {
            Force force = new(forces, key);
            this.forces.Add(force.key, force);
            return force.key;
        }

        public void AddOnForce(RelativeDirection forces, string key)
        {
            if (this.forces.ContainsKey(key))
            {
                this.forces[key].force += forces;
            } else {
                AddForce(forces, key);
            }
        }

        public RelativeDirection ComputeForces()
        {
            RelativeDirection result = new(0, 0, 0);
            foreach (var force in forces.Values)
            {
                result += force.force;
            }
            return result;
        }

        public void RemoveForce(string key)
        {
            forces.Remove(key);
        }
    }
}