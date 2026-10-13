using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace NeonSiege {
 public sealed class Atmosphere : MonoBehaviour {
  public MotionBlur Blur {get;private set;}public Vignette Vignette {get;private set;}
  public void Initialize(){var volume=gameObject.AddComponent<Volume>();volume.isGlobal=true;volume.priority=10;var profile=ScriptableObject.CreateInstance<VolumeProfile>();volume.profile=profile;
   var bloom=profile.Add<Bloom>(true);bloom.intensity.Override(.65f);bloom.threshold.Override(1.1f);bloom.scatter.Override(.6f);
   var grade=profile.Add<ColorAdjustments>(true);grade.postExposure.Override(.35f);grade.contrast.Override(12);grade.saturation.Override(-6);
   var tone=profile.Add<Tonemapping>(true);tone.mode.Override(TonemappingMode.ACES);
   Vignette=profile.Add<Vignette>(true);Vignette.intensity.Override(.23f);Vignette.smoothness.Override(.6f);
   Blur=profile.Add<MotionBlur>(true);Blur.intensity.Override(.12f);Blur.active=false;
   var probe=new GameObject("Arena reflection").AddComponent<ReflectionProbe>();probe.transform.position=Vector3.up*4;probe.size=new Vector3(65,18,65);probe.resolution=128;probe.mode=ReflectionProbeMode.Realtime;probe.refreshMode=ReflectionProbeRefreshMode.OnAwake;probe.timeSlicingMode=ReflectionProbeTimeSlicingMode.IndividualFaces;probe.cullingMask=1;
  }
 }
}
