using UnityEditor.UI;

namespace ET
{
    public static class UnitHelper
    {
        public static Unit GetMyUnitFromZoneScene(Scene zoneScene)
        {
            PlayerComponent playerComponent = zoneScene.GetComponent<PlayerComponent>();
            Scene currentScene = zoneScene.GetComponent<CurrentScenesComponent>().Scene;
            return currentScene.GetComponent<UnitComponent>().Get(playerComponent.MyId);
        }
        
        public static Unit GetMyUnitFromCurrentScene(Scene currentScene)
        {
            PlayerComponent playerComponent = currentScene.Parent.Parent.GetComponent<PlayerComponent>();
            return currentScene.GetComponent<UnitComponent>().Get(playerComponent.MyId);
        }

        public static NumericComponent GetMyUnitNumericComponent(Scene cuurentScene)
        {
            NumericComponent numericComponent = GetMyUnitFromCurrentScene(cuurentScene).GetComponent<NumericComponent>();
            return numericComponent;
        }
    }
}