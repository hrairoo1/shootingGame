using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Utility : MonoBehaviour
{
    // Start is called before the first frame update
    public static void SetBoneConstraint(Transform charaBone, Transform armorBone)
    {
        Dictionary<string, Transform> sourceBones = GetBoneDictionary(charaBone);
        Dictionary<string, Transform> targetBones = GetBoneDictionary(armorBone);

        if (targetBones != null)
        {
            // 対応するボーンに RotateConstraint を適用
            foreach (var pair in targetBones)
            {
                string boneName = pair.Key;
                Transform targetBone = pair.Value;
                if (sourceBones.TryGetValue(boneName, out Transform sourceBone))
                {
                    ApplyRotateConstraint(targetBone, sourceBone);
                    ApplyPositionConstraint(targetBone, sourceBone);
                }
            }
        }
    }

    // 指定したルートからすべての子ボーンを辞書化（名前 -> Transform）
    static Dictionary<string, Transform> GetBoneDictionary(Transform root)
    {
        Dictionary<string, Transform> boneDict = new Dictionary<string, Transform>();
        foreach (Transform bone in root.GetComponentsInChildren<Transform>())
        {
            boneDict[bone.name] = bone;
        }
        return boneDict;
    }

    // RotateConstraint を適用する
    static void ApplyRotateConstraint(Transform target, Transform source)
    {
        RotationConstraint rotateConstraint = target.gameObject.AddComponent<RotationConstraint>();

        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = source;
        constraintSource.weight = 1.0f;

        rotateConstraint.AddSource(constraintSource);
        rotateConstraint.weight = 1.0f;
        rotateConstraint.constraintActive = true;
    }// RotateConstraint を適用する
    static void ApplyPositionConstraint(Transform target, Transform source)
    {
        PositionConstraint positionConstraint = target.gameObject.AddComponent<PositionConstraint>();

        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = source;
        constraintSource.weight = 1.0f;

        positionConstraint.AddSource(constraintSource);
        positionConstraint.weight = 1.0f;
        positionConstraint.constraintActive = true;
    }
}
