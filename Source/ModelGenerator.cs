using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AssetGenerator
{
    internal class ModelGenerator
    {
        /// <summary>
        /// Initializes a glTF asset based on the geometry field of the manifest json.
        /// </summary>
        /// <param name="json">json manifest data</param>
        /// <returns>Runtime glTF object.</returns>
        private Runtime.GLTF InitializeGeometry(JObject json)
        {
            JToken value;
            if (json.TryGetValue("geometry", out value))
            {
                var geometryName = value.ToString();
                switch(geometryName)
                {
                    case "SinglePlane":
                        return Common.SinglePlane();
                    case "MultiNode":
                        return Common.MultiNode();
                    default:
                        throw new NotImplementedException(String.Format("Geometry type {0} not yet implemented!", geometryName));
                }
            }
            else
            {
                throw new MissingMemberException("geometry key missing from manifest file!");
            }
        }
        public ModelGenerator(string manifestURL)
        {
            var json = JObject.Parse(manifestURL);
            var asset = InitializeGeometry(json);
            AddRequiredPropertiesToAsset(json, asset);

            GenerateModels(json, asset);

        }
        private void AddRequiredPropertiesToAsset(JObject json, Runtime.GLTF asset)
        {
            JToken mappings;
            JToken properties, requiredProperties;
            if (json.TryGetValue("propertyMappings", out mappings))
            {
                if (json.TryGetValue("properties", out properties))
                {
                    if (json.TryGetValue("requiredProperties", out requiredProperties))
                    {
                        foreach(var property in requiredProperties)
                        {
                            var entry = properties[property];
                            var mappingIndex = (entry["mapping"] != null) ? entry["mapping"] : 0;
                            var mapping = mappings[mappingIndex];
                            var propertyName = property.ToObject<string>();
                            switch(propertyName)
                            {
                                case "metallicFactor":
                                    {
                                        var sceneIndex = mapping["scene"].ToObject<int>();
                                        var nodeIndex = mapping["node"].ToObject<int>();
                                        var meshPrimitiveIndex = mapping["meshPrimitive"].ToObject<int>();
                                        asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex].Material.MetallicRoughnessMaterial.MetallicFactor = entry["value"].ToObject<int>();
                                        break;
                                    }
                                default:
                                    throw new NotImplementedException(String.Format("propertyName {0} not implemented yet!", propertyName));
                                case "baseColorFactor":
                                    {
                                        var sceneIndex = mapping["scene"].ToObject<int>();
                                        var nodeIndex = mapping["node"].ToObject<int>();
                                        var meshPrimitiveIndex = mapping["meshPrimitive"].ToObject<int>();
                                        asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex].Material.MetallicRoughnessMaterial.BaseColorFactor = entry["value"].ToObject<Vector4>();
                                        break;
                                    }
                            }
                        }
                    }
                }   
            }
        }

        private void GenerateModels(JObject json, Runtime.GLTF asset)
        {
            JToken value;
            if (json.TryGetValue("models", out value))
            {
                var models = value.Children();
                foreach(var model in models)
                {

                }
            }
            else
            {
                throw new MissingMemberException("models key missing from manifest file!");
            }
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