using System.Collections.Generic;

namespace WordFudge.ScoreSystem
{
    public class TileMatrixFactory
    {
        private readonly HashSet<WorldTile> globalVisitedThisCalculationTiles;

        public TileMatrixFactory(HashSet<WorldTile> globalVisitedThisCalculationTiles)
        {
            this.globalVisitedThisCalculationTiles = globalVisitedThisCalculationTiles;
        }

        public List<TileMatrix> CreateMatrices(WorldTile tile)
        {
            globalVisitedThisCalculationTiles.Add(tile);

            List<TileMatrix> matrices = new List<TileMatrix>();

            foreach(WordContainer word in tile.HorizontalWords)
            {
                matrices.Add(new TileMatrix(word, globalVisitedThisCalculationTiles));
            }

            foreach (WordContainer word in tile.VerticalWords)
            {
                matrices.Add(new TileMatrix(word, globalVisitedThisCalculationTiles));
            }

            return matrices;
        }
    }
}
