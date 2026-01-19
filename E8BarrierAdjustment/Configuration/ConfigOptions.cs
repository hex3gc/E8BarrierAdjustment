namespace E8BarrierAdjustment.Configuration
{
    public class ConfigOptions
    {
        public ConfigItem<bool> Barrier_Enabled = new ConfigItem<bool>
        (
            "Enable mechanics",
            "Barrier curse prevention",
            "Taking hits to temporary barrier no longer inflicts Eclipse 8 curse.",
            true
        );

        public ConfigItem<bool> Plating_Enabled = new ConfigItem<bool>
        (
            "Enable mechanics",
            "Sandswept: Plating curse prevention",
            "Taking hits to Makeshift Plating's temporary health no longer inflicts Eclipse 8 curse.",
            true
        );
    }
}