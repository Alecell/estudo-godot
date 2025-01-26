using System.Collections.Generic;
using Godot;
using PhysicsRays;

namespace PhysicsUtils
{
    public class GroundRaysState 
    {
        public bool Colliding { get; private set; }
        public bool WillCollide { get; private set; }
        public readonly Dictionary<string, Dictionary<string, RayParameters>> Rays;

        public GroundRaysState(
            bool colliding, 
            bool willCollide,
            RayParameters BottomFrontDownRay,
            RayParameters BottomMiddleDownRay,
            RayParameters BottomBackDownRay
        ) {
            Colliding = colliding;
            WillCollide = willCollide;
            Rays = new() {
                {
                    "bottom", new Dictionary<string, RayParameters>
                    {
                        { "frontDown", BottomFrontDownRay },
                        { "middleDown", BottomMiddleDownRay },
                        { "backDown", BottomBackDownRay }
                    }
                }
            };
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

    public partial class PhysicsGroundRays
    {
        public RayCast3D BottomFrontDownRay { get; private set; }
        public RayCast3D BottomMiddleDownRay { get; private set; }
        public RayCast3D BottomBackDownRay { get; private set; }

        private readonly Physics physics;
        private readonly float threshold = 0.01f;

        public PhysicsGroundRays(Physics physics)
        {
            this.physics = physics;

            AttachBottomRaycast();
        }

        public GroundRaysState EvaluateCollisions() {
            RayParameters bottomFrontDownRayParameters = new(
                BottomFrontDownRay.IsColliding(),
                BottomFrontDownRay.GlobalTransform.Origin.DistanceTo(BottomFrontDownRay.GetCollisionPoint()),
                BottomFrontDownRay.GetCollisionPoint()
            );

            RayParameters bottomMiddleDownRayParameters = new(
                BottomMiddleDownRay.IsColliding(),
                BottomMiddleDownRay.GlobalTransform.Origin.DistanceTo(BottomMiddleDownRay.GetCollisionPoint()),
                BottomMiddleDownRay.GetCollisionPoint()
            );

            RayParameters bottomBackDownRayParameters = new(
                BottomBackDownRay.IsColliding(),
                BottomBackDownRay.GlobalTransform.Origin.DistanceTo(BottomBackDownRay.GetCollisionPoint()),
                BottomBackDownRay.GetCollisionPoint()
            );

            return new GroundRaysState(
                IsColliding(bottomFrontDownRayParameters, bottomMiddleDownRayParameters, bottomBackDownRayParameters), 
                WillCollide(bottomFrontDownRayParameters, bottomMiddleDownRayParameters, bottomBackDownRayParameters),
                bottomFrontDownRayParameters,
                bottomMiddleDownRayParameters,
                bottomBackDownRayParameters
            );
        }

    private bool IsColliding(
        RayParameters bottomFrontDownRayParameters, 
        RayParameters bottomMiddleDownRayParameters, 
        RayParameters bottomBackDownRayParameters
    ) {
            bool isColliding = bottomFrontDownRayParameters.Colliding ||
                            bottomMiddleDownRayParameters.Colliding ||
                            bottomBackDownRayParameters.Colliding;

            if (isColliding) {
                float frontDistance = bottomFrontDownRayParameters.Colliding
                    ? bottomFrontDownRayParameters.Distance
                    : float.MaxValue;

                float middleDistance = bottomMiddleDownRayParameters.Colliding
                    ? bottomMiddleDownRayParameters.Distance
                    : float.MaxValue;

                float backDistance = bottomBackDownRayParameters.Colliding 
                    ? bottomBackDownRayParameters.Distance
                    : float.MaxValue;

                isColliding = physics.entitySize.Y >= (middleDistance - threshold) ||
                    physics.entitySize.Y >= (frontDistance - threshold) ||
                    physics.entitySize.Y >= (backDistance - threshold);
            }

            return isColliding;
        }

        private bool WillCollide(
            RayParameters bottomFrontDownRayParameters, 
            RayParameters bottomMiddleDownRayParameters, 
            RayParameters bottomBackDownRayParameters
        ) {
            bool willCollide = false;

            if (physics.Velocities.Vertical != 0)
            {
                float positionDisplacement = physics.Velocities.Vertical * physics.Delta;
                float lowestColliderPointGlobal = physics.entity.GlobalTransform.Origin.Y - physics.entitySize.Y / 2;
                float nextLowestColliderPointGlobal = lowestColliderPointGlobal - Mathf.Abs(positionDisplacement);
                
                if (bottomMiddleDownRayParameters.Colliding) {
                    willCollide = nextLowestColliderPointGlobal <= bottomMiddleDownRayParameters.CollisionPoint.Y;
                } else {
                    if (bottomFrontDownRayParameters.Colliding) {
                        willCollide = nextLowestColliderPointGlobal <= bottomFrontDownRayParameters.CollisionPoint.Y;
                    } 
                    
                    if (bottomBackDownRayParameters.Colliding) {
                        willCollide = nextLowestColliderPointGlobal <= bottomBackDownRayParameters.CollisionPoint.Y;
                    }
                }
            }

            return willCollide;
        }

        private void AttachBottomRaycast() {
            var yPosition = physics.entity.GlobalTransform.Origin.Y + physics.entitySize.Y / 2;
            var xPosition = physics.entity.GlobalTransform.Origin.X;
            var zPosition = physics.entity.GlobalTransform.Origin.Z;

            BottomFrontDownRay = new RayCast3D();
            BottomFrontDownRay.Name = "BottomFrontDownRay";
            BottomFrontDownRay.TargetPosition = new Vector3(0, -10, 0);
            BottomFrontDownRay.DebugShapeCustomColor = new Color(1, 0, 0);
            physics.entity.AddChild(BottomFrontDownRay);
            BottomFrontDownRay.Enabled = true;
            BottomFrontDownRay.GlobalTransform = new Transform3D(
                physics.entity.GlobalTransform.Basis,
                new Vector3(xPosition, yPosition, zPosition + physics.entitySize.Z / -2)
            );

            BottomMiddleDownRay = new RayCast3D();
            BottomMiddleDownRay.Name = "BottomMiddleDownRay";
            BottomMiddleDownRay.TargetPosition = new Vector3(0, -10, 0);
            BottomMiddleDownRay.DebugShapeCustomColor = new Color(1, 0, 0);
            physics.entity.AddChild(BottomMiddleDownRay);
            BottomMiddleDownRay.Enabled = true;
            BottomMiddleDownRay.GlobalTransform = new Transform3D(
                physics.entity.GlobalTransform.Basis,
                new Vector3(xPosition, yPosition, zPosition)
            );

            BottomBackDownRay = new RayCast3D();
            BottomBackDownRay.Name = "BottomBackDownRay";
            BottomBackDownRay.TargetPosition = new Vector3(0, -10, 0);
            BottomBackDownRay.DebugShapeCustomColor = new Color(1, 0, 0);
            physics.entity.AddChild(BottomBackDownRay);
            BottomBackDownRay.Enabled = true;
            BottomBackDownRay.GlobalTransform = new Transform3D(
                physics.entity.GlobalTransform.Basis,
                new Vector3(xPosition, yPosition, zPosition + physics.entitySize.Z / 2)
            );
        }
    } 
}
