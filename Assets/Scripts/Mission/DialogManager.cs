using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public bool IsPlaying { get; private set; }

    private Queue<(string speaker, string text)> dialogueQueue;

    // ��b�f�[�^�͊O���t�@�C������ǂݍ��ޑz��
    private Dictionary<string, List<(string speaker, string text)>> dialogues;

    private void Awake()
    {
        // �O��JSON�ǂݍ��݂̑���ɉ��f�[�^
        dialogues = new Dictionary<string, List<(string, string)>>()
        {
            {
                "DIALOGUE_001",
                new List<(string, string)>
                {
                    ("A", "������������܂��G�����邩��"),
                    ("B", "�n���������Ȃ�B�k���邶��Ȃ����B"),
                    ("A", "���A����Ȃ킯�Ȃ���")
                }
            }
        };
    }

    /// <summary>
    /// ��b��ID�ŊJ�n
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
    /// ��b���I���܂őҋ@����R���[�`��
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
    /// ��b�L���[�̏���
    /// </summary>
    private IEnumerator ProcessDialogueQueue()
    {
        while (dialogueQueue.Count > 0)
        {
            var (speaker, text) = dialogueQueue.Dequeue();
            Debug.Log($"{speaker}: {text}");

            // UI�\����N���b�N�҂������ɒu�������\
            yield return new WaitForSeconds(2f);
        }

        IsPlaying = false;
    }
}
