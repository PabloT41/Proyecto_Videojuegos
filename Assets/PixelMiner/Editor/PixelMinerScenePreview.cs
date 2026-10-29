using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using PixelMiner;

[InitializeOnLoad]
public static class PixelMinerScenePreview {
 const string RootName="Vista previa del nivel";
 const string ScenePath="Assets/PixelMiner/Scenes/PixelMiner.unity";
 const string Art="Assets/PixelMiner/Art/";
 static PixelMinerScenePreview(){
 EditorApplication.delayCall+=EnsurePreview;
 EditorSceneManager.sceneOpened+=(scene,mode)=>EditorApplication.delayCall+=EnsurePreview;
 }
 static void EnsurePreview(){
 if(EditorApplication.isPlayingOrWillChangePlaymode)return;
 if(EditorApplication.isCompiling||EditorApplication.isUpdating){EditorApplication.delayCall+=EnsurePreview;return;}
 if(SceneManager.GetActiveScene().path!=ScenePath||GameObject.Find(RootName)!=null)return;
 Show(0);
 }
 [MenuItem("Pixel Miner/Vista previa/Mina")]
 public static void Mine(){Show(0);}
 [MenuItem("Pixel Miner/Vista previa/Guardian de Piedra")]
 public static void Stone(){Show(1);}
 [MenuItem("Pixel Miner/Vista previa/Guardian Legendario")]
 public static void Boss(){Show(2);}
 [MenuItem("Pixel Miner/Vista previa/Encuadrar nivel")]
 public static void Frame(){
 var view=SceneView.lastActiveSceneView;
 if(view!=null){view.in2DMode=true;view.Frame(new Bounds(Vector3.zero,new Vector3(17,11,1)),false);}
 }
 static Material PreviewMaterial(){
 const string path=Art+"VistaPrevia.mat";
 var material=AssetDatabase.LoadAssetAtPath<Material>(path);
 if(material!=null)return material;
 var shader=Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
 if(shader==null)shader=Shader.Find("Sprites/Default");
 material=new Material(shader);
 AssetDatabase.CreateAsset(material,path);
 return material;
 }
 static Transform FindOrCreate(Transform parent,string name){
 var child=parent.Find(name);
 if(child!=null)return child;
 var go=new GameObject(name);go.transform.SetParent(parent,false);
 return go.transform;
 }
 static void Sprite(Transform root,string name,string file,Vector2 position,Vector2 size,int order,bool visible,Material material){
 var tr=FindOrCreate(root,name);
 var renderer=tr.GetComponent<SpriteRenderer>();
 if(renderer==null)renderer=tr.gameObject.AddComponent<SpriteRenderer>();
 var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Art+file+".png");
 if(sprite==null){Debug.LogError("No se encontro el sprite de vista previa: "+file);return;}
 renderer.sprite=sprite;renderer.sharedMaterial=material;renderer.sortingOrder=order;
 tr.localPosition=new Vector3(position.x,position.y,0);
 tr.localScale=new Vector3(size.x/sprite.bounds.size.x,size.y/sprite.bounds.size.y,1);
 tr.gameObject.SetActive(visible);
 }
 static void Show(int mode){
 if(EditorApplication.isPlayingOrWillChangePlaymode){Debug.Log("Sal de Play para cambiar la vista previa del editor.");return;}
 if(SceneManager.GetActiveScene().path!=ScenePath){Debug.Log("Abre la escena PixelMiner.unity para usar esta vista previa.");return;}
 var go=GameObject.Find(RootName);
 if(go==null){
 go=new GameObject(RootName);go.tag="EditorOnly";go.AddComponent<EditorLevelPreview>();
 Undo.RegisterCreatedObjectUndo(go,"Crear vista previa del nivel");
 }
 var root=go.transform;
 var material=PreviewMaterial();
 Sprite(root,"Mina",mode==2?"CuevaProfunda":"CuevaInicial",Vector2.zero,new Vector2(16,9.654f),0,true,material);
 Sprite(root,"Minero","Minero",new Vector2(mode==0?-1.48f:-3.69f,-.58f),new Vector2(4.154f,4.154f),2,true,material);
 Sprite(root,"Cristal","Cristal",new Vector2(.885f,-.154f),new Vector2(3.692f,4.154f),3,mode==0,material);
 Sprite(root,"Guardian de Piedra","GuardianPiedra",new Vector2(.942f,-.404f),new Vector2(4.308f,4.846f),3,mode==1,material);
 Sprite(root,"Guardian Legendario","GuardianLegendario",new Vector2(.942f,-.096f),new Vector2(4.923f,5.538f),3,mode==2,material);
 var camera=Camera.main;
 if(camera!=null){camera.orthographic=true;camera.orthographicSize=5.3f;camera.backgroundColor=new Color(.035f,.05f,.08f);EditorUtility.SetDirty(camera);}
 EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
 EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
 AssetDatabase.SaveAssets();
 Selection.activeGameObject=go;
 Frame();
 SceneView.RepaintAll();
 EditorApplication.QueuePlayerLoopUpdate();
 }
}
