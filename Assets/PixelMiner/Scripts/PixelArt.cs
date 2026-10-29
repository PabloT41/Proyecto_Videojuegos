using UnityEngine;
namespace PixelMiner {
// Pixel art rasterized on an integer grid. All textures use Point filtering.
public static class PixelArt {
 static Texture2D t;
 static int armX,armY,pivotX,pivotY,spriteY;
 static double armAngle;
 static Color C(string s){ColorUtility.TryParseHtmlString("#"+s,out var c);return c;}
 static void R(int x,int y,int w,int h,string c){
 Color color=C(c);
 for(int j=y;j<y+h;j++)for(int i=x;i<x+w;i++){
 int xx=i,yy=j;
 if(armAngle!=0){
 double ca=System.Math.Cos(armAngle),sa=System.Math.Sin(armAngle);
 xx=pivotX+(int)System.Math.Round((i-pivotX)*ca-(j-pivotY)*sa);
 yy=pivotY+(int)System.Math.Round((i-pivotX)*sa+(j-pivotY)*ca);
 }
 xx+=armX;yy+=armY+spriteY;
 int fill=armAngle==0?1:2;
 for(int a=0;a<fill;a++)for(int b=0;b<fill;b++)if(xx+a>=0&&yy+b>=0&&xx+a<t.width&&yy+b<t.height)t.SetPixel(xx+a,t.height-yy-b-1,color);
 }}
 static void ArmPose(bool right,int pose){
 armX=armY=0;armAngle=0;
 if(pose==0)return;
 pivotX=right?100:28;pivotY=49;
 int angle=pose==1?(right?-105:105):pose==2?(right?-155:155):pose==3?(right?95:40):(right?-15:15);
 armAngle=angle*System.Math.PI/180;armY=pose==2?-22:0;
 }
 static void New(int w,int h){spriteY=armX=armY=0;armAngle=0;t=new Texture2D(w,h,TextureFormat.RGBA32,false);t.filterMode=FilterMode.Point;t.SetPixels(new Color[w*h]);}
 static Texture2D End(){t.Apply();return t;}
 static void E(int x,int y,int rx,int ry,string c){for(int j=-ry;j<=ry;j++)for(int i=-rx;i<=rx;i++)if((float)i*i/(rx*rx)+(float)j*j/(ry*ry)<=1)R(x+i,y+j,1,1,c);}
 static void L(int x,int y,int xx,int yy,int width,string c){int n=System.Math.Max(System.Math.Abs(xx-x),System.Math.Abs(yy-y));for(int k=0;k<=n;k++){float a=n==0?0:(float)k/n;R((int)System.Math.Round(x+(xx-x)*a)-width/2,(int)System.Math.Round(y+(yy-y)*a)-width/2,width,width,c);}}
 static void P(string c,params int[] v){int min=t.height,max=0;for(int i=1;i<v.Length;i+=2){min=System.Math.Min(min,v[i]);max=System.Math.Max(max,v[i]);}for(int y=min;y<=max;y++)for(int x=0;x<t.width;x++){bool inside=false;int j=v.Length-2;for(int i=0;i<v.Length;i+=2){if((v[i+1]>y)!=(v[j+1]>y)&&x<(float)(v[j]-v[i])*(y-v[i+1])/(v[j+1]-v[i+1])+v[i])inside=!inside;j=i;}if(inside)R(x,y,1,1,c);}}
 static void Gem(int x,int y,int s,bool gold=false){
 string d=gold?"946342":"235075",m=gold?"e6aa58":"46bbd4",l=gold?"ffe49a":"8df9ed",hi=gold?"fff5cd":"defff5";
 P("15283e",x,y-s-2,x+s/2+2,y-s/3,x+s/2,y+s,x,y+s+s/4,x-s/2,y+s,x-s/2-2,y-s/3);
 P(m,x,y-s,x+s/2,y-s/3,x+s/2-1,y+s-2,x,y+s+s/4-2,x-s/2+1,y+s-2,x-s/2,y-s/3);
 P(l,x,y-s,x+s/2,y-s/3,x,y,x-s/2,y-s/3);
 P(d,x-s/2,y-s/3,x,y,x,y+s+s/4-2,x-s/2+1,y+s-2);
 P(gold?"f3c86d":"66e4df",x,y,x+s/2,y-s/3,x+s/2-1,y+s-2,x,y+s+s/4-2);
 L(x,y-s+2,x,y+s+s/4-4,1,hi);L(x-s/2+1,y-s/3,x,y,1,hi);L(x,y,x+s/2-1,y-s/3,1,hi);
 }
 public static Texture2D Crystal(bool gold=false){New(64,72);E(32,66,24,4,"17243b");Gem(32,32,24,gold);return End();}
 public static Texture2D Miner(){return MinerFrame(-1);}
 // Six discrete poses: anticipation, lift, swing, contact, follow-through, recovery.
 public static Texture2D MinerFrame(int frame,int breath=0){
 New(96,96);int lean=frame==3||frame==4?3:0,dy=frame==3?2:0;
 E(31,89,25,4,"111b2e");
 // Boots, denim, leather belt, gloves and a canvas backpack.
 R(18+lean,65,12,20,"172337");R(34+lean,65,12,20,"172337");
 R(20+lean,67,8,14,"3b657a");R(35+lean,67,8,14,"315469");R(22+lean,69,3,11,"5c8b98");
 R(15+lean,82,16,6,"141b2b");R(33+lean,82,17,6,"141b2b");R(16+lean,82,12,2,"865b46");R(35+lean,82,12,2,"865b46");
 dy+=breath;
 R(9+lean,42+dy,11,23,"222638");R(10+lean,44+dy,7,17,"856244");R(10+lean,48+dy,4,8,"b59661");R(8+lean,49+dy,3,13,"3a4444");
 P("172337",18+lean,40+dy,38+lean,38+dy,47+lean,45+dy,45+lean,67+dy,17+lean,67+dy,13+lean,48+dy);
 R(18+lean,42+dy,23,20,"b36a43");R(20+lean,43+dy,5,19,"e19b5a");R(27+lean,43+dy,12,21,"365f72");
 R(26+lean,44+dy,4,10,"7299a0");R(36+lean,44+dy,3,10,"7299a0");R(25+lean,55+dy,17,10,"477e8d");R(30+lean,57+dy,6,5,"2c4f68");
 R(17+lean,64+dy,28,4,"593d37");R(30+lean,64+dy,5,4,"e5be70");
 // Head in three-quarter view, nose, moustache, beard and helmet lamp.
 R(20+lean,19+dy,23,23,"172337");R(22+lean,22+dy,19,17,"e6ae75");R(24+lean,23+dy,8,9,"ffd49b");
 R(20+lean,25+dy,4,13,"744333");R(39+lean,29+dy,6,6,"e6ae75");R(38+lean,26+dy,3,4,"172337");R(39+lean,26+dy,1,1,"fff4ce");
 R(27+lean,35+dy,15,7,"684131");R(29+lean,34+dy,12,3,"8f5134");R(30+lean,39+dy,8,2,"b77543");R(22+lean,29+dy,3,5,"d28e60");
 P("714c30",15+lean,20+dy,18+lean,12+dy,23+lean,8+dy,36+lean,8+dy,42+lean,13+dy,45+lean,23+dy,15+lean,23+dy);
 R(19+lean,13+dy,22,8,"d99432");R(23+lean,10+dy,13,9,"f2bb4e");R(25+lean,10+dy,3,8,"ffe58a");R(14+lean,20+dy,33,4,"ecb04b");R(15+lean,23+dy,31,2,"8c5c32");
 R(37+lean,14+dy,8,8,"29384b");R(39+lean,15+dy,5,5,"fff0a8");R(40+lean,16+dy,3,3,"fffbe3");
 // Far hand.
 L(19+lean,46+dy,16+lean,57+dy,6,"d29560");R(14+lean,56+dy,7,7,"8d613e");R(14+lean,56+dy,6,3,"dfb16e");
 int[] hx={47,42,45,52,54,49},hy={52,45,41,53,57,53};
 int[] tx={63,38,57,85,80,67},ty={27,18,17,47,64,35};
 int handx=frame<0?48:hx[frame],handy=frame<0?53:hy[frame];
 int tipx=frame<0?67:tx[frame],tipy=frame<0?32:ty[frame];
 L(42+lean,45+dy,handx,handy,8,"172337");L(42+lean,45+dy,handx,handy,5,"cc8c54");
 int bx=handx-(tipx-handx)/3,by=handy-(tipy-handy)/3;
 L(bx,by,tipx,tipy,5,"1a2332");L(bx,by,tipx,tipy,3,"a86e43");L(bx-1,by,tipx-1,tipy,1,"ddb77b");
 int vx=tipx-handx,vy=tipy-handy;float len=(float)System.Math.Sqrt(vx*vx+vy*vy);int px=(int)(-vy/len*14),py=(int)(vx/len*14);
 L(tipx-px,tipy-py,tipx+px,tipy+py,6,"172337");L(tipx-px,tipy-py,tipx+px,tipy+py,4,"83aebb");L(tipx-px,tipy-py-1,tipx+px,tipy+py-1,1,"d9f0e8");
 // Plain steel pick head: no hanging hook or drop.
 R(handx-3,handy-2,7,6,"e0b26f");R(handx-2,handy-2,5,2,"ffe0a0");
 if(frame==2){L(65,19,75,25,1,"8cacc0");L(70,16,81,23,1,"526b8e");}
 if(frame==3){L(78,40,91,39,2,"e7fff5");L(86,34,89,46,2,"a0f8ed");}
 return End();}
 public static Texture2D MinerBreathing(int phase){return MinerFrame(-1,phase==1?-1:0);}
 public static Texture2D Guardian(bool boss){return GolemBoss(boss);}
 // Broad, asymmetric rock plates rather than a humanoid suit of armour.
 static void Slab(string color,params int[] points){
 P(color,points);
 for(int i=0;i<points.Length;i+=2){int j=(i+2)%points.Length;L(points[i],points[i+1],points[j],points[j+1],2,"282733");}
 L(points[0]+2,points[1]+2,points[2]-1,points[3]+2,1,"b0ac94");
 }
 public static Texture2D GolemBoss(bool legendary=true,int pose=0){
 New(128,144);spriteY=24;
 E(66,105,49,5,"151c31");
 // Short planted legs and dark stone joints.
 P("302b35",32,73,48,77,48,104,42,108,24,108,23,98);
 P("393039",82,76,101,75,107,97,102,108,84,108,79,97);
 Slab("615b58",26,88,36,82,45,86,45,105,27,106,24,98);
 P("817d6b",27,88,35,85,38,102,29,103);L(35,91,37,102,1,"42404a");
 Slab("625c57",85,87,98,83,104,94,101,106,85,106,82,98);
 P("827c69",87,88,97,87,99,103,88,104);L(98,94,99,102,1,"44404a");
 // Heavy body silhouette and shoulders.
 P("292735",29,36,38,20,55,13,74,11,94,24,102,39,103,76,91,96,69,99,43,94,28,83,25,51);
 GolemArms(pose);
 // Jagged upper arch, including the raised central peak.
 Slab("898976",29,36,28,23,40,16,55,18,52,32,42,40);
 P("a7a48d",32,25,40,19,48,20,42,29);
 Slab("9a9980",44,18,45,9,56,5,68,9,64,23,53,26);
 P("b4b095",47,11,55,8,61,10,54,15);
 Slab("aaa78c",64,11,70,2,78,0,85,13,82,23,68,22);
 P("c2bba0",71,5,76,2,77,16,68,16);
 Slab("7b7c6b",80,9,91,13,96,27,84,34,76,26,82,20);
 P("a4a18a",84,13,89,16,91,25,84,23);
 Slab("aaa78c",56,26,69,22,80,25,79,40,59,39,51,34);
 P("c0b89b",58,27,67,25,73,27,60,29);
 Slab("828170",81,30,96,26,103,38,98,51,85,48,78,40);
 P("a6a18a",86,33,94,29,99,37,91,40);
 Slab("93927d",35,39,50,30,57,39,53,52,38,56,29,49);
 P("b3ad91",36,42,47,36,48,43,38,48);
 // Purple recessed face, with magenta lights inset deep in rock.
 P("282133",44,49,55,43,77,44,88,51,86,64,77,70,52,68,40,61);
 P(legendary?"54355f":"31535b",45,51,57,47,76,48,84,52,82,62,75,65,52,64,44,59);
 P(legendary?"755083":"51807c",48,51,58,48,74,49,80,52,73,54,51,54);
 L(52,57,76,57,1,"2c2037");
 E(53,56,4,4,"261c30");E(76,56,4,4,"261c30");
 E(53,56,2,3,legendary?"f55ac9":"6fe9dd");E(76,56,2,3,legendary?"f55ac9":"6fe9dd");
 R(52,54,2,2,legendary?"ffd3f5":"d5fff1");R(75,54,2,2,legendary?"ffd3f5":"d5fff1");
 // Chin and overlapping abdominal boulders.
 Slab("898570",34,62,46,66,54,72,45,81,32,76,25,68);
 Slab("9c9780",53,69,65,67,80,70,89,80,74,85,49,80);
 P("b5ad92",55,71,66,70,75,73,57,74);
 Slab("7b786a",87,58,99,54,104,65,98,78,86,81,79,71);
 P("a09b82",91,61,97,58,99,65,94,70,88,68);
 L(91,65,96,62,1,"4b4a4c");L(96,62,97,67,1,"4b4a4c");L(97,67,92,70,1,"4b4a4c");
 Slab("66615c",44,82,58,83,75,86,87,81,94,87,81,96,60,98,43,93,35,85);
 P("837c6b",48,85,62,86,75,90,58,91);
 // Small cracks and lichen, kept aligned to the pixel grid.
 L(69,30,72,32,1,"555449");L(72,32,70,36,1,"555449");
 L(37,28,39,31,1,"555449");R(42,24,3,1,"555449");
 L(18,65,22,69,1,"565249");L(22,69,21,73,1,"565249");
 L(112,66,115,73,1,"625e50");R(64,77,3,1,"625e50");
 R(34,36,3,2,"627865");R(89,25,3,1,"627865");R(98,73,2,2,"627865");
 if(pose==3)GolemArms(pose);
 return End();
 }
 static void GolemArms(int pose){
 ArmPose(false,pose);
 Slab("747567",12,44,27,38,37,48,31,66,10,65,3,58);
 P("9d9d83",13,45,24,42,30,48,21,57,9,57);
 Slab("8c8974",4,59,22,59,32,66,27,82,7,80,1,72);
 P("b0ab8b",5,62,12,62,12,73,4,72);
 Slab("4b454a",4,74,14,78,27,77,26,85,6,83,1,79);
 L(12,78,11,83,1,"272631");L(20,79,19,84,1,"272631");
 ArmPose(true,pose);
 Slab("777567",99,41,112,43,123,55,127,70,118,77,102,70,96,55);
 P("a4a087",105,44,112,46,119,58,108,54);
 Slab("8b8875",105,56,122,61,126,76,116,86,101,81,98,67);
 P("a8a285",108,60,117,64,120,76,108,78);
 Slab("4b454c",102,79,116,83,124,76,122,85,113,90,100,85);
 armAngle=0;armX=armY=0;
 }
 public static Texture2D GuardianAttack(bool legendary,int pose){return GolemBoss(legendary,pose);}
 public static Texture2D SlamArc(int frame){
 New(144,128);
 // A diagonal crescent, rasterized in the same pixel grid as the character.
 int shift=frame*5;
 P("285d68",19,4+shift,46,11+shift,78,33+shift,108,64+shift,125,96+shift,105,119,82,91,59,60,37,30+shift);
 P("eaffdc",23,4+shift,48,16+shift,79,40+shift,105,70+shift,120,98+shift,104,114,84,85,60,55,38,27+shift);
 P("89e6cf",37,17+shift,61,35+shift,87,61+shift,104,88+shift,104,103,87,81,66,52,45,29+shift);
 P("ffffff",23,4+shift,33,10+shift,59,34+shift,78,58+shift,64,49+shift,41,25+shift);
 // Mirror the crescent: its origin is above the golem and its tip points down-left.
 Color[] original=t.GetPixels(),mirrored=new Color[t.width*t.height];
 for(int y=0;y<t.height;y++)for(int x=0;x<t.width;x++)mirrored[y*t.width+x]=original[y*t.width+t.width-1-x];
 t.SetPixels(mirrored);
 return End();
 }
 public static Texture2D AttackRock(){
 New(32,32);
 Slab("8c8875",4,12,12,3,23,5,29,15,25,26,13,29,3,22);
 P("b8ae90",6,12,13,6,21,7,16,14);
 P("5b5957",16,14,27,14,23,24,14,26);
 L(13,8,16,14,1,"656057");L(16,14,13,21,1,"403e48");
 return End();
 }
 public static Texture2D AttackFist(){
 New(40,32);
 Slab("777567",2,10,8,5,21,5,29,11,38,14,37,25,27,29,9,27,1,20);
 P("aaa087",5,11,10,7,19,8,23,14,7,16);
 L(10,17,11,24,2,"45434c");L(19,17,20,26,2,"45434c");L(28,17,29,25,2,"45434c");
 L(7,8,17,7,1,"c1b698");
 return End();
 }
 static void Rock(int x,int y,int s){
 P("141c33",x-s,y,x-s/2,y-s,x+s/3,y-s-2,x+s,y-2,x+s-2,y+s/2,x-s/2,y+s/2);
 P("384259",x-s+2,y-1,x-s/2,y-s+2,x+s/3,y-s,x+s-2,y-2,x,y+2);
 P("242e48",x,y+2,x+s-2,y-2,x+s-2,y+s/2-2,x-s/2,y+s/2-2);
 L(x-s/2,y-s+2,x+s/3,y-s,1,"586078");L(x,y+2,x-s/2,y-s+2,1,"45506a");
 }
 static void Plant(int x,int y,int size){
 L(x,y,x-2,y-size,2,"1c5d63");
 for(int k=0;k<3;k++){int yy=y-k*size/3;P("2a7778",x,yy,x-size/2,yy-size/3,x-size/2+2,yy-2,x,yy+1);P("194950",x,yy,x+size/2,yy-size/3-2,x+size/2-1,yy-1,x,yy+2);}
 }
 public static Texture2D Cave(bool deep){
 New(320,192);R(0,0,320,192,deep?"17172c":"20283d");
 // Curved chamber, masonry, cracks and overhanging stone.
 E(165,137,190,104,deep?"353044":"4a4654");E(168,151,166,80,deep?"48404d":"615960");
 var rng=new System.Random(71);
 for(int row=0;row<4;row++)for(int col=0;col<10;col++){
 int x=col*39-(row%2)*19,y=row*18-8;
 P("171e32",x,y+2,x+36,y,x+38,y+16,x+3,y+19);
 P(row%2==0?"30394f":"354057",x+2,y+4,x+34,y+2,x+35,y+14,x+5,y+16);
 L(x+5,y+5,x+31,y+3,1,"49516b");L(x+8,y+8,x+13,y+13,1,"252d45");
 }
 // Side buttresses and dark recesses.
 for(int side=0;side<2;side++){int x=side==0?6:293;for(int k=0;k<5;k++){P("111c30",x-8,k*19,x+18,k*19-2,x+23,k*19+17,x-4,k*19+22);P("37455b",x-5,k*19+2,x+14,k*19,x+18,k*19+14,x,k*19+17);L(x-3,k*19+3,x+12,k*19+1,1,"506079");}}
 for(int i=0;i<150;i++){int x=rng.Next(25,296),y=rng.Next(79,188);R(x,y,rng.Next(1,4),1,i%3==0?"706574":"514c5c");}
 // Stone ring and raised octagonal altar centered under the interactive gem.
 E(178,144,75,33,"29263f");E(178,141,72,31,"50455f");E(178,139,65,27,"65556e");E(178,139,53,22,"3e3454");
 for(int i=0;i<12;i++){double a=i*System.Math.PI/6;L(178+(int)(System.Math.Cos(a)*54),139+(int)(System.Math.Sin(a)*23),178+(int)(System.Math.Cos(a)*70),140+(int)(System.Math.Sin(a)*30),2,"302d46");}
 E(178,139,47,20,"745386");E(178,138,44,18,"ae77b6");E(178,138,41,16,"4a3c62");
 P("27263e",143,130,156,119,194,118,211,130,210,145,194,155,158,155,143,145);
 P("514564",145,131,159,121,193,120,209,131,193,143,159,143);
 P("75627f",145,130,159,119,194,119,210,130,193,140,160,140);
 L(160,142,160,152,1,"998099");L(193,142,193,152,1,"39324e");L(147,143,158,152,1,"73607c");
 for(int i=0;i<24;i++){double a=i*System.Math.PI/12;int x=178+(int)(System.Math.Cos(a)*47),y=138+(int)(System.Math.Sin(a)*19);R(x,y,2,1,"e0a3ef");if(i%3==0)R(x+1,y-3,1,1,"f2d3ff");}
 // Satellite crystals, warm candles, moss and scattered stones.
 int[] gx={72,254,51,278},gy={83,101,159,164};
 for(int i=0;i<4;i++){E(gx[i],gy[i]+7,7,3,"292c43");Gem(gx[i],gy[i],i%2==0?6:5);}
 int[] cx={102,236,72,269,145},cy={99,83,126,133,170};
 for(int i=0;i<cx.Length;i++){E(cx[i],cy[i]+3,5,2,"413748");R(cx[i]-1,cy[i]-4,3,7,"b7865a");R(cx[i],cy[i]-7,2,5,"f8c075");R(cx[i],cy[i]-6,1,2,"fff1b6");}
 for(int i=0;i<20;i++){int x=rng.Next(28,294),y=rng.Next(84,185);if(x>105&&x<251&&y>104&&y<169)continue;Rock(x,y,rng.Next(2,5));}
 Plant(35,77,17);Plant(284,77,13);Plant(71,43,12);Plant(82,47,9);
 Rock(12,174,26);Rock(309,175,29);Rock(28,185,23);Rock(282,191,24);
 Plant(18,177,25);Plant(300,182,26);Plant(54,191,14);Plant(270,187,12);
 // Hanging vines in the upper corners.
 for(int i=0;i<5;i++){int x=9+i*14;L(x,0,x-3,20+i%3*8,2,"12353e");Plant(x-3,20+i%3*8,10);}
 return End();}
}}








