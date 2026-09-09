using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Jimothy {
public partial class GameSession : MonoBehaviour {
 public static GameSession Instance {get;private set;}
 public SaveData Data {get;private set;}
 public RaccoonMotor Player {get;private set;}
 public GameUI UI {get;private set;}
 public bool Playing {get;private set;}
 public bool Paused {get;private set;}
 public Vector3 Home => ClosingTimeWorld.Home;
 public bool DenOpen {get;private set;}
 public string Objective => Data==null?"":Data.nightGoal==0?"1 / 3 · Find a closing-time snack":Data.nightGoal==1?"2 / 3 · Follow amber steps to the rooftops":Data.nightGoal==2?"3 / 3 · Bring a rooftop keepsake to the den":"FIRST NIGHT COMPLETE · Explore, collect, decorate";
 public LootNode NearbyLoot {get;private set;}
 public string SearchHint => NearbyLoot ? "Search · "+NearbyLoot.DisplayName : "";
 public Dictionary<string,ItemDefinition> Items {get;private set;}
 public readonly List<NeighborAI> Neighbors=new();
 readonly List<LootNode> nodes=new();
 readonly Dictionary<Color,Material> propMaterials=new();
 ScavengeFeedback feedback; Camera gameplayCamera; int guidedRoute=-1,guidedStep;bool recoveringClimb;
 GameObject world,display; float autosave,invulnerable; bool hasSession;
 public bool AtHome => Player && ClosingTimeWorld.DenContains(Player.transform.position);
 public IReadOnlyList<string> DenDisplayedIds => denDisplayedIds;
 readonly List<string> denDisplayedIds=new();
 public bool ThreatNearby => Neighbors.Any(n=>n && (n.IsThreat||n.Suspicion01>.15f) && Vector3.Distance(n.transform.position,Player.transform.position)<12);
 void Awake() {
  Instance=this;MobileQuality.Apply();AudioListener.volume=PlayerPrefs.GetFloat("volume",1);QualitySettings.vSyncCount=0;
  Items=JsonUtility.FromJson<ItemCatalog>(Resources.Load<TextAsset>("Items").text).items.ToDictionary(i=>i.id);
  Items["item_000"].name="Last-call salmon scraps";Items["item_002"].name="Sesame bagel half";Items["item_006"].name="Everything bagel";Items["item_010"].name="Rye sandwich corner";
  feedback=gameObject.AddComponent<ScavengeFeedback>();
  UI=gameObject.AddComponent<GameUI>();UI.Build(this);gameObject.AddComponent<PlaytestStats>();
 }
 public void NewGame() { Begin(new SaveData()); Save(); }
 public void LoadGame() { if(SaveStore.TryLoad(out var data,out var message)){Begin(data);UI.Toast(message);}else UI.Toast(message); }
 void Begin(SaveData state) {
  if(world)Destroy(world);Neighbors.Clear();nodes.Clear();
  Data=state;Data.hiddenDecor??=new();Data.favoriteFinds??=new();Data.bankedDiscoveries??=new();foreach(var id in Data.pantry.Concat(Data.trophies).Distinct())if(!Data.bankedDiscoveries.Contains(id))Data.bankedDiscoveries.Add(id);guidedRoute=-1;guidedStep=0;recoveringClimb=false;
  Data.MigrateToClosingTime(ClosingTimeWorld.StreetStart);
  world=new GameObject("Ballard • closing time");
  ClosingTimeWorld.Build(world.transform);RooftopConnections.Build(world.transform);
  var playerObject=new GameObject("Jimothy");playerObject.transform.SetParent(world.transform);playerObject.layer=2;
  Player=playerObject.AddComponent<RaccoonMotor>();
  var model=Instantiate(Resources.Load<GameObject>("Jimothy"),playerObject.transform);Player.visual=model.transform;model.transform.localScale=Vector3.one*.22f;model.AddComponent<JimothyCoat>().Initialize();
  foreach(var t in model.GetComponentsInChildren<Transform>())t.gameObject.layer=2;
  Player.animator=model.GetComponentInChildren<Animator>();
  if(!Player.animator)Player.animator=model.AddComponent<Animator>();
  Player.animator.runtimeAnimatorController=Resources.Load<RuntimeAnimatorController>("JimothyMotion");Player.animator.applyRootMotion=false;
  var cameraObject=new GameObject("Player Camera");cameraObject.transform.SetParent(world.transform);
  var cam=cameraObject.AddComponent<Camera>();gameplayCamera=cam;cam.tag="MainCamera";cam.fieldOfView=62;cam.nearClipPlane=.15f;cam.farClipPlane=1000;cam.backgroundColor=new Color(.51f,.66f,.69f);cam.clearFlags=CameraClearFlags.SolidColor;cameraObject.AddComponent<AudioListener>();
  Player.Initialize(cam);Player.Teleport(new Vector3(state.x,state.y,state.z));Player.FaceDirection(ClosingTimeWorld.DenContains(Player.transform.position)?0:180);Player.RecenterCamera();
  ClosingTimeWorld.ConfigureAtmosphere(cam,world.transform);MobileQuality.ConfigureCamera(cam);world.AddComponent<ClosingSoundscape>().Initialize(Player);NeighborWorldObstacles.Ensure();SpawnLoot();SpawnNeighbors();RefreshDen();world.AddComponent<DenAtmosphere>().Initialize(Player,cam);
  hasSession=true;Playing=true;Paused=false;DenOpen=false;Player.Running=true;autosave=0;invulnerable=2;UI.ShowHUD();UI.Toast("Last call! Search the snack at your feet, then follow the amber rooftop route.");
 }
 public GameObject Primitive(string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color) {
  var o=GameObject.CreatePrimitive(type);o.name=name;o.transform.SetParent(world.transform);o.transform.position=position;o.transform.localScale=scale;
  Destroy(o.GetComponent<Collider>());if(!propMaterials.TryGetValue(color,out var m)){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=color;propMaterials.Add(color,m);}o.GetComponent<Renderer>().sharedMaterial=m;return o;
 }
 public Transform Piece(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color) {
  var t=Primitive(name,type,Vector3.zero,scale,color).transform;t.SetParent(parent,false);t.localPosition=pos;return t;
 }
 void SpawnLoot() {
  Physics.SyncTransforms();
  if(Data.forageSeed==0)Data.forageSeed=UnityEngine.Random.Range(1,int.MaxValue);
  int index=0;
  foreach(var spot in ClosingTimeWorld.Spots) {
   if(!Items.TryGetValue(spot.itemId,out var item))continue;
   // Small grounded packaging; all cues are contextual and never a road-wide collectible cloud.
   int nodeId=index++;var sites=ScavengeSites.ValidSites(spot);
   if(sites.Count==0){Debug.LogWarning("No validated alternate find site: "+spot.id);sites.Add(new ScavengeSites.Site(spot.position,spot.position,"Search nearby"));}
   var cd=Data.cooldowns.Find(c=>c.node==nodeId);
   if(spot.id=="welcome_snack"&&cd!=null)cd.readyAt=double.MaxValue;
   int harvest=cd?.harvests??0;int slot=ScavengeSites.Slot(Data.forageSeed,nodeId,harvest,sites.Count);
   var o=BuildFindVisual(spot.id,item,sites[slot].position);
   var n=o.AddComponent<LootNode>();n.Initialize(nodeId,item,spot.rooftop);n.ConfigureSites(sites,slot,spot.id=="welcome_snack");nodes.Add(n);
  }
 }
 GameObject BuildFindVisual(string name,ItemDefinition item,Vector3 position){
  // Preserve the interaction anchor; only the displayed mesh is scaled and grounded.
  var root=new GameObject(name);root.transform.SetParent(world.transform);root.transform.position=position;
  var visual=ItemVisuals.Create(item,root.transform,name);visual.transform.localScale=Vector3.one*(item.category=="trophy"?.75f:.55f);
  if(Physics.Raycast(position+Vector3.up*.25f,Vector3.down,out var ground,1.5f,1<<0,QueryTriggerInteraction.Ignore)){
   var bounds=visual.GetComponent<Renderer>().bounds;
   visual.transform.position+=Vector3.up*(ground.point.y+.008f-bounds.min.y);
  }
  root.AddComponent<FindVisualDistance>().renderers=root.GetComponentsInChildren<Renderer>();return root;
 }
 void SpawnNeighbors() {
  var kinds=new[]{NeighborKind.Dog,NeighborKind.Cat,NeighborKind.AngryHuman,NeighborKind.KindHuman,NeighborKind.Fisherman,NeighborKind.Gull,NeighborKind.Passerby,NeighborKind.UnhousedNeighbor,NeighborKind.ImpairedPasserby};
  for(int i=0;i<9;i++) {
   var o=new GameObject(kinds[i%kinds.Length].ToString());o.transform.SetParent(world.transform);o.transform.position=new Vector3(i%2==0?-7.2f:7.2f,.25f,-29+i*7.1f);
   var ai=o.AddComponent<NeighborAI>();ai.Initialize(kinds[i%kinds.Length]);Neighbors.Add(ai);
  }
 }
 public bool Collect(int node,ItemDefinition item) {
  if(!Playing || Paused || !Available(node))return false;
  if(item.category!="valuable" && Data.bag.Count>=24){UI.Toast("Pockets full. Eat something or visit your den.");return false;}
  if(item.category=="valuable")Data.coins+=item.value;else Data.bag.Add(item.id);
  var cd=Data.cooldowns.Find(c=>c.node==node);if(cd==null){cd=new NodeCooldown{node=node};Data.cooldowns.Add(cd);}
  cd.harvests++;
  bool welcome=nodes.Any(n=>n&&n.NodeId==node&&n.Welcome);
  cd.readyAt=item.category=="trophy"||welcome?double.MaxValue:Data.worldSeconds+ScavengeSites.RestockSeconds(Data.forageSeed,node,cd.harvests);
  feedback.Play(false);
  var reveal=GetComponent<DiscoveryReveal>();if(!reveal)reveal=gameObject.AddComponent<DiscoveryReveal>();reveal.Show(item);
  if(item.category=="food"&&Data.nightGoal==0){Data.nightGoal=1;UI.Toast("Snack found! Follow the amber crates and striped ledges up.");}
  else UI.Toast("Found "+item.name);Save();return true;
 }
 public bool Available(int node) {var cd=Data.cooldowns.Find(c=>c.node==node);return cd==null || Data.worldSeconds>=cd.readyAt;}
 public void Eat() {
  if(!Playing||(Paused&&!DenOpen))return;
  if(Data.hunger>=99.9f&&Data.health>=99.9f){UI.Toast("Already full. Save that snack for later.");return;}
  string id=Data.bag.Find(x=>Items.TryGetValue(x,out var item)&&item.category=="food");
  var source=Data.bag;
  if(id==null && AtHome){source=Data.pantry;id=source.Find(x=>Items.TryGetValue(x,out var item)&&item.category=="food");}
  if(id==null){UI.Toast("Search restaurant finds, or eat from your den pantry.");return;}
  source.Remove(id);Data.hunger=Mathf.Min(100,Data.hunger+Items[id].nutrition);Data.health=Mathf.Min(100,Data.health+6);UI.Toast("Delicious. Questionable, but delicious.");if(AtHome)RefreshDen();Save();
 }
 public void Deposit() {
  if(!Playing||(Paused&&!DenOpen)||!AtHome){UI.Toast("Unload your pockets at the den.");return;}
  int earned=0;bool firstNight=Data.nightGoal<3;
  foreach(string id in Data.bag) {
   if(!Items.TryGetValue(id,out var item))continue;
   if(!Data.bankedDiscoveries.Contains(id))Data.bankedDiscoveries.Add(id);
   if(item.category=="trophy"){if(!Data.trophies.Contains(id)){Data.trophies.Add(id);earned+=15;}if(Data.nightGoal==2)Data.nightGoal=3;}
   else {Data.pantry.Add(id);earned++;}
  }
  bool completed=firstNight&&Data.nightGoal==3;if(completed)earned+=20;
  Data.coins+=earned;Data.bag.Clear();RefreshDen();Save();if(earned>0)feedback.Play(true);
  UI.Toast(completed?$"First night complete! +{earned} shinies. Pick a cushion for your den.":$"Finds banked. +{earned} shinies for your den.");
 }
 public void BuyDecor(string id,int cost) {
  if(!Playing||(Paused&&!DenOpen)||!AtHome){UI.Toast("Decorate at your den.");return;}
  if(!DenFurnishings.Catalog.TryGetValue(id,out var upgrade)){UI.Toast("Unknown den furnishing.");return;}cost=upgrade.cost;
  if(Data.bankedDiscoveries.Count<upgrade.finds){UI.Toast("Bank "+upgrade.finds+" different finds to unlock this furnishing.");return;}
  if(Data.decor.Contains(id)){UI.Toast("Already in your den.");return;}
  if(Data.coins<cost){UI.Toast("Keep looking for shiny valuables.");return;}
  Data.coins-=cost;Data.decor.Add(id);RefreshDen();Save();
 }
 public void ToggleDecoration(string id){if(!Playing||!AtHome||!Data.decor.Contains(id))return;if(!Data.hiddenDecor.Remove(id))Data.hiddenDecor.Add(id);RefreshDen();Save();}
 public void ToggleFavorite(string id){if(!Playing||!AtHome||!Items.ContainsKey(id)||(!Data.pantry.Contains(id)&&!Data.trophies.Contains(id)))return;if(!Data.favoriteFinds.Remove(id)){if(Data.favoriteFinds.Count>=8){UI.Toast("Choose up to eight favorites. Unpin one to make room.");return;}Data.favoriteFinds.Add(id);}RefreshDen();Save();}
 void RefreshDen() {
  Data.favoriteFinds.RemoveAll(id=>!Data.pantry.Contains(id)&&!Data.trophies.Contains(id));
  if(display)Destroy(display);display=new GameObject("Persistent den collection");display.transform.SetParent(world.transform);
  denDisplayedIds.Clear();denDisplayedIds.AddRange(DenCollection.Select(Data,Items,ClosingTimeWorld.DenDisplaySlots.Count));
  DenCollection.Build(display.transform,denDisplayedIds,Items,Data.decor.Where(id=>!Data.hiddenDecor.Contains(id)).ToList());
 }

 public void FastTravel() {
  if(!Playing||Paused)return;
  if(!Player.Grounded||Player.IsMantling){UI.Toast("Land safely before fast travel.");return;}
  if(ThreatNearby){UI.Toast("Lose your pursuers before fast travel.");return;}
  Player.Teleport(Home);Player.FaceDirection(0);UI.Toast("Back in the den. Your collection is waiting.");Save();
 }
 public void Hurt(float damage) {
  if(!Playing||Paused||invulnerable>0||AtHome)return;
  Data.health=Mathf.Max(0,Data.health-damage);invulnerable=1.2f;
  if(Data.health<=0)EndRun();
 }
 public void Fell(){Player.Teleport(Home);Data.health=Mathf.Max(0,Data.health-15);if(Data.health<=0)EndRun();else UI.Toast("A rough landing. Back at the den.");}
 void EndRun() {
  Data.bestSeconds=Math.Max(Data.bestSeconds,(int)Data.survivalSeconds);Data.bag.Clear();Data.health=100;Data.hunger=100;Data.survivalSeconds=0;Player.Teleport(Home);Save();
  Playing=false;Player.Running=false;UI.ShowRunEnd(Data.bestSeconds);
 }
 public void OpenDen(){if(!AtHome)return;DenOpen=true;Paused=true;Player.Running=false;Save();}
 public void Search(){if(!Playing||Paused||!NearbyLoot)return;NearbyLoot.Search();}
 public void ReachedRoof(){if(Data.nightGoal==1){Data.nightGoal=2;feedback.Play(true);UI.Toast("Rooftops unlocked! Search for a keepsake, then take it to the den.");Save();}}
 public void Resume(){DenOpen=false;if(!hasSession)return;Playing=true;Paused=false;Player.Running=true;UI.ShowHUD();}
 public void TogglePause(){DenOpen=false;if(!hasSession||!Playing)return;Paused=!Paused;Player.Running=!Paused;if(Paused){Save();UI.ShowPause();}else UI.ShowHUD();}
 public void Menu(){DenOpen=false;if(hasSession)Save();Playing=false;Paused=false;if(Player)Player.Running=false;UI.ShowMenu();}
 public string Guidance {
  get {
   if(Data==null||!Player)return "";
   if(AtHome)return Data.nightGoal==2?"Open Stash → Unload pockets to finish":"Den safe · Stash to display finds · Ramp south to Ballard";
   Vector3 target=ClosingTimeWorld.DenEntrance;string label="Den entrance";
   if(Data.nightGoal==0){var snack=nodes.Where(n=>n.IsAvailable&&!n.IsRooftop).OrderBy(n=>(n.transform.position-Player.transform.position).sqrMagnitude).FirstOrDefault();if(snack){target=snack.transform.position;label=GameUI.MobileLayout?"Snack · Search":"Snack · F / Search";}}
   else if(Data.nightGoal==1||recoveringClimb){
    if((guidedRoute<0||Vector3.Distance(Player.transform.position,ClosingTimeWorld.RouteStarts[guidedRoute])>18)&&ClosingTimeWorld.RouteStarts.Count>0){float best=float.MaxValue;for(int i=0;i<ClosingTimeWorld.RouteStarts.Count;i++){float d=(ClosingTimeWorld.RouteStarts[i]-Player.transform.position).sqrMagnitude;if(d<best){best=d;guidedRoute=i;guidedStep=0;}}}
    if(guidedRoute>=0&&guidedRoute<ClosingTimeWorld.RoofRoutes.Count){var route=ClosingTimeWorld.RoofRoutes[guidedRoute];guidedStep=StableLandingStep(Player.transform.position,Player.Grounded,route,guidedStep);target=route[guidedStep];label=$"{(recoveringClimb?"Back to amber landing":"Amber landing")} {guidedStep+1}/{route.Length} · {(guidedStep==0?"Jump":"Keep moving")}";}

   }
   else if(Data.nightGoal==2&&!Data.bag.Any(id=>Items.TryGetValue(id,out var item)&&item.category=="trophy")){
    var keepsake=nodes.Where(n=>n.IsAvailable&&n.IsRooftop).OrderBy(n=>(n.transform.position-Player.transform.position).sqrMagnitude).FirstOrDefault();if(keepsake){target=keepsake.transform.position;label="Rooftop keepsake · Search";}
   } else if(AtHome)return Data.nightGoal==2?"Open Stash → Unload pockets to finish":"Den safe · Open Stash to bank or decorate";
   Vector3 delta=target-Player.transform.position;float distance=delta.magnitude;Vector3 forward=gameplayCamera?gameplayCamera.transform.forward:Player.transform.forward;forward.y=0;var flat=delta;flat.y=0;float angle=Vector3.SignedAngle(forward,flat,Vector3.up);string direction=Mathf.Abs(angle)>135?"Behind":angle>35?"Right":angle< -35?"Left":"Ahead";
   return $"{label}   ·   {distance:0} m {direction}";
  }
 }
 public static int StableLandingStep(Vector3 position,bool grounded,Vector3[] route,int current){
  if(route==null||route.Length==0)return 0;current=Mathf.Clamp(current,0,route.Length-1);if(!grounded)return current;
  int landed=-1;for(int i=0;i<route.Length;i++){var delta=position-route[i];float height=Mathf.Abs(delta.y);delta.y=0;if(height<.25f&&delta.magnitude<.8f)landed=i;}
  if(landed>=0)return Mathf.Min(landed+1,route.Length-1);
  return position.y<route[current].y-1.4f?0:current;
 }
 static readonly float[] RoofHeights={7.2f,8.1f,9f,7.2f,8.1f};
 public static bool OnRoofFootprint(Vector3 position){
  float x=Mathf.Abs(position.x);if(x<9.55f||x>19.65f)return false;
  for(int row=0;row<5;row++)if(Mathf.Abs(position.z-(-30+row*16))<5.95f&&position.y>=RoofHeights[row]+.12f)return true;
  return false;
 }
 public bool SuppressSaving {get;set;}
 public void Save(){if(!hasSession||SuppressSaving)return;var p=Player.transform.position;Data.x=p.x;Data.y=p.y;Data.z=p.z;if(!SaveStore.Write(Data,out var message))UI.Toast(message);}
 void Update() {
  if(!Playing||Paused)return;
  NearbyLoot=null;float nearest=2.4f;
  foreach(var node in nodes){if(!node||!node.IsAvailable)continue;float d=Vector3.Distance(Player.transform.position+Vector3.up*.5f,node.transform.position);if(d<nearest&&d<node.SearchRadius&&!Physics.Linecast(Player.transform.position+Vector3.up*.7f,node.transform.position+Vector3.up*.2f,1<<0,QueryTriggerInteraction.Ignore)){nearest=d;NearbyLoot=node;}}
  if(Keyboard.current?.fKey.wasPressedThisFrame==true || Gamepad.current?.buttonWest.wasPressedThisFrame==true)Search();
  bool safelyOnRoof=Player.Grounded&&OnRoofFootprint(Player.transform.position);
  if(Data.nightGoal==1&&safelyOnRoof)ReachedRoof();
  if(Data.nightGoal==2&&!Data.bag.Any(id=>Items.TryGetValue(id,out var item)&&item.category=="trophy")){
   if(safelyOnRoof)recoveringClimb=false;else if(Player.Grounded&&Player.transform.position.y<6.8f)recoveringClimb=true;
  }else recoveringClimb=false;
  float dt=Time.deltaTime;Data.worldSeconds+=dt;Data.survivalSeconds+=dt;Data.hunger=Mathf.Max(0,Data.hunger-dt*(AtHome?0:.07f));invulnerable-=dt;
  if(Data.hunger<=0&&!AtHome) {Data.health=Mathf.Max(0,Data.health-dt*2);if(Data.health<=0){EndRun();return;}}
  autosave+=dt;if(autosave>20){autosave=0;Save();}

 }
 void OnApplicationPause(bool paused){if(paused){Save();if(Playing&&!Paused)TogglePause();}}
 void OnApplicationQuit(){Save();}
 void OnDestroy(){foreach(var m in propMaterials.Values)Destroy(m);Instance=null;}
}
public class LootNode:MonoBehaviour {
 int node; ItemDefinition item; Renderer[] meshes; TextMesh cue; bool rooftop;
 List<ScavengeSites.Site> sites;int slot;bool waitingRestock;
 public Vector3 SearchApproach=>sites!=null&&sites.Count>0?sites[slot].approach:transform.position;
 public int NodeId=>node;public bool Welcome{get;private set;}
 public float SearchRadius=>Welcome?2.4f:rooftop?1.55f:1.3f;
 public void ConfigureSites(List<ScavengeSites.Site> options,int initialSlot,bool welcome){sites=options;slot=initialSlot;Welcome=welcome;var cooldown=GameSession.Instance.Data.cooldowns.Find(c=>c.node==node);waitingRestock=!GameSession.Instance.Available(node)||(cooldown!=null&&cooldown.stockedHarvests<cooldown.harvests);foreach(var mesh in meshes)mesh.enabled=!waitingRestock;}
 void RefreshSite(GameSession g){var cd=g.Data.cooldowns.Find(c=>c.node==node);int next=ScavengeSites.Slot(g.Data.forageSeed,node,cd?.harvests??0,sites.Count);if(next!=slot){transform.position=sites[next].position;slot=next;}}

 public bool IsRooftop => rooftop;
 public string DisplayName => item.name;
 public bool IsAvailable => !waitingRestock&&GameSession.Instance.Available(node);
 public void Initialize(int index,ItemDefinition definition,bool roof){node=index;item=definition;rooftop=roof;meshes=GetComponentsInChildren<Renderer>();
  var go=new GameObject("Nearby search cue");go.transform.SetParent(transform,false);go.transform.localPosition=new Vector3(0,.62f,0);cue=go.AddComponent<TextMesh>();cue.text="SEARCH";cue.fontSize=36;cue.characterSize=.013f;cue.anchor=TextAnchor.MiddleCenter;cue.color=new Color(1,.83f,.48f);cue.gameObject.SetActive(false);
 }
 public void Search(){var g=GameSession.Instance;if(IsAvailable&&g.Collect(node,item)){waitingRestock=true;foreach(var mesh in meshes)mesh.enabled=false;cue.gameObject.SetActive(false);if(rooftop)g.ReachedRoof();}}
 void Update(){var g=GameSession.Instance;if(!g||!g.Playing||g.Paused){if(cue)cue.gameObject.SetActive(false);return;}
  if(waitingRestock&&g.Available(node)&&sites!=null){
   // Never materialize under the player's nose. Replenish only once they leave this little area.
   var cd=g.Data.cooldowns.Find(c=>c.node==node);int next=ScavengeSites.Slot(g.Data.forageSeed,node,cd?.harvests??0,sites.Count);
   var playerPosition=g.Player.transform.position;
   if((playerPosition-transform.position).sqrMagnitude>196f&&(playerPosition-sites[next].position).sqrMagnitude>196f){RefreshSite(g);if(cd!=null)cd.stockedHarvests=cd.harvests;waitingRestock=false;}
  }
  bool available=IsAvailable;foreach(var mesh in meshes)mesh.enabled=available;bool close=available&&g.NearbyLoot==this;cue.gameObject.SetActive(close);
  if(close&&Camera.main){float distance=Vector3.Distance(cue.transform.position,Camera.main.transform.position);cue.transform.localScale=Vector3.one*Mathf.Clamp(distance/5f,.25f,1);cue.transform.rotation=Quaternion.LookRotation(cue.transform.position-Camera.main.transform.position);}
 }
}
}
