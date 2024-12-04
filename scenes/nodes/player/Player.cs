using System;
using System.Data.Common;
using Godot;

public partial class Player : CharacterBody3D
{
    [Export]
    private float Speed { get; set; } = 8f;
    private const float JumpVelocity = 10f;

    private PathMovement pathMovement;
    private Vector3 gravity = new Vector3(0, -30f, 0);

    /**
     * TODO: talvez seja legal colocar no _Ready o this do player e outras variaveis a serem enviadas 
       no futuro serem setadas na classe, assim pode evitar passação de parametros durante a execussão, 
       principalmente porque no fim do dia a funçào Move do PathMovement é um método que eu quero executar 
       sempre, não apenas no input ou coisas assim, dessa forma eu tenho as variaveis setadas e internamente 
       o PathMovement lida com o que precisa lidar de forma independente, o player apenas passa a informação 
       para o PathMovement e ele decide oq fazer com isso usando apenas uma chamada.

     * TODO: O player no formato de input atual, quando aperta os 2 inputs ao mesmo tempo ele para
       provavelmente porque temos algo tipo 1 e -1, que fica 0 quando ambos estao ativos. Isso é
       horrivel pra jogabilidade, talvez seja legal fazer um sistema de prioridade, onde o ultimo
       input que foi dado é o que vale, ou seja, se eu aperto pra esquerda e depois pra direita
       o que vale é o pra direita
     */

    public override void _Ready()
    {
        pathMovement = new PathMovement(this);
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!pathMovement.IsOnGround)
        {
            GD.Print("Player is not on the ground");
            Velocity += gravity * (float)delta;
        }

        if (Input.IsActionJustPressed("jump") && pathMovement.IsOnGround)
        {
            Velocity = new Vector3(Velocity.X, JumpVelocity, Velocity.Z);
        }

        float inputDir = Input.GetAxis("move-left", "move-right");

        pathMovement.Move(Speed * inputDir, (float)delta); 
    }
}
