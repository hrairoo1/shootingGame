using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        public string text;
    }

    [System.Serializable]
    public class DialogueData
    {
        public List<DialogueEntry> dialogues;
    }

    [System.Serializable]
    public class DialogueEntry
    {
        public string dialogueId;
        public List<DialogueLine> lines;
    }
    [SerializeField] MissionManager missionManager;
    [SerializeField] BattleUI battleUI;
    public bool IsPlaying { get; private set; }

    private Queue<DialogueLine> dialogueQueue;

    // 会話データは外部ファイルから読み込む想定

    public TextAsset DialogueJson;

    private Dictionary<string, List<DialogueLine>> dialogues;

    private void Awake()
    {
        DialogueData data = JsonUtility.FromJson<DialogueData>(DialogueJson.text);
        dialogues = new Dictionary<string, List<DialogueLine>>();
        foreach (var entry in data.dialogues)
        {
            dialogues[entry.dialogueId] = entry.lines;
        }
    }

    /// <summary>
    /// 会話をIDで開始
    /// </summary>
    public void ShowDialogueById(string dialogueId)
    {
        if (!dialogues.ContainsKey(dialogueId))
        {
            Debug.LogWarning($"Dialogue ID not found: {dialogueId}");
            return;
        }

        dialogueQueue = new Queue<DialogueLine>(dialogues[dialogueId]);
        IsPlaying = true;
        StartCoroutine(ProcessDialogueQueue());
    }

    /// <summary>
    /// 会話が終わるまで待機するコルーチン
    /// </summary>
    public IEnumerator ShowDialogueByIdCoroutine(string dialogueId)
    {
        ShowDialogueById(dialogueId);
        while (IsPlaying)
        {
            yield return null;
        }
        missionManager.OnDialogueFinished(dialogueId);
    }

    /// <summary>
    /// 会話キューの処理
    /// </summary>
    private IEnumerator ProcessDialogueQueue()
    {
        while (dialogueQueue.Count > 0)
        {
            var line = dialogueQueue.Dequeue();
            Debug.Log($"{line.speaker}: {line.text}");

            if (battleUI != null)
            {
                battleUI.speaker.text = line.speaker;
                battleUI.speakerText.text = line.text;
            }

            yield return new WaitForSeconds(2f);
        }

        IsPlaying = false;
    }
}
