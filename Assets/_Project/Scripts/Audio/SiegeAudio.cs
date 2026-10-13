using UnityEngine;
namespace NeonSiege {
 public sealed class SiegeAudio : MonoBehaviour {
  public AudioSource Sfx,Music;AudioClip shot,hit,reload,kill,ui;public float Master=.8f,SfxVolume=.75f,MusicVolume=.25f;
  public void Initialize(){Sfx=gameObject.AddComponent<AudioSource>();Music=gameObject.AddComponent<AudioSource>();var mixer=Resources.Load<UnityEngine.Audio.AudioMixer>("Audio/Siege");if(mixer){Sfx.outputAudioMixerGroup=mixer.FindMatchingGroups("SFX")[0];Music.outputAudioMixerGroup=mixer.FindMatchingGroups("Music")[0];}shot=Tone("Pulse",.11f,100,.7f);hit=Tone("Confirm",.045f,1400,.15f);reload=Tone("Magazine",.18f,380,.4f);kill=Tone("Collapse",.2f,60,.7f);ui=Tone("Interface",.12f,660,.05f);Music.clip=MusicLoop();Music.loop=true;Music.Play();}
  static AudioClip Tone(string name,float duration,float frequency,float noise){const int rate=22050;var data=new float[(int)(rate*duration)];var rng=new System.Random(41);for(int i=0;i<data.Length;i++){float t=(float)i/rate;data[i]=(Mathf.Sin(t*frequency*6.283f*(1-t))* (1-noise)+(float)(rng.NextDouble()*2-1)*noise)*Mathf.Exp(-t/duration*7)*.7f;}var clip=AudioClip.Create(name,data.Length,1,rate,false);clip.SetData(data,0);return clip;}
  static AudioClip MusicLoop(){const int rate=22050;var data=new float[rate*8];float[] notes={55,55,65.4f,49,55,82.4f,73.4f,49};for(int i=0;i<data.Length;i++){float t=(float)i/rate,beat=t%.5f;float f=notes[(int)t];float bass=Mathf.Sin(t*f*6.283f)*Mathf.Exp(-beat*9);float kick=Mathf.Sin(6.283f*(45*beat+4*(1-Mathf.Exp(-beat*30))))*Mathf.Exp(-beat*20);float pad=Mathf.Sin(t*f*4*6.283f)*.07f;data[i]=bass*.25f+kick*.35f+pad;}var clip=AudioClip.Create("Original siege sequence",data.Length,1,rate,false);clip.SetData(data,0);return clip;}
  public void Shot(float pitch=1){Sfx.pitch=pitch;Sfx.PlayOneShot(shot,.55f);}public void Hit(bool fatal){Sfx.PlayOneShot(fatal?kill:hit,.6f);}public void Reload(){Sfx.PlayOneShot(reload,.4f);}public void Click(){Sfx.PlayOneShot(ui,.4f);}
  public void Intensity(int wave){Music.pitch=1+Mathf.Min(wave,10)*.012f;}
  void Update(){Sfx.volume=Master*SfxVolume;Music.volume=Master*MusicVolume;}
 }
}

