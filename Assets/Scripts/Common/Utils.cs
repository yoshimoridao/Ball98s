using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    static int stepCount = 0;

    public static Path FindShortestPath(List<int> _boardMap, int _boardDimension, int _startPos, int _endPos)
    {
        Queue<Path> qPath = new Queue<Path>();
        Path path = null;

        if (DebugUtils.IsDebugEnable())
        {
            Debug.Log("startPos = " + _startPos);
            Debug.Log("_endPos = " + _endPos);
            DebugUtils.ResetColorTrack();
        }

        if (_startPos < Mathf.Pow(_boardDimension, 2))
        {
            qPath.Enqueue(new Path(_startPos));
            path = BreadthFirstSearch(_boardMap, ref qPath, _boardDimension, _endPos);
        }

        return path;
    }

    private static Path BreadthFirstSearch(List<int> _boardMap, ref Queue<Path> _qPath, int _boardDimension, int _endPos)
    {
        if (_qPath.Count == 0 || _boardMap.Count == 0)
            return null;

        // debug
        if (DebugUtils.IsDebugEnable())
        {
            Debug.Log("================== step = " + stepCount);
            stepCount++;
        }

        Path path = _qPath.Dequeue();
        List<Path> traversePath = path.DiscoverNextPos(_boardMap, _boardDimension);

        for (int i = 0; i < traversePath.Count; i++)
        {
            path = traversePath[i];

            // if founded path reaching endPos
            int lastPosIndex = path.GetLastNode();
            if (lastPosIndex == _endPos)
            {
                return path;
            }

            // remove travesed index in board
            int tmpId = _boardMap.FindIndex(x => x == lastPosIndex);
            if (tmpId != -1)
                _boardMap.RemoveAt(tmpId);

            // store path for next search
            _qPath.Enqueue(path);

            // debug
            if (DebugUtils.IsDebugEnable())
                path.ShowDebug();
        }

        // debug
        if (DebugUtils.IsDebugEnable())
        {
            string strDebug = "Query = ";
            foreach (Path p in _qPath)
            {
                strDebug += p.GetLastNode() + "_";
            }
            Debug.Log(strDebug);
        }

        return BreadthFirstSearch(_boardMap, ref _qPath, _boardDimension, _endPos);
    }

    public static void FindPathMathColor(List<Ball> _lBalls, int _boardDimension, List<int> _checkBalls, int _dir, ref List<int> _path)
    {
        if (_path.Count == 0)
            return;

        // check 8 direction
        int nextId = GetNextId(_boardDimension, _path[_path.Count - 1], _dir);

        if (nextId >= 0 && nextId < _lBalls.Count && _checkBalls.Contains(nextId))
        {
            // if match type
            Ball nextBall = _lBalls[nextId];
            Ball fstBall = _lBalls[_path[0]];
            if (nextBall.GetState() == Ball.State.IDLE && nextBall.GetSize() == Ball.Size.BIG)
            {
                bool isKeepFinding = false;
                if (nextBall.GetType() == fstBall.GetType())
                {
                    isKeepFinding = true;
                }
                else if (nextBall.GetType() == Ball.Type.COLORFULL)
                {
                    isKeepFinding = true;
                }
                else if (fstBall.GetType() == Ball.Type.COLORFULL)
                {
                    int colorFullCounter = 0;
                    for (int i = 0; i < _path.Count; i++)
                    {
                        int id = _path[i];
                        Ball.Type type = _lBalls[id].GetType();
                        // get color of path (except ball: color full)
                        if (type != Ball.Type.COLORFULL && type == nextBall.GetType())
                        {
                            isKeepFinding = true;
                            break;
                        }
                        if (type == Ball.Type.COLORFULL)
                            colorFullCounter++;
                    }

                    // if all balls are colorfull
                    if (!isKeepFinding && colorFullCounter == _path.Count)
                        isKeepFinding = true;
                }

                if (isKeepFinding)
                {
                    _path.Add(nextId);
                    // keep going find
                    FindPathMathColor(_lBalls, _boardDimension, _checkBalls, _dir, ref _path);
                }
            }
        }
    }

    private static int GetNextId(int _boardDimension, int _id, int _dir)
    {
        int nextId = _id;
        switch (_dir)
        {
            case 0: // north
            case 1: // south
                nextId = _id + (_dir == 1 ? 1 : -1) * _boardDimension;
                if (nextId % _boardDimension != _id % _boardDimension)    // check same column
                    return -1;
                break;
            case 2: // west
            case 3: // east
                nextId = _id + (_dir == 3 ? 1 : -1);
                if ((int)(nextId / _boardDimension) != (int)(_id / _boardDimension))    // check same row
                    return -1;
                break;
            case 4:
                nextId -= _boardDimension;
                nextId++;
                break;      // n-e
            case 5:
                nextId -= _boardDimension;
                nextId--;
                break;      // n-w
            case 6:
                nextId += _boardDimension;
                nextId++;
                break;      // s-e
            case 7:
                nextId += _boardDimension;
                nextId--;
                break;      // s-w
        }

        return nextId;
    }
}
