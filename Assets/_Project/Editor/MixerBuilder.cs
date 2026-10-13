using System;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEditor;
namespace NeonSiege.Editor {
 public static class MixerBuilder {
  public static void Create(){Directory.CreateDirectory("Assets/_Project/Resources/Audio");const string path="Assets/_Project/Resources/Audio/Siege.mixer";if(AssetDatabase.LoadAssetAtPath<UnityEngine.Audio.AudioMixer>(path))return;Type type=null;foreach(var candidate in TypeCache.GetTypesDerivedFrom<UnityEngine.Audio.AudioMixer>()){if(candidate.FullName=="UnityEditor.Audio.AudioMixerController"){type=candidate;break;}}if(type==null)throw new Exception("Audio mixer authoring API unavailable");const BindingFlags flags=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.Instance;var mixer=(UnityEngine.Object)type.GetMethod("CreateMixerControllerAtPath",flags).Invoke(null,new object[]{path});var master=(UnityEngine.Object)type.GetProperty("masterGroup",flags).GetValue(mixer);var create=type.GetMethod("CreateNewGroup",flags);var sfx=(UnityEngine.Object)create.Invoke(mixer,new object[]{"SFX",true});var music=(UnityEngine.Object)create.Invoke(mixer,new object[]{"Music",true});var serialized=new SerializedObject(master);var children=serialized.FindProperty("m_Children");children.arraySize=2;children.GetArrayElementAtIndex(0).objectReferenceValue=sfx;children.GetArrayElementAtIndex(1).objectReferenceValue=music;serialized.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(mixer);AssetDatabase.SaveAssets();Debug.Log("MIXER_CREATED");}
  public static void Inspect(){foreach(var type in TypeCache.GetTypesDerivedFrom<UnityEngine.Audio.AudioMixer>()){if(type.FullName!="UnityEditor.Audio.AudioMixerController")continue;foreach(var m in type.GetMethods(BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))if(m.Name.Contains("Create")||m.Name.Contains("Group")||m.Name.Contains("Expose"))Debug.Log("MIXER_API "+m);}}
 }
}

