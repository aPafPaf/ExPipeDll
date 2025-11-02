using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ExPipeDll;

public partial class ExPipeDll : BaseSettingsPlugin<ExPipeDllSettings>
{
    List<Entity> entitiesWorldItems = new List<Entity>();
    Dictionary<string, bool> lootClasses = new Dictionary<string, bool>();

    public override bool Initialise()
    {
        Settings.SendButton.OnPressed = DebugSendEntityId;
        Settings.LootClassSettings.UpdateSettings.OnPressed = UpdateDicttonary;

        return true;
    }

    public override void AreaChange(AreaInstance area)
    {
        entitiesWorldItems.Clear();
        UpdateDicttonary();
    }

    public override Job Tick()
    {
        entitiesWorldItems.Clear();
        var worldItems = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.WorldItem];
        entitiesWorldItems = worldItems.Where(x => x.IsValid && x.IsTargetable).ToList();

        if (Settings.LootLoopHotKey.IsPressed())
        {
            LootLoop();
        }

        return null;
    }

    public void UpdateDicttonary()
    {
        lootClasses.Clear();
        lootClasses.Add("QuestItem", Settings.LootClassSettings.QuestItem.Value);
        lootClasses.Add("StackableCurrency", Settings.LootClassSettings.StackableCurrency.Value);
        lootClasses.Add("MapFragment", Settings.LootClassSettings.MapFragments.Value);
        lootClasses.Add("Map", Settings.LootClassSettings.Map.Value);
        lootClasses.Add("HeistBlueprint", Settings.LootClassSettings.HeistBlueprint.Value);
    }
}