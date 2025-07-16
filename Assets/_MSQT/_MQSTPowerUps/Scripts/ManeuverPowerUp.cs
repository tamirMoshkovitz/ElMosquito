using System;
using _MSQT.Player.Scripts;
using _MSQT.Player.Scripts.MosquitoBehaviors;
using UnityEngine;

namespace _MSQT._MQSTPowerUps.Scripts
{
    public class ManeuverPowerUp: _MSQTPowerUps
    {
        private void OnTriggerEnter(Collider other)
        {
            PlayerControler player = other.gameObject.GetComponent<PlayerControler>();
            player.MosquiroBehaviour = new Player.Scripts.MosquitoBehaviors.ManeuverPowerUp(player.MosquiroBehaviour);
            Debug.Log("ManeuverPowerUp");
        }
    }
}