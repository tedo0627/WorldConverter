namespace WorldConverter.Format.Data
{
    public interface ILevelData
    {
        void Load();

        void Save();

        void SetWorldName(string name);

        void SetWorldSpawn(int dimension);
    }
}
