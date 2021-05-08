public class MergeContainer
{
    public CubeEntity e1;
    public CubeEntity e2;

    public void AddEntity(CubeEntity cubeEntity)
    {
        if (e1 == null)
            e1 = cubeEntity;
        else if (e2 == null)
            e2 = cubeEntity;

        if (IsReadyToMerge())
            e1.CubeMerge.MergeTo(e2);
    }

    public bool HasRoom()
    {
        return e1 == null || e2 == null;
    }

    private bool IsReadyToMerge()
    {
        return e1 != null && e2 != null;
    }
}