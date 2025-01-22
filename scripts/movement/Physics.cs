using Godot;
using PhysicsUtils;

enum OffsetTypes
{
    INVALID = -1,
}

public partial class Physics
{
    public float Delta { get; set; }
    public bool ShouldFaceOrientation { get; set; } = true;
    private float offset { get; set; } = (float)OffsetTypes.INVALID;
    private Area3D entity { get; set; }
    private ForceManager forceManager { get; set; }
    private PhysicsRays physicsRays { get; set; }
    private RelativeDirection velocities;
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
        physicsRays = new PhysicsRays(entity);
    }

    // TODO: Baseado no conceito de que tudo deve ser setado, isso aqui n pode se chamar Move
    public void Move()
    {
        velocities = forceManager.ComputeForces();

        Vector3 globalNextPathPoint = HorizontalPathMovement();

        GD.Print(physicsRays.EvaluateGroundCollision().Colliding);

        entity.Position = new Vector3(
            globalNextPathPoint.X,
            entity.Position.Y + velocities.Vertical * Delta,
            globalNextPathPoint.Z
        );
    }

    private Vector3 HorizontalPathMovement()
    {
        Vector3 globalNextPointOnCurve = entity.GlobalTransform.Origin;

        if (velocities.Horizontal != 0)
        {
            Path3D path = Store.Instance.PathManager.FindPath("start");

            if (!IsValidOffset()) SetOffset(path);

            offset += velocities.Horizontal * Delta;
            Vector3 nextPointOnCurve = path.Curve.SampleBaked(offset, true);
            globalNextPointOnCurve = path.ToGlobal(nextPointOnCurve);

            if (ShouldFaceOrientation) FaceOrientation(entity, globalNextPointOnCurve);
        }

        return globalNextPointOnCurve;
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
} 
