using System;
namespace PixelMiner {
[Serializable] public sealed class MineState {
 public int coins,mined,miningLevel,damageLevel,stage,hp=100,enemyHp;
 public bool fighting,defeated,won;
 public int Mining=>1+miningLevel;
 public int Damage=>1+damageLevel*2;
 public int MiningCost=>25+miningLevel*25;
 public int DamageCost=>20+damageLevel*20;
 public int EnemyMax=>stage==1?20:100;
 public int EnemyDamage=>stage==1?8:12;
 public int Reward=>stage==1?75:250;
 public float AttackInterval=>stage==1?2.5f:2f;
 public bool Ready=>stage==0?mined>=50&&miningLevel>0:stage==2;
 public int Mine(){if(fighting||defeated||won||(stage!=0&&stage!=2))return 0;coins+=Mining;mined+=Mining;return Mining;}
 public bool Buy(bool mining){if(fighting||defeated||won)return false;int cost=mining?MiningCost:DamageCost;if(coins<cost)return false;coins-=cost;if(mining)miningLevel++;else damageLevel++;return true;}
 public bool EnterBattle(){if(!Ready||fighting||defeated||won)return false;stage=stage==0?1:3;hp=100;enemyHp=EnemyMax;fighting=true;return true;}
 public int Hit(){if(!fighting)return 0;int dealt=Math.Min(Damage,enemyHp);enemyHp-=dealt;if(enemyHp==0){coins+=Reward;fighting=false;if(stage==1){stage=2;hp=100;}else won=true;}return dealt;}
 public void EnemyAttack(){if(!fighting)return;hp=Math.Max(0,hp-EnemyDamage);if(hp==0){defeated=true;fighting=false;}}
 public void Retry(){if(!defeated)return;stage=stage==1?0:2;defeated=false;fighting=false;hp=100;enemyHp=0;}
}}
