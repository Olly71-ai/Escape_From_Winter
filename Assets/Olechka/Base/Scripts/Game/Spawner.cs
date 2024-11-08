//������� ���� ����
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using NaughtyAttributes;
using UnityEngine.Events;

namespace Olechka
{
    [AddComponentMenu("Olechka scripts / Game / Other / Spawner")]
    [DisallowMultipleComponent]
    public class Spawner : MonoBehaviour
    {
        #region ����������
        [Tooltip("������")]
        [SerializeField]
        GameObject Prefab = null;

        [Tooltip("����� ������")]
        [SerializeField]
        Transform Point_spawn = null;

        [Tooltip("�������� ��� ������")]
        [SerializeField]
        bool Start_bool = false;

        [Tooltip("�� ������������ ����������� ������ ��� ����� ������")]
        [SerializeField]
        bool No_rotation_spawn_object_bool = false;

        [Tooltip("������ ������")]
        [SerializeField]
        bool Timer_bool = false;

        [ShowIf(nameof(Timer_bool))]
        [Tooltip("����� ����� ������� �������")]
        [SerializeField]
        float Time_timer = 1f;

        [ShowIf(nameof(Timer_bool))]
        [Tooltip("��������� �����")]
        [SerializeField]
        bool Loop_bool = false;

        [ShowIf(nameof(Timer_bool))]
        [Tooltip("���������� ����������� ����������")]
        [SerializeField]
        bool Count_spawn_bool = false;

        [ShowIfNew(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(Count_spawn_bool), nameof(Timer_bool))]
        [Tooltip("������� ��� ����������")]
        [SerializeField]
        int Count = 10;

        [HideIf(nameof(Loop_bool))]
        [Tooltip("����� ����� ���������� �����")]
        [SerializeField]
        UnityEvent End_spawn_event = new UnityEvent();

        Coroutine Spawn_coroutine = null;

        List<GameObject> Loop_object_list = new List<GameObject>();



        int Count_active = 0;
        #endregion


        #region ��������� ������
        private void Start()
        {
            if (Start_bool)
                Activation();
        }
        #endregion

        #region ������
        IEnumerator Coroutine_spawn()
        {
            while (Loop_bool || Count > Count_active)
            {
                yield return new WaitForSeconds(Time_timer);

                
                if (Count_spawn_bool)
                {
                    if (!Loop_bool)
                    {
                        Spawn();
                        Count_active++;

                        if (Count <= Count_active)
                            Stop_loop_spawn();
                    }
                    else
                    {
                        for(int i = Loop_object_list.Count - 1; i >= 0; i--)
                        {
                            if (Loop_object_list[i] == null || Loop_object_list[i].gameObject.activeSelf == false)
                                Loop_object_list.RemoveAt(i);
                        }

                        if(Loop_object_list.Count < Count)
                        Spawn();
                    }
                }
                else
                {
                    Spawn();
                }
            }

            End_spawn_event.Invoke();

            Spawn_coroutine = null;

        }

        /// <summary>
        /// ��������
        /// </summary>
        void Spawn()
        {
            Vector3 position = Point_spawn ? Point_spawn.position: transform.position;
            Quaternion rotation = Point_spawn ? Point_spawn.rotation : transform.rotation;

            if (No_rotation_spawn_object_bool)
                rotation = Quaternion.identity;

            GameObject obj = LeanPool.Spawn(Prefab, position, rotation);

            if(Loop_bool && Count_spawn_bool)
                Loop_object_list.Add(obj);
        }
        #endregion


        #region ��������� ������
        /// <summary>
        /// ������������
        /// </summary>
        [ContextMenu("������������")]
        public void Activation()
        {
            if (Timer_bool)
            {
                if (Spawn_coroutine == null)
                {
                    Spawn_coroutine = StartCoroutine(Coroutine_spawn());
                    Count_active = 0;
                }

            }
            else
            {
                Spawn();
            }
        }

        /// <summary>
        /// ���������� ���������� �����
        /// </summary>
        public void Stop_loop_spawn()
        {
            Loop_bool = false;
        }
        #endregion

    }



}