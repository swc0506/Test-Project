#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Fantasy;

public sealed partial class SceneConfigData
{
    private readonly OneToManyList<uint, SceneConfig> _sceneConfigByServerConfigId = new OneToManyList<uint, SceneConfig>();

    //网关服务列表
    private readonly List<SceneConfig> mGateCfgDataList = new List<SceneConfig>();
    //账户缓存连接地址字典
    private readonly Dictionary<long, string> mAddressDic = new Dictionary<long, string>();

    protected override void EndInit()
    {
        _sceneConfigByServerConfigId.Clear();

        foreach (var sceneConfig in List)
        {
            _sceneConfigByServerConfigId.Add(sceneConfig.ServerConfigId, sceneConfig);
        }

        mGateCfgDataList.Clear();

        foreach (SceneConfig config in List)
        {
            switch (config.SceneType)
            {
                case SceneType.Gate:
                    mGateCfgDataList.Add(config);
                    break;
            }
        }

    }
    /// <summary>
    /// 获取网关服
    /// </summary>
    /// <param name="accountId"></param>
    public (string address,long sceneConfigid) GetGate(long accountId)
    {

        //通过取余网关服长度获取指定网关配置地址
        int gateScenesIndex = (int)(accountId % mGateCfgDataList.Count);
        //获取网关Scene配置文件
        SceneConfig gateSceneCfg = mGateCfgDataList[gateScenesIndex];
        //先从缓存中查询该账户已经分配好的连接地址
        if (mAddressDic.TryGetValue(accountId, out var addrss))
        {
            return (addrss,gateSceneCfg.Id); 
        }
        //获取外网（客户端）连接ip和端口
        ServerConfig serverCfg = ServerConfigData.Instance.Get(gateSceneCfg.ServerConfigId);
        MachineConfig machineCfg = MachineConfigData.Instance.Get(serverCfg.MachineId);
        //拼接连接ip和端口
        string address = $"{machineCfg.OuterIP}:{gateSceneCfg.OuterPort}";

        return (address, gateSceneCfg.Id);
    }

    public List<SceneConfig> GetByServerConfigId(uint serverConfigId)
    {
        return _sceneConfigByServerConfigId.GetValueOrDefault(serverConfigId);
    }


  

}