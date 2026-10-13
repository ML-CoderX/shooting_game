using System;
using System.IO;
using UnityEngine;
namespace NeonSiege {
 [Serializable] public sealed class SaveData {
  public int HighScore,Quality=1,Fps=60,Resolution=1,Fullscreen=1,Shadows=1,AntiAliasing=1;public bool VSync,MotionBlur;
  public float Master=.8f,Music=.25f,Sfx=.75f,Sensitivity=.12f,Fov=70;public string Bindings="";
 }
 public static class SaveStore {
  public static string PathName=>Path.Combine(Application.persistentDataPath,"profile.json");
  public static SaveData Load(){try{return File.Exists(PathName)?JsonUtility.FromJson<SaveData>(File.ReadAllText(PathName))??new SaveData():new SaveData();}catch(Exception e) when(e is IOException||e is ArgumentException){Debug.LogWarning("Profile could not be read; defaults restored.");return new SaveData();}}
  public static void Save(SaveData data){try{Directory.CreateDirectory(Application.persistentDataPath);string temp=PathName+".tmp";File.WriteAllText(temp,JsonUtility.ToJson(data,true));File.Copy(temp,PathName,true);File.Delete(temp);}catch(IOException){Debug.LogWarning("Profile could not be saved.");}}
 }
}
