using System;
using System.Text.Json.Serialization;

namespace AdaptiveProvider.TowerAnsible
{
    public class Job
    {
        [JsonPropertyName("job")]
        public int JobId { get; set; }

        [JsonPropertyName("url")]
        public string JobUrl { get; set; }

        //"created": "2020-06-02T15:02:52.319330Z",
        [JsonPropertyName("created")]
        public DateTime? Created { get; set; }

        //"modified": "2020-06-02T15:02:57.181979Z",
        [JsonPropertyName("modified")]
        public DateTime? Modified { get; set; }

        //"name": "Demo Job Template",
        [JsonPropertyName("name")]
        public string Name { get; set; }

        //"description": "",
        [JsonPropertyName("description")]
        public string Description { get; set; }

        //"job_type": "run",
        [JsonPropertyName("job_type")]
        public string JobType { get; set; }

        //"inventory": 1,
        [JsonPropertyName("inventory")]
        public int Inventory { get; set; }

        //"project": 6,
        [JsonPropertyName("project")]
        public int Project { get; set; }

        //"playbook": "hello_world.yml",
        [JsonPropertyName("playbook")]
        public string Playbook { get; set; }

        //"timeout": 0,
        [JsonPropertyName("timeout")]
        public int Timeout { get; set; }

        //"launch_type": "manual",
        [JsonPropertyName("launch_type")]
        public string LaunchType { get; set; }

        //"status": "successful",
        [JsonPropertyName("status")]
        public string Status { get; set; }

        //"failed": false,
        [JsonPropertyName("failed")]
        public bool Failed { get; set; }

        //"started": "2020-06-02T15:02:57.290486Z",
        [JsonPropertyName("started")]
        public DateTime? Started { get; set; }

        //"finished": "2020-06-02T15:03:00.977286Z",
        [JsonPropertyName("finished")]
        public DateTime? Finished { get; set; }

        //"canceled_on": null,
        [JsonPropertyName("canceled_on")]
        public DateTime? CanceledOn { get; set; }

        //"elapsed": 3.687,
        [JsonPropertyName("elapsed")]
        public double Elapsed { get; set; }

    }
}