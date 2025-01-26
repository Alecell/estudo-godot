namespace PhysicsState {
  public class Ground
  {
      public bool Colliding = false;
      public bool WillCollide = false;
  }

  public class State
  {
      public Ground Ground = new Ground();
  }
}