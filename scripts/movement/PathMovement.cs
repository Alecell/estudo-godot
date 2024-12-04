using Godot;

public partial class PathMovement : Node
{
    public bool IsOnGround { get; set; } = false;

    private CharacterBody3D Entity { get; set; }
    /**
     * TODO: Adicionar variante de caminho circular

     * TODO: Adicionar variante de caminho com pontos de controle para alteração de path
     
     * TODO: Adicionar condicional de que quando o player tiver uma Velocity aplicada
     o controle do player deve ser interrompido até que a força esteja próxima de zero

     * TODO: Fazer os dados da store serem acessados de forma dinamica, ou seja carrego uma cena,
     essa cena define um path, um position no path e qual path ela vai, assim posicionar o player
     baseado nisso aqui na linha Store.Instance.PathManager.FindPath("start");, ja que no momento
     o path é mockado
     */

    public PathMovement(CharacterBody3D player)
    {
        Entity = player;
    }

    // TODO: Baseado no conceito de que tudo deve ser setado, isso aqui n pode se chamar Move
    public void Move(float speed, float delta, bool shouldFaceOrientation = true)
    {
        Vector3 displacement = Vector3.Zero;

        if (speed != 0)
        {
            Path3D path = Store.Instance.PathManager.FindPath("start");

            Vector3 playerPositionRelativeToPath = path.ToLocal(Entity.GlobalTransform.Origin);
            float closestOffset = path.Curve.GetClosestOffset(playerPositionRelativeToPath);
            Vector3 nextPointOnCurve = path.Curve.SampleBaked(closestOffset + speed * delta);
            Vector3 globalPointInPath = path.ToGlobal(nextPointOnCurve);
            displacement = globalPointInPath - Entity.GlobalTransform.Origin;

            if (shouldFaceOrientation)
            {
                FaceOrientation(Entity, globalPointInPath);
            }

            Entity.Position = new Vector3(globalPointInPath.X, Entity.Position.Y, globalPointInPath.Z);
        }

        Vector3 movement = new Vector3(displacement.X, Entity.Velocity.Y, displacement.Z);

        if (!Entity.Velocity.IsZeroApprox()) {
            KinematicCollision3D collision = Entity.MoveAndCollide(movement * delta);

            handleCollision(collision);
        }
    }

    private void FaceOrientation(CharacterBody3D player, Vector3 globalPointInPath) {
        Vector3 movementDirection = (player.GlobalTransform.Origin - globalPointInPath).Normalized();
        movementDirection.Y = 0;
        player.LookAt(player.GlobalTransform.Origin + movementDirection, Vector3.Up);
    }

    private void handleCollision(KinematicCollision3D collision)
    {
        if (collision != null)
        {
            IsOnGround = collision.GetNormal().Dot(Vector3.Up) > 0.7;

            if (IsOnGround)
            {
                Entity.Velocity = new Vector3(Entity.Velocity.X, 0, Entity.Velocity.Z);
            }
        }
        else
        {
            IsOnGround = false;
        }
    }
} 
