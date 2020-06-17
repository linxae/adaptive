using System.Text.Json.Serialization;

namespace AdaptiveProvider.TowerAnsible
{
    public class JobLaunch
    {
        public JobLaunch()
        {
            ExtraVars = new { };
        }

        public JobLaunch(object extraVariables, string jobType)
        {
            ExtraVars = extraVariables;
            JobType = jobType;
        }


        [JsonPropertyName("extra_vars")]
        public object ExtraVars { get; set; }

        [JsonPropertyName("job_type")]
        public string JobType { get; set; }
    }
}