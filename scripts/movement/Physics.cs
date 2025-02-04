using Godot;
using PhysicsState;
using PhysicsUtils;

/**
 * TODO: Precisava entender porque quando deu o pau que o player se movia super rapido
 quando pulava ele também entrava no chão. Em tese isso não deveria acontecer, mas
 porque estav acontecendo? preciso entender.
 */

public partial class Physics
{
    enum OffsetTypes
    {
        INVALID = -1,
    }

    public bool ShouldFaceOrientation { get; set; } = true;
    public float Delta { get; set; }
    public State State { get; private set; }
    public RelativeDirection Velocities { get; private set; }
    public Area3D entity { get; private set; }
    public Vector3 entitySize { get; private set; }

    private float offset { get; set; } = (float)OffsetTypes.INVALID;
    private ForceManager forceManager { get; set; }
    private PhysicsGroundRays physicsGroundRays { get; set; }
    /**
     * TODO: Adicionar variante de caminho circular

     * TODO: Adicionar variante de caminho com pontos de controle para alteração de path.
     Fazer curvas entre paths

     * TODO: Fazer os dados da store serem acessados de forma dinamica, ou seja carrego uma cena,
     essa cena define um path, um position no path e qual path ela vai, assim posicionar o player
     baseado nisso aqui na linha Store.Instance.PathManager.FindPath("start");, ja que no momento
     o path é mockado
     */


    public Physics(Area3D entity, ForceManager forceManager)
    {
        this.entity = entity;
        this.forceManager = forceManager;
        entitySize = GetCollisionShapeSizes();
        physicsGroundRays = new PhysicsGroundRays(this);

        State = new State();
    }

    /**
     * TODO: Fazer a parte de lidar com rampas, no caso seria algo de projetar a posição do player
     no proximo frame, ver se ele ta com o ray menor do que deveria e, se tiver, preciso projetar
     ele um pouco mais pra cima pra que ele acompanhe a rampa. Um ponto importante é que preciso
     ter um limite de inclinação, ou seja, se a rampa for muito inclinada, o player não deve subir.
     Mais interessante ainda seria nunca permitir que um angulo de inclinaçào atual e relação ao proximo
     seja discrepante, assim eu consigo subir certas coisas e outras não.

     * TODO: Fazer a emissão de eventos
     */
    public void Apply()
    {
        Velocities = forceManager.ComputeForces(new RelativeDirection(0.1f, 0.1f, 0.1f));
        GD.Print(Velocities.Horizontal.Force, " ", Velocities.Vertical.Force);
        entity.GlobalTransform = ApplyVelocityBasedPositioning();

        var groundState = physicsGroundRays.EvaluateCollisions();

        State.Ground.Colliding = groundState.Colliding || groundState.WillCollide;
        State.Ground.WillCollide = groundState.WillCollide;

        if (groundState.WillCollide)
        {
            var nextEntityPositionY = GetGroundYPosition(groundState);
            entity.GlobalTransform = new Transform3D(
                entity.GlobalTransform.Basis,
                new Vector3(
                    entity.GlobalTransform.Origin.X,
                    nextEntityPositionY,
                    entity.GlobalTransform.Origin.Z
                )
            );
        }
    }

    private Transform3D ApplyVelocityBasedPositioning()
    {
        var globalNextPathPoint = HorizontalPathMovement();
        var nextEntityPositionY = entity.GlobalTransform.Origin.Y + Velocities.Vertical.Force * Delta;

        return new Transform3D(
            entity.GlobalTransform.Basis,
            new Vector3(
                globalNextPathPoint.X,
                nextEntityPositionY,
                globalNextPathPoint.Z
            )
        );
    }

    private Vector3 HorizontalPathMovement()
    {
        Vector3 globalNextPointOnCurve = entity.GlobalTransform.Origin;

        if (!Velocities.IsZero(ForceComponent.HORIZONTAL))
        {
            Path3D path = Store.Instance.PathManager.FindPath("start");

            if (!IsValidOffset()) SetOffset(path);

            offset += Velocities.Horizontal.Force * Delta;
            Vector3 nextPointOnCurve = path.Curve.SampleBaked(offset, true);
            globalNextPointOnCurve = path.ToGlobal(nextPointOnCurve);

            if (ShouldFaceOrientation) FaceOrientation(entity, globalNextPointOnCurve);
        }

        return globalNextPointOnCurve;
    }

    private float GetGroundYPosition(GroundRaysState groundState)
    {
        var entityPosition = entity.GlobalTransform.Origin;
        float distance;

        if (groundState.Rays["bottom"]["middleDown"].Colliding)
        {
            distance = groundState.Rays["bottom"]["middleDown"].Distance;
        }
        else
        {
            distance = Mathf.Min(
                groundState.Rays["bottom"]["frontDown"].Distance,
                groundState.Rays["bottom"]["backDown"].Distance
            );
        }

        if (!float.IsNaN(distance))
        {
            float newEntityPositionY = entityPosition.Y - (distance - entitySize.Y);

            return newEntityPositionY;
        }

        return entityPosition.Y;
    }

    private bool IsValidOffset()
    {
        return offset >= 0;
    }

    private void SetOffset(Path3D path)
    {
        Vector3 playerPositionRelativeToPath = path.ToLocal(entity.GlobalTransform.Origin);
        offset = path.Curve.GetClosestOffset(playerPositionRelativeToPath);
    }

    /**
     * TODO: Preciso de uma verificação de se eu to tentando virar o player pro mesmo lugar onde eu ja
     tava, isso ta dando erro. Basta fazer uma verificação
     */
    private void FaceOrientation(Area3D player, Vector3 globalPointInPath)
    {
        Vector3 movementDirection = (player.GlobalTransform.Origin - globalPointInPath).Normalized();
        movementDirection.Y = 0;
        player.LookAt(player.GlobalTransform.Origin + movementDirection, Vector3.Up);
    }

    private Vector3 GetCollisionShapeSizes()
    {
        var collisionShape = entity.GetNodeOrNull<CollisionShape3D>("CollisionShape3D");

        if (collisionShape == null || collisionShape.Shape == null)
        {
            GD.PrintErr($"No CollisionShape3D found in {entity.Name}!");
            return Vector3.Zero;
        }

        Vector3 globalScale = new Vector3(
            collisionShape.GlobalTransform.Basis.X.Length(),
            collisionShape.GlobalTransform.Basis.Y.Length(),
            collisionShape.GlobalTransform.Basis.Z.Length()
        );

        if (collisionShape.Shape is BoxShape3D boxShape)
        {
            return boxShape.Size * globalScale;
        }
        else if (collisionShape.Shape is CylinderShape3D cylinderShape)
        {
            float radiusScale = (globalScale.X + globalScale.Z) / 2;
            return new Vector3(
                cylinderShape.Radius * 2 * radiusScale,
                cylinderShape.Height * globalScale.Y,
                cylinderShape.Radius * 2 * radiusScale
            );
        }
        else if (collisionShape.Shape is CapsuleShape3D capsuleShape)
        {
            float radiusScale = (globalScale.X + globalScale.Z) / 2;
            return new Vector3(
                capsuleShape.Radius * 2 * radiusScale,
                capsuleShape.Height * globalScale.Y,
                capsuleShape.Radius * 2 * radiusScale
            );
        }
        else if (collisionShape.Shape is SphereShape3D sphereShape)
        {
            float radiusScale = (globalScale.X + globalScale.Z) / 2;
            return new Vector3(
                sphereShape.Radius * 2 * radiusScale,
                sphereShape.Radius * 2 * radiusScale,
                sphereShape.Radius * 2 * radiusScale
            );
        }

        GD.PrintErr($"Unrecognized shape in {entity.Name}");
        return Vector3.Zero;
    }
}
