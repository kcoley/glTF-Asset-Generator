using AssetGenerator.Runtime.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetGenerator.Runtime.GLTFConverter
{
    internal partial class GLTFConverter
    {
        /// <summary>
        /// Converts runtime node to schema.
        /// </summary>
        private int ConvertNodeToSchema(Node runtimeNode)
        {
            if (this.nodeToIndexCache.TryGetValue(runtimeNode, out int nodeIndex))
            {
                return nodeIndex;
            }

            var node = CreateInstance<glTFLoader.Schema.Node>();
            nodes.Add(node);
            nodeIndex = nodes.Count() - 1;
            if (runtimeNode.Name != null)
            {
                node.Name = runtimeNode.Name;
            }
            if (runtimeNode.Matrix.HasValue)
            {
                node.Matrix = runtimeNode.Matrix.Value.ToArray();
            }
            if (runtimeNode.Mesh != null)
            {
                var schemaMesh = ConvertMeshToSchema(runtimeNode, runtimeGLTF, buffer, geometryData, bufferIndex);
                meshes.Add(schemaMesh);
                node.Mesh = meshes.Count() - 1;
            }
            if (runtimeNode.Rotation.HasValue)
            {
                node.Rotation = runtimeNode.Rotation.Value.ToArray();
            }
            if (runtimeNode.Scale.HasValue)
            {
                node.Scale = runtimeNode.Scale.Value.ToArray();
            }
            if (runtimeNode.Translation.HasValue)
            {
                node.Translation = runtimeNode.Translation.Value.ToArray();
            }

            if (runtimeNode.Children != null)
            {
                var childrenIndices = new List<int>();
                foreach (var childNode in runtimeNode.Children)
                {
                    var schemaChildIndex = ConvertNodeToSchema(childNode);
                    childrenIndices.Add(schemaChildIndex);
                }
                node.Children = childrenIndices.ToArray();
            }
            if (runtimeNode.Skin != null && runtimeNode.Skin.SkinJoints != null && runtimeNode.Skin.SkinJoints.Any())
            {
                var inverseBindMatrices = runtimeNode.Skin.SkinJoints.Select(skinJoint =>
                    skinJoint.InverseBindMatrix
                );

                int? inverseBindMatricesAccessorIndex = null;
                if (inverseBindMatrices.Where(inverseBindMatrix => !inverseBindMatrix.IsIdentity).Any())
                {
                    int inverseBindMatricesByteOffset = (int)geometryData.Writer.BaseStream.Position;
                    geometryData.Writer.Write(inverseBindMatrices);
                    int inverseBindMatricesByteLength = (int)geometryData.Writer.BaseStream.Position - inverseBindMatricesByteOffset;

                    // create bufferview
                    var inverseBindMatricesBufferView = CreateBufferView(bufferIndex, "Inverse Bind Matrix", inverseBindMatricesByteLength, inverseBindMatricesByteOffset, null);
                    bufferViews.Add(inverseBindMatricesBufferView);

                    // create accessor
                    var inverseBindMatricesAccessor = CreateAccessor(bufferViews.Count() - 1, 0, glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT, inverseBindMatrices.Count(), "IBM", null, null, glTFLoader.Schema.Accessor.TypeEnum.MAT4, null);
                    accessors.Add(inverseBindMatricesAccessor);
                    inverseBindMatricesAccessorIndex = accessors.Count() - 1;
                }

                var jointIndices = runtimeNode.Skin.SkinJoints.Select(SkinJoint => ConvertNodeToSchema(SkinJoint.Node));

                var skin = new glTFLoader.Schema.Skin
                {
                    Joints = jointIndices.ToArray(),
                    InverseBindMatrices = inverseBindMatricesAccessorIndex,
                };
                skins.Add(skin);
                node.Skin = skins.Count() - 1;

            }
            nodeToIndexCache.Add(runtimeNode, nodeIndex);

            return nodeIndex;
        }
    }
}
