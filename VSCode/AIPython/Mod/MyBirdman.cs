using System.Reflection;
using Monocle;
using TowerFall;

namespace TFModFortRiseAiPython {
  public static class MyBirdman
  {
    public static StateEntity GetState(this Birdman ent) {
      var aiState = new StateEntity { type = "birdman" };

      if ((Counter)Util.GetFieldValue("attackCooldown", typeof(Birdman), ent, BindingFlags.Public | BindingFlags.Instance) 
          && !(bool)Util.GetFieldValue("canArrowAttack", typeof(Birdman), ent, BindingFlags.Public | BindingFlags.Instance)) {
        aiState.state = "resting";
      } else {
        switch (ent.State) {
          case 0:
            aiState.state = "idle";
            break;
          case 1:
            aiState.state = "attack";
            break;
        }
      }
      
      ExtEntity.SetAiState(ent, aiState);
      return aiState;
    }
  }
}
