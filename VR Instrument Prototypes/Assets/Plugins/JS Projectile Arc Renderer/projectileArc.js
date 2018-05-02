#pragma strict

var points : Vector3[] = new Vector3[100];
var draw = true;
var lineRenderer : LineRenderer;
var count = 0;
var followPoints : Vector3[];
private var follow = false;
var currentWaypoint = 1;
var speed : float = .05;
private var currentSpeed : float;
var timeBetweenPoints = .2;

function Start () {
if(GetComponent.<Renderer>().material.HasProperty("_Color"))
GetComponent.<Renderer>().material.color = Color.cyan;
     currentSpeed = speed;

     lineRenderer = gameObject.AddComponent(LineRenderer);
     lineRenderer.material = new Material (Shader.Find("Particles/Additive"));
     lineRenderer.SetColors(Color.yellow,Color.green);
     lineRenderer.SetWidth(0.2,0.2);

while(draw && count < points.Length && GetComponent(MeshRenderer).enabled){
points[count] = transform.position;
count++;
yield WaitForSeconds(timeBetweenPoints);
}
}

function Update () {
lineRenderer.SetVertexCount(count);
    for(var i : int = 0; i < count; i++) {
        lineRenderer.SetPosition(i, points[i]);
    }
    if(follow){
    if(currentWaypoint < followPoints.Length){
    transform.position = Vector3.Lerp(followPoints[currentWaypoint-1],followPoints[currentWaypoint],currentSpeed);
    currentSpeed += speed;
    if(transform.position == followPoints[currentWaypoint]){
    currentWaypoint++;
    currentSpeed = 0;
    }
    }
    else{
    Debug.Log("Boom!!!!");
    transform.position = followPoints[followPoints.Length-1];
    }
    }
}

function OnCollisionEnter (other : Collision){
draw = false;
points[count] = other.contacts[0].point;
GetComponent(MeshRenderer).enabled = false;
}

function FollowPoints(points : Vector3[],length : int){
followPoints = new Vector3[length];
for(var i = 0; i < length; i++){
followPoints[i] = points[i];
}
follow = true;
}