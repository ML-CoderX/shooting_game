using UnityEngine;
namespace NeonSiege {
 public class Damageable : MonoBehaviour {
  public float Health=100,MaxHealth=100;
  public virtual bool TakeDamage(float amount){Health=Mathf.Max(0,Health-amount);return Health<=0;}
 }
}
