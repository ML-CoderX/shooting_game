using UnityEngine;
namespace NeonSiege {
 public enum EnemyKind { Grunt,Gunner,Tank,Swarmer,Boss }
 [CreateAssetMenu(menuName="Neon Siege/Enemy")]
 public sealed class EnemyDefinition : ScriptableObject {
  public EnemyKind Kind;public float Health=60,Speed=3.4f,Damage=10,AttackInterval=1.3f,Scale=1;public int Points=100;
 }
}
