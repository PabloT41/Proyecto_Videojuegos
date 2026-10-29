using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using System.IO;
using PixelMiner;

public static class PixelMinerBuilder {
 [MenuItem("Pixel Miner/Crear escena principal")]
 public static void CreateScene(){
 var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
 var camera=new GameObject("Main Camera").AddComponent<Camera>();camera.tag="MainCamera";camera.orthographic=true;camera.orthographicSize=5;camera.backgroundColor=new Color(.04f,.07f,.12f);camera.clearFlags=CameraClearFlags.SolidColor;
 camera.gameObject.AddComponent<AudioListener>();camera.transform.position=new Vector3(0,0,-10);
 new GameObject("Pixel Miner - Game Controller").AddComponent<PixelMinerGame>();
 Directory.CreateDirectory("Assets/PixelMiner/Scenes");
 EditorSceneManager.SaveScene(scene,"Assets/PixelMiner/Scenes/PixelMiner.unity");
 EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/PixelMiner/Scenes/PixelMiner.unity",true)};
 PlayerSettings.companyName="Proyecto UDLA";PlayerSettings.productName="Pixel Miner - Guardianes de Cristal";
 PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.resizableWindow=true;PlayerSettings.runInBackground=true;
 PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone,ScriptingImplementation.Mono2x);
 PlayerSettings.colorSpace=ColorSpace.Linear;
 AssetDatabase.SaveAssets();
 }
 static void Assert(bool condition,string message){if(!condition)throw new System.Exception("FAILED: "+message);Debug.Log("PASS: "+message);}
 [MenuItem("Pixel Miner/Verificar reglas")]
 public static void TestRules(){
 var s=new MineState();Assert(s.Mining==1&&s.Damage==1&&s.hp==100,"initial values");
 Assert(!s.EnterBattle()&&!s.Buy(true),"locked battle and insufficient funds");
 for(int i=0;i<50;i++)s.Mine();
 Assert(s.mined==50&&s.coins==50,"50 accumulated crystals");
 Assert(!s.Ready,"first mining upgrade required");
 Assert(s.Buy(true)&&s.coins==25&&s.Mining==2,"mining purchase and cost");
 Assert(s.Buy(false)&&s.coins==5&&s.Damage==3,"damage purchase and cost");
 Assert(s.EnterBattle()&&s.enemyHp==20,"stone guardian health");
 int before=s.coins;Assert(s.Mine()==0&&!s.Buy(true)&&s.coins==before,"mining and shop blocked in combat");
 s.EnemyAttack();Assert(s.hp==92,"automatic stone attack amount");
 while(s.fighting)s.Hit();
 Assert(s.stage==2&&s.coins==80&&s.hp==100,"stone reward once and deep cave unlock");
 s.Hit();Assert(s.coins==80,"no repeated reward");
 Assert(s.EnterBattle()&&s.enemyHp==100,"legendary guardian health");
 for(int i=0;i<9;i++)s.EnemyAttack();
 Assert(s.defeated&&!s.fighting&&s.hp==0,"defeat and clamped health");
 int level=s.damageLevel,coins=s.coins;s.Retry();
 Assert(s.stage==2&&s.hp==100&&s.damageLevel==level&&s.coins==coins,"retry preserves progress");
 Assert(s.EnterBattle(),"retry battle");
 while(s.fighting)s.Hit();
 Assert(s.won&&s.coins==coins+250,"final victory and reward");
 Assert(s.Mine()==0&&!s.Buy(true)&&!s.EnterBattle(),"victory locks gameplay");
 var loaded=JsonUtility.FromJson<MineState>(JsonUtility.ToJson(s));
 Assert(loaded.won&&loaded.coins==s.coins,"save round trip");
 // One-click mining at every upgrade must match advertised value.
 var m=new MineState{coins=10000};for(int i=0;i<8;i++){m.Buy(true);int b=m.coins;int n=m.Mine();Assert(m.coins-b==n&&n==m.Mining,"upgrade "+i+" extraction");}
 Directory.CreateDirectory("QA");File.WriteAllText("QA/Reglas_VERIFICADAS.txt","PASS: initial state, progress gate, economy, upgrades, combat locks, 20/100 HP, attack damage, defeat, retry, rewards, victory, save round-trip and extraction scaling.\n");
 }
 [MenuItem("Pixel Miner/Exportar arte original")]
 public static void ExportArt(){
 Directory.CreateDirectory("Assets/PixelMiner/Art");
 var names=new[]{"CuevaInicial","CuevaProfunda","Cristal","CristalLegendario","Minero","GuardianPiedra","GuardianLegendario"};
 var art=new[]{PixelArt.Cave(false),PixelArt.Cave(true),PixelArt.Crystal(),PixelArt.Crystal(true),PixelArt.Miner(),PixelArt.Guardian(false),PixelArt.Guardian(true)};
 for(int i=0;i<names.Length;i++)File.WriteAllBytes("Assets/PixelMiner/Art/"+names[i]+".png",art[i].EncodeToPNG());
 for(int f=0;f<6;f++)File.WriteAllBytes("Assets/PixelMiner/Art/Minero_Picar_"+f+".png",PixelArt.MinerFrame(f).EncodeToPNG());
 for(int f=0;f<2;f++)File.WriteAllBytes("Assets/PixelMiner/Art/Minero_Respirar_"+f+".png",PixelArt.MinerBreathing(f).EncodeToPNG());
 AssetDatabase.Refresh();
 foreach(string name in names){var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/PixelMiner/Art/"+name+".png");importer.textureType=TextureImporterType.Sprite;importer.filterMode=FilterMode.Point;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.spritePixelsPerUnit=32;importer.SaveAndReimport();}
 }
 [MenuItem("Pixel Miner/Compilar para Windows")]
 public static void Build(){
 CreateScene();TestRules();ExportArt();
 var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/PixelMiner/Scenes/PixelMiner.unity"},locationPathName="Build/Windows/PixelMiner.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
 if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("Windows build failed: "+report.summary.result);
 File.WriteAllText("QA/Build_VERIFICADO.txt","Windows x64 build succeeded. Unity "+Application.unityVersion+"\n"+report.summary.totalSize+" bytes.\n");
 }
}



