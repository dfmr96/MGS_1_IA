public class RatBoidSteeringState<T> : State<T>
{
    private ISteering _steering;
    private RatModel _ratModel;
    private ObstacleAvoidance _obs;

    public RatBoidSteeringState(RatModel ratModel, ISteering steering, ObstacleAvoidance obs)
    {
        _ratModel = ratModel;
        _steering = steering;
        _obs = obs;
    }

    public override void Execute()
    {
        if (_steering == null) return;
        _ratModel.SetSpeed(_ratModel.walkSpeed);
        var dir = _obs.GetDir(_steering.GetDir(), false);
        _ratModel.Move(dir);
        _ratModel.LookDir(dir);
    }
    //maxDistance = 10
    //matfh.Clamp(distance,0,maxDistance)-maxDistance
}