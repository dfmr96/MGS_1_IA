using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class StatePathfinding<T> : StateFollowPoints<T>
{
    IMove _move;
    Animator _anim;
    public Node start;
    public Node goal;
    public Transform target;
    public StatePathfinding(Transform entity, IMove move, Animator anim, float distanceToPoint = 0.2F) : base(entity, distanceToPoint)
    {
        _move = move;
        _anim = anim;
    }
    public StatePathfinding(Transform entity, IMove move, Animator anim, List<Vector3> waypoints, float distanceToPoint = 0.2f) : base(entity, waypoints, distanceToPoint)
    {
        _move = move;
        _anim = anim;
    }

    protected override void OnMove(Vector3 dir)
    {
        base.OnMove(dir);
        _move.Move(dir);
        _move.LookDir(dir);
    }
    protected override void OnStartPath()
    {
        base.OnStartPath();
        //_move.SetPosition(_waypoints[0]);
        _anim.SetFloat("Vel", 1);
    }
    protected override void OnFinishPath()
    {
        base.OnFinishPath();
        _anim.SetFloat("Vel", 0);
    }
    public void SetPath()
    {
        List<Node> path = BFS.Run<Node>(start, IsSatisfies, GetConnections);
        //Debug.Log(path.Count);
        if (path.Count <= 0) return;
        SetWaypoints(GetPathVector(path));
    }
    public void SetPathDFS()
    {
        List<Node> path = DFS.Run<Node>(start, IsSatisfies, GetConnections);
        //Debug.Log(path.Count);
        if (path.Count <= 0) return;
        SetWaypoints(GetPathVector(path));
    }
    public void SetPathDijkstra()
    {
        List<Node> path = Dijkstra.Run<Node>(start, IsSatisfies, GetConnections, GetCost);
        //Debug.Log(path.Count);
        if (path.Count <= 0) return;
        SetWaypoints(GetPathVector(path));
    }

    public void SetPathAStar()
    {
        var start = GetNearNode(_entity.position);
        goal = GetNearNode(target.position);
        List<Node> path = ASTAR.Run<Node>(start, IsSatisfies, GetConnections, GetCost, Heuristic);
        //Debug.Log(path.Count);
        if (path.Count <= 0) return;
        SetWaypoints(GetPathVector(path));
    }
    public void SetPathAStarPlus()
    {
        var start = GetNearNode(_entity.position);
        goal = GetNearNode(target.position);
        List<Node> path = ASTAR.Run<Node>(start, IsSatisfies, GetConnections, GetCost, Heuristic);
        path = ASTAR.CleanPath(path, InView);
        if (path.Count <= 0) return;
        SetWaypoints(GetPathVector(path));
    }
    
    public void SetPathAStarPlus(Vector3 targetPosition)
    {
        var start = GetNearNode(_entity.position);
        goal = GetNearNode(targetPosition);
        List<Node> path = ASTAR.Run<Node>(start, IsSatisfies, GetConnections, GetCost, Heuristic);
        path = ASTAR.CleanPath(path, InView);
        if (path.Count <= 0) return;
        SetWaypoints(GetPathVector(path));
    }

    public void SetPathThetaStar()
    {
        var start = GetNearNode(_entity.position);
        goal = GetNearNode(target.position);
        List<Node> path = ThetaStar.Run<Node>(start, IsSatisfies, GetConnections, GetCost, Heuristic, InView);
        //Debug.Log(path.Count);
        if (path.Count <= 0) return;
        SetWaypoints(GetPathVector(path));
    }
    public void SetPathAStarPlusVector()
    {
        var start = GetPoint(_entity.position);
        List<Vector3> path = ASTAR.Run<Vector3>(start, IsSatisfies, GetConnections, GetCost, Heuristic);
        path = ASTAR.CleanPath(path, InView);
        if (path.Count <= 0) return;
        SetWaypoints(path);
    }
    Vector3 GetPoint(Vector3 point)
    {
        return Vector3Int.RoundToInt(point);
    }
    bool InView(Node granparent, Node child)
    {
        return InView(granparent.transform.position, child.transform.position);
    }
    bool InView(Vector3 granparent, Vector3 child)
    {
        Debug.Log("RAYO");
        Vector3 diff = child - granparent;
        return !Physics.Raycast(granparent, diff.normalized, diff.magnitude, Constants.obsMask);
    }
    protected Node GetNearNode(Vector3 pos)
    {
        var colls = Physics.OverlapSphere(pos, Constants.nearNodeDistance, Constants.nodeMask);
        Node nearNode = null;
        float nearDistance = 0;
        for (int i = 0; i < colls.Length; i++)
        {
            var currentNode = colls[i].GetComponent<Node>();
            if (currentNode == null) continue;

            var currentDistance = Vector3.Distance(currentNode.transform.position, pos);
            if (nearNode == null || nearDistance > currentDistance)
            {
                Vector3 dir = currentNode.transform.position - pos;
                if (Physics.Raycast(pos, dir.normalized, dir.magnitude, Constants.obsMask)) continue;

                nearNode = currentNode;
                nearDistance = currentDistance;
            }
        }
        return nearNode;
    }

    float Heuristic(Node node)
    {
        float h = 0;
        h += Vector3.Distance(node.transform.position, goal.transform.position);
        return h;
    }
    float Heuristic(Vector3 node)
    {
        float h = 0;
        h += Vector3.Distance(node, target.transform.position);
        return h;
    }
    float GetCost(Node parent, Node child)
    {
        float multiplierDistance = 1;
        float multiplierTrap = 100;

        float cost = 0;
        cost += Vector3.Distance(parent.transform.position, child.transform.position) * multiplierDistance;
        if (child.hasTrap)
        {
            cost += multiplierTrap;
        }

        return cost;
    }
    float GetCost(Vector3 parent, Vector3 child)
    {
        float multiplierDistance = 1;
        float multiplierTrap = 100;

        float cost = 0;
        cost += Vector3.Distance(parent, child) * multiplierDistance;


        return cost;
    }
    List<Vector3> GetPathVector(List<Node> path)
    {
        List<Vector3> pathVector = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            pathVector.Add(path[i].transform.position);
        }
        return pathVector;
    }
    bool IsSatisfies(Node current)
    {
        return current == goal;
    }
    bool IsSatisfies(Vector3 current)
    {
        var pointToGoal = GetPoint(target.transform.position);
        return Vector3.Distance(current, pointToGoal) <= 1f;
    }
    List<Node> GetConnections(Node current)
    {
        return current.neightbourds;
    }
    List<Vector3> GetConnections(Vector3 current)
    {
        List<Vector3> connections = new List<Vector3>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                //if (x == z && x == -z) continue;
                if (x == 0 && z == 0) continue;
                var point = new Vector3(current.x + x, current.y, current.z + z);
                //if (!InView(current, point)) continue;
                if (!ObstacleManager.Singleton.IsRightPos(point)) continue;
                connections.Add(point);
            }
        }
        return connections;
    }
}
