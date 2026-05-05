using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Dono.MiningGame.Game
{
    public class ActorsManager : MonoBehaviour
    {

        public List<Actor> Actors { get; private set; }
        public GameObject Player { get; set; }

        public void SetPlayer(GameObject player) => Player = player;

        private void Awake()
        {
            Actors = new List<Actor>();
        }
    }

}
