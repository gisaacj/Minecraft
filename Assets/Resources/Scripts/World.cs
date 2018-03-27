﻿using UnityEngine;
using System.Collections;

namespace Minecraft{
    public class World : MonoBehaviour
    {

        public static World currentWorld;
        public int chunkWidth = 16, chunkHeight = 50, seed = 0;
        public float viewRange = 65;
        public Chunk chunkFab;
        public int delRange = 100;

        void Awake()
        {
            Cursor.visible = false;
            currentWorld = this;
            if (seed == 0)
                seed = Random.Range(0, int.MaxValue);
        }

        void Update()
        {

            for (float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += chunkWidth)
            {
                for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += chunkWidth)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    pos.x = Mathf.Floor(pos.x / (float)chunkWidth) * chunkWidth;
                    pos.z = Mathf.Floor(pos.z / (float)chunkWidth) * chunkWidth;
                    Chunk chunk = Chunk.FindChunk(pos);
                    if (chunk != null)
                        continue;
                    if (Vector3.Distance(pos, transform.position) < viewRange)
                    {
                        chunk = (Chunk)Instantiate(chunkFab, pos, Quaternion.identity);
                    }
                }
            }
        }
    }
}