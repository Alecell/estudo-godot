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
        )
        {
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
        public RayCast3D SlopeDownRay { get; private set; }

        private readonly Physics physics;
        private readonly float threshold = 0.05f;

        public PhysicsGroundRays(Physics physics)
        {
            this.physics = physics;

            AttachTopToBottomRaycast();
        }

        public RayCast3D CreateRayCast(string name, Vector3 position)
        {
            var rayCast = new RayCast3D();
            rayCast.Name = name;
            rayCast.TargetPosition = new Vector3(0, -10, 0);
            rayCast.DebugShapeCustomColor = new Color(1, 0, 0);
            rayCast.Enabled = true;
            physics.entity.AddChild(rayCast);
            rayCast.GlobalTransform = new Transform3D(
                physics.entity.GlobalTransform.Basis,
                position
            );

            return rayCast;
        }

        public GroundRaysState EvaluateCollisions()
        {
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
        )
        {
            bool isColliding = bottomFrontDownRayParameters.Colliding ||
                            bottomMiddleDownRayParameters.Colliding ||
                            bottomBackDownRayParameters.Colliding;

            if (isColliding)
            {
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
        )
        {
            bool willCollide = false;

            if (physics.Velocities.Vertical.Force < 0)
            {
                float positionDisplacement = physics.Velocities.Vertical.Force * physics.Delta;
                float lowestColliderPointGlobal = physics.entity.GlobalTransform.Origin.Y - physics.entitySize.Y / 2;
                float nextLowestColliderPointGlobal = lowestColliderPointGlobal - positionDisplacement * -1;

                if (bottomMiddleDownRayParameters.Colliding)
                {
                    willCollide = nextLowestColliderPointGlobal <= bottomMiddleDownRayParameters.CollisionPoint.Y;
                }
                else
                {
                    if (bottomFrontDownRayParameters.Distance > bottomBackDownRayParameters.Distance)
                    {
                        willCollide = nextLowestColliderPointGlobal <= bottomBackDownRayParameters.CollisionPoint.Y;
                    }
                    else
                    {
                        willCollide = nextLowestColliderPointGlobal <= bottomFrontDownRayParameters.CollisionPoint.Y;
                    }
                }
            }

            return willCollide;
        }

        private void AttachTopToBottomRaycast()
        {
            var yPosition = physics.entity.GlobalTransform.Origin.Y + physics.entitySize.Y / 2;
            var xPosition = physics.entity.GlobalTransform.Origin.X;
            var zPosition = physics.entity.GlobalTransform.Origin.Z;

            BottomFrontDownRay = CreateRayCast("BottomFrontDownRay", new Vector3(xPosition, yPosition, zPosition + physics.entitySize.Z / -2));
            BottomMiddleDownRay = CreateRayCast("BottomMiddleDownRay", new Vector3(xPosition, yPosition, zPosition));
            BottomBackDownRay = CreateRayCast("BottomBackDownRay", new Vector3(xPosition, yPosition, zPosition + physics.entitySize.Z / 2));
            SlopeDownRay = CreateRayCast("SlopeDownRay", new Vector3(xPosition, yPosition, zPosition));
        }
    }
}
