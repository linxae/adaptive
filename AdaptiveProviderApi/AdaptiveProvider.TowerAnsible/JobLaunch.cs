using System.Text.Json.Serialization;

namespace AdaptiveProvider.TowerAnsible
{
    public class JobLaunch
    {
        public JobLaunch()
        {
            ExtraVars = new { };
        }

        public JobLaunch(object extraVariables)
        {
            ExtraVars = extraVariables;
        }


        [JsonPropertyName("extra_vars")]
        public object ExtraVars { get; set; }
    }
}