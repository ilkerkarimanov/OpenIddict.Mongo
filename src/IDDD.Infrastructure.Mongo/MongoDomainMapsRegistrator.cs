using IDDD.Infrastructure.Mongo.DocumentMaps;
using System;
using System.Linq;
using System.Reflection;

namespace IDDD.Infrastructure.Mongo
{
    public static class MongoDomainMapsRegistrator
    {
        public static void RegisterDocumentMaps()
        {
            var assembly = typeof(MongoDomainMapsRegistrator).GetTypeInfo().Assembly;

            var classMaps = assembly.GetTypes()
                .Where(t =>
                t.GetTypeInfo().BaseType != null
                && t.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                t.GetTypeInfo().BaseType.GetGenericTypeDefinition() == typeof(MongoDbClassMap<>));

            foreach (var classMap in classMaps)
                Activator.CreateInstance(classMap);
        }
    }
}
