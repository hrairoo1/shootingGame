using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public bool IsPlaying { get; private set; }

    private Queue<(string speaker, string text)> dialogueQueue;

    // 会話データは外部ファイルから読み込む想定
    private Dictionary<string, List<(string speaker, string text)>> dialogues;

    private void Awake()
    {
        // 外部JSON読み込みの代わりに仮データ
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

        dialogueQueue = new Queue<(string speaker, string text)>(dialogues[dialogueId]);
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
    }

    /// <summary>
    /// 会話キューの処理
    /// </summary>
    private IEnumerator ProcessDialogueQueue()
    {
        while (dialogueQueue.Count > 0)
        {
            var (speaker, text) = dialogueQueue.Dequeue();
            Debug.Log($"{speaker}: {text}");

            // UI表示やクリック待ち処理に置き換え可能
            yield return new WaitForSeconds(2f);
        }

        IsPlaying = false;
    }
}
