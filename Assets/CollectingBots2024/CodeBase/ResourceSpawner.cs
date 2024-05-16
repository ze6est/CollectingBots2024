namespace CollectingBots2024.CodeBase
{
    public class ResourceSpawner : Spawner<Resource>
    {
        private void Awake()
        {
            Construct(AssetsPath.ResourcePrefabPath);
        }
    }
}
