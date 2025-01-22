using Godot;
using PhysicsUtils;

public static class KnownForces
{
    public const string Gravity = "Gravity";
    public const string Jump = "Jump";
    public const string Move = "Move";
}

public partial class Player : Area3D
{
	[Export]
	private float Speed { get; set; } = 1f;
	private const float JumpVelocity = 10f;

	private Physics physics;
	private RelativeDirection gravity = new RelativeDirection(0, -1f, 0);
	private ForceManager forceManager;

	/**
	 * TODO: talvez seja legal colocar no _Ready o this do player e outras variaveis a serem enviadas 
	   no futuro serem setadas na classe, assim pode evitar passação de parametros durante a execussão, 
	   principalmente porque no fim do dia a funçào Move do physics é um método que eu quero executar 
	   sempre, não apenas no input ou coisas assim, dessa forma eu tenho as variaveis setadas e internamente 
	   o physics lida com o que precisa lidar de forma independente, o player apenas passa a informação 
	   para o physics e ele decide oq fazer com isso usando apenas uma chamada.

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
	}
	public override void _PhysicsProcess(double delta)
	{
        physics.Delta = (float)delta;
        physics.ShouldFaceOrientation = true;
		// if (!IsOnFloor()) // TODO: physics.IsOnFloor
		// {
            forceManager.AddOnForce(gravity, KnownForces.Gravity);
			// Velocity += gravity * (float)delta;
			/** 
			TODO: 
				physics.addForce(
					new Vector3(
						0, 
						gravity.Y * (float)delta), 
						0
					);
				);
			**/
		// }


		// if (Input.IsActionJustPressed("jump") && IsOnFloor()) // TODO: physics.IsOnFloor
		// {
		// 	Velocity = new Vector3(Velocity.X, JumpVelocity, Velocity.Z);
		// 	// TODO: physics.addForce(new Vector3(0, JumpVelocity, 0), 0);
		// }

		float inputDir = Input.GetAxis("move-left", "move-right");
        if (inputDir != 0)
        {
            forceManager.AddOnForce(new RelativeDirection(inputDir * Speed, 0, 0), KnownForces.Move);
        }


		physics.Move();
		// TODO: physics.Move(delta) precisa do delta?
	}
}
