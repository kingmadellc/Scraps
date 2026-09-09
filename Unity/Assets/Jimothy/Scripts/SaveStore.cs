using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;

namespace Jimothy {
[Serializable] public class NodeCooldown { public int node; public double readyAt; public int harvests, stockedHarvests; }
[Serializable] public class SaveData {
 public int version = 1;
 public int mapRevision, nightGoal;
 public int forageSeed;
 public float x = -4, y = 1.1f, z = 41;
 public float health = 100, hunger = 100;
 public int coins, bestSeconds;
 public int trickScore, bestLandingScore, trickRewardMilestones;
 public double worldSeconds, survivalSeconds;
 public long savedUtc;
 public List<string> bag = new();
 public List<string> pantry = new();
 public List<string> trophies = new();
 public List<string> decor = new();
 public List<string> hiddenDecor = new();
 public List<string> favoriteFinds = new();
 public List<string> bankedDiscoveries = new();
 public List<NodeCooldown> cooldowns = new();
 public void MigrateToClosingTime(Vector3 home) {
  if(mapRevision>=3)return;
  mapRevision=3;x=home.x;y=home.y;z=home.z;cooldowns.Clear();
 }
 public void ApplyOffline(long now) {
  // Returning players never die off-screen. Only world resources advance.
  if (savedUtc > 0) worldSeconds += Math.Clamp(now - savedUtc, 0L, 28800L);
  savedUtc = now;
 }
 public bool Valid() => version == 1 && float.IsFinite(x) && float.IsFinite(y) && float.IsFinite(z)
  && Math.Abs(x) < 1000 && Math.Abs(y) < 1000 && Math.Abs(z) < 1000
  && float.IsFinite(health) && health >= 0 && health <= 100
  && float.IsFinite(hunger) && hunger >= 0 && hunger <= 100 && coins >= 0
  && double.IsFinite(worldSeconds) && worldSeconds >= 0
  && double.IsFinite(survivalSeconds) && survivalSeconds >= 0
  && bag != null && pantry != null && trophies != null && decor != null && cooldowns != null;
}
[Serializable] class SaveEnvelope { public int version=1; public string payload, sha256; }
public static class SaveStore {
 public static string PathName => Path.Combine(Application.persistentDataPath,"jimothy-v1.json");
 static string Hash(string s) { using var h = SHA256.Create(); return Convert.ToBase64String(h.ComputeHash(Encoding.UTF8.GetBytes(s))); }
 public static bool Exists => File.Exists(PathName) || File.Exists(PathName+".bak");
 public static bool TryLoad(out SaveData data, out string message) {
  data=null; message="No saved adventure yet.";
  foreach(var path in new[]{PathName,PathName+".bak"}) {
   if(!File.Exists(path)) continue;
   try {
    var e=JsonUtility.FromJson<SaveEnvelope>(File.ReadAllText(path));
    if(e==null || e.version!=1 || e.payload==null || e.sha256!=Hash(e.payload)) throw new InvalidDataException("Unsupported or damaged save.");
    var d=JsonUtility.FromJson<SaveData>(e.payload);
    if(d==null || !d.Valid()) throw new InvalidDataException("Invalid save state.");
    d.ApplyOffline(DateTimeOffset.UtcNow.ToUnixTimeSeconds()); data=d;
    message=path.EndsWith(".bak") ? "Recovered your backup save." : "Welcome back to Ballard."; return true;
   } catch(Exception ex) when(ex is IOException || ex is ArgumentException || ex is UnauthorizedAccessException) { message="Couldn't read this save. Your files have been preserved."; }
  }
  return false;
 }
 public static bool Write(SaveData data,out string message) {
  message="Adventure saved.";
  try {
   data.savedUtc=DateTimeOffset.UtcNow.ToUnixTimeSeconds();
   if(!data.Valid()) throw new InvalidDataException("Invalid state was not saved.");
   string payload=JsonUtility.ToJson(data);
   string json=JsonUtility.ToJson(new SaveEnvelope { payload=payload,sha256=Hash(payload) });
   Directory.CreateDirectory(Application.persistentDataPath);
   string temp=PathName+".tmp";
#if UNITY_WEBGL && !UNITY_EDITOR
   // Browser virtual filesystem: retain recovery copy, then let autoSyncPersistentDataPath persist to IndexedDB.
   File.WriteAllText(temp,json,Encoding.UTF8);
   if(File.Exists(PathName)){File.Copy(PathName,PathName+".bak",true);File.Delete(PathName);}
   File.Move(temp,PathName);
#else
   using(var stream=new FileStream(temp,FileMode.Create,FileAccess.Write,FileShare.None)) {
    byte[] bytes=Encoding.UTF8.GetBytes(json);stream.Write(bytes,0,bytes.Length);stream.Flush(true);
   }
   if(File.Exists(PathName)) File.Replace(temp,PathName,PathName+".bak"); else File.Move(temp,PathName);
#endif
   return true;
  } catch(Exception ex) when(ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException) { message="Save failed: "+ex.Message; return false; }
 }
}
[Serializable] public class ItemDefinition { public string id, name, category, rarity; public int value, nutrition; }
[Serializable] public class ItemCatalog { public ItemDefinition[] items; }
}
