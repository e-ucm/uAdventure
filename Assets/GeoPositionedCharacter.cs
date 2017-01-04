using UnityEngine;
using System.Collections;
using MapzenGo.Models;
using UnityStandardAssets.Characters.ThirdPerson;
using MapzenGo.Helpers;

public class GeoPositionedCharacter : MonoBehaviour {

    public TileManager tileManager;
    public ThirdPersonCharacter thirdPersonCharacter;

    public void MoveTo(Vector2d latLon)
    {
        destination = latLon;
    }

    public void InstantMoveTo(Vector2d latLon)
    {
        destination = latLon;
        LatLon = latLon;
    }

    public Vector2d LatLon { get; set; }

    private Vector2d destination;

    void Update()
    {
        var tileManagerRelative = GM.LatLonToMeters(tileManager.Latitude, tileManager.Longitude);
        var latLonMeters = GM.LatLonToMeters(LatLon) - tileManagerRelative;
        var destinationMeters = GM.LatLonToMeters(destination) - tileManagerRelative;
        destinationMeters -= latLonMeters;

        thirdPersonCharacter.Move(new Vector3((float)destinationMeters.y, 0, (float)destinationMeters.x), false, false);

        this.LatLon = GM.MetersToLatLon(transform.localPosition.ToVector2xz().ToVector2d() + tileManagerRelative);
        Debug.Log("Character at: " + this.LatLon +" moving to " + destination);
    }
}
