using UnityEngine;
namespace NeonSiege {
 public enum UpgradeKind { Damage,FireRate,Speed,Health,Dash,Magazine,Explosive,Lifesteal,Ricochet }
 [CreateAssetMenu(menuName="Neon Siege/Upgrade")]
 public sealed class UpgradeDefinition : ScriptableObject {
  public UpgradeKind Kind;public string Title,Description;public float Amount=.2f;
  public void Apply(Operator p,WeaponSystem w){switch(Kind){case UpgradeKind.Damage:w.DamageMultiplier+=Amount;break;case UpgradeKind.FireRate:w.RateMultiplier+=Amount;break;case UpgradeKind.Speed:p.SpeedMultiplier+=Amount;break;case UpgradeKind.Health:p.MaxHealth+=30;p.Health=Mathf.Min(p.MaxHealth,p.Health+50);break;case UpgradeKind.Dash:p.DashCooldown=Mathf.Max(.7f,p.DashCooldown*.8f);break;case UpgradeKind.Magazine:w.MagazineMultiplier+=.3f;break;case UpgradeKind.Explosive:w.Explosive=true;break;case UpgradeKind.Lifesteal:w.Lifesteal=true;break;case UpgradeKind.Ricochet:w.Ricochet=true;break;}}
  public bool Available(WeaponSystem w)=>!(Kind==UpgradeKind.Explosive&&w.Explosive||Kind==UpgradeKind.Lifesteal&&w.Lifesteal||Kind==UpgradeKind.Ricochet&&w.Ricochet);
 }
}
