using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using TMPro;
using UnityEngine.EventSystems;

// Place this file in Assets/Editor/ (create that folder if it doesn't exist).
// Editor-only script — it will not be included in your build.
//
// This only ADDS new GameObjects; it never modifies or deletes anything that
// already exists in the scene. Every created object is registered with Unity's
// Undo system, so Ctrl+Z (or just deleting the created objects) fully reverts it.
//
// Run it from: Tools > VR Project > Setup Consultation Audio UI
// (with ConsultationScene open and active)

public static class ConsultationAudioUISetup
{
    private const string CanvasName = "ConsultationAudioCanvas";
    private const string AudioManagerName = "AudioManager";

    [MenuItem("Tools/VR Project/Setup Consultation Audio UI")]
    public static void Setup()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            bool proceed = EditorUtility.DisplayDialog(
                "No EventSystem found",
                "No EventSystem was found in this scene. Button clicks won't register without one. " +
                "If your VR interaction setup provides its own (e.g. XR UI Input Module), that's fine — " +
                "just make sure one exists before testing.\n\nContinue anyway?",
                "Continue", "Cancel");
            if (!proceed) return;
        }

        bool canvasExisted = GameObject.Find(CanvasName) != null;
        bool audioManagerExisted = GameObject.Find(AudioManagerName) != null;

        TextMeshProUGUI subtitleText, translationText, aiHintText, instructionsText;
        EnsureCanvas(out subtitleText, out translationText, out aiHintText, out instructionsText);

        AudioCapture audioCapture;
        SubtitleDisplay subtitleDisplay;
        EnsureAudioManager(out audioCapture, out subtitleDisplay);

        // Always (re)wire — safe even if nothing was missing, and fixes a previously
        // "orphaned" AudioManager that was created before its Canvas existed.
        Undo.RecordObject(subtitleDisplay, "Wire SubtitleDisplay fields");
        subtitleDisplay.subtitleText = subtitleText;
        subtitleDisplay.translationText = translationText;
        subtitleDisplay.hintText = aiHintText;
        subtitleDisplay.instructionsText = instructionsText;

        EnsureButtonListener(audioCapture);

        string summary = (canvasExisted ? "Canvas already existed (reused).\n" : "Canvas created.\n") +
                          (audioManagerExisted ? "AudioManager already existed (reused/rewired)." : "AudioManager created.");

        EditorUtility.DisplayDialog(
            "Setup complete",
            summary + "\n\nNext steps:\n" +
            "1. Reposition/rescale the Canvas onto your actual room surface.\n" +
            "2. Confirm 'Use Mock Data' is checked on the APIClient component.\n" +
            "3. Enter Play mode and click Talk to verify mock text appears.",
            "OK");
    }

    private static void EnsureCanvas(out TextMeshProUGUI subtitleText, out TextMeshProUGUI translationText,
        out TextMeshProUGUI aiHintText, out TextMeshProUGUI instructionsText)
    {
        GameObject canvasGO = GameObject.Find(CanvasName);
        RectTransform canvasRect;

        if (canvasGO == null)
        {
            canvasGO = new GameObject(CanvasName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Consultation Audio Canvas");

            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            canvasRect = canvasGO.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(800, 500);
            // Placeholder placement — you WILL need to move/scale this onto your actual
            // consultation room surface (e.g. a wall panel or desk screen).
            canvasRect.position = new Vector3(0f, 1.5f, 1.2f);
            canvasRect.localScale = Vector3.one * 0.001f;
        }
        else
        {
            canvasRect = canvasGO.GetComponent<RectTransform>();
        }

        // Nothing here is auto-deleted — reposition these manually in the Editor as needed.
        subtitleText = FindOrCreateTextField(canvasRect, "SubtitleText", new Vector2(0, 150), 28, Color.white);
        translationText = FindOrCreateTextField(canvasRect, "TranslationText", new Vector2(0, 60), 22, new Color(0.8f, 0.8f, 0.8f));
        aiHintText = FindOrCreateTextField(canvasRect, "AIHintText", new Vector2(0, -40), 20, new Color(0.7f, 0.9f, 1f));
        instructionsText = FindOrCreateTextField(canvasRect, "InstructionsText", new Vector2(0, -140), 18, new Color(0.6f, 0.6f, 0.6f));

        if (canvasRect.Find("TalkButton") == null)
        {
            GameObject buttonGO = new GameObject("TalkButton", typeof(RectTransform), typeof(Image), typeof(Button));
            Undo.RegisterCreatedObjectUndo(buttonGO, "Create Talk Button");
            buttonGO.transform.SetParent(canvasRect, false);

            RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(200, 60);
            buttonRect.anchoredPosition = new Vector2(0, -220);

            Image buttonImage = buttonGO.GetComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.5f, 0.9f);

            GameObject buttonLabelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            Undo.RegisterCreatedObjectUndo(buttonLabelGO, "Create Talk Button Label");
            buttonLabelGO.transform.SetParent(buttonRect, false);
            TextMeshProUGUI buttonLabel = buttonLabelGO.GetComponent<TextMeshProUGUI>();
            buttonLabel.text = "Talk";
            buttonLabel.fontSize = 24;
            buttonLabel.alignment = TextAlignmentOptions.Center;
            buttonLabel.color = Color.white;
            EnsureFontAssigned(buttonLabel);
            StretchToParent(buttonLabelGO.GetComponent<RectTransform>());
        }
        else
        {
            Transform existingLabel = canvasRect.Find("TalkButton/Label");
            if (existingLabel != null)
            {
                TextMeshProUGUI existingLabelTmp = existingLabel.GetComponent<TextMeshProUGUI>();
                if (existingLabelTmp != null) EnsureFontAssigned(existingLabelTmp);
            }
        }

        Selection.activeGameObject = canvasGO;
    }

    private static TextMeshProUGUI FindOrCreateTextField(RectTransform parent, string name, Vector2 anchoredPos, float fontSize, Color color)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            TextMeshProUGUI existingTmp = existing.GetComponent<TextMeshProUGUI>();
            if (existingTmp != null)
            {
                EnsureFontAssigned(existingTmp);
                return existingTmp;
            }
        }
        return CreateTextField(parent, name, anchoredPos, fontSize, color);
    }

    private static void EnsureAudioManager(out AudioCapture audioCapture, out SubtitleDisplay subtitleDisplay)
    {
        GameObject audioManagerGO = GameObject.Find(AudioManagerName);
        if (audioManagerGO == null)
        {
            audioManagerGO = new GameObject(AudioManagerName);
            Undo.RegisterCreatedObjectUndo(audioManagerGO, "Create AudioManager");
        }

        audioCapture = audioManagerGO.GetComponent<AudioCapture>();
        if (audioCapture == null) audioCapture = Undo.AddComponent<AudioCapture>(audioManagerGO);

        if (audioManagerGO.GetComponent<APIClient>() == null) Undo.AddComponent<APIClient>(audioManagerGO);

        subtitleDisplay = audioManagerGO.GetComponent<SubtitleDisplay>();
        if (subtitleDisplay == null) subtitleDisplay = Undo.AddComponent<SubtitleDisplay>(audioManagerGO);
    }

    private static void EnsureButtonListener(AudioCapture audioCapture)
    {
        GameObject canvasGO = GameObject.Find(CanvasName);
        if (canvasGO == null) return;

        Transform buttonTransform = canvasGO.transform.Find("TalkButton");
        if (buttonTransform == null) return;

        Button button = buttonTransform.GetComponent<Button>();
        if (button == null) return;

        // Avoid stacking duplicate listeners if this is run more than once
        for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
        {
            UnityEventTools.RemovePersistentListener(button.onClick, i);
        }
        UnityEventTools.AddPersistentListener(button.onClick, audioCapture.StartRecording);
    }

    private static TextMeshProUGUI CreateTextField(RectTransform parent, string name, Vector2 anchoredPos, float fontSize, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(700, 80);
        rect.anchoredPosition = anchoredPos;

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = "";
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        EnsureFontAssigned(tmp);

        return tmp;
    }

    // Objects created purely via script never went through Unity's normal "Import TMP
    // Essential Resources" prompt, so their Font Asset field can be left null — which
    // renders as a placeholder "T" icon instead of actual text. This assigns a font
    // whether the object is brand new or was already created (and broken) by an earlier run.
    private static void EnsureFontAssigned(TextMeshProUGUI tmp)
    {
        if (tmp.font != null) return;

        TMP_FontAsset defaultFont = TMP_Settings.defaultFontAsset;
        if (defaultFont == null)
        {
            // Fall back to searching the project for the standard TMP font asset
            string[] guids = AssetDatabase.FindAssets("LiberationSans SDF t:TMP_FontAsset");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                defaultFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            }
        }

        if (defaultFont != null)
        {
            tmp.font = defaultFont;
        }
        else
        {
            Debug.LogWarning("No default TMP font asset found — import TMP Essential Resources via " +
                "Window > TextMeshPro > Import TMP Essential Resources, then re-run this tool.");
        }
    }

    private static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static T FindAnyObjectByType<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER || UNITY_6000_0_OR_NEWER
        return Object.FindAnyObjectByType<T>();
#else
        return Object.FindObjectOfType<T>();
#endif
    }
}
