using Godot;
using PhysicsState;
using PhysicsUtils;


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

     * TODO: Adicionar variante de caminho com pontos de controle para alteração de path
     
     * TODO: Adicionar condicional de que quando o player tiver uma Velocity aplicada
     o controle do player deve ser interrompido até que a força esteja próxima de zero

     * TODO: Fazer os dados da store serem acessados de forma dinamica, ou seja carrego uma cena,
     essa cena define um path, um position no path e qual path ela vai, assim posicionar o player
     baseado nisso aqui na linha Store.Instance.PathManager.FindPath("start");, ja que no momento
     o path é mockado

     * TODO: Testar se nao tem problema mesmo rotacionar o vetor velocity constantemente na direção da curva
     Se funcionar vai ser legal pq isso é algo mais interessante
     */


    public Physics(Area3D entity, ForceManager forceManager)
    {
        this.entity = entity;
        this.forceManager = forceManager;
        entitySize = GetCollisionShapeSizes();
        physicsGroundRays = new PhysicsGroundRays(this);

        State = new State();
    }

    // TODO: Baseado no conceito de que tudo deve ser setado, isso aqui n pode se chamar Move
    public void Move()
    {
        Velocities = forceManager.ComputeForces();
        var groundState = physicsGroundRays.EvaluateCollisions();
        var globalNextPathPoint = HorizontalPathMovement();
        var nextEntityPositionY = entity.GlobalTransform.Origin.Y + Velocities.Vertical * Delta;
        
        State.Ground.Colliding = groundState.Colliding || groundState.WillCollide;
        State.Ground.WillCollide = groundState.WillCollide;

        if (groundState.WillCollide) {
            nextEntityPositionY = GetGroundYPosition(groundState);
        }

        entity.GlobalTransform = new Transform3D(
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

        if (Velocities.Horizontal != 0)
        {
            Path3D path = Store.Instance.PathManager.FindPath("start");

            if (!IsValidOffset()) SetOffset(path);

            offset += Velocities.Horizontal * Delta;
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
        } else
        {
            distance = Mathf.Min(
                groundState.Rays["bottom"]["frontDown"].Distance,
                groundState.Rays["bottom"]["backDown"].Distance
            );
        }

        if (!float.IsNaN(distance)) {
            float newEntityPositionY = entityPosition.Y - (distance - entitySize.Y);

            return newEntityPositionY;
        }

        return entityPosition.Y;
    }

    private bool IsValidOffset() {
        return offset >= 0;
    }

    private void SetOffset(Path3D path) {
        Vector3 playerPositionRelativeToPath = path.ToLocal(entity.GlobalTransform.Origin);
        offset = path.Curve.GetClosestOffset(playerPositionRelativeToPath);
    }

    private void FaceOrientation(Area3D player, Vector3 globalPointInPath) {
        Vector3 movementDirection = (player.GlobalTransform.Origin - globalPointInPath).Normalized();
        movementDirection.Y = 0;
        player.LookAt(player.GlobalTransform.Origin + movementDirection, Vector3.Up);
    }

    private Vector3 GetCollisionShapeSizes()
    {
        var collisionShape = entity.GetNodeOrNull<CollisionShape3D>("CollisionShape3D");
        var mesh = entity.GetNodeOrNull<MeshInstance3D>("PlayerModel");
        GD.Print("Mesh: ", mesh);
        if (mesh.Mesh is ArrayMesh arrayMesh)
        {
            Aabb aabb = arrayMesh.GetAabb();
            GD.Print($"AABB Min: {aabb.Position}, Size: {aabb.Size}, Max: {aabb.End}");
        }


        if (collisionShape == null || collisionShape.Shape == null)
        {
            GD.PrintErr($"No CollisionShape3D found in {entity.Name}!");
            return Vector3.Zero;
        }

        // ✅ Corrected scale extraction
        Vector3 globalScale = new Vector3(
            collisionShape.GlobalTransform.Basis.X.Length(),
            collisionShape.GlobalTransform.Basis.Y.Length(),
            collisionShape.GlobalTransform.Basis.Z.Length()
        );

        if (collisionShape.Shape is BoxShape3D boxShape)
        {
            GD.Print("BoxShape3D");
            return boxShape.Size * globalScale;
        }
        else if (collisionShape.Shape is CylinderShape3D cylinderShape)
        {
            GD.Print("CylinderShape3D");
            float radiusScale = (globalScale.X + globalScale.Z) / 2; // ✅ Maintain circular shape
            return new Vector3(
                cylinderShape.Radius * 2 * radiusScale,
                cylinderShape.Height * globalScale.Y,
                cylinderShape.Radius * 2 * radiusScale
            );
        }
        else if (collisionShape.Shape is CapsuleShape3D capsuleShape)
        {
            GD.Print("CapsuleShape3D");
            float radiusScale = (globalScale.X + globalScale.Z) / 2; // ✅ Maintain circular shape
            return new Vector3(
                capsuleShape.Radius * 2 * radiusScale,
                capsuleShape.Height * globalScale.Y,
                capsuleShape.Radius * 2 * radiusScale
            );
        }
        else if (collisionShape.Shape is SphereShape3D sphereShape)
        {
            GD.Print("SphereShape3D");
            float radiusScale = (globalScale.X + globalScale.Z) / 2; // ✅ Maintain circular shape
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
