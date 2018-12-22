using System;
using System.Collections.Generic;
using WorldConverter.Format.Region;

namespace WorldConverter.Format
{
    public class LevelProviderManager
    {
        public List<ILevelProvider> Provider { get; } = new List<ILevelProvider>();

        public LevelProviderManager()
        {
            this.Provider.Add(new Anvil());
        }

        public ILevelProvider GetProvider(string path)
        {
            for (int i = 0; i < this.Provider.Count; ++i)
            {
                if (this.Provider[i].IsValid(path))
                {
                    return this.Provider[i];
                }
            }
            throw new Exception("provider not found");
        }
    }
}
