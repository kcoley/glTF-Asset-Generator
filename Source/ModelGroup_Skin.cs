using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace AssetGenerator
{
    internal abstract partial class ModelGroup
    {
        protected static partial class Scene
        {
            public static Runtime.Scene CreatePlaneWithSkin()
            {
                Runtime.Scene scene = new Runtime.Scene
                {
                    Nodes = new[]
                    {
                        new Runtime.Node
                        {
                            Skin = new Runtime.Skin(),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = new[]
                                {
                                    new Runtime.MeshPrimitive
                                    {
                                        Material = new Runtime.Material
                                        {
                                            DoubleSided = true
                                        },
                                        Positions = new List<Vector3>()
                                        {
                                            new Vector3(-0.5f,-0.5f, 0.0f),
                                            new Vector3( 0.5f,-0.5f, 0.0f),
                                            new Vector3(-0.5f, 0.0f, 0.0f),
                                            new Vector3( 0.5f, 0.0f, 0.0f),
                                            new Vector3(-0.5f, 0.5f, 0.0f),
                                            new Vector3( 0.5f, 0.5f, 0.0f),
                                        },
                                        Normals = new List<Vector3>()
                                        {
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                        },
                                        Indices = new List<int>
                                        {
                                            0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5
                                        },
                                        Colors = new List<Vector4>()
                                        {
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                        },
                                    }
                                }
                            },
                        },
                        new Runtime.Node
                        {
                            Name = "rootJoint",
                            Children = new[]
                            {
                                new Runtime.Node
                                {
                                    Name = "midJoint",
                                }
                            },
                        },
                    }
                };

                Matrix4x4 matrix1 = Matrix4x4.Identity;
                Matrix4x4 matrix2 = Matrix4x4.Identity;

                var skinNode = scene.Nodes.First();
                var rootJoint = scene.Nodes.ElementAt(1);
                var midJoint = scene.Nodes.ElementAt(1).Children.First();

                skinNode.Skin.SkinJoints = new[]
                {
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: matrix1,
                        node: rootJoint
                    ),
                    new Runtime.SkinJoint
                    (
                        inverseBindMatrix: matrix2,
                        node: midJoint
                    )
                };

                skinNode.Mesh.MeshPrimitives.First().VertexJointWeights = new[]
                {
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                    },
                    new[]
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                    },
                };

                return scene;
            }
            public static Runtime.Scene CreatePlaneWith5Joints()
            {
                Runtime.Scene scene = new Runtime.Scene
                {
                    Nodes = new[]
                    {
                        new Runtime.Node
                        {
                            Skin = new Runtime.Skin(),
                            Mesh = new Runtime.Mesh
                            {
                                MeshPrimitives = new[]
                                {
                                    new Runtime.MeshPrimitive
                                    {
                                        Positions = new List<Vector3>()
                                        {
                                            new Vector3(-0.5f, 0.0f, 0.0f),
                                            new Vector3( 0.5f, 0.0f, 0.0f),
                                            new Vector3(-0.5f, 0.2f, 0.0f),
                                            new Vector3( 0.5f, 0.2f, 0.0f),
                                            new Vector3(-0.5f, 0.4f, 0.0f),
                                            new Vector3( 0.5f, 0.4f, 0.0f),
                                            new Vector3(-0.5f, 0.6f, 0.0f),
                                            new Vector3( 0.5f, 0.6f, 0.0f),
                                            new Vector3(-0.5f, 0.8f, 0.0f),
                                            new Vector3( 0.5f, 0.8f, 0.0f),
                                            new Vector3(-0.5f, 1.0f, 0.0f),
                                            new Vector3( 0.5f, 1.0f, 0.0f),
                                        },
                                        Normals = new List<Vector3>()
                                        {
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                            new Vector3( 0.0f,  0.0f,  1.0f),
                                        },
                                        Indices = new List<int>
                                        {
                                            0,
                                            1,
                                            2,
                                            2,
                                            1,
                                            3,
                                            2,
                                            3,
                                            4,
                                            4,
                                            3,
                                            5,
                                            4,
                                            5,
                                            6,
                                            6,
                                            5,
                                            7,
                                            6,
                                            7,
                                            8,
                                            8,
                                            7,
                                            9,
                                            8,
                                            9,
                                            10,
                                            10,
                                            9,
                                            11,
                                        },
                                        Colors = new List<Vector4>()
                                        {
                                            new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                            new Vector4(0.2f, 0.2f, 0.2f, 0.2f),
                                        },
                                    }
                                }
                            },
                        },
                        new Runtime.Node
                        {
                            Name = "rootJoint",
                            Children = new[]
                            {
                                new Runtime.Node
                                {
                                    Name = "rootMidJoint",
                                    Children = new []
                                    {
                                        new Runtime.Node
                                        {
                                            Name = "midJoint",
                                            Children = new []
                                            {
                                                new Runtime.Node
                                                {
                                                    Name = "midTopJoint",
                                                    Children = new []
                                                    {
                                                        new Runtime.Node
                                                        {
                                                            Name = "topJoint",
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            },
                        },
                    }
                };

            Matrix4x4 identityMatrix = Matrix4x4.Identity;

            var skinNode = scene.Nodes.First();
            var rootJoint = scene.Nodes.ElementAt(1);
            var rootMidJoint = rootJoint.Children.First();
            var midJoint = rootMidJoint.Children.First();
            var midTopJoint = midJoint.Children.First();
            var topJoint = midTopJoint.Children.First();

            skinNode.Skin.SkinJoints = new[]
            {
                new Runtime.SkinJoint
                (
                    inverseBindMatrix: identityMatrix,
                    node: rootJoint
                ),
                new Runtime.SkinJoint
                (
                    inverseBindMatrix: identityMatrix,
                    node: rootMidJoint
                ),
                new Runtime.SkinJoint
                (
                    inverseBindMatrix: identityMatrix,
                    node: midJoint
                ),
                new Runtime.SkinJoint
                (
                    inverseBindMatrix: identityMatrix,
                    node: midTopJoint
                ),
                new Runtime.SkinJoint
                (
                    inverseBindMatrix: identityMatrix,
                    node: topJoint
                )
            };

        skinNode.Mesh.MeshPrimitives.First().VertexJointWeights = new[]
                {
                    new[] // vertex 1
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0,
                        },
                    },
                    new[] // vertex 2
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0,
                        },
                    },
                    new[] // vertex 3
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 1.0f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.0f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0,
                        },
                    },
                    new[] // vertex 4
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 1.0f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.0f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0,
                        },
                    },
                    new[] // vertex 5
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0,
                        },
                    },
                    new[] // vertex 6
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 1,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0,
                        },
                    },
                    new[] // vertex 7
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.5f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0.5f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0,
                        },
                    },
                    new[] // vertex 8
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0.5f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0.5f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0,
                        },
                    },
                    new[] // vertex 9
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0.4f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0.6f,
                        },
                    },
                    new[] // vertex 10
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0.4f,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 0.6f,
                        },
                    },
                    new[] // vertex 11
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 1,
                        },
                    },
                    new[] // vertex 12
                    {
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.First(),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(1),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(2),
                            Weight = 0,
                        },
                        new Runtime.JointWeight
                        {
                            Joint = skinNode.Skin.SkinJoints.ElementAt(3),
                            Weight = 1,
                        },
                    },
                };

                return scene;
            }
        }
    }
}

