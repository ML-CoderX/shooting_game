using UnityEngine;
namespace NeonSiege {
 [CreateAssetMenu(menuName="Neon Siege/Wave configuration")]
 public sealed class WaveDefinition : ScriptableObject {
  public int TotalWaves=12,BaseEnemies=12,EnemiesPerWave=3,MaxConcurrent=24,BossEvery=5;public float SpawnInterval=1.25f,OpeningDelay=3;
  public int Count(int wave)=>BaseEnemies+EnemiesPerWave*wave;
 }
}
