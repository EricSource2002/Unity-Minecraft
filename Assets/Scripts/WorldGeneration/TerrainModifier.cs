using UnityEngine;
using UnityEngine.UI;

public class TerrainModifier : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public GameObject Camera;
    private ToolBar ToolBar;
    private float maxDist = 8;
    void Start()
    {
        ToolBar = this.transform.GetComponent<ToolBar>();
    }
    void Update()
    {
        if(UIController.isUIEnabled) return;
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hitInfo, maxDist, groundLayer))
        {

            Vector3 pointInTargetBlock;
            if (leftClick || rightClick)
            {


                if (leftClick)
                {
                    pointInTargetBlock = hitInfo.point + Camera.transform.forward * .02f;
                }
                else
                {
                    pointInTargetBlock = hitInfo.point - Camera.transform.forward * .02f;
                }


                int chunkPosX = Mathf.FloorToInt(pointInTargetBlock.x / 16f) * 16;
                int chunkPosZ = Mathf.FloorToInt(pointInTargetBlock.z / 16f) * 16;

                ChunkPos cp = new ChunkPos(chunkPosX, chunkPosZ);
                //ROUNDED FLOOR 
                int rfx = Mathf.FloorToInt(pointInTargetBlock.x);
                int rfy = Mathf.FloorToInt(pointInTargetBlock.y);
                int rfz = Mathf.FloorToInt(pointInTargetBlock.z);
                //INDEX OF BLOCK
                int bix = rfx - chunkPosX + 1;
                int biy = rfy;
                int biz = rfz - chunkPosZ + 1;
                //GET BLOCK Center
                Vector3 blockCenter = getBlockWorldCenter(new Vector3(rfx, rfy, rfz), new Vector3(pointInTargetBlock.x, pointInTargetBlock.y, pointInTargetBlock.z));
                if (TerrainGenerator.getBlock(cp, bix - 1, biy, biz - 1) == BlockType.Core) return;
                TerrainGenerator.updateChunk(cp, (blocks, updateBlock) =>
                {
                    if (leftClick)
                    {
                        updateBlock(bix, biy, biz, BlockType.Air);
                    }
                    else if (rightClick &&
                    !Physics.CheckBox(new Vector3(blockCenter.x, blockCenter.y, blockCenter.z),
                     new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, playerLayer))
                    {
                        if (ToolBar.selectedBlock == BlockType.Core) return;
                        updateBlock(bix, biy, biz, ToolBar.selectedBlock);
                    }
                });
            }
        }
    }
    private static Vector3 getBlockWorldCenter(Vector3 rf, Vector3 or)
    {
        Vector3 blockCenter;
        if (rf.x > or.x)
        {
            blockCenter.x = rf.x - .5f;
        }
        else
        {
            blockCenter.x = rf.x + .5f;
        }
        if (rf.x > or.z)
        {
            blockCenter.z = rf.z - .5f;
        }
        else
        {
            blockCenter.z = rf.z + .5f;
        }
        if (rf.y > or.y)
        {
            blockCenter.y = rf.y - .5f;
        }
        else
        {
            blockCenter.y = rf.y + .5f;
        }
        return blockCenter;
    }
}
