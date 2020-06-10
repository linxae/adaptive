using System.Text.Json.Serialization;

namespace AdaptiveProvider.TowerAnsible
{
    public class TowerApiMap
    {
        //"ping": "/api/v2/ping/",
        [JsonPropertyName("ping")]
        public string Ping { get; set; }

        //"instances": "/api/v2/instances/",
        [JsonPropertyName("instances")]
        public string Instances { get; set; }

        //"instance_groups": "/api/v2/instance_groups/",
        [JsonPropertyName("instance_groups")]
        public string Instance_groups { get; set; }

        //"config": "/api/v2/config/",
        [JsonPropertyName("config")]
        public string Config { get; set; }

        //"settings": "/api/v2/settings/",
        [JsonPropertyName("settings")]
        public string Settings { get; set; }

        //"me": "/api/v2/me/",
        [JsonPropertyName("me")]
        public string Me { get; set; }

        //"dashboard": "/api/v2/dashboard/",
        [JsonPropertyName("dashboard")]
        public string Dashboard { get; set; }

        //"organizations": "/api/v2/organizations/",
        [JsonPropertyName("organizations")]
        public string Organizations { get; set; }

        //"users": "/api/v2/users/",
        [JsonPropertyName("users")]
        public string Users { get; set; }

        //"projects": "/api/v2/projects/",
        [JsonPropertyName("projects")]
        public string Projects { get; set; }

        //"project_updates": "/api/v2/project_updates/",
        [JsonPropertyName("project_updates")]
        public string Project_updates { get; set; }

        //"teams": "/api/v2/teams/",
        [JsonPropertyName("teams")]
        public string Teams { get; set; }

        //"credentials": "/api/v2/credentials/",
        [JsonPropertyName("credentials")]
        public string Credentials { get; set; }

        //"credential_types": "/api/v2/credential_types/",
        [JsonPropertyName("credential_types")]
        public string Credential_types { get; set; }

        //"credential_input_sources": "/api/v2/credential_input_sources/",
        [JsonPropertyName("credential_input_sources")]
        public string Credential_input_sources { get; set; }

        //"applications": "/api/v2/applications/",
        [JsonPropertyName("applications")]
        public string Applications { get; set; }

        //"tokens": "/api/v2/tokens/",
        [JsonPropertyName("tokens")]
        public string Tokens { get; set; }

        //"metrics": "/api/v2/metrics/",
        [JsonPropertyName("metrics")]
        public string Metrics { get; set; }

        //"inventory": "/api/v2/inventories/",
        [JsonPropertyName("inventory")]
        public string Inventory { get; set; }

        //"inventory_scripts": "/api/v2/inventory_scripts/",
        [JsonPropertyName("inventory_scripts")]
        public string Inventory_scripts { get; set; }

        //"inventory_sources": "/api/v2/inventory_sources/",
        [JsonPropertyName("inventory_sources")]
        public string Inventory_sources { get; set; }

        //"inventory_updates": "/api/v2/inventory_updates/",
        [JsonPropertyName("inventory_updates")]
        public string Inventory_updates { get; set; }

        //"groups": "/api/v2/groups/",
        [JsonPropertyName("groups")]
        public string Groups { get; set; }

        //"hosts": "/api/v2/hosts/",
        [JsonPropertyName("hosts")]
        public string Hosts { get; set; }

        //"job_templates": "/api/v2/job_templates/",
        [JsonPropertyName("job_templates")]
        public string Job_templates { get; set; }

        //"jobs": "/api/v2/jobs/",
        [JsonPropertyName("jobs")]
        public string Jobs { get; set; }

        //"job_events": "/api/v2/job_events/",
        [JsonPropertyName("job_events")]
        public string Job_events { get; set; }

        //"ad_hoc_commands": "/api/v2/ad_hoc_commands/",
        [JsonPropertyName("ad_hoc_commands")]
        public string Ad_hoc_commands { get; set; }

        //"system_job_templates": "/api/v2/system_job_templates/",
        [JsonPropertyName("system_job_templates")]
        public string System_job_templates { get; set; }

        //"system_jobs": "/api/v2/system_jobs/",
        [JsonPropertyName("system_jobs")]
        public string System_jobs { get; set; }

        //"schedules": "/api/v2/schedules/",
        [JsonPropertyName("schedules")]
        public string Schedules { get; set; }

        //"roles": "/api/v2/roles/",
        [JsonPropertyName("roles")]
        public string Roles { get; set; }

        //"notification_templates": "/api/v2/notification_templates/",
        [JsonPropertyName("notification_templates")]
        public string Notification_templates { get; set; }

        //"notifications": "/api/v2/notifications/",
        [JsonPropertyName("notifications")]
        public string Notifications { get; set; }

        //"labels": "/api/v2/labels/",
        [JsonPropertyName("labels")]
        public string Labels { get; set; }

        //"unified_job_templates": "/api/v2/unified_job_templates/",
        [JsonPropertyName("unified_job_templates")]
        public string Unified_job_templates { get; set; }

        //"unified_jobs": "/api/v2/unified_jobs/",
        [JsonPropertyName("unified_jobs")]
        public string Unified_jobs { get; set; }

        //"activity_stream": "/api/v2/activity_stream/",
        [JsonPropertyName("activity_stream")]
        public string Activity_stream { get; set; }

        //"workflow_job_templates": "/api/v2/workflow_job_templates/",
        [JsonPropertyName("workflow_job_templates")]
        public string Workflow_job_templates { get; set; }

        //"workflow_jobs": "/api/v2/workflow_jobs/",
        [JsonPropertyName("workflow_jobs")]
        public string Workflow_jobs { get; set; }

        //"workflow_approvals": "/api/v2/workflow_approvals/",
        [JsonPropertyName("workflow_approvals")]
        public string Workflow_approvals { get; set; }

        //"workflow_job_template_nodes": "/api/v2/workflow_job_template_nodes/",
        [JsonPropertyName("workflow_job_template_nodes")]
        public string Workflow_job_template_nodes { get; set; }

        //"workflow_job_nodes": "/api/v2/workflow_job_nodes/"
        [JsonPropertyName("workflow_job_nodes")]
        public string Workflow_job_nodes { get; set; }

    }
}