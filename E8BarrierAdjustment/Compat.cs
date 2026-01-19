namespace E8BarrierAdjustment
{
    public class Compat
    {
        private static bool? _sandswept;
        public static bool Sandswept
        {
            get
            {
                if (_sandswept == null)
                {
                    _sandswept = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamSandswept.Sandswept");
                }
                return (bool)_sandswept;
            }
        }
        
        private static bool? _riskOfOptions;
        public static bool RiskOfOptions
        {
            get
            {
                if (_riskOfOptions == null)
                {
                    _riskOfOptions = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
                }
                return (bool)_riskOfOptions;
            }
        }
    }
}