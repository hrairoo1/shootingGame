using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public bool IsPlaying { get; private set; }

    private Queue<(string speaker, string text)> dialogueQueue;

    // 会話データは外部ファイルから読み込む想定
    private Dictionary<string, List<(string speaker, string text)>> dialogues;

    private void Awake()
    {
        // ここで外部JSON読み込みしても良い
        dialogues = new Dictionary<string, List<(string, string)>>()
        {
            {
                "DIALOGUE_001",
                new List<(string, string)>
                {
                    ("A", "もしかしたらまだ敵がいるかも"),
                    ("B", "馬鹿を言うなよ。震えるじゃないか。"),
                    ("A", "そ、そんなわけないか")
                }
            }
        };
    }

    public void ShowDialogueById(string dialogueId)
    {
        if (!dialogues.ContainsKey(dialogueId))
        {
            Debug.LogWarning($"Dialogue ID not found: {dialogueId}");
            return;
        }

        dialogueQueue = new Queue<(string speaker, string text)>(dialogues[dialogueId]);
        IsPlaying = true;
        StartCoroutine(ProcessDialogueQueue());
    }
    public IEnumerator ShowDialogueByIdCoroutine(string dialogueId)
    {
        ShowDialogueById(dialogueId);
        while (IsPlaying)
        {
            yield return null;
        }
    }

    IEnumerator ProcessDialogueQueue()
    {
        while (dialogueQueue.Count > 0)
        {
            var (speaker, text) = dialogueQueue.Dequeue();
            Debug.Log($"{speaker}: {text}");

            // ここでUI表示してユーザー入力待ちなどを実装
            // 例として2秒待つだけにしておく
            yield return new WaitForSeconds(2f);
        }

        IsPlaying = false;
    }
}
