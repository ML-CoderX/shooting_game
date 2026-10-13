using UnityEngine;
namespace NeonSiege {
 public sealed class KillFeedback : MonoBehaviour {
  float stop;GameSession session;
  public void Initialize(GameSession run){session=run;run.Boot.Weapons.Hit+=(position,damage,fatal)=>{if(fatal&&session.State==RunState.Playing){stop=.035f;Time.timeScale=.12f;}};}
  void Update(){if(stop<=0)return;stop-=Time.unscaledDeltaTime;if(stop<=0&&session.State==RunState.Playing)Time.timeScale=1;}
 }
}
