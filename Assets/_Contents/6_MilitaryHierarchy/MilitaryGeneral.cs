using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MilitaryHierarchy;

public class MilitaryGeneral : MonoBehaviour
{
    // ������x�̕������s�k����ƑS�����P�ނ���
    // �h���S�������j����ƑS�����P�ނ���
    // �����̔����ȏオ�����Ƃ��̕����͓P�ނ���

    [SerializeField] GameObject _tankLeaderPrefab;
    [SerializeField] GameObject _tankPrefab;
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField] Transform[] _escapePoints;
    [Header("�����ɖ��߂��邽�߂Ɏg�p����")]
    [SerializeField] Transform _destination;

    /// <summary>
    /// �����̕����|�C���g�����p�\���`�F�b�N���邽�߂̔z��
    /// �����l��true�ŗ��p�\�ɂȂ��Ă���
    /// </summary>
    bool[] _spawnPointAvailable;
    /// <summary>
    /// 0�n�܂�̕����ԍ��ŁA�������s���x��1�������Ă���
    /// </summary>
    int _troopNum;

    void Awake()
    {
        // 0�n�܂�Ŏw�肷��z��ŏ�����
        _spawnPointAvailable = new bool[_spawnPoints.Length];
        System.Array.Fill(_spawnPointAvailable, true);
    }

    void Start()
    {
        // �����𐶐����ĕ������Ɉړ���̖��߂�����
        SpawnTroop(_troopNum);
        MessageBroker.Default.Publish(new DestinationMessage()
        {
            Destination = _destination.position,
            TroopNum = _troopNum,
        });

        _troopNum++;
    }

    /// <summary>
    /// ������1�� ���m3��������4�́A�����_���ȉӏ��ɕ����𐶐�
    /// �������������̃��j�b�g�ɂ͕����ԍ���t�^����
    /// </summary>
    void SpawnTroop(int troopNum)
    {
        // �������������_���ȕ����|�C���g�ɐ���
        Vector3 spawnPos = _spawnPoints[GetEmptySpawnPointIndex()].position;
        GameObject tankLeader = Instantiate(_tankLeaderPrefab, spawnPos, Quaternion.identity);
        tankLeader.GetComponent<TankLeader>().TroopNum = troopNum;

        // ���͔��ߖT�̉������ɔz�u
        float mag = 3.0f;
        List<Vector3> neighbours = new()
        {
            spawnPos + Vector3.forward * mag,
            spawnPos + Vector3.forward * mag + Vector3.right * mag,
            spawnPos + Vector3.right * mag,
            spawnPos + Vector3.back * mag + Vector3.right * mag,
            spawnPos + Vector3.back * mag,
            spawnPos + Vector3.back * mag + Vector3.left * mag,
            spawnPos + Vector3.left * mag,
            spawnPos + Vector3.forward * mag + Vector3.left * mag,
        };

        // 3��������4�̐���
        int spawnQuantity = Random.Range(3, 5);
        for(int i= 0; i < spawnQuantity; i++)
        {
            int neighbourIndex = Random.Range(0, neighbours.Count);
            GameObject tank = Instantiate(_tankPrefab, neighbours[neighbourIndex], Quaternion.identity);
            tank.GetComponent<Tank>().TroopNum = troopNum;
            neighbours.RemoveAt(neighbourIndex);
        }
    }

    /// <summary>
    /// �����_���ȋ󂢂������|�C���g��Ԃ�
    /// </summary>
    int GetEmptySpawnPointIndex()
    {
        for(int i = 0; i < 100; i++)
        {
            int index = Random.Range(0, _spawnPointAvailable.Length);
            if (_spawnPointAvailable[index])
            {
                return index;
            }
        }

        throw new System.InvalidOperationException("�����_���Ȑ����΂肷���Ă���");
    }
}
