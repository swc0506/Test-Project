using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ObjectSystem]
    [FriendClassAttribute(typeof(ET.UnitCache))]
    public class UnitCacheComponentAwakeSystem : AwakeSystem<UnitCacheComponent>
    {
        public override void Awake(UnitCacheComponent self)
        {
            self.UnitCacheKeyList.Clear();
            foreach (Type type in Game.EventSystem.GetTypes().Values)
            {
                if (type != typeof(IUnitCache) && typeof(IUnitCache).IsAssignableFrom(type))
                {
                    self.UnitCacheKeyList.Add(type.Name);
                }
            }

            foreach (string key in self.UnitCacheKeyList)
            {
                UnitCache unitCache = self.AddChild<UnitCache>();
                unitCache.key = key;
                self.UnitCaches.Add(key, unitCache);
            }
        }
    }

    [ObjectSystem]
    public class UnitCacheComponentDestroySystem : DestroySystem<UnitCacheComponent>
    {
        public override void Destroy(UnitCacheComponent self)
        {
            foreach (var unitCache in self.UnitCaches.Values)
            {
                unitCache?.Dispose();
            }
            self.UnitCaches.Clear();
        }
    }

    [FriendClass(typeof(UnitCacheComponent))]
    [FriendClass(typeof(UnitCache))]
    public static class UnitCacheComponentSystem
    {
        public static async ETTask AddOrUpdate(this UnitCacheComponent self, long id, ListComponent<Entity> entityList)
        {
            using (ListComponent<Entity> List = ListComponent<Entity>.Create())
            {
                foreach (Entity entity in entityList)
                {
                    string key = entity.GetType().Name;
                    if (!self.UnitCaches.TryGetValue(key, out UnitCache unitCache))
                    {
                        unitCache = self.AddChild<UnitCache>();
                        unitCache.key = key;
                        self.UnitCaches.Add(key, unitCache);
                    }
                    unitCache.AddOrUpdate(entity);
                    List.Add(entity);
                }
                if (List.Count > 0)
                {
                    await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Save(id, List);
                }
            }
        }

        public static async ETTask<Entity> Get(this UnitCacheComponent self, long unitId, string key)
        {
            if (!self.UnitCaches.TryGetValue(key, out UnitCache unitCache))
            {
                unitCache = self.AddChild<UnitCache>();
                unitCache.key = key;
                self.UnitCaches.Add(key, unitCache) ;
            }
            return await unitCache.Get(unitId);
        }


        public static void Delete(this UnitCacheComponent self, long unitId)
        {
            foreach (UnitCache unitCache in self.UnitCaches.Values)
            {
                unitCache.Delete(unitId);
            }
        }
    }
}
