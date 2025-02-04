using Godot;
using PhysicsUtils;

public partial class Player : Area3D
{
    [Export]
    private float Acceleration { get; set; } = 3f;
    private const float JumpVelocity = 20f;

    private Physics physics;
    private readonly RelativeDirection gravity = new RelativeDirection(0f, -1f, 0f);
    private ForceManager forceManager;

    /**
	 * TODO: O player no formato de input atual, quando aperta os 2 inputs ao mesmo tempo ele para
	   provavelmente porque temos algo tipo 1 e -1, que fica 0 quando ambos estao ativos. Isso é
	   horrivel pra jogabilidade, talvez seja legal fazer um sistema de prioridade, onde o ultimo
	   input que foi dado é o que vale, ou seja, se eu aperto pra esquerda e depois pra direita
	   o que vale é o pra direita
	 */

    public override void _Ready()
    {
        forceManager = new ForceManager();
        physics = new Physics(this, forceManager);
        forceManager.internalForce.Horizontal.Limit = 10f;
    }
    public override void _PhysicsProcess(double delta)
    {
        physics.Delta = (float)delta;
        physics.ShouldFaceOrientation = true;
        float inputDir = Input.GetAxis("move-left", "move-right");

        if (!physics.State.Ground.Colliding)
        {
            forceManager.internalForce.Add(gravity);
        }
        if (physics.State.Ground.WillCollide || physics.State.Ground.Colliding)
        {
            forceManager.internalForce.Set(ForceComponent.VERTICAL, 0f);
        }

        if (Input.IsActionJustPressed("jump") && physics.State.Ground.Colliding)
        {
            forceManager.internalForce.Add(new RelativeDirection(0f, JumpVelocity, 0f));
        }

        if (inputDir != 0)
        {
            forceManager.internalForce.Add(new RelativeDirection(inputDir * Acceleration));
        }

        physics.Apply();
    }
}
