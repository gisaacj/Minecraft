using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft
{
        public enum TerrainType
        {
            Lowlands,
            Highlands,
            Mountains
        }

        public enum BiomeType
        {
            Grasslands,
            Forest,
            Desert,
            Ice
        }


        [System.Serializable]
        public class LandBrush
        {
            public float x;
            public float z;
            public int chunksize;
            public TerrainType terrainType;
            public BiomeType biomeType;

            public LandBrush(float x_, float z_, int chunkSize_, TerrainType terrainType_, BiomeType biomeType_)
            {
                this.x = x_;
                this.z = z_;
                this.chunksize = chunkSize_;
                this.terrainType = terrainType_;
                this.biomeType = biomeType_;
            }
        public void ApplyBrush(Chunk chunk)
        {
            this.x = chunk.transform.position.x;
            this.z = chunk.transform.position.z;
        }
    }

}
