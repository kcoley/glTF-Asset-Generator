using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Newtonsoft.Json;

namespace AssetGenerator
{
    internal class ModelGenerator
    {
        private Object LoadJSON(string manifestURL)
        {
            using (var reader = new StreamReader(manifestURL))
            {
                var json = reader.ReadToEnd();
                var items = JsonConvert.DeserializeObject<dynamic>(json);

                return items;
            }

        }

        private Runtime.GLTF InitializeGeometry(Object json)
        {
            var geometryInfo = json["geometry"].Value;
        }
        public ModelGenerator(string manifestURL)
        {
            var json = LoadJSON(manifestURL);
            var asset = InitializeGeometry(json);

        }
        private static void ModifyGeometry(Runtime.MeshPrimitive meshPrimitive, Dictionary<string, MGProperty> properties)
        {
           foreach(var kvp in properties)
            {
                switch(kvp.Key)
                {
                    case "normals":
                        var normals = kvp.Value.GetVectors();
                        meshPrimitive.Normals = normals;
                        break;
                }
            }

        }
        private static void ModifyMaterial(Runtime.Asset asset, Dictionary<string, MGProperty> properties)
        {
            
        }

        public static void CreateModelWithProperties(Dictionary<string, MGProperty> properties)
        {
            var asset = new Runtime.Asset
            {
                Generator = "glTF Asset Generator",
                Version = "2.0",
                // Extras = new Runtime.Extras
                // {
                //     // Inserts a string into the .gltf containing the properties that are set for a given model, for debug.
                //     Attributes = String.Join(" - ", name)
                // }
            };

            ModifyGeometry(asset, properties);
            ModifyMaterial(asset, properties);
        }
    }
}