using UnityEngine;
using Spine;
using Spine.Unity;

[RequireComponent(typeof(SkeletonAnimation))]
public class CategorySkinFactory : MonoBehaviour
{
    [Header("Spine Data Asset")]
    [Tooltip("Drag your _SkeletonData asset here")]
    private SkeletonDataAsset skeletonDataAsset;

    [SpineSkin(dataField:"skeletonDataAsset")] public string hatSkin;
    [SpineSkin(dataField:"skeletonDataAsset")] public string shirtSkin;
    [SpineSkin(dataField:"skeletonDataAsset")] public string shoesSkin;
    [SpineSkin(dataField:"skeletonDataAsset")] public string facialHairSkin;

    SkeletonAnimation _skeletonAnim;

    void Awake()
    {
        _skeletonAnim = GetComponent<SkeletonAnimation>();
        ApplySelectedSkins();
    }

    [ContextMenu("Apply Selected Skins")]
    public void ApplySelectedSkins()
    {
        var skeleton = _skeletonAnim.Skeleton;
        var data     = skeleton.Data;

        var combined = new Skin("combined-skin");

        AppendIfValid(data, combined, hatSkin);
        AppendIfValid(data, combined, shirtSkin);
        AppendIfValid(data, combined, shoesSkin);
        AppendIfValid(data, combined, facialHairSkin);

        skeleton.SetSkin(combined);
        skeleton.SetSlotsToSetupPose();
        _skeletonAnim.Update(0);
    }

    void AppendIfValid(SkeletonData data, Skin target, string skinName)
    {
        if (string.IsNullOrEmpty(skinName)) return;
        var skin = data.FindSkin(skinName);
        if (skin != null)
            target.AddSkin(skin);     // <-- use AddSkin instead of Append
        else
            Debug.LogWarning($"SkinFactory: “{skinName}” not found");
    }
}