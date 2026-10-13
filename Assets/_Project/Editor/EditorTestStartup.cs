using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace NeonSiege.Editor {
 public static class EditorTestStartup {
  // Unity 6000.6 starts Search after -executeMethod. Entering Play immediately
  // prevents its first-use index creation and makes EnumerateAll index an empty list.
  public static void Prepare(){const string path="UserSettings/Search.index";if(File.Exists(path))return;Directory.CreateDirectory("UserSettings");foreach(var type in TypeCache.GetTypesDerivedFrom<ScriptableObject>()){if(type.FullName!="UnityEditor.Search.SearchDatabase")continue;var create=type.GetMethod("CreateDefaultIndex",BindingFlags.Static|BindingFlags.NonPublic,null,new[]{typeof(string)},null);create?.Invoke(null,new object[]{path});break;}}
 }
}
