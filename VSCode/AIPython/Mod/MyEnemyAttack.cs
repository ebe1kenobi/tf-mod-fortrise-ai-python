using TowerFall;

namespace TFModFortRiseAiPython {
  public static class MyEnemyAttack
  {
    public static StateEntity GetState(this EnemyAttack ent) {
      var aiState = new StateEntity {
        type = "enemyAttack",
      };

      ExtEntity.SetAiState(ent, aiState);
      aiState.canHurt = true;
      return aiState;
    }
  }
}
