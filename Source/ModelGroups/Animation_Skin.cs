using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssetGenerator
{
    internal class Animation_Skin : ModelGroup
    {
        public override ModelGroupId Id => ModelGroupId.Animation_Skin;

        public Animation_Skin(List<string> imageList)
        {
            // There are no common properties in this model group that are reported in the readme.

            Model CreateModel(Action<List<Property>, Runtime.GLTF, IEnumerable<Runtime.Node>> setProperties)
            {
                var properties = new List<Property>();
              //  var planeSkinScene = Scene.CreatePlaneWithSkin();
                var planeSkinScene = Scene.CreatePlaneWith5Joints();

                // Apply the common properties to the gltf.


                // Create the gltf object
                Runtime.GLTF gltf = CreateGLTF(() => planeSkinScene);

                // Apply the properties that are specific to this gltf.
                setProperties(properties, gltf, planeSkinScene.Nodes);

                return new Model
                {
                    Properties = properties,
                    GLTF = gltf
                };
            }
            

            void AnimateWithRotation(Runtime.GLTF gltf)
            {
                var rootJoint = gltf.Scenes.First().Nodes.ElementAt(1);
                var rootMidJoint = rootJoint.Children.First();
                var midJoint = rootMidJoint.Children.First();
                var midTopJoint = midJoint.Children.First();
                var topJoint = midTopJoint.Children.First();
                gltf.Animations = new List<Runtime.Animation>
                {
                    new Runtime.Animation
                    {
                        Channels = new List<Runtime.AnimationChannel>
                        {
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = rootMidJoint,
                                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                                }
                            },
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = midJoint,
                                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                                }
                            },
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = midTopJoint,
                                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                                }
                            },
                            new Runtime.AnimationChannel
                            {
                                Target = new Runtime.AnimationChannelTarget
                                {
                                    Node = topJoint,
                                    Path = Runtime.AnimationChannelTarget.PathEnum.ROTATION,
                                }
                            }
                        }
                    }
                };
                var quarterTurn = (FloatMath.Pi / 2);
                var keyFrames = new[]
                {
                    0.0f,
                    1.0f,
                    2.0f,
                };
                var keyFrameValues45Degrees = new[]
                {
                    Quaternion.Identity,
                    Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn/2, 0.0f),
                    Quaternion.Identity,
                };
                var keyFrameValues90Degrees = new[]
                {
                    Quaternion.Identity,
                    Quaternion.CreateFromYawPitchRoll(0.0f, quarterTurn, 0.0f),
                    Quaternion.Identity,
                };
                var keyFrameValuesNeg90Degrees = new[]
                {
                    Quaternion.Identity,
                    Quaternion.CreateFromYawPitchRoll(0.0f, -quarterTurn, 0.0f),
                    Quaternion.Identity,
                };

                var keyFrameValuesNeutral = new[]
                {
                    Quaternion.Identity,
                    Quaternion.Identity,
                    Quaternion.Identity,
                };

                gltf.Animations.First().Channels.First().Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    keyFrames,
                    keyFrameValues45Degrees
                );
                gltf.Animations.First().Channels.ElementAt(1).Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    keyFrames,
                    keyFrameValuesNeutral
                );
                gltf.Animations.First().Channels.ElementAt(2).Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    keyFrames,
                    keyFrameValuesNeutral
                );
                gltf.Animations.First().Channels.ElementAt(3).Sampler = new Runtime.LinearAnimationSampler<Quaternion>(
                    keyFrames,
                    keyFrameValuesNeutral
                );
            }

            this.Models = new List<Model>
            {
                CreateModel((properties, gltf, nodes) => {
                    properties.Add(new Property(PropertyName.Description, "Skin with two joints."));
                }),
                CreateModel((properties, gltf, nodes) => {
                    AnimateWithRotation(gltf);
                    properties.Add(new Property(PropertyName.Description, "Skin with two joints, one of which is animated with a rotation."));
                }),
            };

            GenerateUsedPropertiesList();
        }
    }
}
