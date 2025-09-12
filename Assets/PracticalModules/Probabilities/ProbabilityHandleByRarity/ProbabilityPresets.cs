using System.Collections.Generic;

namespace PracticalModules.Probabilities.ProbabilityHandleByRarity
{
    /// <summary>
    /// Utility class for creating and managing probability presets
    /// </summary>
    public static class ProbabilityPresets
    {
        /// <summary>
        /// Create a standard gacha probability distribution
        /// </summary>
        public static List<RarityProbability> CreateStandardDistribution()
        {
            return new List<RarityProbability>
            {
                new(Rarity.Common, 0.50f) { enableStreakProtection = false },
                new(Rarity.Uncommon, 0.30f) { enableStreakProtection = false },
                new(Rarity.Rare, 0.15f) { enableStreakProtection = true, maxConsecutiveFailures = 20 },
                new(Rarity.Epic, 0.04f) { enableStreakProtection = true, maxConsecutiveFailures = 50 },
                new(Rarity.Legendary, 0.008f)
                    { enableStreakProtection = true, maxConsecutiveFailures = 75, enableTimeBonus = true },
                new(Rarity.Mythic, 0.002f)
                {
                    enableStreakProtection = true, maxConsecutiveFailures = 100, enableTimeBonus = true,
                    enableDynamicScaling = true
                }
            };
        }

        /// <summary>
        /// Create a generous probability distribution
        /// </summary>
        public static List<RarityProbability> CreateGenerousDistribution()
        {
            return new List<RarityProbability>
            {
                new(Rarity.Common, 0.40f),
                new(Rarity.Uncommon, 0.35f),
                new(Rarity.Rare, 0.20f),
                new(Rarity.Epic, 0.08f),
                new(Rarity.Legendary, 0.015f),
                new(Rarity.Mythic, 0.005f)
            };
        }

        /// <summary>
        /// Create a harsh probability distribution
        /// </summary>
        public static List<RarityProbability> CreateHarshDistribution()
        {
            return new()
            {
                new(Rarity.Common, 0.70f),
                new(Rarity.Uncommon, 0.25f),
                new(Rarity.Rare, 0.04f),
                new(Rarity.Epic, 0.008f),
                new(Rarity.Mythic, 0.0015f),
                new(Rarity.Legendary, 0.0005f)
            };
        }
    }
}