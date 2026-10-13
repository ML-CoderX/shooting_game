using UnityEngine;
namespace NeonSiege {
 [CreateAssetMenu(menuName="Neon Siege/Weapon")]
 public sealed class WeaponDefinition : ScriptableObject {
  public int Slot; public string DisplayName="R-201 VANGUARD";public float Damage=20,FireRate=8,Spread=.7f,Recoil=.5f,ReloadTime=1.6f,Range=100;
  public int Magazine=30,Pellets=1;public bool Pierce,Projectile;public Color Accent=Color.cyan;
 }
}
