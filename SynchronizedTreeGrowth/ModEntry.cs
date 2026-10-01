using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SynchronizedTreeGrowth;

public class ModEntry : Mod
{
    private readonly int _fullyGrownTreeStage = 5;
    private readonly double _growthChance = 0.33;
    private readonly Dictionary<Tree, int> _treeGrowthStages = new();

    // On day ending, get all farm trees
    // Right before it saves, revert growth stage of trees
    // Check if should grow, update growth stage of trees if should.
    public override void Entry(IModHelper helper)
    {
        // When the day starts, reset the should grow
        helper.Events.GameLoop.DayEnding += OnDayEnding;
        helper.Events.GameLoop.Saving += OnSaving;
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs dayEndingEventArgs)
    {
        if (Game1.currentSeason == "winter") return;

        var farm = Game1.getFarm();
        var trees = farm._activeTerrainFeatures
            .OfType<Tree>()
            .Where(tree => !tree.stump.Value // Tree can't be a stump
                           && !tree.IsGrowthBlockedByNearbyTree() // Tree can't be blocked by another fully grown tree
                           && tree.growthStage.Value < _fullyGrownTreeStage // Tree can't be fully grown yet
                           && !tree.fertilized.Value); // Let the original code handle fertilized trees

        _treeGrowthStages.Clear();

        foreach (var tree in trees) _treeGrowthStages.Add(tree, tree.growthStage.Value);
    }

    private void OnSaving(object? sender, SavingEventArgs savingEventArgs)
    {
        if (Game1.currentSeason == "winter") return;

        var gameId = (int)Game1.uniqueIDForThisGame;
        var dayOfMonth = Game1.dayOfMonth;

        var random = new Random(gameId + dayOfMonth);

        if (random.NextDouble() > _growthChance) RestoreGrowthStages();
        else UpdateGrowthStages();
    }

    private void RestoreGrowthStages()
    {
        Monitor.Log("Restoring old growth stages");
        foreach (var (tree, oldGrowthValue) in _treeGrowthStages) tree.growthStage.Set(oldGrowthValue);
    }

    private void UpdateGrowthStages()
    {
        Monitor.Log("Updating growth stages");
        foreach (var (tree, oldGrowthValue) in _treeGrowthStages) tree.growthStage.Set(oldGrowthValue + 1);
    }
}