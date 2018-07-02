using AssetGenerator.Runtime.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace AssetGenerator.Runtime.GLTFConverter
{
    /// <summary>
    /// Convert Runtime Abstraction to Schema.
    /// </summary>
    internal partial class GLTFConverter
    {
        private List<glTFLoader.Schema.Buffer> buffers = new List<glTFLoader.Schema.Buffer>();
        private List<glTFLoader.Schema.BufferView> bufferViews = new List<glTFLoader.Schema.BufferView>();
        private List<glTFLoader.Schema.Accessor> accessors = new List<glTFLoader.Schema.Accessor>();
        private List<glTFLoader.Schema.Material> materials = new List<glTFLoader.Schema.Material>();
        private List<glTFLoader.Schema.Node> nodes = new List<glTFLoader.Schema.Node>();
        private List<glTFLoader.Schema.Scene> scenes = new List<glTFLoader.Schema.Scene>();
        private List<glTFLoader.Schema.Image> images = new List<glTFLoader.Schema.Image>();
        private List<glTFLoader.Schema.Sampler> samplers = new List<glTFLoader.Schema.Sampler>();
        private List<glTFLoader.Schema.Texture> textures = new List<glTFLoader.Schema.Texture>();
        private List<glTFLoader.Schema.Mesh> meshes = new List<glTFLoader.Schema.Mesh>();
        private List<glTFLoader.Schema.Animation> animations = new List<glTFLoader.Schema.Animation>();
        private List<glTFLoader.Schema.Skin> skins = new List<glTFLoader.Schema.Skin>();
        private GLTF runtimeGLTF;
        private Data geometryData;
        private glTFLoader.Schema.Buffer buffer;
        private int bufferIndex = 0;
        private Dictionary<Node, int> nodeToIndexCache = new Dictionary<Node, int>();
        private enum attributeEnum { POSITION, NORMAL, TANGENT, COLOR, TEXCOORDS_0, TEXCOORDS_1, JOINTS_0, WEIGHTS_0 };

        /// <summary>
        /// Set this property to allow creating custom types.
        /// </summary>
        public Func<Type, object> CreateInstanceOverride = type => Activator.CreateInstance(type);

        /// <summary>
        /// Utility struct for holding sampler, image and texture coord indices
        /// </summary>
        private struct TextureIndices
        {
            public int? SamplerIndex;
            public int? ImageIndex;
            public int? TextureCoordIndex;
        }

        private T CreateInstance<T>()
        {
            return (T)this.CreateInstanceOverride(typeof(T));
        }

        /// <summary>
        /// converts the Runtime image to a glTF Image
        /// </summary>
        /// <returns>Returns a gltf Image object</returns>
        private glTFLoader.Schema.Image ConvertImageToSchema(Image runtimeImage)
        {
            var image = CreateInstance<glTFLoader.Schema.Image>();

            image.Uri = runtimeImage.Uri;

            if (runtimeImage.MimeType.HasValue)
            {
                image.MimeType = runtimeImage.MimeType.Value;
            }

            if (runtimeImage.Name != null)
            {
                image.Name = runtimeImage.Name;
            }

            return image;
        }

        private glTFLoader.Schema.Sampler ConvertSamplerToSchema(Sampler runtimeSampler)
        {
            var sampler = CreateInstance<glTFLoader.Schema.Sampler>();

            if (runtimeSampler.MagFilter.HasValue)
            {
                sampler.MagFilter = runtimeSampler.MagFilter.Value;
            }

            if (runtimeSampler.MinFilter.HasValue)
            {
                sampler.MinFilter = runtimeSampler.MinFilter.Value;
            }

            if (runtimeSampler.WrapS.HasValue)
            {
                sampler.WrapS = runtimeSampler.WrapS.Value;
            }

            if (runtimeSampler.WrapT.HasValue)
            {
                sampler.WrapT = runtimeSampler.WrapT.Value;
            }

            if (runtimeSampler.Name != null)
            {
                sampler.Name = runtimeSampler.Name;
            }

            return sampler;
        }

        /// <summary>
        /// Adds a texture to the property components of the GLTFWrapper.
        /// </summary>
        /// <returns>Returns the indicies of the texture and the texture coordinate as an array of two integers if created.  Can also return null if the index is not defined. (</returns>
        private TextureIndices AddTexture(Texture runtimeTexture)
        {
            var indices = new List<int>();
            int? samplerIndex = null;
            int? imageIndex = null;
            int? textureCoordIndex = null;

            if (runtimeTexture != null)
            {
                if (runtimeTexture.Sampler != null)
                {
                    // If a similar sampler is already being used in the list, reuse that index instead of creating a new sampler object
                    if (samplers.Any())
                    {
                        int findIndex = -1;
                        int i = 0;
                        foreach (var sampler in samplers)
                        {
                            if (sampler.SamplersEqual(ConvertSamplerToSchema(runtimeTexture.Sampler)))
                            {
                                findIndex = i;
                                break;
                            }
                            ++i;
                        }
                    }
                    if (!samplerIndex.HasValue)
                    {
                        var sampler = ConvertSamplerToSchema(runtimeTexture.Sampler);
                        samplers.Add(sampler);
                        samplerIndex = samplers.Count() - 1;
                    }
                }
                if (runtimeTexture.Source != null)
                {
                    // If an equivalent image object has already been created, reuse its index instead of creating a new image object
                    var image = ConvertImageToSchema(runtimeTexture.Source);
                    int findImageIndex = -1;

                    if (images.Any())
                    {
                        for (int i = 0; i < images.Count(); ++i)
                        {
                            if (images[i].ImagesEqual(image))
                            {
                                findImageIndex = i;
                                break;
                            }
                        }
                    }

                    if (findImageIndex != -1)
                    {
                        imageIndex = findImageIndex;
                    }

                    if (!imageIndex.HasValue)
                    {
                        images.Add(image);
                        imageIndex = images.Count() - 1;
                    }
                }

                var texture = CreateInstance<glTFLoader.Schema.Texture>();
                if (samplerIndex.HasValue)
                {
                    texture.Sampler = samplerIndex.Value;
                }
                if (imageIndex.HasValue)
                {
                    texture.Source = imageIndex.Value;
                }
                if (runtimeTexture.Name != null)
                {
                    texture.Name = runtimeTexture.Name;
                }
                // If an equivalent texture has already been created, re-use that texture's index instead of creating a new texture
                int findTextureIndex = -1;
                if (textures.Count > 0)
                {
                    for (int i = 0; i < textures.Count(); ++i)
                    {
                        if (textures[i].TexturesEqual(texture))
                        {
                            findTextureIndex = i;
                            break;
                        }
                    }
                }
                if (findTextureIndex > -1)
                {
                    indices.Add(findTextureIndex);
                }
                else
                {
                    textures.Add(texture);
                    indices.Add(textures.Count() - 1);
                }

                if (runtimeTexture.TexCoordIndex.HasValue)
                {
                    indices.Add(runtimeTexture.TexCoordIndex.Value);
                    textureCoordIndex = runtimeTexture.TexCoordIndex.Value;
                }
            }

            TextureIndices textureIndices = new TextureIndices
            {
                SamplerIndex = samplerIndex,
                ImageIndex = imageIndex,
                TextureCoordIndex = textureCoordIndex
            };

            return textureIndices;
        }

        private int GetPaddedSize(int value, int size)
        {
            var remainder = value % size;
            return (remainder == 0 ? value : checked(value + size - remainder));
        }

        /// <summary>
        /// Pads a value to ensure it is a multiple of size
        /// </summary>
        private int Align(Data geometryData, int value, int size)
        {
            var paddedValue = GetPaddedSize(value, size);

            int additionalPaddedBytes = paddedValue - value;
            for (int i = 0; i < additionalPaddedBytes; ++i)
            {
                geometryData.Writer.Write((byte)0);
            };
            value += additionalPaddedBytes;

            return value;
        }

        /// <summary>
        /// Creates a bufferview schema object
        /// </summary>
        private glTFLoader.Schema.BufferView CreateBufferView(int bufferIndex, string name, int byteLength, int byteOffset, int? byteStride)
        {
            var bufferView = CreateInstance<glTFLoader.Schema.BufferView>();

            bufferView.Name = name;
            bufferView.ByteLength = byteLength;
            bufferView.ByteOffset = byteOffset;
            bufferView.Buffer = bufferIndex;

            if (byteStride.HasValue)
            {
                bufferView.ByteStride = byteStride;
            }

            return bufferView;
        }

        /// <summary>
        /// Creates an accessor schema object
        /// </summary>
        private glTFLoader.Schema.Accessor CreateAccessor(int bufferviewIndex, int? byteOffset, glTFLoader.Schema.Accessor.ComponentTypeEnum? componentType, int? count, string name, float[] max, float[] min, glTFLoader.Schema.Accessor.TypeEnum? type, bool? normalized)
        {
            var accessor = CreateInstance<glTFLoader.Schema.Accessor>();

            accessor.BufferView = bufferviewIndex;
            accessor.Name = name;

            if (min != null && min.Any())
            {
                accessor.Min = min;
            }

            if (max != null && max.Any())
            {
                accessor.Max = max;
            }

            if (componentType.HasValue)
            {
                accessor.ComponentType = componentType.Value;
            }

            if (byteOffset.HasValue)
            {
                accessor.ByteOffset = byteOffset.Value;
            }

            if (count.HasValue)
            {
                accessor.Count = count.Value;
            }

            if (type.HasValue)
            {
                accessor.Type = type.Value;
            }

            if (normalized.HasValue && normalized.Value == true)
            {
                accessor.Normalized = normalized.Value;
            }

            return accessor;
        }

        /// <summary>
        /// Converts runtime asset to Schema.
        /// </summary>
        private glTFLoader.Schema.Asset ConvertAssetToSchema(Asset runtimeAsset)
        {
            var extras = CreateInstance<glTFLoader.Schema.Extras>();
            var schemaAsset = CreateInstance<glTFLoader.Schema.Asset>();

            if (runtimeAsset.Generator != null)
            {
                schemaAsset.Generator = runtimeAsset.Generator;
            }

            if (runtimeAsset.Version != null)
            {
                schemaAsset.Version = runtimeAsset.Version;
            }

            if (runtimeAsset.Extras != null)
            {
                schemaAsset.Extras = runtimeAsset.Extras;
            }

            if (runtimeAsset.Copyright != null)
            {
                schemaAsset.Copyright = runtimeAsset.Copyright;
            }

            if (runtimeAsset.MinVersion != null)
            {
                schemaAsset.MinVersion = runtimeAsset.MinVersion;
            }

            return schemaAsset;
        }

        /// <summary>
        /// Converts the morph target list of dictionaries into Morph Target
        /// </summary>
        private IEnumerable<Dictionary<string, int>> GetMeshPrimitiveMorphTargets(MeshPrimitive meshPrimitive, List<float> weights, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var morphTargetDicts = new List<Dictionary<string, int>>();
            if (meshPrimitive.MorphTargets != null)
            {
                foreach (MeshPrimitive morphTarget in meshPrimitive.MorphTargets)
                {
                    var morphTargetAttributes = new Dictionary<string, int>();

                    if (morphTarget.Positions != null && morphTarget.Positions.Any())
                    {
                        if (morphTarget.Positions != null)
                        {
                            //Create BufferView for the position
                            int byteLength = sizeof(float) * 3 * morphTarget.Positions.Count();
                            int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                            var bufferView = CreateBufferView(bufferIndex, "Positions", byteLength, byteOffset, null);

                            bufferViews.Add(bufferView);
                            int bufferviewIndex = bufferViews.Count() - 1;

                            // Create an accessor for the bufferView
                            var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Positions.Count(), "Positions Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);
                            accessors.Add(accessor);
                            geometryData.Writer.Write(morphTarget.Positions.ToArray());
                            morphTargetAttributes.Add("POSITION", accessors.Count() - 1);
                        }
                    }
                    if (morphTarget.Normals != null && morphTarget.Normals.Any())
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Normals.Count();
                        // Create a bufferView
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Normals", byteLength, byteOffset, null);

                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Normals.Count(), "Normals Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                        accessors.Add(accessor);
                        geometryData.Writer.Write(morphTarget.Normals.ToArray());
                        morphTargetAttributes.Add("NORMAL", accessors.Count() - 1);
                    }
                    if (morphTarget.Tangents != null && morphTarget.Tangents.Any())
                    {
                        int byteLength = sizeof(float) * 3 * morphTarget.Tangents.Count();
                        // Create a bufferView
                        int byteOffset = (int)geometryData.Writer.BaseStream.Position;
                        var bufferView = CreateBufferView(bufferIndex, "Tangents", byteLength, byteOffset, null);

                        bufferViews.Add(bufferView);
                        int bufferviewIndex = bufferViews.Count() - 1;

                        // Create an accessor for the bufferView
                        var accessor = CreateAccessor(bufferviewIndex, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, morphTarget.Tangents.Count(), "Tangents Accessor", null, null, glTFLoader.Schema.Accessor.TypeEnum.VEC3, null);

                        accessors.Add(accessor);
                        geometryData.Writer.Write(morphTarget.Tangents.ToArray());
                        morphTargetAttributes.Add("TANGENT", accessors.Count() - 1);
                    }
                    morphTargetDicts.Add(new Dictionary<string, int>(morphTargetAttributes));
                    weights.Add(meshPrimitive.MorphTargetWeight);
                }
            }
            return morphTargetDicts;
        }

        /// <summary>
        /// Converts runtime mesh to schema.
        /// </summary>
        private glTFLoader.Schema.Mesh ConvertMeshToSchema(Node runtimeNode, GLTF gltf, glTFLoader.Schema.Buffer buffer, Data geometryData, int bufferIndex)
        {
            var runtimeMesh = runtimeNode.Mesh;
            var schemaMesh = CreateInstance<glTFLoader.Schema.Mesh>();
            var primitives = new List<glTFLoader.Schema.MeshPrimitive>(runtimeMesh.MeshPrimitives.Count());
            var weights = new List<float>();
            // Loops through each wrapped mesh primitive within the mesh and converts them to mesh primitives, as well as updating the
            // indices in the lists
            foreach (var gPrimitive in runtimeMesh.MeshPrimitives)
            {
                glTFLoader.Schema.MeshPrimitive mPrimitive = ConvertMeshPrimitiveToSchema(runtimeNode, gPrimitive, gltf, buffer, geometryData, bufferIndex);
                if (gPrimitive.MorphTargets != null && gPrimitive.MorphTargets.Any())
                {
                    var morphTargetAttributes = GetMeshPrimitiveMorphTargets(gPrimitive, weights, buffer, geometryData, bufferIndex);
                    mPrimitive.Targets = morphTargetAttributes.ToArray();
                }
                primitives.Add(mPrimitive);
            }
            if (runtimeMesh.Name != null)
            {
                schemaMesh.Name = runtimeMesh.Name;
            }
            if (runtimeMesh.MeshPrimitives != null && primitives.Count > 0)
            {
                schemaMesh.Primitives = primitives.ToArray();
            }
            if (weights.Count > 0)
            {
                schemaMesh.Weights = weights.ToArray();
            }

            return schemaMesh;
        }

        /// <summary>
        /// Converts runtime material to schema.
        /// </summary>
        private glTFLoader.Schema.Material ConvertMaterialToSchema(Material runtimeMaterial, GLTF gltf)
        {
            var material = CreateInstance<glTFLoader.Schema.Material>();

            if (runtimeMaterial.MetallicRoughnessMaterial != null)
            {
                material.PbrMetallicRoughness = CreateInstance<glTFLoader.Schema.MaterialPbrMetallicRoughness>();
                if (runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.HasValue)
                {
                    material.PbrMetallicRoughness.BaseColorFactor = new[]
                    {
                        runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.X,
                        runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.Y,
                        runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.Z,
                        runtimeMaterial.MetallicRoughnessMaterial.BaseColorFactor.Value.W
                    };
                }

                if (runtimeMaterial.MetallicRoughnessMaterial.BaseColorTexture != null)
                {
                    var baseColorIndices = AddTexture(runtimeMaterial.MetallicRoughnessMaterial.BaseColorTexture);

                    material.PbrMetallicRoughness.BaseColorTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                    if (baseColorIndices.ImageIndex.HasValue)
                    {
                        material.PbrMetallicRoughness.BaseColorTexture.Index = baseColorIndices.ImageIndex.Value;
                    }
                    if (baseColorIndices.TextureCoordIndex.HasValue)
                    {
                        material.PbrMetallicRoughness.BaseColorTexture.TexCoord = baseColorIndices.TextureCoordIndex.Value;
                    };
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.MetallicRoughnessTexture != null)
                {
                    var metallicRoughnessIndices = AddTexture(runtimeMaterial.MetallicRoughnessMaterial.MetallicRoughnessTexture);

                    material.PbrMetallicRoughness.MetallicRoughnessTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                    if (metallicRoughnessIndices.ImageIndex.HasValue)
                    {
                        material.PbrMetallicRoughness.MetallicRoughnessTexture.Index = metallicRoughnessIndices.ImageIndex.Value;
                    }
                    if (metallicRoughnessIndices.TextureCoordIndex.HasValue)
                    {
                        material.PbrMetallicRoughness.MetallicRoughnessTexture.TexCoord = metallicRoughnessIndices.TextureCoordIndex.Value;
                    }
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.MetallicFactor.HasValue)
                {
                    material.PbrMetallicRoughness.MetallicFactor = runtimeMaterial.MetallicRoughnessMaterial.MetallicFactor.Value;
                }
                if (runtimeMaterial.MetallicRoughnessMaterial.RoughnessFactor.HasValue)
                {
                    material.PbrMetallicRoughness.RoughnessFactor = runtimeMaterial.MetallicRoughnessMaterial.RoughnessFactor.Value;
                }
            }
            if (runtimeMaterial.EmissiveFactor != null)
            {
                material.EmissiveFactor = new[]
                {
                    runtimeMaterial.EmissiveFactor.Value.X,
                    runtimeMaterial.EmissiveFactor.Value.Y,
                    runtimeMaterial.EmissiveFactor.Value.Z
                };
            }
            if (runtimeMaterial.NormalTexture != null)
            {
                var normalIndicies = AddTexture(runtimeMaterial.NormalTexture);
                material.NormalTexture = CreateInstance<glTFLoader.Schema.MaterialNormalTextureInfo>();

                if (normalIndicies.ImageIndex.HasValue)
                {
                    material.NormalTexture.Index = normalIndicies.ImageIndex.Value;

                }
                if (normalIndicies.TextureCoordIndex.HasValue)
                {
                    material.NormalTexture.TexCoord = normalIndicies.TextureCoordIndex.Value;
                }
                if (runtimeMaterial.NormalScale.HasValue)
                {
                    material.NormalTexture.Scale = runtimeMaterial.NormalScale.Value;
                }
            }
            if (runtimeMaterial.OcclusionTexture != null)
            {
                var occlusionIndicies = AddTexture(runtimeMaterial.OcclusionTexture);
                material.OcclusionTexture = CreateInstance<glTFLoader.Schema.MaterialOcclusionTextureInfo>();
                if (occlusionIndicies.ImageIndex.HasValue)
                {
                    material.OcclusionTexture.Index = occlusionIndicies.ImageIndex.Value;
                };
                if (occlusionIndicies.TextureCoordIndex.HasValue)
                {
                    material.OcclusionTexture.TexCoord = occlusionIndicies.TextureCoordIndex.Value;
                }
                if (runtimeMaterial.OcclusionStrength.HasValue)
                {
                    material.OcclusionTexture.Strength = runtimeMaterial.OcclusionStrength.Value;
                }
            }
            if (runtimeMaterial.EmissiveTexture != null)
            {
                var emissiveIndicies = AddTexture(runtimeMaterial.EmissiveTexture);
                material.EmissiveTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (emissiveIndicies.ImageIndex.HasValue)
                {
                    material.EmissiveTexture.Index = emissiveIndicies.ImageIndex.Value;
                }
                if (emissiveIndicies.TextureCoordIndex.HasValue)
                {
                    material.EmissiveTexture.TexCoord = emissiveIndicies.TextureCoordIndex.Value;
                }
            }
            if (runtimeMaterial.AlphaMode.HasValue)
            {
                material.AlphaMode = runtimeMaterial.AlphaMode.Value;
            }
            if (runtimeMaterial.AlphaCutoff.HasValue)
            {
                material.AlphaCutoff = runtimeMaterial.AlphaCutoff.Value;
            }
            if (runtimeMaterial.Name != null)
            {
                material.Name = runtimeMaterial.Name;
            }
            if (runtimeMaterial.DoubleSided.HasValue)
            {
                material.DoubleSided = runtimeMaterial.DoubleSided.Value;
            }
            if (runtimeMaterial.Extensions != null)
            {
                var extensionsUsed = new List<string>();
                if (material.Extensions == null)
                {
                    material.Extensions = new Dictionary<string, object>();
                }
                if (gltf.ExtensionsUsed == null)
                {
                    gltf.ExtensionsUsed = new List<string>();
                }
                foreach (var runtimeExtension in runtimeMaterial.Extensions)
                {
                    object extension;
                    switch (runtimeExtension.Name)
                    {
                        case nameof(Extensions.KHR_materials_pbrSpecularGlossiness):
                            extension = ConvertPbrSpecularGlossinessExtensionToSchema((Extensions.KHR_materials_pbrSpecularGlossiness)runtimeExtension, gltf);
                            break;
                        case nameof(Extensions.FAKE_materials_quantumRendering):
                            extension = ConvertExtQuantumRenderingToSchema((Extensions.FAKE_materials_quantumRendering)runtimeExtension, gltf);
                            break;
                        default:
                            throw new NotImplementedException("Extension schema conversion not implemented for " + runtimeExtension.Name);
                    }

                    material.Extensions.Add(runtimeExtension.Name, extension);

                    if (!extensionsUsed.Contains(runtimeExtension.Name))
                    {
                        extensionsUsed.Add(runtimeExtension.Name);
                    }
                }
                gltf.ExtensionsUsed = extensionsUsed;
            }

            return material;
        }

        /// <summary>
        /// Converts Runtime PbrSpecularGlossiness to Schema.
        /// </summary>
        private glTFLoader.Schema.MaterialPbrSpecularGlossiness ConvertPbrSpecularGlossinessExtensionToSchema(Extensions.KHR_materials_pbrSpecularGlossiness specGloss, GLTF gltf)
        {
            var materialPbrSpecularGlossiness = CreateInstance<glTFLoader.Schema.MaterialPbrSpecularGlossiness>();

            if (specGloss.DiffuseFactor.HasValue)
            {
                materialPbrSpecularGlossiness.DiffuseFactor = specGloss.DiffuseFactor.Value.ToArray();
            }
            if (specGloss.DiffuseTexture != null)
            {
                TextureIndices textureIndices = AddTexture(specGloss.DiffuseTexture);
                materialPbrSpecularGlossiness.DiffuseTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (textureIndices.ImageIndex.HasValue)
                {
                    materialPbrSpecularGlossiness.DiffuseTexture.Index = textureIndices.ImageIndex.Value;
                }
                if (textureIndices.TextureCoordIndex.HasValue)
                {
                    materialPbrSpecularGlossiness.DiffuseTexture.TexCoord = textureIndices.TextureCoordIndex.Value;
                }
            }
            if (specGloss.SpecularFactor.HasValue)
            {
                materialPbrSpecularGlossiness.SpecularFactor = specGloss.SpecularFactor.Value.ToArray();
            }
            if (specGloss.GlossinessFactor.HasValue)
            {
                materialPbrSpecularGlossiness.GlossinessFactor = specGloss.GlossinessFactor.Value;
            }
            if (specGloss.SpecularGlossinessTexture != null)
            {
                TextureIndices textureIndices = AddTexture(specGloss.SpecularGlossinessTexture);
                materialPbrSpecularGlossiness.SpecularGlossinessTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (textureIndices.ImageIndex.HasValue)
                {
                    materialPbrSpecularGlossiness.SpecularGlossinessTexture.Index = textureIndices.ImageIndex.Value;
                }
                if (textureIndices.TextureCoordIndex.HasValue)
                {
                    materialPbrSpecularGlossiness.SpecularGlossinessTexture.TexCoord = textureIndices.TextureCoordIndex.Value;
                }
            }
            if (specGloss.GlossinessFactor.HasValue)
            {
                materialPbrSpecularGlossiness.GlossinessFactor = specGloss.GlossinessFactor.Value;
            }

            return materialPbrSpecularGlossiness;
        }

        /// <summary>
        /// Converts Runtime Quantum Rendering to Schema (Not an actual glTF feature) 
        /// </summary>
        private glTFLoader.Schema.FAKE_materials_quantumRendering ConvertExtQuantumRenderingToSchema(Extensions.FAKE_materials_quantumRendering quantumRendering, GLTF gltf)
        {
            var materialEXT_QuantumRendering = CreateInstance<glTFLoader.Schema.FAKE_materials_quantumRendering>();

            if (quantumRendering.PlanckFactor.HasValue)
            {
                materialEXT_QuantumRendering.PlanckFactor = quantumRendering.PlanckFactor.Value.ToArray();
            }
            if (quantumRendering.CopenhagenTexture != null)
            {
                TextureIndices textureIndices = AddTexture(quantumRendering.CopenhagenTexture);
                materialEXT_QuantumRendering.CopenhagenTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (textureIndices.ImageIndex.HasValue)
                {
                    materialEXT_QuantumRendering.CopenhagenTexture.Index = textureIndices.ImageIndex.Value;
                }
                if (textureIndices.TextureCoordIndex.HasValue)
                {
                    materialEXT_QuantumRendering.CopenhagenTexture.TexCoord = textureIndices.TextureCoordIndex.Value;
                }
            }
            if (quantumRendering.EntanglementFactor.HasValue)
            {
                materialEXT_QuantumRendering.EntanglementFactor = quantumRendering.EntanglementFactor.Value.ToArray();
            }
            if (quantumRendering.ProbabilisticFactor.HasValue)
            {
                materialEXT_QuantumRendering.ProbabilisticFactor = quantumRendering.ProbabilisticFactor.Value;
            }
            if (quantumRendering.SuperpositionCollapseTexture != null)
            {
                TextureIndices textureIndices = AddTexture(quantumRendering.SuperpositionCollapseTexture);
                materialEXT_QuantumRendering.SuperpositionCollapseTexture = CreateInstance<glTFLoader.Schema.TextureInfo>();
                if (textureIndices.ImageIndex.HasValue)
                {
                    materialEXT_QuantumRendering.SuperpositionCollapseTexture.Index = textureIndices.ImageIndex.Value;
                }
                if (textureIndices.TextureCoordIndex.HasValue)
                {
                    materialEXT_QuantumRendering.SuperpositionCollapseTexture.TexCoord = textureIndices.TextureCoordIndex.Value;
                }
            }
            if (quantumRendering.ProbabilisticFactor.HasValue)
            {
                materialEXT_QuantumRendering.ProbabilisticFactor = quantumRendering.ProbabilisticFactor.Value;
            }

            return materialEXT_QuantumRendering;
        }

        /// <summary>
        /// Converts runtime animation to schema.
        /// </summary>
        private glTFLoader.Schema.Animation ConvertAnimationToSchema(Animation runtimeAnimation, GLTF gltf, Data geometryData, int bufferIndex)
        {
            var animation = CreateInstance<glTFLoader.Schema.Animation>();
            var animationChannels = new List<glTFLoader.Schema.AnimationChannel>();
            var animationSamplers = new List<glTFLoader.Schema.AnimationSampler>();

            foreach (var runtimeAnimationChannel in runtimeAnimation.Channels)
            {
                var animationChannel = new glTFLoader.Schema.AnimationChannel();
                var targetNode = runtimeAnimationChannel.Target.Node;
                var sceneIndex = 0;
                if (gltf.Scene.HasValue)
                {
                    sceneIndex = gltf.Scene.Value;
                }

                var targetNodeIndex = gltf.Scenes.ElementAt(sceneIndex).Nodes.IndexOf(targetNode);
                var runtimeSampler = runtimeAnimationChannel.Sampler;

                // Create Animation Channel

                // Write Input Key frames
                var inputBufferView = CreateBufferView(bufferIndex, "Animation Sampler Input", runtimeSampler.InputKeys.Count() * 4, (int)geometryData.Writer.BaseStream.Position, null);
                bufferViews.Add(inputBufferView);

                geometryData.Writer.Write(runtimeSampler.InputKeys);

                var min = new[] { runtimeSampler.InputKeys.Min() };
                var max = new[] { runtimeSampler.InputKeys.Max() };
                var inputAccessor = CreateAccessor(bufferViews.Count - 1, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, runtimeSampler.InputKeys.Count(), "Animation Sampler Input", max, min, glTFLoader.Schema.Accessor.TypeEnum.SCALAR, null);
                accessors.Add(inputAccessor);

                var inputAccessorIndex = accessors.Count - 1;

                animationChannel.Target = new glTFLoader.Schema.AnimationChannelTarget
                {
                    Node = targetNodeIndex
                };

                switch (runtimeAnimationChannel.Target.Path)
                {
                    case AnimationChannelTarget.PathEnum.TRANSLATION:
                        animationChannel.Target.Path = glTFLoader.Schema.AnimationChannelTarget.PathEnum.translation;
                        break;
                    case AnimationChannelTarget.PathEnum.ROTATION:
                        animationChannel.Target.Path = glTFLoader.Schema.AnimationChannelTarget.PathEnum.rotation;
                        break;
                    case AnimationChannelTarget.PathEnum.SCALE:
                        animationChannel.Target.Path = glTFLoader.Schema.AnimationChannelTarget.PathEnum.scale;
                        break;
                    case AnimationChannelTarget.PathEnum.WEIGHT:
                        animationChannel.Target.Path = glTFLoader.Schema.AnimationChannelTarget.PathEnum.weights;
                        break;
                    default:
                        throw new NotSupportedException($"Animation target path {runtimeAnimationChannel.Target.Path} not supported!");
                }

                // Write the output key frame data
                var outputByteOffset = (int)geometryData.Writer.BaseStream.Position;

                var runtimeSamplerType = runtimeSampler.GetType();
                var runtimeSamplerGenericTypeDefinition = runtimeSamplerType.GetGenericTypeDefinition();
                var runtimeSamplerGenericTypeArgument = runtimeSamplerType.GenericTypeArguments[0];

                glTFLoader.Schema.Accessor.TypeEnum outputAccessorType;
                if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                {
                    outputAccessorType = glTFLoader.Schema.Accessor.TypeEnum.VEC3;
                }
                else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                {
                    outputAccessorType = glTFLoader.Schema.Accessor.TypeEnum.VEC4;
                }
                else
                {
                    throw new ArgumentException("Unsupported animation accessor type!");
                }

                var outputAccessorComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;

                glTFLoader.Schema.AnimationSampler.InterpolationEnum samplerInterpolation;
                if (runtimeSamplerGenericTypeDefinition == typeof(StepAnimationSampler<>))
                {
                    samplerInterpolation = glTFLoader.Schema.AnimationSampler.InterpolationEnum.STEP;

                    if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                    {
                        var specificRuntimeSampler = (StepAnimationSampler<Vector3>)runtimeSampler;
                        geometryData.Writer.Write(specificRuntimeSampler.OutputKeys);
                    }
                    else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                    {
                        var specificRuntimeSampler = (StepAnimationSampler<Quaternion>)runtimeSampler;
                        geometryData.Writer.Write(specificRuntimeSampler.OutputKeys);
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported animation sampler component type!");
                    }
                }
                else if (runtimeSamplerGenericTypeDefinition == typeof(LinearAnimationSampler<>))
                {
                    samplerInterpolation = glTFLoader.Schema.AnimationSampler.InterpolationEnum.LINEAR;

                    if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                    {
                        var specificRuntimeSampler = (LinearAnimationSampler<Vector3>)runtimeSampler;
                        geometryData.Writer.Write(specificRuntimeSampler.OutputKeys);
                    }
                    else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                    {
                        var specificRuntimeSampler = (LinearAnimationSampler<Quaternion>)runtimeSampler;
                        geometryData.Writer.Write(specificRuntimeSampler.OutputKeys);
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported animation sampler type!");
                    }
                }
                else if (runtimeSamplerGenericTypeDefinition == typeof(CubicSplineAnimationSampler<>))
                {
                    samplerInterpolation = glTFLoader.Schema.AnimationSampler.InterpolationEnum.CUBICSPLINE;

                    if (runtimeSamplerGenericTypeArgument == typeof(Vector3))
                    {
                        var specificRuntimeSampler = (CubicSplineAnimationSampler<Vector3>)runtimeSampler;
                        specificRuntimeSampler.OutputKeys.ForEach(key =>
                        {
                            geometryData.Writer.Write(key.InTangent);
                            geometryData.Writer.Write(key.Value);
                            geometryData.Writer.Write(key.OutTangent);
                        });
                    }
                    else if (runtimeSamplerGenericTypeArgument == typeof(Quaternion))
                    {
                        var specificRuntimeSampler = (CubicSplineAnimationSampler<Quaternion>)runtimeSampler;
                        specificRuntimeSampler.OutputKeys.ForEach(key =>
                        {
                            geometryData.Writer.Write(key.InTangent);
                            geometryData.Writer.Write(key.Value);
                            geometryData.Writer.Write(key.OutTangent);
                        });
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                var outputCount = samplerInterpolation == glTFLoader.Schema.AnimationSampler.InterpolationEnum.CUBICSPLINE ? inputAccessor.Count * 3 : inputAccessor.Count;
                var outputByteLength = (int)geometryData.Writer.BaseStream.Position - outputByteOffset;
                var outputBufferView = CreateBufferView(bufferIndex, "Animation Sampler Output", outputByteLength, outputByteOffset, null);
                bufferViews.Add(outputBufferView);

                var outputAccessor = CreateAccessor(bufferViews.Count - 1, 0, outputAccessorComponentType, outputCount, "Animation Sampler Output", null, null, outputAccessorType, null);
                accessors.Add(outputAccessor);
                var outputAccessorIndex = accessors.Count - 1;

                // Create Animation Sampler
                var animationSampler = new glTFLoader.Schema.AnimationSampler
                {
                    Interpolation = samplerInterpolation,
                    Input = inputAccessorIndex,
                    Output = outputAccessorIndex
                };

                animationChannels.Add(animationChannel);
                animationSamplers.Add(animationSampler);

                // This needs to be improved to support instancing
                animationChannel.Sampler = animationSamplers.Count() - 1;
            }

            animation.Channels = animationChannels.ToArray();
            animation.Samplers = animationSamplers.ToArray();

            return animation;
        }

        /// <summary>
        /// Computes and returns the minimum and maximum positions for the mesh primitive.
        /// </summary>
        /// <returns>Returns the result as an array of two vectors, minimum and maximum respectively</returns>
        private Vector3[] GetMinMaxPositions(MeshPrimitive meshPrimitive)
        {

            //get the max and min values
            Vector3 minVal = new Vector3
            {
                X = float.MaxValue,
                Y = float.MaxValue,
                Z = float.MaxValue
            };
            Vector3 maxVal = new Vector3
            {
                X = float.MinValue,
                Y = float.MinValue,
                Z = float.MinValue
            };
            foreach (Vector3 position in meshPrimitive.Positions)
            {
                maxVal.X = Math.Max(position.X, maxVal.X);
                maxVal.Y = Math.Max(position.Y, maxVal.Y);
                maxVal.Z = Math.Max(position.Z, maxVal.Z);

                minVal.X = Math.Min(position.X, minVal.X);
                minVal.Y = Math.Min(position.Y, minVal.Y);
                minVal.Z = Math.Min(position.Z, minVal.Z);
            }
            Vector3[] results = { minVal, maxVal };
            return results;
        }
    }
}
