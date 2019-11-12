using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    private List<int> lNodes = new List<int>();

    public Path(int _id)
    {
        lNodes.Add(_id);
    }
    public Path(Path _path)
    {
        lNodes = new List<int>(_path.lNodes);
    }

    public Path ClonePath()
    {
        return new Path(this);
    }

    public int GetNode(int _index)
    {
        if (_index < lNodes.Count)
            return lNodes[_index];
        return -1;
    }

    public int GetLastNode()
    {
        if (lNodes.Count > 0)
            return lNodes[lNodes.Count - 1];

        return -1;
    }
    public int GetNodesCount()
    {
        return lNodes.Count;
    }

    public List<Path> DiscoverNextPos(List<int> _board, int _boardDimension)
    {
        if (this.lNodes.Count == 0)
            return null;

        List<int> newPositions = new List<int>();
        int lastPos = GetLastNode();
        for (int i = 0; i < 4; i++)
        {
            int nextPos = -1;
            switch (i)
            {
                case 0: // up
                case 1: // down
                    nextPos = lastPos + (i == 0 ? -1 : 1) * _boardDimension;
                    break;
                case 2: // left
                case 3: // right
                    nextPos = lastPos + (i == 2 ? -1 : 1);
                    if (nextPos / _boardDimension != lastPos / _boardDimension) // != row
                        continue;
                    break;
            }

            // if next pos isn't available on board
            if (!_board.Contains(nextPos))
                continue;

            newPositions.Add(nextPos);
        }

        List<Path> foundPaths = new List<Path>();
        for (int i = 0; i < newPositions.Count; i++)
        {
            int nextPos = newPositions[i];
            if (i == newPositions.Count - 1)    // add next to this path
            {
                lNodes.Add(nextPos);
                foundPaths.Add(this);
            }
            // found new path
            else
            {
                Path newPath = ClonePath();
                newPath.lNodes.Add(nextPos);
                foundPaths.Add(newPath);
            }
        }
        
        return foundPaths;
    }

    // ======================================= DEBUG =======================================
    public void ShowDebug()
    {
        string strDebug = "";
        foreach(var pos in lNodes)
        {
            strDebug += pos + "_";
            BoardMgr.Instance.HighlightTile(DebugUtils.GetColorTrack(), pos);
        }

        if (DebugUtils.IsDebugEnable())
            Debug.Log(strDebug);
    }
}
