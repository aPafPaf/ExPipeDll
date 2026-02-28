using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExPipeDll;

public partial class ExPipeDll : BaseSettingsPlugin<ExPipeDllSettings>
{
    List<Entity> entitiesWorldItems = new List<Entity>();
    Dictionary<string, bool> lootClasses = new Dictionary<string, bool>();
    Queue<(uint id, PacketType type)> LootPacketQueue = new();

    public override bool Initialise()
    {
        Settings.SendButton.OnPressed = DebugSendEntityId;
        Settings.LootClassSettings.UpdateSettings.OnPressed = UpdateLootClassDictionary;

        return true;
    }

    public override void AreaChange(AreaInstance area)
    {
        entitiesWorldItems.Clear();
        LootPacketQueue.Clear();
        LootSendTimestamps.Clear();
        UpdateLootClassDictionary();
    }

    public override Job Tick()
    {
        entitiesWorldItems.Clear();
        var worldItems = GameController.IngameState.IngameUi.ItemsOnGroundLabelsVisible
            .Where(x => x.CanPickUp && x.IsVisible)
            .Select(x => x.ItemOnGround)
            .ToList();
        entitiesWorldItems = worldItems.Where(x => x.IsValid && x.IsTargetable).ToList();

        var isLoading = GameController.Game.LoadingState.IsLoading;
        var isConnected = GameController.IngameState.ServerData.NetworkState == NetworkStateE.Connected;
        var isHotKeyPressed = Settings.LootLoopHotKey.IsPressed();

        if (!isHotKeyPressed)
        {
            LootPacketQueue.Clear();
            return null;
        }

        if (!isLoading && isConnected)
        {
            HandleOutgoingPacket();
        }

        if (isHotKeyPressed && !isLoading && isConnected)
        {
            LootLoop();
        }

        return null;
    }

    public void UpdateLootClassDictionary()
    {
        lootClasses.Clear();
        lootClasses.Add("QuestItem", Settings.LootClassSettings.QuestItem.Value);
        lootClasses.Add("StackableCurrency", Settings.LootClassSettings.StackableCurrency.Value);
        lootClasses.Add("MapFragment", Settings.LootClassSettings.MapFragments.Value);
        lootClasses.Add("Map", Settings.LootClassSettings.Map.Value);
        lootClasses.Add("HeistBlueprint", Settings.LootClassSettings.HeistBlueprint.Value);
        lootClasses.Add("DivinationCard", Settings.LootClassSettings.DivinationCard.Value);
        lootClasses.Add("Jewel", Settings.LootClassSettings.Jewel.Value);
        lootClasses.Add("IncubatorStackable", Settings.LootClassSettings.IncubatorStackable.Value);
    }
}