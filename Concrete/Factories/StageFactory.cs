using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class StageFactory
{
    private const int BASE_LENGTH = 1280;
    private enum StageCellSize {
        Small = 1,
        Medium = 2,
        Large = 3,
    }

    private enum StageCellType {
        Empty,
        Planet,
        EndPortal,
        Enemy,
    }

    public StageConfiguration GeneratePlanetStage(int stageNumber)
    {
        var rand = new Random();
        // var planetScale = rand.Next(1, 2);
        var size = new Vector2I(2, 2);

        // var planet = new PlanetConfiguration(
        //     GlobalPosition: size / 2 * BASE_LENGTH,
        //     PlanetTexture: PlanetConstants.PlanetTexture.TerranLarge,
        //     Gravity: 100 * planetScale,
        //     GravityRadiusScale: 7 * planetScale,
        //     RadiusScale: 7 * planetScale);
        
        var enemy = new EnemyConfiguration(
            GlobalPosition: new Vector2(size.X * 0.5f, size.Y * 0.05f) * BASE_LENGTH,
            AmountOfInstances: 2 * stageNumber
        );

        var portal = new PortalConfiguration(
            GlobalPosition: new Vector2(size.X / 2, size.Y * 0.1f) * BASE_LENGTH,
            PortalTexture: PlanetConstants.PortalTexture.PortalGreen,
            Gravity: 2000,
            GravityRadiusScale: 2,
            RadiusScale: 2);
        
        return new StageConfiguration(
            Size: new(BASE_LENGTH * size.X, BASE_LENGTH * size.Y),
            StartPosition: StageConstants.StartPosition.Bottom,
            EnemyConfigurations: [enemy],
            PlanetConfigurations: [],
            PortalConfigurations: [portal]);
    }

    public StageConfiguration GenerateStageConfiguration()
    {
        var rand = new Random();
        var requirements = new List<(StageCellSize cellSize, StageCellType cellType)>
        { 
            (StageCellSize.Small, StageCellType.EndPortal),
            (StageCellSize.Medium, StageCellType.Enemy),
            (StageCellSize.Small, StageCellType.Enemy),
            (StageCellSize.Large, StageCellType.Planet),
            (StageCellSize.Medium, StageCellType.Planet),
        };
        var size = new Vector2I(rand.Next(2, 5), rand.Next(3, 5));
        
        var taken = new List<(Vector2I takenCornerPos, StageCellSize takenSize, StageCellType takenType)>();

        while (requirements.Count > 0)
        {
            var cands = FindSpotForCell(
                requirements[0].cellSize,
                size,
                taken.Select(item => (item.takenCornerPos, item.takenSize)).ToList());

            if (cands.Count > 0)
            {
                var cand = cands.PickRandomElement();
                taken.Add((new((int)cand.X, (int)cand.Y), requirements[0].cellSize, requirements[0].cellType));
            }
            requirements.RemoveAt(0);
        }

        List<Vector2> extraCands = [];
        do
        {
            extraCands = FindSpotForCell(
                StageCellSize.Small,
                size,
                taken.Select(item => (item.takenCornerPos, item.takenSize)).ToList());

            if (extraCands.Count > 0)
            {
                var cand = extraCands.PickRandomElement();
                var candType = Enum.GetValues<StageCellType>()
                    .Where(cellType => cellType != StageCellType.EndPortal)
                    .PickRandomElement();
                taken.Add((new((int)cand.X, (int)cand.Y), StageCellSize.Small, candType));
            }
        }
        while (extraCands.Count > 0);

        var portalConfigurations = taken
            .Where(cell => cell.takenType == StageCellType.EndPortal)
            .Select(cell => CreateEndPortalForCell(cell.takenSize, cell.takenCornerPos));

        var planetConfigurations = taken
            .Where(cell => cell.takenType == StageCellType.Planet)
            .Select(cell => CreatePlanetForCell(cell.takenSize, cell.takenCornerPos));
        
        var enemyConfigurations = taken
            .Where(cell => cell.takenType == StageCellType.Enemy)
            .Select(cell => CreateEnemyForCell(cell.takenSize, cell.takenCornerPos));

        return new StageConfiguration(
            Size: new(BASE_LENGTH * size.X, BASE_LENGTH * size.Y),
            StartPosition: StageConstants.StartPosition.Bottom,
            EnemyConfigurations: enemyConfigurations.ToArray(),
            PlanetConfigurations: planetConfigurations.ToArray(),
            PortalConfigurations: portalConfigurations.ToArray());
    }

    private EnemyConfiguration CreateEnemyForCell(StageCellSize cellSize, Vector2I cellCornerPos)
    {
        var rand = new Random();
        var globalPositionWiggle = (float)((rand.NextDouble() * 2 - 1) * 0.1 + 1);
        var amountWiggle = rand.Next(0, 2);
        return new EnemyConfiguration(
            GlobalPosition: new Vector2(
                BASE_LENGTH * (cellCornerPos.X + (int)cellSize) / 2,
                BASE_LENGTH * (cellCornerPos.Y + (int)cellSize) / 2) * globalPositionWiggle,
            AmountOfInstances: (int)cellSize + amountWiggle
        );
    }

    private PlanetConfiguration CreatePlanetForCell(StageCellSize cellSize, Vector2I cellCornerPos)
    {
        var rand = new Random();
        var globalPositionWiggle = (float)((rand.NextDouble() * 2 - 1) * 0.1 + 1);
        var gravityWiggle = (float)rand.Next(-100, 100);
        var radiusWiggle = (float)rand.NextDouble() + 1;
        return new PlanetConfiguration(
            GlobalPosition: new Vector2(BASE_LENGTH * (cellCornerPos.X + (int)cellSize) / 2, BASE_LENGTH * (cellCornerPos.Y + (int)cellSize) / 2).Mult(globalPositionWiggle),
            PlanetTexture: Enum.GetValues<PlanetConstants.PlanetTexture>().PickRandomElement(),
            Gravity: (float)cellSize * 150 + gravityWiggle,
            RadiusScale: (float)cellSize * radiusWiggle,
            GravityRadiusScale: (float)cellSize * radiusWiggle
        );
    }

    private PortalConfiguration CreateEndPortalForCell(StageCellSize cellSize, Vector2I cellCornerPos)
    {
        return new PortalConfiguration(
            GlobalPosition: new Vector2(BASE_LENGTH * (cellCornerPos.X + (int)cellSize) / 2, BASE_LENGTH * (cellCornerPos.Y + (int)cellSize) / 2),
            PortalTexture: PlanetConstants.PortalTexture.PortalGreen,
            Gravity: (float)cellSize * 2000,
            RadiusScale: (float)cellSize * 2,
            GravityRadiusScale: (float)cellSize * 2
        );
    }

    // Returns the top left corner of the cell square
    private List<Vector2> FindSpotForCell(StageCellSize requestedSize, Vector2I stageSize, List<(Vector2I takenStart, StageCellSize takenSize)> takenCells)
    {
        var stage = new bool[stageSize.X, stageSize.Y];

        foreach (var cell in takenCells)
        {
            for (int x = cell.takenStart.X; x < cell.takenStart.X + (int)cell.takenSize; x++)
            {
                for (int y = cell.takenStart.Y; y < cell.takenStart.Y + (int)cell.takenSize; y++)
                {
                    stage[x, y] = true;
                }
            }
        }

        var possibleSpots = new List<Vector2>();

        for (int x = 0; x < stageSize.X - (int)requestedSize; x++)
        {
            for (int y = 0; y < stageSize.Y - (int)requestedSize; y++)
            {
                bool available = true;

                for (int i = x; i < x + (int)requestedSize; i++)
                {
                    for (int j = y; j < y + (int)requestedSize; j++)
                    {
                        if (stage[i, j])
                        {
                            available = false;
                            break;
                        }
                    }

                    if (!available)
                    {
                        break;
                    }
                }

                if (available)
                {
                    possibleSpots.Add(new Vector2(x, y));
                }
            }
        }

        return possibleSpots;
    }
}