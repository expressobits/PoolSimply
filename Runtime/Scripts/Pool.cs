﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExpressoBits.Pools
{
    [CreateAssetMenu(menuName = "Expresso Bits/Pools/Pool", fileName = "Pool")]
    public class Pool : ScriptableObject, IPool
    {

        public PoolSettings Settings => settings;
        public GameObject Prefab => prefab;
        public Queue<GameObject> Objects => objects;

        [SerializeField] private PoolSettings settings = new PoolSettings() { IncreaseSize = 1 };
        [SerializeField] private GameObject prefab;
        private Queue<GameObject> objects = new Queue<GameObject>();

        private void OnValidate()
        {
            settings.IncreaseSize = Math.Max(settings.IncreaseSize, 1);
        }

        #region Basic Methods
        public void Setup(PoolSettings settings, GameObject prefab)
        {
            this.settings = settings;
            settings.IncreaseSize = Math.Max(settings.IncreaseSize, 1);
            this.prefab = prefab;
            objects = new Queue<GameObject>();
        }

        public GameObject Instantiate()
        {
            return Dequeue();
        }

        public GameObject Instantiate(Vector3 position, Quaternion rotation)
        {
            GameObject gameObject = Instantiate();
            gameObject.transform.SetPositionAndRotation(position, rotation);
            return gameObject;
        }

        public GameObject Instantiate(Transform parent)
        {
            GameObject gameObject = Instantiate();
            gameObject.transform.SetParent(parent);
            return gameObject;
        }

        public GameObject Instantiate(Transform parent, bool instantiateInWorldSpace)
        {
            GameObject gameObject = Instantiate();
            gameObject.transform.SetParent(parent, instantiateInWorldSpace);
            return gameObject;
        }

        public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject gameObject = Instantiate(position, rotation);
            gameObject.transform.SetParent(parent);
            return gameObject;
        }

        public void Destroy(GameObject gameObject)
        {
            Enqueue(gameObject);
        }

        public void Clear()
        {
            foreach (var obj in objects)
            {
                Object.Destroy(obj);
            }

            objects.Clear();
        }

        /**
         * Instance amount GameObjects in queue first params
         **/
        public void InstantiateInPoolAmount(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var gameObject = Object.Instantiate(prefab);
                gameObject.SetActive(false);
                objects.Enqueue(gameObject);
            }
        }
        #endregion

        #region EnqueueAndDequeue
        /**
         * Add to queue prefab and set object disabled
         **/
        private void Enqueue(GameObject obj)
        {
            if (objects == null) objects = new Queue<GameObject>();
            obj.SetActive(false);
            OnPoolerDisable(obj);
            objects.Enqueue(obj);
        }

        /**
         * Get object from queue with prefab model, if no exist
         **/
        private GameObject Dequeue()
        {
            // NOTE This exists for cases that have calls in two instantaneous locations and only one has pooldata as a reference.
            PoolManager.RegisterPoolIfNotExists(this);
            //

            if (objects == null) objects = new Queue<GameObject>();
            if (objects.Count == 0)
            {
                InstantiateInPoolAmount((int)Settings.IncreaseSize);
            }

            GameObject obj = objects.Dequeue();
            if (!obj)
            {
                obj = Object.Instantiate(prefab);
            }
            OnPoolerEnable(obj);
            obj.SetActive(true);
            return obj;
        }
        #endregion

        #region Utils
        private void OnPoolerEnable(GameObject obj)
        {
            foreach (var ipooler in obj.GetComponents<IPooler>())
            {
                ipooler.OnPoolerEnable();
            }
        }

        private void OnPoolerDisable(GameObject obj)
        {
            foreach (var ipooler in obj.GetComponents<IPooler>())
            {
                ipooler.OnPoolerDisable();
            }
        }
        #endregion
    }
}