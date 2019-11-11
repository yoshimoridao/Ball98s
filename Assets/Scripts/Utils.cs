using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    static int stepCount = 0;

    public static Path FindShortestPath(List<int> _board, int _boardDimension, int _startPos, int _endPos)
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
            path = BreadthFirstSearch(_board, ref qPath, _boardDimension, _endPos);
        }

        return path;
    }

    private static Path BreadthFirstSearch(List<int> _board, ref Queue<Path> _qPath, int _boardDimension, int _endPos)
    {
        if (_qPath.Count == 0 || _board.Count == 0)
            return null;

        // debug
        if (DebugUtils.IsDebugEnable())
        {
            Debug.Log("================== step = " + stepCount);
            stepCount++;
        }

        Path path = _qPath.Dequeue();
        List<Path> traversePath = path.DiscoverNextPos(_board, _boardDimension);
        
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
            int tmpId = _board.FindIndex(x => x == lastPosIndex);
            if (tmpId != -1)
                _board.RemoveAt(tmpId);

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
        
        return BreadthFirstSearch(_board, ref _qPath, _boardDimension, _endPos);
    }
}
