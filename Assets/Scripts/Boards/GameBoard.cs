using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.Boards
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField]
        private SnapGrid grid;

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        internal void PickupTile(WorldTile tile)
        {
            throw new NotImplementedException();
        }

        internal void DropTile(WorldTile tile)
        {
            throw new NotImplementedException();
        }
    }
}
