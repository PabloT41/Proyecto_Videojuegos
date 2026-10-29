using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelMiner {
public sealed class PixelMinerGame : MonoBehaviour {
 const string SaveKey="PixelMiner.Save.v1";
 MineState state=new MineState();
 Texture2D cave,deep,crystal,gold,miner,stone,boss;
 Texture2D[] miningFrames=new Texture2D[6];
 Texture2D[] breathingFrames=new Texture2D[2];
 Texture2D[] stoneAttack=new Texture2D[4],bossAttack=new Texture2D[4];
 Texture2D[] slamArcs=new Texture2D[3];
 Texture2D attackRock; float enemyImpactLeft;
 float swingLeft,impactLeft; int queuedSwings;
 Font font; AudioSource audioSource; AudioClip mineTone,hitTone,buyTone;
 bool menu=true,paused,muted,help,resetConfirm;
 float attackClock,hitFlash,toastTime,elapsed;
 string toast="La cueva te espera.";
 readonly List<Pop> pops=new List<Pop>();
 sealed class Pop { public Vector2 p; public float life=1; public string text; public Color color; }
 static readonly Color Ink=Hex("0b1220"),Panel=Hex("131f30"),Line=Hex("293b50"),White=Hex("ecf4f4"),Soft=Hex("9fb2c5"),Mint=Hex("75e5d0"),Gold=Hex("f2c36c");
 static Color Hex(string s){ColorUtility.TryParseHtmlString("#"+s,out var c);return c;}
 GUIStyle Style(int size,Color color,TextAnchor align=TextAnchor.UpperLeft,bool bold=false){
 return new GUIStyle(GUI.skin.label){font=font,fontSize=size,normal={textColor=color},alignment=align,fontStyle=bold?FontStyle.Bold:FontStyle.Normal,wordWrap=true,padding=new RectOffset(0,0,0,0)};
 }
 void Awake(){
 Application.targetFrameRate=60;
 font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
 cave=PixelArt.Cave(false);deep=PixelArt.Cave(true);crystal=PixelArt.Crystal();gold=PixelArt.Crystal(true);miner=PixelArt.Miner();stone=PixelArt.Guardian(false);boss=PixelArt.Guardian(true);
 audioSource=gameObject.AddComponent<AudioSource>();mineTone=Tone(720,.055f);hitTone=Tone(150,.08f);buyTone=Tone(1050,.12f);
 for(int i=0;i<miningFrames.Length;i++)miningFrames[i]=PixelArt.MinerFrame(i);
 for(int i=0;i<2;i++)breathingFrames[i]=PixelArt.MinerBreathing(i);
 for(int i=0;i<4;i++){stoneAttack[i]=PixelArt.GuardianAttack(false,i+1);bossAttack[i]=PixelArt.GuardianAttack(true,i+1);}
 attackRock=PixelArt.AttackRock();
 for(int i=0;i<3;i++)slamArcs[i]=PixelArt.SlamArc(i);
 muted=PlayerPrefs.GetInt("PixelMiner.Muted",0)==1;
 }
 AudioClip Tone(float hz,float duration){int n=(int)(44100*duration);var data=new float[n];for(int i=0;i<n;i++)data[i]=Mathf.Sin(i*hz*2*Mathf.PI/44100)*.07f*(1f-(float)i/n);var clip=AudioClip.Create("Feedback",n,1,44100,false);clip.SetData(data,0);return clip;}
 void Sound(AudioClip clip){if(!muted)audioSource.PlayOneShot(clip);}
 void Update(){
 if(menu||paused||help||resetConfirm||state.won||state.defeated)return;
 elapsed+=Time.deltaTime;enemyImpactLeft=System.Math.Max(0,enemyImpactLeft-Time.deltaTime);
 float oldSwing=swingLeft;
 swingLeft=System.Math.Max(0,swingLeft-Time.deltaTime);
 impactLeft=System.Math.Max(0,impactLeft-Time.deltaTime);
 if(oldSwing>.24f&&swingLeft<=.24f)impactLeft=.22f;
 if(oldSwing>0&&swingLeft<=0&&queuedSwings>0){queuedSwings--;swingLeft=.48f;}hitFlash=Mathf.Max(0,hitFlash-Time.deltaTime*4);
 toastTime=Mathf.Max(0,toastTime-Time.deltaTime);
 for(int i=pops.Count-1;i>=0;i--){pops[i].life-=Time.deltaTime;pops[i].p.y-=45*Time.deltaTime;if(pops[i].life<=0)pops.RemoveAt(i);}
 if(state.fighting){attackClock+=Time.deltaTime;if(attackClock>=state.AttackInterval){attackClock-=state.AttackInterval;state.EnemyAttack();enemyImpactLeft=.28f;Sound(hitTone);Notify("-"+state.EnemyDamage+" PV · El guardián ataca");if(state.defeated)Save();}}
 }
 void OnApplicationQuit(){if(!menu)Save();}
 void OnApplicationPause(bool pause){if(pause&&!menu)Save();}
 void Save(){PlayerPrefs.SetString(SaveKey,JsonUtility.ToJson(state));PlayerPrefs.Save();}
 bool HasSave()=>PlayerPrefs.HasKey(SaveKey);
 void Continue(){try{state=JsonUtility.FromJson<MineState>(PlayerPrefs.GetString(SaveKey))??new MineState();}catch{state=new MineState();}if(state.fighting){state.fighting=false;state.stage=state.stage==1?0:2;state.hp=100;}menu=false;attackClock=0;enemyImpactLeft=0;Notify("Expedición recuperada. Los combates comienzan con vida completa.");}
 void Notify(string text){toast=text;toastTime=4;}
 void NewGame(){state=new MineState();menu=false;paused=false;resetConfirm=false;elapsed=0;attackClock=0;enemyImpactLeft=0;pops.Clear();swingLeft=impactLeft=0;queuedSwings=0;Save();Notify("Haz clic en el cristal. Tu primer objetivo son 50 cristales acumulados.");}
 void Rect(float x,float y,float w,float h,Color color){var old=GUI.color;GUI.color=color;GUI.DrawTexture(new Rect(x,y,w,h),Texture2D.whiteTexture);GUI.color=old;}
 void Box(float x,float y,float w,float h,Color color){Rect(x,y,w,h,Line);Rect(x+1,y+1,w-2,h-2,color);}
 void Text(float x,float y,float w,float h,string text,int size=16,Color? color=null,bool bold=false,TextAnchor align=TextAnchor.UpperLeft){GUI.Label(new Rect(x,y,w,h),text,Style(size,color??White,align,bold));}
 bool Button(float x,float y,float w,float h,string label,bool enabled=true,bool primary=false){
 Rect(x,y+3,w,h,Hex("070d17"));bool hover=new Rect(x,y,w,h).Contains(Event.current.mousePosition);
 Box(x,y,w,h,enabled?(primary?(hover?Hex("a2f3df"):Mint):(hover?Hex("30445c"):Hex("203249"))):Hex("192332"));
 Text(x+12,y,w-24,h,label,w<110?12:15,enabled?(primary?Ink:White):Hex("728092"),true,TextAnchor.MiddleCenter);
 bool old=GUI.enabled;GUI.enabled=old&&enabled;bool clicked=GUI.Button(new Rect(x,y,w,h),GUIContent.none,GUIStyle.none);GUI.enabled=old;return clicked;
 }
 void Bar(float x,float y,float w,float ratio,Color c){Rect(x,y,w,8,Hex("08111f"));Rect(x,y,w*Mathf.Clamp01(ratio),8,c);}
 void Art(Texture2D t,float x,float y,float w,float h){GUI.DrawTexture(new Rect(x,y,w,h),t,ScaleMode.StretchToFill,true);}
 void OnGUI(){
 float scale=Mathf.Min(Screen.width/1280f,Screen.height/800f);
 GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-1280*scale)/2,(Screen.height-800*scale)/2),Quaternion.identity,new Vector3(scale,scale,1));
 Rect(0,0,1280,800,Ink);
 if(menu){DrawMenu();return;}
 GUI.enabled=!Blocked();DrawGame();GUI.enabled=true;
 if(state.won)Result(true);else if(state.defeated)Result(false);
 else if(help)Help();else if(resetConfirm)ResetDialog();else if(paused)PauseDialog();
 }
 void DrawMenu(){
 Art(deep,0,0,1280,800);Rect(0,0,1280,800,new Color(.025f,.04f,.08f,.6f));
 Text(72,54,500,30,"UDLA  /  AVENTURA INCREMENTAL",14,Mint,true);
 Text(72,137,700,90,"PIXEL MINER",68,White,true);
 Text(76,226,600,45,"Guardianes de Cristal",30,Gold,true);
 Text(76,303,530,110,"Un mapa olvidado. Una mina ancestral.\nEl secreto de la cueva está en tus manos.",22,White);
 Text(76,414,490,76,"Extrae cristales, mejora tu pico y desafía a los protectores de piedra hasta descubrir el cristal legendario.",17,Soft);
 GUI.enabled=!help&&!resetConfirm;
 if(Button(76,527,300,56,"COMENZAR EXPEDICIÓN",true,true)){if(HasSave())resetConfirm=true;else NewGame();}
 if(HasSave()&&Button(76,600,300,48,"CONTINUAR EXPEDICIÓN"))Continue();
 if(Button(392,527,162,56,"CÓMO JUGAR"))help=true;
 Art(gold,833,214,224,252);Art(miner,722,424,216,216);Art(stone,950,358,224,252);
 Text(76,731,1000,25,"SOLO CLIC IZQUIERDO   ·   4 ETAPAS   ·   UNA LEYENDA POR DESCUBRIR",13,Soft,true);
 GUI.enabled=true;if(help)Help();if(resetConfirm)ResetDialog();
 }
 void DrawGame(){
 Text(32,22,540,35,"PIXEL MINER",27,White,true);Text(33,61,540,25,"GUARDIANES DE CRISTAL",12,Mint,true);
 Box(784,20,180,63,Panel);Text(800,30,150,18,"MONEDAS",11,Gold,true);Text(800,49,150,30,state.coins.ToString("N0"),24,White,true);
 if(Button(980,27,85,46,muted?"MUDO":"AUDIO")){muted=!muted;PlayerPrefs.SetInt("PixelMiner.Muted",muted?1:0);}
 if(Button(1077,27,72,46,"AYUDA"))help=true;
 if(Button(1161,27,88,46,"PAUSA"))paused=true;
 string[] labels={"01  MINA INICIAL","02  GUARDIÁN","03  CUEVA PROFUNDA","04  LEYENDA"};
 for(int i=0;i<4;i++){float x=32+i*307;Box(x,104,295,42,state.stage==i?Hex("23403f"):Panel);Text(x+12,104,271,42,labels[i],13,state.stage==i?Mint:(state.stage>i?Gold:Soft),true,TextAnchor.MiddleLeft);}
 Box(32,165,836,506,Panel);
 Art(state.stage>=2?deep:cave,34,167,832,502);
 Rect(34,167,832,93,new Color(.035f,.05f,.10f,.80f));
 bool battle=state.fighting;
 Text(56,186,650,35,battle?(state.stage==1?"GUARDIÁN DE PIEDRA":"GUARDIÁN LEGENDARIO"):(state.stage>=2?"EL CORAZÓN DE LA CUEVA":"LA VETA DEL DESPERTAR"),23,White,true);
 Text(56,224,700,28,battle?"Haz clic sobre el guardián para atacar.":"Haz clic sobre el cristal para extraer monedas.",15,Soft);
 // Pixel figures and interaction region occupy the same coordinates.
 DrawCaveMagic();
 int frame=swingLeft>0?System.Math.Min(5,(int)((.48f-swingLeft)/.08f)):-1;
 int breath=Mathf.Sin(elapsed*2.1f)>.25f?1:0;
 float recoil=enemyImpactLeft>0?Mathf.Sin(enemyImpactLeft*65)*3:0;
 Art(frame<0?breathingFrames[breath]:miningFrames[frame],(battle?150:265)+recoil,340,216,216);
 if(battle){
 Text(333,266,350,24,state.enemyHp+" / "+state.EnemyMax+" PV",16,Gold,true,TextAnchor.MiddleCenter);
 Bar(333,297,350,(float)state.enemyHp/state.EnemyMax,state.stage==1?Mint:Gold);
 float offset=hitFlash>0?Mathf.Sin(elapsed*100)*4:0;
 float windup=state.AttackInterval-.8f;
 float attack=attackClock>=windup?(attackClock-windup)/.8f:-1;
 int pose=attack<0?0:attack<.4f?1:2;
 if(enemyImpactLeft>0)pose=2;
 Texture2D enemy=state.stage==1?(pose==0?stone:stoneAttack[pose-1]):(pose==0?boss:bossAttack[pose-1]);
 if(state.stage==1){float compress=pose==1?4:pose==3?8:0;Art(enemy,387+offset,313+compress,224,252-compress);}
 else Art(enemy,371+offset,279,256,288);
 DrawEnemyAttack(attack);
 if(GUI.Button(new Rect(365,278,275,290),GUIContent.none,GUIStyle.none)&&!Blocked()){
 int n=state.Hit();hitFlash=1;Sound(hitTone);PopText("-"+n,new Vector2(485,350),Gold);
 if(!state.fighting){Save();attackClock=0;Notify(state.won?"El cristal legendario ha despertado.":"Guardián vencido: +75 monedas. La cueva profunda está abierta.");}
 }
 Text(317,594,400,26,"PRÓXIMO ATAQUE",11,Soft,true,TextAnchor.MiddleCenter);
 Bar(352,621,330,attackClock/state.AttackInterval,Hex("db7974"));
 }else{
 float bounce=Mathf.Sin(elapsed*2)*5;
 Art(crystal,400,318+bounce+(impactLeft>0?2:0),192,216-(impactLeft>0?4:0));
 DrawImpact();
 if(GUI.Button(new Rect(365,326,270,252),GUIContent.none,GUIStyle.none)&&!Blocked()){
 int n=state.Mine();BeginMiningSwing();Sound(mineTone);PopText("+"+n+" monedas",Event.current.mousePosition,Mint);
 if(state.mined>=50&&state.mined-n<50)Notify("¡50 cristales! Compra la primera mejora de minería para avanzar.");
 if(state.mined%10<state.Mining)Save();
 }
 Text(329,596,380,38,"CLIC PARA EXTRAER  ·  +"+state.Mining+" / clic",14,Mint,true,TextAnchor.MiddleCenter);
 }
 foreach(var p in pops)Text(p.p.x-90,p.p.y,200,34,p.text,23,new Color(p.color.r,p.color.g,p.color.b,p.life),true,TextAnchor.MiddleCenter);
 // Bottom expedition vitals.
 Box(32,687,836,81,Panel);
 Text(52,700,200,24,"MINERO",12,Soft,true);Text(52,723,180,27,state.hp+" / 100 PV",18,White,true);
 Bar(216,730,210,state.hp/100f,Hex("dd7c7b"));
 Text(464,701,180,22,"EXTRAÍDOS EN TOTAL",11,Soft,true);Text(464,726,180,27,state.mined+" cristales",18,White,true);
 Text(684,701,140,22,"DAÑO POR CLIC",11,Soft,true);Text(684,726,150,27,state.Damage+(state.Damage==1?" punto":" puntos"),18,Gold,true);
 DrawShop();
 }
 void DrawEnemyAttack(float progress){
 bool legendary=state.stage==3;
 if(!legendary){
 if(progress>=0&&progress<.64f)Text(368,562,280,24,"¡GOLPE DESCENDENTE!",12,Gold,true,TextAnchor.MiddleCenter);
 // Both fists swing from above the head; the crescent traces their downward path.
 if(progress>=.64f||enemyImpactLeft>.12f){
 float swing=progress>=.64f?Mathf.Clamp01((progress-.64f)/.36f):1;
 int arc=System.Math.Min(2,(int)(swing*3));
 Art(slamArcs[arc],267-swing*38,302+swing*36,244,240);
 }
 if(enemyImpactLeft>0){
 float age=1-enemyImpactLeft/.28f;
 for(int i=0;i<12;i++){
 float a=i*.52f;
 float x=316+Mathf.Sin(a)*(20+age*75);
 float y=542+Mathf.Sin(a+1.57f)*(6+age*14);
 Rect((int)x,(int)y,i%2==0?7:4,3,new Color(.75f,.69f,.55f,1-age));
 }
 // A brief ground shock connects the slam with the miner without stretching an arm.
 Rect(214,542,102,3,new Color(.68f,.95f,.80f,enemyImpactLeft/.28f));
 }
 }else if(progress>=0){
 if(progress<.4f){
 Art(attackRock,414,371-progress*60,48,48);
 Text(368,562,280,24,"¡LANZAMIENTO!",12,Gold,true,TextAnchor.MiddleCenter);
 }else{
 float travel=Mathf.Clamp01((progress-.4f)/.6f);
 float x=426+(210-426)*travel;
 float y=355+89*travel-Mathf.Sin(travel*Mathf.PI)*45;
 for(int i=1;i<=3;i++)Rect((int)x+50+i*10,(int)y+16+i*3,12,2,new Color(.72f,.66f,.55f,.40f-i*.08f));
 Art(attackRock,(int)x,(int)y,54,54);
 }}
 if(enemyImpactLeft>0){
 float age=1-enemyImpactLeft/.28f;
 for(int i=0;i<10;i++){
 float a=i*.63f;
 Rect((int)(236+Mathf.Sin(a)*(15+age*32)),(int)(469+Mathf.Sin(a+1.57f)*(12+age*22)),i%2==0?5:3,4,new Color(.9f,.77f,.51f,1-age));
 }
 }
 }
 void BeginMiningSwing(){
 // Do not restart the anticipation on rapid clicks: finish each swing.
 if(swingLeft<=0)swingLeft=.48f;else queuedSwings=System.Math.Min(2,queuedSwings+1);
 }
 void DrawCaveMagic(){
 // Integer-aligned motes keep the lighting and dust in the pixel-art style.
 for(int i=0;i<18;i++){
 float phase=elapsed*.38f+i*.69f;
 float x=497+Mathf.Sin(i*2.41f)*125;
 float y=524-(phase%2.5f)*70;
 float a=(.35f+.25f*Mathf.Sin(phase*3));
 Rect((int)x,(int)y,i%4==0?4:2,i%4==0?4:2,new Color(.77f,.49f,1,a));
 }
 float pulse=.35f+.08f*Mathf.Sin(elapsed*2);
 Rect(492,518,10,2,new Color(.55f,1,1,pulse));
 }
 void DrawImpact(){
 if(impactLeft<=0)return;
 float p=1-impactLeft/.22f;
 for(int i=0;i<9;i++){
 float angle=i*.75f;
 float x=462+Mathf.Sin(angle)*(8+p*32);
 float y=443+Mathf.Sin(angle+1.57f)*(7+p*28)+p*p*16;
 Rect((int)x,(int)y,i%2==0?5:3,i%2==0?5:3,new Color(.65f,1,.93f,1-p));
 }
 }
 bool Blocked()=>paused||help||resetConfirm||state.won||state.defeated;
 void PopText(string text,Vector2 point,Color c){if(pops.Count>20)pops.RemoveAt(0);pops.Add(new Pop{text=text,p=point,color=c});}
 void DrawShop(){
 Box(888,165,360,603,Panel);
 Text(910,187,310,35,"EL TALLER",23,White,true);Text(910,225,310,45,"Invierte en extracción o combate.\nLas mejoras se conservan al perder.",14,Soft);
 Rect(910,281,316,1,Line);
 Text(910,302,300,30,"Pico de minero",20,Mint,true);
 Text(910,339,310,48,"Nivel "+state.miningLevel+"  ·  +"+state.Mining+" monedas / clic\nSiguiente nivel: +"+(state.Mining+1)+" / clic",15,Soft);
 if(Button(910,398,316,45,"MEJORAR  ·  "+state.MiningCost+" MONEDAS",!Blocked()&&!state.fighting&&state.coins>=state.MiningCost,true)){state.Buy(true);Sound(buyTone);Save();Notify("Pico mejorado: +"+state.Mining+" monedas por clic.");}
 Text(910,470,300,30,"Filo de cristal",20,Gold,true);
 Text(910,507,310,48,"Nivel "+state.damageLevel+"  ·  "+state.Damage+" daño / clic\nSiguiente nivel: "+(state.Damage+2)+" daño / clic",15,Soft);
 if(Button(910,566,316,45,"MEJORAR  ·  "+state.DamageCost+" MONEDAS",!Blocked()&&!state.fighting&&state.coins>=state.DamageCost)){state.Buy(false);Sound(buyTone);Save();Notify("Filo mejorado: "+state.Damage+" puntos de daño por clic.");}
 Text(910,639,310,44,state.fighting?"Taller cerrado durante el combate.":"Consejo: combina ambas mejoras antes de desafiar al guardián.",14,Soft);
 string action=state.fighting?"COMBATE EN CURSO":state.stage==0?"DESAFIAR GUARDIÁN":"DESAFIAR LEYENDA";
 if(Button(910,700,316,46,action,!Blocked()&&state.Ready&&!state.fighting,true)){state.EnterBattle();attackClock=0;Save();Notify("¡Combate! Haz clic sobre el guardián. Ataca cada "+state.AttackInterval+" s.");}
 string objective=state.stage==0?(state.mined<50?"Objetivo: extrae "+state.mined+" / 50 cristales acumulados.":state.miningLevel==0?"Objetivo: compra la primera mejora de minería.":"Objetivo: derrota al Guardián de Piedra."):(state.stage==2?"Objetivo: mejora tus herramientas y derrota al jefe de 100 PV.":"Objetivo: ataca al guardián antes de quedarte sin vida.");
 Text(36,776,1200,23,toastTime>0?toast:objective,13,toastTime>0?Mint:Soft);
 }
 void Overlay(string eyebrow,string title){
 Rect(0,0,1280,800,new Color(.015f,.025f,.05f,.88f));Box(300,164,680,474,Panel);
 Text(344,201,592,25,eyebrow,13,Mint,true,TextAnchor.MiddleCenter);
 Text(344,244,592,55,title,32,White,true,TextAnchor.MiddleCenter);
 }
 void Result(bool win){
 Overlay(win?"EXPEDICIÓN COMPLETADA":"EL GUARDIÁN RESISTE",win?"El secreto de la cueva":"Vuelve a intentarlo");
 Art(win?gold:miner,348,337,112,126);
 Text(486,330,439,124,win?"Has derrotado al Guardián Legendario. El cristal más valioso de la cueva revela la energía de una civilización olvidada.":"Tu vida llegó a cero. Conservas tus monedas y mejoras: vuelve al taller, prepara el pico y desafía al guardián otra vez.",19,Soft);
 Text(344,485,592,40,win?"Recompensa final: 250 monedas  ·  Total: "+state.coins+" monedas":"La vida se recuperará al regresar a la mina.",16,Gold,true,TextAnchor.MiddleCenter);
 if(Button(344,555,284,48,win?"NUEVA EXPEDICIÓN":"VOLVER A LA MINA",true,true)){if(win)NewGame();else{state.Retry();attackClock=0;Save();}}
 if(Button(648,555,284,48,"MENÚ PRINCIPAL")){Save();menu=true;}
 }
 void Help(){
 Overlay("GUÍA DEL EXPLORADOR","Todo comienza con un clic");
 Text(344,321,592,190,"1. Haz clic en el cristal: comienzas con 1 moneda por clic.\n2. Reúne 50 cristales acumulados y mejora la minería.\n3. Desafía al guardián de 20 PV: ganarás 75 monedas.\n4. Usa la cueva profunda para comprar mejoras.\n5. Vence al Guardián Legendario de 100 PV.\n\nLos guardianes atacan automáticamente. Solo necesitas el clic izquierdo; no hay movimiento por teclado.",17,Soft);
 if(Button(464,558,352,48,"ENTENDIDO",true,true))help=false;
 }
 void PauseDialog(){
 Overlay("EXPEDICIÓN EN PAUSA","Respira. La mina puede esperar.");
 Text(344,335,592,80,"El combate y los ataques están detenidos.\nTu progreso se guarda automáticamente.",19,Soft,false,TextAnchor.MiddleCenter);
 if(Button(344,448,284,48,"CONTINUAR",true,true))paused=false;
 if(Button(648,448,284,48,"MENÚ PRINCIPAL")){Save();paused=false;menu=true;}
 if(Button(464,527,352,44,"REINICIAR EXPEDICIÓN")){paused=false;resetConfirm=true;}
 }
 void ResetDialog(){
 Overlay("NUEVA EXPEDICIÓN","¿Empezar desde cero?");
 Text(344,338,592,90,"Se reemplazará el progreso guardado: monedas, mejoras y guardianes vencidos.",20,Soft,false,TextAnchor.MiddleCenter);
 if(Button(344,521,284,48,"CANCELAR"))resetConfirm=false;
 if(Button(648,521,284,48,"EMPEZAR DE NUEVO",true,true))NewGame();
 }
}}














