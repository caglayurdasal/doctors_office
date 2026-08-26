using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

// Place this file in Assets/Editor/ alongside ConsultationAudioUISetup.cs.
// Editor-only script — not included in your build.
//
// There is currently no separate "doctor" character asset in this project —
// the only humanoid model available is Ch22_nonPBR@Standing Greeting.fbx
// (the same one restored for Reception). This reuses it as a placeholder
// doctor character in ConsultationScene, since sourcing/importing a distinct
// model isn't worth the time this close to your deadline.
//
// Run it from: Tools > VR Project > Add Placeholder Doctor Character
// (with ConsultationScene open and active)

public static class DoctorCharacterSetup
{
    private const string ModelPath = "Assets/character/Ch22_nonPBR@Standing Greeting.fbx";
    private const string ControllerPath = "Assets/character/CHaracter.controller";
    private const string DoctorObjectName = "DoctorCharacter";

    [MenuItem("Tools/VR Project/Add Placeholder Doctor Character")]
    public static void AddDoctorCharacter()
    {
        if (GameObject.Find(DoctorObjectName) != null)
        {
            EditorUtility.DisplayDialog(
                "Already exists",
                $"A '{DoctorObjectName}' GameObject already exists in this scene. " +
                "Delete it first if you want to re-add it from scratch.",
                "OK");
            return;
        }

        GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (modelAsset == null)
        {
            EditorUtility.DisplayDialog(
                "Model not found",
                $"Couldn't find the character model at:\n{ModelPath}\n\n" +
                "Make sure you've pulled the Git LFS file successfully (check its file size " +
                "is tens of MB, not ~133 bytes) and that the path/filename matches exactly.",
                "OK");
            return;
        }

        // Instantiate as a proper linked prefab instance (not a disconnected copy),
        // so it stays connected to the source asset like a normal prefab drag-in.
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset);
        instance.name = DoctorObjectName;
        Undo.RegisterCreatedObjectUndo(instance, "Add Placeholder Doctor Character");

        // Placeholder position — you WILL need to move this to wherever makes sense
        // in your consultation room (e.g. standing near the exam table/chair).
        instance.transform.position = new Vector3(0f, 0f, 0f);
        instance.transform.rotation = Quaternion.identity;

        // Wire up the same Animator Controller used for the Reception character, if present
        Animator animator = instance.GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller != null)
            {
                Undo.RecordObject(animator, "Assign Animator Controller");
                animator.runtimeAnimatorController = controller;
            }
            else
            {
                Debug.LogWarning($"Animator Controller not found at {ControllerPath} — the character " +
                    "will be placed but won't play any animation until one is assigned.");
            }
        }

        Selection.activeGameObject = instance;

        EditorUtility.DisplayDialog(
            "Doctor character added",
            "Added a placeholder doctor character (reusing the Ch22 model) to the scene.\n\n" +
            "Next steps:\n" +
            "1. Reposition it to stand in a sensible spot in the consultation room.\n" +
            "2. Scale/rotate if needed to match the room proportions.\n" +
            "3. Consider swapping in a distinct doctor model later if time allows — " +
            "this is a functional placeholder, not a final asset.",
            "OK");
    }
}
