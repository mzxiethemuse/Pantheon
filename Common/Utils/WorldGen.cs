using Terraria.IO;
using Terraria.WorldBuilding;

namespace Pantheon.Common.Utils;

public abstract class WorldGenTask : GenPass {
    
    /// <summary>
    /// consult https://github.com/tModLoader/tModLoader/wiki/Vanilla-World-Generation-Steps
    /// </summary>
    public virtual string PlaceToInsert { get;}
    
    protected WorldGenTask(string name, double loadWeight) : base(name, loadWeight)
    {
    }

    public virtual void Apply(GenerationProgress? progress = null, GameConfiguration? gameConfiguration = null)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        Apply(progress, configuration);
    }
    
}