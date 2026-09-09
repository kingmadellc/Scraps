using UnityEditor;
using UnityEditor.SceneManagement;
namespace Jimothy.Editor {
public static class PlaytestLaunch {
 public static void Start() {
  EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");
  EditorApplication.delayCall += () => { EditorApplication.isPlaying = true; };
 }
}
}
