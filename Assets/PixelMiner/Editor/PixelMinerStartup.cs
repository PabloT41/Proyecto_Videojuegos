using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PixelMinerStartup {
 const string ScenePath="Assets/PixelMiner/Scenes/PixelMiner.unity";
 static PixelMinerStartup(){EditorApplication.delayCall+=OpenInitialScene;}
 static void OpenInitialScene(){
 if(EditorApplication.isPlayingOrWillChangePlaymode||EditorApplication.isCompiling||EditorApplication.isUpdating){EditorApplication.delayCall+=OpenInitialScene;return;}
 EditorSettings.defaultBehaviorMode=EditorBehaviorMode.Mode2D;
 var scene=SceneManager.GetActiveScene();
 if(!SessionState.GetBool("PixelMiner.InitialSceneChecked",false)){
 SessionState.SetBool("PixelMiner.InitialSceneChecked",true);
 if(string.IsNullOrEmpty(scene.path)&&!scene.isDirty&&System.IO.File.Exists(ScenePath))
 EditorSceneManager.OpenScene(ScenePath);
 }
 if(SceneView.lastActiveSceneView!=null)SceneView.lastActiveSceneView.in2DMode=true;
 }
}
