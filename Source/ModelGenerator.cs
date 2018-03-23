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
        private void SetTexture(string textureName, Runtime.Material material)
        {
            var texture = new Runtime.Texture();
            var stringTemplate = "Textures/{0}";
            var image = new Runtime.Image();
            switch (textureName)
            {
                case "normalTexture":
                    {
                        image.Uri = String.Format(stringTemplate, "Normal_Plane.png");
                        material.NormalTexture = new Runtime.Texture
                        {
                            Source = image,
                            Name = "Normal Texture",
                        };
                        break;
                    }
                case "occlusionTexture":
                    {
                        image.Uri = String.Format(stringTemplate, "Occlusion_Plane.png");
                        material.OcclusionTexture = new Runtime.Texture
                        {
                            Source = image,
                            Name = "Occlusion Texture",
                        };
                        break;
                    }
                case "emissiveTexture":
                    {
                        image.Uri = String.Format(stringTemplate, "Emissive_Plane.png");
                        material.EmissiveTexture = new Runtime.Texture
                        {
                            Source = image,
                            Name = "Emissive Texture",
                        };
                        break;
                    }
            }
        }
        private void ApplyProperty(string propertyName, JToken mapping, Runtime.GLTF asset, JToken entry)
        {
            int sceneIndex = -1, nodeIndex = -1, meshPrimitiveIndex = -1;
            Runtime.MeshPrimitive meshPrimitive = null;
            switch (propertyName)
            {
                case "metallicFactor":
                    sceneIndex = mapping["scene"].ToObject<int>();
                    nodeIndex = mapping["node"].ToObject<int>();
                    meshPrimitiveIndex = mapping["meshPrimitive"].ToObject<int>();
                    meshPrimitive = asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex];
                    if (meshPrimitive.Material == null)
                    {
                        meshPrimitive.Material = new Runtime.Material();
                        meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    }

                    meshPrimitive.Material.MetallicRoughnessMaterial.MetallicFactor = entry["value"].ToObject<int>();

                    break;
                default:
                    throw new NotImplementedException(String.Format("propertyName {0} not implemented yet!", propertyName));
                case "baseColorFactor":
                    sceneIndex = mapping["scene"].ToObject<int>();
                    nodeIndex = mapping["node"].ToObject<int>();
                    meshPrimitiveIndex = mapping["meshPrimitive"].ToObject<int>();
                    meshPrimitive = asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex];
                    if (meshPrimitive.Material == null)
                    {
                        meshPrimitive.Material = new Runtime.Material();
                        meshPrimitive.Material.MetallicRoughnessMaterial = new Runtime.PbrMetallicRoughness();
                    }
                    var value = new Vector4(entry["value"][0].ToObject<int>(), entry["value"][1].ToObject<int>(), entry["value"][2].ToObject<int>(), entry["value"][3].ToObject<int>());
                    asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex].Material.MetallicRoughnessMaterial.BaseColorFactor = value;
                    break;
                case "normalTextureScale":
                    sceneIndex = mapping["scene"].ToObject<int>();
                    nodeIndex = mapping["node"].ToObject<int>();
                    meshPrimitiveIndex = mapping["meshPrimitive"].ToObject<int>();
                    meshPrimitive = asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex];
                    if (meshPrimitive.Material == null)
                    {
                        meshPrimitive.Material = new Runtime.Material();
                    }
                    meshPrimitive.Material.NormalScale = entry["value"].ToObject<float>();
                    break;

                case "emissiveFactor":
                    sceneIndex = mapping["scene"].ToObject<int>();
                    nodeIndex = mapping["node"].ToObject<int>();
                    meshPrimitiveIndex = mapping["meshPrimitive"].ToObject<int>();
                    meshPrimitive = asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex];
                    if (meshPrimitive.Material == null)
                    {
                        meshPrimitive.Material = new Runtime.Material();
                    }
                    var emissiveFactor = new Vector3(entry["value"][0].ToObject<float>(), entry["value"][1].ToObject<float>(), entry["value"][2].ToObject<float>());
                    asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex].Material.EmissiveFactor = emissiveFactor;
                    break;

                case "occlusionTextureStrength":
                    sceneIndex = mapping["scene"].ToObject<int>();
                    nodeIndex = mapping["node"].ToObject<int>();
                    meshPrimitiveIndex = mapping["meshPrimitive"].ToObject<int>();
                    meshPrimitive = asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex];
                    if (meshPrimitive.Material == null)
                    {
                        meshPrimitive.Material = new Runtime.Material();
                    }
                    meshPrimitive.Material.OcclusionStrength = entry["value"].ToObject<float>();
                    break;

                case "normalTexture":
                case "occlusionTexture":
                case "emissiveTexture":
                    sceneIndex = mapping["scene"].ToObject<int>();
                    nodeIndex = mapping["node"].ToObject<int>();
                    meshPrimitiveIndex = mapping["meshPrimitive"].ToObject<int>();
                    meshPrimitive = asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex];
                    if (meshPrimitive.Material == null)
                    {
                        meshPrimitive.Material = new Runtime.Material();
                    }
                    SetTexture(propertyName, meshPrimitive.Material);
                    break;

                case "normals":
                    sceneIndex = mapping["scene"].ToObject<int>();
                    nodeIndex = mapping["node"].ToObject<int>();
                    meshPrimitiveIndex = mapping["meshPrimitive"].ToObject<int>();
                    meshPrimitive = asset.Scenes[sceneIndex].Nodes[nodeIndex].Mesh.MeshPrimitives[meshPrimitiveIndex];
                    if (meshPrimitive.Normals == null)
                    {
                        meshPrimitive.Normals = new List<Vector3>();
                    }
                    foreach (var normal in entry["value"])
                    {
                        var normalVec = new Vector3(normal[0].ToObject<float>(), normal[1].ToObject<float>(), normal[2].ToObject<float>());
                        meshPrimitive.Normals.Add(normalVec);
                    }
                    break;
            }
        }
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
                switch (geometryName)
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
            var json = JObject.Parse(File.ReadAllText(manifestURL));
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
                        foreach (var property in requiredProperties)
                        {
                            var p = property.ToString();
                            var entry = properties[p];
                            var mappingIndex = (entry["mapping"] != null) ? entry["mapping"].ToObject<int>() : 0;
                            var mapping = mappings[mappingIndex];
                            var propertyName = property.ToObject<string>();
                            ApplyProperty(propertyName, mapping, asset, entry);
                        }
                    }
                }
            }
        }

        private void GenerateModels(JObject json, Runtime.GLTF asset)
        {
            JToken value;
            JToken mappings;
            JToken properties;
            List<Runtime.GLTF> generatedModels = new List<Runtime.GLTF>();
            if (json.TryGetValue("propertyMappings", out mappings))
            {
                if (json.TryGetValue("properties", out properties))
                {
                    if (json.TryGetValue("models", out value))
                    {
                        var models = value.Children();
                        foreach (var model in models)
                        {
                            var copiedAsset = DeepCopy.CloneObject(asset);
                            if (model["properties"] != null)
                            {
                                foreach (var property in model["properties"])
                                {
                                    var p = property.ToString();
                                    var entry = properties[p];
                                    var mappingIndex = (entry["mapping"] != null) ? entry["mapping"].ToObject<int>() : 0;
                                    var mapping = mappings[mappingIndex];
                                    var propertyName = property.ToObject<string>();
                                    ApplyProperty(propertyName, mapping, copiedAsset, entry);
                                }
                            }
                            // At this point, should have a generated asset.
                            generatedModels.Add(copiedAsset);
                        }
                    }
                    else
                    {
                        throw new MissingMemberException("models key missing from manifest file!");
                    }

                }
            }

        }
        private static void ModifyGeometry(Runtime.MeshPrimitive meshPrimitive, Dictionary<string, MGProperty> properties)
        {
            foreach (var kvp in properties)
            {
                switch (kvp.Key)
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

            //  ModifyGeometry(asset, properties);
            ModifyMaterial(asset, properties);
        }
    }
}