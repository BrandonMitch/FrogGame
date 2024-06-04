using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Reference", menuName = "Variable/Player Reference")]
public class PlayerReferenceSO : ScriptableObject
{
    private List<Player> players;

    public void RegisterPlayer(Player player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }
    public void DeregisterPlayer(Player player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }

    public bool HasPlayer()
    {
        if (players.Count >= 1)
        {
            return true;
        }
        return false;
    }

    public Player GetPlayer()
    {
        if (HasPlayer())
        {
            return players[0];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the closet player that is active
    /// </summary>
    /// <param name="location"> point you are searching from</param>
    /// <returns>the closest player</returns>
    public Player GetPlayer(Vector2 location)
    {
        Player closetPlayer = null;
        float smallestDist = float.MaxValue;
        foreach (Player p in players)
        {
            var d = Vector2.Distance(p.transform.position, location);
            if (d < smallestDist)
            {
                smallestDist = d;
                closetPlayer = p;
            }
        }
        return closetPlayer;
    }
}
