using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateCloudApi
{
    public class Provisioner
    {
        public Provisioner(string type, string handler, string read, string create, string update, string delete)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException($"Resource Configuration {nameof(type)} cannot be empty or whitespace");
            }

            if (string.IsNullOrWhiteSpace(handler))
            {
                throw new ArgumentException($"Resource {type} Configuration {nameof(handler)} cannot be empty or whitespace");
            }

            if (string.IsNullOrWhiteSpace(read))
            {
                throw new ArgumentException($"Resource {type} Configuration {nameof(read)} cannot be empty or whitespace");
            }

            if (string.IsNullOrWhiteSpace(create))
            {
                throw new ArgumentException($"Resource {type} Configuration {nameof(create)} cannot be empty or whitespace");
            }

            if (string.IsNullOrWhiteSpace(update))
            {
                throw new ArgumentException($"Resource {type} Configuration {nameof(update)} cannot be empty or whitespace");
            }

            if (string.IsNullOrWhiteSpace(delete))
            {
                throw new ArgumentException($"Resource {type} Configuration {nameof(delete)} cannot be empty or whitespace");
            }

            Type = type;
            Handler = handler;
            Read = read;
            Create = create;
            Update = update;
            Delete = delete;
        }

        public Provisioner()
        {

        }

        public string Type { get; set; }
        public string Handler { get; set; }
        public string Read { get; set; }
        public string Create { get; set; }
        public string Update { get; set; }
        public string Delete { get; set; }

    }
}
