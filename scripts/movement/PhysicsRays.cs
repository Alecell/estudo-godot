using System;
using Godot;

namespace PhysicsUtils
{
    public class GroundState 
    {
        public bool Colliding { get; private set; }
        public bool WillCollide { get; private set; }

        public GroundState(bool colliding, bool willCollide)
        {
            Colliding = colliding;
            WillCollide = willCollide;
        }

    }

    /**
     *  TODO: Os colisores front e back devem ser posicionados na posiçào do path
     *  e não necessariamente na frente ou a traz dele
     *
     *  TODO: Colocar ShapeCast aqui pra fazer uma projeçào futura da posição do personagem
     *  isso é util para os casos em que o personagem entrar no chao quando a velocidade
     *  estiver muito alta, assim podendo castar o shape sem precisar de muita complexidade
     *  em termos de estrategias de raycast   
     */

    public partial class PhysicsRays
    {
        public RayCast3D BottomFrontDownRay { get; private set; }
        public RayCast3D BottomMiddleDownRay { get; private set; }
        public RayCast3D BottomBackDownRay { get; private set; }

        private Vector3 collisionShapeSizes = new Vector3(0, 0, 0);
        private Area3D entity;
        private float threshold = 0.01f;

        public PhysicsRays(Area3D entity)
        {
            this.entity = entity;
            collisionShapeSizes = GetCollisionShapeSizes();

            AttachBottomRaycast();
        }

        public GroundState EvaluateGroundCollision() {
            return new GroundState(
                IsOnGround(), 
                false
            );
        }

        private bool IsOnGround()
        {
            bool isColliding = BottomFrontDownRay.IsColliding() || 
                            BottomMiddleDownRay.IsColliding() || 
                            BottomBackDownRay.IsColliding();

            if (isColliding) {
                float frontDistance = BottomFrontDownRay.IsColliding() 
                    ? BottomFrontDownRay.GlobalTransform.Origin.DistanceTo(BottomFrontDownRay.GetCollisionPoint()) 
                    : float.MaxValue;

                float middleDistance = BottomMiddleDownRay.IsColliding() 
                    ? BottomMiddleDownRay.GlobalTransform.Origin.DistanceTo(BottomMiddleDownRay.GetCollisionPoint()) 
                    : float.MaxValue;

                float backDistance = BottomBackDownRay.IsColliding() 
                    ? BottomBackDownRay.GlobalTransform.Origin.DistanceTo(BottomBackDownRay.GetCollisionPoint()) 
                    : float.MaxValue;
                
                isColliding = collisionShapeSizes.Y >= (middleDistance - threshold) ||
                    collisionShapeSizes.Y >= (frontDistance - threshold) ||
                    collisionShapeSizes.Y >= (backDistance - threshold);
            }

            return isColliding;
        }

        private Vector3 GetCollisionShapeSizes() {
            var collisionShape = entity.GetNodeOrNull<CollisionShape3D>("CollisionShape3D");

            if (collisionShape == null || collisionShape.Shape == null)
            {
                GD.PrintErr($"No CollisionShape3D found in {entity.Name}!");
                return Vector3.Zero;
            }

            if (collisionShape.Shape is BoxShape3D boxShape)
            {
                return boxShape.Size;
            }
            else if (collisionShape.Shape is CylinderShape3D cylinderShape)
            {
                return new Vector3(
                    cylinderShape.Radius * 2,
                    cylinderShape.Height,    
                    cylinderShape.Radius * 2 
                );
            }
            else if (collisionShape.Shape is CapsuleShape3D capsuleShape)
            {
                return new Vector3(
                    capsuleShape.Radius * 2,
                    capsuleShape.Height,    
                    capsuleShape.Radius * 2 
                );
            }
            else if (collisionShape.Shape is SphereShape3D sphereShape)
            {
                return new Vector3(
                    sphereShape.Radius * 2,
                    sphereShape.Radius * 2,
                    sphereShape.Radius * 2 
                );
            }

            GD.PrintErr($"Unrecognized shape in {entity.Name}");
            return Vector3.Zero;
        }

        private void AttachBottomRaycast() {
            BottomFrontDownRay = new RayCast3D();
            BottomFrontDownRay.Name = "BottomFrontDownRay";
            BottomFrontDownRay.Position += new Vector3(0, collisionShapeSizes.Y / 2, collisionShapeSizes.X / 2);
            BottomFrontDownRay.TargetPosition = new Vector3(0, -10, 0);
            BottomFrontDownRay.DebugShapeCustomColor = new Color(1, 0, 0);
            entity.AddChild(BottomFrontDownRay);
            BottomFrontDownRay.Enabled = true;

            BottomMiddleDownRay = new RayCast3D();
            BottomMiddleDownRay.Name = "BottomMiddleDownRay";
            BottomMiddleDownRay.Position += new Vector3(0, collisionShapeSizes.Y / 2, 0);
            BottomMiddleDownRay.TargetPosition = new Vector3(0, -10, 0);
            BottomMiddleDownRay.DebugShapeCustomColor = new Color(1, 0, 0);
            entity.AddChild(BottomMiddleDownRay);
            BottomMiddleDownRay.Enabled = true;

            BottomBackDownRay = new RayCast3D();
            BottomBackDownRay.Name = "BottomBackDownRay";
            BottomBackDownRay.Position -= new Vector3(0, collisionShapeSizes.Y / -2, collisionShapeSizes.X / 2);
            BottomBackDownRay.TargetPosition = new Vector3(0, -10, 0);
            BottomBackDownRay.DebugShapeCustomColor = new Color(1, 0, 0);
            entity.AddChild(BottomBackDownRay);
            BottomBackDownRay.Enabled = true;
        }
    } 
}
