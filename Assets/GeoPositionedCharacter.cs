using UnityEngine;
using System.Collections;
using MapzenGo.Models;
using UnityStandardAssets.Characters.ThirdPerson;
using MapzenGo.Helpers;

public class GeoPositionedCharacter : MonoBehaviour {

    public TileManager tileManager;
    public ThirdPersonCharacter thirdPersonCharacter;
    public float minDistanceToWalk = 2; // 2 meters

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

    void Start()
    {
        Input.compass.enabled = true;
    }

    void Update()
    {
        var tileManagerRelative = GM.LatLonToMeters(tileManager.Latitude, tileManager.Longitude);
        var latLonMeters = GM.LatLonToMeters(LatLon) - tileManagerRelative;
        var destinationMeters = GM.LatLonToMeters(destination) - tileManagerRelative;
        destinationMeters -= latLonMeters;

        if(destinationMeters.sqrMagnitude <= minDistanceToWalk * minDistanceToWalk)
        {
            thirdPersonCharacter.Move(new Vector3((float)destinationMeters.y, 0, (float)destinationMeters.x), false, false);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, Input.compass.trueHeading);
        }

        this.LatLon = GM.MetersToLatLon(transform.localPosition.ToVector2xz().ToVector2d() + tileManagerRelative);
        //Debug.Log("Character at: " + this.LatLon +" moving to " + destination);
    }

    public bool IsLookingTo(Vector2d point)
    {
        return IsLookingTowards((point - LatLon).normalized);
    }

    public bool IsLookingTowards(Vector2d direction)
    {
        return Vector3.Angle(direction.ToVector3().normalized, Quaternion.Euler(0,0,Input.compass.trueHeading) * Vector3.forward) < 15; // 15 degree tolerance
    }
}
