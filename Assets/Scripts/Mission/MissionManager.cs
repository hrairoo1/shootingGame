using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// �}�b�v�ǂݍ��݁A�v���C���[�����ʒu�ݒ�A�~�b�V�����i�s�A�G�����A����܂œ���
/// </summary>
public class MissionManager : MonoBehaviour
{
    #region �f�[�^�\��

    [System.Serializable]
    public class MapData
    {
        public string sceneName;           // �V�[����
        public Vector3 playerSpawnPosition; // �v���C���[�̏����ʒu
        public Vector3 cameraPosition;     // �J���������ʒu�i�K�v�Ȃ�j
    }

    [System.Serializable]
    public class SpawnInfo
    {
        public string unit;    // ���j�b�g��
        public string tagId;        // ����p�^�O
        public Vector3 position;
        public float radius;
    }

    [System.Serializable]
    public class Branch
    {
        public string conditionType; // allDead / tagDead
        public string tagId;
        public int nextWaveId;
    }

    [System.Serializable]
    public class WaveData
    {
        public int waveId;
        public string type; // Action_ShowDialogue / Action_SpawnEnemies
        public string dialogueId;
        public List<SpawnInfo> spawns;
        public List<Branch> branches;
        public bool multiple;
    }

    [System.Serializable]
    public class MissionData
    {
        public MapData mapData;
        public List<WaveData> missionWaves;
    }
    private HashSet<string> finishedDialogues = new HashSet<string>();

    #endregion

    public MissionData mission; // JSON����ǂݍ���
    private List<GameObject> activeEnemies = new List<GameObject>();
    private HashSet<int> executedWaves = new HashSet<int>();
    public TextAsset MissionJson;

    void Start()
    {
        mission = JsonUtility.FromJson<MissionData>(MissionJson.text);
        SetupMap();
        StartCoroutine(RunMission());
    }

    void SetupMap()
    {
        Debug.Log($"[�}�b�v�ݒ�] �V�[��={mission.mapData.sceneName}");
        // �{���� SceneManager.LoadScene(mission.mapData.sceneName) ���s��
        // �����ł͗�Ƃ��ăv���C���[�ʒu�����ݒ�
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = mission.mapData.playerSpawnPosition;
        }
    }

    IEnumerator RunMission()
    {
        yield return StartCoroutine(ExecuteWave(1)); // wave1�X�^�[�g�z��
    }

    IEnumerator ExecuteWave(int waveId)
    {
        if (executedWaves.Contains(waveId))
            yield break;

        var wave = mission.missionWaves.FirstOrDefault(w => w.waveId == waveId);
        if (wave == null)
        {
            Debug.LogWarning($"Wave {waveId} ��������܂���");
            yield break;
        }

        executedWaves.Add(waveId);

        switch (wave.type)
        {
            case "Action_ShowDialogue":
                yield return StartCoroutine(ShowDialogueByIdCoroutine(wave.dialogueId));
                yield return StartCoroutine(HandleBranches(wave));
                break;

            case "Action_SpawnUnits":
                SpawnEnemies(wave.spawns);
                yield return StartCoroutine(HandleBranches(wave));
                break;
        }
    }

    void SpawnEnemies(List<SpawnInfo> spawns)
    {
        Debug.Log("spawn");
        foreach (var spawn in spawns)
        {
            Vector3 spawnPos = spawn.position;
            if (spawn.radius > 0)
            {
                Vector3 offset = Random.insideUnitSphere * spawn.radius;
                offset.y = 0;
                spawnPos += offset;
            }

            GameObject prefab = Resources.Load<GameObject>($"Enemies/{spawn.unit}");
            if (prefab != null)
            {
                GameObject unit = Instantiate(prefab, spawnPos, Quaternion.identity);
                UnitInfo u = unit.GetComponent<UnitInfo>();
                if (u != null)
                    u.InitTag(spawn.tagId, this);

                activeEnemies.Add(unit);
            }
            else
            {
                Debug.LogWarning($"�G�v���n�u {spawn.unit} ��������܂���");
            }
        }
    }

    public void OnEnemyKilled(GameObject unit, string tagId)
    {
        activeEnemies.Remove(unit);
    }

    IEnumerator HandleBranches(WaveData wave)
    {
        List<Branch> remaining = new List<Branch>(wave.branches);

        while (remaining.Count > 0)
        {
            for (int i = remaining.Count - 1; i >= 0; i--)
            {
                if (CheckCondition(remaining[i]))
                {
                    StartCoroutine(ExecuteWave(remaining[i].nextWaveId));
                    if (!wave.multiple)
                        yield break;
                    else
                        remaining.RemoveAt(i);
                }
            }
            yield return null;
        }
    }

    bool CheckCondition(Branch branch)
    {
        switch (branch.conditionType)
        {
            case "allDead":
                return activeEnemies.Count == 0;
            case "tagDead":
                return !activeEnemies.Any(e => e.GetComponent<UnitInfo>().TagId == branch.tagId);
            case "dialogueFinished":
                return finishedDialogues.Contains(branch.tagId); // tagId �� dialogueId �Ƃ��Ďg��
        }
        return false;
    }

    IEnumerator ShowDialogueByIdCoroutine(string dialogueId)
    {
        Debug.Log($"[��b] ID={dialogueId}");

        // ���ۂ� DialogManager �� UI �\�����đҋ@
        yield return new WaitForSeconds(2f); // �_�~�[�\������

        OnDialogueFinished(dialogueId);
    }
    public void OnDialogueFinished(string dialogueId)
    {
        finishedDialogues.Add(dialogueId);
    }
}
