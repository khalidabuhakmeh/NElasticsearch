﻿using System.Text;
using System.Threading.Tasks;
using NElasticsearch.Mapping;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// See http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices.html#mapping-management
    /// </summary>
    public static class MappingManagementCommands
    {
        public static async Task PutMappingFor<T>(this ElasticsearchClient client, string indexName)
        {
            await PutMappingFor<T>(client, new[] { indexName });
        }

        public static async Task PutMappingFor<T>(this ElasticsearchClient client, string[] indexNames)
        {
            var typeName = TypeMappingWriter.GetMappingTypeNameFor<T>();

            // We are only going to do a put mapping if there are attributes asking for it
            var sb = new StringBuilder();
            if (TypeMappingWriter.GetMappingFor<T>(sb, typeName))
            {
                await PutMapping(client, indexNames, typeName, sb.ToString());
            }
        }

        public static async Task PutMapping(this ElasticsearchClient client,
            string indexName, string typeName, object mapping)
        {
            await PutMapping(client, new[] {indexName}, typeName, mapping);
        }

        public static async Task PutMapping(this ElasticsearchClient client,
            string[] indexNames, string typeName, object mapping)
        {
            var sb = new StringBuilder();
            if (indexNames == null || indexNames.Length == 0)
            {
                sb.Append("*");
            }
            else
            {
                sb.Append(string.Join(",", indexNames));
            }
            sb.Append('/');
            sb.Append(typeName);
            sb.Append("/_mapping");

            await client.Execute(RestMethod.PUT, sb.ToString(), mapping);
        }

        // TODO Get mapping API
        // TODO Get Field Mapping API

        public static async Task DeleteMapping(this ElasticsearchClient client,
            string indexName, string typeName)
        {
            await client.Execute(RestMethod.DELETE, indexName + "/" + typeName + "/_mapping");
        }

        // TODO type exists API
    }
}
