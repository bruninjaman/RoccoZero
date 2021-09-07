﻿namespace O9K.Evader;

using System;
using System.Collections.Generic;

using Divine.Map.Components;
using Divine.Numerics;

internal sealed class NavMeshPathfinding //TODO Remove
{
    public float CellSize { get; internal set; }

    public uint AddObstacle(Vector3 start, Vector3 end, float radius)
    {
        //Console.WriteLine("NavMeshPathfinding.AddObstacle(Vector3 start, Vector3 end, float radius)");
        return 0;
    }

    public uint AddObstacle(Vector3 position, float v)
    {
        //Console.WriteLine("NavMeshPathfinding.AddObstacle(Vector3 position, float v)");
        return 0;
    }

    public void Dispose()
    {
        Console.WriteLine("NavMeshPathfinding.Dispose()");
    }

    public IEnumerable<Vector3> CalculatePathFromObstacle(Vector3 position1, Vector3 position2, float rotationRad, float speed, float turnRate, float v1, bool v2, out bool success)
    {
        //Console.WriteLine("NavMeshPathfinding.CalculatePathFromObstacle(Vector3 position1, Vector3 position2, float rotationRad, float speed, float turnRate, float v1, bool v2, out bool success)");
        success = false;
        return new List<Vector3>();
    }

    public void RemoveObstacle(uint obstacleId)
    {
        //Console.WriteLine("NavMeshPathfinding.RemoveObstacle(uint obstacleId)");
    }

    public MeshCellFlags GetCellFlags(Vector2 p)
    {
        //Console.WriteLine("NavMeshPathfinding.GetCellFlags(Vector2 p)");
        return MeshCellFlags.None;
    }

    public void GetCellPosition(Vector3 vector3, out int heroX, out int heroY)
    {
        //Console.WriteLine("NavMeshPathfinding.GetCellPosition(Vector3 vector3, out int heroX, out int heroY)");
        heroX = 0;
        heroY = 0;
    }
}