using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimplexNoise;


namespace Minecraft{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    public class Chunk : MonoBehaviour
    {
        public static List<Chunk> chunks = new List<Chunk>();

        public static int width
        {
            get { return World.currentWorld.chunkWidth; }
        }

        public static int height
        {
            get { return World.currentWorld.chunkHeight; }
        }

        public byte[,,] map;
        public Mesh visualMesh;
        //MeshRenderer meshRenderer;
        MeshCollider meshCollider;
        MeshFilter meshFilter;

        void Start()
        {
            chunks.Add(this);
            meshCollider = GetComponent<MeshCollider>();
            meshFilter = GetComponent<MeshFilter>();
            CalculateMapFromScratch();
            //random();
            StartCoroutine(CreateVisualMesh());
        }

        void random()
        {
            map = new byte[width, height, width];

            TerrainType terrainType = GetTerrainFor(transform.position.x, transform.position.z);
            BiomeType biomeType = GetBiomeFor(transform.position.x, transform.position.z);

            float myHeight = GetHeightOf(terrainType);
            byte dirt = (byte)ItemDatabase.GetItemByName("Dirt").id;
            byte grassydirt = (byte)ItemDatabase.GetItemByName("Grass").id;
            byte ice = (byte)ItemDatabase.GetItemByName("Stone").id;
            byte sand = (byte)ItemDatabase.GetItemByName("Bedrock").id;


            for (int x = 0; x < width; x++)
            {
                float percent = (float)x / (float)(width - 1);
                float xBalance = CurvePoint(
                    percent,
                    (GetHeightOf(transform.position.x - width, transform.position.z) + myHeight) / 2,
                    myHeight,
                    (GetHeightOf(transform.position.x + width, transform.position.z) + myHeight) / 2);

                for (int z = 0; z < width; z++)
                {
                    percent = (float)z / (float)(width - 1);
                    float zBalance = CurvePoint(
                        percent,
                        (GetHeightOf(transform.position.x, transform.position.z - width) + myHeight) / 2,
                        myHeight,
                        (GetHeightOf(transform.position.x, transform.position.z + width) + myHeight) / 2);
                    float finalHeight = (xBalance + zBalance) / 2;
                    for (int y = 0; y < finalHeight; y++)
                    {
                        Vector3 offset0 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
                        Vector3 offset1 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
                        Vector3 offset2 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
                        float clusterValue = CalculateNoiseValue(new Vector3(x, y, z) + transform.position, offset0, 0.02f);
                        float blobValue = CalculateNoiseValue(new Vector3(x, y, z) + transform.position, offset1, 0.05f);
                        float mountainValue = CalculateNoiseValue(new Vector3(x, y, z) + transform.position, offset2, 0.009f);
                        switch (terrainType)
                        {
                            case TerrainType.Lowlands:
                                finalHeight = blobValue;
                                break;
                            case TerrainType.Highlands:
                                finalHeight = clusterValue;
                                break;
                            case TerrainType.Mountains:
                                finalHeight = mountainValue;
                                break;
                        }
                        switch (biomeType)
                        {
                            default:
                                if (y >= finalHeight - 1)
                                    map[x, y, z] = grassydirt;
                                else
                                    map[x, y, z] = dirt;
                                break;
                            case BiomeType.Ice:
                                if (y >= finalHeight - 2)
                                    map[x, y, z] = ice;
                                else
                                    map[x, y, z] = dirt;
                                break;
                            case BiomeType.Desert:
                                if (y >= finalHeight - 7)
                                    map[x, y, z] = sand;
                                else
                                    map[x, y, z] = dirt;
                                break;

                        }
                    }
                }
            }
        }

        void Update()
        {

        }

        public static byte GetTheoreticalByte(Vector3 pos)
        {
            Random.InitState(World.currentWorld.seed);
            Vector3 grain0Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            Vector3 grain1Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            Vector3 grain2Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            return GetTheoreticalByte(pos, grain0Offset, grain1Offset, grain2Offset);
        }

        public static byte GetTheoreticalByte(Vector3 pos, Vector3 offset0, Vector3 offset1, Vector3 offset2)
        {
            float heightBase = 10;
            float maxHeight = height - 10;
            float heightSwing = maxHeight - heightBase;
            byte brick = 1;
            float clusterValue = CalculateNoiseValue(pos, offset1, 0.02f);
            float blobValue = CalculateNoiseValue(pos, offset1, 0.05f);
            float mountainValue = CalculateNoiseValue(pos, offset0, 0.009f);
            if ((mountainValue == 0) && (blobValue < 0.2f))
                brick = 2;
            else if (clusterValue > 0.9f)
                brick = 1;
            else if (clusterValue > 0.8f)
                brick = 3;

            mountainValue = Mathf.Sqrt(mountainValue);
            mountainValue *= heightSwing;
            mountainValue += heightBase;
            mountainValue += (blobValue * 10) - 5f;
            if (mountainValue >= pos.y)
                return brick;
            return 0;
        }

        public virtual void CalculateMapFromScratch()
        {
            map = new byte[width, height, width];
            Random.InitState(World.currentWorld.seed);
            Vector3 grain0Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            Vector3 grain1Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            Vector3 grain2Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            for (int x = 0; x < World.currentWorld.chunkWidth; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        map[x, y, z] = GetTheoreticalByte(new Vector3(x, y, z) + transform.position, grain0Offset, grain1Offset, grain2Offset);
                    }
                }
            }
        }

        public static float CalculateNoiseValue(Vector3 pos, Vector3 offset, float scale)
        {
            float noiseX = Mathf.Abs((pos.x + offset.x) * scale);
            float noiseY = Mathf.Abs((pos.y + offset.y) * scale);
            float noiseZ = Mathf.Abs((pos.z + offset.z) * scale);
            return Mathf.Max(0, Noise.Generate(noiseX, noiseY, noiseZ));
        }

        public virtual IEnumerator CreateVisualMesh(bool isChunkload = true)
        {
            visualMesh = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        if (map[x, y, z] == 0)
                            continue;
                        byte brick = map[x, y, z];
                        // Left wall
                        if (IsTransparent(x - 1, y, z))
                            BuildFace(brick, new Vector3(x, y, z+1), Vector3.left, verts, uvs, tris);
                        // Right wall
                        if (IsTransparent(x + 1, y, z))
                            BuildFace(brick, new Vector3(x + 1, y, z + 1), Vector3.right, verts, uvs, tris);
                        // Bottom wall
                        if (IsTransparent(x, y - 1, z))
                            BuildFace(brick, new Vector3(x, y, z), Vector3.down, verts, uvs, tris);
                        // Top wall
                        if (IsTransparent(x, y + 1, z))
                            BuildFace(brick, new Vector3(x, y + 1, z), Vector3.up, verts, uvs, tris);
                        // Back
                        if (IsTransparent(x, y, z - 1))
                            BuildFace(brick, new Vector3(x, y, z), Vector3.back, verts, uvs, tris);
                        // Front
                        if (IsTransparent(x, y, z + 1))
                            BuildFace(brick, new Vector3(x, y, z + 1), Vector3.forward, verts, uvs, tris);


                    }
                }
                if (isChunkload && Time.time > Time.deltaTime)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            visualMesh.vertices = verts.ToArray();
            visualMesh.uv = uvs.ToArray();
            visualMesh.triangles = tris.ToArray();
            visualMesh.RecalculateBounds();
            visualMesh.RecalculateNormals();
            meshFilter.mesh = visualMesh;
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = visualMesh;
            yield return 0;
        }

        public virtual void BuildFace(byte brick, Vector3 corner, Vector3 normal, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
        {
            Vector3 right = Vector3.zero;
            Vector3 up = Vector3.zero;
            bool reversed = false;

            if (normal == Vector3.down || normal == Vector3.right || normal == Vector3.forward)
            {
                reversed = true;
            }
            int index = verts.Count;
            if (normal == Vector3.up || normal == Vector3.down)
            {
                up = Vector3.forward;
                right = Vector3.right;
            }
            else if (normal == Vector3.left || normal == Vector3.right)
            {
                up = Vector3.up;
                right = Vector3.back;
            }
            else if (normal == Vector3.forward || normal == Vector3.back)
            {
                up = Vector3.up;
                right = Vector3.right;
            }
            verts.Add(corner);
            verts.Add(corner + up);
            verts.Add(corner + up + right);
            verts.Add(corner + right);
            if (reversed)
            {
                tris.Add(index + 0);
                tris.Add(index + 2);
                tris.Add(index + 1);

                tris.Add(index + 0);
                tris.Add(index + 3);
                tris.Add(index + 2);
            }
            else
            {
                tris.Add(index + 0);
                tris.Add(index + 1);
                tris.Add(index + 2);

                tris.Add(index + 0);
                tris.Add(index + 2);
                tris.Add(index + 3);
            }
            Vector2 offset = Vector3.zero;
            float resolution = 0.0625f;

            Vector2 _00 = new Vector2(0, 0) * resolution;
            Vector2 _01 = new Vector2(0, 1) * resolution;
            Vector2 _11 = new Vector2(1, 1) * resolution;
            Vector2 _10 = new Vector2(1, 0) * resolution;

            ItemTexture texture = ItemDatabase.GetItemById(brick).texture;
            if (normal == Vector3.up)
            {
                offset = texture.top;
            }
            else if (normal == Vector3.down)
            {
                offset = texture.bottom;
            }
            else if (normal == Vector3.left)
            {
                offset = texture.left;
            }
            else if (normal == Vector3.right)
            {
                offset = texture.right;
            }
            else if (normal == Vector3.forward)
            {
                offset = texture.front;
            }
            else if (normal == Vector3.back)
            {
                offset = texture.back;
            }

            uvs.Add(_00 + offset);
            uvs.Add(_01 + offset);
            uvs.Add(_11 + offset);
            uvs.Add(_10 + offset);
        }

        public virtual bool IsTransparent(int x, int y, int z)
        {
            if (y < 0)
                return false;
            byte brick = GetByte(x, y, z);
            switch (brick)
            {
                case 0:
                    return true;
                default:
                    return false;
            }
        }

        public virtual byte GetByte(int x, int y, int z)
        {
            if ((y < 0) || (y >= height))
                return 0;
            if ((x < 0) || (z < 0) || (x >= width) || (z >= width))
            {
                Vector3 worldPos = new Vector3(x, y, z) + transform.position;
                Chunk chunk = Chunk.FindChunk(worldPos);
                if (chunk == this)
                    return 0;
                if (chunk == null)
                {
                    return GetTheoreticalByte(worldPos);
                }
                return chunk.GetByte(worldPos);
            }
            return map[x, y, z];
        }

        public virtual byte GetByte(Vector3 worldPos)
        {
            worldPos -= transform.position;
            int x = Mathf.FloorToInt(worldPos.x);
            int y = Mathf.FloorToInt(worldPos.y);
            int z = Mathf.FloorToInt(worldPos.z);
            return GetByte(x, y, z);
        }

        public static Chunk FindChunk(Vector3 pos)
        {
            for (int a = 0; a < chunks.Count; a++)
            {
                Vector3 cpos = chunks[a].transform.position;
                if ((pos.x < cpos.x) || (pos.z < cpos.z) || (pos.x >= cpos.x + width) || (pos.z >= cpos.z + width))
                    continue;
                return chunks[a];
            }
            return null;
        }

        public bool SetBrick(byte brick, Vector3 worldPos)
        {
            worldPos -= transform.position;
            return SetBrick(brick, Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z));
        }

        public bool SetBrick(byte brick, int x, int y, int z)
        {
            if ((x < 0) || (y < 0) || (z < 0) || (x >= width) || (y >= height || (z >= width)))
            {
                return false;
            }
            if (map[x, y, z] == brick)
                return false;
            map[x, y, z] = brick;
            StartCoroutine(CreateVisualMesh(false));
            if (x == 0)
            {
                Chunk chunk = FindChunk(new Vector3(x - 2, y, z) + transform.position);
                if (chunk != null)
                {
                    StartCoroutine(chunk.CreateVisualMesh(false));
                }
            }
            if (x == width - 1)
            {
                Chunk chunk = FindChunk(new Vector3(x + 2, y, z) + transform.position);
                if (chunk != null)
                {
                    StartCoroutine(chunk.CreateVisualMesh(false));
                }
            }
            if (z == 0)
            {
                Chunk chunk = FindChunk(new Vector3(x, y, z - 2) + transform.position);
                if (chunk != null)
                {
                    StartCoroutine(chunk.CreateVisualMesh(false));
                }
            }
            if (z == width - 1)
            {
                Chunk chunk = FindChunk(new Vector3(x, y, z + 2) + transform.position);
                if (chunk != null)
                {
                    StartCoroutine(chunk.CreateVisualMesh(false));
                }
            }
            return true;
        }
        public static List<LandBrush> GetBrushesFor(float x, float z)
        {
            List<LandBrush> brushes = new List<LandBrush>();

            TerrainType terrainType = GetTerrainFor(x, z);
            BiomeType biomeType = GetBiomeFor(x, z);

            Random.InitState(World.currentWorld.seed + Mathf.FloorToInt(x * 7 + z * 13));


            float numBrushes = 8;
            if (terrainType == TerrainType.Mountains)
                numBrushes = 18;
            while (numBrushes > 0)
            {
                numBrushes--;
                brushes.Add(new LandBrush(x, z, World.currentWorld.chunkWidth, terrainType, biomeType));
            }

            return brushes;
        }

        public static TerrainType GetTerrainFor(float x, float z)
        {

            Random.InitState(World.currentWorld.seed + Mathf.FloorToInt(x * 7 + z * 13));

            return (TerrainType)Mathf.FloorToInt(Random.value * 3);

        }
        public static BiomeType GetBiomeFor(float x, float z)
        {

            x = Mathf.FloorToInt(x / 160);
            z = Mathf.FloorToInt(x / 160);

            Random.InitState(World.currentWorld.seed + Mathf.FloorToInt(x * 7 + z * 13));

            return (BiomeType)Mathf.FloorToInt(Random.value * 4);

        }

        public static float GetHeightOf(float x, float z)
        {
            return GetHeightOf(GetTerrainFor(x, z));
        }

        public static float GetHeightOf(TerrainType terrainType)
        {
            switch (terrainType)
            {
                default: return 8;
                case TerrainType.Highlands:
                    return 13;
                case TerrainType.Mountains:
                    return 30;
            }
        }

        public static float CurvePoint(float percent, float val1, float val2, float val3)
        {
            float p1 = (1 - percent) * val1 + percent * val2;
            float p2 = (1 - percent) * val2 + percent * val3;
            return (1 - percent) * p1 + percent * p2;
        }
    }
}


