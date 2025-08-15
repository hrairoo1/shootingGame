using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

/// <summary>
/// �}�b�v�ǂݍ��݁A�v���C���[�����ʒu�ݒ�A�~�b�V�����i�s�A�G�����A����܂œ���
/// </summary>
public class MissionManager : MonoBehaviour
{
    #region �f�[�^�\��
    public GameObject enem;
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
        public string unitType;
        public string unit;    // ���j�b�g��
        public string tagId;        // ����p�^�O
        public int count;
        public Vector3 position;
        public float radius;
    }

    [System.Serializable]
    public class Branch
    {
        public string conditionType; // allDead / tagDead
        public string tagId;
        public int threshold;
        public float waitForSeconds;
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
    public List<GameObject> activeUnits = new List<GameObject>();
    private HashSet<int> executedWaves = new HashSet<int>();
    public TextAsset MissionJson;
    [SerializeField] private DialogueManager dialogueManager;
    // �E�F�[�u�J�n���ɓG�X�|�[���t���O���Z�b�g
    private bool enemiesSpawnedThisWave = false;
    private float branchStartTime;

    void Start()
    {
        if (dialogueManager == null)
            dialogueManager = FindObjectOfType<DialogueManager>();
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
            player.GetComponent<Unit>().faction = Unit.Faction.Ally;
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
                yield return StartCoroutine(dialogueManager.ShowDialogueByIdCoroutine(wave.dialogueId));
                yield return StartCoroutine(HandleBranches(wave));
                break;

            case "Action_SpawnUnits":
                SpawnUnits(wave.spawns);
                yield return StartCoroutine(HandleBranches(wave));
                break;
        }
    }

    void SpawnUnits(List<SpawnInfo> spawns)
    {
        foreach (var spawn in spawns)
        {
            int spawnCount = spawn.count;
            if (spawnCount <= 0) spawnCount = 1;
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPos = spawn.position;
                if (spawn.radius > 0)
                {
                    Vector3 offset = Random.insideUnitSphere * spawn.radius;
                    offset.y = 0;
                    spawnPos += offset;
                }

                Addressables.InstantiateAsync(spawn.unit, spawnPos, Quaternion.identity)
                .Completed += (handle) =>
                {

                    GameObject unit = handle.Result;
                    if (unit != null)
                    {
                        Unit u = unit.GetComponent<Unit>();
                        if (u != null)
                        {
                            u.InitTag(spawn.tagId, this);
                            string factionStr = spawn.unitType; // ������Ŏ󂯎��
                            bool success = Enum.TryParse<Unit.Faction>(factionStr, out Unit.Faction factionEnum);

                            if (success)
                            {
                                u.faction = factionEnum;
                            }
                            if(factionStr == "Enemy") enemiesSpawnedThisWave = true;
                            else
                            {
                                Debug.LogWarning("Faction parse failed for: " + factionStr);
                            }
                            activeUnits.Add(unit);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"�G�v���n�u {spawn.unit} ��������܂���");
                    }
                };
            }
        }
    }

    public void OnEnemyKilled(GameObject unit, string tagId)
    {
        activeUnits.Remove(unit);
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
            case "allEnemyDead":
                // �G���X�|�[�����Ă��Ȃ��Ȃ画�肵�Ȃ�
                if (!enemiesSpawnedThisWave) return false;
                return activeUnits.All(e => e.GetComponent<Unit>().faction != Unit.Faction.Enemy);
            case "allAllyDead":
                return activeUnits.Any(e => e.GetComponent<Unit>().faction != Unit.Faction.Ally);
            case "tagDead":
                return !activeUnits.Any(e => e.GetComponent<Unit>().TagId == branch.tagId);
            case "enemyCountLessOrEqual":
                return activeUnits.Count(u =>
                    u.GetComponent<Unit>().faction.ToString() == branch.tagId) <= branch.threshold;
            case "dialogueEnd":
                return finishedDialogues.Contains(branch.tagId); // tagId �� dialogueId �Ƃ��Ďg��
            case "waitForSeconds":
                if (branchStartTime == 0f)
                    branchStartTime = Time.time;
                return Time.time - branchStartTime >= branch.waitForSeconds;
            case "alwaysTrue":
                return true; // �������ő��i�s
            }
        return false;
    }
    public void OnDialogueFinished(string dialogueId)
    {
        finishedDialogues.Add(dialogueId);
    }
}
