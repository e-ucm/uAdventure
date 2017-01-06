using UnityEngine;
using System.Collections;
using MapzenGo.Models;
using UnityStandardAssets.Characters.ThirdPerson;
using MapzenGo.Helpers;

public class GeoPositionedCharacter : MonoBehaviour
{

    public TileManager tileManager;
    public ThirdPersonCharacter thirdPersonCharacter;
    public float minDistanceToWalk = 10; // 2 meters

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
        var latLonMeters = GM.LatLonToMeters(LatLon.y, LatLon.x) - tileManagerRelative;
        var destinationMeters = GM.LatLonToMeters(destination.y, destination.x) - tileManagerRelative;
        destinationMeters -= latLonMeters;

        if (destinationMeters.sqrMagnitude >= minDistanceToWalk * minDistanceToWalk)
            thirdPersonCharacter.Move(Vector3.ClampMagnitude(new Vector3((float)destinationMeters.x, 0, (float)destinationMeters.y), minDistanceToWalk*3) / (minDistanceToWalk*3), false, false);
        else
        {
            thirdPersonCharacter.Move(new Vector3(0, 0, 0), false, false);
            transform.localRotation = Quaternion.Euler(0, 0, Input.compass.trueHeading);
        }


        /* if (destinationMeters.sqrMagnitude >= minDistanceToWalk * minDistanceToWalk)
         {
             Debug.Log("Moving because: " + destinationMeters.magnitude);
         }
         else
         {
             thirdPersonCharacter.Move(new Vector3((float)latLonMeters.y, 0, (float)latLonMeters.x), false, false);
             transform.localRotation = Quaternion.Euler(0, 0, Input.compass.trueHeading);
         }*/


        this.LatLon = GM.MetersToLatLon(transform.localPosition.ToVector2xz().ToVector2d() + tileManagerRelative);
        //Debug.Log("Character at: " + this.LatLon +" moving to " + destination);
    }

    public bool IsLookingTo(Vector2d point)
    {
        return IsLookingTowards((point - LatLon).normalized);
    }

    public bool IsLookingTowards(Vector2d direction)
    {
        return Vector3.Angle(direction.ToVector3().normalized, Quaternion.Euler(0, 0, Input.compass.trueHeading) * Vector3.forward) < 15; // 15 degree tolerance
    }
}
    