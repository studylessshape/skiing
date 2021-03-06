using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class MapChunkList
{
    [SerializeField] private List<MapChunk> usingChunks;
    public List<MapChunk> UsingChunks => usingChunks;

    [SerializeField] private List<MapChunk> unuseChunks;
    public List<MapChunk> UnuseChunks => unuseChunks;

    private int allChunkNumber;
    private Vector2 chunkSizeConfig;

    private GameObject chunkPrefab;
    private Transform parentObject;
    private TileBase groundFillTile;

    public MapChunkList(int chunkNumber)
    {
        allChunkNumber = chunkNumber;
        usingChunks = new List<MapChunk>(allChunkNumber);
        unuseChunks = new List<MapChunk>(allChunkNumber);
    }

    /// <summary>
    /// 初始化区块列表
    /// </summary>
    /// <param name="chunkPrefab">区块预置体</param>
    /// <param name="genPosition">生成的位置</param>
    /// <param name="parentObject">父物体</param>
    /// <param name="boundSize">区块大小设置</param>
    /// <param name="groundFileTile">地面</param>
    public void InitializeList(GameObject chunkPrefab, Vector2 genPosition, Transform parentObject, Vector2 boundSize, TileBase groundFillTile)
    {
        this.groundFillTile = groundFillTile;
        this.parentObject = parentObject;
        chunkSizeConfig = boundSize;
        for (int genNum = allChunkNumber; genNum > 1; genNum--)
        {
            MapChunk mapChunk = CreateChunk(chunkPrefab, genPosition, parentObject, groundFillTile);
            mapChunk.enabled = false;
            unuseChunks.Add(mapChunk);
        }
        usingChunks.Add(CreateChunk(chunkPrefab, Vector2.zero, parentObject, groundFillTile));
    }

    #region 区块生成相关方法
    /// <summary>
    /// 创建一个区块实例
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    MapChunk CreateChunk(GameObject prefab, Vector2 position, Transform parent, TileBase tile)
    {
        GameObject newChunk = GameObject.Instantiate(prefab);
        if (newChunk is null)
            throw new System.NullReferenceException();

        MapChunk newChunkMap = newChunk.GetComponent<MapChunk>();
        newChunkMap.InitializeChunk(chunkSizeConfig, position, tile, parent);
        return newChunkMap;
    }

    /// <summary>
    /// 简单地获取一个区块
    /// </summary>
    /// <returns></returns>
    private MapChunk GetUnuseChunk(bool isCreateNewIfEmpty)
    {
        if (unuseChunks.Count > 0)
        {
            MapChunk res = unuseChunks[0];
            res.enabled = true;
            unuseChunks.RemoveAt(0);
            return res;
        }
        if(isCreateNewIfEmpty)
        {
            allChunkNumber++;
            return CreateChunk(chunkPrefab, Vector2.zero, parentObject, groundFillTile);
        }
        return null;
    }

    /// <summary>
    /// 新增一个区块到使用区块的List中，位置将设置为传入的参数
    /// </summary>
    /// <param name="center"></param>
    /// <returns></returns>
    public MapChunk AddUseChunk(Vector2 center, bool isCreateNewIfEmpty = false)
    {
        if (!IsCreatedChunk(center))
        {
            MapChunk newChunk = GetUnuseChunk(isCreateNewIfEmpty);
            if (newChunk != null)
            {
                newChunk.InitializeChunk(chunkSizeConfig, center);
                UsingChunks.Add(newChunk);
                return newChunk;
            }
        }
        return null;
    }

    /// <summary>
    /// 判断此处是否生成了区块
    /// </summary>
    /// <param name="center"></param>
    /// <returns></returns>
    public bool IsCreatedChunk(Vector2 center)
    {
        foreach (MapChunk chunk in usingChunks)
        {
            if (center == (Vector2)chunk.bounds.center)
                return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// 移除指定区块，如果未使用区块的List已经存在则返回false，否则返回true
    /// </summary>
    public bool AddUnuseChunk(MapChunk mapChunk)
    {
        if (unuseChunks.Contains(mapChunk))
            return false;
        unuseChunks.Add(mapChunk);
        mapChunk.enabled = false;
        return true;
    }

    /// <summary>
    /// 将所有在未使用中的区块，从UsingChunks中移除
    /// </summary>
    public void CleanUpList()
    {
        foreach(var chunk in unuseChunks)
        {
            usingChunks.Remove(chunk);
        }
    }
}
