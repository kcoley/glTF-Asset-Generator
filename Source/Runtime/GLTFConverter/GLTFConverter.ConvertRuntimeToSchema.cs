using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        /// <summary>
        /// Converts Runtime GLTF to Schema GLTF object.
        /// </summary>
        public glTFLoader.Schema.Gltf ConvertRuntimeToSchema(GLTF runtimeGLTF, Data geometryData)
        {
            this.runtimeGLTF = runtimeGLTF;
            this.geometryData = geometryData;
            var gltf = this.CreateInstance<glTFLoader.Schema.Gltf>();

            if (runtimeGLTF.Asset != null)
            {
                gltf.Asset = ConvertAssetToSchema(runtimeGLTF.Asset);
            }

            this.buffer = CreateInstance<glTFLoader.Schema.Buffer>();
            this.buffer.Uri = geometryData.Name;

            // for each scene, create a node for each mesh and compute the indices for the scene object
            foreach (var runtimeScene in runtimeGLTF.Scenes)
            {
                var sceneIndicesSet = new List<int>();
                // loops through each mesh and converts it into a Node, with optional transformation info if available
                foreach (var node in runtimeScene.Nodes)
                {
                    sceneIndicesSet.Add(ConvertNodeToSchema(node));
                }

                var scene = CreateInstance<glTFLoader.Schema.Scene>();
                scene.Nodes = sceneIndicesSet.ToArray();
                scenes.Add(scene);
            }
            if (scenes != null && scenes.Any())
            {
                gltf.Scenes = scenes.ToArray();
                gltf.Scene = 0;
            }
            if (runtimeGLTF.Animations != null && runtimeGLTF.Animations.Any())
            {
                var animations = new List<glTFLoader.Schema.Animation>();
                foreach (var runtimeAnimation in runtimeGLTF.Animations)
                {
                    var animation = ConvertAnimationToSchema(runtimeAnimation, runtimeGLTF, geometryData, bufferIndex: 0);
                    animations.Add(animation);
                }
                gltf.Animations = animations.ToArray();
            }

            if (meshes != null && meshes.Any())
            {
                gltf.Meshes = meshes.ToArray();
            }
            if (materials != null && materials.Any())
            {
                gltf.Materials = materials.ToArray();
            }
            if (accessors != null && accessors.Any())
            {
                gltf.Accessors = accessors.ToArray();
            }
            if (bufferViews != null && bufferViews.Any())
            {
                gltf.BufferViews = bufferViews.ToArray();
            }

            gltf.Buffers = new[] { buffer };
            if (nodes != null && nodes.Any())
            {
                gltf.Nodes = nodes.ToArray();
            }

            if (images.Any())
            {
                gltf.Images = images.ToArray();

            }
            if (textures.Any())
            {
                gltf.Textures = textures.ToArray();
            }
            if (skins.Any())
            {
                gltf.Skins = skins.ToArray();
            }
            if (samplers.Any())
            {
                gltf.Samplers = samplers.ToArray();
            }
            if (animations.Any())
            {
                gltf.Animations = animations.ToArray();
            }
            if (runtimeGLTF.Scene.HasValue)
            {
                gltf.Scene = runtimeGLTF.Scene.Value;
            }
            if (runtimeGLTF.ExtensionsUsed != null && runtimeGLTF.ExtensionsUsed.Any())
            {
                gltf.ExtensionsUsed = runtimeGLTF.ExtensionsUsed.ToArray();
            }
            if (runtimeGLTF.ExtensionsRequired != null && runtimeGLTF.ExtensionsRequired.Any())
            {
                gltf.ExtensionsRequired = runtimeGLTF.ExtensionsRequired.ToArray();
            }
            buffer.ByteLength = (int)geometryData.Writer.BaseStream.Position;

            return gltf;
        }
    }
}
